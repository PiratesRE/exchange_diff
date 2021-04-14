using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSInstanceFailedPermanentException : MailboxReplicationPermanentException
	{
		public MRSInstanceFailedPermanentException(string mrsInstance, LocalizedString exceptionMessage) : base(MrsStrings.MRSInstanceFailed(mrsInstance, exceptionMessage))
		{
			this.mrsInstance = mrsInstance;
			this.exceptionMessage = exceptionMessage;
		}

		public MRSInstanceFailedPermanentException(string mrsInstance, LocalizedString exceptionMessage, Exception innerException) : base(MrsStrings.MRSInstanceFailed(mrsInstance, exceptionMessage), innerException)
		{
			this.mrsInstance = mrsInstance;
			this.exceptionMessage = exceptionMessage;
		}

		protected MRSInstanceFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mrsInstance = (string)info.GetValue("mrsInstance", typeof(string));
			this.exceptionMessage = (LocalizedString)info.GetValue("exceptionMessage", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mrsInstance", this.mrsInstance);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string MrsInstance
		{
			get
			{
				return this.mrsInstance;
			}
		}

		public LocalizedString ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string mrsInstance;

		private readonly LocalizedString exceptionMessage;
	}
}
