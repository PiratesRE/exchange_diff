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
	[Cmdlet("Get", "LinkedUser", DefaultParameterSetName = "Identity")]
	public sealed class GetLinkedUser : GetRecipientBase<UserIdParameter, ADUser>
	{
		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetLinkedUser.SortPropertiesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetLinkedUser.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADUser aduser = (ADUser)dataObject;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			return new LinkedUser(aduser);
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<LinkedUserSchema>();
			}
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.LinkedUser
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			OrgPersonPresentationObjectSchema.DisplayName,
			OrgPersonPresentationObjectSchema.FirstName,
			OrgPersonPresentationObjectSchema.LastName,
			OrgPersonPresentationObjectSchema.Office,
			OrgPersonPresentationObjectSchema.City
		};
	}
}
