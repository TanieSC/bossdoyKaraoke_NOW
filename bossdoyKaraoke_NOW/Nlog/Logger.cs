using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace bossdoyKaraoke_NOW.Nlog
{
    public static class Logger
    {

        static string m_logdirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\karaokeNow\logs\"; //AppDomain.CurrentDomain.BaseDirectory + @"logs\";
        static string m_logfilepath = m_logdirectory + DateTime.Now.Date.ToString("MMMM-dd-yyyy") + ".txt";  //logfile.txt";

        public static void LogFile(string sExceptionName, string sEventName, string sControlName, int nErrorLineNo, string sFormName)

        {

            StreamWriter log;
            Directory.CreateDirectory(m_logdirectory);
            if (!File.Exists(m_logfilepath))
            {
                log = new StreamWriter(m_logfilepath);

            }

            else

            {

                log = File.AppendText(m_logfilepath);

            }

            // Write to the file:
            log.WriteLine("=======================================================================================");

            log.WriteLine("Data Time: " + DateTime.Now);

            log.WriteLine("Exception Name: " + sExceptionName);

            log.WriteLine("Event Name: " + sEventName);

            log.WriteLine("Control Name: " + sControlName);

            log.WriteLine("Error Line No.: " + nErrorLineNo);

            log.WriteLine("Form Name: " + sFormName);

            // Close the stream:

            log.Close();

        }




        /* private static readonly NLog.Logger  _logger; //NLog logger
         private const string _DEFAULTLOGGER = "CustomLogger";

         static Logger()
         {
             _logger =  LogManager.GetLogger(_DEFAULTLOGGER) ?? LogManager.GetCurrentClassLogger();
         }

         public static void MyLogger() {
             _logger.Log(LogLevel.Debug, "Debug message");
         }

         #region Public Methods
         /// <summary>
         /// This method writes the Debug information to trace file
         /// </summary>
         /// <param name="message">The message.</param>
         public static void Debug(String message)
         {
             if (!_logger.IsDebugEnabled) return;
             _logger.Debug(message);
         }

         /// <summary>
         /// This method writes the Information to trace file
         /// </summary>
         /// <param name="message">The message.</param>
         public static void Info(String message)
         {
             if (!_logger.IsInfoEnabled) return;
             _logger.Info(message);
         }

         /// <summary>
         /// This method writes the Warning information to trace file
         /// </summary>
         /// <param name="message">The message.</param>
         public static void Warn(String message)
         {
             if (!_logger.IsWarnEnabled) return;
             _logger.Warn(message);
         }

         /// <summary>
         /// This method writes the Error Information to trace file
         /// </summary>
         /// <param name="error">The error.</param>
         /// <param name="exception">The exception.</param>
         public static void Error(String error, Exception exception = null)
         {
             if (!_logger.IsErrorEnabled) return;
             _logger.ErrorException(error, exception);
         }

         /// <summary>
         /// This method writes the Fatal exception information to trace target
         /// </summary>
         /// <param name="message">The message.</param>
         public static void Fatal(String message)
         {
             if (!_logger.IsFatalEnabled) return;
             _logger.Warn(message);
         }

         /// <summary>
         /// This method writes the trace information to trace target
         /// </summary>
         /// <param name="message">The message.</param>
         public static void Trace(String message)
         {
             if (!_logger.IsTraceEnabled) return;
             _logger.Trace(message);
         }

         #endregion */
    }

    public static class ExceptionHelper

    {

        public static int LineNumber(this Exception e)

        {

            int linenum = 0;

            try

            {

                linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(":line") + 5));

            }

            catch

            {

                //Stack trace is not available!

            }

            return linenum;

        }

    }
}
