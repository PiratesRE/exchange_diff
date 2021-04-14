using System;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	internal sealed class WebSitesNotConfiguredException : Exception
	{
		public WebSitesNotConfiguredException() : base("No IIS web sites were configured on the server.")
		{
		}

		public WebSitesNotConfiguredException(Exception innerException) : base("No IIS web sites were configured on the server.", innerException)
		{
		}

		private const string ErrorMessage = "No IIS web sites were configured on the server.";
	}
}
