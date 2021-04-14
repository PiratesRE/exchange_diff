using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum BodyCharsetFlags
	{
		None = 0,
		DetectCharset = 0,
		DisableCharsetDetection = 1,
		CharsetDetectionMask = 65535,
		PreferGB18030 = 65536,
		PreferIso885915 = 131072,
		PreserveUnicode = 262144,
		DoNotPreferIso2022jp = 1048576
	}
}
