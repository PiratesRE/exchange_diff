using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class LocalQuery
	{
		internal LocalQuery(ClientContext clientContext, DateTime deadline)
		{
			this.clientContext = clientContext;
			this.deadline = deadline;
		}

		internal abstract BaseQueryResult GetData(BaseQuery query);

		protected ClientContext clientContext;

		protected DateTime deadline;
	}
}
