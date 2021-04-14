using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IObjectLogPropertyDefinition<T>
	{
		string FieldName { get; }

		object GetValue(T objectToLog);
	}
}
