using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeCfgDataHandler : ConfigurationDataHandler
	{
		public UpgradeCfgDataHandler(ISetupContext context, Role roleInfo, MonadConnection connection) : base(context, roleInfo.RoleName, "Install-" + roleInfo.RoleName, connection)
		{
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			if (base.SetupContext.IsDatacenter && base.SetupContext.IsFfo)
			{
				base.Parameters.AddWithValue("IsFfo", true);
			}
			base.Parameters.AddWithValue("LanguagePacksPath", base.GetMsiSourcePath());
			base.Parameters.AddWithValue("UpdatesDir", this.UpdatesDir);
			SetupLogger.TraceExit();
		}

		private LongPath UpdatesDir
		{
			get
			{
				return base.SetupContext.UpdatesDir;
			}
			set
			{
				base.SetupContext.UpdatesDir = value;
			}
		}
	}
}
