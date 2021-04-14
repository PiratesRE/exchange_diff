using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class HeavyBlockingOperationEventArgs : EventArgs
	{
		internal HeavyBlockingOperationEventArgs(IUMHeavyBlockingOperation operation)
		{
			this.operation = operation;
		}

		internal TimeSpan Latency
		{
			get
			{
				return this.latency;
			}
			set
			{
				this.latency = value;
			}
		}

		internal Exception Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		internal IUMHeavyBlockingOperation Operation
		{
			get
			{
				return this.operation;
			}
		}

		internal HeavyBlockingOperationCompletionType CompletionType
		{
			get
			{
				return this.completionType;
			}
			set
			{
				this.completionType = value;
			}
		}

		private HeavyBlockingOperationCompletionType completionType;

		private IUMHeavyBlockingOperation operation;

		private Exception error;

		private TimeSpan latency;
	}
}
