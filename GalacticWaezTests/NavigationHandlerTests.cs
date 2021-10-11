using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    public class NavigationHandlerTests
    {
        private static readonly IPlayerInfo testPlayer = new Fakes.NavTestPlayerInfo(
            1337, 418, new VectorInt3(13400000, 500000, 12600000), 30);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NavNull()
        {
            new NavigationHandler(null);
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NullCommand()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new NavigationHandler(nav).HandleCommand(null, null, testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_ReturnFalse_NotTo()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsFalse(new NavigationHandler(nav).HandleCommand("hello world", null, testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_InvalidRange()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "--range=hi foo", testPlayer, rsp));
            Assert.AreEqual("Invalid range argument.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_NegativeRange()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "--range=-5 hello", testPlayer, rsp));
            Assert.AreEqual("Range must be a positive number.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_ZeroRange()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "--range=0 hello", testPlayer, rsp));
            Assert.AreEqual("Range must be a positive number.", rsp.Messages[0]);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_Navigate0()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "hello world", testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
            Assert.AreEqual(testPlayer.WarpRange, nav.Range);
            Assert.AreEqual(testPlayer, nav.Player);
            Assert.AreEqual("hello world", nav.Goal);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_Navigate1_EmptyGoalName()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "", testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
            Assert.AreEqual(testPlayer.WarpRange, nav.Range);
            Assert.AreEqual(testPlayer, nav.Player);
            Assert.AreEqual("", nav.Goal);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_Navigate2()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "--range=42", testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
            Assert.AreEqual(42 * GalacticWaez.GalacticWaez.SectorsPerLY, nav.Range);
            Assert.AreEqual(testPlayer, nav.Player);
            Assert.AreEqual("", nav.Goal);
        }

        [TestMethod]
        public void HandleCommand_ReturnTrue_Navigate3()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", "--range=42 funky munky", testPlayer, rsp));
            Assert.AreEqual(0, rsp.Messages.Count);
            Assert.AreEqual(42 * GalacticWaez.GalacticWaez.SectorsPerLY, nav.Range);
            Assert.AreEqual(testPlayer, nav.Player);
            Assert.AreEqual("funky munky", nav.Goal);
        }

        [TestMethod]
        public void HandleCommand_SearchEmptyString_NullArgs()
        {
            var nav = new Fakes.FakeNavigator();
            var rsp = new Fakes.TestResponder();
            Assert.IsTrue(new NavigationHandler(nav)
                .HandleCommand("to", null, testPlayer, rsp));
            Assert.AreEqual(testPlayer.WarpRange, nav.Range);
            Assert.AreEqual(testPlayer, nav.Player);
            Assert.AreEqual("", nav.Goal);
        }
    }
}
