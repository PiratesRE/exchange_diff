using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetSizeOfColumn : SizeOfColumn, IJetColumn, IColumn
	{
		internal JetSizeOfColumn(string name, Column termColumn, bool compressedSize) : base(name, termColumn, compressedSize)
		{
		}

		protected override object GetValue(ITWIR context)
		{
			if (base.CompressedSize)
			{
				JetPhysicalColumn jetPhysicalColumn = base.TermColumn as JetPhysicalColumn;
				if (jetPhysicalColumn != null)
				{
					JetTableOperator jetTableOperator = context as JetTableOperator;
					if (jetTableOperator != null)
					{
						return jetTableOperator.GetPhysicalColumnCompressedSize(jetPhysicalColumn);
					}
					JetJoinOperator jetJoinOperator = context as JetJoinOperator;
					if (jetJoinOperator != null)
					{
						return jetJoinOperator.GetPhysicalColumnCompressedSize(jetPhysicalColumn);
					}
				}
			}
			return base.GetValue(context);
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			object value = this.GetValue(cursor);
			return JetColumnValueHelper.GetAsByteArray(value, this);
		}
	}
}
