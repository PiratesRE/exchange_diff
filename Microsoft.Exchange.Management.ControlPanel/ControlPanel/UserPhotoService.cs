using System;
using System.IO;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class UserPhotoService : DataSourceService, IUploadHandler, IUserPhotoService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Set-UserPhoto?Identity@W:Self")]
		public PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters param)
		{
			param.FaultIfNull();
			SetUserPhotoParameters setUserPhotoParameters = (SetUserPhotoParameters)param;
			setUserPhotoParameters.PhotoStream = context.FileStream;
			Identity identity = Identity.ParseIdentity(setUserPhotoParameters.Identity);
			if (identity == null || string.IsNullOrEmpty(identity.RawIdentity))
			{
				throw new BadQueryParameterException("Identity");
			}
			return this.SetPhoto(identity, context.FileStream);
		}

		public Type SetParameterType
		{
			get
			{
				return typeof(SetUserPhotoParameters);
			}
		}

		public int MaxFileSize
		{
			get
			{
				return 20971520;
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UserPhoto?Identity@R:Self")]
		public PowerShellResults<UserPhoto> GetSavedPhoto(Identity identity)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Get-UserPhoto");
			return base.GetObject<UserPhoto>(pscommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UserPhoto?Identity@R:Self")]
		public PowerShellResults<UserPhoto> GetPreviewPhoto(Identity identity)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Get-UserPhoto");
			pscommand.AddParameters(new GetUserPhotoParameters
			{
				Preview = new SwitchParameter(true)
			});
			return base.GetObject<UserPhoto>(pscommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-UserPhoto?Identity@W:Self")]
		public PowerShellResults SetPhoto(Identity identity, Stream stream)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Set-UserPhoto");
			SetUserPhotoParameters setUserPhotoParameters = new SetUserPhotoParameters();
			setUserPhotoParameters.PhotoStream = stream;
			setUserPhotoParameters.Preview = new SwitchParameter(true);
			return base.Invoke(pscommand, new Identity[]
			{
				identity
			}, setUserPhotoParameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-UserPhoto?Identity@W:Self")]
		public PowerShellResults SavePhoto(Identity identity)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Set-UserPhoto");
			SetUserPhotoParameters setUserPhotoParameters = new SetUserPhotoParameters();
			setUserPhotoParameters.Save = new SwitchParameter(true);
			return base.Invoke(pscommand, new Identity[]
			{
				identity
			}, setUserPhotoParameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-UserPhoto?Identity@W:Self")]
		public PowerShellResults CancelPhoto(Identity identity)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Set-UserPhoto");
			SetUserPhotoParameters setUserPhotoParameters = new SetUserPhotoParameters();
			setUserPhotoParameters.Cancel = new SwitchParameter(true);
			return base.Invoke(pscommand, new Identity[]
			{
				identity
			}, setUserPhotoParameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-UserPhoto?Identity@W:Self")]
		public PowerShellResults RemovePhoto(Identity identity)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Remove-UserPhoto");
			RemoveUserPhotoParameters parameters = new RemoveUserPhotoParameters();
			return base.Invoke(pscommand, new Identity[]
			{
				identity
			}, parameters);
		}

		internal const string GetCmdlet = "Get-UserPhoto";

		internal const string RemoveCmdlet = "Remove-UserPhoto";

		internal const string SetCmdlet = "Set-UserPhoto";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const int MaxFileSizeAllowed = 20971520;

		private const string Noun = "UserPhoto";

		private const string GetUserPhotoRole = "Get-UserPhoto?Identity@R:Self";

		private const string RemoveUserPhotoRole = "Remove-UserPhoto?Identity@W:Self";

		private const string SetUserPhotoRole = "Set-UserPhoto?Identity@W:Self";
	}
}
