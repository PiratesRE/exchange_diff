using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetConstantColumn : ConstantColumn, IJetColumn, IColumn
	{
		internal JetConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value) : base(name, type, visibility, size, maxLength, value)
		{
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			object value = this.GetValue(cursor);
			return JetColumnValueHelper.GetAsByteArray(value, this);
		}
	}
}
