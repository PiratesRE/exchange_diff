using System;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal enum STGC
	{
		STGC_DEFAULT,
		STGC_OVERWRITE,
		STGC_ONLYIFCURRENT,
		STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
		STGC_CONSOLIDATE = 8
	}
}
