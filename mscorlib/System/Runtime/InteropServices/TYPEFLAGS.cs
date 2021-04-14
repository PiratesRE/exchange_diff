﻿using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.TYPEFLAGS instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Flags]
	[Serializable]
	public enum TYPEFLAGS : short
	{
		TYPEFLAG_FAPPOBJECT = 1,
		TYPEFLAG_FCANCREATE = 2,
		TYPEFLAG_FLICENSED = 4,
		TYPEFLAG_FPREDECLID = 8,
		TYPEFLAG_FHIDDEN = 16,
		TYPEFLAG_FCONTROL = 32,
		TYPEFLAG_FDUAL = 64,
		TYPEFLAG_FNONEXTENSIBLE = 128,
		TYPEFLAG_FOLEAUTOMATION = 256,
		TYPEFLAG_FRESTRICTED = 512,
		TYPEFLAG_FAGGREGATABLE = 1024,
		TYPEFLAG_FREPLACEABLE = 2048,
		TYPEFLAG_FDISPATCHABLE = 4096,
		TYPEFLAG_FREVERSEBIND = 8192,
		TYPEFLAG_FPROXY = 16384
	}
}
