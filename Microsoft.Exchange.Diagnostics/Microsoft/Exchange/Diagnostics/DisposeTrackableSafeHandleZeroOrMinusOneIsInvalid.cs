using System;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid : SafeHandleZeroOrMinusOneIsInvalid, IDisposeTrackable, IDisposable
	{
		protected DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid() : this(IntPtr.Zero, true)
		{
		}

		protected DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid(IntPtr handle) : this(handle, true)
		{
		}

		protected DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid(IntPtr handle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(handle);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public abstract DisposeTracker GetDisposeTracker();

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private readonly DisposeTracker disposeTracker;
	}
}
