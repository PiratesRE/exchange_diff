using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "GlobalLocatorServiceMsaUser", DefaultParameterSetName = "MsaUserNetIDParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetGlobalLocatorServiceMsaUser : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "MsaUserNetIDParameterSet")]
		public SmtpAddress MsaUserMemberName
		{
			get
			{
				return (SmtpAddress)base.Fields["MsaUserMemberName"];
			}
			set
			{
				base.Fields["MsaUserMemberName"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "MsaUserNetIDParameterSet")]
		[ValidateNotNullOrEmpty]
		public NetID MsaUserNetId
		{
			get
			{
				return (NetID)base.Fields["MsaUserNetID"];
			}
			set
			{
				base.Fields["MsaUserNetID"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetGls("MsaUser", this.MsaUserNetId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			if (!base.Fields.IsModified("ExternalDirectoryOrganizationId") && !base.Fields.IsModified("MsaUserMemberName"))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorNoPropertyWasModified), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				Guid externalDirectoryOrganizationId = base.ExternalDirectoryOrganizationId;
				GlobalLocatorServiceTenant globalLocatorServiceTenant;
				if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(externalDirectoryOrganizationId, out globalLocatorServiceTenant))
				{
					base.WriteGlsTenantNotFoundError(externalDirectoryOrganizationId);
				}
			}
			this.currentGlsMsaUser = new GlobalLocatorServiceMsaUser
			{
				MsaUserNetId = this.MsaUserNetId
			};
			string address = null;
			string text = this.MsaUserNetId.ToString();
			Guid externalDirectoryOrganizationId2;
			string resourceForest;
			string accountForest;
			string tenantContainerCN;
			if (!glsDirectorySession.TryGetTenantForestsByMSAUserNetID(text, out externalDirectoryOrganizationId2, out resourceForest, out accountForest, out tenantContainerCN) || !glsDirectorySession.TryGetMSAUserMemberName(text, out address))
			{
				base.WriteGlsMsaUserNotFoundError(text);
			}
			this.currentGlsMsaUser.MsaUserMemberName = SmtpAddress.Parse(address);
			this.currentGlsMsaUser.ExternalDirectoryOrganizationId = externalDirectoryOrganizationId2;
			this.currentGlsMsaUser.ResourceForest = resourceForest;
			this.currentGlsMsaUser.AccountForest = accountForest;
			this.currentGlsMsaUser.TenantContainerCN = tenantContainerCN;
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			GlobalLocatorServiceMsaUser globalLocatorServiceMsaUser = new GlobalLocatorServiceMsaUser
			{
				MsaUserNetId = this.MsaUserNetId,
				ExternalDirectoryOrganizationId = (base.Fields.IsModified("ExternalDirectoryOrganizationId") ? base.ExternalDirectoryOrganizationId : this.currentGlsMsaUser.ExternalDirectoryOrganizationId),
				MsaUserMemberName = (base.Fields.IsModified("MsaUserMemberName") ? this.MsaUserMemberName : this.currentGlsMsaUser.MsaUserMemberName)
			};
			glsDirectorySession.UpdateMSAUser(globalLocatorServiceMsaUser.MsaUserNetId.ToString(), globalLocatorServiceMsaUser.MsaUserMemberName.ToString(), globalLocatorServiceMsaUser.ExternalDirectoryOrganizationId);
			base.WriteObject(globalLocatorServiceMsaUser);
		}

		private GlobalLocatorServiceMsaUser currentGlsMsaUser;
	}
}
