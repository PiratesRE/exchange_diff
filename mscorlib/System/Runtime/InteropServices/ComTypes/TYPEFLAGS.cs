using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum TYPEFLAGS : short
	{
		[__DynamicallyInvokable]
		TYPEFLAG_FAPPOBJECT = 1,
		[__DynamicallyInvokable]
		TYPEFLAG_FCANCREATE = 2,
		[__DynamicallyInvokable]
		TYPEFLAG_FLICENSED = 4,
		[__DynamicallyInvokable]
		TYPEFLAG_FPREDECLID = 8,
		[__DynamicallyInvokable]
		TYPEFLAG_FHIDDEN = 16,
		[__DynamicallyInvokable]
		TYPEFLAG_FCONTROL = 32,
		[__DynamicallyInvokable]
		TYPEFLAG_FDUAL = 64,
		[__DynamicallyInvokable]
		TYPEFLAG_FNONEXTENSIBLE = 128,
		[__DynamicallyInvokable]
		TYPEFLAG_FOLEAUTOMATION = 256,
		[__DynamicallyInvokable]
		TYPEFLAG_FRESTRICTED = 512,
		[__DynamicallyInvokable]
		TYPEFLAG_FAGGREGATABLE = 1024,
		[__DynamicallyInvokable]
		TYPEFLAG_FREPLACEABLE = 2048,
		[__DynamicallyInvokable]
		TYPEFLAG_FDISPATCHABLE = 4096,
		[__DynamicallyInvokable]
		TYPEFLAG_FREVERSEBIND = 8192,
		[__DynamicallyInvokable]
		TYPEFLAG_FPROXY = 16384
	}
}
