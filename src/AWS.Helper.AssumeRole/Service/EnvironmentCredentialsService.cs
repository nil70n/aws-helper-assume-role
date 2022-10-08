using Amazon.Runtime;
using System;

namespace AWS.Helper.AssumeRole
{
    internal static class EnvironmentCredentialsService
    {
        private const string ACCESS_KEY_VAR_NAME = "AWS_ACCESS_KEY_ID";
        private const string SECRET_ACCESS_KEY_VAR_NAME = "AWS_SECRET_ACCESS_KEY";

        private static bool HasEnvironmentKeys (string accessKey, string secretAccessKey) => 
            !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretAccessKey);

        public static AWSCredentials GetEnvironmentCredentials()
        {
            string accessKey = Environment.GetEnvironmentVariable(ACCESS_KEY_VAR_NAME);
            string secretAccessKey = Environment.GetEnvironmentVariable(SECRET_ACCESS_KEY_VAR_NAME);

            return HasEnvironmentKeys(accessKey, secretAccessKey)
                ? new BasicAWSCredentials(accessKey, secretAccessKey)
                : FallbackCredentialsFactory.GetCredentials();
        }
    }
}
