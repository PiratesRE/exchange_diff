using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum GCHandleType
	{
		[__DynamicallyInvokable]
		Weak,
		[__DynamicallyInvokable]
		WeakTrackResurrection,
		[__DynamicallyInvokable]
		Normal,
		[__DynamicallyInvokable]
		Pinned
	}
}
