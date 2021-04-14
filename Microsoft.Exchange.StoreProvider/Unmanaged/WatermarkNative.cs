using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct WatermarkNative
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(WatermarkNative));

		internal Guid mailboxGuid;

		internal Guid consumerGuid;

		internal long llEventCounter;
	}
}
