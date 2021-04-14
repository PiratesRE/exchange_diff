using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetVirtualColumn : VirtualColumn, IJetColumn, IColumn
	{
		internal JetVirtualColumn(VirtualColumnId virtualColumnId, string name, Type type, bool nullable, Visibility visibility, int maxLength, int size, Table table) : base(virtualColumnId, name, type, nullable, visibility, maxLength, size, table)
		{
		}

		public byte[] GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			JetTableOperator jetTableOperator = cursor as JetTableOperator;
			if (jetTableOperator != null)
			{
				return jetTableOperator.GetVirtualColumnValueAsBytes(this);
			}
			return null;
		}

		protected override int GetSize(ITWIR context)
		{
			JetTableOperator jetTableOperator = context as JetTableOperator;
			if (jetTableOperator != null)
			{
				return jetTableOperator.GetVirtualColumnSize(this);
			}
			return 0;
		}

		protected override object GetValue(ITWIR context)
		{
			JetTableOperator jetTableOperator = context as JetTableOperator;
			if (jetTableOperator != null)
			{
				return jetTableOperator.GetVirtualColumnValue(this);
			}
			return null;
		}
	}
}
