using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.Authentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LowPasswordConfidenceException : LocalizedException
	{
		public LowPasswordConfidenceException(LocalizedString message) : base(message)
		{
		}

		public LowPasswordConfidenceException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LowPasswordConfidenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
