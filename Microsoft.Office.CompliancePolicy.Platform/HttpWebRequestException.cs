using System;

namespace Microsoft.Office.CompliancePolicy
{
	internal class HttpWebRequestException : Exception
	{
		public HttpWebRequestException(Exception exception) : base("HttpWebRequest exception", exception)
		{
		}
	}
}
