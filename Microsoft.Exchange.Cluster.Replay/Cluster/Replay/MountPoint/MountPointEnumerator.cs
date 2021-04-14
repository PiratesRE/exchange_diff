using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Cluster.Shared.MountPoint;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal class MountPointEnumerator : DisposeTrackableBase
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.VolumeManagerTracer;
			}
		}

		public MountPointEnumerator(string volumeName)
		{
			this.m_volumeName = volumeName;
		}

		public IEnumerable<MountedFolderPath> GetMountPoints()
		{
			string mountPoint;
			while (this.GetNextMountPoint(out mountPoint))
			{
				yield return new MountedFolderPath(mountPoint);
			}
			yield break;
		}

		private bool GetNextMountPoint(out string mountPoint)
		{
			int num = 260;
			StringBuilder stringBuilder = new StringBuilder(num);
			mountPoint = null;
			if (this.m_findHandle != null)
			{
				bool flag = NativeMethods.FindNextVolumeMountPoint(this.m_findHandle, stringBuilder, (uint)num);
				if (flag)
				{
					mountPoint = stringBuilder.ToString();
				}
				else
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 18 && MountPointEnumerator.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Exception arg = new Win32Exception(lastWin32Error);
						MountPointEnumerator.Tracer.TraceError<int, Exception>((long)this.GetHashCode(), "FindNextVolumeMountPoint() failed with Win32 EC: {0}. Win32Exception: {1}", lastWin32Error, arg);
					}
				}
				return flag;
			}
			this.m_findHandle = NativeMethods.FindFirstVolumeMountPoint(this.m_volumeName, stringBuilder, (uint)num);
			if (this.m_findHandle == null || this.m_findHandle.IsInvalid)
			{
				if (MountPointEnumerator.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					Exception arg2 = new Win32Exception(lastWin32Error2);
					MountPointEnumerator.Tracer.TraceError<int, Exception>((long)this.GetHashCode(), "FindFirstVolumeMountPoint() failed with Win32 EC: {0}. Win32Exception: {1}", lastWin32Error2, arg2);
				}
				return false;
			}
			mountPoint = stringBuilder.ToString();
			return true;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.m_findHandle != null)
			{
				if (!this.m_findHandle.IsInvalid)
				{
					this.m_findHandle.Close();
				}
				this.m_findHandle = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MountPointEnumerator>(this);
		}

		private readonly string m_volumeName;

		private SafeVolumeMountPointFindHandle m_findHandle;
	}
}
