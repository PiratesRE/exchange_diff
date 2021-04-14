using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedSyncProtocolException : MailboxReplicationPermanentException
	{
		public UnsupportedSyncProtocolException(string protocol) : base(MrsStrings.UnsupportedSyncProtocol(protocol))
		{
			this.protocol = protocol;
		}

		public UnsupportedSyncProtocolException(string protocol, Exception innerException) : base(MrsStrings.UnsupportedSyncProtocol(protocol), innerException)
		{
			this.protocol = protocol;
		}

		protected UnsupportedSyncProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.protocol = (string)info.GetValue("protocol", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("protocol", this.protocol);
		}

		public string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		private readonly string protocol;
	}
}
