using System;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal interface IPageableObject
	{
		int Index { get; set; }
	}
}
