using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	public abstract class SetOverrideBase : SetTopologySystemConfigurationObjectTask<SettingOverrideIdParameter, SettingOverride>
	{
		protected abstract bool IsFlight { get; }

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

		[Parameter(Mandatory = false)]
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
		[Parameter(Mandatory = false)]
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

		protected override ObjectId RootId
		{
			get
			{
				return SettingOverride.GetContainerId(this.IsFlight);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetExchangeSettings(this.DataObject.Name);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ConfigurationSettingsException).IsInstanceOfType(exception);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.MinVersion != null)
			{
				if (this.MaxVersion != null && this.MinVersion > this.MaxVersion)
				{
					base.WriteError(new SettingOverrideMinVersionGreaterThanMaxVersionException(this.MinVersion.ToString(), this.MaxVersion.ToString()), ErrorCategory.InvalidOperation, null);
				}
				if (this.FixVersion != null && this.MinVersion > this.FixVersion)
				{
					base.WriteError(new SettingOverrideMinVersionGreaterThanMaxVersionException(this.MinVersion.ToString(), this.FixVersion.ToString()), ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			SettingOverrideXml xml = this.DataObject.Xml;
			if (base.Fields.IsModified("MinVersion"))
			{
				xml.MinVersion = this.MinVersion;
			}
			if (base.Fields.IsModified("MaxVersion"))
			{
				xml.MaxVersion = this.MaxVersion;
				if (this.MaxVersion != null)
				{
					xml.FixVersion = null;
				}
			}
			if (base.Fields.IsModified("FixVersion"))
			{
				xml.FixVersion = this.FixVersion;
				if (this.FixVersion != null)
				{
					xml.MaxVersion = null;
				}
			}
			if (base.Fields.IsModified("Server"))
			{
				xml.Server = this.Server;
			}
			if (base.Fields.IsModified("Parameters"))
			{
				xml.Parameters = this.Parameters;
			}
			if (base.Fields.IsModified("Reason"))
			{
				xml.Reason = this.Reason;
			}
			xml.ModifiedBy = base.ExecutingUserIdentityName;
			this.DataObject.Xml = xml;
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
