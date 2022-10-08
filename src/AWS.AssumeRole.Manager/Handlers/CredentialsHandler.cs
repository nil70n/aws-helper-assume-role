using Amazon.Runtime;
using System;

namespace AWS.Helper.AssumeRole
{
    internal sealed class CredentialsHandler
    {
        private IDateTimeProvider _dateTimeProvider;
        private readonly int _credentialsDurationSeconds;

        private CredentialsContainer? _credentialsContainer;
        private readonly object _lock = new object();

        private readonly Func<AWSCredentials> GetAwsCredentials;

        public CredentialsHandler(
            Func<AWSCredentials> getAwsCredentials,
            int credentialsDurationSeconds,
            IDateTimeProvider dateTimeProvider)
        {
            GetAwsCredentials = getAwsCredentials;
            _dateTimeProvider = dateTimeProvider;
            _credentialsDurationSeconds = credentialsDurationSeconds;
        }

        private bool MustRefreshCredentials () =>
            _credentialsContainer is null
                || (_dateTimeProvider.UtcNow - _credentialsContainer.CreatedAt).TotalSeconds >= (_credentialsDurationSeconds - 30);

        public AWSCredentials GetCredentials()
        {
            if (MustRefreshCredentials())
            {
                lock (_lock)
                {
                    if (MustRefreshCredentials())
                    {
                        var credentials = GetAwsCredentials();
                        _credentialsContainer = new CredentialsContainer(credentials, _dateTimeProvider.UtcNow);
                    }
                }
            }

            return _credentialsContainer!.Credentials;
        }
    }
}
