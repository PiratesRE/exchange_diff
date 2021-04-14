using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "GlobalLocatorServiceMsaUser", DefaultParameterSetName = "MsaUserNetIDParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveGlobalLocatorServiceMsaUser : ManageGlobalLocatorServiceBase
	{
		private new Guid ExternalDirectoryOrganizationId { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "MsaUserNetIDParameterSet")]
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
				return Strings.ConfirmationMessageRemoveGls("MsaUser", this.MsaUserNetId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			string text = this.MsaUserNetId.ToString();
			if (!glsDirectorySession.MSAUserExists(text))
			{
				base.WriteGlsMsaUserNotFoundError(text);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			glsDirectorySession.RemoveMSAUser(this.MsaUserNetId.ToString());
		}
	}
}
