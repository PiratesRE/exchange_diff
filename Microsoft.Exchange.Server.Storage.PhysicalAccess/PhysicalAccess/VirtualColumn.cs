using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class VirtualColumn : Column, IEquatable<VirtualColumn>
	{
		protected VirtualColumn(VirtualColumnId virtualColumnId, string name, Type type, bool nullable, Visibility visibility, int maxLength, int size, Table table) : base(name, type, nullable, visibility, maxLength, size, table)
		{
			this.virtualColumnId = virtualColumnId;
			base.CacheHashCode();
		}

		public VirtualColumnId VirtualColumnId
		{
			get
			{
				return this.virtualColumnId;
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
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails && base.Table != null)
			{
				sb.AppendFormat("(vid={0})", this.virtualColumnId);
			}
		}

		public bool Equals(VirtualColumn other)
		{
			return object.ReferenceEquals(this, other) || (!(other == null) && (this.VirtualColumnId == other.VirtualColumnId && base.Table == other.Table));
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			return this.Equals(other as VirtualColumn);
		}

		protected override int GetSize(ITWIR context)
		{
			return 0;
		}

		protected override object GetValue(ITWIR context)
		{
			return null;
		}

		private VirtualColumnId virtualColumnId;
	}
}
