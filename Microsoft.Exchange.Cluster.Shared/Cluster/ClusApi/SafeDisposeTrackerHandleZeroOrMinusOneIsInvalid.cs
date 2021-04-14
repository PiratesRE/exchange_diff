using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal abstract class SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid : SafeHandleZeroOrMinusOneIsInvalid, IDisposeTrackable, IDisposable
	{
		public SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid() : base(true)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			base.SetHandle(IntPtr.Zero);
		}

		public SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid(bool ownsHandleLifetime) : base(ownsHandleLifetime)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			base.SetHandle(IntPtr.Zero);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (disposing && this.m_disposeTracker != null)
				{
					this.m_disposeTracker.Dispose();
					this.m_disposeTracker = null;
				}
				this.m_disposed = true;
				base.Dispose(disposing);
			}
		}

		private DisposeTracker m_disposeTracker;

		private bool m_disposed;
	}
}
