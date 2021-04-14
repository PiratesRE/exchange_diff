using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallUMCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallUMCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "UnifiedMessagingRole", "Install-UnifiedMessagingRole", connection)
		{
			this.unifiedMessagingRoleConfigurationInfo = (UnifiedMessagingRoleConfigurationInfo)base.InstallableUnitConfigurationInfo;
		}

		public List<CultureInfo> SelectedCultures
		{
			get
			{
				return this.unifiedMessagingRoleConfigurationInfo.SelectedCultures;
			}
			set
			{
				this.unifiedMessagingRoleConfigurationInfo.SelectedCultures = value;
			}
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			if (base.SetupContext.SourceDir != null)
			{
				base.Parameters.AddWithValue("SourcePath", base.SetupContext.SourceDir.PathName + Path.DirectorySeparatorChar);
			}
			SetupLogger.TraceExit();
		}

		private UnifiedMessagingRoleConfigurationInfo unifiedMessagingRoleConfigurationInfo;
	}
}
