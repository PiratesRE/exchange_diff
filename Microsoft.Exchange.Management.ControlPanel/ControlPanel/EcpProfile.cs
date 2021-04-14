using System;
using System.Globalization;
using System.Web.Configuration;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("EcpProfile", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class EcpProfile : ScriptComponent
	{
		public static string CurrentLanguage
		{
			get
			{
				return CultureInfo.CurrentCulture.IetfLanguageTag;
			}
		}

		private string DisplayName
		{
			get
			{
				return RbacPrincipal.Current.RbacConfiguration.ExecutingUserDisplayName;
			}
		}

		private string UserIDHash
		{
			get
			{
				return RbacPrincipal.Current.Identity.Name.GetHashCode().ToString();
			}
		}

		private string Theme
		{
			get
			{
				return ThemeResource.Private_GetThemeResource(this, string.Empty);
			}
		}

		private bool EnableWizardNextOnError
		{
			get
			{
				return StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["EnableWizardNextOnError"]);
			}
		}

		private bool GetResilienceEnabled()
		{
			return StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["ResilienceEnabled"]);
		}

		private bool GetIsInstrumentationEnabled()
		{
			Random random = new Random();
			double num = (double)random.Next(0, 100000) / 100000.0;
			double configDoubleValue = AppConfigLoader.GetConfigDoubleValue("ClientInstrumentationProbability", 0.0, 1.0, 1.0);
			return num < configDoubleValue;
		}

		private int GetInstrumentationUploadDuration()
		{
			return AppConfigLoader.GetConfigIntValue("ClientInstrumentationUploadDuration", 1, int.MaxValue, 55);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("DisplayName", this.DisplayName, true);
			descriptor.AddProperty("Language", EcpProfile.CurrentLanguage, true);
			descriptor.AddProperty("UserIDHash", this.UserIDHash, true);
			descriptor.AddProperty("Theme", this.Theme, true);
			descriptor.AddProperty("ScriptPath", ThemeResource.ScriptPath, true);
			descriptor.AddProperty("DecodedDirectionMark", RtlUtil.DecodedDirectionMark);
			descriptor.AddProperty("IsDataCenter", Util.IsDataCenter);
			descriptor.AddProperty("IsCrossPremiseMigration", VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.CrossPremiseMigration.Enabled);
			descriptor.AddProperty("AllowMailboxArchiveOnlyMigration", VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.AllowMailboxArchiveOnlyMigration.Enabled);
			descriptor.AddProperty("AllowRemoteOnboardingMovesOnly", VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.AllowRemoteOnboardingMovesOnly.Enabled);
			descriptor.AddProperty("IsInstrumentationEnabled", this.GetIsInstrumentationEnabled());
			descriptor.AddProperty("InstrumentationUploadDuration", this.GetInstrumentationUploadDuration());
			descriptor.AddProperty("IsResilienceEnabled", this.GetResilienceEnabled());
			descriptor.AddProperty("EnableWizardNextOnError", this.EnableWizardNextOnError, true);
			descriptor.AddProperty("HighContrastCssFile", CssFiles.HighContrastCss.ToUrl(this));
		}
	}
}
