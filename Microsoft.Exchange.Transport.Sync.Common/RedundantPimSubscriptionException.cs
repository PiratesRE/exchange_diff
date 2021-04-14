using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RedundantPimSubscriptionException : LocalizedException
	{
		public RedundantPimSubscriptionException(string emailAddress) : base(Strings.RedundantPimSubscription(emailAddress))
		{
			this.emailAddress = emailAddress;
		}

		public RedundantPimSubscriptionException(string emailAddress, Exception innerException) : base(Strings.RedundantPimSubscription(emailAddress), innerException)
		{
			this.emailAddress = emailAddress;
		}

		protected RedundantPimSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.emailAddress = (string)info.GetValue("emailAddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("emailAddress", this.emailAddress);
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		private readonly string emailAddress;
	}
}
