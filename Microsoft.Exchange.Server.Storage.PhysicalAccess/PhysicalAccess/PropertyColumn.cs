using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class PropertyColumn : ExtendedPropertyColumn
	{
		protected PropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn) : base(name, type, PropertyColumn.GetVisibility(propTag, dependOn), size, maxLength, table, propTag)
		{
			this.rowPropBagCreator = rowPropBagCreator;
			this.dependOn = dependOn;
			base.CacheHashCode();
		}

		public Func<IRowAccess, IRowPropertyBag> PropertyBagCreator
		{
			get
			{
				return this.rowPropBagCreator;
			}
		}

		public Column[] DependOn
		{
			get
			{
				return this.dependOn;
			}
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
			if (this.dependOn != null)
			{
				foreach (Column column in this.dependOn)
				{
					column.EnumerateColumns(callback, state);
				}
			}
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append(this.Name);
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
			{
				sb.Append(":PROP[");
				base.StorePropTag.AppendToString(sb, true);
				sb.Append("]");
			}
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			ExtendedPropertyColumn extendedPropertyColumn = other as ExtendedPropertyColumn;
			return extendedPropertyColumn != null && base.StorePropTag == extendedPropertyColumn.StorePropTag && base.Table == extendedPropertyColumn.Table;
		}

		protected override int CalculateHashCode()
		{
			return ((base.Table == null) ? 0 : base.Table.GetHashCode()) ^ (int)base.StorePropTag.PropTag;
		}

		protected override int GetSize(ITWIR context)
		{
			return context.GetPropertyColumnSize(this);
		}

		protected override object GetValue(ITWIR context)
		{
			return context.GetPropertyColumnValue(this);
		}

		private static Visibility GetVisibility(StorePropTag propTag, Column[] columns)
		{
			Visibility v = (propTag.PropInfo != null) ? propTag.PropInfo.Visibility : Visibility.Public;
			Visibility visibility;
			if (columns == null || columns.Length <= 0)
			{
				visibility = Visibility.Public;
			}
			else
			{
				visibility = VisibilityHelper.Select(from col in columns
				select col.Visibility);
			}
			Visibility v2 = visibility;
			return VisibilityHelper.Select(v, v2);
		}

		private readonly Func<IRowAccess, IRowPropertyBag> rowPropBagCreator;

		private readonly Column[] dependOn;
	}
}
