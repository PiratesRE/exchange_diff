using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class DataColumnViewAttribute : Attribute
	{
		public DataColumnViewAttribute(RowType rowType)
		{
			this.rowTypeBits = rowType;
		}

		public bool IsViewOf(RowType rowType)
		{
			return (this.rowTypeBits & rowType) == rowType;
		}

		private readonly RowType rowTypeBits;
	}
}
