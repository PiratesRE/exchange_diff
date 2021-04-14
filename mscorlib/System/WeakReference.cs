using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	[Serializable]
	public class WeakReference : ISerializable
	{
		[__DynamicallyInvokable]
		public WeakReference(object target) : this(target, false)
		{
		}

		[__DynamicallyInvokable]
		public WeakReference(object target, bool trackResurrection)
		{
			this.Create(target, trackResurrection);
		}

		protected WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			object value = info.GetValue("TrackedObject", typeof(object));
			bool boolean = info.GetBoolean("TrackResurrection");
			this.Create(value, boolean);
		}

		[__DynamicallyInvokable]
		public virtual extern bool IsAlive { [SecuritySafeCritical] [__DynamicallyInvokable] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[__DynamicallyInvokable]
		public virtual bool TrackResurrection
		{
			[__DynamicallyInvokable]
			get
			{
				return this.IsTrackResurrection();
			}
		}

		[__DynamicallyInvokable]
		public virtual extern object Target { [SecuritySafeCritical] [__DynamicallyInvokable] [MethodImpl(MethodImplOptions.InternalCall)] get; [SecuritySafeCritical] [__DynamicallyInvokable] [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected override extern void Finalize();

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("TrackedObject", this.Target, typeof(object));
			info.AddValue("TrackResurrection", this.IsTrackResurrection());
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Create(object target, bool trackResurrection);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTrackResurrection();

		internal IntPtr m_handle;
	}
}
