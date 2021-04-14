using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPilotIdentiferException : LocalizedException
	{
		public InvalidPilotIdentiferException(LocalizedString message) : base(message)
		{
		}

		public InvalidPilotIdentiferException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPilotIdentiferException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
