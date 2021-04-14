using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UserNotAllowedForUMEnabledException : LocalizedException
	{
		public UserNotAllowedForUMEnabledException() : base(Strings.ExceptionUserNotAllowedForUMEnabled)
		{
		}

		public UserNotAllowedForUMEnabledException(Exception innerException) : base(Strings.ExceptionUserNotAllowedForUMEnabled, innerException)
		{
		}

		protected UserNotAllowedForUMEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
