using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface IDataTypeResolver
	{
		Type Resolve(string typeName, Type targetType);
	}
}
