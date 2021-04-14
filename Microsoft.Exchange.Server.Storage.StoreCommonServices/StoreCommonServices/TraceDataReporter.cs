using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal abstract class TraceDataReporter<TContainer>
	{
		protected TraceDataReporter(StoreDatabase database, IBinaryLogger logger, TContainer dataContainer)
		{
			this.logger = logger;
			this.database = database;
			this.dataContainer = dataContainer;
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public IBinaryLogger Logger
		{
			get
			{
				return this.logger;
			}
		}

		public TContainer DataContainer
		{
			get
			{
				return this.dataContainer;
			}
		}

		private readonly StoreDatabase database;

		private readonly IBinaryLogger logger;

		private readonly TContainer dataContainer;
	}
}
