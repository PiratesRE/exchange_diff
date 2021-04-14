using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	internal class EtwTraceCollector
	{
		public EtwTraceCollector(string guidFileLocation, Dictionary<string, string> etlFilePaths, string traceLogFilePath)
		{
			if (string.IsNullOrEmpty(guidFileLocation))
			{
				throw new ArgumentException("guidfilelocation");
			}
			if (string.IsNullOrEmpty(traceLogFilePath))
			{
				throw new ArgumentException("traceLogFilePath");
			}
			this.guidFileLocation = guidFileLocation;
			this.etlFilePaths = etlFilePaths;
			this.traceLogFilePath = traceLogFilePath;
		}

		internal bool Initialize()
		{
			Log.LogInformationMessage("Starting ETW data collection for server {0}", new object[]
			{
				Environment.MachineName
			});
			using (Dictionary<string, string>.Enumerator enumerator = this.etlFilePaths.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<string, string> keyValuePair = enumerator.Current;
					if (!string.IsNullOrEmpty(this.traceLogFilePath) && !keyValuePair.Key.Equals("NT Kernel Logger"))
					{
						string defaultArgs = string.Format("-start \"{0}\" -guid \"{1}\" ", keyValuePair.Key, this.guidFileLocation);
						EtwTraceCollector.TraceLogCollection state = new EtwTraceCollector.TraceLogCollection(keyValuePair.Value, 15000, keyValuePair.Key, " -matchanykw 0x00000001", defaultArgs);
						this.RunTraceLog(state);
					}
					return true;
				}
			}
			return false;
		}

		private void RunTraceLog(EtwTraceCollector.TraceLogCollection state)
		{
			try
			{
				if (this.ExecuteTraceLogCommand(state.GetArguments()))
				{
					Thread.Sleep(state.SleepTimeInMs);
				}
			}
			finally
			{
				string arguments = string.Format("-stop \"{0}\"", state.TraceName);
				if (!this.ExecuteTraceLogCommand(arguments))
				{
					Log.LogErrorMessage("tracelog.exe exited with a non-zero error code for {0}", new object[]
					{
						state.TraceName
					});
				}
			}
		}

		private bool ExecuteTraceLogCommand(string arguments)
		{
			Log.LogInformationMessage("ETW Calculated Counter: Starting tracelog.exe collection for server {0} using args {1}", new object[]
			{
				Environment.MachineName,
				arguments
			});
			using (Process process = Process.Start(new ProcessStartInfo
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				FileName = this.traceLogFilePath,
				Arguments = arguments
			}))
			{
				process.WaitForExit(30000);
				if (process.ExitCode != 0)
				{
					return false;
				}
			}
			return true;
		}

		private readonly string traceLogFilePath;

		private readonly string guidFileLocation;

		private readonly Dictionary<string, string> etlFilePaths;

		private struct TraceLogCollection
		{
			public TraceLogCollection(string traceFilePath, int sleepTimeInMs, string traceName, string customArgs, string defaultArgs)
			{
				if (string.IsNullOrEmpty(traceFilePath))
				{
					throw new ArgumentNullException("traceFilePath");
				}
				if (string.IsNullOrEmpty(traceName))
				{
					throw new ArgumentNullException("traceName");
				}
				if (string.IsNullOrEmpty(defaultArgs))
				{
					throw new ArgumentNullException("defaultArgs");
				}
				if (sleepTimeInMs < 1)
				{
					sleepTimeInMs = 15000;
				}
				this.TraceFilePath = traceFilePath;
				this.SleepTimeInMs = sleepTimeInMs;
				this.TraceName = traceName;
				this.CustomArgs = customArgs;
				this.DefaultArgs = defaultArgs;
			}

			public string GetArguments()
			{
				string text = string.Format("{0} -f \"{1}\" -seq {2} -min {3} -max {4}", new object[]
				{
					this.DefaultArgs,
					this.TraceFilePath,
					500.ToString(),
					2.ToString(),
					200.ToString()
				});
				if (!string.IsNullOrEmpty(this.CustomArgs))
				{
					string.Join(text, new string[]
					{
						this.CustomArgs
					});
				}
				return text;
			}

			private const int MaximumFileSize = 500;

			private const int MaximumBufferCount = 200;

			private const int MinimumBufferCount = 2;

			public readonly string TraceFilePath;

			public readonly string TraceName;

			public readonly int SleepTimeInMs;

			public readonly string CustomArgs;

			public readonly string DefaultArgs;
		}
	}
}
