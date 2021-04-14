using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class Column : IColumn, IEquatable<Column>
	{
		protected Column(string name, Type type, bool nullable, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table)
		{
			if (schemaExtension)
			{
				this.minVersion = int.MaxValue;
			}
			else
			{
				this.minVersion = 0;
			}
			this.maxVersion = int.MaxValue;
			this.table = table;
			this.name = name;
			this.type = type;
			this.extendedTypeCode = ValueTypeHelper.GetExtendedTypeCode(type);
			this.nullable = nullable;
			this.visibility = visibility;
			this.maxLength = maxLength;
			this.size = size;
		}

		protected Column(string name, Type type, bool nullable, Visibility visibility, int maxLength, int size, Table table) : this(name, type, nullable, false, visibility, maxLength, size, table)
		{
		}

		public virtual bool IsNullable
		{
			get
			{
				return this.nullable;
			}
		}

		public bool SchemaExtension
		{
			get
			{
				return this.minVersion != 0;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
			internal set
			{
				this.table = value;
				this.CacheHashCode();
			}
		}

		public int MinVersion
		{
			get
			{
				return this.minVersion;
			}
			set
			{
				this.minVersion = value;
			}
		}

		public int MaxVersion
		{
			get
			{
				return this.maxVersion;
			}
			set
			{
				this.maxVersion = value;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public ExtendedTypeCode ExtendedTypeCode
		{
			get
			{
				return this.extendedTypeCode;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Size
		{
			get
			{
				return this.size;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public virtual Column ActualColumn
		{
			get
			{
				return this;
			}
		}

		public virtual Column ColumnForEquality
		{
			get
			{
				return this;
			}
		}

		public virtual Column[] ArgumentColumns
		{
			get
			{
				return null;
			}
		}

		public static bool operator ==(Column col1, Column col2)
		{
			return object.ReferenceEquals(col1, col2) || (col1 != null && col2 != null && col1.Equals(col2));
		}

		public static bool operator !=(Column col1, Column col2)
		{
			return !(col1 == col2);
		}

		public object Evaluate(ITWIR twir)
		{
			return twir.GetColumnValue(this);
		}

		public virtual void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
		}

		public abstract void AppendToString(StringBuilder sb, StringFormatOptions formatOptions);

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendToString(stringBuilder, StringFormatOptions.IncludeDetails);
			return stringBuilder.ToString();
		}

		protected internal abstract bool ActualColumnEquals(Column other);

		public bool Equals(Column other)
		{
			return other != null && this.ActualColumnEquals(other.ColumnForEquality);
		}

		public override bool Equals(object other)
		{
			if (other != null)
			{
				Column column = other as Column;
				if (column != null)
				{
					return this.Equals(column);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		int IColumn.GetSize(ITWIR context)
		{
			if (!this.IsNullable && this.Size > 0)
			{
				return this.Size;
			}
			return this.GetSize(context);
		}

		object IColumn.GetValue(ITWIR context)
		{
			return this.GetValue(context);
		}

		protected abstract int GetSize(ITWIR context);

		protected abstract object GetValue(ITWIR context);

		protected void CacheHashCode()
		{
			this.hashCode = this.CalculateHashCode();
		}

		protected virtual int CalculateHashCode()
		{
			return this.Name.GetHashCode() ^ this.Type.GetHashCode() ^ ((this.Table == null) ? 0 : this.Table.GetHashCode());
		}

		public virtual void GetNameOrIdForSerialization(out string columnName, out uint columnId)
		{
			columnName = this.Name;
			columnId = 0U;
		}

		public bool TryGetColumnMaxSize(out int columnMaxSize)
		{
			columnMaxSize = Math.Max(this.MaxLength, this.Size);
			return true;
		}

		private readonly bool nullable;

		private readonly ExtendedTypeCode extendedTypeCode;

		private readonly int maxLength;

		private readonly Type type;

		protected string name;

		private readonly int size;

		private readonly Visibility visibility;

		private int minVersion;

		private int maxVersion;

		private Table table;

		private int hashCode;
	}
}
