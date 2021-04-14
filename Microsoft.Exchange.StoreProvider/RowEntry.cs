using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct RowEntry
	{
		public RowEntry.RowOp RowFlags
		{
			get
			{
				return this.rowOp;
			}
		}

		public ICollection<PropValue> Values
		{
			get
			{
				return this.propValues;
			}
		}

		public static RowEntry Add(ICollection<PropValue> propValues)
		{
			return new RowEntry(RowEntry.RowOp.Add, propValues);
		}

		public static RowEntry Modify(ICollection<PropValue> propValues)
		{
			return new RowEntry(RowEntry.RowOp.Modify, propValues);
		}

		public static RowEntry Remove(ICollection<PropValue> propValues)
		{
			return new RowEntry(RowEntry.RowOp.Remove, propValues);
		}

		public static RowEntry Empty()
		{
			return new RowEntry(RowEntry.RowOp.Empty, Array<PropValue>.Empty);
		}

		internal bool IsEmpty
		{
			get
			{
				return RowEntry.RowOp.Empty == this.rowOp && 0 == this.propValues.Count;
			}
		}

		private RowEntry(RowEntry.RowOp rowOp, ICollection<PropValue> propValues)
		{
			this.rowOp = rowOp;
			this.propValues = propValues;
		}

		private readonly RowEntry.RowOp rowOp;

		private readonly ICollection<PropValue> propValues;

		public enum RowOp
		{
			Add = 1,
			Modify,
			Remove = 4,
			Empty
		}
	}
}
