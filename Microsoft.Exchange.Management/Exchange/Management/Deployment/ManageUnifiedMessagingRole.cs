using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageUnifiedMessagingRole : ManageRole
	{
		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			this.Language = CultureInfo.CreateSpecificCulture("en-us");
			try
			{
				UmLanguagePack umLanguagePack = UmLanguagePackUtils.GetUmLanguagePack(this.Language);
				this.TeleProductCode = umLanguagePack.TeleProductCode;
				this.TransProductCode = umLanguagePack.TransProductCode;
				this.TtsProductCode = umLanguagePack.TtsProductCode;
				this.ProductCode = umLanguagePack.ProductCode;
			}
			catch (UnSupportedUMLanguageException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			base.WriteVerbose(Strings.UmLanguagePackProductCode(this.ProductCode));
		}

		[Parameter(Mandatory = false)]
		public string LogFilePath
		{
			get
			{
				return (string)base.Fields["LogFilePath"];
			}
			set
			{
				base.Fields["LogFilePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ProductCode
		{
			get
			{
				return (Guid)base.Fields["ProductCode"];
			}
			set
			{
				base.Fields["ProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TeleProductCode
		{
			get
			{
				return (Guid)base.Fields["TeleProductCode"];
			}
			set
			{
				base.Fields["TeleProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TransProductCode
		{
			get
			{
				return (Guid)base.Fields["TransProductCode"];
			}
			set
			{
				base.Fields["TransProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TtsProductCode
		{
			get
			{
				return (Guid)base.Fields["TtsProductCode"];
			}
			set
			{
				base.Fields["TtsProductCode"] = value;
			}
		}
	}
}
