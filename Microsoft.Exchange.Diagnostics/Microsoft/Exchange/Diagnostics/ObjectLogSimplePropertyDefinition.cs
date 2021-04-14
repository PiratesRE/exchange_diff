using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ObjectLogSimplePropertyDefinition<T> : IObjectLogPropertyDefinition<T>
	{
		public ObjectLogSimplePropertyDefinition(string fieldName, Func<T, object> getValueFunc)
		{
			this.fieldName = fieldName;
			this.getValueFunc = getValueFunc;
		}

		string IObjectLogPropertyDefinition<!0>.FieldName
		{
			get
			{
				return this.fieldName;
			}
		}

		object IObjectLogPropertyDefinition<!0>.GetValue(T objectToLog)
		{
			return this.getValueFunc(objectToLog);
		}

		private string fieldName;

		private Func<T, object> getValueFunc;
	}
}
