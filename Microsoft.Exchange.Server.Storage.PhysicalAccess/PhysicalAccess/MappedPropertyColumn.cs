using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class MappedPropertyColumn : ExtendedPropertyColumn
	{
		protected MappedPropertyColumn(Column actualColumn, StorePropTag propTag) : base(actualColumn.Name, actualColumn.Type, actualColumn.IsNullable, MappedPropertyColumn.GetVisibility(propTag, actualColumn), actualColumn.Size, actualColumn.MaxLength, actualColumn.Table, propTag)
		{
			this.actualColumn = actualColumn;
			base.CacheHashCode();
		}

		public override Column ActualColumn
		{
			get
			{
				return this.actualColumn;
			}
		}

		public override Column ColumnForEquality
		{
			get
			{
				Column column = this.ActualColumn;
				while (column is MappedPropertyColumn)
				{
					column = column.ActualColumn;
				}
				return column;
			}
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
			this.actualColumn.EnumerateColumns(callback, state);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append(this.Name);
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
			{
				sb.Append(":MAP[");
				base.StorePropTag.AppendToString(sb, true);
				sb.Append("->");
				this.actualColumn.AppendToString(sb, formatOptions);
				sb.Append("]");
			}
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			return this.actualColumn.ActualColumnEquals(other);
		}

		protected override int CalculateHashCode()
		{
			return this.actualColumn.GetHashCode();
		}

		protected override int GetSize(ITWIR context)
		{
			return ((IColumn)this.actualColumn).GetSize(context);
		}

		protected override object GetValue(ITWIR context)
		{
			return ((IColumn)this.actualColumn).GetValue(context);
		}

		private static Visibility GetVisibility(StorePropTag propTag, Column column)
		{
			Visibility v = (propTag.PropInfo != null) ? propTag.PropInfo.Visibility : Visibility.Public;
			Visibility v2 = (column != null) ? column.Visibility : Visibility.Public;
			return VisibilityHelper.Select(v, v2);
		}

		private Column actualColumn;
	}
}
