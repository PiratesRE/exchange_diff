using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidSipNameDomainException : LocalizedException
	{
		public InvalidSipNameDomainException() : base(Strings.ExceptionInvalidSipNameDomain)
		{
		}

		public InvalidSipNameDomainException(Exception innerException) : base(Strings.ExceptionInvalidSipNameDomain, innerException)
		{
		}

		protected InvalidSipNameDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
