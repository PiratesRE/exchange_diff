using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IOperationExecutionTrackingKey
	{
		int GetTrackingKeyHashValue();

		int GetSimpleHashValue();

		bool IsTrackingKeyEqualTo(IOperationExecutionTrackingKey other);

		string TrackingKeyToString();
	}
}
