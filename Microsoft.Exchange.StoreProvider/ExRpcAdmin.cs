using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExRpcAdmin : MapiUnk
	{
		internal SafeExRpcAdminHandle IExRpcAdmin
		{
			get
			{
				return this.iExRpcAdmin;
			}
		}

		internal ExRpcAdmin(SafeExRpcAdminHandle iExRpcAdmin) : base(iExRpcAdmin, null, null)
		{
			this.iExRpcAdmin = iExRpcAdmin;
			this.threadLockCount = 0U;
			this.owningThread = null;
		}

		protected override void MapiInternalDispose()
		{
			this.iExRpcAdmin = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExRpcAdmin>(this);
		}

		internal void Lock()
		{
			lock (this)
			{
				if (this.threadLockCount > 0U)
				{
					if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
					{
						StackTrace owningThreadStack = this.GetOwningThreadStack();
						throw MapiExceptionHelper.ObjectReenteredException(string.Concat(new object[]
						{
							"ExRpcAdmin object is already being used by thread ",
							this.owningThread.ManagedThreadId,
							".",
							(owningThreadStack != null) ? ("\nCall stack of the thread using the connection:\n" + owningThreadStack.ToString()) : ""
						}));
					}
					this.threadLockCount += 1U;
					if (this.threadLockCount == 0U)
					{
						throw MapiExceptionHelper.ObjectLockCountOverflowException("ExRpcAdmin object lock count has overflowed.");
					}
				}
				else
				{
					this.threadLockCount = 1U;
					this.owningThread = Thread.CurrentThread;
				}
			}
		}

		internal void Unlock()
		{
			lock (this)
			{
				if (this.threadLockCount <= 0U)
				{
					throw MapiExceptionHelper.ObjectNotLockedException("ExRpcAdmin object is being unlocked, but not currently locked.");
				}
				if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
				{
					StackTrace owningThreadStack = this.GetOwningThreadStack();
					throw MapiExceptionHelper.ObjectNotLockedException(string.Concat(new object[]
					{
						"ExRpcAdmin object is being unlocked, but currently locked by thread ",
						this.owningThread.ManagedThreadId,
						".",
						(owningThreadStack != null) ? ("\nCall stack of the thread using the connection:\n" + owningThreadStack.ToString()) : ""
					}));
				}
				this.threadLockCount -= 1U;
			}
		}

		internal uint GetFeatureVersion(VersionedFeature versionedFeature)
		{
			uint result = 0U;
			if (this.IsAdminVersionLessThan(6, 24))
			{
				result = 0U;
			}
			else
			{
				int num = this.iExRpcAdmin.HrGetFeatureVersion((uint)versionedFeature, out result);
				if (num != 0 && num != -2147024809)
				{
					base.ThrowIfError("Unable to read the feature version", num);
				}
			}
			return result;
		}

		private StackTrace GetOwningThreadStack()
		{
			StackTrace result = null;
			lock (this)
			{
				if (this.owningThread != null)
				{
					try
					{
						if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
						{
							this.owningThread.Suspend();
						}
						try
						{
							result = new StackTrace(this.owningThread, true);
						}
						finally
						{
							if (this.owningThread.ManagedThreadId != Environment.CurrentManagedThreadId)
							{
								this.owningThread.Resume();
							}
						}
					}
					catch
					{
					}
				}
			}
			return result;
		}

		private bool IsAdminVersionLessThan(int verMajor, int verMinor)
		{
			int num = 0;
			int num2 = 0;
			this.GetAdminVersion(out num, out num2);
			return num < verMajor || (num == verMajor && num2 < verMinor);
		}

		private byte[] GetLegacyMailboxBasicInfo(Guid guidMdb, Guid guidMailbox)
		{
			int num = 0;
			int num2 = this.iExRpcAdmin.HrGetMailboxInfoSize(out num);
			if (num2 != 0)
			{
				base.ThrowIfError("Unable to get size of mailbox info.", num2);
			}
			byte[] array = new byte[num];
			num2 = this.iExRpcAdmin.HrGetMailboxBasicInfo(guidMdb, guidMailbox, array, num);
			if (num2 != 0)
			{
				base.ThrowIfError("Unable to read mailbox info.", num2);
			}
			return array;
		}

		private byte[] GetMailboxSignatureBasicInfo(Guid guidMdb, Guid guidMailbox, MailboxSignatureFlags flags)
		{
			SafeExMemoryHandle safeExMemoryHandle = null;
			byte[] result;
			try
			{
				int num = 0;
				int num2 = this.iExRpcAdmin.HrGetMailboxSignatureBasicInfo(guidMdb, guidMailbox, (uint)flags, out num, out safeExMemoryHandle);
				if (num2 != 0)
				{
					base.ThrowIfError("Unable to read mailbox signature basic info.", num2);
				}
				if (safeExMemoryHandle.IsInvalid)
				{
					throw MapiExceptionHelper.ArgumentException("ptrMailboxBasicInfo", "Invalid parameter.");
				}
				if (0 >= num)
				{
					throw MapiExceptionHelper.ArgumentException("cbMailboxBasicInfo", "Invalid parameter.");
				}
				byte[] array = new byte[num];
				safeExMemoryHandle.CopyTo(array, 0, num);
				result = array;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public static ExRpcAdmin Create(string clientId, string server, string user, string domain, string password)
		{
			if (!MapiApplicationId.TryNormalizeClientType(ref clientId))
			{
				clientId = "Client=Management;";
			}
			ExRpcModule.Bind();
			ExRpcAdmin result = null;
			SafeExRpcAdminHandle safeExInterfaceHandle = null;
			try
			{
				SafeExRpcManageHandle safeExRpcManageHandle = null;
				try
				{
					safeExRpcManageHandle = (SafeExRpcManageHandle)ExRpcModule.GetExRpcManage();
					int num = safeExRpcManageHandle.AdminConnect(clientId, server, user, domain, password, out safeExInterfaceHandle);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to make admin interface connection to server.", num, safeExRpcManageHandle, null);
					}
				}
				finally
				{
					safeExRpcManageHandle.DisposeIfValid();
				}
				result = new ExRpcAdmin(safeExInterfaceHandle);
				safeExInterfaceHandle = null;
			}
			finally
			{
				safeExInterfaceHandle.DisposeIfValid();
			}
			return result;
		}

		public void GetAdminVersion(out int verMajor, out int verMinor)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				short num2;
				short num3;
				int num = this.iExRpcAdmin.HrGetServerVersion(out num2, out num3);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get server admin interface version.", num);
				}
				verMajor = (int)num2;
				verMinor = (int)num3;
			}
			finally
			{
				this.Unlock();
			}
		}

		public void DeletePrivateMailbox(Guid guidMdb, Guid guidMailbox, int flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrDeletePrivateMailbox(guidMdb, guidMailbox, flags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to delete mailbox.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public uint GetMailboxSignatureServerVersion()
		{
			base.CheckDisposed();
			this.Lock();
			uint featureVersion;
			try
			{
				featureVersion = this.GetFeatureVersion(VersionedFeature.MailboxSignatureServerVersion);
			}
			finally
			{
				this.Unlock();
			}
			return featureVersion;
		}

		public uint GetDeleteMailboxServerVersion()
		{
			base.CheckDisposed();
			this.Lock();
			uint featureVersion;
			try
			{
				featureVersion = this.GetFeatureVersion(VersionedFeature.DeleteMailboxServerVersion);
			}
			finally
			{
				this.Unlock();
			}
			return featureVersion;
		}

		public uint GetInTransitStatusServerVersion()
		{
			base.CheckDisposed();
			this.Lock();
			uint featureVersion;
			try
			{
				featureVersion = this.GetFeatureVersion(VersionedFeature.InTransitStatusServerVersion);
			}
			finally
			{
				this.Unlock();
			}
			return featureVersion;
		}

		public uint GetMailboxShapeServerVersion()
		{
			base.CheckDisposed();
			this.Lock();
			uint featureVersion;
			try
			{
				featureVersion = this.GetFeatureVersion(VersionedFeature.MailboxShapeServerVersion);
			}
			finally
			{
				this.Unlock();
			}
			return featureVersion;
		}

		public byte[] GetMailboxBasicInfo(Guid guidMdb, Guid guidMailbox, MailboxSignatureFlags flags)
		{
			base.CheckDisposed();
			this.Lock();
			byte[] result;
			try
			{
				byte[] array;
				if (this.IsAdminVersionLessThan(7, 9) && (this.IsAdminVersionLessThan(6, 24) || (MailboxSignatureFlags.GetLegacy & flags) != MailboxSignatureFlags.None))
				{
					if ((MailboxSignatureFlags.GetMailboxSignature & flags) != MailboxSignatureFlags.None)
					{
						throw MapiExceptionHelper.ArgumentException("flags", "Invalid parameter.");
					}
					array = this.GetLegacyMailboxBasicInfo(guidMdb, guidMailbox);
				}
				else
				{
					array = this.GetMailboxSignatureBasicInfo(guidMdb, guidMailbox, flags);
				}
				result = array;
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public byte[] GetMailboxBasicInfo(Guid guidMdb, Guid guidMailbox)
		{
			return this.GetMailboxBasicInfo(guidMdb, guidMailbox, MailboxSignatureFlags.GetLegacy);
		}

		public void SetMailboxBasicInfo(Guid guidMdb, Guid guidMailbox, byte[] rgbInfo)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrSetMailboxBasicInfo(guidMdb, guidMailbox, rgbInfo, rgbInfo.Length);
				if (num != 0)
				{
					base.ThrowIfError("Unable to write mailbox info.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void NotifyOnDSChange(Guid guidMdb, Guid guidObject, NotifyOnDsChangeObjectType objectType)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrNotifyOnDSChange(guidMdb, guidObject, (int)objectType);
				if (num != 0)
				{
					base.ThrowIfError("Unable to notify on DS change", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public MdbStatus[] ListMdbStatus(Guid[] mdbGuids)
		{
			base.CheckDisposed();
			this.Lock();
			MdbStatus[] result;
			try
			{
				MdbStatus[] array;
				if (mdbGuids == null || mdbGuids.Length == 0)
				{
					array = this.ListMdbStatus(true);
				}
				else
				{
					uint[] array2 = new uint[mdbGuids.Length];
					int num = this.iExRpcAdmin.HrListMdbStatus(mdbGuids.Length, mdbGuids, array2);
					if (num != 0)
					{
						base.ThrowIfError("Unable to list Mdb status.", num);
					}
					array = new MdbStatus[mdbGuids.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = new MdbStatus(mdbGuids[i], array2[i]);
					}
				}
				result = array;
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public MdbStatus[] ListMdbStatus(bool basicInformation)
		{
			base.CheckDisposed();
			this.Lock();
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4008062269U);
			}
			MdbStatus[] mdbStatus2;
			try
			{
				SafeExMemoryHandle ptrMdbStatus = null;
				try
				{
					MdbStatus[] mdbStatus = null;
					int num = 0;
					int num2 = this.iExRpcAdmin.HrListAllMdbStatus(basicInformation, out num, out ptrMdbStatus);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to list Mdb status.", num2);
					}
					mdbStatus = new MdbStatus[num];
					int i = 0;
					ptrMdbStatus.ForEach<MDBSTATUSRAW>(num, delegate(MDBSTATUSRAW item)
					{
						mdbStatus[i++] = new MdbStatus(item, ptrMdbStatus.DangerousGetHandle());
					});
					mdbStatus2 = mdbStatus;
				}
				finally
				{
					if (ptrMdbStatus != null)
					{
						ptrMdbStatus.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return mdbStatus2;
		}

		public PropValue[][] GetMailboxTable(Guid guidMdb, params PropTag[] propTagsRequested)
		{
			return this.GetMailboxTableInfo(guidMdb, Guid.Empty, MailboxTableFlags.MailboxTableFlagsNone, propTagsRequested);
		}

		public PropValue[][] GetMailboxTableInfo(Guid guidMdb, Guid guidMailbox, params PropTag[] propTagsRequested)
		{
			return this.GetMailboxTableInfo(guidMdb, guidMailbox, MailboxTableFlags.MailboxTableFlagsNone, propTagsRequested);
		}

		public PropValue[][] GetMailboxTableInfo(Guid guidMdb, Guid guidMailbox, MailboxTableFlags mailboxTableFlags, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num;
					if (this.IsAdminVersionLessThan(6, 17))
					{
						num = this.iExRpcAdmin.HrGetMailboxTable(guidMdb, (uint)mailboxTableFlags, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					}
					else if (Guid.Empty != guidMailbox)
					{
						if (this.IsAdminVersionLessThan(6, 28))
						{
							num = this.iExRpcAdmin.HrGetMailboxTableEntry(guidMdb, guidMailbox, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
						}
						else
						{
							num = this.iExRpcAdmin.HrGetMailboxTableEntryFlags(guidMdb, guidMailbox, (uint)mailboxTableFlags, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
						}
					}
					else
					{
						num = this.iExRpcAdmin.HrGetMailboxTable(guidMdb, (uint)mailboxTableFlags, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					}
					if (num != 0)
					{
						base.ThrowIfError("Unable to get mailbox information.", num);
					}
					if (safeExProwsHandle == null || safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue[]>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle);
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public Watermark[] GetWatermarksForMailbox(Guid guidMdb, ref Guid guidMdbVersion, Guid guidMailbox)
		{
			this.Lock();
			Watermark[] result;
			try
			{
				int countWatermarks = 0;
				SafeExMemoryHandle safeExMemoryHandle = null;
				Watermark[] array = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetWatermarksForMailbox(guidMdb, ref guidMdbVersion, guidMailbox, out countWatermarks, out safeExMemoryHandle);
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to get watermarks for mailbox " + guidMailbox, num, this.iExRpcAdmin, null);
					}
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
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetPublicFoldersGlobalsTable(Guid guidMdb, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetPublicFolderGlobalsTable(guidMdb, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get globals table information.", num);
					}
					if (safeExProwsHandle == null || safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue[]>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle);
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetResourceMonitorDigest(Guid guidMdb, params PropTag[] propTagsRequested)
		{
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetResourceMonitorDigest(guidMdb, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get the resource monitor digest.", num);
					}
					if (safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue[]>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle);
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetLogonTable(MdbFlags mdbFlags, Guid guidMdb, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetLogonTable(guidMdb, PropTagHelper.SPropTagArray(propTagsRequested), (int)mdbFlags, out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get mailbox information.", num);
					}
					result = SRowSet.Unmarshal(safeExProwsHandle);
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public RawSecurityDescriptor GetMailboxSecurityDescriptor(Guid guidMdb, Guid guidMailbox)
		{
			base.CheckDisposed();
			this.Lock();
			RawSecurityDescriptor result;
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					int num = 0;
					RawSecurityDescriptor rawSecurityDescriptor = null;
					int num2 = this.iExRpcAdmin.HrGetMailboxSecurityDescriptor(guidMdb, guidMailbox, out safeExMemoryHandle, out num);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to get mailbox SecurityDescriptor.", num2);
					}
					if (!safeExMemoryHandle.IsInvalid && num > 0)
					{
						byte[] array = new byte[num];
						safeExMemoryHandle.CopyTo(array, 0, num);
						try
						{
							rawSecurityDescriptor = new RawSecurityDescriptor(array, 0);
						}
						catch (ArgumentNullException ex)
						{
							throw MapiExceptionHelper.ArgumentNullException(ex.ParamName);
						}
						catch (ArgumentOutOfRangeException ex2)
						{
							throw MapiExceptionHelper.ArgumentOutOfRangeException(ex2.ParamName, ex2.Message);
						}
						catch (ArgumentException ex3)
						{
							throw MapiExceptionHelper.ArgumentException(ex3.ParamName, ex3.Message);
						}
					}
					result = rawSecurityDescriptor;
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public void SetMailboxSecurityDescriptor(Guid guidMdb, Guid guidMailbox, RawSecurityDescriptor sd)
		{
			if (sd == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("sd");
			}
			base.CheckDisposed();
			this.Lock();
			try
			{
				byte[] array = new byte[sd.BinaryLength];
				sd.GetBinaryForm(array, 0);
				int num = this.iExRpcAdmin.HrSetMailboxSecurityDescriptor(guidMdb, guidMailbox, array, array.Length);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set mailbox SecurityDescriptor.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void MountDatabase(Guid guidStorageGroup, Guid guidMdb, int ulFlags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrMountDatabase(guidStorageGroup, guidMdb, ulFlags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to mount database.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void UnmountDatabase(Guid guidStorageGroup, Guid guidMdb, int flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrUnmountDatabase(guidStorageGroup, guidMdb, flags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to unmount database.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public CheckpointStatus[] FlushCache()
		{
			base.CheckDisposed();
			this.Lock();
			CheckpointStatus[] checkpointStatus2;
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					CheckpointStatus[] checkpointStatus = null;
					int num = 0;
					int num2 = this.iExRpcAdmin.HrFlushCache(out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to flush cache.", num2);
					}
					checkpointStatus = new CheckpointStatus[num];
					int i = 0;
					safeExMemoryHandle.ForEach<CHECKPOINTSTATUSRAW>(num, delegate(CHECKPOINTSTATUSRAW item)
					{
						checkpointStatus[i++] = new CheckpointStatus(ref item);
					});
					checkpointStatus2 = checkpointStatus;
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return checkpointStatus2;
		}

		public void GetLastBackupTimes(Guid guidMdb, out DateTime lastCompleteBackupTime, out DateTime lastIncrementalBackupTime)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				long fileTime = 0L;
				long fileTime2 = 0L;
				int num = this.iExRpcAdmin.HrGetLastBackupTimes(guidMdb, out fileTime, out fileTime2);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get the time of last backup.", num);
				}
				lastCompleteBackupTime = DateTime.FromFileTimeUtc(fileTime);
				lastIncrementalBackupTime = DateTime.FromFileTimeUtc(fileTime2);
			}
			finally
			{
				this.Unlock();
			}
		}

		public void GetLastBackupInfo(Guid guidMdb, out DateTime lastCompleteBackupTime, out DateTime lastIncrementalBackupTime, out DateTime lastDifferentialBackup, out DateTime lastCopyBackup, out bool? SnapFull, out bool? SnapIncremental, out bool? SnapDifferential, out bool? SnapCopy)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				long fileTime = 0L;
				long fileTime2 = 0L;
				long fileTime3 = 0L;
				long fileTime4 = 0L;
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				int num5 = this.iExRpcAdmin.HrGetLastBackupInfo(guidMdb, out fileTime, out fileTime2, out fileTime3, out fileTime4, out num, out num2, out num3, out num4);
				if (num5 != 0)
				{
					base.ThrowIfError("Unable to get the time of last backup.", num5);
				}
				lastCompleteBackupTime = DateTime.FromFileTimeUtc(fileTime);
				lastIncrementalBackupTime = DateTime.FromFileTimeUtc(fileTime2);
				lastDifferentialBackup = DateTime.FromFileTimeUtc(fileTime3);
				lastCopyBackup = DateTime.FromFileTimeUtc(fileTime4);
				if (num != -1)
				{
					SnapFull = new bool?(num == 1);
				}
				else
				{
					SnapFull = null;
				}
				if (num2 != -1)
				{
					SnapIncremental = new bool?(num2 == 1);
				}
				else
				{
					SnapIncremental = null;
				}
				if (num3 != -1)
				{
					SnapDifferential = new bool?(num3 == 1);
				}
				else
				{
					SnapDifferential = null;
				}
				if (num4 != -1)
				{
					SnapCopy = new bool?(num4 == 1);
				}
				else
				{
					SnapCopy = null;
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void GetDatabaseSize(Guid guidMdb, out ulong dbSize, out ulong dbAvailableSpace)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				ulong num2;
				ulong num3;
				ulong num4;
				int num = this.iExRpcAdmin.HrGetDatabaseSize(guidMdb, out num2, out num3, out num4);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get the size of the database.", num);
				}
				dbSize = num2 * num4;
				dbAvailableSpace = num3 * num4;
			}
			finally
			{
				this.Unlock();
			}
		}

		public void PrePopulateCache(Guid guidMdb, string legacyDN, Guid guidMailbox, byte[] partitionHint, string dcName)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrPrePopulateCache(guidMdb, legacyDN, guidMailbox, partitionHint, dcName);
				if (num != 0)
				{
					base.ThrowIfError("Unable to prepopulate the cache for user " + legacyDN + " on Domain Controller " + dcName, num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void SyncMailboxesWithDS(Guid guidMdb)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrSyncMailboxesWithDS(guidMdb);
				if (num != 0)
				{
					base.ThrowIfError("Unable to sync mailboxes with DS", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void SyncMailboxWithDS(Guid guidMdb, Guid guidMailbox)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrSyncMailboxWithDS(guidMdb, guidMailbox);
				if (num != 0)
				{
					base.ThrowIfError("Unable to sync mailbox with DS", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void PurgeCachedMdbObject(Guid guidMdb)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrPurgeCachedMdbObject(guidMdb);
				if (num != 0)
				{
					base.ThrowIfError("Unable to purge the MDB DS cache", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void PurgeCachedMailboxObject(Guid guidMailbox)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrPurgeCachedMailboxObject(guidMailbox);
				if (num != 0)
				{
					base.ThrowIfError("Unable to purge cached mailbox object", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void ClearAbsentInDsFlagOnMailbox(Guid guidMdb, Guid guidMailbox)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrClearAbsentInDsFlagOnMailbox(guidMdb, guidMailbox);
				if (num != 0)
				{
					base.ThrowIfError("Unable to clear Absent In DS flag on mailbox object", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public bool HasLocalReplicas(Guid guidMdb)
		{
			base.CheckDisposed();
			this.Lock();
			bool result;
			try
			{
				bool flag = false;
				int num = this.iExRpcAdmin.HrHasLocalReplicas(guidMdb, out flag);
				if (num != 0)
				{
					base.ThrowIfError("Unable to determine if the store has local replicas", num);
				}
				result = flag;
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetReplicaInformationTable(Guid guidMdb, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetReplicaInformationTable(guidMdb, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get replica information.", num);
					}
					result = SRowSet.Unmarshal(safeExProwsHandle);
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public void CiCreatePropertyStore(Guid mdbGuid, Guid instanceGuid)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiCreatePropertyStore(mdbGuid, instanceGuid);
				if (num != 0)
				{
					base.ThrowIfError("Unable to create CI Property Store", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiDeletePropertyStore(Guid mdbGuid, Guid instanceGuid)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiDeletePropertyStore(mdbGuid, instanceGuid);
				if (num != 0)
				{
					base.ThrowIfError("Unable to delete CI Property Store", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiDeleteMailboxMapping(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiDeleteMailboxMapping(mdbGuid, instanceGuid, mailboxGuid);
				if (num != 0)
				{
					base.ThrowIfError("Unable to delete CI mailbox mapping", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiUpdateRetryTable(Guid mdbGuid, Guid instanceGuid, uint[] documentIds, Guid[] mailboxGuids, int[] hresults, short[] initialStates)
		{
			if (documentIds.Length != mailboxGuids.GetLength(0))
			{
				throw MapiExceptionHelper.ArgumentException("mailboxGuids", "Size of mailboxGuids array should match size of documentIds array.");
			}
			if (documentIds.Length != hresults.Length)
			{
				throw MapiExceptionHelper.ArgumentException("hresults", "Size of hresults array should match size of documentIds array.");
			}
			if (documentIds.Length != initialStates.Length)
			{
				throw MapiExceptionHelper.ArgumentException("initialStates", "Size of initialStates array should match size of documentIds array.");
			}
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiUpdateRetryTable(mdbGuid, instanceGuid, documentIds.Length, documentIds, mailboxGuids, hresults, initialStates);
				if (num != 0)
				{
					base.ThrowIfError("Unable to update CI Retry table ", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiEnumerateRetryList(Guid mdbGuid, Guid instanceGuid, out uint[] documentIds, out Guid[] mailboxGuids, out short[] states)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				SafeExMemoryHandle safeExMemoryHandle2 = null;
				SafeExMemoryHandle safeExMemoryHandle3 = null;
				try
				{
					int num = 0;
					int num2 = this.iExRpcAdmin.HrCiEnumerateRetryTable(mdbGuid, instanceGuid, out num, out safeExMemoryHandle, out safeExMemoryHandle2, out safeExMemoryHandle3);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to get CI retry list", num2);
					}
					if (num == 0)
					{
						documentIds = null;
						mailboxGuids = null;
						states = null;
					}
					else
					{
						documentIds = new uint[num];
						safeExMemoryHandle.CopyTo(documentIds, 0, num);
						mailboxGuids = new Guid[num];
						safeExMemoryHandle2.CopyTo(mailboxGuids, 0, num);
						states = new short[num];
						safeExMemoryHandle3.CopyTo(states, 0, num);
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
					if (safeExMemoryHandle2 != null)
					{
						safeExMemoryHandle2.Dispose();
					}
					if (safeExMemoryHandle3 != null)
					{
						safeExMemoryHandle3.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiGetEntryIdFromDocumentId(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, uint documentId, out byte[] entryId)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				try
				{
					int num = 0;
					int num2 = this.iExRpcAdmin.HrCiEntryIdFromDocumentId(mdbGuid, instanceGuid, mailboxGuid, documentId, out num, out safeExLinkedMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to get entry ID for CI document", num2);
					}
					entryId = new byte[num];
					safeExLinkedMemoryHandle.CopyTo(entryId, 0, num);
				}
				finally
				{
					if (safeExLinkedMemoryHandle != null)
					{
						safeExLinkedMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiGetWaterMark(Guid mdbGuid, Guid instanceGuid, bool isHighWatermark, out ulong watermark, out DateTime lastAccessTime)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				System.Runtime.InteropServices.ComTypes.FILETIME ft;
				int num = this.iExRpcAdmin.HrCiGetWaterMark(mdbGuid, instanceGuid, isHighWatermark, out watermark, out ft);
				lastAccessTime = ExRpcAdmin.DateTimeFromFileTime(ft);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get CI watermark", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetWaterMark(Guid mdbGuid, Guid instanceGuid, bool isHighWatermark, ulong watermark)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiSetWaterMark(mdbGuid, instanceGuid, isHighWatermark, watermark);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set CI watermark", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiGetMailboxState(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, out DateTime startTime, out uint state, out ulong eventCounter)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				System.Runtime.InteropServices.ComTypes.FILETIME ft;
				int num = this.iExRpcAdmin.HrCiGetMailboxState(mdbGuid, instanceGuid, mailboxGuid, out ft, out state, out eventCounter);
				startTime = ExRpcAdmin.DateTimeFromFileTime(ft);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get the CI state of a mailbox", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetMailboxState(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, DateTime startTime, uint state)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				long num = startTime.ToFileTimeUtc();
				System.Runtime.InteropServices.ComTypes.FILETIME filetime;
				filetime.dwHighDateTime = (int)((ulong)num >> 32);
				filetime.dwLowDateTime = (int)(num & (long)((ulong)-1));
				int num2 = this.iExRpcAdmin.HrCiSetMailboxStateAndStartDate(mdbGuid, instanceGuid, mailboxGuid, ref filetime, state);
				if (num2 != 0)
				{
					base.ThrowIfError("Unable to set the CI state and start date of a mailbox", num2);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetMailboxState(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, uint state)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiSetMailboxState(mdbGuid, instanceGuid, mailboxGuid, state);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set the CI state of a mailbox", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetMailboxEventCounter(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, ulong eventCounter)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiSetMailboxEventCounter(mdbGuid, instanceGuid, mailboxGuid, eventCounter);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set the CI event counter of a mailbox", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiEnumerateMailboxesByState(Guid mdbGuid, Guid instanceGuid, uint state, out Guid[] mailboxGuids)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					int num = 0;
					int num2 = this.iExRpcAdmin.HrCiEnumerateMailboxesByState(mdbGuid, instanceGuid, state, out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to enumerate mailboxes with the specified CI state", num2);
					}
					mailboxGuids = null;
					if (num != 0)
					{
						mailboxGuids = new Guid[num];
						safeExMemoryHandle.CopyTo(mailboxGuids, 0, num);
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiPurgeMailboxes(Guid mdbGuid, Guid instanceGuid, uint state)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiPurgeMailboxes(mdbGuid, instanceGuid, state);
				if (num != 0)
				{
					base.ThrowIfError("Unable to purge CI data for mailboxes", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetCiEnabled(Guid mdbGuid, bool isCiEnabled)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiSetCiEnabled(mdbGuid, isCiEnabled);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set CI enabled flag", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetIndexedPtags(Guid mdbGuid, PropTag[] proptags)
		{
			if (proptags == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("proptags");
			}
			if (proptags.Length == 0)
			{
				throw MapiExceptionHelper.ArgumentException("proptags", "proptags length should not be 0");
			}
			base.CheckDisposed();
			this.Lock();
			try
			{
				int[] array = new int[proptags.Length];
				for (int i = 0; i < proptags.Length; i++)
				{
					array[i] = (int)proptags[i];
				}
				int num = this.iExRpcAdmin.HrCiSetIndexedPtags(mdbGuid, proptags.Length, array);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set the list of ptags indexed by CI", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void DoMaintenanceTask(Guid guidMdb, MaintenanceTask task)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrDoMaintenanceTask(guidMdb, (int)task);
				if (num != 0)
				{
					base.ThrowIfError("Task " + task + " failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void ExecuteTask(Guid guidMdb, Guid taskClass, int taskId)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrExecuteTask(guidMdb, taskClass, taskId);
				if (num != 0)
				{
					base.ThrowIfError("HrExecuteTask failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSetCatalogState(Guid mdbGuid, Guid instanceGuid, short catalogState, byte[] blob)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				uint cbPropertyBlob = (uint)((blob == null) ? 0 : blob.Length);
				int num = this.iExRpcAdmin.HrCiSetCatalogState(mdbGuid, instanceGuid, catalogState, cbPropertyBlob, blob);
				if (num != 0)
				{
					base.ThrowIfError("Unable to Set Catalog State", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiGetCatalogState(Guid mdbGuid, Guid instanceGuid, out short catalogState, out byte[] blob)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				blob = null;
				catalogState = 0;
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					ulong num = 0UL;
					int num2 = this.iExRpcAdmin.HrCiGetCatalogState(mdbGuid, instanceGuid, out catalogState, out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to Get Catalog State", num2);
					}
					if (!safeExMemoryHandle.IsInvalid && num > 0UL)
					{
						blob = new byte[num];
						safeExMemoryHandle.CopyTo(blob, 0, (int)num);
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiUpdateFailedItem(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, uint documentId, uint errorCode, uint flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiUpdateFailedItem(mdbGuid, instanceGuid, mailboxGuid, documentId, errorCode, flags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to Update Failed Item", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiEnumerateFailedItems(Guid mdbGuid, Guid instanceGuid, uint lastMaxDocId, out PropValue[][] failedItems)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				failedItems = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrCiEnumerateFailedItems(mdbGuid, instanceGuid, lastMaxDocId, out safeExLinkedMemoryHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to Enumerate Failed Items", num);
					}
					PropValue[][] array = SRowSet.Unmarshal(safeExLinkedMemoryHandle);
					if (array.Length != 0)
					{
						failedItems = array;
					}
				}
				finally
				{
					if (safeExLinkedMemoryHandle != null)
					{
						safeExLinkedMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiEnumerateFailedItemsByMailbox(Guid mdbGuid, Guid instanceGuid, Guid mailboxGuid, uint lastMaxDocId, out PropValue[][] failedItems)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				failedItems = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrCiEnumerateFailedItemsByMailbox(mdbGuid, instanceGuid, mailboxGuid, lastMaxDocId, out safeExLinkedMemoryHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to Enumerate Failed Items", num);
					}
					PropValue[][] array = SRowSet.Unmarshal(safeExLinkedMemoryHandle);
					if (array.Length != 0)
					{
						failedItems = array;
					}
				}
				finally
				{
					if (safeExLinkedMemoryHandle != null)
					{
						safeExLinkedMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiUpdateFailedItemAndRetryTableByErrorCode(Guid mdbGuid, Guid instanceGuid, uint errorCode, uint lastMaxDocId, out uint curMaxDocId, out uint itemNumber)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiUpdateFailedItemAndRetryTableByErrorCode(mdbGuid, instanceGuid, errorCode, lastMaxDocId, out curMaxDocId, out itemNumber);
				if (num != 0)
				{
					base.ThrowIfError("Unable to Enumerate Failed Items", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiSeedPropertyStore(Guid mdbGuid, Guid sourceInstanceGuid, Guid destinationInstanceGuid)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiSeedPropertyStore(mdbGuid, sourceInstanceGuid, destinationInstanceGuid);
				if (num != 0)
				{
					base.ThrowIfError("Unable to seed property store.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void CiGetTableSize(Guid mdbGuid, Guid instanceGuid, short tableId, ulong flags, out ulong size)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCiGetTableSize(mdbGuid, instanceGuid, tableId, flags, out size);
				if (num != 0)
				{
					base.ThrowIfError("iExRpcAdmin.HrCiGetTableSize returned error.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public byte[] MultiMailboxSearch(Guid guidMdb, byte[] searchRequest)
		{
			if (guidMdb.Equals(Guid.Empty))
			{
				throw MapiExceptionHelper.ArgumentNullException("mmsInvalidMdbGuid");
			}
			if (searchRequest == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("mmsSearchRequest");
			}
			if (searchRequest.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("mmsSearchRequest", "Search Request cannot be empty.");
			}
			base.CheckDisposed();
			this.Lock();
			byte[] array = null;
			ulong num = 0UL;
			SafeExMemoryHandle safeExMemoryHandle = null;
			try
			{
				int num2 = this.iExRpcAdmin.HrMultiMailboxSearch(guidMdb, (ulong)((long)searchRequest.Length), searchRequest, out num, out safeExMemoryHandle);
				if (num2 != 0)
				{
					base.ThrowIfError("Multi Mailbox Search failed.", num2);
				}
				if (0UL != num && safeExMemoryHandle != null)
				{
					array = new byte[num];
					safeExMemoryHandle.CopyTo(array, 0, (int)num);
				}
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
					safeExMemoryHandle = null;
				}
				this.Unlock();
			}
			return array;
		}

		public byte[] GetMultiMailboxSearchKeywordStats(Guid guidMdb, byte[] searchRequest)
		{
			if (guidMdb.Equals(Guid.Empty))
			{
				throw MapiExceptionHelper.ArgumentNullException("mmsInvalidMdbGuid");
			}
			if (searchRequest == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("mmsSearchRequest");
			}
			if (searchRequest.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("mmsSearchRequest", "Search Request cannot be empty");
			}
			base.CheckDisposed();
			this.Lock();
			byte[] array = null;
			ulong num = 0UL;
			SafeExMemoryHandle safeExMemoryHandle = null;
			try
			{
				int num2 = this.iExRpcAdmin.HrGetMultiMailboxSearchKeywordStats(guidMdb, (ulong)((long)searchRequest.Length), searchRequest, out num, out safeExMemoryHandle);
				if (num2 != 0)
				{
					base.ThrowIfError("Multi Mailbox Keyword Stats Search failed.", num2);
				}
				if (0UL != num && safeExMemoryHandle != null && !safeExMemoryHandle.IsInvalid)
				{
					array = new byte[num];
					safeExMemoryHandle.CopyTo(array, 0, (int)num);
				}
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
					safeExMemoryHandle = null;
				}
				this.Unlock();
			}
			return array;
		}

		public unsafe byte[] SerializeAndFormatRestriction(Restriction restriction)
		{
			base.CheckDisposed();
			this.Lock();
			byte[] array = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			SRestriction* ptr = null;
			try
			{
				ulong num = 0UL;
				if (restriction != null)
				{
					int bytesToMarshal = restriction.GetBytesToMarshal();
					byte* ptr2 = stackalloc byte[(UIntPtr)bytesToMarshal];
					ptr = (SRestriction*)ptr2;
					ptr2 += (SRestriction.SizeOf + 7 & -8);
					restriction.MarshalToNative(ptr, ref ptr2);
					int num2 = this.iExRpcAdmin.HrFormatSearchRestriction(ptr, out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("HrFormatSearchRestriction failed.", num2);
					}
					if (num > 0UL && safeExMemoryHandle != null && !safeExMemoryHandle.IsInvalid)
					{
						array = new byte[num];
						safeExMemoryHandle.CopyTo(array, 0, array.Length);
					}
				}
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
					safeExMemoryHandle = null;
				}
				this.Unlock();
			}
			return array;
		}

		private static DateTime DateTimeFromFileTime(System.Runtime.InteropServices.ComTypes.FILETIME ft)
		{
			DateTime result = DateTime.MinValue;
			long fileTime = (long)ft.dwHighDateTime << 32 | ((long)ft.dwLowDateTime & (long)((ulong)-1));
			if (ft.dwHighDateTime != 0 || ft.dwLowDateTime != 0)
			{
				result = DateTime.FromFileTimeUtc(fileTime);
			}
			return result;
		}

		public void StartContentReplication(Guid guidMdb, byte[] entryId, ReplicationFlags flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrTriggerPFSync(guidMdb, entryId.Length, entryId, (int)flags);
				if (num != 0)
				{
					base.ThrowIfError("StartContentReplication failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void StartHierarchyReplication(Guid guidMdb, ReplicationFlags flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrTriggerPFSync(guidMdb, 0, null, (int)flags);
				if (num != 0)
				{
					base.ThrowIfError("StartHierachyReplication failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void SetPFReplicas(Guid guidMdb, string[] foldersDN, int[] ageLimits)
		{
			if (ageLimits.Length != foldersDN.Length)
			{
				throw MapiExceptionHelper.ArgumentException("ageLimits", "ageLimits and foldersDN should contain the same number of elements.");
			}
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrSetPFReplicas(guidMdb, foldersDN, ageLimits, ageLimits.Length);
				if (num != 0)
				{
					base.ThrowIfError("SetPFReplicas failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void GetPublicFolderDN(byte[] folderId, string folderName, out string publicFolderDN)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				publicFolderDN = string.Empty;
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetPublicFolderDN(folderId.Length, folderId, folderName, out safeExMemoryHandle);
					if (num != 0)
					{
						base.ThrowIfError("GetPublicFolderDN failed.", num);
					}
					if (safeExMemoryHandle.DangerousGetHandle() != IntPtr.Zero)
					{
						publicFolderDN = Marshal.PtrToStringAnsi(safeExMemoryHandle.DangerousGetHandle());
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		private static void DisposeIfNonNull(IDisposable ptr)
		{
			if (ptr != null)
			{
				ptr.Dispose();
			}
		}

		private void ProcessSnapshotOperation(Guid mdbGuid, SnapshotOperationCode operationCode, uint flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrProcessSnapshotOperation(mdbGuid, (uint)operationCode, flags);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable execute {0} on snapshot.", operationCode), num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void StartBlockModeReplicationToPassive(Guid guidMdb, string passiveName, uint ulFirstGenToSend)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrStartBlockModeReplicationToPassive(guidMdb, passiveName, ulFirstGenToSend);
				if (num != 0)
				{
					base.ThrowIfError("StartBlockModeReplicationToPassive rpc failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void StartSendingGranularLogData(Guid guidMdb, string pipeName, uint flags, uint maxIoDepth, uint maxIoLatency)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrStartSendingGranularLogData(guidMdb, pipeName, flags, maxIoDepth, maxIoLatency);
				if (num != 0)
				{
					base.ThrowIfError("StartSendingGranularLogData rpc failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void StopSendingGranularLogData(Guid guidMdb, uint flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrStopSendingGranularLogData(guidMdb, flags);
				if (num != 0)
				{
					base.ThrowIfError("StopSendingGranularLogData rpc failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public PropValue[][] GetRestrictionTable(MdbFlags mdbFlags, Guid guidMdb, Guid guidMailbox, byte[] entryIdFolder, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetRestrictionTable(guidMdb, guidMailbox, entryIdFolder.Length, entryIdFolder, PropTagHelper.SPropTagArray(propTagsRequested), (int)mdbFlags, out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get restriction information.", num);
					}
					result = SRowSet.Unmarshal(safeExProwsHandle);
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetViewsTable(MdbFlags mdbFlags, Guid guidMdb, Guid guidMailbox, byte[] entryIdFolder, params PropTag[] propTagsRequested)
		{
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			if (propTagsRequested.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTagsRequested", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetViewsTable(guidMdb, guidMailbox, entryIdFolder.Length, entryIdFolder, PropTagHelper.SPropTagArray(propTagsRequested), (int)mdbFlags, out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to get view information.", num);
					}
					result = SRowSet.Unmarshal(safeExProwsHandle);
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public void ISIntegCheck(Guid guidMdb, Guid guidMailbox, uint flags, int dbtasks, uint[] dbtaskids, out string requestId)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				requestId = string.Empty;
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrISIntegCheck(guidMdb, guidMailbox, flags, dbtasks, dbtaskids, out safeExMemoryHandle);
					if (num != 0)
					{
						base.ThrowIfError("Queue online isinteg request failed.", num);
					}
					if (safeExMemoryHandle.DangerousGetHandle() != IntPtr.Zero && requestId != null)
					{
						requestId = Marshal.PtrToStringAnsi(safeExMemoryHandle.DangerousGetHandle());
					}
				}
				finally
				{
					if (safeExMemoryHandle != null)
					{
						safeExMemoryHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void ForceNewLog(Guid guidMdb)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrForceNewLog(guidMdb);
				if (num != 0)
				{
					base.ThrowIfError("Cannot force a new log to be generated.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public PerRPCPerformanceStatistics GetStorePerRPCStats()
		{
			base.CheckDisposed();
			this.Lock();
			PerRPCPerformanceStatistics result;
			try
			{
				PerRpcStats nativeStats;
				uint storePerRPCStats = this.iExRpcAdmin.GetStorePerRPCStats(out nativeStats);
				result = PerRPCPerformanceStatistics.CreateFromNative(storePerRPCStats, nativeStats);
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] GetDatabaseProcessInfo(Guid mdbGuid, params PropTag[] propTags)
		{
			if (propTags == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTags");
			}
			if (propTags.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentException("propTags", "List of property tags should be not empty.");
			}
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrGetDatabaseProcessInfo(mdbGuid, PropTagHelper.SPropTagArray(propTags), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Unable to list process info.", num);
					}
					if (safeExProwsHandle == null || safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue[]>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle);
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public PropValue[][] StoreIntegrityCheckEx(Guid guidMdb, Guid guidMailbox, Guid guidRequest, uint operation, uint flags, uint[] taskIds, params PropTag[] propTagsRequested)
		{
			base.CheckDisposed();
			this.Lock();
			PropValue[][] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrStoreIntegrityCheckEx(guidMdb, guidMailbox, guidRequest, operation, flags, taskIds, PropTagHelper.SPropTagArray(propTagsRequested), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("Store Integrity check operation failed.", num);
					}
					if (safeExProwsHandle == null || safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue[]>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle);
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public void CreateUserInfo(Guid guidMdb, Guid guidUserInfo, uint flags, PropValue[] properties)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrCreateUserInfo(guidMdb, guidUserInfo, flags, properties ?? Array<PropValue>.Empty);
				if (num != 0)
				{
					base.ThrowIfError("HrCreateUserInfo failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public PropValue[] ReadUserInfo(Guid guidMdb, Guid guidUserInfo, uint flags, PropTag[] propTags)
		{
			base.CheckDisposed();
			this.Lock();
			PropValue[] result;
			try
			{
				SafeExProwsHandle safeExProwsHandle = null;
				try
				{
					int num = this.iExRpcAdmin.HrReadUserInfo(guidMdb, guidUserInfo, flags, PropTagHelper.SPropTagArray(propTags), out safeExProwsHandle);
					if (num != 0)
					{
						base.ThrowIfError("HrReadUserInfo failed.", num);
					}
					if (safeExProwsHandle == null || safeExProwsHandle.IsInvalid)
					{
						result = Array<PropValue>.Empty;
					}
					else
					{
						result = SRowSet.Unmarshal(safeExProwsHandle)[0];
					}
				}
				finally
				{
					if (safeExProwsHandle != null)
					{
						safeExProwsHandle.Dispose();
					}
				}
			}
			finally
			{
				this.Unlock();
			}
			return result;
		}

		public void UpdateUserInfo(Guid guidMdb, Guid guidUserInfo, uint flags, PropValue[] properties, PropTag[] deletePropTag)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrUpdateUserInfo(guidMdb, guidUserInfo, flags, properties ?? Array<PropValue>.Empty, PropTagHelper.SPropTagArray(deletePropTag));
				if (num != 0)
				{
					base.ThrowIfError("HrUpdateUserInfo failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void DeleteUserInfo(Guid guidMdb, Guid guidUserInfo, uint flags)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				int num = this.iExRpcAdmin.HrDeleteUserInfo(guidMdb, guidUserInfo, flags);
				if (num != 0)
				{
					base.ThrowIfError("HrDeleteUserInfo failed.", num);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void SnapshotPrepare(Guid mdbGuid, uint flags)
		{
			this.ProcessSnapshotOperation(mdbGuid, SnapshotOperationCode.Prepare, flags);
		}

		public void SnapshotFreeze(Guid mdbGuid, uint flags)
		{
			this.ProcessSnapshotOperation(mdbGuid, SnapshotOperationCode.Freeze, flags);
		}

		public void SnapshotThaw(Guid mdbGuid, uint flags)
		{
			this.ProcessSnapshotOperation(mdbGuid, SnapshotOperationCode.Thaw, flags);
		}

		public void SnapshotTruncateLogInstance(Guid mdbGuid, uint flags)
		{
			this.ProcessSnapshotOperation(mdbGuid, SnapshotOperationCode.Truncate, flags);
		}

		public void SnapshotStop(Guid mdbGuid, uint flags)
		{
			this.ProcessSnapshotOperation(mdbGuid, SnapshotOperationCode.Stop, flags);
		}

		public void LogReplayRequest(Guid guidMdb, uint ulgenLogReplayMax, uint ulLogReplayFlags, out uint ulgenLogReplayNext, out JET_DBINFOMISC dbinfo, out uint patchReplyPageNumber, out byte[] patchReplyToken, out byte[] patchReplyData, out uint[] pagesToBePatched)
		{
			base.CheckDisposed();
			this.Lock();
			ulgenLogReplayNext = 0U;
			patchReplyPageNumber = 0U;
			patchReplyToken = null;
			patchReplyData = null;
			pagesToBePatched = null;
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				SafeExMemoryHandle safeExMemoryHandle2 = null;
				SafeExMemoryHandle safeExMemoryHandle3 = null;
				SafeExMemoryHandle safeExMemoryHandle4 = null;
				try
				{
					uint num = 0U;
					uint num3;
					uint num4;
					uint num5;
					uint num6;
					int num2 = this.iExRpcAdmin.HrLogReplayRequest(guidMdb, ulgenLogReplayMax, ulLogReplayFlags, out ulgenLogReplayNext, out num, out safeExMemoryHandle, out num3, out num4, out safeExMemoryHandle2, out num5, out safeExMemoryHandle3, out num6, out safeExMemoryHandle4);
					if (num2 != 0)
					{
						base.ThrowIfError("LogReplayRequest rpc failed.", num2);
					}
					byte[] array = new byte[num];
					Marshal.Copy(safeExMemoryHandle.DangerousGetHandle(), array, 0, (int)num);
					InteropShim.DeserializeDatabaseInfo(array, out dbinfo);
					patchReplyPageNumber = num3;
					if (num4 != 0U)
					{
						patchReplyToken = new byte[num4];
						safeExMemoryHandle2.CopyTo(patchReplyToken, 0, (int)num4);
					}
					if (num5 != 0U)
					{
						patchReplyData = new byte[num5];
						safeExMemoryHandle3.CopyTo(patchReplyData, 0, (int)num5);
					}
					if (num6 != 0U)
					{
						pagesToBePatched = new uint[num6];
						safeExMemoryHandle4.CopyTo(pagesToBePatched, 0, (int)num6);
					}
				}
				finally
				{
					ExRpcAdmin.DisposeIfNonNull(safeExMemoryHandle);
					ExRpcAdmin.DisposeIfNonNull(safeExMemoryHandle2);
					ExRpcAdmin.DisposeIfNonNull(safeExMemoryHandle3);
					ExRpcAdmin.DisposeIfNonNull(safeExMemoryHandle4);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		public void GetPhysicalDatabaseInformation(Guid guidMdb, out JET_DBINFOMISC dbinfomisc)
		{
			base.CheckDisposed();
			this.Lock();
			try
			{
				SafeExMemoryHandle safeExMemoryHandle = null;
				try
				{
					uint num = 0U;
					int num2 = this.iExRpcAdmin.HrGetPhysicalDatabaseInformation(guidMdb, out num, out safeExMemoryHandle);
					if (num2 != 0)
					{
						base.ThrowIfError("GetPhysicalDatabaseInformation rpc failed.", num2);
					}
					byte[] array = new byte[num];
					Marshal.Copy(safeExMemoryHandle.DangerousGetHandle(), array, 0, (int)num);
					InteropShim.DeserializeDatabaseInfo(array, out dbinfomisc);
				}
				finally
				{
					ExRpcAdmin.DisposeIfNonNull(safeExMemoryHandle);
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		private SafeExRpcAdminHandle iExRpcAdmin;

		private uint threadLockCount;

		private Thread owningThread;
	}
}
