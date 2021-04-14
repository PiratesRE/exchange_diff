using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ModifiedSearchFolders
	{
		public ISet<ExchangeId> InsertedInto
		{
			get
			{
				return this.insertedInto;
			}
		}

		public ISet<ExchangeId> DeletedFrom
		{
			get
			{
				return this.deletedFrom;
			}
		}

		public ISet<ExchangeId> Updated
		{
			get
			{
				return this.updated;
			}
		}

		private readonly ISet<ExchangeId> insertedInto = new HashSet<ExchangeId>();

		private readonly ISet<ExchangeId> deletedFrom = new HashSet<ExchangeId>();

		private readonly ISet<ExchangeId> updated = new HashSet<ExchangeId>();
	}
}
