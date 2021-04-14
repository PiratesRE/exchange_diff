using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public sealed class DownloadCanceledException : Exception
	{
		public DownloadCanceledException() : base("The operation is canceled.")
		{
		}
	}
}
