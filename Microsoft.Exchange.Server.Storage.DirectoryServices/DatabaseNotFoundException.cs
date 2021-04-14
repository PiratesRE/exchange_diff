using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class DatabaseNotFoundException : StoreException
	{
		public DatabaseNotFoundException(LID lid, Guid databaseId) : base(lid, ErrorCodeValue.NotFound)
		{
			this.databaseId = databaseId;
		}

		public DatabaseNotFoundException(LID lid, Guid databaseId, Exception innerException) : base(lid, ErrorCodeValue.NotFound, string.Empty, innerException)
		{
		}

		public Guid DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		private Guid databaseId;
	}
}
