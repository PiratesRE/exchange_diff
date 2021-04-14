using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Set", "CountryList", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetCountryList : SetSystemConfigurationObjectTask<CountryListIdParameter, CountryList>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetCountryList(this.Identity.ToString());
			}
		}
	}
}
