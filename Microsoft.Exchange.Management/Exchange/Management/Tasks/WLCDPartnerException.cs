using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDPartnerException : RecipientTaskException
	{
		public WLCDPartnerException(LocalizedString message) : base(message)
		{
		}

		public WLCDPartnerException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDPartnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
