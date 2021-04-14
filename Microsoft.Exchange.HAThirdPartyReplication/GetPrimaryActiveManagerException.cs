using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetPrimaryActiveManagerException : ThirdPartyReplicationException
	{
		public GetPrimaryActiveManagerException(string reason) : base(ThirdPartyReplication.GetPamError(reason))
		{
			this.reason = reason;
		}

		public GetPrimaryActiveManagerException(string reason, Exception innerException) : base(ThirdPartyReplication.GetPamError(reason), innerException)
		{
			this.reason = reason;
		}

		protected GetPrimaryActiveManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
