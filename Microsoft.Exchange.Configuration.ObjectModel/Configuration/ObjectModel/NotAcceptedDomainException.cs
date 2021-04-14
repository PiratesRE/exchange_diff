using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotAcceptedDomainException : LocalizedException
	{
		public NotAcceptedDomainException(LocalizedString message) : base(message)
		{
		}

		public NotAcceptedDomainException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NotAcceptedDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
