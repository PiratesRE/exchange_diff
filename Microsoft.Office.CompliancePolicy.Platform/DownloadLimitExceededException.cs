using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public sealed class DownloadLimitExceededException : Exception
	{
		public DownloadLimitExceededException(long size) : base(string.Format("The total download size limit ({0}) has been exceeded.", size))
		{
		}
	}
}
