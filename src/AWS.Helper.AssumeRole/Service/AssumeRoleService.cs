using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole
{
    internal sealed class AssumeRoleService
    {
        public static async Task<Credentials> AssumeRoleAsync(
            AmazonSecurityTokenServiceClient stsClient, string roleARN, string sessionName, int durationSeconds)
        {
            var request = new AssumeRoleRequest
            {
                DurationSeconds = durationSeconds,
                RoleArn = roleARN,
                RoleSessionName = sessionName
            };

            var response = await stsClient.AssumeRoleAsync(request);

            return response.Credentials;
        }
    }
}
