using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class PhysicalColumn : Column, IEquatable<PhysicalColumn>
	{
		protected PhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength) : base(name, type, nullable, schemaExtension, visibility, maxLength, size, table)
		{
			this.physicalName = physicalName;
			this.index = index;
			this.identity = identity;
			this.streamSupport = streamSupport;
			this.notFetchedByDefault = notFetchedByDefault;
			this.maxInlineLength = maxInlineLength;
			base.CacheHashCode();
		}

		protected PhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength) : this(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, false, visibility, maxLength, size, table, index, maxInlineLength)
		{
		}

		public bool IsIdentity
		{
			get
			{
				return this.identity;
			}
		}

		public bool StreamSupport
		{
			get
			{
				return this.streamSupport;
			}
		}

		public bool NotFetchedByDefault
		{
			get
			{
				return this.notFetchedByDefault;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public int MaxInlineLength
		{
			get
			{
				return this.maxInlineLength;
			}
		}

		public string PhysicalName
		{
			get
			{
				return this.physicalName ?? this.Name;
			}
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails && base.Table != null)
			{
				sb.Append(base.Table.Name);
				sb.Append(".");
			}
			sb.Append(this.Name);
		}

		public bool Equals(PhysicalColumn other)
		{
			return object.ReferenceEquals(this, other);
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			return this.Equals(other as PhysicalColumn);
		}

		protected override int GetSize(ITWIR context)
		{
			return context.GetPhysicalColumnSize(this);
		}

		protected override object GetValue(ITWIR context)
		{
			return context.GetPhysicalColumnValue(this);
		}

		public override void GetNameOrIdForSerialization(out string columnName, out uint columnId)
		{
			columnName = this.Name;
			columnId = 0U;
		}

		private readonly bool identity;

		private readonly bool streamSupport;

		private readonly bool notFetchedByDefault;

		private readonly int maxInlineLength;

		private readonly string physicalName;

		private int index = -1;
	}
}
