using System;

namespace System.Globalization
{
	internal sealed class AppDomainSortingSetupInfo
	{
		internal AppDomainSortingSetupInfo()
		{
		}

		internal AppDomainSortingSetupInfo(AppDomainSortingSetupInfo copy)
		{
			this._useV2LegacySorting = copy._useV2LegacySorting;
			this._useV4LegacySorting = copy._useV4LegacySorting;
			this._pfnIsNLSDefinedString = copy._pfnIsNLSDefinedString;
			this._pfnCompareStringEx = copy._pfnCompareStringEx;
			this._pfnLCMapStringEx = copy._pfnLCMapStringEx;
			this._pfnFindNLSStringEx = copy._pfnFindNLSStringEx;
			this._pfnFindStringOrdinal = copy._pfnFindStringOrdinal;
			this._pfnCompareStringOrdinal = copy._pfnCompareStringOrdinal;
			this._pfnGetNLSVersionEx = copy._pfnGetNLSVersionEx;
		}

		internal IntPtr _pfnIsNLSDefinedString;

		internal IntPtr _pfnCompareStringEx;

		internal IntPtr _pfnLCMapStringEx;

		internal IntPtr _pfnFindNLSStringEx;

		internal IntPtr _pfnCompareStringOrdinal;

		internal IntPtr _pfnGetNLSVersionEx;

		internal IntPtr _pfnFindStringOrdinal;

		internal bool _useV2LegacySorting;

		internal bool _useV4LegacySorting;
	}
}
