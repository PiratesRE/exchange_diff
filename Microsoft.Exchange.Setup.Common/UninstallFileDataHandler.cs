using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UninstallFileDataHandler : FileDataHandler
	{
		public UninstallFileDataHandler(ISetupContext context, MsiConfigurationInfo msiConfig, MonadConnection connection) : base(context, "uninstall-msipackage", msiConfig, connection)
		{
			if (msiConfig is DatacenterMsiConfigurationInfo)
			{
				base.WorkUnit.Text = Strings.RemoveDatacenterFileText;
				return;
			}
			base.WorkUnit.Text = Strings.RemoveFileText;
		}

		public bool IsUpgrade
		{
			get
			{
				return this.isUpgrade;
			}
			set
			{
				this.isUpgrade = value;
			}
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("ProductCode", this.MsiConfigurationInfo.ProductCode);
			if (this.IsUpgrade)
			{
				base.Parameters.AddWithValue("PropertyValues", string.Format("BYPASS_CONFIGURED_CHECK=1 DEFAULTLANGUAGENAME={0}", CultureInfo.InstalledUICulture.ThreeLetterWindowsLanguageName));
			}
			else
			{
				base.Parameters.AddWithValue("WarnOnRebootRequests", true);
				int num = 0;
				foreach (string installableUnitName in base.SelectedInstallableUnits)
				{
					if (!InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(installableUnitName))
					{
						num++;
					}
				}
				bool flag = true;
				if (num < base.SetupContext.UnpackedRoles.Count)
				{
					base.Parameters.AddWithValue("features", base.GetFeatures().ToArray());
					flag = false;
				}
				base.Parameters.AddWithValue("PropertyValues", string.Format("DEFAULTLANGUAGENAME={0} COMPLETEUNINSTALLATION={1}", base.SetupContext.ExchangeCulture.ThreeLetterWindowsLanguageName, flag ? 1 : 0));
			}
			SetupLogger.TraceExit();
		}

		private bool isUpgrade;
	}
}
