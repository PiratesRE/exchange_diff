using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class GroupIdParameter : RecipientIdParameter
	{
		public GroupIdParameter(string identity) : base(identity)
		{
		}

		public GroupIdParameter()
		{
		}

		public GroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public GroupIdParameter(WindowsGroup group) : base(group.Id)
		{
		}

		public GroupIdParameter(DistributionGroup group) : base(group.Id)
		{
		}

		public GroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GroupIdParameter.AllowedRecipientTypes;
			}
		}

		public new static GroupIdParameter Parse(string identity)
		{
			return new GroupIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeGroup(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Group,
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup
		};
	}
}
