using System;

namespace Microsoft.Exchange.Data
{
	internal struct TypedValue
	{
		public TypedValue(StreamPropertyType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}

		public StreamPropertyType Type;

		public object Value;
	}
}
