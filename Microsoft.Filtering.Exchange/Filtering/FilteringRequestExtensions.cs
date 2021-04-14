using System;

namespace Microsoft.Filtering
{
	public static class FilteringRequestExtensions
	{
		public static void AddRecoveryOptions(this FilteringRequest request, RecoveryOptions options)
		{
			request.AddProperty("EnableRecovery", true);
			if (options.HasFlag(RecoveryOptions.Crash))
			{
				request.AddProperty("RecoverFromCrash", true);
			}
			if (options.HasFlag(RecoveryOptions.Timeout))
			{
				request.AddProperty("RecoverFromTimeout", true);
			}
		}
	}
}
