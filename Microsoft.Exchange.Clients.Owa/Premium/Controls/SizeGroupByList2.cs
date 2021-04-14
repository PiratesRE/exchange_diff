using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class SizeGroupByList2 : GroupByList2
	{
		public SizeGroupByList2(SortOrder sortOrder, ItemList2 itemList, UserContext userContext) : base(ColumnId.Size, sortOrder, itemList, userContext)
		{
			base.SetGroupRange(SizeGroupByList2.sizeGroupRange);
		}

		public SizeGroupByList2(ColumnId groupByColumn, SortOrder sortOrder, ItemList2 itemList, UserContext userContext) : base(groupByColumn, sortOrder, itemList, userContext)
		{
			base.SetGroupRange(SizeGroupByList2.sizeGroupRange);
		}

		private static readonly IGroupRange[] sizeGroupRange = new IGroupRange[]
		{
			new SizeGroupByList2.SizeGroupRange(-461075480, int.MinValue, 10240),
			new SizeGroupByList2.SizeGroupRange(-346569563, 10240, 25600),
			new SizeGroupByList2.SizeGroupRange(-1064340177, 25600, 102400),
			new SizeGroupByList2.SizeGroupRange(-1891554117, 102400, 512000),
			new SizeGroupByList2.SizeGroupRange(1764690851, 512000, 1048576),
			new SizeGroupByList2.SizeGroupRange(-1718242455, 1048576, 5242880),
			new SizeGroupByList2.SizeGroupRange(-639396640, 5242880, int.MaxValue)
		};

		private class SizeGroupRange : IGroupRange
		{
			public string Header
			{
				get
				{
					return LocalizedStrings.GetNonEncoded(this.header);
				}
			}

			public SizeGroupRange(Strings.IDs header, int start, int end)
			{
				this.header = header;
				this.start = start;
				this.end = end;
			}

			public bool IsInGroup(IListViewDataSource dataSource, Column column)
			{
				object itemProperty = dataSource.GetItemProperty<object>(column[0]);
				long num = 0L;
				if (itemProperty != null)
				{
					if (itemProperty is int)
					{
						num = (long)((int)itemProperty);
					}
					else if (itemProperty is long)
					{
						num = (long)itemProperty;
					}
				}
				return (long)this.start <= num && num < (long)this.end;
			}

			private Strings.IDs header;

			private int start;

			private int end;
		}
	}
}
