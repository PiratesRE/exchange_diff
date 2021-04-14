using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class NullTrackableOperation : IOperationExecutionTrackable
	{
		public static IOperationExecutionTrackable Instance
		{
			get
			{
				return NullTrackableOperation.operation;
			}
		}

		public IOperationExecutionTrackingKey GetTrackingKey()
		{
			return null;
		}

		private static IOperationExecutionTrackable operation = new NullTrackableOperation();
	}
}
