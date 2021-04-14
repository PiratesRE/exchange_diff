using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupedQueryResult : QueryResult, IDisposable
	{
		internal GroupedQueryResult(MapiTable mapiTable, PropertyDefinition[] propertyDefinitions, IList<PropTag> alteredProperties, int rowTypeColumnIndex, bool isRowTypeInOriginalDataColumns, StoreSession session, int estimatedItemCount, SortOrder sortOrder) : base(mapiTable, propertyDefinitions, alteredProperties, session, true, sortOrder)
		{
			this.estimatedItemCount = estimatedItemCount;
			this.rowTypeColumnIndex = rowTypeColumnIndex;
			this.isRowTypeInOriginalDataColumns = isRowTypeInOriginalDataColumns;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GroupedQueryResult>(this);
		}

		public object[][][] GetViewArray(int rowCount)
		{
			object[][] rows = base.GetRows(rowCount);
			List<object[][]> list = new List<object[][]>();
			List<object[]> list2 = null;
			foreach (object[] row in rows)
			{
				int rowType = this.GetRowType(row);
				if (rowType == 1)
				{
					if (list2 == null)
					{
						list2 = new List<object[]>();
					}
					list2.Add(this.TrimRow(row));
				}
				else if (list2 != null)
				{
					list.Add(list2.ToArray());
					list2 = null;
				}
			}
			if (list2 != null)
			{
				list.Add(list2.ToArray());
			}
			return list.ToArray();
		}

		public int EstimatedItemCount
		{
			get
			{
				base.CheckDisposed("EstimatedItemCount");
				return this.estimatedItemCount;
			}
		}

		private int GetRowType(object[] row)
		{
			object obj = row[this.rowTypeColumnIndex];
			if (obj is int)
			{
				return (int)obj;
			}
			ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "GroupedQueryResult::GetRowType. Invalid row type. Row type found = {0}.", new object[]
			{
				obj
			});
			throw new CorruptDataException(ServerStrings.ExFoundInvalidRowType);
		}

		private object[] TrimRow(object[] row)
		{
			object[] array = row;
			if (!this.isRowTypeInOriginalDataColumns)
			{
				array = new object[row.Length - 1];
				Array.Copy(row, 0, array, 0, array.Length);
			}
			return array;
		}

		private readonly int rowTypeColumnIndex;

		private readonly bool isRowTypeInOriginalDataColumns;

		private readonly int estimatedItemCount;
	}
}
