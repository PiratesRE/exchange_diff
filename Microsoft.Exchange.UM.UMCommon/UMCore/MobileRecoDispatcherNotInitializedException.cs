using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MobileRecoDispatcherNotInitializedException : MobileRecoRequestCannotBeHandledException
	{
		public MobileRecoDispatcherNotInitializedException() : base(Strings.MobileRecoDispatcherNotInitialized)
		{
		}

		public MobileRecoDispatcherNotInitializedException(Exception innerException) : base(Strings.MobileRecoDispatcherNotInitialized, innerException)
		{
		}

		protected MobileRecoDispatcherNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
