using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Filtering;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Filtering
{
	internal class TextExtractionLog
	{
		public string LogPath
		{
			get
			{
				if (this.logPath == null)
				{
					this.logPath = string.Empty;
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
					{
						if (registryKey != null)
						{
							object value = registryKey.GetValue("TextExtractionLogPath");
							if (value != null)
							{
								this.logPath = value.ToString();
							}
						}
					}
				}
				return this.logPath;
			}
		}

		public void Start()
		{
			if (string.IsNullOrEmpty(this.LogPath))
			{
				this.enabled = false;
				return;
			}
			this.CreateLog();
			this.ConfigureLog();
			ExTraceGlobals.FilteringServiceApiTracer.TraceDebug((long)this.GetHashCode(), "Text Extraction Information logging has been started.");
			this.enabled = true;
		}

		public void Stop()
		{
			if (this.log != null)
			{
				this.FlushBuffer();
				this.log.Close();
				this.log = null;
			}
			this.enabled = false;
			ExTraceGlobals.FilteringServiceApiTracer.TraceDebug((long)this.GetHashCode(), "Text Extraction Information logging has been stopped.");
		}

		public void FlushBuffer()
		{
			if (this.log != null)
			{
				this.log.Flush();
			}
		}

		public void Trace(string exMessageId, TextExtractionData teData)
		{
			if (!this.enabled || this.log == null)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.textExtractionLogSchemaMapping);
			logRowFormatter[1] = exMessageId;
			logRowFormatter[2] = teData.StreamId;
			logRowFormatter[3] = teData.StreamSize;
			logRowFormatter[4] = teData.ParentId;
			logRowFormatter[5] = teData.Types;
			logRowFormatter[6] = teData.ModuleUsed;
			logRowFormatter[7] = teData.TextExtractionResult;
			logRowFormatter[8] = teData.SkippedModules;
			logRowFormatter[9] = teData.FailedModules;
			logRowFormatter[10] = teData.DisabledModules;
			logRowFormatter[11] = teData.AdditionalInformation;
			this.Append(logRowFormatter);
		}

		private void Append(LogRowFormatter row)
		{
			try
			{
				if (this.log != null)
				{
					this.log.Append(row, TextExtractionSchema.TimeStampFieldIndex);
				}
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || ex is ThreadAbortException || ex is AccessViolationException || ex is SEHException)
				{
					throw;
				}
				ExTraceGlobals.FilteringServiceApiTracer.TraceError<string>((long)this.GetHashCode(), "Failed to append a row in the text extraction log. Error: {0}", ex.Message);
			}
		}

		private void ConfigureLog()
		{
			long num = 10485760L;
			long maxDirectorySize = 50L * num;
			int bufferSize = 4096;
			TimeSpan maxAge = TimeSpan.FromDays(4.0);
			TimeSpan streamFlushInterval = TimeSpan.FromMinutes(1.0);
			TimeSpan backgroundWriteInterval = TimeSpan.FromSeconds(1.0);
			this.log.Configure(this.LogPath, maxAge, maxDirectorySize, num, bufferSize, streamFlushInterval, backgroundWriteInterval);
		}

		private void CreateLog()
		{
			this.log = new AsyncLog("TELOG", new LogHeaderFormatter(this.textExtractionLogSchemaMapping), "TextExtractionLog");
		}

		private const string LogComponentName = "TextExtractionLog";

		private const string LogPathRegistryKey = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string LogPathRegistryValue = "TextExtractionLogPath";

		private AsyncLog log;

		private bool enabled;

		private string logPath;

		private LogSchema textExtractionLogSchemaMapping = new LogSchema("Microsoft Exchange Server", TextExtractionSchema.DefaultVersion.ToString(), "Text Extraction Log", (from csvfield in TextExtractionSchema.DefaultSchema.Fields
		select csvfield.Name).ToArray<string>());
	}
}
