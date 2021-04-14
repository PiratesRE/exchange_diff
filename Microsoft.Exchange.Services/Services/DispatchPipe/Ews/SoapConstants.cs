using System;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	internal static class SoapConstants
	{
		public const string Soap11NamespaceUri = "http://schemas.xmlsoap.org/soap/envelope/";

		public const string Soap12NamespaceUri = "http://www.w3.org/2003/05/soap-envelope";

		public const string EnvelopeElementName = "Envelope";

		public const string HeaderElementName = "Header";

		public const string BodyElementName = "Body";

		public const string TextXmlUtf8ContentType = "text/xml; charset=utf-8";
	}
}
