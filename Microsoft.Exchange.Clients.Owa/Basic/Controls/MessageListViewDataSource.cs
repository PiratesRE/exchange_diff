using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class MessageListViewDataSource : ListViewDataSource
	{
		public MessageListViewDataSource(Hashtable properties, Folder folder, SortBy[] sortBy) : base(properties, folder)
		{
			if (sortBy == null)
			{
				throw new ArgumentNullException("sortBy");
			}
			this.sortBy = sortBy;
		}

		public override void LoadData(int startRange, int endRange)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewDataSource.LoadData(Start)");
			if (startRange < 1)
			{
				throw new ArgumentOutOfRangeException("startRange", "startRange must be greater than 0");
			}
			if (endRange < startRange)
			{
				throw new ArgumentOutOfRangeException("endRange", "endRange must be greater than or equal to startRange");
			}
			PropertyDefinition[] dataColumns = base.CreateProperyTable();
			if (0 < base.Folder.ItemCount)
			{
				using (QueryResult queryResult = base.Folder.ItemQuery(ItemQueryType.None, null, this.sortBy, dataColumns))
				{
					this.LoadData(queryResult, startRange, endRange);
					return;
				}
			}
			this.SetRangeNull();
		}

		public override int LoadData(StoreObjectId storeObjectId, int itemsPerPage)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListViewDataSource.LoadData(Start)");
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			if (itemsPerPage <= 0)
			{
				throw new ArgumentOutOfRangeException("itemsPerPage", "itemsPerPage has to be greater than zero");
			}
			PropertyDefinition[] dataColumns = base.CreateProperyTable();
			int num = 1;
			if (0 < base.Folder.ItemCount)
			{
				using (QueryResult queryResult = base.Folder.ItemQuery(ItemQueryType.None, null, this.sortBy, dataColumns))
				{
					int estimatedRowCount = queryResult.EstimatedRowCount;
					queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, storeObjectId));
					int currentRow = queryResult.CurrentRow;
					num = currentRow / itemsPerPage + 1;
					if (num < 1 || currentRow >= estimatedRowCount)
					{
						num = 1;
					}
					int num2 = (num - 1) * itemsPerPage + 1;
					int endRange = num2 + itemsPerPage - 1;
					this.LoadData(queryResult, num2, endRange);
					return num;
				}
			}
			this.SetRangeNull();
			return num;
		}

		private void LoadData(QueryResult queryResult, int startRange, int endRange)
		{
			if (startRange < 1)
			{
				throw new ArgumentOutOfRangeException("startRange", "startRange must be greater than 0");
			}
			if (endRange < startRange)
			{
				throw new ArgumentOutOfRangeException("endRange", "endRange must be greater than or equal to startRange");
			}
			int estimatedRowCount = queryResult.EstimatedRowCount;
			if (estimatedRowCount < startRange)
			{
				base.Items = null;
			}
			else
			{
				queryResult.SeekToOffset(SeekReference.OriginBeginning, startRange - 1);
				if (estimatedRowCount < endRange)
				{
					endRange = estimatedRowCount;
				}
				int rowCount = endRange - startRange + 1;
				base.Items = Utilities.FetchRowsFromQueryResult(queryResult, rowCount);
			}
			if (base.Folder.ItemCount == 0 || base.Items == null || base.Items.Length == 0)
			{
				this.SetRangeNull();
				return;
			}
			base.StartRange = startRange;
			base.EndRange = startRange + base.Items.Length - 1;
		}

		private void SetRangeNull()
		{
			base.StartRange = 0;
			base.EndRange = -1;
		}

		private SortBy[] sortBy;
	}
}
