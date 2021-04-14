using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExMapiAttachHandle : SafeExMapiPropHandle, IExMapiAttach, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiAttachHandle()
		{
		}

		internal SafeExMapiAttachHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiAttachHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiAttachHandle>(this);
		}
	}
}
