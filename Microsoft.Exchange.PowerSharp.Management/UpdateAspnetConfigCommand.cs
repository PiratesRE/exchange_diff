using System;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateAspnetConfigCommand : SyntheticCommand<object>
	{
		private UpdateAspnetConfigCommand() : base("Update-AspnetConfig")
		{
		}

		public UpdateAspnetConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
