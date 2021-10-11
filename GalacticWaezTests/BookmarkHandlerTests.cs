using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;
using static GalacticWaezTests.GalaxyTestData;

namespace GalacticWaezTests
{
    [TestClass]
    public class BookmarkHandlerTests
    {
        static readonly IPlayerInfo player = new Fakes.NavTestPlayerInfo(1337, 1337, default, 30);

        [TestMethod]
        public void HandleCommand_ReturnFalse_NullCmdToken()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand(null, "", player, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_EmptyCmdToken()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("", "", player, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NonBookmarkCommand()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("", "", player, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_LoremIpsumCommand()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand(LoremIpsum, "", player, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_Bookmarks_NullArgs()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", null, player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Required:"));
        }

        [TestMethod]
        public void HandleCommand_Bookmarks_EmptyArgs()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", "", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Required:"));
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NotMine1()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("hello", "", player, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_SuccessResponse_Show()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("bookmarks", "show", player, rsp));
            Assert.AreEqual("Modified 3 map markers.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_SuccessResponse_Hide()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("bookmarks", "hide", player, rsp));
            Assert.AreEqual("Modified 3 map markers.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_SuccessResponse_Clear()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("bookmarks", "clear", player, rsp));
            Assert.AreEqual("Modified 3 map markers.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_SuccessResponse_ShorthandClear_NullArgs()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("clear", null, player, rsp));
            Assert.AreEqual("Modified 3 map markers.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_SuccessResponse_ShorthandClear_EmptyArgs()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("clear", "", player, rsp));
            Assert.AreEqual("Modified 3 map markers.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_FailResponse_ShorthandClearWithOptions()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.HappyBookmarkManager(default), delegate { })
                .HandleCommand("clear", "moo", player, rsp));
            Assert.AreEqual("'clear' does not take options.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_FailResponse_UnrecognizedOption()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", "clr", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Unrecognized option: clr"));
        }

        [TestMethod]
        public void HandleCommand_FailResponse_UnrecognizedShow()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", "show foo", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Unrecognized option:"));
        }

        [TestMethod]
        public void HandleCommand_FailResponse_UnrecognizedHide()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", "hide foo", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Unrecognized option:"));
        }

        [TestMethod]
        public void HandleCommand_FailResponse_UnrecognizedClear()
        {
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new BookmarkHandler(new Fakes.FakeBookmarkManager(), delegate { })
                .HandleCommand("bookmarks", "clear foo", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith("Unrecognized option:"));
        }
    }
}
