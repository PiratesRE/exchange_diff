using System;
using System.IO;
using System.Management.Automation;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class UMBasePromptService : DataSourceService, IUploadHandler
	{
		public Type SetParameterType
		{
			get
			{
				return typeof(UploadUMParameter);
			}
		}

		public int MaxFileSize
		{
			get
			{
				return 5242880;
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Import-UMPrompt?PromptFileStream&PromptFileName@W:Organization")]
		public PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters param)
		{
			param.FaultIfNull();
			UploadUMParameter uploadUMParameter = (UploadUMParameter)param;
			uploadUMParameter.PromptFileStream = context.FileStream;
			uploadUMParameter.PromptFileName = Path.GetFileName(context.FileName);
			if (uploadUMParameter.UMAutoAttendant == null && uploadUMParameter.UMDialPlan == null)
			{
				uploadUMParameter.UMAutoAttendant.FaultIfNull();
			}
			return this.ImportObject(uploadUMParameter);
		}

		private PowerShellResults ImportObject(UploadUMParameter parameters)
		{
			Identity translationIdentity = (parameters.UMAutoAttendant != null) ? parameters.UMAutoAttendant : parameters.UMDialPlan;
			return base.Invoke(new PSCommand().AddCommand("Import-UMPrompt"), translationIdentity, parameters);
		}

		private const string ProcessUploadRole = "Import-UMPrompt?PromptFileStream&PromptFileName@W:Organization";
	}
}
