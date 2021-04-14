using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class NotificationItem
	{
		public NotificationItem(string serviceName, string component, string tag, string message, ResultSeverityLevel severity)
		{
			this.ServiceName = serviceName;
			this.ResultName = NotificationItem.GenerateResultName(serviceName, component, tag);
			this.Message = message;
			this.CustomProperties = new Dictionary<string, string>();
			this.Severity = severity;
			this.TimeStamp = DateTime.UtcNow;
		}

		public NotificationItem(string serviceName, string component, string tag, string message, string stateAttribute1, ResultSeverityLevel severity) : this(serviceName, component, tag, message, severity)
		{
			this.StateAttribute1 = stateAttribute1;
		}

		public string ServiceName { get; set; }

		public string ResultName { get; set; }

		public Dictionary<string, string> CustomProperties { get; set; }

		public ResultSeverityLevel Severity { get; set; }

		public DateTime TimeStamp { get; set; }

		public double SampleValue { get; set; }

		public string Message { get; set; }

		public string StateAttribute1 { get; set; }

		public string StateAttribute2 { get; set; }

		public string StateAttribute3 { get; set; }

		public string StateAttribute4 { get; set; }

		public string StateAttribute5 { get; set; }

		public string Exception { get; set; }

		public static string GenerateResultName(string serviceName, string component, string tag = null)
		{
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentException("String argument cannot be null or empty", "serviceName");
			}
			if (string.IsNullOrEmpty(component))
			{
				throw new ArgumentException("String argument cannot be null or empty", "component");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}/{1}", serviceName, component);
			if (!string.IsNullOrEmpty(tag))
			{
				stringBuilder.AppendFormat("/{0}", tag);
			}
			return stringBuilder.ToString();
		}

		public void AddCustomProperty(string name, object value)
		{
			this.CustomProperties.Add(name, value.ToString());
		}

		public void Publish(bool throwOnError = false)
		{
			string extensionXml = CrimsonHelper.ConvertDictionaryToXml(this.CustomProperties);
			ProbeResult probeResult = new ProbeResult();
			probeResult.ResultName = this.ResultName;
			probeResult.SampleValue = this.SampleValue;
			probeResult.ServiceName = this.ServiceName;
			probeResult.IsNotified = true;
			probeResult.ExecutionStartTime = (probeResult.ExecutionEndTime = this.TimeStamp);
			probeResult.Error = this.Message;
			probeResult.ExtensionXml = extensionXml;
			probeResult.StateAttribute1 = this.StateAttribute1;
			probeResult.StateAttribute2 = this.StateAttribute2;
			probeResult.StateAttribute3 = this.StateAttribute3;
			probeResult.StateAttribute4 = this.StateAttribute4;
			probeResult.StateAttribute5 = this.StateAttribute5;
			probeResult.Exception = this.Exception;
			probeResult.WorkItemId = DefinitionIdGenerator<ProbeDefinition>.GetIdForNotification(this.ResultName);
			switch (this.Severity)
			{
			case ResultSeverityLevel.Informational:
			case ResultSeverityLevel.Verbose:
				probeResult.ResultType = ResultType.Succeeded;
				break;
			default:
				probeResult.ResultType = ResultType.Failed;
				break;
			}
			try
			{
				probeResult.Write(null);
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceDebug<Exception>(WTFLog.DataAccess, TracingContext.Default, "Notification publishing failed with exception {0}", arg, null, "Publish", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\NotificationItem.cs", 254);
				if (throwOnError)
				{
					throw;
				}
			}
		}
	}
}
