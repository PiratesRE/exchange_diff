using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BadRequestException : LocalizedException
	{
		public BadRequestException() : base(Strings.BadRequestMessage)
		{
		}

		public BadRequestException(Exception innerException) : base(Strings.BadRequestMessage, innerException)
		{
		}

		protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
