using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MobileRecoRequestCannotBeHandledException : LocalizedException
	{
		public MobileRecoRequestCannotBeHandledException(LocalizedString message) : base(message)
		{
		}

		public MobileRecoRequestCannotBeHandledException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MobileRecoRequestCannotBeHandledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
