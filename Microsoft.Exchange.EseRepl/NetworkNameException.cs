using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkNameException : TransientException
	{
		public NetworkNameException(string netName) : base(Strings.NetworkNameNotFound(netName))
		{
			this.netName = netName;
		}

		public NetworkNameException(string netName, Exception innerException) : base(Strings.NetworkNameNotFound(netName), innerException)
		{
			this.netName = netName;
		}

		protected NetworkNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.netName = (string)info.GetValue("netName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("netName", this.netName);
		}

		public string NetName
		{
			get
			{
				return this.netName;
			}
		}

		private readonly string netName;
	}
}
