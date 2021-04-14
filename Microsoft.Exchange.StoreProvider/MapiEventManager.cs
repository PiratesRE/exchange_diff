using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiEventManager
	{
		private MapiEventManager(ExRpcAdmin exRpcAdmin, Guid consumerGuid, Guid mdbGuid) : this(exRpcAdmin, consumerGuid, mdbGuid, Guid.Empty)
		{
		}

		private MapiEventManager(ExRpcAdmin exRpcAdmin, Guid consumerGuid, Guid mdbGuid, byte[] mdbVersionInfo) : this(exRpcAdmin, consumerGuid, mdbGuid, new Guid(mdbVersionInfo))
		{
		}

		private MapiEventManager(ExRpcAdmin exRpcAdmin, Guid consumerGuid, Guid mdbGuid, Guid mdbVersionGuid)
		{
			if (exRpcAdmin == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("exRpcAdmin");
			}
			this.exrpcAdmin = exRpcAdmin;
			this.consumerGuid = consumerGuid;
			this.mdbGuid = mdbGuid;
			this.mdbVersionGuid = mdbVersionGuid;
		}

		public Guid ConsumerGuid
		{
			get
			{
				this.exrpcAdmin.CheckDisposed();
				return this.consumerGuid;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				this.exrpcAdmin.CheckDisposed();
				return this.mdbGuid;
			}
		}

		public byte[] MdbVersion
		{
			get
			{
				this.exrpcAdmin.CheckDisposed();
				return this.mdbVersionGuid.ToByteArray();
			}
		}

		public static MapiEventManager Create(ExRpcAdmin exRpcAdmin, Guid consumerGuid, Guid mdbGuid, byte[] mdbVersionInfo)
		{
			return new MapiEventManager(exRpcAdmin, consumerGuid, mdbGuid, mdbVersionInfo);
		}

		public static MapiEventManager Create(ExRpcAdmin exRpcAdmin, Guid consumerGuid, Guid mdbGuid)
		{
			return new MapiEventManager(exRpcAdmin, consumerGuid, mdbGuid);
		}

		public unsafe void SaveWatermarks(params Watermark[] watermarks)
		{
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			try
			{
				if (watermarks == null)
				{
					throw MapiExceptionHelper.ArgumentNullException("watermarks");
				}
				if (watermarks.Length == 0)
				{
					throw MapiExceptionHelper.ArgumentException("watermarks", "need at least one watermark.");
				}
				int hresult = 0;
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				int num = WatermarkNative.SizeOf + 7 & -8;
				try
				{
					fixed (byte* ptr = new byte[num * watermarks.Length])
					{
						WatermarkNative* ptr2 = (WatermarkNative*)ptr;
						for (int i = 0; i < watermarks.Length; i++)
						{
							this.WatermarkToNativeWatermark(watermarks[i], ptr2);
							ptr2++;
						}
						hresult = this.exrpcAdmin.IExRpcAdmin.HrSaveWatermarks(pguidMdb, ref guid, watermarks.Length, (IntPtr)((void*)ptr));
					}
				}
				finally
				{
					byte* ptr = null;
				}
				MapiExceptionHelper.ThrowIfErrorOrWarning("Unable to save watermarks.", hresult, true, this.exrpcAdmin.IExRpcAdmin, null);
				this.mdbVersionGuid = guid;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
		}

		public Watermark GetWatermark(Guid mailboxGuid)
		{
			Watermark[] watermarks = this.GetWatermarks(mailboxGuid);
			if (watermarks.Length == 0)
			{
				return null;
			}
			return watermarks[0];
		}

		public Watermark[] GetWatermarks()
		{
			return this.GetWatermarks(MapiEventManager.AllWatermarks);
		}

		internal Watermark[] GetWatermarks(Guid guidMailbox)
		{
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			Watermark[] result;
			try
			{
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				Guid pguidConsumer = this.consumerGuid;
				Watermark[] array = null;
				int countWatermarks = 0;
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					int num = this.exrpcAdmin.IExRpcAdmin.HrGetWatermarks(pguidMdb, ref guid, pguidConsumer, guidMailbox, out countWatermarks, out safeExMemoryHandle);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to get watermarks for consumer " + this.consumerGuid, num, this.exrpcAdmin.IExRpcAdmin, null);
					}
					this.mdbVersionGuid = guid;
					array = Watermark.Unmarshal(safeExMemoryHandle, countWatermarks);
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
				result = array;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
			return result;
		}

		public void DeleteWatermark(Guid mailboxGuid)
		{
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			try
			{
				int num = 0;
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				Guid pguidConsumer = this.consumerGuid;
				int num2 = this.exrpcAdmin.IExRpcAdmin.HrDeleteWatermarksForConsumer(pguidMdb, ref guid, mailboxGuid, pguidConsumer, out num);
				if (num2 != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to delete watermarks.", num2, this.exrpcAdmin.IExRpcAdmin, null);
				}
				this.mdbVersionGuid = guid;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
		}

		public void DeleteWatermarks()
		{
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			try
			{
				int num = 0;
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				Guid pguidConsumer = this.consumerGuid;
				Guid allWatermarks = MapiEventManager.AllWatermarks;
				int num2 = this.exrpcAdmin.IExRpcAdmin.HrDeleteWatermarksForConsumer(pguidMdb, ref guid, allWatermarks, pguidConsumer, out num);
				if (num2 != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to delete watermarks.", num2, this.exrpcAdmin.IExRpcAdmin, null);
				}
				this.mdbVersionGuid = guid;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted)
		{
			long num = 0L;
			return this.ReadEvents(startCounter, eventCountWanted, 0, null, ReadEventsFlags.None, out num);
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, ReadEventsFlags flags)
		{
			return this.ReadEvents(startCounter, eventCountWanted, flags, true);
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, ReadEventsFlags flags, bool includeSid)
		{
			long num = 0L;
			return this.ReadEvents(startCounter, eventCountWanted, 0, null, flags, includeSid, out num);
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, int eventCountToCheck, Restriction filter, out long endCounter)
		{
			return this.ReadEvents(startCounter, eventCountWanted, eventCountToCheck, filter, ReadEventsFlags.None, out endCounter);
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, int eventCountToCheck, Restriction filter, ReadEventsFlags flags, out long endCounter)
		{
			return this.ReadEvents(startCounter, eventCountWanted, eventCountToCheck, filter, flags, true, out endCounter);
		}

		public unsafe MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, int eventCountToCheck, Restriction filter, ReadEventsFlags flags, bool includeSid, out long endCounter)
		{
			FaultInjectionUtils.FaultInjectionTracer.TraceTest(3462802749U);
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			MapiEvent[] result;
			try
			{
				MapiEvent[] array = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				int num = 0;
				SRestriction* ptr = null;
				try
				{
					if (startCounter < 0L)
					{
						throw MapiExceptionHelper.ArgumentOutOfRangeException("startCounter", "Invalid start event counter: " + startCounter);
					}
					if (eventCountWanted < 0)
					{
						throw MapiExceptionHelper.ArgumentOutOfRangeException("eventCountWanted", "Invalid count of events requested: " + eventCountWanted);
					}
					if (eventCountToCheck < 0)
					{
						throw MapiExceptionHelper.ArgumentOutOfRangeException("eventCountToCheck", "Invalid count of events to check requested: " + eventCountToCheck);
					}
					if (filter != null)
					{
						int bytesToMarshal = filter.GetBytesToMarshal();
						byte* ptr2 = stackalloc byte[(UIntPtr)bytesToMarshal];
						ptr = (SRestriction*)ptr2;
						ptr2 += (SRestriction.SizeOf + 7 & -8);
						filter.MarshalToNative(ptr, ref ptr2);
					}
					int num2 = this.exrpcAdmin.IExRpcAdmin.HrReadMapiEvents(pguidMdb, ref guid, startCounter, eventCountWanted, eventCountToCheck, ptr, (int)flags, out num, out safeExLinkedMemoryHandle, out endCounter);
					if (num2 != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to read events.", num2, this.exrpcAdmin.IExRpcAdmin, null);
					}
					this.mdbVersionGuid = guid;
					array = new MapiEvent[num];
					if (num > 0)
					{
						IntPtr intPtr = safeExLinkedMemoryHandle.DangerousGetHandle();
						for (int i = 0; i < num; i++)
						{
							MapiEventNative mapiEventNative = (MapiEventNative)Marshal.PtrToStructure(intPtr, typeof(MapiEventNative));
							intPtr = (IntPtr)((long)intPtr + (long)MapiEventNative.SizeOf);
							array[i] = new MapiEvent(ref mapiEventNative, includeSid);
						}
					}
				}
				finally
				{
					if (safeExLinkedMemoryHandle != null)
					{
						safeExLinkedMemoryHandle.Dispose();
					}
				}
				result = array;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
			return result;
		}

		public MapiEvent ReadLastEvent()
		{
			return this.ReadLastEvent(true);
		}

		public MapiEvent ReadLastEvent(bool includeSid)
		{
			this.exrpcAdmin.CheckDisposed();
			this.exrpcAdmin.Lock();
			MapiEvent result;
			try
			{
				MapiEvent mapiEvent = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				Guid pguidMdb = this.mdbGuid;
				Guid guid = this.mdbVersionGuid;
				try
				{
					int num = this.exrpcAdmin.IExRpcAdmin.HrReadLastMapiEvent(pguidMdb, ref guid, out safeExLinkedMemoryHandle);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to read the last event.", num, this.exrpcAdmin.IExRpcAdmin, null);
					}
					this.mdbVersionGuid = guid;
					MapiEventNative mapiEventNative = (MapiEventNative)Marshal.PtrToStructure(safeExLinkedMemoryHandle.DangerousGetHandle(), typeof(MapiEventNative));
					mapiEvent = new MapiEvent(ref mapiEventNative, includeSid);
				}
				finally
				{
					if (safeExLinkedMemoryHandle != null)
					{
						safeExLinkedMemoryHandle.Dispose();
					}
				}
				result = mapiEvent;
			}
			finally
			{
				this.exrpcAdmin.Unlock();
			}
			return result;
		}

		private unsafe void WatermarkToNativeWatermark(Watermark watermark, WatermarkNative* pwatermarkNative)
		{
			pwatermarkNative->consumerGuid = this.consumerGuid;
			pwatermarkNative->mailboxGuid = watermark.MailboxGuid;
			pwatermarkNative->llEventCounter = watermark.EventCounter;
		}

		private static readonly Guid AllWatermarks = new Guid("bb6de7aa-f803-4cee-a985-86f568f3a9c9");

		private ExRpcAdmin exrpcAdmin;

		private Guid consumerGuid;

		private Guid mdbGuid;

		private Guid mdbVersionGuid;
	}
}
