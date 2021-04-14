using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public static class IndexCapabilities
	{
		public static bool SupportsCaseInsensitiveStringComparison
		{
			get
			{
				return IndexCapabilities.supportsCaseInsensitiveStringComparison;
			}
			set
			{
				IndexCapabilities.supportsCaseInsensitiveStringComparison = value;
			}
		}

		private static bool supportsCaseInsensitiveStringComparison;
	}
}
