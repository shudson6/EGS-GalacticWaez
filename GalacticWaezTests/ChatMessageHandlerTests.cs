using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;
using Eleon;

namespace GalacticWaezTests
{
    [TestClass]
    public class ChatMessageHandlerTests
    {
        static readonly Fakes.TestResponder rsp = new Fakes.TestResponder();

        static readonly ICommandHandler happyHandler =
            new FakeCommandHandler((_0, _1, _2, _3) => true);
        static readonly ICommandHandler failHandler =
            new FakeCommandHandler((_0, _1, _2, _3) => false);
        static readonly ICommandHandler cmdHandlerFailOnInvoke =
            new FakeCommandHandler((_0, _1, _2, _3) => throw new AssertFailedException());

        static readonly IResponseManager rspMgr = new Fakes.FakeResponseManager(_ => rsp);
        static readonly IResponseManager rspMgrFailOnInvoke =
            new Fakes.FakeResponseManager(_ => throw new AssertFailedException());

        static readonly IPlayerProvider playerProviderFailOnInvoke =
            new Fakes.FakePlayerProvider(_ => throw new AssertFailedException());
        static readonly IPlayerProvider fakePlayerProvider =
            new Fakes.FakePlayerProvider(_ => new Fakes.NavTestPlayerInfo(1337, 1337, default, 30));

        class FakeCommandHandler : ICommandHandler
        {
            public delegate bool HandlerDelegate(
                string cmd, string args, IPlayerInfo player, IResponder rsp);

            private readonly HandlerDelegate DoStuff;

            public FakeCommandHandler(HandlerDelegate doStuff) => DoStuff = doStuff;

            public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
                => DoStuff(cmdToken, args, player, responder);
        }

        [TestInitialize]
        public void SetupTest()
        {
            rsp.Messages.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contstructor_Throw_NullPlayerProvider()
        {
            new ChatMessageHandler(null, rspMgr, happyHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throw_NullResponseManager()
        {
            new ChatMessageHandler(new Fakes.FakePlayerProvider(null), null, happyHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throw_NullPPandRM()
        {
            new ChatMessageHandler(null, null, happyHandler);
        }

        [TestMethod]
        public void Constructor_NoHandlers_NoProblem()
        {
            new ChatMessageHandler(new Fakes.FakePlayerProvider(null), rspMgr);
        }

        [TestMethod]
        public void HandleChatMessage_Nop_NullMessage()
        {
            var chat = new ChatMessageHandler(playerProviderFailOnInvoke, rspMgrFailOnInvoke, 
                cmdHandlerFailOnInvoke);
            chat.HandleChatMessage(null);
        }

        [TestMethod]
        public void HandleChatMessage_Nop_NullTextMessage()
        {
            var chat = new ChatMessageHandler(playerProviderFailOnInvoke, rspMgrFailOnInvoke, 
                cmdHandlerFailOnInvoke);
            chat.HandleChatMessage(new MessageData { Text = null });
        }

        [TestMethod]
        public void HandleChatMessage_Nop_EmptyTextMessage()
        {
            var chat = new ChatMessageHandler(playerProviderFailOnInvoke, rspMgrFailOnInvoke, 
                cmdHandlerFailOnInvoke);
            chat.HandleChatMessage(new MessageData { Text = "" });
        }

        [TestMethod]
        public void HandleChatMessage_Nop_NotWaezMessage()
        {
            var chat = new ChatMessageHandler(playerProviderFailOnInvoke, rspMgrFailOnInvoke, 
                cmdHandlerFailOnInvoke);
            chat.HandleChatMessage(new MessageData { Text = "hello world" });
        }

        [TestMethod]
        public void HandleChatMessage_CantIdenfityPlayer()
        {
            var chat = new ChatMessageHandler(new Fakes.FakePlayerProvider(_ => null), rspMgr,
                cmdHandlerFailOnInvoke);
            var msg = new MessageData { Text = "/waez hello", SenderEntityId = 42 };
            chat.HandleChatMessage(msg);
            Assert.AreEqual("Can't identify requesting player", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleChatMessage_NoCommand()
        {
            var chat = new ChatMessageHandler(fakePlayerProvider, rspMgr,
                cmdHandlerFailOnInvoke);
            var msg = new MessageData { Text = "/waez", SenderEntityId = 42 };
            chat.HandleChatMessage(msg);
            Assert.IsTrue(rsp.Messages[0].StartsWith("hm?"));
        }

        [TestMethod]
        public void HandleChatMessage_NoCommand_Whitespace()
        {
            var chat = new ChatMessageHandler(fakePlayerProvider, rspMgr,
                cmdHandlerFailOnInvoke);
            var msg = new MessageData { Text = "/waez ", SenderEntityId = 42 };
            chat.HandleChatMessage(msg);
            Assert.IsTrue(rsp.Messages[0].StartsWith("hm?"));
        }
    }
}
