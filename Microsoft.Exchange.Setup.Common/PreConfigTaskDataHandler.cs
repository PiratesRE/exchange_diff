using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PreConfigTaskDataHandler : SetupSingleTaskDataHandler
	{
		public PreConfigTaskDataHandler(ISetupContext context, MonadConnection connection) : base(context, "Start-PreConfiguration", connection)
		{
			base.WorkUnit.Text = Strings.PreConfigurationDisplayName;
			base.WorkUnit.CanShowExecutedCommand = false;
		}

		public List<string> SelectedInstallableUnits
		{
			get
			{
				return this.selectedInstallableUnits;
			}
			set
			{
				this.selectedInstallableUnits = value;
			}
		}

		public string[] Roles
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SelectedInstallableUnits != null)
				{
					foreach (string text in this.SelectedInstallableUnits)
					{
						if (!InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(text))
						{
							list.Add(text);
						}
					}
				}
				return list.ToArray();
			}
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			if (base.SetupContext.IsDatacenter)
			{
				base.Parameters.AddWithValue("IsDatacenter", base.SetupContext.IsDatacenter);
			}
			if (base.SetupContext.IsDatacenterDedicated)
			{
				base.Parameters.AddWithValue("IsDatacenterDedicated", base.SetupContext.IsDatacenterDedicated);
			}
			base.Parameters.AddWithValue("Mode", base.SetupContext.InstallationMode);
			base.Parameters.AddWithValue("Roles", this.Roles);
			if (base.SetupContext.InstallWindowsComponents)
			{
				base.Parameters.AddWithValue("InstallWindowsComponents", true);
			}
			if (base.SetupContext.IsSchemaUpdateRequired || base.SetupContext.IsOrgConfigUpdateRequired)
			{
				base.Parameters.AddWithValue("ADToolsNeeded", true);
			}
			SetupLogger.TraceExit();
		}

		private const string installWindowsComponentsArgument = "InstallWindowsComponents";

		private const string adToolsNeededArgument = "ADToolsNeeded";

		private List<string> selectedInstallableUnits;
	}
}
