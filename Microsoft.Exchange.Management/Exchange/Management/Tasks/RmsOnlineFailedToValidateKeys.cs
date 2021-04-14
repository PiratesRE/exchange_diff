using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsOnlineFailedToValidateKeys : LocalizedException
	{
		public RmsOnlineFailedToValidateKeys() : base(Strings.RmsOnlineFailedToValidateKeys)
		{
		}

		public RmsOnlineFailedToValidateKeys(Exception innerException) : base(Strings.RmsOnlineFailedToValidateKeys, innerException)
		{
		}

		protected RmsOnlineFailedToValidateKeys(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
