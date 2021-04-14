using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class WeakReference<T> : ISerializable where T : class
	{
		[__DynamicallyInvokable]
		public WeakReference(T target) : this(target, false)
		{
		}

		[__DynamicallyInvokable]
		public WeakReference(T target, bool trackResurrection)
		{
			this.Create(target, trackResurrection);
		}

		internal WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			T target = (T)((object)info.GetValue("TrackedObject", typeof(T)));
			bool boolean = info.GetBoolean("TrackResurrection");
			this.Create(target, boolean);
		}

		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetTarget(out T target)
		{
			T target2 = this.Target;
			target = target2;
			return target2 != null;
		}

		[__DynamicallyInvokable]
		public void SetTarget(T target)
		{
			this.Target = target;
		}

		private extern T Target { [SecuritySafeCritical] [MethodImpl(MethodImplOptions.InternalCall)] get; [SecuritySafeCritical] [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected override extern void Finalize();

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("TrackedObject", this.Target, typeof(T));
			info.AddValue("TrackResurrection", this.IsTrackResurrection());
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Create(T target, bool trackResurrection);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTrackResurrection();

		internal IntPtr m_handle;
	}
}
