using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class VirtualListViewState
	{
		public const string SourceContainerIdName = "sId";

		public const string MultiLineName = "mL";

		public const string SortedColumnName = "sC";

		public const string SortOrderName = "sO";

		[OwaEventField("sId")]
		public ObjectId SourceContainerId;

		[OwaEventField("mL")]
		public bool IsMultiLine;

		[OwaEventField("sC")]
		public ColumnId SortedColumn;

		[OwaEventField("sO")]
		public SortOrder SortOrder;
	}
}
