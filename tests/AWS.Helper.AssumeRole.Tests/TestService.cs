using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole.Tests
{
    internal sealed class TestService
    {
        private readonly AmazonIdentityManagementServiceClient _client;

        public TestService()
        {
            var environmentCredentials = new EnvironmentVariablesAWSCredentials().GetCredentials();
            _client = new AmazonIdentityManagementServiceClient(environmentCredentials.AccessKey, environmentCredentials.SecretKey);
        }

        #region Creation of required objects for testing
        public async Task<User> CreateUserAsync(string userName)
        {
            var response = await _client.CreateUserAsync(new CreateUserRequest
            {
                UserName = userName
            });

            return response.User;
        }

        public async Task<AccessKey> CreateAccessKeyAsync(string userName)
        {
            var response = await _client.CreateAccessKeyAsync(new CreateAccessKeyRequest
            {
                UserName = userName
            });

            return response.AccessKey;
        }

        public async Task<ManagedPolicy> CreatePolicyAsync(string policyName, string policyDocument)
        {
            var response = await _client.CreatePolicyAsync(new CreatePolicyRequest
            {
                PolicyName = policyName,
                PolicyDocument = policyDocument
            });

            return response.Policy;
        }

        public async Task<Role> CreateRoleAsync(string roleName, string rolePermissionsDocument)
        {
            var response = await _client.CreateRoleAsync(new CreateRoleRequest
            {
                RoleName = roleName,
                AssumeRolePolicyDocument = rolePermissionsDocument
            });

            return response.Role;
        }

        public async Task AttachRoleAsync(string policyArn, string roleName)
        {
            await _client.AttachRolePolicyAsync(new AttachRolePolicyRequest
            {
                PolicyArn = policyArn,
                RoleName = roleName
            });
        }
        #endregion

        public async Task DeleteResourcesAsync(
            string accessKeyId,
            string userName,
            string policyArn,
            string roleName)
        {
            var detachPolicyResponse = await _client.DetachRolePolicyAsync(new DetachRolePolicyRequest
            {
                PolicyArn = policyArn,
                RoleName = roleName,
            });

            var delPolicyResponse = await _client.DeletePolicyAsync(new DeletePolicyRequest
            {
                PolicyArn = policyArn,
            });

            var delRoleResponse = await _client.DeleteRoleAsync(new DeleteRoleRequest
            {
                RoleName = roleName,
            });

            var delAccessKey = await _client.DeleteAccessKeyAsync(new DeleteAccessKeyRequest
            {
                AccessKeyId = accessKeyId,
                UserName = userName,
            });

            var delUserResponse = await _client.DeleteUserAsync(new DeleteUserRequest
            {
                UserName = userName,
            });
        }
    }
}
