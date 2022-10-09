using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole.Tests
{
    public sealed class AWSArtifacts
    {
        public User User { get; set; }
        public AccessKey AccessKey { get; set; }
        public AWSCredentials UserCredentials { get; set; }
        public Role Role { get; set; }
        public ManagedPolicy ManagedPolicy { get; set; }
    }

    public sealed class AWSArtifactsFixture : IDisposable
    {
        private const string _userName = "test-user";
        private const string _roleName = "assume-role";
        private const string _s3PolicyName = "s3-list-buckets-policy";


        private const int INTERVAL_SECONDS = 15;
        private const string POLICY_DOCUMENT =
            "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:ListAllMyBuckets\"],\"Effect\":\"Allow\",\"Resource\":\"*\"}]}";
        private string GetAssumeRolePolicyDocument(string userArn) =>
            "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{" +
            $"\"AWS\":\"{userArn}\"" +
            "},\"Action\":\"sts:AssumeRole\"}]}";

        private readonly TestService _testService;

        public AWSArtifactsFixture()
        {
            _testService = new TestService();
            Artifacts = new AWSArtifacts();

            Task.WaitAll(Initialize());
        }

        private async Task Initialize()
        {
            Artifacts.User = await _testService.CreateUserAsync(_userName);
            Artifacts.AccessKey = await _testService.CreateAccessKeyAsync(_userName);

            // Waiting for the user be available
            Thread.Sleep(INTERVAL_SECONDS * 1000);

            Artifacts.Role = await _testService.CreateRoleAsync(_roleName, GetAssumeRolePolicyDocument(Artifacts.User.Arn));
            Artifacts.ManagedPolicy = await _testService.CreatePolicyAsync(_s3PolicyName, POLICY_DOCUMENT);

            // Waiting for the policy be available
            Thread.Sleep(INTERVAL_SECONDS * 1000);

            await _testService.AttachRoleAsync(Artifacts.ManagedPolicy.Arn, _roleName);

            // Waiting for the policy be attached
            Thread.Sleep(INTERVAL_SECONDS * 1000);

            Artifacts.UserCredentials = new BasicAWSCredentials(Artifacts.AccessKey.AccessKeyId, Artifacts.AccessKey.SecretAccessKey);
        }

        public AWSArtifacts Artifacts { get; private set; }

        public void Dispose()
        {
            Task.WaitAll(
                _testService.DeleteResourcesAsync(
                    Artifacts.AccessKey.AccessKeyId, _userName, Artifacts.ManagedPolicy.Arn, _roleName));
        }
    }
}
