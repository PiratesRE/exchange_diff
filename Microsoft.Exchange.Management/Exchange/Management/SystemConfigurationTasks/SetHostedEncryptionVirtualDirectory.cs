using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "HostedEncryptionVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHostedEncryptionVirtualDirectory : SetWebAppVirtualDirectory<ADE4eVirtualDirectory>
	{
		[Parameter]
		public MultiValuedProperty<string> AllowedFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["AllowedFileTypes"];
			}
			set
			{
				base.Fields["AllowedFileTypes"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> AllowedMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["AllowedMimeTypes"];
			}
			set
			{
				base.Fields["AllowedMimeTypes"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> BlockedFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["BlockedFileTypes"];
			}
			set
			{
				base.Fields["BlockedFileTypes"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> BlockedMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["BlockedMimeTypes"];
			}
			set
			{
				base.Fields["BlockedMimeTypes"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ForceSaveFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ForceSaveFileTypes"];
			}
			set
			{
				base.Fields["ForceSaveFileTypes"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ForceSaveMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ForceSaveMimeTypes"];
			}
			set
			{
				base.Fields["ForceSaveMimeTypes"] = value;
			}
		}

		[Parameter]
		public bool? AlwaysShowBcc
		{
			get
			{
				return (bool?)base.Fields["AlwaysShowBcc"];
			}
			set
			{
				base.Fields["AlwaysShowBcc"] = value;
			}
		}

		[Parameter]
		public bool? CheckForForgottenAttachments
		{
			get
			{
				return (bool?)base.Fields["CheckForForgottenAttachments"];
			}
			set
			{
				base.Fields["CheckForForgottenAttachments"] = value;
			}
		}

		[Parameter]
		public bool? HideMailTipsByDefault
		{
			get
			{
				return (bool?)base.Fields["HideMailTipsByDefault"];
			}
			set
			{
				base.Fields["HideMailTipsByDefault"] = value;
			}
		}

		[Parameter]
		public uint? MailTipsLargeAudienceThreshold
		{
			get
			{
				return (uint?)base.Fields["MailTipsLargeAudienceThreshold"];
			}
			set
			{
				base.Fields["MailTipsLargeAudienceThreshold"] = value;
			}
		}

		[Parameter]
		public int? MaxRecipientsPerMessage
		{
			get
			{
				return (int?)base.Fields["MaxRecipientsPerMessage"];
			}
			set
			{
				base.Fields["MaxRecipientsPerMessage"] = value;
			}
		}

		[Parameter]
		public int? MaxMessageSizeInKb
		{
			get
			{
				return (int?)base.Fields["MaxMessageSizeInKb"];
			}
			set
			{
				base.Fields["MaxMessageSizeInKb"] = value;
			}
		}

		[Parameter]
		public string ComposeFontColor
		{
			get
			{
				return (string)base.Fields["ComposeFontColor"];
			}
			set
			{
				base.Fields["ComposeFontColor"] = value;
			}
		}

		[Parameter]
		public string ComposeFontName
		{
			get
			{
				return (string)base.Fields["ComposeFontName"];
			}
			set
			{
				base.Fields["ComposeFontName"] = value;
			}
		}

		[Parameter]
		public int? ComposeFontSize
		{
			get
			{
				return (int?)base.Fields["ComposeFontSize"];
			}
			set
			{
				base.Fields["ComposeFontSize"] = value;
			}
		}

		[Parameter]
		public int? MaxImageSizeKB
		{
			get
			{
				return (int?)base.Fields["MaxImageSizeKB"];
			}
			set
			{
				base.Fields["MaxImageSizeKB"] = value;
			}
		}

		[Parameter]
		public int? MaxAttachmentSizeKB
		{
			get
			{
				return (int?)base.Fields["MaxAttachmentSizeKB"];
			}
			set
			{
				base.Fields["MaxAttachmentSizeKB"] = value;
			}
		}

		[Parameter]
		public int? MaxEncryptedContentSizeKB
		{
			get
			{
				return (int?)base.Fields["MaxEncryptedContentSizeKB"];
			}
			set
			{
				base.Fields["MaxEncryptedContentSizeKB"] = value;
			}
		}

		[Parameter]
		public int? MaxEmailStringSize
		{
			get
			{
				return (int?)base.Fields["MaxEmailStringSize"];
			}
			set
			{
				base.Fields["MaxEmailStringSize"] = value;
			}
		}

		[Parameter]
		public int? MaxPortalStringSize
		{
			get
			{
				return (int?)base.Fields["MaxPortalStringSize"];
			}
			set
			{
				base.Fields["MaxPortalStringSize"] = value;
			}
		}

		[Parameter]
		public int? MaxFwdAllowed
		{
			get
			{
				return (int?)base.Fields["MaxFwdAllowed"];
			}
			set
			{
				base.Fields["MaxFwdAllowed"] = value;
			}
		}

		[Parameter]
		public int? PortalInactivityTimeout
		{
			get
			{
				return (int?)base.Fields["PortalInactivityTimeout"];
			}
			set
			{
				base.Fields["PortalInactivityTimeout"] = value;
			}
		}

		[Parameter]
		public int? TDSTimeOut
		{
			get
			{
				return (int?)base.Fields["TDSTimeOut"];
			}
			set
			{
				base.Fields["TDSTimeOut"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetHostedEncryptionVirtualDirectory(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			ADE4eVirtualDirectory dataObject = this.DataObject;
			WebAppVirtualDirectoryHelper.UpdateMetabase(dataObject, dataObject.MetabasePath, true);
			TaskLogger.LogExit();
		}

		protected override void UpdateDataObject(ADE4eVirtualDirectory dataObject)
		{
			if (base.Fields.Contains("AllowedFileTypes"))
			{
				dataObject.AllowedFileTypes = this.AllowedFileTypes;
			}
			if (base.Fields.Contains("AllowedMimeTypes"))
			{
				dataObject.AllowedMimeTypes = this.AllowedMimeTypes;
			}
			if (base.Fields.Contains("BlockedFileTypes"))
			{
				dataObject.BlockedFileTypes = this.BlockedFileTypes;
			}
			if (base.Fields.Contains("BlockedMimeTypes"))
			{
				dataObject.BlockedMimeTypes = this.BlockedMimeTypes;
			}
			if (base.Fields.Contains("ForceSaveFileTypes"))
			{
				dataObject.ForceSaveFileTypes = this.ForceSaveFileTypes;
			}
			if (base.Fields.Contains("ForceSaveMimeTypes"))
			{
				dataObject.ForceSaveMimeTypes = this.ForceSaveMimeTypes;
			}
			if (base.Fields.Contains("AlwaysShowBcc"))
			{
				dataObject.AlwaysShowBcc = this.AlwaysShowBcc;
			}
			if (base.Fields.Contains("CheckForForgottenAttachments"))
			{
				dataObject.CheckForForgottenAttachments = this.CheckForForgottenAttachments;
			}
			if (base.Fields.Contains("HideMailTipsByDefault"))
			{
				dataObject.HideMailTipsByDefault = this.HideMailTipsByDefault;
			}
			if (base.Fields.Contains("MailTipsLargeAudienceThreshold"))
			{
				dataObject.MailTipsLargeAudienceThreshold = this.MailTipsLargeAudienceThreshold;
			}
			if (base.Fields.Contains("MaxRecipientsPerMessage"))
			{
				dataObject.MaxRecipientsPerMessage = this.MaxRecipientsPerMessage;
			}
			if (base.Fields.Contains("MaxMessageSizeInKb"))
			{
				dataObject.MaxMessageSizeInKb = this.MaxMessageSizeInKb;
			}
			if (base.Fields.Contains("ComposeFontColor"))
			{
				dataObject.ComposeFontColor = this.ComposeFontColor;
			}
			if (base.Fields.Contains("ComposeFontName"))
			{
				dataObject.ComposeFontName = this.ComposeFontName;
			}
			if (base.Fields.Contains("ComposeFontSize"))
			{
				dataObject.ComposeFontSize = this.ComposeFontSize;
			}
			if (base.Fields.Contains("MaxImageSizeKB"))
			{
				dataObject.MaxImageSizeKB = this.MaxImageSizeKB;
			}
			if (base.Fields.Contains("MaxAttachmentSizeKB"))
			{
				dataObject.MaxAttachmentSizeKB = this.MaxAttachmentSizeKB;
			}
			if (base.Fields.Contains("MaxEncryptedContentSizeKB"))
			{
				dataObject.MaxEncryptedContentSizeKB = this.MaxEncryptedContentSizeKB;
			}
			if (base.Fields.Contains("MaxEmailStringSize"))
			{
				dataObject.MaxEmailStringSize = this.MaxEmailStringSize;
			}
			if (base.Fields.Contains("MaxPortalStringSize"))
			{
				dataObject.MaxPortalStringSize = this.MaxPortalStringSize;
			}
			if (base.Fields.Contains("MaxFwdAllowed"))
			{
				dataObject.MaxFwdAllowed = this.MaxFwdAllowed;
			}
			if (base.Fields.Contains("PortalInactivityTimeout"))
			{
				dataObject.PortalInactivityTimeout = this.PortalInactivityTimeout;
			}
			if (base.Fields.Contains("TDSTimeOut"))
			{
				dataObject.TDSTimeOut = this.TDSTimeOut;
			}
			dataObject.SaveSettings();
			if (base.Fields.Contains("GzipLevel") && base.GzipLevel != dataObject.GzipLevel)
			{
				dataObject.GzipLevel = base.GzipLevel;
				base.CheckGzipLevelIsNotError(dataObject.GzipLevel);
			}
			base.UpdateDataObject(dataObject);
		}
	}
}
