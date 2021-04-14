using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetFunctionColumn : FunctionColumn, IJetColumn, IColumn
	{
		internal JetFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, params Column[] argumentColumns) : base(name, type, size, maxLength, table, function, functionName, argumentColumns)
		{
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			object value = this.GetValue(cursor);
			return JetColumnValueHelper.GetAsByteArray(value, this);
		}
	}
}
