using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("The IDispatchImplAttribute is deprecated.", false)]
	[ComVisible(true)]
	[Serializable]
	public enum IDispatchImplType
	{
		SystemDefinedImpl,
		InternalImpl,
		CompatibleImpl
	}
}
