using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TlsFailureException : LocalizedException
	{
		public TlsFailureException(string failureReason) : base(Strings.TlsFailureException(failureReason))
		{
			this.failureReason = failureReason;
		}

		public TlsFailureException(string failureReason, Exception innerException) : base(Strings.TlsFailureException(failureReason), innerException)
		{
			this.failureReason = failureReason;
		}

		protected TlsFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
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
