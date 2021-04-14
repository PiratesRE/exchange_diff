using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SharingXmlNamespaces
	{
		public const string Sharing = "http://schemas.microsoft.com/sharing/2008";

		public const string ExchangeSharingProvider = "http://schemas.microsoft.com/exchange/sharing/2008";

		public const string ExchangeWebServicesTypes = "http://schemas.microsoft.com/exchange/services/2006/types";

		public const string ExchangeWebServicesMessages = "http://schemas.microsoft.com/exchange/services/2006/messages";
	}
}
