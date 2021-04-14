using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;

namespace Microsoft.Exchange.Diagnostics
{
	public class FileBasedDeserializedTypeGatherer : IDeserializedTypesGatherer
	{
		public FileBasedDeserializedTypeGatherer(string logPath, TimeSpan dumpInterval)
		{
			try
			{
				this.filePath = logPath;
				this.dumpInterval = dumpInterval;
				this.dumpTimer = new Timer(this.dumpInterval.TotalMilliseconds);
				this.dumpTimer.Elapsed += this.FlushIfNecessary;
				this.dumpTimer.Start();
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					this.ProcessId = currentProcess.Id;
				}
			}
			catch
			{
				this.filePath = null;
			}
		}

		public int ProcessId { get; private set; }

		public bool AddStackTrace { get; set; }

		public bool ClearAfterSave { get; set; }

		public void Add(string typeName, string assemblyName)
		{
			if (ExchangeBinaryFormatterFactory.LoggingEnabled)
			{
				if (this.filePath == null || this.dumpTimer == null)
				{
					return;
				}
				this.map.TryAdd(this.BuildKey(typeName, assemblyName), 0);
			}
		}

		private string BuildKey(string typeName, string assemblyName)
		{
			string text = null;
			if (this.AddStackTrace)
			{
				StackTrace stackTrace = new StackTrace(2);
				text = stackTrace.ToString();
			}
			return string.Concat(new string[]
			{
				typeName,
				"--",
				assemblyName,
				"--",
				text
			}) ?? "NoStack";
		}

		private void FlushIfNecessary(object state, ElapsedEventArgs e)
		{
			try
			{
				if (this.filePath != null && this.map.Any<KeyValuePair<string, int>>())
				{
					ConcurrentDictionary<string, int> concurrentDictionary = null;
					if (DateTime.UtcNow - this.lastRefreshUtcTime >= this.dumpInterval)
					{
						lock (this.lockObj)
						{
							if (DateTime.UtcNow - this.lastRefreshUtcTime >= this.dumpInterval)
							{
								concurrentDictionary = this.map;
								if (this.ClearAfterSave)
								{
									this.map = new ConcurrentDictionary<string, int>();
								}
								this.lastRefreshUtcTime = DateTime.UtcNow;
							}
						}
					}
					if (concurrentDictionary != null)
					{
						if (!Directory.Exists(this.filePath))
						{
							Directory.CreateDirectory(this.filePath);
						}
						string path = Path.Combine(this.filePath, "DeserializedTypesEncountered_" + this.ProcessId + ".log");
						using (StreamWriter streamWriter = new StreamWriter(path))
						{
							foreach (KeyValuePair<string, int> keyValuePair in concurrentDictionary)
							{
								streamWriter.WriteLine(keyValuePair.Key);
							}
							streamWriter.Flush();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private TimeSpan dumpInterval;

		private object lockObj = new object();

		private DateTime lastRefreshUtcTime = DateTime.UtcNow;

		private string filePath;

		private Timer dumpTimer;

		private ConcurrentDictionary<string, int> map = new ConcurrentDictionary<string, int>();
	}
}
