using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapException : LocalizedException
	{
		public ImapException(string failureReason) : base(CXStrings.ImapErrorMsg(failureReason))
		{
			this.failureReason = failureReason;
		}

		public ImapException(string failureReason, Exception innerException) : base(CXStrings.ImapErrorMsg(failureReason), innerException)
		{
			this.failureReason = failureReason;
		}

		protected ImapException(SerializationInfo info, StreamingContext context) : base(info, context)
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
