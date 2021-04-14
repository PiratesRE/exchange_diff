using System;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusterNotifyHandle : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		public AmClusterNotifyHandle() : base(true)
		{
			base.SetHandle((IntPtr)(-1));
		}

		public static AmClusterNotifyHandle InvalidHandle
		{
			get
			{
				return AmClusterNotifyHandle.sm_invalidHandle;
			}
		}

		internal void DangerousCloseHandle()
		{
			this.CloseNotifyPort();
		}

		protected override bool ReleaseHandle()
		{
			return this.CloseNotifyPort();
		}

		private bool CloseNotifyPort()
		{
			bool result = true;
			lock (this)
			{
				if (!this.m_isClosed)
				{
					this.m_isClosed = true;
					if (!this.IsInvalid)
					{
						AmTrace.Debug("Calling CloseClusterNotifyPort() (handle=0x{0:x})", new object[]
						{
							this.handle
						});
						try
						{
							result = ClusapiMethods.CloseClusterNotifyPort(this.handle);
							goto IL_BD;
						}
						catch (AccessViolationException ex)
						{
							AmTrace.Error("Ignoring AccessViolationException exception while Closing cluster notify port (error={0})", new object[]
							{
								ex
							});
							goto IL_BD;
						}
					}
					AmTrace.Debug("Skipped CloseClusterNotifyPort() since handle is invalid (handle=0x{0:x})", new object[]
					{
						this.handle
					});
				}
				else
				{
					AmTrace.Debug("Skipped CloseClusterNotifyPort() the handle was closed already (handle=0x{0:x})", new object[]
					{
						this.handle
					});
				}
				IL_BD:;
			}
			return result;
		}

		private static AmClusterNotifyHandle sm_invalidHandle = new AmClusterNotifyHandle();

		private bool m_isClosed;
	}
}
