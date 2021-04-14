using System;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class DisableExchangePiiMappingCommand : SyntheticCommand<object>
	{
		private DisableExchangePiiMappingCommand() : base("Disable-ExchangePiiMapping")
		{
		}

		public DisableExchangePiiMappingCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
