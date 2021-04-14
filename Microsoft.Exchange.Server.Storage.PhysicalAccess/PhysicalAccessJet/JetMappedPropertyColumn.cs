using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetMappedPropertyColumn : MappedPropertyColumn, IJetColumn, IColumn
	{
		internal JetMappedPropertyColumn(Column actualColumn, StorePropTag propTag) : base(actualColumn, propTag)
		{
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			return cursor.GetColumnValueAsBytes(this.ActualColumn);
		}
	}
}
