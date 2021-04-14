using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class FunctionColumn : Column
	{
		protected FunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, params Column[] argumentColumns) : base(name, type, true, VisibilityHelper.Select(from col in argumentColumns
		select col.Visibility), maxLength, size, table)
		{
			this.function = function;
			this.functionName = functionName;
			this.argumentColumns = argumentColumns;
			base.CacheHashCode();
		}

		public Func<object[], object> Function
		{
			get
			{
				return this.function;
			}
		}

		public string FunctionName
		{
			get
			{
				return this.functionName;
			}
		}

		public override Column[] ArgumentColumns
		{
			get
			{
				return this.argumentColumns;
			}
		}

		public static FunctionColumn CreateEscrowUpdateFunctionColumn(Table table, Column column, int deltaValue)
		{
			return Factory.CreateFunctionColumn("EscrowUpdateFunctionColumn", typeof(int), PropertyTypeHelper.SizeFromPropType(PropertyType.Int32), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Int32), table, delegate(object[] columnValues)
			{
				int num = (int)columnValues[0];
				int val = num + deltaValue;
				return Math.Max(0, val);
			}, "EscrowUpdateFunction", new Column[]
			{
				column
			});
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
			if (this.argumentColumns != null)
			{
				foreach (Column column in this.argumentColumns)
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
				sb.Append(":FNC[");
				sb.Append(this.functionName);
				sb.Append("(");
				if (this.argumentColumns != null)
				{
					for (int i = 0; i < this.argumentColumns.Length; i++)
					{
						if (i != 0)
						{
							sb.Append(", ");
						}
						this.argumentColumns[i].AppendToString(sb, formatOptions);
					}
				}
				sb.Append(")]");
			}
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			FunctionColumn functionColumn = other as FunctionColumn;
			if (functionColumn != null && this.Name == functionColumn.Name && this.Function == functionColumn.Function && this.FunctionName == functionColumn.FunctionName && (object.ReferenceEquals(this.ArgumentColumns, functionColumn.ArgumentColumns) || (this.ArgumentColumns != null && functionColumn.ArgumentColumns != null && this.ArgumentColumns.Length == functionColumn.ArgumentColumns.Length)))
			{
				if (!object.ReferenceEquals(this.ArgumentColumns, functionColumn.ArgumentColumns))
				{
					for (int i = 0; i < this.ArgumentColumns.Length; i++)
					{
						if (!this.ArgumentColumns[i].Equals(functionColumn.ArgumentColumns[i]))
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		protected override int CalculateHashCode()
		{
			int num = this.Name.GetHashCode() ^ this.Function.GetHashCode();
			if (this.ArgumentColumns != null)
			{
				foreach (Column column in this.ArgumentColumns)
				{
					num ^= column.GetHashCode();
				}
			}
			if (this.FunctionName != null)
			{
				num ^= this.FunctionName.GetHashCode();
			}
			return num;
		}

		protected override int GetSize(ITWIR context)
		{
			object value = this.GetValue(context);
			return SizeOfColumn.GetColumnSize(this, value).GetValueOrDefault();
		}

		protected override object GetValue(ITWIR context)
		{
			object[] array = null;
			if (this.ArgumentColumns != null)
			{
				array = new object[this.ArgumentColumns.Length];
				for (int i = 0; i < this.ArgumentColumns.Length; i++)
				{
					array[i] = this.argumentColumns[i].Evaluate(context);
					if (array[i] is LargeValue && context is IColumnStreamAccess && this.argumentColumns[i] is PhysicalColumn)
					{
						array[i] = this.GetStreamableColumnValue((IColumnStreamAccess)context, (PhysicalColumn)this.argumentColumns[i]);
					}
				}
			}
			return this.function(array);
		}

		private byte[] GetStreamableColumnValue(IColumnStreamAccess streamAccess, PhysicalColumn column)
		{
			byte[] result;
			using (PhysicalColumnStream physicalColumnStream = new PhysicalColumnStream(streamAccess, column, true))
			{
				byte[] array = new byte[physicalColumnStream.Length];
				physicalColumnStream.Read(array, 0, array.Length);
				result = array;
			}
			return result;
		}

		private readonly Func<object[], object> function;

		private readonly string functionName;

		private readonly Column[] argumentColumns;
	}
}
