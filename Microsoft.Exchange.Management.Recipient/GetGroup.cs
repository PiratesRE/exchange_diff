using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "Group", DefaultParameterSetName = "Identity")]
	public sealed class GetGroup : GetRecipientBase<GroupIdParameter, ADGroup>
	{
		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetGroup.SortPropertiesArray;
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return GetGroup.RecipientTypesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return this.RecipientTypeDetails;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<WindowsGroupSchema>();
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetGroup.AllowedRecipientTypeDetails, value);
				base.Fields["RecipientTypeDetails"] = value;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return WindowsGroup.FromDataObject((ADGroup)dataObject);
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailNonUniversalGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailUniversalDistributionGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailUniversalSecurityGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.NonUniversalGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UniversalDistributionGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UniversalSecurityGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomList
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName
		};

		private static readonly RecipientType[] RecipientTypesArray = new RecipientType[]
		{
			RecipientType.Group,
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup
		};
	}
}
