using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDeSerializeImportedTrustedPublishingDomainException : LocalizedException
	{
		public FailedToDeSerializeImportedTrustedPublishingDomainException(Exception ex) : base(Strings.FailedToDeSerializeImportedTrustedPublishingDomain(ex))
		{
			this.ex = ex;
		}

		public FailedToDeSerializeImportedTrustedPublishingDomainException(Exception ex, Exception innerException) : base(Strings.FailedToDeSerializeImportedTrustedPublishingDomain(ex), innerException)
		{
			this.ex = ex;
		}

		protected FailedToDeSerializeImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ex", this.ex);
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly Exception ex;
	}
}
