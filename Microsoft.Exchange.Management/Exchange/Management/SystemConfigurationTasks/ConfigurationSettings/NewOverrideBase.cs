using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	public abstract class NewOverrideBase : NewSystemConfigurationObjectTask<SettingOverride>
	{
		protected abstract bool IsFlight { get; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return base.Fields["Name"] as string;
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version MinVersion
		{
			get
			{
				return base.Fields["MinVersion"] as Version;
			}
			set
			{
				base.Fields["MinVersion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version MaxVersion
		{
			get
			{
				return base.Fields["MaxVersion"] as Version;
			}
			set
			{
				base.Fields["MaxVersion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version FixVersion
		{
			get
			{
				return base.Fields["FixVersion"] as Version;
			}
			set
			{
				base.Fields["FixVersion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Server
		{
			get
			{
				return base.Fields["Server"] as string[];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return base.Fields["Parameters"] as MultiValuedProperty<string>;
			}
			set
			{
				base.Fields["Parameters"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Reason
		{
			get
			{
				return base.Fields["Reason"] as string;
			}
			set
			{
				base.Fields["Reason"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				SwitchParameter? switchParameter = base.Fields["Force"] as SwitchParameter?;
				if (switchParameter == null)
				{
					return default(SwitchParameter);
				}
				return switchParameter.GetValueOrDefault();
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewExchangeSettings(this.Name);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.MaxVersion != null && this.FixVersion != null)
			{
				base.WriteError(new SettingOverrideMaxVersionAndFixVersionSpecifiedException(), ErrorCategory.InvalidOperation, null);
			}
			if (this.MinVersion != null)
			{
				if (this.MaxVersion != null && this.MinVersion > this.MaxVersion)
				{
					base.WriteError(new SettingOverrideMinVersionGreaterThanMaxVersionException(this.MinVersion.ToString(), this.MaxVersion.ToString()), ErrorCategory.InvalidOperation, null);
				}
				if (this.FixVersion != null && this.MinVersion >= this.FixVersion)
				{
					base.WriteError(new SettingOverrideMinVersionGreaterThanMaxVersionException(this.MinVersion.ToString(), this.FixVersion.ToString()), ErrorCategory.InvalidOperation, null);
				}
			}
			try
			{
				SettingOverride.Validate(this.GetOverride());
			}
			catch (SettingOverrideException ex)
			{
				if (this.Force)
				{
					base.WriteWarning(ex.Message);
				}
				else
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			SettingOverride settingOverride = (SettingOverride)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			settingOverride.SetName(this.Name, this.IsFlight);
			settingOverride.Xml = this.GetXml();
			TaskLogger.LogExit();
			return settingOverride;
		}

		protected virtual SettingOverrideXml GetXml()
		{
			return new SettingOverrideXml
			{
				MinVersion = this.MinVersion,
				MaxVersion = this.MaxVersion,
				FixVersion = this.FixVersion,
				Server = this.Server,
				Parameters = this.Parameters,
				Reason = this.Reason,
				ModifiedBy = base.ExecutingUserIdentityName
			};
		}

		protected abstract VariantConfigurationOverride GetOverride();
	}
}
