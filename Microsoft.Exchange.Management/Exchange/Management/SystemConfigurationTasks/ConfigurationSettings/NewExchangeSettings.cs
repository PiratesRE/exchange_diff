using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("New", "ExchangeSettings", SupportsShouldProcess = true)]
	public sealed class NewExchangeSettings : NewSystemConfigurationObjectTask<ExchangeSettings>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
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
				return Strings.ConfirmationMessageNewExchangeSettings(base.Name.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ExchangeSettings exchangeSettings = (ExchangeSettings)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (!this.Force && !SetExchangeSettings.IsSchemaRegistered(base.Name))
			{
				base.WriteError(new ExchangeSettingsInvalidSchemaException(base.Name), ErrorCategory.InvalidOperation, null);
			}
			exchangeSettings.SetId(this.ConfigurationSession, base.Name);
			exchangeSettings.InitializeSettings();
			TaskLogger.LogExit();
			return exchangeSettings;
		}
	}
}
