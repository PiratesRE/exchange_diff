using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class ConstantColumn : Column
	{
		protected ConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value) : base(name, type, value == null, visibility, maxLength, size, null)
		{
			this.value = value;
			base.CacheHashCode();
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			if (this.Name != null)
			{
				sb.Append(this.Name);
				sb.Append(":");
			}
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
			{
				sb.Append("CONST");
			}
			sb.Append("[");
			if ((formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.None)
			{
				sb.AppendAsString(this.Value);
			}
			else
			{
				sb.Append((this.Value != null) ? "X" : "Null");
			}
			sb.Append("]");
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			ConstantColumn constantColumn = other as ConstantColumn;
			return constantColumn != null && object.ReferenceEquals(base.Type, constantColumn.Type) && ValueHelper.ValuesEqual(this.Value, constantColumn.Value);
		}

		protected override int CalculateHashCode()
		{
			if (this.Value != null)
			{
				return this.Value.GetHashCode();
			}
			return 0;
		}

		protected override int GetSize(ITWIR context)
		{
			return SizeOfColumn.GetColumnSize(this, this.Value).GetValueOrDefault();
		}

		protected override object GetValue(ITWIR context)
		{
			return this.value;
		}

		private readonly object value;
	}
}
