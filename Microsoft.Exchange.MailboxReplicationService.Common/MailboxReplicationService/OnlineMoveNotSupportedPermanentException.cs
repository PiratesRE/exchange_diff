using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OnlineMoveNotSupportedPermanentException : MailboxReplicationPermanentException
	{
		public OnlineMoveNotSupportedPermanentException(string mbxGuid) : base(MrsStrings.OnlineMoveNotSupported(mbxGuid))
		{
			this.mbxGuid = mbxGuid;
		}

		public OnlineMoveNotSupportedPermanentException(string mbxGuid, Exception innerException) : base(MrsStrings.OnlineMoveNotSupported(mbxGuid), innerException)
		{
			this.mbxGuid = mbxGuid;
		}

		protected OnlineMoveNotSupportedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxGuid = (string)info.GetValue("mbxGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxGuid", this.mbxGuid);
		}

		public string MbxGuid
		{
			get
			{
				return this.mbxGuid;
			}
		}

		private readonly string mbxGuid;
	}
}
