using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPException : LocalizedException
	{
		public IMAPException(string failureReason) : base(Strings.IMAPException(failureReason))
		{
			this.failureReason = failureReason;
		}

		public IMAPException(string failureReason, Exception innerException) : base(Strings.IMAPException(failureReason), innerException)
		{
			this.failureReason = failureReason;
		}

		protected IMAPException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failureReason = (string)info.GetValue("failureReason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failureReason", this.failureReason);
		}

		public string FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		private readonly string failureReason;
	}
}
