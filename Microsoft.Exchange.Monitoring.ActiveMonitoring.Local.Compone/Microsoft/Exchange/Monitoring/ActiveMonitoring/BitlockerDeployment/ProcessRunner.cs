using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment
{
	internal static class ProcessRunner
	{
		public static int Run(string executableFilename, string arguments, int timeout, string workingDirectory, out string outputString, out string errorString)
		{
			return ProcessRunner.Run(executableFilename, arguments, null, timeout, workingDirectory, out outputString, out errorString);
		}

		public static int Run(string executableFilename, string arguments, Dictionary<string, string> environmentVariables, int timeout, string workingDirectory, out string outputString, out string errorString)
		{
			if (executableFilename == null)
			{
				throw new ArgumentNullException("executableFileName");
			}
			int result = 0;
			StringBuilder standardOutput = new StringBuilder();
			StringBuilder standardError = new StringBuilder();
			using (Process process = new Process())
			{
				using (ManualResetEvent stdOutputEvent = new ManualResetEvent(false))
				{
					using (ManualResetEvent stdErrorEvent = new ManualResetEvent(false))
					{
						process.StartInfo = new ProcessStartInfo();
						process.StartInfo.FileName = executableFilename;
						if (workingDirectory != null)
						{
							process.StartInfo.WorkingDirectory = workingDirectory;
						}
						if (environmentVariables != null)
						{
							foreach (KeyValuePair<string, string> keyValuePair in environmentVariables)
							{
								process.StartInfo.EnvironmentVariables[keyValuePair.Key] = keyValuePair.Value;
							}
						}
						process.StartInfo.CreateNoWindow = true;
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.Arguments = arguments;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.RedirectStandardError = true;
						process.OutputDataReceived += delegate(object sendingProcess, DataReceivedEventArgs outLine)
						{
							if (outLine.Data == null)
							{
								stdOutputEvent.Set();
								return;
							}
							if (outLine.Data != string.Empty)
							{
								standardOutput.AppendLine(outLine.Data);
							}
						};
						process.ErrorDataReceived += delegate(object sendingProcess, DataReceivedEventArgs outLine)
						{
							if (outLine.Data == null)
							{
								stdErrorEvent.Set();
								return;
							}
							if (outLine.Data != string.Empty)
							{
								standardError.AppendLine(outLine.Data);
							}
						};
						process.Start();
						process.BeginOutputReadLine();
						process.BeginErrorReadLine();
						if (timeout != -1)
						{
							process.WaitForExit(timeout);
						}
						else
						{
							process.WaitForExit();
						}
						if (!process.HasExited)
						{
							ExWatson.SendHangWatsonReport(new ProcessRunner.ExchangeProcessTimeoutException(), process);
							process.Close();
							throw new TimeoutException(string.Format("Process took more than {0} seconds to complete", 100000));
						}
						stdOutputEvent.WaitOne(100000);
						stdErrorEvent.WaitOne(100000);
						result = process.ExitCode;
					}
				}
			}
			try
			{
				outputString = standardOutput.ToString();
			}
			catch (ArgumentOutOfRangeException)
			{
				outputString = null;
			}
			try
			{
				errorString = standardError.ToString();
			}
			catch (ArgumentOutOfRangeException)
			{
				errorString = null;
			}
			return result;
		}

		public const int NoTimeout = -1;

		public const int StdOutputErrorEventTimeout = 100000;

		public class ExchangeProcessTimeoutException : Exception
		{
		}
	}
}
