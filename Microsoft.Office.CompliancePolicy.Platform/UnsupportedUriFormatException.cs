using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public sealed class UnsupportedUriFormatException : Exception
	{
		public UnsupportedUriFormatException(string uri) : base(string.Format("The HTTP client doesn't support the format of the URI ({0}).", uri))
		{
		}
	}
}
