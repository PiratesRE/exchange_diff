using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class LegacyHttpHandler : AutodiscoverDiscoveryHttpHandler
	{
		internal override void GenerateResponse(HttpContext context)
		{
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			base.AddEndpointEnabledHeaders(response);
			HttpStatusCode statusCode = request.IsAuthenticated ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
			response.StatusCode = (int)statusCode;
			this.GenerateErrorResponse(context);
		}

		private void GenerateErrorResponse(HttpContext context)
		{
			HttpResponse response = context.Response;
			response.ContentType = "text/xml; charset=utf-8";
			using (TextWriter output = response.Output)
			{
				XmlWriter xmlFragment = XmlWriter.Create(output, new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					ConformanceLevel = ConformanceLevel.Document
				});
				bool useClientCertificateAuthentication = false;
				RequestData requestData = new RequestData(null, useClientCertificateAuthentication, CallerRequestedCapabilities.GetInstance(context));
				Common.GenerateErrorResponseDontLog(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "600", Strings.InvalidRequest, string.Empty, requestData, base.GetType().AssemblyQualifiedName);
			}
		}
	}
}
