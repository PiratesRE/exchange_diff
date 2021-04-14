using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _Actions
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_Actions));

		internal uint ulVersion;

		internal uint cActions;

		internal unsafe _Action* lpAction;
	}
}
