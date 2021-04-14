using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class ConversionColumn : Column
	{
		protected ConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn) : base(name, type, argumentColumn.IsNullable, argumentColumn.Visibility, maxLength, size, table)
		{
			this.conversionFunction = conversionFunction;
			this.functionName = functionName;
			this.argumentColumns[0] = argumentColumn;
			base.CacheHashCode();
		}

		public override bool IsNullable
		{
			get
			{
				return this.ArgumentColumn.IsNullable;
			}
		}

		public Func<object, object> ConversionFunction
		{
			get
			{
				return this.conversionFunction;
			}
		}

		public string FunctionName
		{
			get
			{
				return this.functionName;
			}
		}

		public Column ArgumentColumn
		{
			get
			{
				return this.argumentColumns[0];
			}
		}

		public override Column[] ArgumentColumns
		{
			get
			{
				return this.argumentColumns;
			}
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
			this.ArgumentColumn.EnumerateColumns(callback, state);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append(this.Name);
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
			{
				sb.Append(":CVT[");
				sb.Append(this.functionName);
				sb.Append("(");
				this.ArgumentColumn.AppendToString(sb, formatOptions);
				sb.Append(")]");
			}
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			ConversionColumn conversionColumn = other as ConversionColumn;
			return conversionColumn != null && this.ConversionFunction == conversionColumn.ConversionFunction && this.FunctionName == conversionColumn.FunctionName && this.ArgumentColumn == conversionColumn.ArgumentColumn && this.Name == conversionColumn.Name;
		}

		protected override int CalculateHashCode()
		{
			int num = this.ConversionFunction.GetHashCode() ^ this.ArgumentColumn.GetHashCode() ^ this.Name.GetHashCode();
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
			object obj = this.ArgumentColumn.Evaluate(context);
			if (obj == null)
			{
				return null;
			}
			return this.conversionFunction(obj);
		}

		private readonly Func<object, object> conversionFunction;

		private readonly string functionName;

		private readonly Column[] argumentColumns = new Column[1];
	}
}
