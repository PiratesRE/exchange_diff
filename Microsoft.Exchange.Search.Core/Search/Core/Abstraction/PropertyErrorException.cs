using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class PropertyErrorException : OperationFailedException
	{
		public PropertyErrorException(string property) : this(property, Strings.PropertyError(property), null)
		{
		}

		public PropertyErrorException(string property, Exception innerException) : this(property, Strings.PropertyError(property), innerException)
		{
		}

		protected PropertyErrorException(string property, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.property = property;
		}

		protected PropertyErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
		}

		private readonly string property;
	}
}
