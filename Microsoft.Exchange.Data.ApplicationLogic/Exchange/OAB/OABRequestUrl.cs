using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	public sealed class OABRequestUrl
	{
		public static Guid GetOabGuidFromRequest(HttpRequest request)
		{
			int num = request.ApplicationPath.Length + 1;
			if (request.Path.Length > num)
			{
				int num2 = request.Path.IndexOf('/', num + 1);
				if (num2 > num)
				{
					int length = num2 - num;
					string g = request.Path.Substring(num, length);
					try
					{
						return new Guid(g);
					}
					catch (ArgumentException)
					{
					}
					catch (FormatException)
					{
					}
				}
			}
			ExTraceGlobals.DataTracer.TraceError<string>(0L, "[OABRequestUrl::GetOabGuidFromRequest] unable to parse OAB GUID from request path: {0}", request.Path);
			return Guid.Empty;
		}
	}
}
