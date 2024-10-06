using NLog;
using ILogger = NLog.ILogger;

namespace Wallets.Exceptions
{
    #region Interfaces
    public enum Exceptionlevel { Debug, Information, Warning, Error, Critical }

    public class DefaultExceptionConfig
    {
        public Exceptionlevel exLevel { get; set; } = Exceptionlevel.Error;
    }
    #endregion

    public static class DefaultException
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static bool enThrowException { get; set; } = true;
        public static string DefaultLogTag = "[#Maccos]";
        public static string DebugLogTag = "[#Debug-Exception]";
        public static string InformationLogTag = "[#Information-Exception]";
        public static string WarningLogTag = "[#Warning-Exception]";
        public static string ErrorLogTag = "[#Error-Exception]";
        public static string CriticalLogTag = "[#Critical-Exception]";
        public static string UnknownLogTag = "[#Unknown-Exception]";
        public static string DefaultExceptionMessage = "Service is currently unavailable. We apologize for the inconvenience.";

        #region Public Methods
        public async static Task<T> ExceptionControllerHandler<T>(Func<Task<T>> action)
        {
            try
            {
                enThrowException = false;
                return await action();
            }
            catch (Exception ex)
            {
                enThrowException = true;
                Throw(null, ex?.Message, new DefaultExceptionConfig { exLevel = Exceptionlevel.Error });
                return default;
            }
        }

        public static void Throw(this Exception ex, string logMsg = "", DefaultExceptionConfig config = null)
        {
            if (config == null) config = new DefaultExceptionConfig();
            if (!string.IsNullOrEmpty(logMsg)) config?.exLevel.LogCustom(logMsg);
            if (ex != null) throw new Exception(ex?.Message?.DoMessageAdjustments() ?? DefaultExceptionMessage);
        }

        public static Exceptionlevel GetLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 100 and < 200 => Exceptionlevel.Debug,
                >= 200 and < 300 => Exceptionlevel.Information,
                >= 300 and < 400 => Exceptionlevel.Information,
                >= 400 and < 500 => Exceptionlevel.Warning,
                >= 500 => Exceptionlevel.Error,
                _ => Exceptionlevel.Critical
            };
        }
        #endregion

        #region Private Methods
        private static string DoMessageAdjustments(this string exMessage)
        {
            if (string.IsNullOrEmpty(exMessage)) return DefaultExceptionMessage;
            return exMessage;
        }

        private static void LogCustom(this Exceptionlevel level, string msg)
        {
            switch (level)
            {
                case Exceptionlevel.Debug:
                    _logger.Error(DefaultLogTag + "[#Debug-Exception]: " + msg);
                    break;
                case Exceptionlevel.Information:
                    _logger.Error(DefaultLogTag + "[#Information-Exception]: " + msg);
                    break;
                case Exceptionlevel.Warning:
                    _logger.Error(DefaultLogTag + "[#Warning-Exception]: " + msg);
                    break;
                case Exceptionlevel.Error:
                    _logger.Error(DefaultLogTag + "[#Error-Exception]: " + msg);
                    break;
                case Exceptionlevel.Critical:
                    _logger.Error(DefaultLogTag + "[#Critical-Exception]: " + msg);
                    break;

                default:
                    _logger.Error(DefaultLogTag + "[#Unknown-Exception]: " + msg);
                    break;
            }
        }

        private static void Log(this Exceptionlevel level, string msg)
        {
            switch (level)
            {
                case Exceptionlevel.Debug:
                    _logger.Debug(msg);
                    break;
                case Exceptionlevel.Information:
                    _logger.Info(msg);
                    break;
                case Exceptionlevel.Warning:
                    _logger.Warn(msg);
                    break;
                case Exceptionlevel.Error:
                    _logger.Error(msg);
                    break;
                case Exceptionlevel.Critical:
                    _logger.Fatal(msg);
                    break;

                default:
                    _logger.Fatal(msg);
                    break;
            }
        }
        #endregion
    }

}