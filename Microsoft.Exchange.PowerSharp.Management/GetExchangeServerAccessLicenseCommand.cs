using System;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetExchangeServerAccessLicenseCommand : SyntheticCommand<object>
	{
		private GetExchangeServerAccessLicenseCommand() : base("Get-ExchangeServerAccessLicense")
		{
		}

		public GetExchangeServerAccessLicenseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
