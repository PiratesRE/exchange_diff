using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiStream : Stream, IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
		internal MapiStream(IExMapiStream iStream, MapiStore mapiStore)
		{
			if (iStream == null || iStream.IsInvalid)
			{
				throw MapiExceptionHelper.ArgumentException("iStream", "Unable to create MapiStream object with null or invalid interface handle.");
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(11874, 25, (long)this.GetHashCode(), "MapiStream.MapiStream: this={0}", TraceUtils.MakeHash(this));
			}
			this.iStream = iStream;
			this.childRef = null;
			this.mapiStore = mapiStore;
			if (this.mapiStore != null)
			{
				this.childRef = mapiStore.AddChildReference(this);
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.disposed = false;
			this.dirty = false;
		}

		internal void CheckDisposed()
		{
			if (this.disposed)
			{
				throw MapiExceptionHelper.ObjectDisposedException(base.GetType().ToString());
			}
		}

		internal void LockStore()
		{
			if (this.mapiStore != null)
			{
				this.mapiStore.LockConnection();
			}
		}

		internal void UnlockStore()
		{
			if (this.mapiStore != null)
			{
				this.mapiStore.UnlockConnection();
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed();
				this.LockStore();
				bool result;
				try
				{
					result = true;
				}
				finally
				{
					this.UnlockStore();
				}
				return result;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed();
				this.LockStore();
				bool result;
				try
				{
					result = true;
				}
				finally
				{
					this.UnlockStore();
				}
				return result;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed();
				this.LockStore();
				bool result;
				try
				{
					result = true;
				}
				finally
				{
					this.UnlockStore();
				}
				return result;
			}
		}

		public override void Flush()
		{
			this.CheckDisposed();
			this.LockStore();
			try
			{
				if (this.dirty)
				{
					int num = this.iStream.Commit(0);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to flush stream.", num, this.iStream, this.LastLowLevelException);
					}
					this.dirty = false;
				}
			}
			finally
			{
				this.UnlockStore();
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed();
				this.LockStore();
				long cbSize;
				try
				{
					STATSTG statstg;
					int num = this.iStream.Stat(out statstg, 0);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to get Length of stream.", num, this.iStream, this.LastLowLevelException);
					}
					cbSize = statstg.cbSize;
				}
				finally
				{
					this.UnlockStore();
				}
				return cbSize;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed();
				this.LockStore();
				long result;
				try
				{
					long num2;
					int num = this.iStream.Seek(0L, 1, out num2);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to get Position of stream.", num, this.iStream, this.LastLowLevelException);
					}
					result = num2;
				}
				finally
				{
					this.UnlockStore();
				}
				return result;
			}
			set
			{
				this.CheckDisposed();
				this.LockStore();
				try
				{
					long num2;
					int num = this.iStream.Seek(value, 0, out num2);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to set Position of stream.", num, this.iStream, this.LastLowLevelException);
					}
				}
				finally
				{
					this.UnlockStore();
				}
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("offset", "cannot be negative.");
			}
			if (count < 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("count", "cannot be negative.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return 0;
			}
			this.LockStore();
			int result;
			try
			{
				uint num;
				int hresult = this.iStream.Read(buffer, checked((uint)count), out num);
				MapiExceptionHelper.ThrowIfError("Unable to Read from stream.", hresult, this.iStream, this.LastLowLevelException);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<int, int, uint>(12386, 25, (long)this.GetHashCode(), "MapiStream.Read: offset={0}, count={1}, result={2}", offset, count, num);
				}
				result = checked((int)num);
			}
			finally
			{
				this.UnlockStore();
			}
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed();
			this.LockStore();
			long result;
			try
			{
				long num2;
				int num = this.iStream.Seek(offset, (int)origin, out num2);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to Seek to stream position.", num, this.iStream, this.LastLowLevelException);
				}
				long num3 = num2;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string, string>(10338, 25, (long)this.GetHashCode(), "MapiStream.Seek: offset={0}, origin={1}, result={2}", offset.ToString(), origin.ToString(), num3.ToString());
				}
				result = num3;
			}
			finally
			{
				this.UnlockStore();
			}
			return result;
		}

		public int LockRegion(long offset, long count, int lockType)
		{
			this.CheckDisposed();
			this.LockStore();
			int result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string, string>(14435, 25, (long)this.GetHashCode(), "MapiStream.LockRegion params: offset={0}, count={0}, lockType={0}", offset.ToString(), count.ToString(), lockType.ToString());
				}
				int num = this.iStream.LockRegion(offset, count, lockType);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to LockRegion of stream.", num, this.iStream, this.LastLowLevelException);
				}
				result = num;
			}
			finally
			{
				this.UnlockStore();
			}
			return result;
		}

		public int UnlockRegion(long offset, long count, int lockType)
		{
			this.CheckDisposed();
			this.LockStore();
			int result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string, string>(14436, 25, (long)this.GetHashCode(), "MapiStream.UnlockRegion params: offset={0}, count={0}, lockType={0}", offset.ToString(), count.ToString(), lockType.ToString());
				}
				int num = this.iStream.UnlockRegion(offset, count, lockType);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to UnlockRegion of stream.", num, this.iStream, this.LastLowLevelException);
				}
				result = num;
			}
			finally
			{
				this.UnlockStore();
			}
			return result;
		}

		public override void SetLength(long length)
		{
			this.CheckDisposed();
			this.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14434, 25, (long)this.GetHashCode(), "MapiStream.SetLength params: length={0}", length.ToString());
				}
				int num = this.iStream.SetSize(length);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to SetLength of stream.", num, this.iStream, this.LastLowLevelException);
				}
				this.dirty = true;
			}
			finally
			{
				this.UnlockStore();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			this.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(9314, 25, (long)this.GetHashCode(), "MapiStream.Write params: offset={0}, count={1}", offset.ToString(), count.ToString());
				}
				if (count > 0)
				{
					if (offset == 0)
					{
						int num2;
						int num = this.iStream.Write(buffer, count, out num2);
						if (num != 0)
						{
							MapiExceptionHelper.ThrowIfError("Unable to Write to stream.", num, this.iStream, this.LastLowLevelException);
						}
					}
					else
					{
						byte[] array = new byte[count];
						Array.Copy(buffer, offset, array, 0, count);
						int num2;
						int num = this.iStream.Write(array, count, out num2);
						if (num != 0)
						{
							MapiExceptionHelper.ThrowIfError("Unable to Write to stream at offset.", num, this.iStream, this.LastLowLevelException);
						}
					}
					this.dirty = true;
				}
			}
			finally
			{
				this.UnlockStore();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(25))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15970, 25, (long)this.GetHashCode(), "MapiStream.Dispose: this={0}", TraceUtils.MakeHash(this));
				}
				if (!this.disposed)
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					this.LockStore();
					try
					{
						try
						{
							if (this.iStream != null)
							{
								this.Flush();
							}
						}
						finally
						{
							this.iStream.DisposeIfValid();
							this.iStream = null;
							if (this.childRef != null)
							{
								DisposableRef.RemoveFromList(this.childRef);
								this.childRef.Dispose();
								this.childRef = null;
							}
						}
					}
					finally
					{
						this.UnlockStore();
						this.mapiStore = null;
						this.disposed = true;
						FaultInjectionUtils.FaultInjectionTracer.TraceTest(3110481213U);
					}
				}
				base.Dispose(disposing);
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void ForceLeakReport()
		{
			this.disposeTracker = null;
		}

		private Exception LastLowLevelException
		{
			get
			{
				if (this.mapiStore != null)
				{
					return this.mapiStore.LastLowLevelException;
				}
				return null;
			}
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		private IExMapiStream iStream;

		private MapiStore mapiStore;

		private DisposableRef childRef;

		private bool disposed;

		private bool dirty;

		private DisposeTracker disposeTracker;

		internal enum LockType
		{
			LockWrite = 1,
			LockExclusive,
			LockOnlyOnce = 4
		}
	}
}
