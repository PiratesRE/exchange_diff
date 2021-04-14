using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct MDBSTATUSRAW
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(MDBSTATUSRAW));

		internal Guid guidMdb;

		internal uint ulStatus;

		internal uint cbMdbName;

		internal uint cbVServerName;

		internal uint cbMdbLegacyDN;

		internal uint cbStorageGroupName;

		internal uint ibMdbName;

		internal uint ibVServerName;

		internal uint ibMdbLegacyDN;

		internal uint ibStorageGroupName;
	}
}
