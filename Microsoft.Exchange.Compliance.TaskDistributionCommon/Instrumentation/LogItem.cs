using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Compliance.CrimsonEvents;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal class LogItem : EventNotificationItem
	{
		public LogItem(string serviceName, string component, string notificationReason, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, notificationReason, severity)
		{
		}

		public LogItem(string serviceName, string component, string notificationReason, string message, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, message, severity)
		{
		}

		public LogItem(string serviceName, string component, string notificationReason, string message, string stateAttribute1, ResultSeverityLevel severity = ResultSeverityLevel.Error) : base(serviceName, component, notificationReason, message, stateAttribute1, severity)
		{
		}

		public static bool DisableComplianceCrimsonEvents
		{
			get
			{
				return LogItem.disableComplianceCrimsonEvents;
			}
			set
			{
				LogItem.disableComplianceCrimsonEvents = value;
			}
		}

		public new static void Publish(string serviceName, string component, string tag, string notificationReason, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (LogItem.DisableComplianceCrimsonEvents)
			{
				return;
			}
			if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(component))
			{
				throw new ArgumentException("serviceName and component must have non-null/non-empty values.");
			}
			LogItem logItem = new LogItem(serviceName, component, tag, notificationReason, severity);
			logItem.Publish(throwOnError);
		}

		public new static void Publish(string serviceName, string component, string tag, string notificationReason, string stateAttribute1, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (LogItem.DisableComplianceCrimsonEvents)
			{
				return;
			}
			if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(component))
			{
				throw new ArgumentException("serviceName and component must have non-null/non-empty values.");
			}
			LogItem logItem = new LogItem(serviceName, component, tag, notificationReason, stateAttribute1, severity);
			logItem.Publish(throwOnError);
		}

		public new static void PublishPeriodic(string serviceName, string component, string tag, string notificationReason, string periodicKey, TimeSpan period, ResultSeverityLevel severity = ResultSeverityLevel.Error, bool throwOnError = false)
		{
			if (LogItem.DisableComplianceCrimsonEvents)
			{
				return;
			}
			if (string.IsNullOrEmpty(periodicKey))
			{
				throw new ArgumentException("periodicKey must have non-null/non-empty value.");
			}
			if (LogItem.CanPublishPeriodic(periodicKey, period))
			{
				LogItem.Publish(serviceName, component, tag, notificationReason, severity, throwOnError);
			}
		}

		public new void Publish(bool throwOnError)
		{
			string extensionXml = CrimsonHelper.ConvertDictionaryToXml(base.CustomProperties);
			NativeMethods.ProbeResultUnmanaged probeResultUnmanaged = new ProbeResult
			{
				ResultName = base.ResultName,
				SampleValue = base.SampleValue,
				ServiceName = base.ServiceName,
				IsNotified = true,
				ExecutionStartTime = base.TimeStamp,
				ExecutionEndTime = base.TimeStamp,
				Error = base.Message,
				ExtensionXml = extensionXml,
				StateAttribute1 = base.StateAttribute1,
				StateAttribute2 = base.StateAttribute2,
				StateAttribute3 = base.StateAttribute3,
				StateAttribute4 = base.StateAttribute4,
				StateAttribute5 = base.StateAttribute5,
				WorkItemId = DefinitionIdGenerator<ProbeDefinition>.GetIdForNotification(base.ResultName)
			}.ToUnmanaged();
			try
			{
				ComplianceCrimsonEvent complianceCrimsonEvent = ComplianceCrimsonEvents.EvtDarRuntimeLog;
				if (base.Severity == ResultSeverityLevel.Informational)
				{
					complianceCrimsonEvent = ComplianceCrimsonEvents.EvtDiscoveryInfo;
				}
				complianceCrimsonEvent.LogGeneric(new object[]
				{
					DateTime.UtcNow.Ticks.ToString(),
					probeResultUnmanaged.ServiceName,
					probeResultUnmanaged.ResultName,
					probeResultUnmanaged.WorkItemId.ToString(),
					LogItem.build,
					probeResultUnmanaged.MachineName,
					probeResultUnmanaged.Error,
					probeResultUnmanaged.Exception,
					probeResultUnmanaged.RetryCount.ToString(),
					probeResultUnmanaged.StateAttribute1,
					probeResultUnmanaged.StateAttribute2,
					probeResultUnmanaged.StateAttribute3,
					probeResultUnmanaged.StateAttribute4,
					probeResultUnmanaged.StateAttribute5,
					base.Severity.ToString(),
					probeResultUnmanaged.ExecutionStartTime,
					probeResultUnmanaged.ExtensionXml,
					probeResultUnmanaged.Version.ToString()
				});
			}
			catch
			{
				if (throwOnError)
				{
					throw;
				}
			}
			finally
			{
				if (LogSettings.IsMonitored(base.ResultName))
				{
					base.Publish(throwOnError);
				}
			}
		}

		private static bool CanPublishPeriodic(string periodicKey, TimeSpan period)
		{
			bool result;
			lock (LogItem.periodicEventDictionary)
			{
				DateTime d;
				if (!LogItem.periodicEventDictionary.TryGetValue(periodicKey, out d) || DateTime.UtcNow > d + period)
				{
					LogItem.periodicEventDictionary[periodicKey] = DateTime.UtcNow;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static readonly string build = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private static readonly Dictionary<string, DateTime> periodicEventDictionary = new Dictionary<string, DateTime>();

		private static bool disableComplianceCrimsonEvents = false;
	}
}
