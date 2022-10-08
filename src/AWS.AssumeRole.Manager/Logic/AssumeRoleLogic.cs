using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole
{
    internal sealed class AssumeRoleLogic
    {
        public static async Task<Credentials> AssumeRoleAsync(
            AWSCredentials userCredentials, string roleARN, string sessionName, int durationSeconds)
        {
            var stsClient = new AmazonSecurityTokenServiceClient(userCredentials);

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
