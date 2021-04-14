using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DatabaseWithForest
	{
		public DatabaseWithForest(Guid database, string resourceForest, Guid initiatingRequestId)
		{
			this.Database = database;
			this.ResourceForest = resourceForest;
			this.InitiatingRequestId = initiatingRequestId;
		}

		public Guid Database { get; set; }

		public string ResourceForest { get; set; }

		public Guid InitiatingRequestId { get; set; }
	}
}
