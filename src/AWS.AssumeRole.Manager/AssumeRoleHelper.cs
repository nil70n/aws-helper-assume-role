using Amazon.Runtime;
using System.Threading.Tasks;

namespace AWS.Helper.AssumeRole
{
    public sealed class AssumeRoleHelper
    {
        private const string DEFAULT_SESSION_NAME = "aws-temporary-session";

        private readonly AWSCredentials _userCredentials;
        private readonly CredentialsHandler _credentialsHandler;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AssumeRoleHelper(
            string roleArn, string? sessionName = null, int durationSeconds = 900)
            :this(FallbackCredentialsFactory.GetCredentials(), roleArn, sessionName, durationSeconds) { }

        public AssumeRoleHelper(
            string userAccessKey, string userSecretKey, string roleArn, string? sessionName, int durationSeconds = 900)
            :this(new BasicAWSCredentials(userAccessKey, userSecretKey), roleArn, sessionName, durationSeconds) { }

        public AssumeRoleHelper(
            AWSCredentials userCredentials, string roleArn, IDateTimeProvider dateTimeProvider)
            :this(userCredentials, roleArn, null, 900, dateTimeProvider) { }

        public AssumeRoleHelper(
            AWSCredentials userCredentials, string roleArn, string? sessionName = null, int durationSeconds = 900)
            : this(userCredentials, roleArn, sessionName, durationSeconds, null) { }

        public AssumeRoleHelper(
            AWSCredentials userCredentials, string roleArn, string? sessionName = null, int durationSeconds = 900, IDateTimeProvider? dateTimeProvider = null)
        {
            RoleArn = roleArn;
            SessionName = sessionName ?? DEFAULT_SESSION_NAME;
            DurationSeconds = durationSeconds;
            _userCredentials = userCredentials;
            _dateTimeProvider = dateTimeProvider ?? new DateTimeProvider();
            _credentialsHandler = new CredentialsHandler(AssumeRole, DurationSeconds, _dateTimeProvider);
        }

        private string RoleArn { get; set; }
        private string SessionName { get; set; }
        private int DurationSeconds { get; set; }

        private AWSCredentials AssumeRole()
        {
            var credentialsTask = AssumeRoleLogic.AssumeRoleAsync(_userCredentials, RoleArn, SessionName, DurationSeconds);

            Task.WaitAll(credentialsTask);

            return credentialsTask.Result;
        }

        public AWSCredentials GetAssumedRoleCredentials()
        {
            return _credentialsHandler.GetCredentials();
        }
    }
}
