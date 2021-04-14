using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientAccessRulesLogEvent : ILogEvent
	{
		internal ClientAccessRulesLogEvent(OrganizationId orgId, string username, IPEndPoint endpoint, ClientAccessAuthenticationMethod authenticationType, string blockingRuleName, double latency, bool blocked)
		{
			ActivityContext.ActivityId.FormatForLog();
			this.datapointProperties = new Dictionary<string, object>
			{
				{
					"ORGID",
					orgId.ToString()
				},
				{
					"USER",
					username
				},
				{
					"IP",
					endpoint.Address.ToString()
				},
				{
					"PORT",
					endpoint.Port.ToString()
				},
				{
					"AUTHTYPE",
					authenticationType.ToString()
				},
				{
					"RULE",
					blockingRuleName
				},
				{
					"LATENCY",
					latency.ToString()
				},
				{
					"BLOCKED",
					blocked.ToString()
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
