using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	[Flags]
	internal enum EhloOptionsFlags
	{
		None = 0,
		AnonymousTls = 1,
		UnusedCanBeRepurposed = 2,
		BinaryMime = 4,
		Chunking = 8,
		Dsn = 16,
		EightBitMime = 32,
		EnhancedStatusCodes = 64,
		Pipelining = 128,
		StartTls = 256,
		Xexch50 = 512,
		XlongAddr = 1024,
		Xoorg = 2048,
		Xorar = 4096,
		Xproxy = 8192,
		Xrdst = 16384,
		Xshadow = 32768,
		XproxyFrom = 65536,
		XshadowRequest = 131072,
		XAdrc = 262144,
		XExProps = 524288,
		XproxyTo = 1048576,
		XSessionMdbGuid = 2097152,
		XAttr = 4194304,
		XFastIndex = 8388608,
		XSysProbe = 16777216,
		XMsgId = 33554432,
		XrsetProxyTo = 67108864,
		SmtpUtf8 = 134217728,
		XOrigFrom = 268435456,
		XSessionType = 536870912,
		AllFlags = 268435455
	}
}
