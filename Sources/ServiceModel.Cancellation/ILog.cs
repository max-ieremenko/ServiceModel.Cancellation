namespace ServiceModel.Cancellation
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarningEnabled { get; }

        bool IsErrorEnabled { get; }

        void Debug(string source, string message);

        void Info(string source, string message);

        void Warning(string source, string message);

        void Error(string source, string message);
    }
}