using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
	internal abstract class BaseConfigHandler
	{
		public BaseConfigHandler()
		{
			this.InitializeCallbacks();
		}

		private void InitializeCallbacks()
		{
			if (this.eventCallbacks == null)
			{
				this.eventCallbacks = new Delegate[6];
				this.eventCallbacks[0] = new BaseConfigHandler.NotifyEventCallback(this.NotifyEvent);
				this.eventCallbacks[1] = new BaseConfigHandler.BeginChildrenCallback(this.BeginChildren);
				this.eventCallbacks[2] = new BaseConfigHandler.EndChildrenCallback(this.EndChildren);
				this.eventCallbacks[3] = new BaseConfigHandler.ErrorCallback(this.Error);
				this.eventCallbacks[4] = new BaseConfigHandler.CreateNodeCallback(this.CreateNode);
				this.eventCallbacks[5] = new BaseConfigHandler.CreateAttributeCallback(this.CreateAttribute);
			}
		}

		public abstract void NotifyEvent(ConfigEvents nEvent);

		public abstract void BeginChildren(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		public abstract void EndChildren(int fEmpty, int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		public abstract void Error(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		public abstract void CreateNode(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		public abstract void CreateAttribute(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RunParser(string fileName);

		protected Delegate[] eventCallbacks;

		private delegate void NotifyEventCallback(ConfigEvents nEvent);

		private delegate void BeginChildrenCallback(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		private delegate void EndChildrenCallback(int fEmpty, int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		private delegate void ErrorCallback(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		private delegate void CreateNodeCallback(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);

		private delegate void CreateAttributeCallback(int size, ConfigNodeSubType subType, ConfigNodeType nType, int terminal, [MarshalAs(UnmanagedType.LPWStr)] string text, int textLength, int prefixLength);
	}
}
