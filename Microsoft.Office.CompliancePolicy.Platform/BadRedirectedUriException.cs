using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class BadRedirectedUriException : Exception
	{
		public BadRedirectedUriException(string uri, Exception innerException) : base(string.Format("Bad redirect URI: {0}.", uri), innerException)
		{
		}
	}
}
