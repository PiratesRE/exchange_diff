using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TypeConversionException : LocalizedException
	{
		public TypeConversionException(LocalizedString message) : base(message)
		{
		}

		public TypeConversionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TypeConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
