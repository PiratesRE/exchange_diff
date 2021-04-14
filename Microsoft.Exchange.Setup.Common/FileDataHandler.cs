using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FileDataHandler : SetupSingleTaskDataHandler
	{
		public FileDataHandler(ISetupContext context, string commandText, MsiConfigurationInfo msiConfig, MonadConnection connection) : base(context, commandText, connection)
		{
			this.msiConfigurationInfo = msiConfig;
			this.SelectedInstallableUnits = new List<string>();
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("logfile", this.msiConfigurationInfo.LogFilePath);
			SetupLogger.TraceExit();
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

		protected List<string> GetFeatures()
		{
			List<string> list = new List<string>();
			foreach (string text in this.SelectedInstallableUnits.ToArray())
			{
				if (InstallableUnitConfigurationInfoManager.IsRoleBasedConfigurableInstallableUnit(text))
				{
					list.Add(text.Replace("Role", ""));
				}
			}
			return list;
		}

		protected virtual MsiConfigurationInfo MsiConfigurationInfo
		{
			get
			{
				return this.msiConfigurationInfo;
			}
			set
			{
				this.msiConfigurationInfo = value;
			}
		}

		private MsiConfigurationInfo msiConfigurationInfo;

		private List<string> selectedInstallableUnits;
	}
}
