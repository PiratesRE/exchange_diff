using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal abstract class RequestFactory
	{
		internal abstract UcwaWebRequest CreateRequest(string absoluteUri, string method);
	}
}
