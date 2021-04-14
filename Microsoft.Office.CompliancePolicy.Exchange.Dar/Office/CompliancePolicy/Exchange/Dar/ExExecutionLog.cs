using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExExecutionLog : ExecutionLog, IWorkloadLogger
	{
		public ExExecutionLog(DarServiceProvider provider)
		{
			this.provider = provider;
		}

		public override void LogInformation(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, contextData, null, ResultSeverityLevel.Informational, customData);
		}

		public override void LogVerbose(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, contextData, null, ResultSeverityLevel.Verbose, customData);
		}

		public override void LogWarnining(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, contextData, null, ResultSeverityLevel.Warning, customData);
		}

		public override void LogError(string client, string tenantId, string correlationId, Exception exception, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, contextData, exception, ResultSeverityLevel.Error, customData);
		}

		public void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			if (eventType != ActivityEventType.WatsonActivity)
			{
				return;
			}
			this.LogOneEntry("SystemWorkload", activityScope.TenantId, activityScope.ClientRequestId, ExecutionLog.EventType.CriticalError, activityScope.ClientInfo, activityScope.ToString(), null, (from t in activityScope.Metadata
			select new KeyValuePair<string, object>(t.Key.ToString(), t.Value)).ToArray<KeyValuePair<string, object>>());
		}

		public override void LogOneEntry(string client, string correlationId, ExecutionLog.EventType eventType, string contextData, Exception exception)
		{
			this.LogOneEntry(client, null, correlationId, eventType, null, contextData, exception, null);
		}

		public override void LogOneEntry(string client, string tenantId, string correlationId, ExecutionLog.EventType eventType, string tag, string contextData, Exception exception, params KeyValuePair<string, object>[] customData)
		{
			if (exception != null)
			{
				exception.ToString();
			}
			ResultSeverityLevel resultLevel = (ResultSeverityLevel)0;
			switch (eventType)
			{
			case ExecutionLog.EventType.Verbose:
				resultLevel = ResultSeverityLevel.Verbose;
				break;
			case ExecutionLog.EventType.Information:
				resultLevel = ResultSeverityLevel.Informational;
				break;
			case ExecutionLog.EventType.Warning:
				resultLevel = ResultSeverityLevel.Warning;
				break;
			case ExecutionLog.EventType.Error:
				resultLevel = ResultSeverityLevel.Error;
				break;
			case ExecutionLog.EventType.CriticalError:
				resultLevel = ResultSeverityLevel.Critical;
				break;
			}
			this.LogOneEntry(client, tenantId, correlationId, contextData, exception, resultLevel, customData);
		}

		private void LogOneEntry(string client, string tenantId, string correlationId, string contextData, Exception exception, ResultSeverityLevel resultLevel, params KeyValuePair<string, object>[] customData)
		{
			if (exception != null)
			{
				exception.ToString();
			}
			string tag = null;
			if (string.IsNullOrEmpty(client))
			{
				client = "Unknown";
			}
			if (customData != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in customData)
				{
					if (keyValuePair.Key == "Tag")
					{
						tag = (keyValuePair.Value as string);
						break;
					}
				}
			}
			string text;
			if (exception == null || string.IsNullOrEmpty(contextData))
			{
				text = exception + contextData;
			}
			else
			{
				text = contextData + ". Exception: " + exception;
			}
			string component = client;
			string message = text;
			LogItem logItem = new LogItem(component, tag, resultLevel, message, correlationId, tenantId, this.GetCustomProperties(customData));
			logItem.Publish();
		}

		private Dictionary<string, string> GetCustomProperties(KeyValuePair<string, object>[] customData)
		{
			if (customData == null || customData.Length == 0)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> keyValuePair in customData)
			{
				dictionary[keyValuePair.Key] = ((keyValuePair.Value != null) ? keyValuePair.Value.ToString() : null);
			}
			return dictionary;
		}

		private DarServiceProvider provider;
	}
}
