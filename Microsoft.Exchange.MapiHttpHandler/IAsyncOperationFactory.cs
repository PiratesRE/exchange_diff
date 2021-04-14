using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAsyncOperationFactory
	{
		AsyncOperation Create(string requestType, HttpContextBase context);
	}
}
