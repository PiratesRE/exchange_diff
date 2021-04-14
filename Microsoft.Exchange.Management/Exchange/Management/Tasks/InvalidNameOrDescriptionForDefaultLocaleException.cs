using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidNameOrDescriptionForDefaultLocaleException : LocalizedException
	{
		public InvalidNameOrDescriptionForDefaultLocaleException() : base(Strings.InvalidNameOrDescriptionForDefaultLocale)
		{
		}

		public InvalidNameOrDescriptionForDefaultLocaleException(Exception innerException) : base(Strings.InvalidNameOrDescriptionForDefaultLocale, innerException)
		{
		}

		protected InvalidNameOrDescriptionForDefaultLocaleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
