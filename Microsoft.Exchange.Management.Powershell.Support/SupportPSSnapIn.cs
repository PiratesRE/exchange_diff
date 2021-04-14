using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[RunInstaller(true)]
	public sealed class SupportPSSnapIn : CustomPSSnapIn
	{
		public override string Name
		{
			get
			{
				return SupportPSSnapIn.PSSnapInName;
			}
		}

		public override string Description
		{
			get
			{
				return Strings.ExchangeSupportPSSnapInDescription;
			}
		}

		public override string Vendor
		{
			get
			{
				return "Microsoft Corporation";
			}
		}

		public override Collection<CmdletConfigurationEntry> Cmdlets
		{
			get
			{
				if (this.cmdlets == null)
				{
					this.cmdlets = new Collection<CmdletConfigurationEntry>();
					foreach (CmdletConfigurationEntry item in CmdletConfiguration.SupportCmdletConfigurationEntries)
					{
						this.cmdlets.Add(item);
					}
				}
				return this.cmdlets;
			}
		}

		public override Collection<FormatConfigurationEntry> Formats
		{
			get
			{
				if (this.formats == null)
				{
					this.formats = new Collection<FormatConfigurationEntry>();
					foreach (FormatConfigurationEntry item in CmdletConfiguration.SupportFormatConfigurationEntries)
					{
						this.formats.Add(item);
					}
				}
				return this.formats;
			}
		}

		public static readonly string PSSnapInName = "Microsoft.Exchange.Management.Powershell.Support";

		private Collection<CmdletConfigurationEntry> cmdlets;

		private Collection<FormatConfigurationEntry> formats;
	}
}
