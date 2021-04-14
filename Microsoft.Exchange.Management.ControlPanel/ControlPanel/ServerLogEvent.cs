using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ServerLogEvent : ILogEvent
	{
		internal ServerLogEvent(PSCommand psCommand, IEnumerable pipelineInput, int requestLatency, string requestId, string exception, int resultSize)
		{
			string value = ActivityContext.ActivityId.FormatForLog();
			this.datapointProperties = new Dictionary<string, object>
			{
				{
					"TIME",
					requestLatency.ToString()
				},
				{
					"SID",
					HttpContext.Current.GetSessionID()
				},
				{
					"CMD",
					PSCommandExtension.ToLogString(psCommand, pipelineInput)
				},
				{
					"REQID",
					requestId
				},
				{
					"URL",
					HttpContext.Current.Request.RawUrl
				},
				{
					"EX",
					exception
				},
				{
					"ACTID",
					value
				},
				{
					"RS",
					resultSize.ToString()
				},
				{
					"BLD",
					ExchangeSetupContext.InstalledVersion.ToString()
				}
			};
		}

		public string EventId
		{
			get
			{
				string str = ActivityContextLoggerId.Request.ToString();
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					string property = currentActivityScope.GetProperty(ExtensibleLoggerMetadata.EventId);
					if (!string.IsNullOrEmpty(property))
					{
						str = property;
					}
				}
				return this.clientAppId + "." + str;
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.datapointProperties;
		}

		private Dictionary<string, object> datapointProperties;

		private readonly string clientAppId = RbacContext.ClientAppId.ToString();
	}
}
