using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ActivityContextException : LocalizedException
	{
		public ActivityContextException(LocalizedString message) : base(message)
		{
		}

		public ActivityContextException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ActivityContextException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
