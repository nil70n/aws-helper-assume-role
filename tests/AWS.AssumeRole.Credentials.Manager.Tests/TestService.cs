using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole
{
    internal sealed class TestService
    {
        private const string _policyDocument = 
            "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{\"AWS\":\"{0}\"},\"Action\":\"sts:AssumeRole\"}]}";

        private const string _assumeRolePolicyDocument =
            "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:ListAllMyBuckets\"],\"Effect\":\"Allow\",\"Resource\":\"*\"}]}";

        private readonly AmazonIdentityManagementServiceClient _client = new AmazonIdentityManagementServiceClient();

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

        //public static async Task<List<S3Bucket>> ListMyBucketAsync(AmazonS3Client client)
        //{
        //    var response = await client.ListBucketsAsync();

        //    return response.Buckets;
        //}
    }
}
