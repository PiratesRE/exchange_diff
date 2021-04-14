using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcHttpModules
{
	internal class RpcHttpLogEvent : ILogEvent
	{
		public RpcHttpLogEvent(string stageProperty)
		{
			this.rpcHttpLogAttributes = new SortedDictionary<RpcHttpLogEvent.LoggingAttribute, string>();
			this.Stage = stageProperty;
		}

		public string EventId
		{
			get
			{
				return "RpcHttp";
			}
		}

		public string Stage
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.Stage);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.Stage, value);
			}
		}

		public string UserName
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.UserName);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.UserName, value);
			}
		}

		public string OutlookSessionId
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.OutlookSessionId);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.OutlookSessionId, value);
			}
		}

		public string AuthType
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.AuthType);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.AuthType, value);
			}
		}

		public string Status
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.Status);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.Status, value);
			}
		}

		public string HttpVerb
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.HttpVerb);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.HttpVerb, value);
			}
		}

		public string UriQueryString
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.UriQueryString);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.UriQueryString, value);
			}
		}

		public string RpcHttpUserName
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.RpcHttpUserName);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.RpcHttpUserName, value);
			}
		}

		public string ServerTarget
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.ServerTarget);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.ServerTarget, value);
			}
		}

		public string FEServer
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.FEServer);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.FEServer, value);
			}
		}

		public string RequestId
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.RequestId);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.RequestId, value);
			}
		}

		public string AssociationGuid
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.AssociationGuid);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.AssociationGuid, value);
			}
		}

		public string ClientIp
		{
			get
			{
				return this.GetAttribute(RpcHttpLogEvent.LoggingAttribute.ClientIp);
			}
			set
			{
				this.SetAttribute(RpcHttpLogEvent.LoggingAttribute.ClientIp, value);
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			KeyValuePair<string, object>[] array = new KeyValuePair<string, object>[this.rpcHttpLogAttributes.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<RpcHttpLogEvent.LoggingAttribute, string> keyValuePair in this.rpcHttpLogAttributes)
			{
				array[num++] = new KeyValuePair<string, object>(keyValuePair.Key.ToString(), keyValuePair.Value);
			}
			return array;
		}

		private string GetAttribute(RpcHttpLogEvent.LoggingAttribute attribute)
		{
			string result = null;
			if (this.rpcHttpLogAttributes.TryGetValue(attribute, out result))
			{
				return result;
			}
			return null;
		}

		private void SetAttribute(RpcHttpLogEvent.LoggingAttribute attribute, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.rpcHttpLogAttributes[attribute] = value;
			}
		}

		private readonly SortedDictionary<RpcHttpLogEvent.LoggingAttribute, string> rpcHttpLogAttributes;

		internal enum LoggingAttribute
		{
			Stage,
			UserName,
			OutlookSessionId,
			AuthType,
			Status,
			HttpVerb,
			UriQueryString,
			RpcHttpUserName,
			ServerTarget,
			FEServer,
			RequestId,
			AssociationGuid,
			ClientIp
		}
	}
}
