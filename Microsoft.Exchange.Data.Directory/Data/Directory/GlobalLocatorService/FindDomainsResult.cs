using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class FindDomainsResult
	{
		internal FindDomainsResult(FindDomainResult[] findDomainResults)
		{
			this.FindDomainResults = new ReadOnlyCollection<FindDomainResult>(findDomainResults);
		}

		internal readonly ReadOnlyCollection<FindDomainResult> FindDomainResults;
	}
}
