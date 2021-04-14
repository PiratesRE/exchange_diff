using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface ISearchConfigurationProvider
	{
		void ApplyConfiguration(ISearchPolicy policy, ref SearchMailboxesInputs inputs);
	}
}
