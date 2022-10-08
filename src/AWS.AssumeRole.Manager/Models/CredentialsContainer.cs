using Amazon.Runtime;
using System;

namespace AWS.Helper.AssumeRole
{
    internal sealed class CredentialsContainer
    {
        public CredentialsContainer(
            AWSCredentials credentials,
            DateTimeOffset createdAt)
        {
            Credentials = credentials;
            CreatedAt = createdAt;
        }

        public AWSCredentials Credentials { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
