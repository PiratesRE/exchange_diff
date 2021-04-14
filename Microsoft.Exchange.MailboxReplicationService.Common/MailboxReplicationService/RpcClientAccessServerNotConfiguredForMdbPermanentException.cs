using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RpcClientAccessServerNotConfiguredForMdbPermanentException : MailboxReplicationPermanentException
	{
		public RpcClientAccessServerNotConfiguredForMdbPermanentException(string mdbID) : base(MrsStrings.RpcClientAccessServerNotConfiguredForMdb(mdbID))
		{
			this.mdbID = mdbID;
		}

		public RpcClientAccessServerNotConfiguredForMdbPermanentException(string mdbID, Exception innerException) : base(MrsStrings.RpcClientAccessServerNotConfiguredForMdb(mdbID), innerException)
		{
			this.mdbID = mdbID;
		}

		protected RpcClientAccessServerNotConfiguredForMdbPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbID = (string)info.GetValue("mdbID", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbID", this.mdbID);
		}

		public string MdbID
		{
			get
			{
				return this.mdbID;
			}
		}

		private readonly string mdbID;
	}
}
