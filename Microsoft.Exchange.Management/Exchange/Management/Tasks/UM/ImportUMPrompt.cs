using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Import", "UMPrompt", DefaultParameterSetName = "UploadDialPlanPrompts", SupportsShouldProcess = true)]
	public sealed class ImportUMPrompt : UMPromptTaskBase<UMDialPlanIdParameter>
	{
		private new UMDialPlanIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPromptsStream")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPrompts")]
		public override UMAutoAttendantIdParameter UMAutoAttendant
		{
			get
			{
				return (UMAutoAttendantIdParameter)base.Fields["UMAutoAttendant"];
			}
			set
			{
				base.Fields["UMAutoAttendant"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPrompts")]
		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPromptsStream")]
		[ValidateNotNullOrEmpty]
		public override UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return this.Identity;
			}
			set
			{
				this.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPrompts")]
		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPromptsStream")]
		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPromptsStream")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPrompts")]
		public string PromptFileName
		{
			get
			{
				return (string)base.Fields["PromptFileName"];
			}
			set
			{
				base.Fields["PromptFileName"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPrompts")]
		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPrompts")]
		public byte[] PromptFileData
		{
			get
			{
				return (byte[])base.Fields["PromptFileData"];
			}
			set
			{
				base.Fields["PromptFileData"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadDialPlanPromptsStream")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "UploadAutoAttendantPromptsStream")]
		public Stream PromptFileStream
		{
			get
			{
				return (Stream)base.Fields["PromptFileStream"];
			}
			set
			{
				base.Fields["PromptFileStream"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("UploadAutoAttendantPrompts" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageImportUMPromptAutoAttendantPrompts(this.PromptFileName, this.UMAutoAttendant.ToString());
				}
				return Strings.ConfirmationMessageImportUMPromptDialPlanPrompts(this.PromptFileName, this.UMDialPlan.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (!ImportUMPrompt.IsDesktopExperienceRoleInstalled())
			{
				ServerIdParameter serverIdParameter = new ServerIdParameter();
				base.WriteError(new DesktopExperienceRequiredException(serverIdParameter.Fqdn), ErrorCategory.NotInstalled, null);
			}
			base.InternalValidate();
			long num = (this.PromptFileData != null) ? ((long)this.PromptFileData.Length) : this.PromptFileStream.Length;
			if (num > 5242880L)
			{
				this.HandleOversizeAudioData();
			}
			if (this.PromptFileName.Length > 128 || !string.Equals(Path.GetFileName(this.PromptFileName), this.PromptFileName))
			{
				base.WriteError(new InvalidFileNameException(128), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADConfigurationObject config = null;
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (parameterSetName == "UploadDialPlanPrompts" || parameterSetName == "UploadDialPlanPromptsStream")
				{
					config = this.DataObject;
					goto IL_7D;
				}
				if (parameterSetName == "UploadAutoAttendantPrompts" || parameterSetName == "UploadAutoAttendantPromptsStream")
				{
					config = base.AutoAttendant;
					goto IL_7D;
				}
			}
			ExAssert.RetailAssert(false, "Invalid parameter set {0}", new object[]
			{
				base.ParameterSetName
			});
			try
			{
				IL_7D:
				ITempFile tempFile = null;
				string extension = Path.GetExtension(this.PromptFileName);
				if (string.Equals(extension, ".wav", StringComparison.OrdinalIgnoreCase))
				{
					tempFile = TempFileFactory.CreateTempWavFile();
				}
				else
				{
					if (!string.Equals(extension, ".wma", StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidFileNameException(128);
					}
					tempFile = TempFileFactory.CreateTempWmaFile();
				}
				using (tempFile)
				{
					using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.Create, FileAccess.Write))
					{
						if (this.PromptFileData != null)
						{
							fileStream.Write(this.PromptFileData, 0, this.PromptFileData.Length);
						}
						else
						{
							CommonUtil.CopyStream(this.PromptFileStream, fileStream);
						}
					}
					using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession(Environment.UserName, config))
					{
						publishingSession.Upload(tempFile.FilePath, this.PromptFileName);
					}
				}
			}
			catch (UnsupportedCustomGreetingSizeFormatException)
			{
				this.HandleOversizeAudioData();
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, (ErrorCategory)1000, null);
			}
			catch (SystemException ex)
			{
				if (!this.HandleException(ex))
				{
					throw;
				}
				base.WriteError(ex, (ErrorCategory)1000, null);
			}
			TaskLogger.LogExit();
		}

		private void HandleOversizeAudioData()
		{
			base.WriteError(new AudioDataIsOversizeException(5, 5L), ErrorCategory.NotSpecified, null);
		}

		private bool HandleException(SystemException e)
		{
			return e is IOException || e is SecurityException || e is NotSupportedException || e is UnauthorizedAccessException;
		}

		private static bool IsDesktopExperienceRoleInstalled()
		{
			string systemDirectory = Environment.SystemDirectory;
			return !string.IsNullOrEmpty(systemDirectory) && Environment.OSVersion.Version.Major >= 6 && ImportUMPrompt.CheckVersionInfo(Path.Combine(systemDirectory, "wmvcore.dll"), 3802) && ImportUMPrompt.CheckVersionInfo(Path.Combine(systemDirectory, "wmspdmod.dll"), 3804) && ImportUMPrompt.CheckVersionInfo(Path.Combine(systemDirectory, "wmspdmoe.dll"), 3804);
		}

		private static bool CheckVersionInfo(string fullPath, int revisionNumber)
		{
			try
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fullPath);
				if (versionInfo == null || string.IsNullOrEmpty(versionInfo.FileVersion) || (versionInfo.FileVersion.StartsWith("10.0.0") && versionInfo.FilePrivatePart < revisionNumber))
				{
					return false;
				}
				return true;
			}
			catch (FileNotFoundException)
			{
			}
			return false;
		}

		internal abstract class ParameterSet
		{
			internal const string UploadDialPlanPrompts = "UploadDialPlanPrompts";

			internal const string UploadAutoAttendantPrompts = "UploadAutoAttendantPrompts";

			internal const string UploadDialPlanPromptsStream = "UploadDialPlanPromptsStream";

			internal const string UploadAutoAttendantPromptsStream = "UploadAutoAttendantPromptsStream";
		}
	}
}
