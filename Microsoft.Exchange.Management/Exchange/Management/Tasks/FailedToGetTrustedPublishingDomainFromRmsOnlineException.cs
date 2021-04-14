using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetTrustedPublishingDomainFromRmsOnlineException : LocalizedException
	{
		public FailedToGetTrustedPublishingDomainFromRmsOnlineException(Exception e, Exception inner) : base(Strings.RmsOnlineFailedToGetTpd(e, inner))
		{
			this.e = e;
			this.inner = inner;
		}

		public FailedToGetTrustedPublishingDomainFromRmsOnlineException(Exception e, Exception inner, Exception innerException) : base(Strings.RmsOnlineFailedToGetTpd(e, inner), innerException)
		{
			this.e = e;
			this.inner = inner;
		}

		protected FailedToGetTrustedPublishingDomainFromRmsOnlineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.e = (Exception)info.GetValue("e", typeof(Exception));
			this.inner = (Exception)info.GetValue("inner", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("e", this.e);
			info.AddValue("inner", this.inner);
		}

		public Exception E
		{
			get
			{
				return this.e;
			}
		}

		public Exception Inner
		{
			get
			{
				return this.inner;
			}
		}

		private readonly Exception e;

		private readonly Exception inner;
	}
}
