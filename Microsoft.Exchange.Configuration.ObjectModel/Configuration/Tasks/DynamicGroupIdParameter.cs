using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DynamicGroupIdParameter : RecipientIdParameter
	{
		public DynamicGroupIdParameter(string identity) : base(identity)
		{
		}

		public DynamicGroupIdParameter()
		{
		}

		public DynamicGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DynamicGroupIdParameter(DynamicDistributionGroup ddl) : base(ddl.Id)
		{
		}

		public DynamicGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return DynamicGroupIdParameter.AllowedRecipientTypes;
			}
		}

		public new static DynamicGroupIdParameter Parse(string identity)
		{
			return new DynamicGroupIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeDynamicGroup(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.DynamicDistributionGroup
		};
	}
}
