using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetConversionColumn : ConversionColumn, IJetColumn, IColumn
	{
		internal JetConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn) : base(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn)
		{
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			object value = this.GetValue(cursor);
			return JetColumnValueHelper.GetAsByteArray(value, this);
		}
	}
}
