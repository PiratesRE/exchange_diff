using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class ListViewState
	{
		public const string SourceContainerIdName = "sId";

		public const string SortedColumnIdName = "sC";

		public const string SortDirectionName = "sO";

		public const string MultiLineName = "mL";

		public const string StartRangeName = "sR";

		public const string EndRangeName = "eR";

		public const string TotalCountName = "tC";

		[OwaEventField("sId")]
		public ObjectId SourceContainerId;

		[OwaEventField("sC")]
		public ColumnId SortedColumnId;

		[OwaEventField("sO")]
		public SortOrder SortDirection;

		[OwaEventField("mL")]
		public bool IsMultiLine;

		[OwaEventField("sR")]
		public int StartRange;

		[OwaEventField("eR")]
		public int EndRange;

		[OwaEventField("tC")]
		public int TotalCount;
	}
}
