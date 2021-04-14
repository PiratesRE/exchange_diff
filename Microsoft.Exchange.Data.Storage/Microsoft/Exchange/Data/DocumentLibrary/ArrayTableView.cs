using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ArrayTableView : ITableView, IPagedView
	{
		internal ArrayTableView(QueryFilter queryFilter, SortBy[] sortBy, PropertyDefinition[] propertyDefinitions, List<object[]> arrayView)
		{
			if (arrayView == null)
			{
				throw new ArgumentNullException("arrayView");
			}
			this.propertyDefinitions = propertyDefinitions;
			this.currentRow = 0;
			if (queryFilter != null)
			{
				this.arrayView = new List<object[]>();
				Query query = Query.BuildQuery(queryFilter, propertyDefinitions, 6);
				for (int i = 0; i < this.arrayView.Count; i++)
				{
					if (query.IsMatch(arrayView[i]))
					{
						this.arrayView.Add(arrayView[i]);
					}
				}
			}
			else
			{
				this.arrayView = new List<object[]>(arrayView);
			}
			if (sortBy != null)
			{
				this.arrayView.Sort(new SortByComparer(sortBy, this.propertyDefinitions));
			}
		}

		public int EstimatedRowCount
		{
			get
			{
				return this.arrayView.Count;
			}
		}

		public int CurrentRow
		{
			get
			{
				return this.currentRow;
			}
		}

		public bool SeekToCondition(SeekReference seekReference, QueryFilter seekFilter)
		{
			Query query = Query.BuildQuery(seekFilter, this.propertyDefinitions, 6);
			int num = 0;
			int num2 = 1;
			switch (seekReference)
			{
			case SeekReference.OriginBeginning:
				num = 0;
				num2 = 1;
				break;
			case SeekReference.OriginCurrent:
				num = this.currentRow;
				num2 = 1;
				break;
			case SeekReference.BackwardFromCurrent:
				num = this.currentRow;
				num2 = -1;
				break;
			case SeekReference.BackwardFromEnd:
				num = this.arrayView.Count - 1;
				num2 = -1;
				break;
			}
			while (num >= 0 && num < this.arrayView.Count)
			{
				if (query.IsMatch(this.arrayView[num]))
				{
					this.currentRow = num;
					return true;
				}
				num += num2;
			}
			this.currentRow = this.arrayView.Count;
			return false;
		}

		public int SeekToOffset(SeekReference seekReference, int offset)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			int num;
			switch (seekReference)
			{
			case SeekReference.OriginBeginning:
				num = offset;
				goto IL_6C;
			case SeekReference.OriginCurrent:
				num = this.currentRow + offset;
				goto IL_6C;
			case SeekReference.BackwardFromCurrent:
				num = this.currentRow - offset;
				goto IL_6C;
			case SeekReference.BackwardFromEnd:
				num = this.arrayView.Count - offset;
				goto IL_6C;
			}
			throw new ArgumentOutOfRangeException("seekReference");
			IL_6C:
			if (num < 0)
			{
				num = 0;
			}
			else if (num > this.arrayView.Count)
			{
				num = this.arrayView.Count;
			}
			this.currentRow = num;
			return this.currentRow;
		}

		public object[][] GetRows(int rowCount)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount");
			}
			if (this.arrayView.Count == 0)
			{
				return Array<object[]>.Empty;
			}
			int num;
			if (rowCount > 2147483647 - this.currentRow)
			{
				num = this.arrayView.Count - this.currentRow;
			}
			else
			{
				num = ((rowCount + this.currentRow > this.arrayView.Count) ? (this.arrayView.Count - this.currentRow) : rowCount);
			}
			object[][] array = new object[num][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (object[])this.arrayView[this.currentRow + i].Clone();
			}
			this.currentRow += num;
			return array;
		}

		private readonly List<object[]> arrayView;

		private readonly PropertyDefinition[] propertyDefinitions;

		private int currentRow;
	}
}
