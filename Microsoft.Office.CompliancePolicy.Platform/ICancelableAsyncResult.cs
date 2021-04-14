using System;

namespace Microsoft.Office.CompliancePolicy
{
	public interface ICancelableAsyncResult : IAsyncResult
	{
		bool IsCanceled { get; }

		void Cancel();
	}
}
