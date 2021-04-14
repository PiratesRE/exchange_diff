using System;

namespace Microsoft.Exchange.Net.JobQueues
{
	[Serializable]
	public class EnqueueResult
	{
		public EnqueueResult(EnqueueResultType result) : this(result, null)
		{
		}

		public EnqueueResult(EnqueueResultType result, string resultDetail)
		{
			this.Result = result;
			this.ResultDetail = resultDetail;
		}

		public static EnqueueResult Success = new EnqueueResult(EnqueueResultType.Successful);

		public EnqueueResultType Result;

		public string ResultDetail;

		public QueueType Type = QueueType.Unknown;
	}
}
