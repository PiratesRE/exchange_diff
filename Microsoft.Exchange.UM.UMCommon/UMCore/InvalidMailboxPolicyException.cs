using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidMailboxPolicyException : LocalizedException
	{
		public InvalidMailboxPolicyException(LocalizedString message) : base(message)
		{
		}

		public InvalidMailboxPolicyException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidMailboxPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
