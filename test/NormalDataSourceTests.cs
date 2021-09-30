using Microsoft.VisualStudio.TestTools.UnitTesting;
using GalacticWaez;
using System.Linq;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class NormalDataSourceTests
    {
        [TestMethod]
        public void ReturnsNull_WhenBothReturnNull()
        {
            Assert.IsNull(new NormalDataSource(
                new Fakes.NullFileDataSource(),
                new Fakes.NullDataSource())
                .GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileNullAndScanEmpty()
        {
            Assert.IsNull(new NormalDataSource(
                new Fakes.NullFileDataSource(),
                new Fakes.EmptyDataSource())
                .GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileEmptyAndScanNull()
        {
            Assert.IsNull(new NormalDataSource(
                new Fakes.EmptyFileDataSource(),
                new Fakes.NullDataSource())
                .GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenBothEmpty()
        {
            Assert.IsNull(new NormalDataSource(
                new Fakes.EmptyFileDataSource(),
                new Fakes.EmptyDataSource())
                .GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsExpected_WhenFileGood()
        {
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var fileSource = new Fakes.FileDataSource("stardata-test-small.csv");
            var actual = new NormalDataSource(fileSource, null).GetGalaxyData();
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void ReturnsExpected_WhenFileEmpty()
        {
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var scanSource = new Fakes.FileDataSource("stardata-test-small.csv");
            var actual = new NormalDataSource(new Fakes.EmptyFileDataSource(), scanSource).GetGalaxyData();
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void ReturnsExpected_WhenFileNull()
        {
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var scanSource = new Fakes.FileDataSource("stardata-test-small.csv");
            var actual = new NormalDataSource(new Fakes.NullFileDataSource(), scanSource).GetGalaxyData();
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void ReturnsNull_WhenSourcesAreNull()
        {
            Assert.IsNull(new NormalDataSource(null, null).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileSourceNull_ScanNull()
        {
            Assert.IsNull(new NormalDataSource(null, new Fakes.NullDataSource()).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileSourceNull_ScanEmpty()
        {
            Assert.IsNull(new NormalDataSource(null, new Fakes.EmptyDataSource()).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsExpected_WhenFileSourceNull()
        {
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var scanSource = new Fakes.FileDataSource("stardata-test-small.csv");
            var actual = new NormalDataSource(null, scanSource).GetGalaxyData();
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileNull_ScanSourceNull()
        {
            Assert.IsNull(new NormalDataSource(new Fakes.NullFileDataSource(), null).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileEmpty_ScanSourceNull()
        {
            Assert.IsNull(new NormalDataSource(new Fakes.EmptyFileDataSource(), null).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsExpected_WhenScanSourceNull()
        {
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var fileSource = new Fakes.FileDataSource("stardata-test-small.csv");
            var actual = new NormalDataSource(fileSource, null).GetGalaxyData();
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }
    }
}
