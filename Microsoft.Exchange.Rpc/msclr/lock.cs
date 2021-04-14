using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace msclr
{
	internal class @lock : IDisposable
	{
		public @lock(object _object)
		{
			this.acquire(-1);
		}

		private void ~lock()
		{
			this.release();
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool is_locked()
		{
			return this.m_locked;
		}

		public implicit operator string()
		{
			return (!this.m_locked) ? _detail_class._safe_false : _detail_class._safe_true;
		}

		public void acquire(TimeSpan _timeout)
		{
			if (!this.m_locked)
			{
				Monitor.TryEnter(this.m_object, _timeout, ref this.m_locked);
				if (!this.m_locked)
				{
					throw Marshal.GetExceptionForHR(-2147024638);
				}
			}
		}

		public void acquire()
		{
			if (!this.m_locked)
			{
				Monitor.TryEnter(this.m_object, -1, ref this.m_locked);
				if (!this.m_locked)
				{
					throw Marshal.GetExceptionForHR(-2147024638);
				}
			}
		}

		public void acquire(int _timeout)
		{
			if (!this.m_locked)
			{
				Monitor.TryEnter(this.m_object, _timeout, ref this.m_locked);
				if (!this.m_locked)
				{
					throw Marshal.GetExceptionForHR(-2147024638);
				}
			}
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool try_acquire(TimeSpan _timeout)
		{
			if (!this.m_locked)
			{
				Monitor.TryEnter(this.m_object, _timeout, ref this.m_locked);
				if (!this.m_locked)
				{
					return false;
				}
			}
			return true;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool try_acquire(int _timeout)
		{
			if (!this.m_locked)
			{
				Monitor.TryEnter(this.m_object, _timeout, ref this.m_locked);
				if (!this.m_locked)
				{
					return false;
				}
			}
			return true;
		}

		public void release()
		{
			if (this.m_locked)
			{
				Monitor.Exit(this.m_object);
				this.m_locked = false;
			}
		}

		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.release();
			}
			else
			{
				base.Finalize();
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private object m_object = _object;

		private bool m_locked = false;

		private struct is_not<System::Object,System::Threading::ReaderWriterLock>
		{
		}
	}
}
