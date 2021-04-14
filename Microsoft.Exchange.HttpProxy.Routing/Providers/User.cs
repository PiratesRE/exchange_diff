using System;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	internal class User
	{
		public Guid? DatabaseGuid { get; set; }

		public string DatabaseResourceForest { get; set; }

		public Guid? ArchiveDatabaseGuid { get; set; }

		public string ArchiveDatabaseResourceForest { get; set; }

		public Guid? ArchiveGuid { get; set; }

		public DateTime? LastModifiedTime { get; set; }
	}
}
