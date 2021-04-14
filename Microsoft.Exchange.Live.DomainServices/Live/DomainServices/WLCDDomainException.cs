using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Live.DomainServices
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDDomainException : WLCDPartnerException
	{
		public WLCDDomainException(LocalizedString message) : base(message)
		{
		}

		public WLCDDomainException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
