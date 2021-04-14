using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SecurityGroupIdParameter : SecurityPrincipalIdParameter
	{
		public SecurityGroupIdParameter(string identity) : base(identity)
		{
		}

		public SecurityGroupIdParameter()
		{
		}

		public SecurityGroupIdParameter(DistributionGroup group) : base(group.Id)
		{
		}

		public SecurityGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return SecurityGroupIdParameter.AllowedRecipientTypes;
			}
		}

		public new static SecurityGroupIdParameter Parse(string identity)
		{
			return new SecurityGroupIdParameter(identity);
		}

		protected override SecurityPrincipalIdParameter CreateSidParameter(string identity)
		{
			return new SecurityGroupIdParameter(identity);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Group,
			RecipientType.MailUniversalSecurityGroup
		};
	}
}
