using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedCommunicationException : ThirdPartyReplicationException
	{
		public FailedCommunicationException(string reason) : base(ThirdPartyReplication.FailedCommunication(reason))
		{
			this.reason = reason;
		}

		public FailedCommunicationException(string reason, Exception innerException) : base(ThirdPartyReplication.FailedCommunication(reason), innerException)
		{
			this.reason = reason;
		}

		protected FailedCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
