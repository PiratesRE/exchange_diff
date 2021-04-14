using System;
using System.Security;

namespace System.Globalization
{
	internal struct InternalEncodingDataItem
	{
		[SecurityCritical]
		internal unsafe sbyte* webName;

		internal ushort codePage;
	}
}
