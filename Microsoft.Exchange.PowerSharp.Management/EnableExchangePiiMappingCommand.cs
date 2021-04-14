using System;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class EnableExchangePiiMappingCommand : SyntheticCommand<object>
	{
		private EnableExchangePiiMappingCommand() : base("Enable-ExchangePiiMapping")
		{
		}

		public EnableExchangePiiMappingCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
