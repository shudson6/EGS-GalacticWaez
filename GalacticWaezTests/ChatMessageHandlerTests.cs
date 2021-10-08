using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;
using Eleon;

namespace GalacticWaezTests
{
    [TestClass]
    public class ChatMessageHandlerTests
    {
        static readonly ICommandHandler happyHandler =
            new FakeCommandHandler((_0, _1, _2, _3) => true);
        static readonly ICommandHandler failHandler =
            new FakeCommandHandler((_0, _1, _2, _3) => false);
        static readonly ICommandHandler failIfInvoked =
            new FakeCommandHandler((_0, _1, _2, _3) => throw new AssertFailedException());

        static readonly IResponseManager rspMgr = new Fakes.FakeResponseManager(_ => rsp);
        static readonly IResponseManager failRspMgr =
            new Fakes.FakeResponseManager(_ => throw new AssertFailedException());

        static readonly IResponder failOnResponse =
            new Fakes.FakeResponder(_ => throw new AssertFailedException());
        static readonly Fakes.TestResponder rsp = new Fakes.TestResponder();

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
        public void HandleChatMessage_CantIdenfityPlayer()
        {
            var chat = new ChatMessageHandler(new Fakes.FakePlayerProvider(null), rspMgr, happyHandler);
            var msg = new MessageData { Text = "/waez hello", SenderEntityId = 42 };
            chat.HandleChatMessage(msg);
            Assert.AreEqual("Can't identify requesting player", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleChatMessage_Nop_NullMessage()
        {
            var chat = new ChatMessageHandler(new Fakes.FakePlayerProvider(null), failRspMgr, failIfInvoked);
            chat.HandleChatMessage(null);
            Assert.AreEqual(0, rsp.Messages.Count);
        }
    }
}
