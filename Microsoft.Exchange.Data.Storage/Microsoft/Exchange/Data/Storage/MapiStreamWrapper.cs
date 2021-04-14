using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MapiStreamWrapper : Stream, IDisposeTrackable, IDisposable
	{
		private static T CallMapiWithReturnValue<T>(string methodName, object obj, StoreSession session, StorageGlobals.MapiCallWithReturnValue<T> mapiCall)
		{
			bool flag = false;
			T result;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = mapiCall();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExWrappedStreamFailure, ex, session, obj, "{0}. MapiException = {1}.", new object[]
				{
					string.Format(methodName, new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExWrappedStreamFailure, ex2, session, obj, "{0}. MapiException = {1}.", new object[]
				{
					string.Format(methodName, new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		internal static void CallMapi(string methodName, object obj, StoreSession session, StorageGlobals.MapiCall mapiCall)
		{
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiCall();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExWrappedStreamFailure, ex, session, obj, "{0}. MapiException = {1}.", new object[]
				{
					string.Format(methodName, new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExWrappedStreamFailure, ex2, session, obj, "{0}. MapiException = {1}.", new object[]
				{
					string.Format(methodName, new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		internal MapiStreamWrapper(MapiStream stream, StoreSession session)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.disposeTracker = this.GetDisposeTracker();
				this.session = session;
				if (stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				this.mapiStream = stream;
				disposeGuard.Success();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiStreamWrapper>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			if (isDisposing)
			{
				this.isDisposed = true;
				this.disposeTracker.Dispose();
				MapiStreamWrapper.CallMapi("Dispose", this, this.session, delegate
				{
					this.mapiStream.Dispose();
				});
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed("CanRead");
				return MapiStreamWrapper.CallMapiWithReturnValue<bool>("CanRead", this, this.session, () => this.mapiStream.CanRead);
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed("CanWrite");
				return MapiStreamWrapper.CallMapiWithReturnValue<bool>("CanWrite", this, this.session, () => this.mapiStream.CanWrite);
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("CanSeek");
				return MapiStreamWrapper.CallMapiWithReturnValue<bool>("CanSeek", this, this.session, () => this.mapiStream.CanSeek);
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed("Length:get");
				return MapiStreamWrapper.CallMapiWithReturnValue<long>("Length", this, this.session, () => this.mapiStream.Length);
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed("Position:get");
				return MapiStreamWrapper.CallMapiWithReturnValue<long>("Position::get", this, this.session, () => this.mapiStream.Position);
			}
			set
			{
				this.CheckDisposed("Position:set");
				MapiStreamWrapper.CallMapi("Position::set", this, this.session, delegate
				{
					this.mapiStream.Position = value;
				});
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Read");
			return MapiStreamWrapper.CallMapiWithReturnValue<int>("Read", this, this.session, () => this.mapiStream.Read(buffer, offset, count));
		}

		public override void Flush()
		{
			this.CheckDisposed("Flush");
			MapiStreamWrapper.CallMapi("Flush", this, this.session, delegate
			{
				this.mapiStream.Flush();
			});
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("Seek");
			return MapiStreamWrapper.CallMapiWithReturnValue<long>("Seek", this, this.session, () => this.mapiStream.Seek(offset, origin));
		}

		public override void SetLength(long length)
		{
			this.CheckDisposed("SetLength");
			MapiStreamWrapper.CallMapi("SetLength", this, this.session, delegate
			{
				this.mapiStream.SetLength(length);
			});
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Write");
			MapiStreamWrapper.CallMapi("Write", this, this.session, delegate
			{
				this.mapiStream.Write(buffer, offset, count);
			});
		}

		public void LockRegion(long offset, long cb, int lockType)
		{
			this.CheckDisposed("LockRegion");
			MapiStreamWrapper.CallMapi("LockRegion", this, this.session, delegate
			{
				this.mapiStream.LockRegion(offset, cb, lockType);
			});
		}

		public void UnlockRegion(long offset, long cb, int lockType)
		{
			this.CheckDisposed("UnlockRegion");
			MapiStreamWrapper.CallMapi("UnlockRegion", this, this.session, delegate
			{
				this.mapiStream.UnlockRegion(offset, cb, lockType);
			});
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly MapiStream mapiStream;

		private readonly DisposeTracker disposeTracker;

		private StoreSession session;

		private bool isDisposed;
	}
}
