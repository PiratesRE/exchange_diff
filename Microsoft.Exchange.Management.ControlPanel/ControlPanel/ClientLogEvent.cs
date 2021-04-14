using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientLogEvent : ILogEvent
	{
		internal ClientLogEvent(Datapoint datapoint)
		{
			this.datapoint = datapoint;
			LocalSession localSession = RbacPrincipal.GetCurrent(false) as LocalSession;
			string value = (localSession == null) ? string.Empty : localSession.LogonTypeFlag;
			this.datapointProperties = new Dictionary<string, object>
			{
				{
					"TIME",
					this.datapoint.Time
				},
				{
					"SID",
					HttpContext.Current.GetSessionID()
				},
				{
					"USID",
					(HttpContext.Current.User is RbacSession) ? HttpContext.Current.GetCachedUserUniqueKey() : string.Empty
				},
				{
					"SRC",
					this.datapoint.Src
				},
				{
					"REQID",
					this.datapoint.ReqId
				},
				{
					"IP",
					this.GetClientIP()
				},
				{
					"UA",
					HttpUtility.UrlEncode(HttpContext.Current.Request.UserAgent)
				},
				{
					"BLD",
					ExchangeSetupContext.InstalledVersion.ToString()
				},
				{
					"LTYPE",
					value
				}
			};
		}

		public string EventId
		{
			get
			{
				return RbacContext.ClientAppId.ToString() + "." + this.datapoint.Name;
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.datapointProperties;
		}

		private string GetClientIP()
		{
			string text = HttpContext.Current.Request.Headers["X-Forwarded-For"];
			if (string.IsNullOrEmpty(text))
			{
				text = HttpContext.Current.Request.UserHostAddress;
			}
			return text;
		}

		private readonly Datapoint datapoint;

		private Dictionary<string, object> datapointProperties;
	}
}
