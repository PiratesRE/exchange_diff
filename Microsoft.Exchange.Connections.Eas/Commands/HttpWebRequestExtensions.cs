using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class HttpWebRequestExtensions
	{
		public static HttpWebResponse TryGetResponse(this HttpWebRequest request)
		{
			return (HttpWebResponse)request.GetResponse();
		}

		public static HttpWebResponse GetHttpResponse(this HttpWebRequest request, List<int> expectedHttpStatusCodes)
		{
			HttpWebResponse result;
			try
			{
				result = request.TryGetResponse();
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse == null || !expectedHttpStatusCodes.Contains((int)httpWebResponse.StatusCode))
				{
					throw;
				}
				result = httpWebResponse;
			}
			return result;
		}
	}
}
