using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Core.LocStrings;

namespace Microsoft.Exchange.Configuration.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFlightingException : WinRMDataExchangeException
	{
		public InvalidFlightingException() : base(Strings.InvalidFlighting)
		{
		}

		public InvalidFlightingException(Exception innerException) : base(Strings.InvalidFlighting, innerException)
		{
		}

		protected InvalidFlightingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
