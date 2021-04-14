using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetPropertyColumn : PropertyColumn, IJetColumn, IColumn
	{
		public JetPropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn) : base(name, type, size, maxLength, table, propTag, rowPropBagCreator, dependOn)
		{
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			object value = this.GetValue(cursor);
			return JetColumnValueHelper.GetAsByteArray(value, this);
		}
	}
}
