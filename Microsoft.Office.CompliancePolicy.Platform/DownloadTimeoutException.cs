using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public sealed class DownloadTimeoutException : Exception
	{
		public DownloadTimeoutException() : base("The operation timed out.")
		{
		}
	}
}
