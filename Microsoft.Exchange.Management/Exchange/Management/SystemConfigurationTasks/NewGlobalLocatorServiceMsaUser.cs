using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "GlobalLocatorServiceMsaUser", DefaultParameterSetName = "MsaUserNetIDParameterSet", SupportsShouldProcess = true)]
	public sealed class NewGlobalLocatorServiceMsaUser : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MsaUserNetIDParameterSet")]
		public new Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			}
			set
			{
				base.Fields["ExternalDirectoryOrganizationId"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = true)]
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
				return Strings.ConfirmationMessageNewGls("MsaUser", this.MsaUserNetId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			string text = this.MsaUserNetId.ToString();
			if (glsDirectorySession.MSAUserExists(text))
			{
				base.WriteGlsMsaUserAlreadyExistsError(text);
			}
			Guid externalDirectoryOrganizationId = this.ExternalDirectoryOrganizationId;
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(externalDirectoryOrganizationId, out globalLocatorServiceTenant))
			{
				base.WriteGlsTenantNotFoundError(externalDirectoryOrganizationId);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			GlobalLocatorServiceMsaUser globalLocatorServiceMsaUser = new GlobalLocatorServiceMsaUser
			{
				ExternalDirectoryOrganizationId = this.ExternalDirectoryOrganizationId,
				MsaUserMemberName = this.MsaUserMemberName,
				MsaUserNetId = this.MsaUserNetId
			};
			glsDirectorySession.AddMSAUser(globalLocatorServiceMsaUser.MsaUserNetId.ToString(), globalLocatorServiceMsaUser.MsaUserMemberName.ToString(), globalLocatorServiceMsaUser.ExternalDirectoryOrganizationId);
			base.WriteObject(globalLocatorServiceMsaUser);
		}
	}
}
