using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Common.LocStrings
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReadOnlyPropertyBagException : LocalizedException
	{
		public ReadOnlyPropertyBagException() : base(Strings.ExceptionReadOnlyPropertyBag)
		{
		}

		public ReadOnlyPropertyBagException(Exception innerException) : base(Strings.ExceptionReadOnlyPropertyBag, innerException)
		{
		}

		protected ReadOnlyPropertyBagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
