using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "MailPublicFolder", DefaultParameterSetName = "Identity")]
	public sealed class GetMailPublicFolder : GetRecipientBase<MailPublicFolderIdParameter, ADPublicFolder>
	{
		private new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["OrganizationalUnit"];
			}
			set
			{
				base.Fields["OrganizationalUnit"] = value;
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMailPublicFolder.SortPropertiesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetMailPublicFolder.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailPublicFolder.FromDataObject((ADPublicFolder)dataObject);
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ADPublicFolderSchema>();
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (MapiTaskHelper.IsDatacenter)
			{
				OrganizationIdParameter organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(base.Organization, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				return MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			}
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return false;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StoragePermanentException || base.IsKnownException(exception);
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.PublicFolder
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.Alias,
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName
		};
	}
}
