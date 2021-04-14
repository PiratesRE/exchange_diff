using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkNotUsableException : TransientException
	{
		public NetworkNotUsableException(string netName, string nodeName, string reason) : base(Strings.NetworkNotUsable(netName, nodeName, reason))
		{
			this.netName = netName;
			this.nodeName = nodeName;
			this.reason = reason;
		}

		public NetworkNotUsableException(string netName, string nodeName, string reason, Exception innerException) : base(Strings.NetworkNotUsable(netName, nodeName, reason), innerException)
		{
			this.netName = netName;
			this.nodeName = nodeName;
			this.reason = reason;
		}

		protected NetworkNotUsableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.netName = (string)info.GetValue("netName", typeof(string));
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("netName", this.netName);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("reason", this.reason);
		}

		public string NetName
		{
			get
			{
				return this.netName;
			}
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string netName;

		private readonly string nodeName;

		private readonly string reason;
	}
}
