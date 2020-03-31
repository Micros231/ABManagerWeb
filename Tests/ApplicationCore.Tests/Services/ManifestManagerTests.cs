using NUnit.Framework;
using NSubstitute;
using ABManagerWeb.ApplicationCore.Services;
using ABManagerWeb.ApplicationCore.Interfaces;
using System.Threading.Tasks;
using ABManagerWeb.ApplicationCore.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ABManagerWeb.ApplicationCore.Tests.Services
{
    [TestFixture]
    class ManifestManagerTests
    {
        private ManifestManager _manager;
        private List<ManifestInfo> _fakeManifestInfoContainer;
        private Func<Stream, Task> _fakeAddHandler;
        [SetUp]
        protected void SetUp()
        {
            var repositoryFake = Substitute.For<IManifestInfoRepository>();
            var loggerFake = Substitute.For <ILogger<ManifestManager>>();
            _manager = new ManifestManager(repositoryFake, loggerFake);
            _fakeManifestInfoContainer = new List<ManifestInfo>();
            _fakeAddHandler = Substitute.For<Func<Stream, Task>>();

            repositoryFake.AddAsync(Arg.Any<ManifestInfo>()).Returns(callInfo =>
            {
                var manifestInfo = (ManifestInfo)callInfo[0];
                if (manifestInfo != null)
                {
                    _fakeManifestInfoContainer.Add(manifestInfo);
                }
                return manifestInfo;
            });
            repositoryFake.DeleteAsync(Arg.Any<ManifestInfo>()).Returns(callInfo =>
            {
                var manifestInfo = (ManifestInfo)callInfo[0];
                if (manifestInfo != null)
                {
                    _fakeManifestInfoContainer.Remove(manifestInfo);
                }
                return Task.CompletedTask;
            });
            repositoryFake.GetByIdAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                string id = (string)callInfo[0];
                if (string.IsNullOrEmpty(id))
                {
                    return Task.FromResult<ManifestInfo>(null);
                }
                var manifest = _fakeManifestInfoContainer.FirstOrDefault(manifestInfo => manifestInfo.Id == id);
                return Task.FromResult<ManifestInfo>(manifest);
            });
            repositoryFake.GetByPathAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                string path = (string)callInfo[0];
                if (string.IsNullOrEmpty(path))
                {
                    return Task.FromResult<ManifestInfo>(null);
                }
                var manifest = _fakeManifestInfoContainer.FirstOrDefault(manifestInfo => manifestInfo.Path == path);
                return Task.FromResult<ManifestInfo>(manifest);
            });
            repositoryFake.GetByVersionAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                string version = (string)callInfo[0];
                if (string.IsNullOrEmpty(version))
                {
                    return Task.FromResult<ManifestInfo>(null);
                }
                var manifest = _fakeManifestInfoContainer.FirstOrDefault(manifestInfo => manifestInfo.Version == version);
                return Task.FromResult<ManifestInfo>(manifest);
            });
            repositoryFake.GetCurrentManifestAsync().Returns(callInfo =>
            {
                return _fakeManifestInfoContainer.LastOrDefault();
            });
        }

        [Test]
        public async Task AddManifestInfo_AddManifestByVersion1_IsNotNull()
        {
            //arrange
            string version1 = "version1";

            //act
            var manifestInfo = await _manager.AddManifestAsync(version1, _fakeAddHandler);

            //assert
            Assert.IsNotNull(manifestInfo);
        }
        [Test]
        public async Task AddManifestInfo_AddManifestByVersion1_Version1Returned()
        {
            //arrage
            string version1 = "version1";

            //act
            var manifestInfo = await _manager.AddManifestAsync(version1, _fakeAddHandler);

            //assert
            Assert.IsNotNull(manifestInfo);
            Assert.AreEqual(manifestInfo.Version, version1);
        }
        [Test]
        public async Task AddManifestInfo_AddManifestByVersion1_RelativePathReturned()
        {
            //arrage
            string version1 = "version1";
            string relativePath = _manager.GetRelativePathByVersion(version1);

            //act
            var manifestInfo = await _manager.AddManifestAsync(version1, _fakeAddHandler);

            //assert
            Assert.IsNotNull(manifestInfo);
            Assert.AreEqual(manifestInfo.Path, relativePath);
        }
        [Test]
        public async Task GetCurrentManifest_Add2ManifestAndGetCurrent_LastManifest()
        {

            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler); 

            //act
            var currentManifestInfo = await _manager.GetCurrentManifestInfoAsync();

            //assert
            Assert.IsNotNull(currentManifestInfo);
            Assert.AreEqual(currentManifestInfo, manifestInfo2);
            Assert.AreNotEqual(currentManifestInfo, manifestInfo1);
        }
        [Test]
        public async Task GetManifestByVersion_Add2ManifestAndGetFirstByVersion_FirstManifest()
        {
            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler);

            //act
            var currentManifestInfo = await _manager.GetManifestInfoByVersionAsync(manifestInfo1.Version);

            //assert
            Assert.IsNotNull(currentManifestInfo);
            Assert.AreEqual(currentManifestInfo, manifestInfo1);
            Assert.AreNotEqual(currentManifestInfo, manifestInfo2);
        }
        [Test]
        public async Task GetManifestByPath_Add2ManifestAndGetFirstByPath_FirstManifest()
        {
            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler);

            //act
            var currentManifestInfo = await _manager.GetManifestInfoByPathAsync(manifestInfo1.Path);

            //assert
            Assert.IsNotNull(currentManifestInfo);
            Assert.AreEqual(currentManifestInfo, manifestInfo1);
            Assert.AreNotEqual(currentManifestInfo, manifestInfo2);
        }
        [Test]
        public async Task RemoveManifestByInfo_Add2ManifestAndRemoveFirst_IsFalseContainsFirstInfoAndIsTrueContainsSecondInfo()
        {
            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler);

            //act
            await _manager.RemoveManifestAsync(manifestInfo1);

            //assert
            Assert.IsFalse(_fakeManifestInfoContainer.Contains(manifestInfo1));
            Assert.IsTrue(_fakeManifestInfoContainer.Contains(manifestInfo2));
                
        }
        [Test]
        public async Task RemoveManifestByVersion_Add2ManifestAndRemoveFirstVersion_IsFalseContainsFirstInfoAndIsTrueContainsSecondInfo()
        {
            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler);

            //act
            await _manager.RemoveManifestToVersionAsync(manifestInfo1.Version);

            //assert
            Assert.IsFalse(_fakeManifestInfoContainer.Contains(manifestInfo1));
            Assert.IsTrue(_fakeManifestInfoContainer.Contains(manifestInfo2));
        }
        [Test]
        public async Task RemoveManifestByPath_Add2ManifestAndRemoveFirstPath_IsFalseContainsFirstInfoAndIsTrueContainsSecondInfo()
        {
            //arrage
            string version1 = "version1";
            string version2 = "version2";
            var manifestInfo1 = await _manager.AddManifestAsync(version1, _fakeAddHandler);
            var manifestInfo2 = await _manager.AddManifestAsync(version2, _fakeAddHandler);

            //act
            await _manager.RemoveManifestToPathAsync(manifestInfo1.Path);

            //assert
            Assert.IsFalse(_fakeManifestInfoContainer.Contains(manifestInfo1));
            Assert.IsTrue(_fakeManifestInfoContainer.Contains(manifestInfo2));
        }
    }
}
