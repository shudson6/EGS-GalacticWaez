using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;
using static GalacticWaezTests.GalaxyTestData;

namespace GalacticWaezTests
{
    [TestClass]
    public class HelpHandlerTests
    {
        private static readonly Fakes.TestResponder rsp = new Fakes.TestResponder();
        private static readonly IPlayerInfo player = new Fakes.FakePlayerInfo();

        [TestInitialize]
        public void SetupTest()
        {
            rsp.Messages.Clear();
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NullCommand()
        {
            Assert.IsFalse(new HelpHandler().HandleCommand(null, "world", player, rsp));
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_EmptyCommand()
        {
            Assert.IsFalse(new HelpHandler().HandleCommand("", "world", player, rsp));
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NonHelpCommand()
        {
            Assert.IsFalse(new HelpHandler().HandleCommand("to", "world", player, rsp));
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_LoremIpsumCommand()
        {
            Assert.IsFalse(new HelpHandler().HandleCommand(LoremIpsum, "world", player, rsp));
        }

        [TestMethod]
        public void HandleCommand_Respond_NullArgs()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", null, player, rsp));
            Assert.AreEqual(HelpHandler.HelpText, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_EmptyArgs()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "", player, rsp));
            Assert.AreEqual(HelpHandler.HelpText, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_BadArgs()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "$#!t", player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith(HelpHandler.HelpText));
        }

        [TestMethod]
        public void HandleCommand_Respond_LoremIpsumArgs()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", LoremIpsum, player, rsp));
            Assert.IsTrue(rsp.Messages[0].StartsWith(HelpHandler.HelpText));
        }
        
        [TestMethod]
        public void HandleCommand_Respond_WhitespaceArgs()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "   \t", player, rsp));
            Assert.AreEqual(HelpHandler.HelpText, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_To()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "to", player, rsp));
            Assert.AreEqual(HelpHandler.ToHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_ToWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "to  ", player, rsp));
            Assert.AreEqual(HelpHandler.ToHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Status()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "status", player, rsp));
            Assert.AreEqual(HelpHandler.StatusHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_StatusWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "status  ", player, rsp));
            Assert.AreEqual(HelpHandler.StatusHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Clear()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "clear", player, rsp));
            Assert.AreEqual(HelpHandler.ClearHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_ClearWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "clear  ", player, rsp));
            Assert.AreEqual(HelpHandler.ClearHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Bookmarks()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "bookmarks", player, rsp));
            Assert.AreEqual(HelpHandler.BookmarksHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_BookmarksWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "bookmarks  ", player, rsp));
            Assert.AreEqual(HelpHandler.BookmarksHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Help()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "help", player, rsp));
            Assert.AreEqual(HelpHandler.HelpHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_HelpWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "help  ", player, rsp));
            Assert.AreEqual(HelpHandler.HelpHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Pinfo()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "pinfo", player, rsp));
            Assert.AreEqual(HelpHandler.PinfoHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_PinfoWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "pinfo  ", player, rsp));
            Assert.AreEqual(HelpHandler.PinfoHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Restart()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "restart", player, rsp));
            Assert.AreEqual(HelpHandler.RestartHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_RestartWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "restart  ", player, rsp));
            Assert.AreEqual(HelpHandler.RestartHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_Store()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "store", player, rsp));
            Assert.AreEqual(HelpHandler.StoreHelp, rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_Respond_StoreWhitespace()
        {
            Assert.IsTrue(new HelpHandler().HandleCommand("help", "store  ", player, rsp));
            Assert.AreEqual(HelpHandler.StoreHelp, rsp.Messages[0]);
        }
    }
}
