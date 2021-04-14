using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IWMIDataProvider
	{
		Dictionary<string, object>[] Run(string wmiQuery);
	}
}
