using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.Authentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOAuthLinkedAccountException : LocalizedException
	{
		public InvalidOAuthLinkedAccountException(string partnerApplication, string linkedAccount) : base(SecurityStrings.InvalidOAuthLinkedAccountException(partnerApplication, linkedAccount))
		{
			this.partnerApplication = partnerApplication;
			this.linkedAccount = linkedAccount;
		}

		public InvalidOAuthLinkedAccountException(string partnerApplication, string linkedAccount, Exception innerException) : base(SecurityStrings.InvalidOAuthLinkedAccountException(partnerApplication, linkedAccount), innerException)
		{
			this.partnerApplication = partnerApplication;
			this.linkedAccount = linkedAccount;
		}

		protected InvalidOAuthLinkedAccountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.partnerApplication = (string)info.GetValue("partnerApplication", typeof(string));
			this.linkedAccount = (string)info.GetValue("linkedAccount", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("partnerApplication", this.partnerApplication);
			info.AddValue("linkedAccount", this.linkedAccount);
		}

		public string PartnerApplication
		{
			get
			{
				return this.partnerApplication;
			}
		}

		public string LinkedAccount
		{
			get
			{
				return this.linkedAccount;
			}
		}

		private readonly string partnerApplication;

		private readonly string linkedAccount;
	}
}
