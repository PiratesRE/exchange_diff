using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NspiDataException : Exception
	{
		internal NspiDataException(string methodName, string message) : base(string.Format("{0} :: {1}", methodName, message))
		{
		}
	}
}
