using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CategorizedQueryParams
	{
		public IReadOnlyDictionary<Column, Column> HeaderRenameDictionary { get; set; }

		public IReadOnlyDictionary<Column, Column> LeafRenameDictionary { get; set; }

		public CategorizedTableCollapseState CollapseState { get; set; }
	}
}
