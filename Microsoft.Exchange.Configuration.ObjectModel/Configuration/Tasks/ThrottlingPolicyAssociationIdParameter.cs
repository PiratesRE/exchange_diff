using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ThrottlingPolicyAssociationIdParameter : RecipientIdParameter
	{
		public ThrottlingPolicyAssociationIdParameter()
		{
		}

		public ThrottlingPolicyAssociationIdParameter(string identity) : base(identity)
		{
		}

		public ThrottlingPolicyAssociationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ThrottlingPolicyAssociationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public new static ThrottlingPolicyAssociationIdParameter Parse(string identity)
		{
			return new ThrottlingPolicyAssociationIdParameter(identity);
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return ThrottlingPolicyAssociationIdParameter.AllowedRecipientTypes;
			}
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.MailContact,
			RecipientType.Computer
		};
	}
}
