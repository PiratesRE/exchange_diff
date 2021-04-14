using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class PropertyTypeErrorException : PropertyErrorException
	{
		public PropertyTypeErrorException(string property) : this(property, null)
		{
		}

		public PropertyTypeErrorException(string property, Exception innerException) : base(property, Strings.PropertyTypeError(property), innerException)
		{
		}

		protected PropertyTypeErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
