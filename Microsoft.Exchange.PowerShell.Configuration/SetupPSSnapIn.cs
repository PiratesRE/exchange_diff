using System;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.PowerShell
{
	public sealed class SetupPSSnapIn : ExchangePSSnapIn
	{
		public override string Name
		{
			get
			{
				return SetupPSSnapIn.PSSnapInName;
			}
		}

		public override Collection<FormatConfigurationEntry> Formats
		{
			get
			{
				return new Collection<FormatConfigurationEntry>();
			}
		}

		public override Collection<CmdletConfigurationEntry> Cmdlets
		{
			get
			{
				if (this.cmdlets == null)
				{
					this.cmdlets = new Collection<CmdletConfigurationEntry>();
					foreach (CmdletConfigurationEntry item in CmdletConfigurationEntries.SetupCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item);
					}
				}
				return this.cmdlets;
			}
		}

		public static readonly string PSSnapInName = "Microsoft.Exchange.Management.PowerShell.Setup";

		private Collection<CmdletConfigurationEntry> cmdlets;
	}
}
