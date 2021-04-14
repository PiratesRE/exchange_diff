using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public static class MapiStreamLock
	{
		public static void Release(MapiStream stream)
		{
			if (stream.Logon == null)
			{
				return;
			}
			MapiStreamLock.ListOfLocks listOfLocks = (MapiStreamLock.ListOfLocks)stream.Logon.MapiMailbox.SharedState.GetComponentData(MapiStreamLock.mapiStreamLockSlot);
			if (listOfLocks == null)
			{
				return;
			}
			using (LockManager.Lock(listOfLocks))
			{
				MapiStreamLock.ListOfLocks listOfLocks2 = new MapiStreamLock.ListOfLocks(1);
				foreach (MapiStreamLock.LockOnStream lockOnStream in listOfLocks)
				{
					if (object.ReferenceEquals(lockOnStream.StreamObject, stream))
					{
						listOfLocks2.Add(lockOnStream);
					}
				}
				foreach (MapiStreamLock.LockOnStream item in listOfLocks2)
				{
					listOfLocks.Remove(item);
				}
			}
		}

		public static ErrorCode LockRegion(MapiStream stream, ulong regionStart, ulong regionLength, bool exclusive)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			ulong num = regionStart + regionLength - 1UL;
			if (num < regionStart)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Region overflow in Lock Region");
				return ErrorCode.CreateInvalidParameter((LID)42392U);
			}
			MapiStreamLock.ListOfLocks listOfLocks = (MapiStreamLock.ListOfLocks)stream.Logon.MapiMailbox.SharedState.GetComponentData(MapiStreamLock.mapiStreamLockSlot);
			if (listOfLocks == null)
			{
				listOfLocks = new MapiStreamLock.ListOfLocks(2);
				MapiStreamLock.ListOfLocks listOfLocks2 = (MapiStreamLock.ListOfLocks)stream.Logon.MapiMailbox.SharedState.CompareExchangeComponentData(MapiStreamLock.mapiStreamLockSlot, null, listOfLocks);
				if (listOfLocks2 != null)
				{
					listOfLocks = listOfLocks2;
				}
			}
			using (LockManager.Lock(listOfLocks))
			{
				foreach (MapiStreamLock.LockOnStream lockOnStream in listOfLocks)
				{
					if (lockOnStream.PropertyTag == stream.Ptag && lockOnStream.ObjectId == MapiStreamLock.GetMapiObjectExchangeId(stream.ParentObject) && !lockOnStream.IsExpired)
					{
						if (regionStart >= lockOnStream.RegionStart && regionStart <= lockOnStream.RegionEnd && (lockOnStream.Exclusive || exclusive))
						{
							errorCode = ErrorCode.CreateLockViolation((LID)34200U);
							break;
						}
						if (num >= lockOnStream.RegionStart && num <= lockOnStream.RegionEnd && (lockOnStream.Exclusive || exclusive))
						{
							errorCode = ErrorCode.CreateLockViolation((LID)50584U);
							break;
						}
						if (regionStart <= lockOnStream.RegionStart && num >= lockOnStream.RegionEnd && (lockOnStream.Exclusive || exclusive))
						{
							errorCode = ErrorCode.CreateLockViolation((LID)47512U);
							break;
						}
					}
				}
				if (errorCode == ErrorCode.NoError)
				{
					MapiStreamLock.LockOnStream item = new MapiStreamLock.LockOnStream(stream, regionStart, regionLength, exclusive);
					listOfLocks.Add(item);
				}
			}
			return errorCode;
		}

		public static ErrorCode UnLockRegion(MapiStream stream, ulong regionStart, ulong regionLength, bool exclusive)
		{
			ErrorCode result = ErrorCode.NoError;
			ulong num = regionStart + regionLength - 1UL;
			if (num < regionStart)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Region overflow in Unlock Region");
				return ErrorCode.CreateInvalidParameter((LID)63896U);
			}
			MapiStreamLock.ListOfLocks listOfLocks = (MapiStreamLock.ListOfLocks)stream.Logon.MapiMailbox.SharedState.GetComponentData(MapiStreamLock.mapiStreamLockSlot);
			if (listOfLocks == null)
			{
				return ErrorCode.NoError;
			}
			using (LockManager.Lock(listOfLocks))
			{
				MapiStreamLock.ListOfLocks listOfLocks2 = new MapiStreamLock.ListOfLocks(1);
				foreach (MapiStreamLock.LockOnStream lockOnStream in listOfLocks)
				{
					if (object.ReferenceEquals(lockOnStream.StreamObject, stream) && lockOnStream.Exclusive == exclusive && lockOnStream.RegionStart >= regionStart && lockOnStream.RegionEnd <= num)
					{
						listOfLocks2.Add(lockOnStream);
						if (lockOnStream.IsExpired)
						{
							result = ErrorCode.CreateNetworkError((LID)39320U);
						}
					}
				}
				foreach (MapiStreamLock.LockOnStream item in listOfLocks2)
				{
					listOfLocks.Remove(item);
				}
			}
			return result;
		}

		public static void CanAccess(LID lid, MapiStream stream, ulong regionStart, ulong regionLength, bool exclusive)
		{
			ulong num = (regionLength != 0UL) ? (regionStart + regionLength - 1UL) : regionStart;
			if (num < regionStart)
			{
				DiagnosticContext.TraceLocation(lid);
				throw new StoreException((LID)55704U, ErrorCodeValue.InvalidParameter);
			}
			MapiStreamLock.ListOfLocks listOfLocks = (MapiStreamLock.ListOfLocks)stream.Logon.MapiMailbox.SharedState.GetComponentData(MapiStreamLock.mapiStreamLockSlot);
			if (listOfLocks == null)
			{
				return;
			}
			using (LockManager.Lock(listOfLocks))
			{
				foreach (MapiStreamLock.LockOnStream lockOnStream in listOfLocks)
				{
					if (!object.ReferenceEquals(lockOnStream.StreamObject, stream))
					{
						if (lockOnStream.IsExpired)
						{
							DiagnosticContext.TraceLocation(lid);
							throw new StoreException((LID)51608U, ErrorCodeValue.NetworkError);
						}
						if (regionLength != 0UL && lockOnStream.PropertyTag == stream.Ptag && lockOnStream.ObjectId == MapiStreamLock.GetMapiObjectExchangeId(stream.ParentObject))
						{
							if (regionStart >= lockOnStream.RegionStart && regionStart <= lockOnStream.RegionEnd)
							{
								if (lockOnStream.Exclusive || exclusive)
								{
									DiagnosticContext.TraceLocation(lid);
									throw new StoreException((LID)43416U, ErrorCodeValue.LockViolation);
								}
								break;
							}
							else if (num >= lockOnStream.RegionStart && num <= lockOnStream.RegionEnd)
							{
								if (lockOnStream.Exclusive || exclusive)
								{
									DiagnosticContext.TraceLocation(lid);
									throw new StoreException((LID)59800U, ErrorCodeValue.LockViolation);
								}
								break;
							}
							else if (regionStart <= lockOnStream.RegionStart && num >= lockOnStream.RegionEnd)
							{
								if (lockOnStream.Exclusive || exclusive)
								{
									DiagnosticContext.TraceLocation(lid);
									throw new StoreException((LID)35224U, ErrorCodeValue.LockViolation);
								}
								break;
							}
						}
					}
				}
			}
		}

		internal static void Initialize()
		{
			if (MapiStreamLock.mapiStreamLockSlot == -1)
			{
				MapiStreamLock.mapiStreamLockSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		private static ExchangeId GetMapiObjectExchangeId(MapiBase mapiObject)
		{
			switch (mapiObject.MapiObjectType)
			{
			case MapiObjectType.Attachment:
				return ((MapiAttachment)mapiObject).Atid;
			case MapiObjectType.Folder:
				return ((MapiFolder)mapiObject).Fid;
			case MapiObjectType.Message:
			case MapiObjectType.EmbeddedMessage:
				return ((MapiMessage)mapiObject).Mid;
			}
			throw new ExExceptionNoSupport((LID)44856U, "This object does not have the Id");
		}

		internal const int DefaultListSize = 2;

		private static int mapiStreamLockSlot = -1;

		private class ListOfLocks : List<MapiStreamLock.LockOnStream>, IComponentData
		{
			internal ListOfLocks(int inititalCapacity) : base(inititalCapacity)
			{
			}

			bool IComponentData.DoCleanup(Context context)
			{
				return base.Count == 0;
			}
		}

		private class LockOnStream
		{
			public LockOnStream(MapiStream stream, ulong regionStart, ulong regionLength, bool exclusive)
			{
				this.streamObject = stream;
				this.propertyTag = stream.Ptag;
				this.objectId = MapiStreamLock.GetMapiObjectExchangeId(stream.ParentObject);
				this.regionStart = regionStart;
				this.regionLength = regionLength;
				this.exclusive = exclusive;
				this.lockGrantTime = DateTime.UtcNow;
			}

			public bool IsExpired
			{
				get
				{
					return this.lockGrantTime + MapiStreamLock.LockOnStream.expirationTime < DateTime.UtcNow;
				}
			}

			public object StreamObject
			{
				get
				{
					return this.streamObject;
				}
			}

			public StorePropTag PropertyTag
			{
				get
				{
					return this.propertyTag;
				}
			}

			public ExchangeId ObjectId
			{
				get
				{
					return this.objectId;
				}
			}

			public ulong RegionStart
			{
				get
				{
					return this.regionStart;
				}
			}

			public ulong RegionLength
			{
				get
				{
					return this.regionLength;
				}
			}

			public ulong RegionEnd
			{
				get
				{
					return this.regionStart + this.regionLength - 1UL;
				}
			}

			public bool Exclusive
			{
				get
				{
					return this.exclusive;
				}
			}

			private static TimeSpan expirationTime = TimeSpan.FromMinutes(30.0);

			private readonly object streamObject;

			private readonly StorePropTag propertyTag;

			private readonly ExchangeId objectId;

			private readonly ulong regionStart;

			private readonly ulong regionLength;

			private readonly bool exclusive;

			private readonly DateTime lockGrantTime;
		}
	}
}
