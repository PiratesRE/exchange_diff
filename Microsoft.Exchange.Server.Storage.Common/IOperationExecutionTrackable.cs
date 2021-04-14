using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IOperationExecutionTrackable
	{
		IOperationExecutionTrackingKey GetTrackingKey();
	}
}
