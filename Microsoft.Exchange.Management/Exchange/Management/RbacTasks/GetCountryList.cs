using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "CountryList", DefaultParameterSetName = "Identity")]
	public sealed class GetCountryList : GetSystemConfigurationObjectTask<CountryListIdParameter, CountryList>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
