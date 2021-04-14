using System;
using System.Security;
using Microsoft.Win32;

namespace System.IO
{
	internal abstract class SearchResultHandler<TSource>
	{
		[SecurityCritical]
		internal abstract bool IsResultIncluded(ref Win32Native.WIN32_FIND_DATA findData);

		[SecurityCritical]
		internal abstract TSource CreateObject(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData);
	}
}
