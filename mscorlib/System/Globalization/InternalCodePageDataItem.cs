using System;
using System.Security;

namespace System.Globalization
{
	internal struct InternalCodePageDataItem
	{
		internal ushort codePage;

		internal ushort uiFamilyCodePage;

		internal uint flags;

		[SecurityCritical]
		internal unsafe sbyte* Names;
	}
}
