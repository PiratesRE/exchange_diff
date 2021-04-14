using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ProcessLog
	{
		internal static ITempFile GenerateDump()
		{
			ITempFile tempFile = TempFileFactory.CreateTempFile();
			try
			{
				using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						if (ProcessLog.logDirectory.Exists)
						{
							FileInfo[] files = ProcessLog.logDirectory.GetFiles("*.log");
							foreach (FileInfo fileInfo in files)
							{
								try
								{
									streamWriter.WriteLine();
									streamWriter.WriteLine("--- Dumping Process Log ---");
									using (FileStream fileStream2 = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
									{
										using (StreamReader streamReader = new StreamReader(fileStream2))
										{
											streamWriter.WriteLine(streamReader.ReadToEnd());
											streamWriter.WriteLine();
										}
									}
								}
								catch (IOException ex)
								{
									streamWriter.WriteLine();
									streamWriter.WriteLine("--- Failed to Dump Process Log! ---");
									streamWriter.WriteLine(ex.ToString());
									streamWriter.WriteLine();
								}
							}
						}
					}
				}
			}
			catch (IOException)
			{
			}
			return tempFile;
		}

		internal static void WriteLine(string s, params object[] args)
		{
			lock (ProcessLog.logLock)
			{
				if (ProcessLog.logWriter == null)
				{
					ProcessLog.logWriter = ProcessLog.OpenOrCreateLogFile();
					ProcessLog.logWriter.WriteLine("--- Starting new process log for {0} ---", ProcessLog.processName);
				}
				ProcessLog.logWriter.Write(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}\t{2}: ", new object[]
				{
					ExDateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
					ProcessLog.pid,
					ProcessLog.processName
				}));
				if (args == null || args.Length == 0)
				{
					ProcessLog.logWriter.WriteLine(s);
				}
				else
				{
					ProcessLog.logWriter.WriteLine(s, args);
				}
				ProcessLog.logWriter.Flush();
			}
		}

		private static StreamWriter OpenOrCreateLogFile()
		{
			if (!ProcessLog.logDirectory.Exists)
			{
				ProcessLog.logDirectory.Create();
			}
			string text = null;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				text = (ProcessLog.processName = currentProcess.ProcessName);
				ProcessLog.pid = currentProcess.Id.ToString(CultureInfo.InvariantCulture);
			}
			StreamWriter result;
			if (!ProcessLog.TryOpenExistingLog(text, out result))
			{
				string path = Path.Combine(ProcessLog.logDirectory.FullName, text + "-" + Guid.NewGuid().ToString() + ".log");
				FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
				result = new StreamWriter(stream);
			}
			return result;
		}

		private static bool TryOpenExistingLog(string logPrefixName, out StreamWriter sw)
		{
			sw = null;
			string searchPattern = logPrefixName + "*.log";
			foreach (FileInfo fileInfo in ProcessLog.logDirectory.GetFiles(searchPattern))
			{
				try
				{
					if (fileInfo.Length > 1048576L)
					{
						string destFileName = Path.Combine(ProcessLog.logDirectory.FullName, logPrefixName + ".old");
						fileInfo.MoveTo(destFileName);
					}
					else if (sw == null)
					{
						FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
						fileStream.Seek(0L, SeekOrigin.End);
						sw = new StreamWriter(fileStream);
					}
					else
					{
						sw.WriteLine("--- Merging data from log {0} ---", fileInfo.Name);
						sw.WriteLine(File.ReadAllText(fileInfo.FullName));
						File.Delete(fileInfo.FullName);
					}
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
			}
			return null != sw;
		}

		private const string TempSubDirectory = "0c2410f0-f78a-11dc-95ff-0800200c9a66";

		private const int MaxLogSizeInBytes = 1048576;

		private const string LogExtension = ".log";

		private const string OldExtension = ".old";

		private static StreamWriter logWriter;

		private static object logLock = new object();

		private static string processName;

		private static string pid;

		private static DirectoryInfo logDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "0c2410f0-f78a-11dc-95ff-0800200c9a66"));
	}
}
