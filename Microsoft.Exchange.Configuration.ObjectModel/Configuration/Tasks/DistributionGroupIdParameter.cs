using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DistributionGroupIdParameter : RecipientIdParameter
	{
		public DistributionGroupIdParameter(string identity) : base(identity)
		{
		}

		public DistributionGroupIdParameter()
		{
		}

		public DistributionGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DistributionGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DistributionGroupIdParameter(DistributionGroup dl) : base(dl.Id)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return DistributionGroupIdParameter.AllowedRecipientTypes;
			}
		}

		public new static DistributionGroupIdParameter Parse(string identity)
		{
			return new DistributionGroupIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeDistributionGroup(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup
		};
	}
}
