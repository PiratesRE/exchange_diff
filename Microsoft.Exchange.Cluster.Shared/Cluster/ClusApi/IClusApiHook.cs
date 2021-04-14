using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IClusApiHook
	{
		int CallBack(ClusApiHooks api, string hintStr, Func<int> func);
	}
}
