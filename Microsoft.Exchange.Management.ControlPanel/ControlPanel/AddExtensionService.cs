using System;
using System.Management.Automation;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AddExtensionService : DataSourceService, IUploadHandler
	{
		public virtual Type SetParameterType
		{
			get
			{
				return typeof(UploadExtensionParameter);
			}
		}

		public int MaxFileSize
		{
			get
			{
				return 10485760;
			}
		}

		protected virtual void AddParameters(PSCommand installCommand, WebServiceParameters param)
		{
			installCommand.AddParameters(param);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-App?FileStream@W:Self")]
		public PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters param)
		{
			param.FaultIfNull();
			UploadExtensionParameter uploadExtensionParameter = (UploadExtensionParameter)param;
			uploadExtensionParameter.FileStream = context.FileStream;
			if (RbacPrincipal.Current.RbacConfiguration.HasRoleOfType(RoleType.MyReadWriteMailboxApps))
			{
				uploadExtensionParameter.AllowReadWriteMailbox = new SwitchParameter(true);
			}
			PSCommand pscommand = new PSCommand().AddCommand("New-App");
			this.AddParameters(pscommand, param);
			return base.Invoke<ExtensionRow>(pscommand);
		}

		public const string NewApp = "New-App";

		public const int MaxExtensionSize = 10;

		public const int BytesInMB = 1048576;

		private const string ProcessUploadRole = "New-App?FileStream@W:Self";
	}
}
