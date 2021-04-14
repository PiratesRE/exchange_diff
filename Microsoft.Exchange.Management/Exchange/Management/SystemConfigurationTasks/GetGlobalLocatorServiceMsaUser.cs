using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "GlobalLocatorServiceMsaUser", DefaultParameterSetName = "MsaUserNetIDParameterSet")]
	public sealed class GetGlobalLocatorServiceMsaUser : ManageGlobalLocatorServiceBase
	{
		private new Guid ExternalDirectoryOrganizationId { get; set; }

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

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			GlobalLocatorServiceMsaUser globalLocatorServiceMsaUser = new GlobalLocatorServiceMsaUser
			{
				MsaUserNetId = this.MsaUserNetId
			};
			string text = globalLocatorServiceMsaUser.MsaUserNetId.ToString();
			string address = null;
			Guid externalDirectoryOrganizationId;
			string resourceForest;
			string accountForest;
			string tenantContainerCN;
			if (!glsDirectorySession.TryGetTenantForestsByMSAUserNetID(text, out externalDirectoryOrganizationId, out resourceForest, out accountForest, out tenantContainerCN) || !glsDirectorySession.TryGetMSAUserMemberName(text, out address))
			{
				base.WriteGlsMsaUserNotFoundError(text);
			}
			globalLocatorServiceMsaUser.MsaUserMemberName = SmtpAddress.Parse(address);
			globalLocatorServiceMsaUser.ExternalDirectoryOrganizationId = externalDirectoryOrganizationId;
			globalLocatorServiceMsaUser.ResourceForest = resourceForest;
			globalLocatorServiceMsaUser.AccountForest = accountForest;
			globalLocatorServiceMsaUser.TenantContainerCN = tenantContainerCN;
			base.WriteObject(globalLocatorServiceMsaUser);
		}
	}
}
