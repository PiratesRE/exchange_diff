using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class HandleCache : DisposeTrackableBase
	{
		public HandleCache()
		{
			this.cache = new Dictionary<long, HandleCache.HandleRec>();
			this.nextHandleValue = 1L;
		}

		public T GetObject<T>(long handle) where T : class
		{
			MrsTracer.ProxyService.Function("HandleCache.GetObject<{1}>({0})", new object[]
			{
				handle,
				typeof(T).Name
			});
			T result;
			lock (this.locker)
			{
				HandleCache.HandleRec handleRec = this.FindObject(handle, HandleCache.FindObjectOptions.MustExist);
				T t = handleRec.Obj as T;
				if (t == null)
				{
					MrsTracer.ProxyService.Error("Handle {0} has wrong type: {1}, expected {2}.", new object[]
					{
						handle,
						handleRec.Obj.GetType().Name,
						typeof(T).Name
					});
					throw new InvalidHandleTypePermanentException(handle, handleRec.Obj.GetType().Name, typeof(T).Name);
				}
				result = t;
			}
			return result;
		}

		public long GetParentHandle(long handle)
		{
			MrsTracer.ProxyService.Function("HandleCache.GetParentHandle({0})", new object[]
			{
				handle
			});
			long parentHandle;
			lock (this.locker)
			{
				HandleCache.HandleRec handleRec = this.FindObject(handle, HandleCache.FindObjectOptions.MustExist);
				parentHandle = handleRec.ParentHandle;
			}
			return parentHandle;
		}

		public long AddObject(object obj, long parentHandle)
		{
			MrsTracer.ProxyService.Function("HandleCache.AddObject({0})", new object[]
			{
				obj.GetType().Name
			});
			long handle;
			lock (this.locker)
			{
				HandleCache.HandleRec handleRec = new HandleCache.HandleRec();
				handleRec.Obj = obj;
				handleRec.ParentHandle = parentHandle;
				handleRec.Handle = this.nextHandleValue;
				this.nextHandleValue += 1L;
				this.cache.Add(handleRec.Handle, handleRec);
				MrsTracer.ProxyService.Debug("HandleCache.AddObject({0}) returns {1}", new object[]
				{
					obj.GetType().Name,
					handleRec.Handle
				});
				handle = handleRec.Handle;
			}
			return handle;
		}

		public void ReleaseObject(long handle)
		{
			MrsTracer.ProxyService.Function("HandleCache.ReleaseObject({0})", new object[]
			{
				handle
			});
			lock (this.locker)
			{
				HandleCache.HandleRec handleRec = this.FindObject(handle, HandleCache.FindObjectOptions.MayBeAbsent);
				if (handleRec != null)
				{
					this.RemoveFromCache(handleRec);
				}
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.locker)
				{
					if (this.cache.Count > 0)
					{
						MrsTracer.ProxyService.Error("HandleCache being disposed with {0} open handles", new object[]
						{
							this.cache.Count
						});
						while (this.cache.Count > 0)
						{
							HandleCache.HandleRec rec = this.cache.Values.First<HandleCache.HandleRec>();
							this.RemoveFromCache(rec);
						}
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<HandleCache>(this);
		}

		private void RemoveFromCache(HandleCache.HandleRec rec)
		{
			Lazy<List<HandleCache.HandleRec>> lazy = new Lazy<List<HandleCache.HandleRec>>(() => new List<HandleCache.HandleRec>());
			foreach (HandleCache.HandleRec handleRec in this.cache.Values)
			{
				if (handleRec.ParentHandle == rec.Handle)
				{
					lazy.Value.Add(handleRec);
				}
			}
			if (lazy.IsValueCreated)
			{
				foreach (HandleCache.HandleRec rec2 in lazy.Value)
				{
					this.RemoveFromCache(rec2);
				}
			}
			IDisposable disposable = rec.Obj as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			this.cache.Remove(rec.Handle);
		}

		private HandleCache.HandleRec FindObject(long handle, HandleCache.FindObjectOptions options)
		{
			HandleCache.HandleRec result;
			if (this.cache.TryGetValue(handle, out result))
			{
				return result;
			}
			if ((options & HandleCache.FindObjectOptions.MustExist) != HandleCache.FindObjectOptions.MayBeAbsent)
			{
				MrsTracer.ProxyService.Error("Handle {0} is not found in MRS handle cache.", new object[]
				{
					handle
				});
				throw new HandleNotFoundPermanentException(handle);
			}
			return null;
		}

		public const long NoParent = -1L;

		private Dictionary<long, HandleCache.HandleRec> cache;

		private long nextHandleValue;

		private object locker = new object();

		[Flags]
		private enum FindObjectOptions
		{
			MayBeAbsent = 0,
			MustExist = 1
		}

		private class HandleRec
		{
			public long Handle { get; set; }

			public long ParentHandle { get; set; }

			public object Obj { get; set; }
		}
	}
}
