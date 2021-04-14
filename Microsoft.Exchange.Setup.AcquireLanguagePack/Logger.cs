using System;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class Logger
	{
		public static bool ErrorFound
		{
			get
			{
				return Logger.errorFound;
			}
			set
			{
				Logger.errorFound = value;
			}
		}

		private static void NewLogSetup()
		{
			Logger.isNewLog = false;
			if (!Directory.Exists(Logger.PathToDirLog))
			{
				Directory.CreateDirectory(Logger.PathToDirLog);
			}
			StreamWriter streamWriter = new StreamWriter(Logger.PathToFileLog, true);
			streamWriter.WriteLine("\n\n ************************\n");
			streamWriter.Close();
		}

		public static void LoggerMessage(string message)
		{
			lock (Logger.objLock)
			{
				if (Logger.isNewLog)
				{
					Logger.NewLogSetup();
				}
				StreamWriter streamWriter = new StreamWriter(Logger.PathToFileLog, true);
				streamWriter.WriteLine("-- " + message);
				streamWriter.Close();
			}
		}

		public static void LogError(Exception exception)
		{
			lock (Logger.objLock)
			{
				Logger.errorFound = true;
				if (Logger.isNewLog)
				{
					Logger.NewLogSetup();
				}
				StreamWriter streamWriter = new StreamWriter(Logger.PathToFileLog, true);
				streamWriter.WriteLine(Strings.ErrorInLog + " -- " + exception.ToString());
				streamWriter.Close();
			}
		}

		public static void UserCancel()
		{
			Logger.errorFound = true;
		}

		public static readonly string PathToDirLog = Path.Combine(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")), "ExchangeSetupLogs\\");

		public static readonly string PathToFileLog = Path.Combine(Logger.PathToDirLog, "LPSetupUILog.log");

		private static bool isNewLog = true;

		private static bool errorFound = false;

		private static object objLock = new object();
	}
}
