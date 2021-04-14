using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MobileRecoDispatcherStoppingException : MobileRecoRequestCannotBeHandledException
	{
		public MobileRecoDispatcherStoppingException() : base(Strings.MobileRecoDispatcherStopping)
		{
		}

		public MobileRecoDispatcherStoppingException(Exception innerException) : base(Strings.MobileRecoDispatcherStopping, innerException)
		{
		}

		protected MobileRecoDispatcherStoppingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
