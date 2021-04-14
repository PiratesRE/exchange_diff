using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class OperatorDiagnostics : IComparable<OperatorDiagnostics>
	{
		public OperatorDiagnostics(string flowIdentifier, DiagnosticsLogConfig.LogDefaults logDefaults)
		{
			this.FlowIdentifier = flowIdentifier;
			this.DropBreadcrumb(OperatorLocation.DiagnosticsStarted, "OperatorDiagnostics", TimeSpan.Zero, null);
			this.failedItemsLog = new DiagnosticsLog(new DiagnosticsLogConfig(logDefaults), OperatorDiagnostics.failedItemsSchemaColumns);
			this.languageDetectionLog = new DiagnosticsLog(new DiagnosticsLogConfig(OperatorDiagnosticsFactory.LanguageDetectionLogDefaults), OperatorDiagnostics.languageDetectionSchemaColumns);
		}

		public string FlowIdentifier { get; private set; }

		public Guid InstanceGuid { get; set; }

		public string InstanceName { get; set; }

		public OperatorTimingEntry LastEntry
		{
			get
			{
				OperatorTimingEntry result;
				lock (this.lockObject)
				{
					result = this.operatorTimings[this.operatorTimings.Count - 1];
				}
				return result;
			}
		}

		public TimeSpan GetSplitTime()
		{
			return this.timer.GetSplitTime();
		}

		public XElement GetBreadcrumbs(bool verbose)
		{
			XElement xelement = new XElement("Session");
			xelement.Add(new XElement("FlowIdentifier", this.FlowIdentifier));
			XElement xelement2 = new XElement("Breadcrumbs");
			xelement.Add(xelement2);
			lock (this.lockObject)
			{
				if (verbose)
				{
					for (int i = 1; i < 33; i++)
					{
						int index = (this.breadcrumbIndex + i) % 32;
						XElement breadcrumb = this.GetBreadcrumb(index);
						if (breadcrumb != null)
						{
							xelement2.Add(breadcrumb);
						}
					}
				}
				else
				{
					xelement2.Add(this.GetBreadcrumb(this.breadcrumbIndex));
				}
			}
			return xelement;
		}

		public TimeSpan DropBreadcrumb(OperatorLocation location, string operatorName)
		{
			return this.DropBreadcrumb(location, operatorName, this.timer.GetLapTime(), null);
		}

		public TimeSpan DropBreadcrumb(OperatorLocation location, string operatorName, string exception)
		{
			return this.DropBreadcrumb(location, operatorName, this.timer.GetLapTime(), null);
		}

		public TimeSpan DropBreadcrumb(OperatorLocation location, string operatorName, TimeSpan elapsed)
		{
			return this.DropBreadcrumb(location, operatorName, elapsed, null);
		}

		public TimeSpan DropBreadcrumb(OperatorLocation location, string operatorName, TimeSpan elapsed, string exception)
		{
			if (location == OperatorLocation.None)
			{
				throw new ArgumentException("location");
			}
			lock (this.lockObject)
			{
				if (++this.breadcrumbIndex == 32)
				{
					this.breadcrumbIndex = 0;
				}
				this.breadcrumbs[this.breadcrumbIndex].OperatorName = operatorName;
				this.breadcrumbs[this.breadcrumbIndex].Location = location;
				this.breadcrumbs[this.breadcrumbIndex].Timestamp = DateTime.UtcNow;
				this.breadcrumbs[this.breadcrumbIndex].Elapsed = elapsed;
				this.breadcrumbs[this.breadcrumbIndex].Exception = exception;
			}
			if (this.operatorTimings.Count < 32)
			{
				this.operatorTimings.Add(new OperatorTimingEntry
				{
					Name = operatorName,
					Location = location,
					Elapsed = (long)elapsed.TotalMilliseconds
				});
			}
			return elapsed;
		}

		public void ClearOperatorTimings()
		{
			this.operatorTimings.Clear();
			this.timer.Reset();
		}

		public string GetOperatorTimings(List<string> operatorTimingEntryNames)
		{
			return OperatorTimingEntry.SerializeList(this.operatorTimings, operatorTimingEntryNames);
		}

		public bool GetOperatorTimingEntry(string entryName, OperatorLocation operatorLocation, out OperatorTimingEntry entry)
		{
			entry = default(OperatorTimingEntry);
			foreach (OperatorTimingEntry operatorTimingEntry in this.operatorTimings)
			{
				if (operatorTimingEntry.Name == entryName && operatorTimingEntry.Location == operatorLocation)
				{
					entry = operatorTimingEntry;
					return true;
				}
			}
			return false;
		}

		public int CompareTo(OperatorDiagnostics other)
		{
			int num = string.Compare(this.InstanceName, other.InstanceName, StringComparison.OrdinalIgnoreCase);
			if (num != 0)
			{
				return num;
			}
			num = this.InstanceGuid.CompareTo(other.InstanceGuid);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(this.FlowIdentifier, other.FlowIdentifier, StringComparison.OrdinalIgnoreCase);
		}

		public void LogFailedItem(DateTime lastAttemptTime, string identity, Guid correlationId, bool partiallyProcessed, int attemptCount, string errorCode, string errorMessage)
		{
			this.failedItemsLog.Append(new object[]
			{
				null,
				lastAttemptTime.ToString("u"),
				identity,
				correlationId,
				partiallyProcessed,
				attemptCount,
				errorCode,
				errorMessage
			});
		}

		public void LogLanguageDetection(Guid contextID, string detectedLanguage, long languageDetectorTime, long wordBreakerTime, long messageLength, string messageCodePage, string messageLocaleID, string internetCPID)
		{
			this.languageDetectionLog.Append(new object[]
			{
				null,
				contextID,
				detectedLanguage,
				languageDetectorTime,
				wordBreakerTime,
				messageLength,
				messageCodePage,
				messageLocaleID,
				internetCPID
			});
		}

		private XElement GetBreadcrumb(int index)
		{
			if (this.breadcrumbs[index].Location == OperatorLocation.None)
			{
				return null;
			}
			XElement xelement = new XElement("Breadcrumb");
			xelement.Add(new XElement("Operator", this.breadcrumbs[index].OperatorName));
			xelement.Add(new XElement("Location", this.breadcrumbs[index].Location));
			xelement.Add(new XElement("Timestamp", this.breadcrumbs[index].Timestamp));
			xelement.Add(new XElement("Elapsed", (long)this.breadcrumbs[index].Elapsed.TotalMilliseconds));
			if (this.breadcrumbs[index].Exception != null)
			{
				xelement.Add(new XElement("Exception", this.breadcrumbs[index].Exception));
			}
			return xelement;
		}

		private const int BreadcrumbsSize = 32;

		private static string[] failedItemsSchemaColumns = new string[]
		{
			"date-time",
			"failed-time",
			"identity",
			"correlation-id",
			"attempt-count",
			"partially-processed",
			"error-code",
			"error-message"
		};

		private static string[] languageDetectionSchemaColumns = new string[]
		{
			"date-time",
			"contextid",
			"detectedlanguage",
			"languagedetectortime",
			"wordbreakertime",
			"messagelength",
			"messagecodepage",
			"messagelocaleid",
			"internetcpid"
		};

		private readonly object lockObject = new object();

		private readonly OperatorDiagnostics.Breadcrumb[] breadcrumbs = new OperatorDiagnostics.Breadcrumb[32];

		private readonly LapTimer timer = new LapTimer();

		private int breadcrumbIndex = -1;

		private List<OperatorTimingEntry> operatorTimings = new List<OperatorTimingEntry>(20);

		private DiagnosticsLog failedItemsLog;

		private DiagnosticsLog languageDetectionLog;

		private struct Breadcrumb
		{
			public OperatorLocation Location;

			public string OperatorName;

			public DateTime Timestamp;

			public object Exception;

			public TimeSpan Elapsed;
		}
	}
}
