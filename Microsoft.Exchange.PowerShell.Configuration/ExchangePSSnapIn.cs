using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.PowerShell
{
	public abstract class ExchangePSSnapIn : CustomPSSnapIn
	{
		public ExchangePSSnapIn()
		{
			Globals.InitializeMultiPerfCounterInstance("EMS");
		}

		public override string Vendor
		{
			get
			{
				return "Microsoft Corporation";
			}
		}

		public override string Description
		{
			get
			{
				return Strings.ExchangePSSnapInDescription;
			}
		}

		public override Collection<FormatConfigurationEntry> Formats
		{
			get
			{
				if (this.formats == null)
				{
					this.formats = new Collection<FormatConfigurationEntry>();
					foreach (FormatConfigurationEntry item in this.formatConfigurationEntries)
					{
						this.formats.Add(item);
					}
				}
				return this.formats;
			}
		}

		private FormatConfigurationEntry[] formatConfigurationEntries = new FormatConfigurationEntry[]
		{
			new FormatConfigurationEntry("Exchange.format.ps1xml")
		};

		private Collection<FormatConfigurationEntry> formats;
	}
}
