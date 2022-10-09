using Amazon;
using Amazon.S3;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AWS.Helper.AssumeRole.Tests
{
    public class AssumeRoleHelperTests : IClassFixture<AWSArtifactsFixture>
    {
        private readonly AWSArtifactsFixture _fixture;
        private Mock<IDateTimeProvider> dateTimeProviderMock;

        public AssumeRoleHelperTests(AWSArtifactsFixture fixture)
        {
            _fixture = fixture;
            dateTimeProviderMock = new Mock<IDateTimeProvider>();

            dateTimeProviderMock.Setup(s => s.UtcNow).Returns(DateTimeOffset.UtcNow);
        }

        [Fact]
        public async Task Action_Fails_WhenUserDoesntAssumeRole()
        {
            var s3Client = new AmazonS3Client(
                _fixture.Artifacts.UserCredentials, 
                RegionEndpoint.USEast1);

            await Assert.ThrowsAsync<AmazonS3Exception>(async () => await s3Client.ListBucketsAsync());
        }

        [Fact]
        public async Task Action_Success_WhenUserAssumeRole()
        {
            var helper = new AssumeRoleHelper(_fixture.Artifacts.UserCredentials, _fixture.Artifacts.Role.Arn, dateTimeProviderMock.Object);

            var assumedRoleCredentials = helper.GetAssumedRoleCredentials();

            var s3Client = new AmazonS3Client(assumedRoleCredentials, RegionEndpoint.USEast1);

            var exception = await Record.ExceptionAsync(async () => await s3Client.ListBucketsAsync());

            Assert.Null(exception);
        }

        [Fact]
        public void Action_RefreshCredentials_WhenFirstIsExpired()
        {
            var helper = new AssumeRoleHelper(_fixture.Artifacts.UserCredentials, _fixture.Artifacts.Role.Arn, dateTimeProviderMock.Object);

            var firstCredentials = helper.GetAssumedRoleCredentials();

            dateTimeProviderMock
                .Setup(s => s.UtcNow)
                .Returns(DateTimeOffset.UtcNow.AddHours(30));

            var secondCredentials = helper.GetAssumedRoleCredentials();

            Assert.NotEqual(firstCredentials.GetCredentials().AccessKey, secondCredentials.GetCredentials().AccessKey);
        }

        [Fact]
        public void Action_ReuseCredentials_WhenFirstIsNotExpired()
        {
            var helper = new AssumeRoleHelper(_fixture.Artifacts.UserCredentials, _fixture.Artifacts.Role.Arn, dateTimeProviderMock.Object);

            var firstCredentials = helper.GetAssumedRoleCredentials();

            dateTimeProviderMock
                .Setup(s => s.UtcNow)
                .Returns(DateTimeOffset.UtcNow.AddMinutes(3));

            var secondCredentials = helper.GetAssumedRoleCredentials();

            Assert.Equal(firstCredentials.GetCredentials().AccessKey, secondCredentials.GetCredentials().AccessKey);
        }
    }
}
