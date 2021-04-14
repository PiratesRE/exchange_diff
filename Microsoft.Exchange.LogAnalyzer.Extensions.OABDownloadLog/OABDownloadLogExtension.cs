using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog
{
	public sealed class OABDownloadLogExtension : LogExtension
	{
		public OABDownloadLogExtension(Job job) : base(job)
		{
		}

		public override string GetName()
		{
			return "OABDownloadLog";
		}

		public override void Initialize()
		{
		}

		public override Type GetLogAnalyzerBaseType()
		{
			return typeof(OABDownloadLogAnalyzer);
		}

		public override void ProcessLine(LogSourceLine line, List<LogLine> logLinesProcessed)
		{
			if (line.LogSource.Schema.IsHeader(line))
			{
				List<string> list;
				if (!StringUtils.TryGetColumns(line.Text, ',', ref list))
				{
					Log.LogErrorMessage(string.Format("Format Exception: Unable to parse OABDownload log header from line - '{0}'", line.Text), new object[0]);
					return;
				}
				if (string.IsNullOrEmpty(list[0]) || !list[0].Equals("DateTime", StringComparison.OrdinalIgnoreCase))
				{
					Log.LogErrorMessage(string.Format("Format Exception: OABDownload log Header is in an incorrect format. The first parsed column is not equal to DateTime: - '{0}'", line.Text), new object[0]);
					return;
				}
				this.logHeader = list;
				return;
			}
			else
			{
				if (line.LogSource.Schema.IsComment(line))
				{
					return;
				}
				if (this.logHeader == null || this.logHeader.Count == 0)
				{
					Log.LogErrorMessage("Format Exception: OABDownload log line processing skipped since we have not yet parsed a valid header.", new object[0]);
					return;
				}
				try
				{
					OABDownloadLogLine item = new OABDownloadLogLine(this.logHeader, line);
					logLinesProcessed.Add(item);
				}
				catch (InvalidDataException ex)
				{
					Log.LogErrorMessage("Skipped corrupted log line. Exception - {0}", new object[]
					{
						ex
					});
				}
				return;
			}
		}

		public const string Name = "OABDownloadLog";

		private List<string> logHeader;
	}
}
