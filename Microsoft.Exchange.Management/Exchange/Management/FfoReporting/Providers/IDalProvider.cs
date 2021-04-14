using System;
using System.Collections;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	public interface IDalProvider
	{
		IEnumerable GetSingleDataPage(string targetObjectTypeName, string dalObjectTypeName, string methodName, QueryFilter filter);

		IEnumerable GetAllDataPages(string targetObjectTypeName, string dalObjectTypeName, string methodName, QueryFilter queryFilter);
	}
}
