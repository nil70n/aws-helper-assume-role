using Amazon;
using Amazon.IdentityManagement.Model;
using Amazon.S3;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AWS.Helper.AssumeRole.Tests
{
    public class AssumeRoleHelperTests : IDisposable
    {
        private const int INTERVAL_SECONDS = 15;

        private const string USER_NAME = "test-user";
        private const string ROLE_NAME = "allow-assume-role";
        private const string S3_POLICY_NAME = "s3-list-buckets-policy";
        private const string POLICY_DOCUMENT = "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:ListAllMyBuckets\"],\"Effect\":\"Allow\",\"Resource\":\"*\"}]}";

        private string GetAssumeRolePolicyDocument (string userArn) => "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{" +
            $"\"AWS\":\"{ userArn }\""+
            "},\"Action\":\"sts:AssumeRole\"}]}";

        private readonly TestService _testService;

        private User _user;
        private AccessKey _accessKey;
        private Role _role;
        private ManagedPolicy _policy;

        public AssumeRoleHelperTests()
        {
            _testService = new TestService();

            Task.WaitAll(SetUpAsync());
        }

        private async Task SetUpAsync()
        {
            _user = await _testService.CreateUserAsync(USER_NAME);
            _accessKey = await _testService.CreateAccessKeyAsync(USER_NAME);

            // Waiting for the user be available
            Thread.Sleep(INTERVAL_SECONDS * 1000);

            _role = await _testService.CreateRoleAsync(ROLE_NAME, GetAssumeRolePolicyDocument(_user.Arn));
            _policy = await _testService.CreatePolicyAsync(S3_POLICY_NAME, POLICY_DOCUMENT);

            // Waiting for the policy be available
            Thread.Sleep(INTERVAL_SECONDS * 1000);

            await _testService.AttachRoleAsync(_policy.Arn, ROLE_NAME);

            // Waiting for the policy be attached
            Thread.Sleep(INTERVAL_SECONDS * 1000);
        }

        [Fact]
        public async Task Action_ThrowsError_WhenUserDoesnAssumeRole()
        {
            var s3Client = new AmazonS3Client(_accessKey.AccessKeyId, _accessKey.SecretAccessKey, RegionEndpoint.USEast1);

            await Assert.ThrowsAsync<AmazonS3Exception>(async () => await s3Client.ListBucketsAsync());
        }

        public void Dispose()
        {
            Task.WaitAll(
                _testService.DeleteResourcesAsync(_accessKey.AccessKeyId, USER_NAME, _policy.Arn, ROLE_NAME)); 
        }
    }
}
