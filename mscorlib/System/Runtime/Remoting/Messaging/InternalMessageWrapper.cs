using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[SecurityCritical]
	[ComVisible(true)]
	public class InternalMessageWrapper
	{
		public InternalMessageWrapper(IMessage msg)
		{
			this.WrappedMessage = msg;
		}

		[SecurityCritical]
		internal object GetIdentityObject()
		{
			IInternalMessage internalMessage = this.WrappedMessage as IInternalMessage;
			if (internalMessage != null)
			{
				return internalMessage.IdentityObject;
			}
			InternalMessageWrapper internalMessageWrapper = this.WrappedMessage as InternalMessageWrapper;
			if (internalMessageWrapper != null)
			{
				return internalMessageWrapper.GetIdentityObject();
			}
			return null;
		}

		[SecurityCritical]
		internal object GetServerIdentityObject()
		{
			IInternalMessage internalMessage = this.WrappedMessage as IInternalMessage;
			if (internalMessage != null)
			{
				return internalMessage.ServerIdentityObject;
			}
			InternalMessageWrapper internalMessageWrapper = this.WrappedMessage as InternalMessageWrapper;
			if (internalMessageWrapper != null)
			{
				return internalMessageWrapper.GetServerIdentityObject();
			}
			return null;
		}

		protected IMessage WrappedMessage;
	}
}
