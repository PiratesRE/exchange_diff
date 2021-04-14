using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
using ATL;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal class CReplicaVssWriterInterop : IServiceComponent
	{
		public CReplicaVssWriterInterop(IReplicaInstanceManager replicaInstanceManager)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriter::CReplicaVssWriter");
			this.m_fValidObj = false;
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_restoreInProgressComponents = new List<ComponentInformation>();
		}

		public virtual string Name
		{
			get
			{
				return "VSS Writer";
			}
		}

		public virtual FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.VSSWriter;
			}
		}

		public virtual bool IsCritical
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return false;
			}
		}

		public virtual bool IsEnabled
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				if (!RegistryParameters.EnableVssWriter)
				{
					return false;
				}
				if (ThirdPartyManager.IsInitialized && ThirdPartyManager.IsThirdPartyReplicationEnabled)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriter::IsEnabled() returns false because TPR is enabled");
					return false;
				}
				return true;
			}
		}

		public virtual bool IsRetriableOnError
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return false;
			}
		}

		public virtual void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public virtual bool Start()
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriter::Start(): Starting Replica Vss Writer...");
			int num = this.Initialize();
			if (num < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "CReplicaVssWriter::Start(): Replica Vss Writer Initialize() *FAILED* with hr = 0x{0:x}", num);
				return false;
			}
			return true;
		}

		public virtual void Stop()
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriter::Stop(): Replica Vss Writer shutting down...");
			this.Shutdown();
		}

		public unsafe int Initialize()
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriter::Initialize");
			this.m_populateMetadataDelegate = new IdentifyDelegate(this.Identify);
			this.m_prepareBackupDelegate = new PrepareBackupDelegate(this.PrepareBackup);
			this.m_prepareSnapshotDelegate = new PrepareSnapshotDelegate(this.PrepareSnapshot);
			this.m_freezeOrThawDelegate = new FreezeOrThawDelegate(this.FreezeOrThaw);
			this.m_abortDelegate = new AbortDelegate(this.Abort);
			this.m_postSnapshotDelegate = new PostSnapshotDelegate(this.PostSnapshot);
			this.m_backupCompleteDelegate = new BackupCompleteDelegate(this.BackupComplete);
			this.m_shutdownBackupDelegate = new ShutdownBackupDelegate(this.ShutdownBackup);
			this.m_preRestoreDelegate = new PreRestoreDelegate(this.PreRestore);
			this.m_postRestoreDelegate = new PostRestoreDelegate(this.PostRestore);
			this.m_pfnIdentify = Marshal.GetFunctionPointerForDelegate(this.m_populateMetadataDelegate).ToPointer();
			this.m_pfnPrepareBackup = Marshal.GetFunctionPointerForDelegate(this.m_prepareBackupDelegate).ToPointer();
			this.m_pfnPrepareSnapshot = Marshal.GetFunctionPointerForDelegate(this.m_prepareSnapshotDelegate).ToPointer();
			this.m_pfnFreezeOrThaw = Marshal.GetFunctionPointerForDelegate(this.m_freezeOrThawDelegate).ToPointer();
			this.m_pfnAbort = Marshal.GetFunctionPointerForDelegate(this.m_abortDelegate).ToPointer();
			this.m_pfnPostSnapshot = Marshal.GetFunctionPointerForDelegate(this.m_postSnapshotDelegate).ToPointer();
			this.m_pfnBackupComplete = Marshal.GetFunctionPointerForDelegate(this.m_backupCompleteDelegate).ToPointer();
			this.m_pfnShutdownBackup = Marshal.GetFunctionPointerForDelegate(this.m_shutdownBackupDelegate).ToPointer();
			this.m_pfnPreRestore = Marshal.GetFunctionPointerForDelegate(this.m_preRestoreDelegate).ToPointer();
			this.m_pfnPostRestore = Marshal.GetFunctionPointerForDelegate(this.m_postRestoreDelegate).ToPointer();
			CReplicaVssWriter* ptr = <Module>.@new(144UL);
			CReplicaVssWriter* replicaWriter;
			try
			{
				if (ptr != null)
				{
					replicaWriter = <Module>.CReplicaVssWriter.{ctor}(ptr, this.m_pfnIdentify, this.m_pfnPrepareBackup, this.m_pfnPrepareSnapshot, this.m_pfnFreezeOrThaw, this.m_pfnAbort, this.m_pfnPostSnapshot, this.m_pfnBackupComplete, this.m_pfnShutdownBackup, this.m_pfnPreRestore, this.m_pfnPostRestore);
				}
				else
				{
					replicaWriter = 0L;
				}
			}
			catch
			{
				<Module>.delete((void*)ptr);
				throw;
			}
			this.m_replicaWriter = replicaWriter;
			this.m_backupInstances = new Dictionary<Guid, BackupInstance>();
			int num = <Module>.CReplicaVssWriter.Initialize(this.m_replicaWriter);
			if (num >= 0)
			{
				this.m_fValidObj = true;
			}
			else
			{
				object[] array = new object[1];
				int num2 = num;
				array[0] = num2.ToString("X");
				ReplayEventLogConstants.Tuple_VssInitFailed.LogEvent(string.Empty, array);
			}
			return num;
		}

		public unsafe void Shutdown()
		{
			this.m_fValidObj = false;
			CReplicaVssWriter* replicaWriter = this.m_replicaWriter;
			if (replicaWriter != null)
			{
				<Module>.CReplicaVssWriter.Uninitialize(replicaWriter);
			}
			CReplicaVssWriter* replicaWriter2 = this.m_replicaWriter;
			if (replicaWriter2 != null)
			{
				object obj = calli(System.Void* modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32), replicaWriter2, 1, *(*(long*)replicaWriter2));
			}
			this.m_replicaWriter = null;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool Identify(IVssCreateWriterMetadata* pMetadata)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = false;
			byte result;
			try
			{
				bool flag5;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Identify");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag4 = true;
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Identify FValidObj failed.");
						return false;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong>((long)this.GetHashCode(), "pMetadata = 0x{0:X16}", pMetadata);
					List<ReplayConfiguration> list = this.FindAllDatabaseConfigurations();
					if (list.Count < 1)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "VSSWriter Identify failed because we didn't find any replicas");
						flag3 = false;
						return false;
					}
					foreach (ReplayConfiguration replayConfiguration in list)
					{
						if (replayConfiguration.Database.Recovery)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Skip recovery database {0}.", replayConfiguration.Database.DistinguishedName);
						}
						else
						{
							flag3 = true;
							num = this.IdentifyReplica(pMetadata, replayConfiguration);
							if (num < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "VSSWriter Identify IdentifyReplica failed with {0:X8}", num);
								flag2 = this.FSetWriterFailureFromHResult(num);
								return flag2;
							}
						}
					}
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, 33559, *(*(long*)pMetadata + 72L));
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "VSSWriter Identify SetBackupSchema failed with {0:X8}", num);
						flag2 = this.FSetWriterFailureFromHResult(num);
						return flag2;
					}
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,VSS_RESTOREMETHOD_ENUM,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,VSS_WRITERRESTORE_ENUM,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride)), pMetadata, 2, 0L, 0L, 3, 0, *(*(long*)pMetadata + 48L));
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "VSSWriter Identify SetRestoreMethod failed with {0:X8}", num);
						flag2 = this.FSetWriterFailureFromHResult(num);
						return flag2;
					}
					flag = true;
					flag2 = true;
					return true;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Identify");
					flag2 = this.FSetWriterFailureFromHResult(num);
					flag5 = flag2;
				}
				result = (flag5 ? 1 : 0);
			}
			finally
			{
				if (flag3)
				{
					if (flag)
					{
						object[] messageArgs = new object[0];
						ReplayEventLogConstants.Tuple_VSSWriterMetadata.LogEvent(string.Empty, messageArgs);
					}
					else
					{
						object[] array = new object[1];
						int num2 = num;
						array[0] = num2.ToString("X");
						ReplayEventLogConstants.Tuple_VSSWriterMetadataError.LogEvent(string.Empty, array);
					}
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Identify -- {0}", flag2);
				if (flag4)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool PrepareBackup(IVssWriterComponents* pComponents)
		{
			BackupInstance backupInstance = null;
			List<ReplayConfiguration> list = null;
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			ushort* ptr = null;
			ushort* ptr2 = null;
			ushort* ptr3 = null;
			Guid guid = default(Guid);
			byte result;
			try
			{
				bool flag4;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackup");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag3 = true;
					_GUID guid3;
					Guid guid2 = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid3));
					guid = guid2;
					backupInstance = new BackupInstance(guid2);
					uint num2 = 0U;
					uint num3 = 0;
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackup FValidObj failed");
						num = -2147212301;
						flag2 = this.FSetWriterFailureFromHResult(-2147212301);
						return flag2;
					}
					if (this.m_backupInstances.ContainsKey(guid2))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "CReplicaVssWriterInterop::PrepareBackup Backup instance already in progress");
						num = -2147212301;
						flag2 = this.FSetWriterFailureFromHResult(-2147212301);
						return flag2;
					}
					if (<Module>.CReplicaVssWriter.AreComponentsSelected(this.m_replicaWriter) == null)
					{
						backupInstance.m_vssbackuptype = 5;
						this.m_backupInstances.Add(guid2, backupInstance);
						flag2 = true;
						return true;
					}
					object[] array = new object[0];
					if (pComponents != null)
					{
						num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponents, ref num2, *(*(long*)pComponents));
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareBackup Failed to find any component hr = {0:X8}", num);
							flag2 = this.FSetWriterFailureFromHResult(num);
							return flag2;
						}
					}
					int num4 = <Module>.CReplicaVssWriter.GetBackupType(this.m_replicaWriter);
					backupInstance.m_vssbackuptype = num4;
					int num5 = num4;
					if (num5 <= 0 || (num5 > 3 && num5 != 5))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareBackup Failed due to unknown backup type {0}", num4);
						num = -2147212300;
						flag2 = this.FSetWriterFailureFromHResult(-2147212300);
						return flag2;
					}
					if (num2 == 0U)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "PrepareBackup no component was selected ulcompCount == 0 ");
						backupInstance.m_fNoComponents = true;
						this.m_backupInstances.Add(guid2, backupInstance);
						flag2 = true;
						return true;
					}
					object[] array2 = new object[0];
					list = this.FindAllDatabaseConfigurations();
					num3 = 0;
					while (num3 < num2)
					{
						CComPtr<IVssComponent> ccomPtr<IVssComponent>;
						<Module>.ATL.CComPtr<IVssComponent>.{ctor}(ref ccomPtr<IVssComponent>);
						try
						{
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssComponent**), pComponents, num3, <Module>.ATL.CComPtrBase<IVssComponent>.&(ref ccomPtr<IVssComponent>), *(*(long*)pComponents + 16L));
							if (num >= 0)
							{
								goto IL_245;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponent failed with {0:X8}", num);
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_237;
						IL_245:
						try
						{
							if (0L != <Module>.ATL.CComPtrBase<IVssComponent>..PEAVIVssComponent@@(ref ccomPtr<IVssComponent>))
							{
								goto IL_29D;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "GetComponent failure (returned NULL)");
							num = <Module>.HRESULT_FROM_WIN32(1610);
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_28F;
						IL_29D:
						try
						{
							if (ptr != null)
							{
								<Module>.SysFreeString(ptr);
								ptr = null;
							}
							_NoAddRefReleaseOnCComPtr<IVssComponent>* ptr4 = <Module>.ATL.CComPtrBase<IVssComponent>.->(ref ccomPtr<IVssComponent>);
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr4, ref ptr, *(*(long*)ptr4 + 40L));
							if (num >= 0)
							{
								goto IL_30E;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentName failed with {0:X8}", num);
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_300;
						IL_30E:
						try
						{
							if (null != ptr)
							{
								goto IL_356;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "GetComponentName failure (returned NULL)");
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_348;
						IL_356:
						try
						{
							if (ptr2 != null)
							{
								<Module>.SysFreeString(ptr2);
								ptr2 = null;
							}
							_NoAddRefReleaseOnCComPtr<IVssComponent>* ptr5 = <Module>.ATL.CComPtrBase<IVssComponent>.->(ref ccomPtr<IVssComponent>);
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr5, ref ptr2, *(*(long*)ptr5 + 24L));
							if (num >= 0)
							{
								goto IL_3C7;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetLogicalPath failed with {0:X8}", num);
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_3B9;
						IL_3C7:
						try
						{
							if (null != ptr2)
							{
								goto IL_40F;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "GetLogicalPath failure (returned NULL)");
							flag2 = this.FSetWriterFailureFromHResult(num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_401;
						IL_40F:
						List<ReplayConfiguration>.Enumerator enumerator;
						try
						{
							enumerator = list.GetEnumerator();
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						for (;;)
						{
							try
							{
								if (!enumerator.MoveNext())
								{
									break;
								}
								ReplayConfiguration replayConfiguration = enumerator.Current;
								this.FreeUnmanagedWideString(ptr3);
								ptr3 = this.GetUnmanagedWideString(replayConfiguration.IdentityGuid.ToString());
								ushort* ptr6 = ptr;
								ushort* ptr7 = ptr3;
								int num6 = 0;
								for (;;)
								{
									short num7 = *(short*)ptr7;
									short num8 = *(short*)ptr6;
									if (num7 < num8)
									{
										break;
									}
									if (num7 > num8)
									{
										break;
									}
									if (num7 == 0)
									{
										goto IL_4A3;
									}
									ptr7 += 2L / 2L;
									ptr6 += 2L / 2L;
								}
								goto IL_501;
								IL_4A3:
								if (0 != num6)
								{
									goto IL_501;
								}
								num = this.PrepareBackupReplica(<Module>.ATL.CComPtrBase<IVssComponent>..PEAVIVssComponent@@(ref ccomPtr<IVssComponent>), replayConfiguration, backupInstance);
								if (num >= 0)
								{
									break;
								}
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareBackupReplica failed with {0:X8}", num);
								flag2 = this.FSetWriterFailureFromHResult(num);
							}
							catch
							{
								<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
								throw;
							}
							goto IL_4F3;
							IL_501:
							try
							{
								continue;
							}
							catch
							{
								<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
								throw;
							}
							break;
						}
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						num3++;
						continue;
						IL_4F3:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_401:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_3B9:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_348:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_300:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_28F:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
						IL_237:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return flag2;
					}
					object[] array3 = new object[0];
					backupInstance.m_fBackupPrepared = true;
					this.m_backupInstances.Add(guid, backupInstance);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Adding backup instance {0}", guid);
					object[] array4 = new object[0];
					flag = true;
					flag2 = true;
					return true;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackup");
					flag2 = this.FSetWriterFailureFromHResult(num);
					flag4 = flag2;
				}
				result = (flag4 ? 1 : 0);
			}
			finally
			{
				if (flag)
				{
					object[] messageArgs = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterBackup.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] array5 = new object[2];
					array5[0] = guid;
					int num9 = num;
					array5[1] = num9.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterBackupError.LogEvent(string.Empty, array5);
				}
				if (ptr != null)
				{
					<Module>.SysFreeString(ptr);
					ptr = null;
				}
				if (ptr2 != null)
				{
					<Module>.SysFreeString(ptr2);
					ptr2 = null;
				}
				IntPtr hglobal = new IntPtr((void*)ptr3);
				Marshal.FreeHGlobal(hglobal);
				if (flag3)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackup -- {0}", flag2);
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool PrepareSnapshot()
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Guid guid = default(Guid);
			BackupInstance backupInstance = null;
			byte result;
			try
			{
				bool flag5;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshot");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag3 = true;
					bool flag4 = ((<Module>.CReplicaVssWriter.AreComponentsSelected(this.m_replicaWriter) == 0) ? 1 : 0) != 0;
					_GUID guid3;
					Guid guid2 = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid3));
					guid = guid2;
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshot FValidObj failed");
						num = -2147212301;
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					object[] array = new object[0];
					this.m_backupInstances.ContainsKey(guid2);
					flag = this.m_backupInstances.TryGetValue(guid2, out backupInstance);
					if (!flag)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Backup instance not found. Returning VSS_E_WRITERERROR_RETRYABLE.");
						num = -2147212301;
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					object[] array2 = new object[0];
					object[] array3 = new object[0];
					bool fBackupPrepared = backupInstance.m_fBackupPrepared;
					SnapshotPrepareGrbit flags = (SnapshotPrepareGrbit)12;
					int vssbackuptype = backupInstance.m_vssbackuptype;
					if (vssbackuptype != 1)
					{
						if (vssbackuptype != 2)
						{
							if (vssbackuptype != 3)
							{
								if (vssbackuptype != 5)
								{
									object[] array4 = new object[0];
									num = <Module>.HRESULT_FROM_WIN32(87);
									flag = this.FSetWriterFailureFromHResult(num);
									return flag;
								}
								flags = (SnapshotPrepareGrbit)14;
							}
							else
							{
								flags = (SnapshotPrepareGrbit)15;
							}
						}
						else
						{
							flags = (SnapshotPrepareGrbit)13;
						}
					}
					backupInstance.m_fBackupPrepared = false;
					backupInstance.m_fFrozen = false;
					backupInstance.m_fSnapPrepared = false;
					if (!flag4)
					{
						if (backupInstance.m_fNoComponents)
						{
							flag = true;
							return true;
						}
						num = this.PrepareSnapshotComponents();
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareSnapshotComponents failed with {0:X8}", num);
							flag = this.FSetWriterFailureFromHResult(num);
							return flag;
						}
					}
					else
					{
						num = this.PrepareSnapshotVolume();
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareSnapshotVolume failed with {0:X8}", num);
							flag = this.FSetWriterFailureFromHResult(num);
							return flag;
						}
					}
					(new object[1])[0] = guid2;
					foreach (StorageGroupBackup storageGroupBackup in backupInstance.m_StorageGroupBackups)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Finalize snapshot prepare for Instance {0}.", storageGroupBackup.m_guidSGIdentityGuid);
						if (storageGroupBackup.m_fIsPassive)
						{
							if (!storageGroupBackup.m_fFrozen)
							{
								num = this.FreezeOrThawReplica(storageGroupBackup, true);
								if (num < 0)
								{
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid, int>((long)this.GetHashCode(), "CReplicaVssWriterInterop::FreezeOrThawReplica for {0} failed with {1:X8}", storageGroupBackup.m_guidSGIdentityGuid, num);
									flag = this.FSetWriterFailureFromHResult(num);
									return flag;
								}
								storageGroupBackup.m_fFrozen = true;
							}
							else
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Instance {0} is already in frozen, so skip freezing it", storageGroupBackup.m_guidSGIdentityGuid);
							}
							num = this.HrBeginSurrogateBackupOnPrimarySG(storageGroupBackup);
							if (num < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid, int>((long)this.GetHashCode(), "HrBeginSurrogateBackupsOnPrimarySGs for {0} failed with {1:X8}", storageGroupBackup.m_guidSGIdentityGuid, num);
								flag = this.FSetWriterFailureFromHResult(num);
								return flag;
							}
						}
						else
						{
							CReplicaVssWriterInterop.SnapshotPrepare(storageGroupBackup.m_guidSGIdentityGuid, (uint)flags);
						}
					}
					backupInstance.m_fSnapPrepared = true;
					flag = true;
					flag2 = true;
					return true;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshot");
					flag = this.FSetWriterFailureFromHResult(num);
					flag5 = flag;
				}
				result = (flag5 ? 1 : 0);
			}
			finally
			{
				if (flag2)
				{
					object[] messageArgs = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterSnapshot.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] array5 = new object[2];
					array5[0] = guid;
					int num2 = num;
					array5[1] = num2.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterSnapshotError.LogEvent(string.Empty, array5);
				}
				if (flag3)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshot -- {0}", flag);
			}
			return result != 0;
		}

		public unsafe int FreezeOrThaw([MarshalAs(UnmanagedType.U1)] bool fFreeze, [MarshalAs(UnmanagedType.U1)] bool fLock)
		{
			int num = 0;
			bool flag = false;
			Guid guid = default(Guid);
			int result;
			try
			{
				int num2;
				try
				{
					object[] array = new object[1];
					string text;
					if (fFreeze)
					{
						text = "Freeze";
					}
					else
					{
						text = "Thaw";
					}
					array[0] = text;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThaw to {0}", array);
					if (fLock)
					{
						<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
						flag = true;
					}
					_GUID guid3;
					Guid guid2 = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid3));
					guid = guid2;
					if (!this.m_fValidObj)
					{
						num = -2147212301;
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThawFValidObj failed. Failed with {0:X8}.", -2147212301);
						return -2147212301;
					}
					object[] array2 = new object[0];
					this.m_backupInstances.ContainsKey(guid2);
					if (!this.m_backupInstances.ContainsKey(guid2))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0}: the snapshot id {1} was not found in m_backupInstances.", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThaw", guid2);
						num = -2147212301;
						return -2147212301;
					}
					(new object[1])[0] = guid2;
					BackupInstance backupInstance = this.m_backupInstances[guid2];
					num = this.FreezeOrThawReplicas(fFreeze);
					if (num < 0)
					{
						return num;
					}
					this.m_backupInstances[guid2].m_fFrozen = fFreeze;
					return num;
				}
				catch (Exception ex)
				{
					num = -3;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "{0}:some exception is thrown {1}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThaw", ex);
					num = this.HandleException(ex, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThaw");
					num2 = num;
				}
				result = num2;
			}
			finally
			{
				if (num >= 0)
				{
					if (fFreeze)
					{
						object[] messageArgs = new object[]
						{
							guid
						};
						ReplayEventLogConstants.Tuple_VSSWriterFreeze.LogEvent(string.Empty, messageArgs);
					}
					else
					{
						object[] messageArgs2 = new object[]
						{
							guid
						};
						ReplayEventLogConstants.Tuple_VSSWriterThaw.LogEvent(string.Empty, messageArgs2);
					}
				}
				else if (fFreeze)
				{
					object[] messageArgs3 = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterFreezeError.LogEvent(string.Empty, messageArgs3);
				}
				else
				{
					object[] messageArgs4 = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterThawError.LogEvent(string.Empty, messageArgs4);
				}
				if (flag)
				{
					new object[0];
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThaw -- {0:x}", num);
			}
			return result;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool Abort()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Guid guid = default(Guid);
			byte result;
			try
			{
				bool flag4;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Abort");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag3 = true;
					_GUID guid3;
					Guid guid2 = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid3));
					guid = guid2;
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Abort FValidObj failed. Failed with {0:X8}.", -2147212301);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					if (!this.m_backupInstances.ContainsKey(guid2))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "{0}: the snapshot id {1} was not found in m_backupInstances.", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Abort", guid2);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					object[] array = new object[0];
					BackupInstance backupInstance = this.m_backupInstances[guid2];
					if (this.m_backupInstances.ContainsKey(guid2))
					{
						this.CleanupBackup(guid2);
					}
					object[] array2 = new object[0];
					this.m_backupInstances.ContainsKey(guid2);
					flag2 = true;
					flag = true;
					return true;
				}
				catch (Exception e)
				{
					int hr = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Abort");
					flag = this.FSetWriterFailureFromHResult(hr);
					flag4 = flag;
				}
				result = (flag4 ? 1 : 0);
			}
			finally
			{
				if (flag2)
				{
					object[] messageArgs = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterAbort.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] messageArgs2 = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterAbortError.LogEvent(string.Empty, messageArgs2);
				}
				if (flag3)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::Abort -- {0}", flag);
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool PostSnapshot(IVssWriterComponents* pComponents)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Guid guid = default(Guid);
			byte result;
			try
			{
				bool flag4;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag3 = true;
					_GUID guid3;
					Guid guid2 = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid3));
					guid = guid2;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Guid ID in Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot is {0}", guid2);
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot FValidObj failed. Failed with {0:X8}.", -2147212301);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					if (!this.m_backupInstances.ContainsKey(guid2))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot: the snapshot id {0} was not found in m_backupInstances.", guid2);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					object[] array = new object[0];
					BackupInstance backupInstance = this.m_backupInstances[guid2];
					this.m_backupInstances[guid2].m_fPostSnapshot = true;
					flag2 = true;
					flag = true;
					return true;
				}
				catch (Exception e)
				{
					int hr = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot");
					flag = this.FSetWriterFailureFromHResult(hr);
					flag4 = flag;
				}
				result = (flag4 ? 1 : 0);
			}
			finally
			{
				if (flag2)
				{
					object[] messageArgs = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterPostSnapshot.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] messageArgs2 = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterPostSnapshotError.LogEvent(string.Empty, messageArgs2);
				}
				if (flag3)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostSnapshot -- {0}", flag);
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool BackupComplete(IVssWriterComponents* pComponents)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Guid empty = Guid.Empty;
			byte result;
			try
			{
				bool flag4;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupComplete");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag3 = true;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong>((long)this.GetHashCode(), "pComponents = 0x{0:X16}", pComponents);
					if (!this.m_fValidObj)
					{
						num = -2147212301;
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupComplete FValidObj failed. Failed with {0:X8}.", -2147212301);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					if (null == pComponents)
					{
						flag = true;
						return true;
					}
					num = this.VssIdCurrentSnapshotSetIdFromComponents(pComponents, ref empty);
					if (num < 0)
					{
						flag = this.FSetWriterFailureFromHResult(num);
						return flag;
					}
					object[] array = new object[0];
					this.m_backupInstances.ContainsKey(empty);
					object[] array2 = new object[0];
					BackupInstance backupInstance = this.m_backupInstances[empty];
					if (<Module>.CReplicaVssWriter.AreComponentsSelected(this.m_replicaWriter) == null)
					{
						flag = true;
						return true;
					}
					if (<Module>.CReplicaVssWriter.AreComponentsSelected(this.m_replicaWriter) != null && this.m_backupInstances[empty].m_fNoComponents)
					{
						flag = true;
						return true;
					}
					int num2;
					if (this.m_backupInstances[empty].m_vssbackuptype != 1 && this.m_backupInstances[empty].m_vssbackuptype != 2)
					{
						num2 = 0;
					}
					else
					{
						num2 = 1;
					}
					bool fTruncateLogs = (byte)num2 != 0;
					num = this.BackupCompleteTruncateLogsComponents(pComponents, empty, fTruncateLogs);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "BackupCompleteTruncateLogsComponents failed with {0:X8}.", num);
						flag = this.FSetWriterFailureFromHResult(num);
						return flag;
					}
					flag2 = true;
					flag = true;
					return true;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupComplete");
					flag = this.FSetWriterFailureFromHResult(num);
					flag4 = flag;
				}
				result = (flag4 ? 1 : 0);
			}
			finally
			{
				if (!empty.Equals(Guid.Empty))
				{
					this.CleanupBackup(empty);
					new object[0];
					this.m_backupInstances.ContainsKey(empty);
				}
				if (flag2)
				{
					object[] messageArgs = new object[]
					{
						empty
					};
					ReplayEventLogConstants.Tuple_VSSWriterBackupComplete.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] array3 = new object[2];
					array3[0] = empty;
					int num3 = num;
					array3[1] = num3.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteError.LogEvent(string.Empty, array3);
				}
				if (flag3)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupComplete -- {0}", flag);
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool ShutdownBackup(_GUID snapshotSetId)
		{
			bool flag = false;
			Guid guid = default(Guid);
			int num = 0;
			try
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::ShutdownBackup");
				<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
				Guid arg = this.GuidFromUnmanagedGuid(snapshotSetId);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "snapshotSetId = {0}", arg);
				Guid guid2 = this.GuidFromUnmanagedGuid(snapshotSetId);
				guid = guid2;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "current id is {0}", guid2);
				if (!this.m_fValidObj)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::ShutdownBackup FValidObj failed. Returning VSS_E_WRITERERROR_RETRYABLE.");
					num = -2147212301;
					return true;
				}
				if (this.m_backupInstances.ContainsKey(guid2))
				{
					this.CleanupBackup(guid2);
				}
				else
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "ShutdownBackup couldn't find {0} in backupinstance list", guid2);
				}
				object[] array = new object[0];
				this.m_backupInstances.ContainsKey(guid2);
				flag = true;
			}
			catch (Exception e)
			{
				num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::ShutdownBackup");
			}
			finally
			{
				if (flag)
				{
					object[] messageArgs = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterOnBackupShutdown.LogEvent(string.Empty, messageArgs);
				}
				else
				{
					object[] messageArgs2 = new object[]
					{
						guid
					};
					ReplayEventLogConstants.Tuple_VSSWriterOnBackupShutdownError.LogEvent(string.Empty, messageArgs2);
				}
				if (num < 0)
				{
					int num2;
					if (num != -2147212300)
					{
						if (num == -2147024882)
						{
							num2 = -2147212303;
							goto IL_182;
						}
						if (num != -3)
						{
							num2 = -2147212301;
							goto IL_182;
						}
					}
					num2 = -2147212300;
					IL_182:
					<Module>.CReplicaVssWriter.SetWriterFailure(this.m_replicaWriter, num2);
					flag = (num2 == -2147212301 || flag);
				}
				<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::ShutdownBackup -- {0}", flag);
			}
			return flag;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool PreRestore(IVssWriterComponents* pComponents)
		{
			bool flag = false;
			bool flag2 = false;
			byte result;
			try
			{
				bool flag3;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PreRestore");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag2 = true;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong>((long)this.GetHashCode(), "pComponents = 0x{0:X16}", pComponents);
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PreRestore FValidObj failed. Returning {0:X8}.", -2147212301);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					int num = this.HrOnRestore(pComponents, 0);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PreRestore: HrOnRestore failed with {0:X8}", num);
						flag = this.FSetWriterFailureFromHResult(num);
						return flag;
					}
					flag = true;
					return true;
				}
				catch (Exception e)
				{
					int num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PreRestore");
					flag = this.FSetWriterFailureFromHResult(num);
					flag3 = flag;
				}
				result = (flag3 ? 1 : 0);
			}
			finally
			{
				if (flag2)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PreRestore -- {0}", flag);
			}
			return result != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool PostRestore(IVssWriterComponents* pComponents)
		{
			bool flag = false;
			bool flag2 = false;
			byte result;
			try
			{
				bool flag3;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostRestore");
					<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
					flag2 = true;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong>((long)this.GetHashCode(), "pComponents = 0x{0:X16}", pComponents);
					if (!this.m_fValidObj)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostRestore FValidObj failed. Returning {0:X8}.", -2147212301);
						flag = this.FSetWriterFailureFromHResult(-2147212301);
						return flag;
					}
					int num = this.HrOnRestore(pComponents, 1);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostRestore: HrOnRestore failed with {0:X8}", num);
						flag = this.FSetWriterFailureFromHResult(num);
						return flag;
					}
					flag = true;
					return true;
				}
				catch (Exception e)
				{
					int num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostRestore");
					flag = this.FSetWriterFailureFromHResult(num);
					flag3 = flag;
				}
				result = (flag3 ? 1 : 0);
			}
			finally
			{
				if (flag2)
				{
					<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PostRestore -- {0}", flag);
			}
			return result != 0;
		}

		public unsafe int IdentifyReplica(IVssCreateWriterMetadata* pMetadata, ReplayConfiguration replica)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			string text6 = null;
			string text7 = null;
			string text8 = null;
			string str = null;
			string text9 = null;
			string arg = null;
			string text10 = null;
			string str2 = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::IdentifyReplica");
			int num = 0;
			ushort* ptr = null;
			ushort* ptr2 = null;
			ushort* ptr3 = null;
			ushort* ptr4 = null;
			ushort* ptr5 = null;
			ESE_ICON_DESCRIPTION ese_ICON_DESCRIPTION = new ESE_ICON_DESCRIPTION();
			ESE_ICON_DESCRIPTION ese_ICON_DESCRIPTION2 = new ESE_ICON_DESCRIPTION();
			ESE_ICON_DESCRIPTION ese_ICON_DESCRIPTION3 = new ESE_ICON_DESCRIPTION();
			ESE_ICON_DESCRIPTION ese_ICON_DESCRIPTION4 = new ESE_ICON_DESCRIPTION();
			int result3;
			try
			{
				int num9;
				try
				{
					Guid identityGuid = replica.IdentityGuid;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "replica->Name = {0}, replica->IdentityGuid = {1}", replica.Name, identityGuid);
					text = replica.IdentityGuid.ToString();
					bool databaseIsPrivate;
					int result;
					try
					{
						string nodeNameFromFqdn;
						string format;
						string format2;
						if (ReplayConfigType.RemoteCopyTarget == replica.Type)
						{
							nodeNameFromFqdn = SharedHelper.GetNodeNameFromFqdn(replica.TargetMachine);
							format = "Microsoft Exchange Server\\Microsoft Information Store\\Replica\\{0}";
							format2 = "Microsoft Exchange Server\\Microsoft Information Store\\Replica\\{0}\\{1}";
						}
						else
						{
							object[] array = new object[0];
							if (ReplayConfigType.RemoteCopySource != replica.Type)
							{
								ReplayConfigType type = replica.Type;
							}
							nodeNameFromFqdn = SharedHelper.GetNodeNameFromFqdn(replica.SourceMachine);
							format = "Microsoft Exchange Server\\Microsoft Information Store\\{0}";
							format2 = "Microsoft Exchange Server\\Microsoft Information Store\\{0}\\{1}";
						}
						text2 = string.Format(format, nodeNameFromFqdn);
						text3 = string.Format(format2, nodeNameFromFqdn, text);
						text4 = text;
						text5 = replica.Name;
						object[] args = new object[]
						{
							text2,
							text3,
							text,
							text5,
							replica.Type
						};
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "logicalPathDatabase is '{0}', logicalPathSubcomponents is {1}, logicalNameDatabase is {2}, databaseName is {3}, database type is {4}", args);
						text6 = string.Format("{0}*.{1}", replica.LogFilePrefix, replica.LogExtension);
						text7 = replica.DestinationLogPath;
						string text11 = text7;
						if (text11[text11.Length - 1] != '\\')
						{
							text7 += "\\";
						}
						text8 = string.Format("{0}.chk", replica.LogFilePrefix);
						str = replica.DestinationSystemPath;
						text9 = replica.DestinationEdbPath;
						arg = replica.DatabaseName;
						databaseIsPrivate = replica.DatabaseIsPrivate;
						num = this.LoadIcon(101L, ese_ICON_DESCRIPTION);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "IdentifyReplica failed to load icon for storage group, returned {0:X8}", num);
							return num;
						}
						num = this.LoadIcon(103L, ese_ICON_DESCRIPTION2);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "IdentifyReplica failed to load icon for logs, returned {0:X8}", num);
							return num;
						}
						num = this.LoadIcon(105L, ese_ICON_DESCRIPTION4);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "IdentifyReplica failed to load icon for private database, returned {0:X8}", num);
							return num;
						}
						num = this.LoadIcon(107L, ese_ICON_DESCRIPTION3);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "IdentifyReplica failed to load icon for public database, returned {0:X8}", num);
							return num;
						}
					}
					catch (TransientException ex)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<TransientException>((long)this.GetHashCode(), "Caught an exchange transient exception while reading config for VSS: {0}", ex);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						object[] messageArgs = new object[]
						{
							"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::IdentifyReplica",
							num,
							ex.ToString()
						};
						ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs);
						result = num;
						goto IL_7A9;
					}
					ptr = this.GetUnmanagedWideString(text2);
					ptr2 = this.GetUnmanagedWideString(text4);
					ptr3 = this.GetUnmanagedWideString(text5);
					ref byte byte& = ref ese_ICON_DESCRIPTION.pvData[0];
					try
					{
						num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,VSS_COMPONENT_TYPE,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, 2, ptr, ptr2, ptr3, ref byte&, ese_ICON_DESCRIPTION.ulSize, 1, 1, 1, 1, 0, *(*(long*)pMetadata + 16L));
					}
					catch
					{
						throw;
					}
					if (num < 0)
					{
						object[] array2 = new object[2];
						int num2 = num;
						array2[0] = num2.ToString("X");
						array2[1] = text5;
						ReplayEventLogConstants.Tuple_VSSWriterAddDatabaseComponentError.LogEvent(string.Empty, array2);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "AddComponent for database {0} return {1:X8}", text2, num);
						return num;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "AddComponent for DB '{0}' logical path '{1}' logical name '{2}'", text5, text2, text4);
					int result2;
					try
					{
						text10 = Path.GetFileName(text9);
						str2 = Path.GetDirectoryName(text9);
					}
					catch (ArgumentException ex2)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string>((long)this.GetHashCode(), "splitting the path {0} on edb {1} failed", text9, arg);
						num = <Module>.HRESULT_FROM_WIN32(161);
						object[] messageArgs2 = new object[]
						{
							"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::IdentifyReplica",
							num,
							ex2.ToString()
						};
						ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs2);
						result2 = num;
						goto IL_7A1;
					}
					this.FreeUnmanagedWideString(ptr);
					ptr = this.GetUnmanagedWideString(text3);
					this.FreeUnmanagedWideString(ptr4);
					ptr4 = this.GetUnmanagedWideString(str2);
					this.FreeUnmanagedWideString(ptr5);
					ptr5 = this.GetUnmanagedWideString(text10);
					ESE_ICON_DESCRIPTION ese_ICON_DESCRIPTION5;
					if (databaseIsPrivate)
					{
						ese_ICON_DESCRIPTION5 = ese_ICON_DESCRIPTION4;
					}
					else
					{
						ese_ICON_DESCRIPTION5 = ese_ICON_DESCRIPTION3;
					}
					ref byte byte&2 = ref ese_ICON_DESCRIPTION5.pvData[0];
					try
					{
						num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,VSS_COMPONENT_TYPE,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, 2, ptr, ref <Module>.?A0x8c154cf1.g_wszFileSubComp, ref <Module>.?A0x8c154cf1.g_wszFileSubComp, ref byte&2, ese_ICON_DESCRIPTION5.ulSize, 1, 1, 0, 1, 0, *(*(long*)pMetadata + 16L));
					}
					catch
					{
						throw;
					}
					if (num < 0)
					{
						object[] array3 = new object[2];
						int num3 = num;
						array3[0] = num3.ToString("X");
						array3[1] = text5;
						ReplayEventLogConstants.Tuple_VSSWriterAddDatabaseComponentError.LogEvent(string.Empty, array3);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "AddComponent for EDB {0} returned {1:X8}", text, num);
						return num;
					}
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, ptr, ref <Module>.?A0x8c154cf1.g_wszFileSubComp, ptr4, ptr5, 0, 0L, 3841, *(*(long*)pMetadata + 40L));
					if (num < 0)
					{
						object[] array4 = new object[2];
						int num4 = num;
						array4[0] = num4.ToString("X");
						array4[1] = text10;
						ReplayEventLogConstants.Tuple_VSSWriterDbFileInfoError.LogEvent(string.Empty, array4);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "AddFilesToFileGroup for EDB {0} returned {1:X8}", text, num);
						return num;
					}
					ref byte byte&3 = ref ese_ICON_DESCRIPTION2.pvData[0];
					try
					{
						num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,VSS_COMPONENT_TYPE,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, 2, ptr, ref <Module>.?A0x8c154cf1.g_wszLogsSubComp, ref <Module>.?A0x8c154cf1.g_wszLogsSubComp, ref byte&3, ese_ICON_DESCRIPTION2.ulSize, 1, 1, 0, 1, 0, *(*(long*)pMetadata + 16L));
					}
					catch
					{
						throw;
					}
					if (num < 0)
					{
						object[] array5 = new object[2];
						int num5 = num;
						array5[0] = num5.ToString("X");
						array5[1] = text5;
						ReplayEventLogConstants.Tuple_VSSWriterAddDatabaseComponentError.LogEvent(string.Empty, array5);
						int num6 = num;
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "AddComponent for logs on DB '{0}' returned {1:X8}", text, num6.ToString("X"));
						return num;
					}
					object[] args2 = new object[]
					{
						text5,
						text3,
						"Logs"
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "AddComponent for logs on DB '{0}' logical path '{1}' logical name '{2}'", args2);
					ptr4 = this.GetUnmanagedWideString(text7);
					ptr5 = this.GetUnmanagedWideString(text6);
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, ptr, ref <Module>.?A0x8c154cf1.g_wszLogsSubComp, ptr4, ptr5, 0, 0L, 3855, *(*(long*)pMetadata + 40L));
					if (num < 0)
					{
						object[] array6 = new object[2];
						int num7 = num;
						array6[0] = num7.ToString("X");
						array6[1] = text6;
						ReplayEventLogConstants.Tuple_VSSWriterDbFileInfoError.LogEvent(string.Empty, array6);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "AddFilesToFileGroup for logs for DB {0} returned {1:X8}", text, num);
						return num;
					}
					this.FreeUnmanagedWideString(ptr4);
					this.FreeUnmanagedWideString(ptr5);
					ptr4 = this.GetUnmanagedWideString(str);
					ptr5 = this.GetUnmanagedWideString(text8);
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.Byte modopt(System.Runtime.CompilerServices.CompilerMarshalOverride),System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)), pMetadata, ptr, ref <Module>.?A0x8c154cf1.g_wszLogsSubComp, ptr4, ptr5, 0, 0L, 3841, *(*(long*)pMetadata + 40L));
					if (num < 0)
					{
						object[] array7 = new object[2];
						int num8 = num;
						array7[0] = num8.ToString("X");
						array7[1] = text8;
						ReplayEventLogConstants.Tuple_VSSWriterDbFileInfoError.LogEvent(string.Empty, array7);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "AddFilesToFileGroup for checkpoint for DB {0} returned {1:X8}", text, num);
						return num;
					}
					return num;
					IL_7A1:
					return result2;
					IL_7A9:
					return result;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::IdentifyReplica");
					num9 = num;
				}
				result3 = num9;
			}
			finally
			{
				if (num < 0)
				{
					object[] array8 = new object[1];
					int num10 = num;
					array8[0] = num10.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterAddComponentsError.LogEvent(string.Empty, array8);
				}
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
				IntPtr hglobal2 = new IntPtr((void*)ptr2);
				Marshal.FreeHGlobal(hglobal2);
				IntPtr hglobal3 = new IntPtr((void*)ptr3);
				Marshal.FreeHGlobal(hglobal3);
				IntPtr hglobal4 = new IntPtr((void*)ptr4);
				Marshal.FreeHGlobal(hglobal4);
				IntPtr hglobal5 = new IntPtr((void*)ptr5);
				Marshal.FreeHGlobal(hglobal5);
				ese_ICON_DESCRIPTION.pvData = null;
				ese_ICON_DESCRIPTION2.pvData = null;
				ese_ICON_DESCRIPTION4.pvData = null;
				ese_ICON_DESCRIPTION3.pvData = null;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::IdentifyReplica, hr = {0:X8}", num);
			}
			return result3;
		}

		public unsafe int LoadIcon(ushort* wszIconName, ESE_ICON_DESCRIPTION pIcon)
		{
			new object[0];
			new object[0];
			int result = 0;
			pIcon.pvData = null;
			HINSTANCE__* ptr = <Module>.LoadLibraryExW((ushort*)(&<Module>.??_C@_1GA@HJENNCB@?$AAM?$AAi?$AAc?$AAr?$AAo?$AAs?$AAo?$AAf?$AAt?$AA?4?$AAE?$AAx?$AAc?$AAh?$AAa?$AAn?$AAg?$AAe?$AA?4?$AAC?$AAl?$AAu?$AAs?$AAt?$AAe?$AAr?$AA?4?$AAR?$AAe?$AAp?$AAl?$AAi@), null, 2);
			if (null == ptr)
			{
				result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
			}
			else
			{
				HRSRC__* ptr2 = <Module>.FindResourceW(ptr, (ushort*)wszIconName, 14L);
				if (null == ptr2)
				{
					result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
				}
				else
				{
					void* ptr3 = <Module>.LoadResource(ptr, ptr2);
					if (ptr3 == null)
					{
						result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
					}
					else
					{
						MEMICONDIR* ptr4 = <Module>.LockResource(ptr3);
						if (ptr4 == null)
						{
							result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
						}
						else
						{
							ptr2 = <Module>.FindResourceW(ptr, *(ushort*)(ptr4 + 18L / (long)sizeof(MEMICONDIR)), 3L);
							if (ptr2 == null)
							{
								result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
							}
							else
							{
								uint num = <Module>.SizeofResource(ptr, ptr2);
								if (0 == num)
								{
									result = -939586614;
								}
								else
								{
									try
									{
										pIcon.pvData = new byte[num];
										ptr3 = <Module>.LoadResource(ptr, ptr2);
										if (null != ptr3)
										{
											ref byte byte& = ref pIcon.pvData[0];
											try
											{
												ulong num2 = num;
												<Module>.memcpy_s(ref byte&, num2, <Module>.LockResource(ptr3), num2);
											}
											catch
											{
												throw;
											}
											goto IL_13F;
										}
										result = <Module>.HRESULT_FROM_WIN32(<Module>.GetLastError());
									}
									catch (OutOfMemoryException)
									{
										result = -939523085;
										goto IL_13F;
									}
								}
							}
						}
					}
				}
			}
			if (pIcon.pvData != null)
			{
				pIcon.pvData = null;
			}
			IL_13F:
			if (null != ptr)
			{
				<Module>.FreeLibrary(ptr);
			}
			return result;
		}

		public unsafe int PrepareBackupReplica(IVssComponent* pComponent, ReplayConfiguration replica, BackupInstance backupInstance)
		{
			string text = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackupReplica");
			Guid identityGuid = replica.IdentityGuid;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid, ReplayConfigType>((long)this.GetHashCode(), "replica->Name = {0}, replica->IdentityGuid = {1}, replica->Type = {2}", replica.Name, identityGuid, replica.Type);
			int num = 0;
			bool flag = false;
			_GUID guid2;
			Guid guid = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid2));
			Guid guid3 = default(Guid);
			string arg = replica.IdentityGuid.ToString();
			ushort* ptr = null;
			bool flag2 = false;
			JET_SIGNATURE? value = null;
			JET_SIGNATURE logfileSignature = default(JET_SIGNATURE);
			int result;
			try
			{
				long num2;
				long num3;
				int num7;
				try
				{
					if (replica.CircularLoggingEnabled)
					{
						int vssbackuptype = backupInstance.m_vssbackuptype;
						if (2 == vssbackuptype || 3 == vssbackuptype)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Circular logging in invalid configuration for backup for instance {0}", arg);
							num = <Module>.HRESULT_FROM_WIN32(87);
							return num;
						}
					}
					if (string.IsNullOrEmpty(replica.DatabaseName))
					{
						object[] array = new object[0];
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Empty dbname for copy {0}", arg);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					Guid identityGuid2 = replica.IdentityGuid;
					long num4;
					if (!this.m_replicaInstanceManager.GetRunningInstanceSignatureAndCheckpoint(identityGuid2, out value, out num2, out num3, out num4))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Could not find running instance for {0}", arg);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					if (value == null)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Could not find log file signature for {0}. File checker might not have run yet.", arg);
						object[] array2 = new object[1];
						Guid identityGuid3 = replica.IdentityGuid;
						array2[0] = identityGuid3;
						ReplayEventLogConstants.Tuple_VSSWriterMissingLogFileSignature.LogEvent(string.Empty, array2);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					logfileSignature = (T)value;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}", logfileSignature.ToString());
					JET_LOGTIME rhs = new JET_LOGTIME(0UL);
					if (logfileSignature.ulRandom == 0U || logfileSignature.logtimeCreate == rhs)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Log file signature for {0} is zeroes. File checker might not have run yet.", arg);
						object[] array3 = new object[1];
						Guid identityGuid4 = replica.IdentityGuid;
						array3[0] = identityGuid4;
						ReplayEventLogConstants.Tuple_VSSWriterMissingLogFileSignature.LogEvent(string.Empty, array3);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					flag2 = (((ReplayConfigType.RemoteCopyTarget == replica.Type) ? 1 : 0) != 0);
					if (flag2)
					{
						int vssbackuptype2 = backupInstance.m_vssbackuptype;
						if (vssbackuptype2 == 2 || vssbackuptype2 == 3)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, long, long>((long)this.GetHashCode(), "{0} checking log files from lastGenerationBackedUp+1 gen {1} to lowest required gen {2}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackupReplica", num4 + 1L, num2 - 1L);
							for (long num5 = num4 + 1L; num5 < num2; num5 += 1L)
							{
								string text2 = Path.Combine(replica.DestinationLogPath, EseHelper.MakeLogfileName(replica.LogFilePrefix, "." + replica.LogExtension, num5));
								if (!File.Exists(text2))
								{
									object[] array4 = new object[2];
									Guid identityGuid5 = replica.IdentityGuid;
									array4[0] = identityGuid5;
									array4[1] = text2;
									ReplayEventLogConstants.Tuple_VSSWriterMissingFile.LogEvent(string.Empty, array4);
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "file {0} needed for incremental/differential backup was not found", text2);
									num = <Module>.HRESULT_FROM_WIN32(2);
									return num;
								}
							}
						}
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, long, long>((long)this.GetHashCode(), "{0} checking log files from gen {1} to  gen {2} and also check point files", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackupReplica", num2, num3);
						for (long num6 = num2; num6 < num3 + 1L; num6 += 1L)
						{
							string text3 = Path.Combine(replica.DestinationLogPath, EseHelper.MakeLogfileName(replica.LogFilePrefix, "." + replica.LogExtension, num6));
							if (!File.Exists(text3))
							{
								object[] array5 = new object[2];
								Guid identityGuid6 = replica.IdentityGuid;
								array5[0] = identityGuid6;
								array5[1] = text3;
								ReplayEventLogConstants.Tuple_VSSWriterMissingFile.LogEvent(string.Empty, array5);
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "file {0} was not found", text3);
								num = <Module>.HRESULT_FROM_WIN32(2);
								return num;
							}
						}
						string text4 = Path.Combine(replica.DestinationSystemPath, EseHelper.MakeCheckpointFileName(replica.LogFilePrefix));
						if (!File.Exists(text4))
						{
							object[] array6 = new object[2];
							Guid identityGuid7 = replica.IdentityGuid;
							array6[0] = identityGuid7;
							array6[1] = text4;
							ReplayEventLogConstants.Tuple_VSSWriterMissingFile.LogEvent(string.Empty, array6);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "file {0} was not found", text4);
							num = <Module>.HRESULT_FROM_WIN32(2);
							return num;
						}
					}
					text = replica.GetXmlDescription(logfileSignature);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), text);
				}
				catch (TransientException ex)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Could not find running instance for {0}", arg);
					num = <Module>.HRESULT_FROM_WIN32(21);
					object[] messageArgs = new object[]
					{
						"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareBackupReplica",
						num,
						ex.ToString()
					};
					ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs);
					num7 = num;
					goto IL_695;
				}
				ptr = this.GetUnmanagedWideString(text);
				num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16 modopt(System.Runtime.CompilerServices.IsConst)*), pComponent, ptr, *(*(long*)pComponent + 72L));
				if (num < 0)
				{
					object[] array7 = new object[2];
					array7[0] = guid;
					int num8 = num;
					array7[1] = num8.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterSetPrivateMetadataError.LogEvent(string.Empty, array7);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "SetBackupMetadata fails with hr: {0}", num);
					return num;
				}
				bool flag3 = this.FVssIdCurrentSnapshotSetIdFromReplica(replica, ref guid3);
				if (flag3 && backupInstance.m_vssIdCurrentSnapshotSetId != guid3)
				{
					if (!this.m_backupInstances[guid3].m_fBackupPrepared)
					{
						num = <Module>.HRESULT_FROM_WIN32(32);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid, Guid>((long)this.GetHashCode(), "Storage Group already being backed up by another backup instance, current backup instance is {0} owner instance is {1}", backupInstance.m_vssIdCurrentSnapshotSetId, guid3);
						return num;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Found an orphaned backup instance {0}", guid3);
					object[] messageArgs2 = new object[]
					{
						guid3
					};
					ReplayEventLogConstants.Tuple_VSSWriterOrphanedBackupInstance.LogEvent(string.Empty, messageArgs2);
					this.CleanupBackup(guid3);
				}
				string logExtension = string.Format(".{0}", replica.LogExtension);
				string nodeNameFromFqdn = SharedHelper.GetNodeNameFromFqdn(replica.SourceMachine);
				Guid identityGuid8 = replica.IdentityGuid;
				StorageGroupBackup item = new StorageGroupBackup(nodeNameFromFqdn, identityGuid8, logfileSignature, num2, num3, replica.DestinationLogPath, replica.LogFilePrefix, logExtension, flag2);
				backupInstance.m_StorageGroupBackups.Add(item);
				flag = true;
				return num;
				IL_695:
				result = num7;
			}
			finally
			{
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "leave CReplicaVssWriterInterop::PrepareBackupReplica, hr = {0:X8}", num);
				if (flag)
				{
					int vssbackuptype3 = backupInstance.m_vssbackuptype;
					if (vssbackuptype3 != 1 && vssbackuptype3 != 5)
					{
						object[] messageArgs3 = new object[]
						{
							guid,
							replica.Name,
							replica.DatabaseName
						};
						ReplayEventLogConstants.Tuple_VSSWriterBackupDatabaseIncrementalDifferential.LogEvent(string.Empty, messageArgs3);
					}
					else
					{
						object[] messageArgs4 = new object[]
						{
							guid,
							replica.Name,
							replica.DatabaseName
						};
						ReplayEventLogConstants.Tuple_VSSWriterBackupDatabaseFullCopy.LogEvent(string.Empty, messageArgs4);
					}
				}
				else
				{
					object[] array8 = new object[3];
					array8[0] = guid;
					int num9 = num;
					array8[1] = num9.ToString("X");
					array8[2] = replica.Name;
					ReplayEventLogConstants.Tuple_VSSWriterBackupDatabaseError.LogEvent(string.Empty, array8);
				}
			}
			return result;
		}

		public unsafe int PrepareSnapshotComponents()
		{
			ReplayConfiguration replayConfiguration = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotComponents");
			int num = 0;
			bool flag = false;
			_GUID guid2;
			Guid guid = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid2));
			Guid guid3 = guid;
			Guid guid4 = default(Guid);
			new object[0];
			this.m_backupInstances.ContainsKey(guid);
			int result;
			try
			{
				List<ReplayConfiguration> list = this.FindAllDatabaseConfigurations();
				List<StorageGroupBackup>.Enumerator enumerator = this.m_backupInstances[guid].m_StorageGroupBackups.GetEnumerator();
				IL_76:
				while (enumerator.MoveNext())
				{
					StorageGroupBackup storageGroupBackup = enumerator.Current;
					bool flag2 = false;
					List<ReplayConfiguration>.Enumerator enumerator2 = list.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						replayConfiguration = enumerator2.Current;
						if (replayConfiguration.IdentityGuid == storageGroupBackup.m_guidSGIdentityGuid)
						{
							IL_F6:
							num = this.PrepareSnapshotReplica(replayConfiguration);
							if (num < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareSnapshotReplica failed with {0:X8}", num);
								return num;
							}
							goto IL_76;
						}
					}
					if (!flag2)
					{
						num = <Module>.HRESULT_FROM_WIN32(1610);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "PrepareSnapshotComponents component not found");
						return num;
					}
					goto IL_F6;
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					object[] array = new object[2];
					array[0] = guid3;
					int num2 = num;
					array[1] = num2.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterSnapshotError.LogEvent(string.Empty, array);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotComponents, hr = {0:X8}", num);
			}
			return result;
		}

		public unsafe int PrepareSnapshotVolume()
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotVolume");
			int num = 0;
			bool flag = false;
			_GUID guid2;
			Guid guid = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid2));
			Guid guid3 = guid;
			new object[0];
			this.m_backupInstances.ContainsKey(guid);
			new object[0];
			int count = this.m_backupInstances[guid].m_StorageGroupBackups.Count;
			int result;
			try
			{
				foreach (ReplayConfiguration replica in this.FindAllDatabaseConfigurations())
				{
					num = this.PrepareSnapshotVolumeReplica(replica);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "PrepareSnapshotVolume (Remote Copy) failed with {0:X8}", num);
						return num;
					}
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					object[] array = new object[2];
					array[0] = guid3;
					int num2 = num;
					array[1] = num2.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterCheckInstanceVolumeDependenciesError.LogEvent(string.Empty, array);
					object[] array2 = new object[2];
					array2[0] = guid3;
					int num3 = num;
					array2[1] = num3.ToString("X");
					ReplayEventLogConstants.Tuple_VSSWriterSnapshotError.LogEvent(string.Empty, array2);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotVolume, hr = {0:X8}", num);
			}
			return result;
		}

		public unsafe int PrepareSnapshotReplica(ReplayConfiguration replica)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotReplica");
			Guid identityGuid = replica.IdentityGuid;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid, ReplayConfigType>((long)this.GetHashCode(), "replica->Name = {0}, replica->IdentityGuid = {1}, replica->Type {2}", replica.Name, identityGuid, replica.Type);
			int num = 0;
			bool flag = false;
			_GUID guid2;
			Guid guid = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid2));
			Guid guid3 = guid;
			string text = replica.IdentityGuid.ToString();
			ushort* ptr = null;
			int result;
			try
			{
				int num2;
				try
				{
					Guid identityGuid2 = replica.IdentityGuid;
					CopyStatusEnum copyStatus = this.m_replicaInstanceManager.GetCopyStatus(identityGuid2);
					if (!((ReplayConfigType.RemoteCopyTarget != replica.Type) ? (CopyStatusEnum.Mounted == copyStatus) : (CopyStatusEnum.Healthy == copyStatus)))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, int>((long)this.GetHashCode(), "PrepareSnapshotReplica failing because replica {0} is in state '{1}'.", text, (int)copyStatus);
						object[] messageArgs = new object[]
						{
							replica.Name,
							((CopyStatusEnum)copyStatus).ToString()
						};
						ReplayEventLogConstants.Tuple_VSSReplicaCopyUnhealthy.LogEvent(string.Empty, messageArgs);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					string destinationLogPath = replica.DestinationLogPath;
					string destinationSystemPath = replica.DestinationSystemPath;
					string destinationEdbPath = replica.DestinationEdbPath;
					ptr = this.GetUnmanagedWideString(destinationLogPath);
					if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) == null)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "PrepareSnapshotReplica found log file path {0} on SG {1} not affected", destinationLogPath, text);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						return num;
					}
					int vssbackuptype = this.m_backupInstances[guid].m_vssbackuptype;
					if (2 != vssbackuptype && 3 != vssbackuptype)
					{
						this.FreeUnmanagedWideString(ptr);
						ptr = this.GetUnmanagedWideString(destinationSystemPath);
						if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) == null)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "PrepareSnapshotReplica found checkpoint file path {0} on SG {1} not affected", destinationSystemPath, text);
							num = <Module>.HRESULT_FROM_WIN32(1610);
							return num;
						}
						this.FreeUnmanagedWideString(ptr);
						ptr = this.GetUnmanagedWideString(destinationEdbPath);
						if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) == null)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "PrepareSnapshotReplica found EDB path {0} on SG {1} not affected", destinationEdbPath, text);
							num = <Module>.HRESULT_FROM_WIN32(1610);
							return num;
						}
					}
					num = 0;
					flag = true;
					return 0;
				}
				catch (TransientException ex)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<TransientException>((long)this.GetHashCode(), "Caught an exchange transient exception while reading config for VSS: {0}", ex);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					object[] messageArgs2 = new object[]
					{
						"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotReplica",
						num,
						ex.ToString()
					};
					ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs2);
					num2 = num;
				}
				result = num2;
			}
			finally
			{
				if (!flag)
				{
					object[] array = new object[3];
					array[0] = guid3;
					int num3 = num;
					array[1] = num3.ToString("X");
					array[2] = replica.Name;
					ReplayEventLogConstants.Tuple_VSSWriterCheckDatabaseVolumeDependenciesError.LogEvent(string.Empty, array);
				}
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotReplica, hr = {0:X8}", num);
			}
			return result;
		}

		public unsafe int PrepareSnapshotVolumeReplica(ReplayConfiguration replica)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "enter CReplicaVssWriterInterop::PrepareSnapshotVolumeReplica");
			Guid identityGuid = replica.IdentityGuid;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "replica->Name = {0}, replica->IdentityGuid = {1}", replica.Name, identityGuid);
			int num = 0;
			_GUID guid2;
			Guid guid = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid2));
			string arg = replica.IdentityGuid.ToString();
			bool flag = ((ReplayConfigType.RemoteCopyTarget == replica.Type) ? 1 : 0) != 0;
			Guid b = default(Guid);
			ushort* ptr = null;
			new object[0];
			this.m_backupInstances.ContainsKey(guid);
			int result;
			try
			{
				int num2;
				try
				{
					Guid identityGuid2 = replica.IdentityGuid;
					if (this.m_replicaInstanceManager.GetStatusRetrieverCallback(identityGuid2) == null)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "PrepareSnapshotVolumeReplica failing because replica {0} is not running", arg);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					Guid identityGuid3 = replica.IdentityGuid;
					CopyStatusEnum copyStatus = this.m_replicaInstanceManager.GetCopyStatus(identityGuid3);
					if (!((!flag) ? (CopyStatusEnum.Mounted == copyStatus) : (CopyStatusEnum.Healthy == copyStatus)))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, int>((long)this.GetHashCode(), "PrepareSnapshotVolumeReplica failing because replica {0} is in state '{1}'.", arg, (int)copyStatus);
						object[] messageArgs = new object[]
						{
							replica.Name,
							((CopyStatusEnum)copyStatus).ToString()
						};
						ReplayEventLogConstants.Tuple_VSSReplicaCopyUnhealthy.LogEvent(string.Empty, messageArgs);
						num = <Module>.HRESULT_FROM_WIN32(21);
						return num;
					}
					string destinationLogPath = replica.DestinationLogPath;
					string destinationSystemPath = replica.DestinationSystemPath;
					string destinationEdbPath = replica.DestinationEdbPath;
					ptr = this.GetUnmanagedWideString(destinationLogPath);
					string nodeNameFromFqdn = SharedHelper.GetNodeNameFromFqdn(replica.SourceMachine);
					if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) != null)
					{
						if (!this.FVssIdCurrentSnapshotSetIdFromReplica(replica, ref b))
						{
							Guid identityGuid4 = replica.IdentityGuid;
							StorageGroupBackup item = new StorageGroupBackup(nodeNameFromFqdn, identityGuid4, flag);
							this.m_backupInstances[guid].m_StorageGroupBackups.Add(item);
						}
						else if (guid != b)
						{
							num = <Module>.HRESULT_FROM_WIN32(32);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Storage Group already being backed up by another backup instance. Failed with {0:X8}", num);
						}
						return num;
					}
					this.FreeUnmanagedWideString(ptr);
					ptr = this.GetUnmanagedWideString(destinationSystemPath);
					if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) != null)
					{
						if (!this.FVssIdCurrentSnapshotSetIdFromReplica(replica, ref b))
						{
							Guid identityGuid5 = replica.IdentityGuid;
							StorageGroupBackup item2 = new StorageGroupBackup(nodeNameFromFqdn, identityGuid5, flag);
							this.m_backupInstances[guid].m_StorageGroupBackups.Add(item2);
						}
						else if (guid != b)
						{
							num = <Module>.HRESULT_FROM_WIN32(32);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Storage Group already being backed up by another backup instance. Failed with {0:X8}", num);
						}
						return num;
					}
					this.FreeUnmanagedWideString(ptr);
					ptr = this.GetUnmanagedWideString(destinationEdbPath);
					if (<Module>.CReplicaVssWriter.IsPathAffected(this.m_replicaWriter, ptr) != null)
					{
						if (!this.FVssIdCurrentSnapshotSetIdFromReplica(replica, ref b))
						{
							Guid identityGuid6 = replica.IdentityGuid;
							StorageGroupBackup item3 = new StorageGroupBackup(nodeNameFromFqdn, identityGuid6, flag);
							this.m_backupInstances[guid].m_StorageGroupBackups.Add(item3);
						}
						else if (guid != b)
						{
							num = <Module>.HRESULT_FROM_WIN32(32);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Storage Group already being backed up by another backup instance. Failed with {0:X8}", num);
						}
						return num;
					}
					num = 0;
					return 0;
				}
				catch (TransientException ex)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<TransientException>((long)this.GetHashCode(), "Caught an exchange transient exception while reading config for VSS: {0}", ex);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					object[] messageArgs2 = new object[]
					{
						"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotVolumeReplica",
						num,
						ex.ToString()
					};
					ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs2);
					num2 = num;
				}
				result = num2;
			}
			finally
			{
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::PrepareSnapshotVolumeReplica, hr = {0:X8}", num);
			}
			return result;
		}

		public unsafe int HrBeginSurrogateBackupsOnPrimarySGs()
		{
			int num = 0;
			_GUID guid;
			Guid key = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid));
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "enter {0}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrBeginSurrogateBackupsOnPrimarySGs");
			List<StorageGroupBackup>.Enumerator enumerator = this.m_backupInstances[key].m_StorageGroupBackups.GetEnumerator();
			if (enumerator.MoveNext())
			{
				do
				{
					StorageGroupBackup storageGroupBackup = enumerator.Current;
					if (storageGroupBackup.m_fIsPassive)
					{
						num = this.HrBeginSurrogateBackupOnPrimarySG(storageGroupBackup);
						if (num < 0)
						{
							break;
						}
					}
				}
				while (enumerator.MoveNext());
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, int>((long)this.GetHashCode(), "leave {0} -- {1:x}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrBeginSurrogateBackupsOnPrimarySGs", num);
			return num;
		}

		public unsafe int HrBeginSurrogateBackupOnPrimarySG(StorageGroupBackup sgb)
		{
			int num = 0;
			_GUID guid;
			Guid key = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid));
			ushort* ptr = null;
			ushort* ptr2 = null;
			void* hccx = null;
			int vssbackuptype = this.m_backupInstances[key].m_vssbackuptype;
			uint btBackupType = 48U;
			if (vssbackuptype != 1)
			{
				if (vssbackuptype != 2)
				{
					if (vssbackuptype != 3)
					{
						if (vssbackuptype != 5)
						{
							new object[0];
						}
						else
						{
							btBackupType = 51U;
						}
					}
					else
					{
						btBackupType = 50U;
					}
				}
				else
				{
					btBackupType = 49U;
				}
			}
			else
			{
				btBackupType = 48U;
			}
			NATIVE_SIGNATURE native_SIGNATURE = default(NATIVE_SIGNATURE);
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "enter {0}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrBeginSurrogateBackupOnPrimarySG");
			new object[0];
			new object[0];
			if (sgb.m_fComponentBackup)
			{
				ptr = (ushort*)Marshal.StringToHGlobalUni(sgb.m_serverName).ToPointer();
				ptr2 = (ushort*)Marshal.StringToHGlobalUni(sgb.m_guidSGIdentityGuid.ToString()).ToPointer();
				native_SIGNATURE = sgb.m_logfileSignature.GetNativeSignature();
				object[] array = new object[6];
				array[0] = sgb.m_serverName;
				array[1] = sgb.m_guidSGIdentityGuid.ToString();
				array[2] = native_SIGNATURE.ulRandom;
				DateTime? dateTime = native_SIGNATURE.logtimeCreate.ToDateTime();
				array[3] = dateTime;
				array[4] = native_SIGNATURE.szComputerName;
				array[5] = sgb.m_logfileSignature.ToString();
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "wszActiveServer {0}, wszAnnotation {1}, NATIVE_SIGNATURE({2},{3},{4}) {5}", array);
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(native_SIGNATURE));
				IntPtr hglobal = intPtr;
				try
				{
					Marshal.StructureToPtr(native_SIGNATURE, intPtr, false);
					num = <Module>.HrESESurrogateBackupBegin(ptr, ptr2, (void*)intPtr, btBackupType, (int)sgb.m_lowestGenerationRequired, (int)sgb.m_highestGenerationRequired, &hccx);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid, int>((long)this.GetHashCode(), "HrESESurrogateBackupBegin of {0} failed with {1:x}", sgb.m_guidSGIdentityGuid, num);
						goto IL_1F7;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(hglobal);
				}
				sgb.m_hccx = hccx;
				sgb.m_fSurrogateBackupBegun = true;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "set m_fSurrogateBackupBegun to {0}", true);
			}
			IL_1F7:
			IntPtr hglobal2 = new IntPtr((void*)ptr);
			Marshal.FreeHGlobal(hglobal2);
			IntPtr hglobal3 = new IntPtr((void*)ptr2);
			Marshal.FreeHGlobal(hglobal3);
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, int>((long)this.GetHashCode(), "leave {0} -- {1:x}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrBeginSurrogateBackupOnPrimarySG", num);
			return num;
		}

		public unsafe int HrEndSurrogateBackupOnPrimarySG(StorageGroupBackup sgb, int hrBackupResult)
		{
			int num = 0;
			if (sgb.m_fIsPassive && sgb.m_fSurrogateBackupBegun)
			{
				void* hccx = sgb.m_hccx;
				new object[0];
				num = <Module>.HrESESurrogateBackupEnd(hrBackupResult, &hccx);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "CReplicaVssWriterInterop::HrEndSurrogateBackupOnPrimarySG returned hr = {0:X8}", num);
				sgb.m_fSurrogateBackupBegun = false;
				sgb.m_hccx = null;
			}
			return num;
		}

		public unsafe int FreezeOrThawReplicas([MarshalAs(UnmanagedType.U1)] bool fFreeze)
		{
			object[] array = new object[2];
			array[0] = "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThawReplicas";
			string text;
			if (fFreeze)
			{
				text = "Freeze";
			}
			else
			{
				text = "Thaw";
			}
			array[1] = text;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "enter {0} to {1}", array);
			int num = 0;
			_GUID guid;
			Guid key = this.GuidFromUnmanagedGuid(*<Module>.CReplicaVssWriter.GetCurrentSnapshotSetId(this.m_replicaWriter, &guid));
			new object[0];
			this.m_backupInstances.ContainsKey(key);
			List<StorageGroupBackup>.Enumerator enumerator = this.m_backupInstances[key].m_StorageGroupBackups.GetEnumerator();
			if (enumerator.MoveNext())
			{
				do
				{
					StorageGroupBackup storageGroupBackup = enumerator.Current;
					if (fFreeze != storageGroupBackup.m_fFrozen)
					{
						if (storageGroupBackup.m_fIsPassive)
						{
							num = this.FreezeOrThawReplica(storageGroupBackup, fFreeze);
							if (num < 0)
							{
								goto IL_140;
							}
						}
						else if (fFreeze)
						{
							CReplicaVssWriterInterop.SnapshotFreeze(storageGroupBackup.m_guidSGIdentityGuid, 0U);
						}
						else
						{
							CReplicaVssWriterInterop.SnapshotThaw(storageGroupBackup.m_guidSGIdentityGuid, 0U);
						}
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "set m_fFrozen from {0} to {1}", storageGroupBackup.m_fFrozen, fFreeze);
						storageGroupBackup.m_fFrozen = fFreeze;
					}
					else
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "Instance {0} is already in state of {1}, so skip FreezeOrThaw", storageGroupBackup.m_guidSGIdentityGuid, fFreeze);
					}
				}
				while (enumerator.MoveNext());
				goto IL_1CC;
				IL_140:
				if (fFreeze)
				{
					List<StorageGroupBackup>.Enumerator enumerator2 = this.m_backupInstances[key].m_StorageGroupBackups.GetEnumerator();
					if (enumerator2.MoveNext())
					{
						do
						{
							StorageGroupBackup storageGroupBackup2 = enumerator2.Current;
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "Thaw instance {0} .", storageGroupBackup2.m_guidSGIdentityGuid, fFreeze);
							if (storageGroupBackup2.m_fIsPassive)
							{
								this.FreezeOrThawReplica(storageGroupBackup2, false);
							}
							else
							{
								CReplicaVssWriterInterop.SnapshotThaw(storageGroupBackup2.m_guidSGIdentityGuid, 0U);
							}
						}
						while (enumerator2.MoveNext());
					}
				}
				(new object[1])[0] = num;
			}
			IL_1CC:
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, int>((long)this.GetHashCode(), "leave {0} -- {1:x}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThawReplicas", num);
			return num;
		}

		public int FreezeOrThawReplica(StorageGroupBackup sgb, [MarshalAs(UnmanagedType.U1)] bool fFreeze)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThawReplica");
			int num = 0;
			Guid guidSGIdentityGuid = sgb.m_guidSGIdentityGuid;
			ReplayState runningInstanceState = this.m_replicaInstanceManager.GetRunningInstanceState(guidSGIdentityGuid);
			if (runningInstanceState == null)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Could not find running instance for {0}", guidSGIdentityGuid);
				num = -2147212301;
			}
			else
			{
				StateLock suspendLock = runningInstanceState.SuspendLock;
				if (fFreeze)
				{
					if (!suspendLock.TryEnter(LockOwner.Backup, true))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup could not get the suspendLock for {0}", guidSGIdentityGuid);
						num = -2147212301;
					}
					else
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup got the suspendLock for {0}", guidSGIdentityGuid);
						if (this.m_replicaInstanceManager.CreateTempLogFileForRunningInstance(guidSGIdentityGuid))
						{
							goto IL_141;
						}
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup could not create temp log file for {0}", guidSGIdentityGuid);
						suspendLock.Leave(LockOwner.Backup);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup released the lock for {0} because we failed to freeze", guidSGIdentityGuid);
						num = -2147212303;
					}
				}
				else
				{
					if (suspendLock.CurrentOwner == LockOwner.Backup)
					{
						suspendLock.Leave(LockOwner.Backup);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup released the lock for {0} after thaw", guidSGIdentityGuid);
						goto IL_141;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<Guid>((long)this.GetHashCode(), "Backup doesn't currently own the lock for {0} so thaw is not releasing it", guidSGIdentityGuid);
					goto IL_141;
				}
			}
			<Module>.CReplicaVssWriter.SetWriterFailure(this.m_replicaWriter, num);
			IL_141:
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FreezeOrThawReplica -- {0:x}", num);
			return num;
		}

		public unsafe int VssIdCurrentSnapshotSetIdFromComponents(IVssWriterComponents* pComponents, ref Guid vssIdCurrentSnapshotSetId)
		{
			int num = 0;
			uint num2 = 0U;
			ushort* ptr = null;
			ushort* ptr2 = null;
			bool flag = false;
			int result4;
			try
			{
				int num7;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::VssIdCurrentSnapshotSetIdFromComponents");
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong>((long)this.GetHashCode(), "pComponents = 0x{0:X16}", pComponents);
					List<ReplayConfiguration> list = this.FindAllDatabaseConfigurations();
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponents, ref num2, *(*(long*)pComponents));
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentCount failed with {0:X8}", num);
						return num;
					}
					uint num3 = 0;
					while (num3 < num2)
					{
						CComPtr<IVssComponent> ccomPtr<IVssComponent>;
						<Module>.ATL.CComPtr<IVssComponent>.{ctor}(ref ccomPtr<IVssComponent>);
						try
						{
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssComponent**), pComponents, num3, <Module>.ATL.CComPtrBase<IVssComponent>.&(ref ccomPtr<IVssComponent>), *(*(long*)pComponents + 16L));
							if (num >= 0)
							{
								goto IL_D8;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponent failed with {0:X8}", num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_CA;
						IL_D8:
						try
						{
							if (0L != <Module>.ATL.CComPtrBase<IVssComponent>..PEAVIVssComponent@@(ref ccomPtr<IVssComponent>))
							{
								goto IL_129;
							}
							num = <Module>.HRESULT_FROM_WIN32(1610);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponent failure (returned NULL). Failed with {0:X8}.", num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_11B;
						IL_129:
						int result;
						int result2;
						int result3;
						try
						{
							object[] array = new object[0];
							try
							{
								_NoAddRefReleaseOnCComPtr<IVssComponent>* ptr3 = <Module>.ATL.CComPtrBase<IVssComponent>.->(ref ccomPtr<IVssComponent>);
								num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr3, ref ptr, *(*(long*)ptr3 + 40L));
								if (num < 0)
								{
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentName failed with {0:X8}", num);
									result = num;
									goto IL_2E6;
								}
								if (null == ptr)
								{
									num = <Module>.HRESULT_FROM_WIN32(1610);
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentName failure (returned NULL). Failed with {0:X8}.", num);
									result2 = num;
									goto IL_2DA;
								}
								flag = false;
								foreach (ReplayConfiguration replayConfiguration in list)
								{
									object[] array2 = new object[0];
									try
									{
										ptr2 = this.GetUnmanagedWideString(replayConfiguration.IdentityGuid.ToString());
										ushort* ptr4 = ptr;
										ushort* ptr5 = ptr2;
										int num4 = 0;
										for (;;)
										{
											short num5 = *(short*)ptr5;
											short num6 = *(short*)ptr4;
											if (num5 < num6 || num5 > num6)
											{
												goto IL_256;
											}
											if (num5 == 0)
											{
												break;
											}
											ptr5 += 2L / 2L;
											ptr4 += 2L / 2L;
										}
										if (0 == num4)
										{
											if (!this.FVssIdCurrentSnapshotSetIdFromReplica(replayConfiguration, ref vssIdCurrentSnapshotSetId))
											{
												num = <Module>.HRESULT_FROM_WIN32(1610);
												ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "FVssIdCurrentSnapshotSetIdFromReplica failed with {0:X8}.", num);
												result3 = num;
												goto IL_268;
											}
											flag = true;
											break;
										}
										IL_256:;
									}
									finally
									{
										this.FreeUnmanagedWideString(ptr2);
										ptr2 = null;
									}
									continue;
									IL_268:
									goto IL_2CE;
								}
							}
							finally
							{
								<Module>.SysFreeString(ptr);
								ptr = null;
							}
							if (flag)
							{
								goto IL_2BE;
							}
							num = <Module>.HRESULT_FROM_WIN32(1610);
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::VssIdCurrentSnapshotSetIdFromComponents failed with {0:X8} because a component was not found.", num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_2B3;
						IL_2BE:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						num3++;
						continue;
						IL_2B3:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
						IL_2CE:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result3;
						IL_2DA:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result2;
						IL_2E6:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result;
						IL_11B:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
						IL_CA:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
					}
					return num;
				}
				catch (Exception e)
				{
					num = this.HandleException(e, "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::VssIdCurrentSnapshotSetIdFromComponents");
					num7 = num;
				}
				result4 = num7;
			}
			finally
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::VssIdCurrentSnapshotSetIdFromComponents -- {0:x}", num);
			}
			return result4;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool FVssIdCurrentSnapshotSetIdFromReplica(ReplayConfiguration replica, ref Guid vssIdCurrentSnapshotSetId)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FVssIdCurrentSnapshotSetIdFromReplica");
			bool flag = false;
			Dictionary<Guid, BackupInstance>.ValueCollection.Enumerator enumerator = this.m_backupInstances.Values.GetEnumerator();
			if (enumerator.MoveNext())
			{
				BackupInstance backupInstance;
				for (;;)
				{
					backupInstance = enumerator.Current;
					List<StorageGroupBackup>.Enumerator enumerator2 = backupInstance.m_StorageGroupBackups.GetEnumerator();
					if (enumerator2.MoveNext())
					{
						do
						{
							StorageGroupBackup storageGroupBackup = enumerator2.Current;
							Guid identityGuid = replica.IdentityGuid;
							if (storageGroupBackup.m_guidSGIdentityGuid == identityGuid)
							{
								goto IL_88;
							}
						}
						while (enumerator2.MoveNext());
					}
					if (!enumerator.MoveNext())
					{
						goto IL_BF;
					}
				}
				IL_88:
				vssIdCurrentSnapshotSetId = backupInstance.m_vssIdCurrentSnapshotSetId;
				Guid identityGuid2 = replica.IdentityGuid;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid, Guid>((long)this.GetHashCode(), "found instance {0} contains sg {1}", backupInstance.m_vssIdCurrentSnapshotSetId, identityGuid2);
				flag = true;
			}
			IL_BF:
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<bool>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::FVssIdCurrentSnapshotSetIdFromReplica -- {0}", flag);
			return flag;
		}

		public unsafe int BackupCompleteTruncateLogsComponents(IVssWriterComponents* pComponents, Guid vssIdCurrentSnapshotSetId, [MarshalAs(UnmanagedType.U1)] bool fTruncateLogs)
		{
			List<ReplayConfiguration> list = null;
			int num = 0;
			uint num2 = 0U;
			uint num3 = 0;
			bool flag = false;
			ushort* ptr = null;
			ushort* ptr2 = null;
			bool flag2 = false;
			int result5;
			try
			{
				int num7;
				try
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsComponents with {0}", vssIdCurrentSnapshotSetId);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<ulong, Guid, bool>((long)this.GetHashCode(), "pComponents = 0x{0:X16}, vssIdCurrentSnapshotSetId = {1}, fTruncateLogs = {2}", pComponents, vssIdCurrentSnapshotSetId, fTruncateLogs);
					list = this.FindAllDatabaseConfigurations();
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponents, ref num2, *(*(long*)pComponents));
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentCount failed with {0:X8}", num);
						return num;
					}
					num3 = 0;
					while (num3 < num2)
					{
						CComPtr<IVssComponent> ccomPtr<IVssComponent>;
						<Module>.ATL.CComPtr<IVssComponent>.{ctor}(ref ccomPtr<IVssComponent>);
						try
						{
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssComponent**), pComponents, num3, <Module>.ATL.CComPtrBase<IVssComponent>.&(ref ccomPtr<IVssComponent>), *(*(long*)pComponents + 16L));
							if (num >= 0)
							{
								goto IL_F2;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponent failed with {0:X8}", num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_E4;
						IL_F2:
						try
						{
							if (0L != <Module>.ATL.CComPtrBase<IVssComponent>..PEAVIVssComponent@@(ref ccomPtr<IVssComponent>))
							{
								goto IL_142;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "GetComponent failure (returned NULL)");
							num = <Module>.HRESULT_FROM_WIN32(1610);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_134;
						IL_142:
						try
						{
							_NoAddRefReleaseOnCComPtr<IVssComponent>* ptr3 = <Module>.ATL.CComPtrBase<IVssComponent>.->(ref ccomPtr<IVssComponent>);
							num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.Boolean*), ptr3, ref flag, *(*(long*)ptr3 + 48L));
							if (num >= 0)
							{
								goto IL_19C;
							}
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "BackupCompleteTruncateLogs failed with {0:X8}", num);
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_18E;
						IL_19C:
						int result;
						int result2;
						int result3;
						int result4;
						try
						{
							object[] array = new object[0];
							try
							{
								_NoAddRefReleaseOnCComPtr<IVssComponent>* ptr4 = <Module>.ATL.CComPtrBase<IVssComponent>.->(ref ccomPtr<IVssComponent>);
								num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr4, ref ptr, *(*(long*)ptr4 + 40L));
								if (num < 0)
								{
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "GetComponentName failed with {0:X8}", num);
									result = num;
									goto IL_3AC;
								}
								if (null == ptr)
								{
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "GetComponentName failure (returned NULL)");
									num = <Module>.HRESULT_FROM_WIN32(1610);
									result2 = num;
									goto IL_39D;
								}
								if (!flag)
								{
									string text = new string((char*)ptr);
									object[] messageArgs = new object[]
									{
										vssIdCurrentSnapshotSetId,
										text
									};
									ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteFailureWarning.LogEvent(string.Empty, messageArgs);
									flag2 = true;
									goto IL_38D;
								}
								bool flag3 = false;
								foreach (ReplayConfiguration replayConfiguration in list)
								{
									this.FreeUnmanagedWideString(ptr2);
									ptr2 = this.GetUnmanagedWideString(replayConfiguration.IdentityGuid.ToString());
									ushort* ptr5 = ptr;
									ushort* ptr6 = ptr2;
									int num4 = 0;
									for (;;)
									{
										short num5 = *(short*)ptr6;
										short num6 = *(short*)ptr5;
										if (num5 < num6 || num5 > num6)
										{
											break;
										}
										if (num5 == 0)
										{
											goto IL_2C9;
										}
										ptr6 += 2L / 2L;
										ptr5 += 2L / 2L;
									}
									continue;
									IL_2C9:
									if (0 == num4)
									{
										num = this.BackupCompleteTruncateLogsReplica(replayConfiguration, vssIdCurrentSnapshotSetId, fTruncateLogs);
										if (num < 0)
										{
											result3 = num;
											goto IL_37E;
										}
										goto IL_366;
									}
								}
								if (!flag3)
								{
									string text2 = new string((char*)ptr);
									object[] messageArgs2 = new object[]
									{
										vssIdCurrentSnapshotSetId,
										text2
									};
									ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteUnknownGuid.LogEvent(string.Empty, messageArgs2);
									ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "BackupCompleteTruncateLogs failed because a component was not found");
									num = <Module>.HRESULT_FROM_WIN32(1610);
									result4 = num;
									goto IL_36F;
								}
							}
							finally
							{
								<Module>.SysFreeString(ptr);
								ptr = null;
							}
						}
						catch
						{
							<Module>.___CxxCallUnwindDtor(ldftn(ATL.CComPtr<IVssComponent>.{dtor}), (void*)(&ccomPtr<IVssComponent>));
							throw;
						}
						goto IL_366;
						IL_394:
						num3++;
						continue;
						IL_366:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						goto IL_394;
						IL_38D:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						goto IL_394;
						IL_36F:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result4;
						IL_37E:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result3;
						IL_39D:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result2;
						IL_3AC:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return result;
						IL_18E:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
						IL_134:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
						IL_E4:
						<Module>.ATL.CComPtr<IVssComponent>.{dtor}(ref ccomPtr<IVssComponent>);
						return num;
					}
					num = (flag2 ? -4 : num);
					return num;
				}
				catch (Exception ex)
				{
					num = -3;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "{0}:some exception is thrown {1}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsComponents", ex);
					object[] messageArgs3 = new object[]
					{
						"Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsComponents",
						-3,
						ex.ToString()
					};
					ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs3);
					num7 = -3;
				}
				result5 = num7;
			}
			finally
			{
				<Module>.SysFreeString(ptr);
				ptr = null;
				IntPtr hglobal = new IntPtr((void*)ptr2);
				Marshal.FreeHGlobal(hglobal);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsComponents {0:X8}", num);
			}
			return result5;
		}

		public int BackupCompleteTruncateLogsReplica(ReplayConfiguration replica, Guid vssIdCurrentSnapshotSetId, [MarshalAs(UnmanagedType.U1)] bool fTruncateLogs)
		{
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsReplica with {0}", vssIdCurrentSnapshotSetId);
			object[] array = new object[5];
			array[0] = replica.Name;
			Guid identityGuid = replica.IdentityGuid;
			array[1] = identityGuid;
			array[2] = replica.Type;
			array[3] = vssIdCurrentSnapshotSetId;
			array[4] = fTruncateLogs;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "replica->Name = {0}, replica->IdentityGuid = {1}, replica->Type {2}, vssIdCurrentSnapshotSetId = {3}, fTruncateLogs = {4}", array);
			int num = 0;
			replica.IdentityGuid.ToString();
			List<StorageGroupBackup>.Enumerator enumerator = this.m_backupInstances[vssIdCurrentSnapshotSetId].m_StorageGroupBackups.GetEnumerator();
			if (enumerator.MoveNext())
			{
				StorageGroupBackup storageGroupBackup;
				do
				{
					storageGroupBackup = enumerator.Current;
					if (replica.IdentityGuid == storageGroupBackup.m_guidSGIdentityGuid)
					{
						goto IL_D4;
					}
				}
				while (enumerator.MoveNext());
				goto IL_174;
				IL_D4:
				if (storageGroupBackup.m_fIsPassive)
				{
					num = this.HrEndSurrogateBackupOnPrimarySG(storageGroupBackup, 0);
					if (num < 0)
					{
						Guid identityGuid2 = replica.IdentityGuid;
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid, int>((long)this.GetHashCode(), "HrEndSurrogateBackupOnPrimarySG failed for {0}:{1} with {2:X8}.", replica.DatabaseName, identityGuid2, num);
						goto IL_17E;
					}
					goto IL_1B9;
				}
				else
				{
					if (fTruncateLogs)
					{
						SnapshotTruncateLogGrbit flags;
						if (ReplayConfigType.SingleCopySource == replica.Type)
						{
							flags = SnapshotTruncateLogGrbit.AllDatabasesSnapshot;
						}
						else
						{
							if (ReplayConfigType.RemoteCopySource != replica.Type)
							{
								new object[0];
								num = <Module>.HRESULT_FROM_WIN32(1610);
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsReplica Failed with {0:X8}.", num);
								goto IL_17E;
							}
							flags = SnapshotTruncateLogGrbit.None;
						}
						CReplicaVssWriterInterop.SnapshotTruncateLogInstance(storageGroupBackup.m_guidSGIdentityGuid, (uint)flags);
						goto IL_1B9;
					}
					goto IL_1EB;
				}
			}
			IL_174:
			new object[0];
			num = -4;
			IL_17E:
			object[] array2 = new object[3];
			array2[0] = vssIdCurrentSnapshotSetId;
			int num2 = num;
			array2[1] = num2.ToString("X");
			array2[2] = replica.Name;
			ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteWithFailure.LogEvent(string.Empty, array2);
			IL_1B9:
			if (fTruncateLogs)
			{
				object[] messageArgs = new object[]
				{
					vssIdCurrentSnapshotSetId,
					replica.Name
				};
				ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteLogsTruncated.LogEvent(string.Empty, messageArgs);
				goto IL_218;
			}
			IL_1EB:
			object[] messageArgs2 = new object[]
			{
				vssIdCurrentSnapshotSetId,
				replica.Name
			};
			ReplayEventLogConstants.Tuple_VSSWriterBackupCompleteNoTruncateRequested.LogEvent(string.Empty, messageArgs2);
			IL_218:
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::BackupCompleteTruncateLogsReplica {0:X8}", num);
			return num;
		}

		public void CleanupBackup(Guid vssIdCurrentSnapshotSetId)
		{
			BackupInstance backupInstance = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::CleanupBackup with {0}", vssIdCurrentSnapshotSetId);
			if (!this.m_backupInstances.TryGetValue(vssIdCurrentSnapshotSetId, out backupInstance))
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Bacup instance not found. Returning VSS_E_WRITERERROR_RETRYABLE.");
				this.FSetWriterFailureFromHResult(-2147212301);
			}
			else
			{
				try
				{
					foreach (StorageGroupBackup storageGroupBackup in backupInstance.m_StorageGroupBackups)
					{
						try
						{
							if (storageGroupBackup.m_fIsPassive)
							{
								if (storageGroupBackup.m_fFrozen)
								{
									this.FreezeOrThawReplica(storageGroupBackup, false);
								}
								if (storageGroupBackup.m_fSurrogateBackupBegun)
								{
									int num = this.HrEndSurrogateBackupOnPrimarySG(storageGroupBackup, -1);
								}
							}
							else
							{
								CReplicaVssWriterInterop.SnapshotStop(storageGroupBackup.m_guidSGIdentityGuid, 0U);
							}
						}
						catch (Exception arg)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "{0}: exception is thrown {1}", "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::CleanupBackup", arg);
						}
					}
				}
				finally
				{
					backupInstance.m_fSnapPrepared = false;
					backupInstance.m_StorageGroupBackups.Clear();
					this.m_backupInstances.Remove(vssIdCurrentSnapshotSetId);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::CleanupBackup");
				}
			}
		}

		[return: MarshalAs(UnmanagedType.U1)]
		private bool FValidObj()
		{
			return this.m_fValidObj;
		}

		private unsafe ushort* GetUnmanagedWideString(string str)
		{
			return (ushort*)Marshal.StringToHGlobalUni(str).ToPointer();
		}

		private unsafe void FreeUnmanagedWideString(ushort* wsz)
		{
			IntPtr hglobal = new IntPtr((void*)wsz);
			Marshal.FreeHGlobal(hglobal);
		}

		private unsafe Guid GuidFromUnmanagedGuid(_GUID guid)
		{
			byte[] array = new byte[16];
			Marshal.Copy((IntPtr)((void*)(&guid)), array, 0, 16);
			Guid result = new Guid(array);
			return result;
		}

		private static void SnapshotPrepare(Guid dbGuid, uint flags)
		{
			IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null);
			new object[0];
			try
			{
				newStoreControllerInstance.SnapshotPrepare(dbGuid, flags);
			}
			finally
			{
				if (newStoreControllerInstance != null)
				{
					newStoreControllerInstance.Dispose();
				}
			}
		}

		private static void SnapshotFreeze(Guid dbGuid, uint flags)
		{
			IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null);
			new object[0];
			try
			{
				newStoreControllerInstance.SnapshotFreeze(dbGuid, flags);
			}
			finally
			{
				if (newStoreControllerInstance != null)
				{
					newStoreControllerInstance.Dispose();
				}
			}
		}

		private static void SnapshotThaw(Guid dbGuid, uint flags)
		{
			IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null);
			new object[0];
			try
			{
				newStoreControllerInstance.SnapshotThaw(dbGuid, flags);
			}
			finally
			{
				if (newStoreControllerInstance != null)
				{
					newStoreControllerInstance.Dispose();
				}
			}
		}

		private static void SnapshotTruncateLogInstance(Guid dbGuid, uint flags)
		{
			IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null);
			new object[0];
			try
			{
				newStoreControllerInstance.SnapshotTruncateLogInstance(dbGuid, flags);
			}
			finally
			{
				if (newStoreControllerInstance != null)
				{
					newStoreControllerInstance.Dispose();
				}
			}
		}

		private static void SnapshotStop(Guid dbGuid, uint flags)
		{
			IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null);
			new object[0];
			try
			{
				newStoreControllerInstance.SnapshotStop(dbGuid, flags);
			}
			finally
			{
				if (newStoreControllerInstance != null)
				{
					newStoreControllerInstance.Dispose();
				}
			}
		}

		private int SetWriterFailureFromHr(int hr)
		{
			int num;
			if (hr != -2147212300)
			{
				if (hr == -2147024882)
				{
					num = -2147212303;
					goto IL_2B;
				}
				if (hr != -3)
				{
					num = -2147212301;
					goto IL_2B;
				}
			}
			num = -2147212300;
			IL_2B:
			<Module>.CReplicaVssWriter.SetWriterFailure(this.m_replicaWriter, num);
			return num;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		private bool FSetWriterFailureFromHResult(int hr)
		{
			bool flag = true;
			if (hr < 0)
			{
				flag = false;
				int num;
				if (hr != -2147212300)
				{
					if (hr == -2147024882)
					{
						num = -2147212303;
						goto IL_33;
					}
					if (hr != -3)
					{
						num = -2147212301;
						goto IL_33;
					}
				}
				num = -2147212300;
				IL_33:
				<Module>.CReplicaVssWriter.SetWriterFailure(this.m_replicaWriter, num);
				flag = (num == -2147212301 || flag);
			}
			return flag;
		}

		private uint JetBackupTypeFromVSSBackupType(int vssbackuptype)
		{
			uint result = 48U;
			if (vssbackuptype != 1)
			{
				if (vssbackuptype != 2)
				{
					if (vssbackuptype != 3)
					{
						if (vssbackuptype != 5)
						{
							new object[0];
						}
						else
						{
							result = 51U;
						}
					}
					else
					{
						result = 50U;
					}
				}
				else
				{
					result = 49U;
				}
			}
			else
			{
				result = 48U;
			}
			return result;
		}

		private int HandleException(Exception e, string function)
		{
			int hrforException = Marshal.GetHRForException(e);
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "{0}:some exception is thrown {1}", function, e);
			object[] messageArgs = new object[]
			{
				function,
				hrforException,
				e.ToString()
			};
			ReplayEventLogConstants.Tuple_VSSWriterException.LogEvent(string.Empty, messageArgs);
			if (null == e as MapiRetryableException && null == e as MapiExceptionCallFailed && null == e as MapiExceptionMdbOffline && null == e as MapiExceptionBackupInProgress && null == e as MapiExceptionInvalidParameter && null == e as MapiExceptionDatabaseError)
			{
				bool flag = ((!GrayException.IsGrayException(e)) ? 1 : 0) != 0;
				if (flag)
				{
					ExWatson.SendReport(e, ReportOptions.ReportTerminateAfterSend, null);
					<Module>.ForceNTFriendlyCrash();
				}
			}
			return (hrforException >= 0) ? -3 : hrforException;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		private bool StringEquals(string string1, string string2)
		{
			return string.Equals(string1, string2, StringComparison.InvariantCultureIgnoreCase);
		}

		private void AddToRestoreInProgress(ComponentInformation componentInformation)
		{
			new object[0];
			new object[0];
			this.FRestoreInProgress(componentInformation);
			this.m_restoreInProgressComponents.Add(componentInformation);
		}

		[return: MarshalAs(UnmanagedType.U1)]
		private bool RemoveFromRestoreInProgress(ComponentInformation componentInformation)
		{
			new object[0];
			return this.m_restoreInProgressComponents.Remove(componentInformation);
		}

		[return: MarshalAs(UnmanagedType.U1)]
		private bool FRestoreInProgress(ComponentInformation componentInformation)
		{
			new object[0];
			return this.m_restoreInProgressComponents.Contains(componentInformation);
		}

		private List<ReplayConfiguration> FindAllDatabaseConfigurations()
		{
			return ReplayConfigurationHelper.GetAllLocalCopyConfigurationsForVss();
		}

		private int HrGetDirectoryInfo(string directoryPath, ref DirectoryInfo directoryInfo)
		{
			int num = 0;
			int result;
			try
			{
				directoryInfo = new DirectoryInfo(directoryPath);
				return num;
			}
			catch (ArgumentNullException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentNullException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetDirectoryInfo: {0} ", arg);
				num = <Module>.HRESULT_FROM_WIN32(1610);
				return num;
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<SecurityException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetDirectoryInfo: {0} on path {1}.", arg2, directoryPath);
				num = <Module>.HRESULT_FROM_WIN32(5);
				result = num;
			}
			catch (ArgumentException arg3)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetDirectoryInfo: {0} Target log path {1} contains invalid characters.", arg3, directoryPath);
				return <Module>.HRESULT_FROM_WIN32(87);
			}
			catch (PathTooLongException arg4)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<PathTooLongException, string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetDirectoryInfo: {0} Target log path {1} is {2} long.", arg4, directoryPath, directoryPath.Length);
				num = <Module>.HRESULT_FROM_WIN32(24);
				return num;
			}
			return result;
		}

		private int HrGetFileInfos(DirectoryInfo directoryInfo, string fileNameFilter, ref FileInfo[] fileInfos)
		{
			new object[0];
			new object[0];
			string.IsNullOrEmpty(fileNameFilter);
			int num = 0;
			int result;
			try
			{
				fileInfos = directoryInfo.GetFiles(fileNameFilter, SearchOption.TopDirectoryOnly);
				return num;
			}
			catch (ArgumentNullException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentNullException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetFileInfos: {0} ", arg);
				num = <Module>.HRESULT_FROM_WIN32(1610);
				return num;
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<SecurityException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetFileInfos: {0} on path {1} for files {2}.", arg2, directoryInfo.ToString(), fileNameFilter);
				num = <Module>.HRESULT_FROM_WIN32(5);
				result = num;
			}
			catch (ArgumentException arg3)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetFileInfos: {0} Target log path {1} contains invalid characters.", arg3, directoryInfo.ToString());
				return <Module>.HRESULT_FROM_WIN32(87);
			}
			catch (DirectoryNotFoundException arg4)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<DirectoryNotFoundException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetFileInfos: {0} Target log path {1} is invalid.", arg4, directoryInfo.ToString());
				num = <Module>.HRESULT_FROM_WIN32(1168);
				return num;
			}
			return result;
		}

		private int HrDeleteFile(string fileIdentifier)
		{
			new object[0];
			string.IsNullOrEmpty(fileIdentifier);
			int num = 0;
			int result;
			try
			{
				if (File.Exists(fileIdentifier))
				{
					File.Delete(fileIdentifier);
				}
				return num;
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} Invalid path {1}.", arg, fileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(87);
				return num;
			}
			catch (PathTooLongException arg2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<PathTooLongException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} Path roo long {1}.", arg2, fileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(24);
				return num;
			}
			catch (DirectoryNotFoundException arg3)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<DirectoryNotFoundException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} Directory not found {1}.", arg3, Path.GetDirectoryName(fileIdentifier));
				num = <Module>.HRESULT_FROM_WIN32(1168);
				return num;
			}
			catch (IOException arg4)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<IOException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} IO Exception for {1}.", arg4, fileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(1117);
				return num;
			}
			catch (NotSupportedException arg5)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<NotSupportedException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} Path has invalid format {1}.", arg5, fileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(50);
				result = num;
			}
			catch (UnauthorizedAccessException arg6)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<UnauthorizedAccessException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteFile: {0} No access {1}.", arg6, fileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(5);
				return num;
			}
			return result;
		}

		private int HrMoveFile(string sourceFileIdentifier, string destinationFileIdentifier)
		{
			new object[0];
			string.IsNullOrEmpty(sourceFileIdentifier);
			new object[0];
			string.IsNullOrEmpty(destinationFileIdentifier);
			int num = 0;
			int result;
			try
			{
				File.Move(sourceFileIdentifier, destinationFileIdentifier);
				return num;
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} Source {1} or destination {2} invalid path .", arg, sourceFileIdentifier, destinationFileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(87);
				return num;
			}
			catch (PathTooLongException arg2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<PathTooLongException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} Source {1} or destination {2} path roo long.", arg2, sourceFileIdentifier, destinationFileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(24);
				return num;
			}
			catch (DirectoryNotFoundException arg3)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<DirectoryNotFoundException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} Source {1} or destination {2} directory not found .", arg3, Path.GetDirectoryName(sourceFileIdentifier), Path.GetDirectoryName(destinationFileIdentifier));
				num = <Module>.HRESULT_FROM_WIN32(1168);
				return num;
			}
			catch (IOException arg4)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<IOException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} Source {1} or destination {2} IO Exception.", arg4, sourceFileIdentifier, destinationFileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(1117);
				return num;
			}
			catch (NotSupportedException arg5)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<NotSupportedException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} Source {1} or destination {2} path has invalid format .", arg5, sourceFileIdentifier, destinationFileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(50);
				result = num;
			}
			catch (UnauthorizedAccessException arg6)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<UnauthorizedAccessException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrMoveFile: {0} No access {1}.", arg6, destinationFileIdentifier);
				num = <Module>.HRESULT_FROM_WIN32(5);
				return num;
			}
			return result;
		}

		private int HrDeleteTargetLogFiles(ComponentInformation componentInformation, string logFileName)
		{
			string text = null;
			DirectoryInfo directoryInfo = null;
			FileInfo[] array = null;
			new object[0];
			try
			{
				text = Path.GetFileName(logFileName);
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: {0} Target log file filter name {1} contains invalid path characters.", arg, componentInformation.m_logBaseName);
				return <Module>.HRESULT_FROM_WIN32(87);
			}
			int num = this.HrGetDirectoryInfo(componentInformation.m_logPathTarget, ref directoryInfo);
			if (num < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: HrGetDirectoryInfo failed with {0:X8} ", num);
				return num;
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Delete log files with name filter {0}, in log path target {1} .", text, componentInformation.m_logPathTarget);
			int num2 = this.HrGetFileInfos(directoryInfo, text, ref array);
			if (num2 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: HrGetFileInfos failed with {0:X8}", num2);
				return num2;
			}
			foreach (FileInfo fileInfo in array)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Delete log file {0} .", fileInfo.FullName);
				int result;
				try
				{
					fileInfo.Delete();
					goto IL_186;
				}
				catch (IOException arg2)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<IOException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: {0} IO Exception for {1}.", arg2, fileInfo.FullName);
					num2 = <Module>.HRESULT_FROM_WIN32(1117);
					return num2;
				}
				catch (SecurityException arg3)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<SecurityException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: {0} Missing permissions for {1}.", arg3, fileInfo.FullName);
					num2 = <Module>.HRESULT_FROM_WIN32(50);
					result = num2;
				}
				catch (UnauthorizedAccessException arg4)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<UnauthorizedAccessException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDeleteTargetLogFiles: {0} No access {1}.", arg4, fileInfo.FullName);
					num2 = <Module>.HRESULT_FROM_WIN32(5);
					return num2;
				}
				return result;
				IL_186:;
			}
			return num2;
		}

		private int HrRetrieveXmlNode(XmlDocument xmlDocument, string nodeName, ref XmlNode xmlNode)
		{
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					xmlNode = null;
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(nodeName);
					if (elementsByTagName.Count == 1)
					{
						xmlNode = elementsByTagName.Item(0);
						return 0;
					}
					if (elementsByTagName.Count == 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlNode: No elements with name {0} found.", nodeName);
						return <Module>.HRESULT_FROM_WIN32(1168);
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlNode: The number of elements with name {0} found is {1}", nodeName, elementsByTagName.Count);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_89;
			}
			arg = 1;
			IL_89:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlNode: xmlDocument {0}, nodeName {1}, xmlNode {2}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private int HrRemoveXmlNode(XmlDocument xmlDocument, string nodeName)
		{
			XmlNode xmlNode = null;
			byte arg2;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlNode(xmlDocument, nodeName, ref xmlNode);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRemoveXmlNode: HrRetrieveXmlNode failed with {0:X8}", num);
						return num;
					}
					try
					{
						xmlNode.ParentNode.RemoveChild(xmlNode);
					}
					catch (ArgumentException arg)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, ArgumentException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRemoveXmlNode: Remove node {0} failed with {1}.", nodeName, arg);
						num = <Module>.HRESULT_FROM_WIN32(87);
						return num;
					}
					return num;
				}
			}
			else if (null != nodeName)
			{
				arg2 = 0;
				goto IL_79;
			}
			arg2 = 1;
			IL_79:
			byte arg3 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRemoveXmlNode: xmlDocument {0}, nodeName {1}.", arg3 != 0, arg2 != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private int HrUpdateXmlNode(XmlDocument xmlDocument, string nodeName, string newValue)
		{
			XmlNode xmlNode = null;
			if (null != xmlDocument)
			{
				if (null == nodeName)
				{
					goto IL_F7;
				}
				if (!(null == newValue))
				{
					int num = this.HrRetrieveXmlNode(xmlDocument, nodeName, ref xmlNode);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrUpdateXmlNode: HrRetrieveXmlNode failed with {0:X8}", num);
						return num;
					}
					if (XmlNodeType.Element != xmlNode.NodeType)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<XmlNodeType>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrUpdateXmlNode: Invalid xml node type {0}", xmlNode.NodeType);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					XmlNodeList childNodes = xmlNode.ChildNodes;
					if (childNodes.Count != 1)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrUpdateXmlNode: The number of children elements of the element with name {0} found is {1}", nodeName, childNodes.Count);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					xmlNode = childNodes.Item(0);
					if (XmlNodeType.Text == xmlNode.NodeType)
					{
						xmlNode.Value = newValue;
						return num;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<XmlNodeType, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrUpdateXmlNode: Invalid xml node type {0}, value {1}", xmlNode.NodeType, xmlNode.Value);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
			}
			byte arg;
			if (null != nodeName)
			{
				arg = 0;
				goto IL_FD;
			}
			IL_F7:
			arg = 1;
			IL_FD:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrUpdateXmlNode: xmlDocument {0}, nodeName {1}, newValue {2}", arg2 != 0, arg != 0, null == newValue);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private int HrRetrieveXmlString(XmlDocument xmlDocument, string nodeName, ref string xmlValue)
		{
			XmlNode xmlNode = null;
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlNode(xmlDocument, nodeName, ref xmlNode);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlString: HrRetrieveXmlNode failed with {0:X8}", num);
						return num;
					}
					if (XmlNodeType.Element != xmlNode.NodeType)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<XmlNodeType>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlString: Invalid xml node type {0}", xmlNode.NodeType);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					XmlNodeList childNodes = xmlNode.ChildNodes;
					if (childNodes.Count != 1)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlString: The number of children elements of the element with name {0} found is {1}", nodeName, childNodes.Count);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					xmlNode = childNodes.Item(0);
					if (XmlNodeType.Text == xmlNode.NodeType)
					{
						xmlValue = xmlNode.Value;
						return num;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<XmlNodeType, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlString: Invalid xml node type {0}, value {1}", xmlNode.NodeType, xmlNode.Value);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_F2;
			}
			arg = 1;
			IL_F2:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlString: xmlDocument {0}, nodeName {1}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private int HrRetrieveXmlGuid(XmlDocument xmlDocument, string nodeName, ref ValueType guid)
		{
			string g = null;
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlString(xmlDocument, nodeName, ref g);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlGuid: HrRetrieveXmlString failed with {0:X8}", num);
						return num;
					}
					ValueType valueType = default(Guid);
					(Guid)valueType = new Guid(g);
					guid = valueType;
					return num;
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_5F;
			}
			arg = 1;
			IL_5F:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlGuid: xmlDocument {0}, nodeName {1}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private unsafe int HrRetrieveXmlUInt32(XmlDocument xmlDocument, string nodeName, uint* uint32)
		{
			string text = null;
			byte arg3;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlString(xmlDocument, nodeName, ref text);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt32: HrRetrieveXmlString failed with {0:X8}", num);
						return num;
					}
					int result;
					try
					{
						*uint32 = (int)Convert.ToUInt32(text);
						return num;
					}
					catch (FormatException arg)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<FormatException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt32: Conversion to UInt32 failed {0} for {1}.", arg, text);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						result = num;
					}
					catch (OverflowException arg2)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<OverflowException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt32: Conversion to UInt32 failed {0} for {1}.", arg2, text);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						return num;
					}
					return result;
				}
			}
			else if (null != nodeName)
			{
				arg3 = 0;
				goto IL_AB;
			}
			arg3 = 1;
			IL_AB:
			byte arg4 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt32: xmlDocument {0}, nodeName {1}", arg4 != 0, arg3 != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private unsafe int HrRetrieveXmlUInt64(XmlDocument xmlDocument, string nodeName, ulong* uint64)
		{
			string text = null;
			byte arg3;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlString(xmlDocument, nodeName, ref text);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt64: HrRetrieveXmlString failed with {0:X8}", num);
						return num;
					}
					int result;
					try
					{
						*uint64 = (long)Convert.ToUInt64(text);
						return num;
					}
					catch (FormatException arg)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<FormatException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt64: Conversion to UInt64 failed {0} for {1}.", arg, text);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						result = num;
					}
					catch (OverflowException arg2)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<OverflowException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt64: Conversion to UInt32 failed {0} for {1}.", arg2, text);
						num = <Module>.HRESULT_FROM_WIN32(1610);
						return num;
					}
					return result;
				}
			}
			else if (null != nodeName)
			{
				arg3 = 0;
				goto IL_AB;
			}
			arg3 = 1;
			IL_AB:
			byte arg4 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlUInt64: xmlDocument {0}, nodeName {1}", arg4 != 0, arg3 != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private unsafe int HrRetrieveXmlDateTime(XmlDocument xmlDocument, string nodeName, DateTime* dateTime)
		{
			string value = null;
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlString(xmlDocument, nodeName, ref value);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlDateTime: HrRetrieveXmlString failed with {0:X8}", num);
						return num;
					}
					DateTime dateTime2 = Convert.ToDateTime(value);
					*dateTime = dateTime2;
					return num;
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_4E;
			}
			arg = 1;
			IL_4E:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlDateTime: xmlDocument {0}, nodeName {1}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private unsafe int HrRetrieveXmlBool(XmlDocument xmlDocument, string nodeName, bool* result)
		{
			string text = null;
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					int num = this.HrRetrieveXmlString(xmlDocument, nodeName, ref text);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlBool: HrRetrieveXmlString failed with {0:X8}", num);
						return num;
					}
					string yes = ComponentInformation.YES;
					if (string.Equals(text, yes, StringComparison.InvariantCultureIgnoreCase))
					{
						*result = 1;
					}
					else
					{
						string no = ComponentInformation.NO;
						if (string.Equals(text, no, StringComparison.InvariantCultureIgnoreCase))
						{
							*result = 0;
						}
						else
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlBool: Expected element value boolean {0}.", text);
							num = <Module>.HRESULT_FROM_WIN32(1610);
						}
					}
					return num;
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_94;
			}
			arg = 1;
			IL_94:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrRetrieveXmlBool: xmlDocument {0}, nodeName {1}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private unsafe int HrFindXmlElement(XmlDocument xmlDocument, string nodeName, bool* foundIt)
		{
			int result = 0;
			byte arg;
			if (null != xmlDocument)
			{
				if (null != nodeName)
				{
					*foundIt = 0;
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(nodeName);
					if (elementsByTagName.Count == 1)
					{
						*foundIt = 1;
					}
					else if (elementsByTagName.Count == 0)
					{
						*foundIt = 0;
					}
					else
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrFindXmlElement: The number of elements with name {0} found is {1}", nodeName, elementsByTagName.Count);
						result = <Module>.HRESULT_FROM_WIN32(1610);
					}
					return result;
				}
			}
			else if (null != nodeName)
			{
				arg = 0;
				goto IL_64;
			}
			arg = 1;
			IL_64:
			byte arg2 = (null == xmlDocument) ? 1 : 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<bool, bool>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrFindXmlElement: xmlDocument {0}, nodeName {1}", arg2 != 0, arg != 0);
			return <Module>.HRESULT_FROM_WIN32(1610);
		}

		private int HrSaveRestoreEnvironmentXml(ComponentInformation componentInformation, XmlDocument xmlDocument)
		{
			new object[0];
			int num = 0;
			try
			{
				StringWriter stringWriter = new StringWriter(new StringBuilder());
				xmlDocument.Save(stringWriter);
				componentInformation.m_restoreEnvXml = stringWriter.GetStringBuilder().ToString();
			}
			catch (XmlException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<XmlException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} Cannot generate restore environment file content {1}.", arg, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(13);
				return num;
			}
			int result;
			try
			{
				File.WriteAllText(componentInformation.m_restoreEnv, componentInformation.m_restoreEnvXml, Encoding.Unicode);
				return num;
			}
			catch (ArgumentException arg2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} Invalid path {1}.", arg2, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(87);
				return num;
			}
			catch (PathTooLongException arg3)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<PathTooLongException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} Path roo long {1}.", arg3, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(24);
				return num;
			}
			catch (DirectoryNotFoundException arg4)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<DirectoryNotFoundException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} Directory not found {1}.", arg4, Path.GetDirectoryName(componentInformation.m_restoreEnv));
				num = <Module>.HRESULT_FROM_WIN32(1168);
				return num;
			}
			catch (IOException arg5)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<IOException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} IO Exception for {1}.", arg5, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(1117);
				return num;
			}
			catch (NotSupportedException arg6)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<NotSupportedException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} Path has invalid format {1}.", arg6, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(50);
				return num;
			}
			catch (SecurityException arg7)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<SecurityException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} No permissions to write {1}.", arg7, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(5);
				result = num;
			}
			catch (UnauthorizedAccessException arg8)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<UnauthorizedAccessException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrSaveRestoreEnvironmentXml: {0} No access {1}.", arg8, componentInformation.m_restoreEnv);
				num = <Module>.HRESULT_FROM_WIN32(5);
				return num;
			}
			return result;
		}

		private unsafe int HrOnRestore(IVssWriterComponents* pComponents, [MarshalAs(UnmanagedType.U1)] bool fPostRestore)
		{
			uint num = 0U;
			IVssComponent* ptr = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore");
			List<ComponentInformation> list = new List<ComponentInformation>();
			if (!this.m_fValidObj)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore FValidObj failed. Returning VSS_E_WRITERERROR_RETRYABLE.");
				return -2147212301;
			}
			_VSS_BACKUP_TYPE vss_BACKUP_TYPE = <Module>.CReplicaVssWriter.GetBackupType(this.m_replicaWriter);
			int num2;
			if ((_VSS_BACKUP_TYPE)2 != vss_BACKUP_TYPE && (_VSS_BACKUP_TYPE)3 != vss_BACKUP_TYPE)
			{
				num2 = 0;
			}
			else
			{
				num2 = 1;
			}
			bool flag = (byte)num2 != 0;
			int num3 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponents, ref num, *(*(long*)pComponents));
			if (num3 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponentCount failed with {0:X8}", num3);
				return num3;
			}
			for (uint num4 = 0U; num4 < num; num4 += 1U)
			{
				try
				{
					num3 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssComponent**), pComponents, num4, ref ptr, *(*(long*)pComponents + 16L));
					if (num3 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failed with {0:X8}", num3);
						return num3;
					}
					if (null == ptr)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failure (returned NULL)");
						num3 = <Module>.HRESULT_FROM_WIN32(1610);
						return num3;
					}
					bool flag2 = false;
					num3 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.Boolean*), ptr, ref flag2, *(*(long*)ptr + 112L));
					if (num3 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: IsSelectedForRestore failed with {0:X8}", num3);
						return num3;
					}
					if (flag2)
					{
						ComponentInformation componentInformation = new ComponentInformation();
						componentInformation.m_uIndex = num4;
						componentInformation.m_fPostRestore = (fPostRestore != null);
						componentInformation.m_fIncrementalBackupSet = flag;
						componentInformation.m_fSelectedForRestore = flag2;
						if (fPostRestore != null)
						{
							VSS_FILE_RESTORE_STATUS status;
							num3 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,VSS_FILE_RESTORE_STATUS*), ptr, ref status, *(*(long*)ptr + 288L));
							if (num3 < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetFileRestoreStatus failed with {0:X8}", num3);
								return num3;
							}
							componentInformation.m_status = status;
						}
						list.Add(componentInformation);
						object[] args = new object[]
						{
							num4,
							fPostRestore != null,
							flag,
							flag2
						};
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: Component {0}, fPostRestore {1}, fIncrementalBackupSet {2}, fSelectedForRestore {3} .", args);
					}
					else
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<uint>((long)this.GetHashCode(), "Component {0} not selected for restore.", num4);
					}
				}
				finally
				{
					if (null != ptr)
					{
						IVssComponent* ptr2 = ptr;
						object obj = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ptr2, *(*(long*)ptr2 + 16L));
						ptr = null;
					}
				}
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: No databases selected for restore.");
				return <Module>.HRESULT_FROM_WIN32(1610);
			}
			foreach (ComponentInformation componentInformation2 in list)
			{
				if (fPostRestore == null || (VSS_FILE_RESTORE_STATUS)2 == componentInformation2.m_status)
				{
					try
					{
						num3 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssComponent**), pComponents, componentInformation2.m_uIndex, ref ptr, *(*(long*)pComponents + 16L));
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failed with {0:X8}", num3);
							return num3;
						}
						if (null == ptr)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failure (returned NULL)");
							num3 = <Module>.HRESULT_FROM_WIN32(1610);
							return num3;
						}
						num3 = this.HrGetComponentRestoreInfo(ptr, componentInformation2);
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failed with {0:X8}", num3);
							return num3;
						}
						num3 = this.HrDetermineRestoreScenario(ptr, componentInformation2);
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failed with {0:X8}", num3);
							return num3;
						}
						num3 = this.HrLoadAlternateRestoreTargets(ptr, componentInformation2);
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: GetComponent failed with {0:X8}", num3);
							return num3;
						}
					}
					finally
					{
						if (null != ptr)
						{
							IVssComponent* ptr3 = ptr;
							object obj2 = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ptr3, *(*(long*)ptr3 + 16L));
							ptr = null;
						}
					}
					VssRestoreScenario rstscen = componentInformation2.m_rstscen;
					if (rstscen > VssRestoreScenario.rstscenUnknown)
					{
						if (rstscen > VssRestoreScenario.rstscenAlternateDB)
						{
							if (rstscen != VssRestoreScenario.rstscenAlternateLoc)
							{
								goto IL_44E;
							}
							num3 = this.HrVerifyAlternateLoc(componentInformation2);
							if (num3 < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: HrCompareRestoreTargetsWithAD failed with {0:X8}", num3);
								return num3;
							}
						}
						else
						{
							num3 = this.HrCompareRestoreTargetsWithAD(componentInformation2);
							if (num3 < 0)
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: HrCompareRestoreTargetsWithAD failed with {0:X8}", num3);
								return num3;
							}
						}
						num3 = this.HrCheckRestoreEnv(componentInformation2);
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: HrCheckRestoreEnv failed with {0:X8}", num3);
							return num3;
						}
						if (fPostRestore == null)
						{
							continue;
						}
						num3 = this.HrAdditionalPostRestoreTasks(componentInformation2);
						if (num3 < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: HrAdditionalPostRestoreTasks failed with {0:X8}", num3);
							return num3;
						}
						continue;
					}
					IL_44E:
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore: Unknown restore scenario.");
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrOnRestore -- {0:X8}", num3);
			return num3;
		}

		private unsafe int HrGetComponentRestoreInfo(IVssComponent* pComponent, ComponentInformation componentInformation)
		{
			string text = null;
			string logBaseName = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			new object[0];
			new object[0];
			int num = 0;
			uint num2 = 0U;
			bool flag = true;
			ushort* ptr = null;
			new object[0];
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo");
			try
			{
				num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponent, ref num2, *(*(long*)pComponent + 272L));
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: GetRestoreSubcomponentCount failed with {0:X8}", num);
					return num;
				}
				object[] array = new object[0];
				byte b = (num2 > 0U) ? 1 : 0;
				componentInformation.m_fSubComponentsExplicitlySelected = (b != 0);
				int fLogsSelectedForRestore = (b == 0) ? 1 : 0;
				componentInformation.m_fLogsSelectedForRestore = (fLogsSelectedForRestore != 0);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<uint, uint>((long)this.GetHashCode(), "{0} subcomponents explicitly selected for component {1}.", num2, componentInformation.m_uIndex);
				object[] array2 = new object[0];
				object[] array3 = new object[0];
				num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.Boolean*), pComponent, ref flag, *(*(long*)pComponent + 120L));
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: GetAdditionalRestores failed with {0:X8}", num);
					return num;
				}
				if (0 == num)
				{
					if (flag)
					{
						componentInformation.m_fAdditionalRestores = true;
						object[] array4 = new object[0];
					}
					else
					{
						object[] array5 = new object[0];
						componentInformation.m_fRunRecovery = true;
					}
				}
				else
				{
					componentInformation.m_fAdditionalRestores = true;
					object[] array6 = new object[0];
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<uint, bool, bool>((long)this.GetHashCode(), "Component {0}, fAdditionalRestores {1}, fRunRecovery {2}.", componentInformation.m_uIndex, componentInformation.m_fAdditionalRestores, componentInformation.m_fRunRecovery);
				num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), pComponent, ref ptr, *(*(long*)pComponent + 80L));
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: GetBackupMetadata failed with {0:X8}", num);
					return num;
				}
				if (null == ptr)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Failed to retrieve the backup metadata.");
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				string text6 = new string((char*)ptr);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<uint, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Component {0}, Backup metadata {1}", componentInformation.m_uIndex, text6);
				XmlDocument xmlDocument = new XmlDocument();
				int result;
				try
				{
					xmlDocument.LoadXml(text6);
				}
				catch (XmlException)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Invalid XML.");
					num = <Module>.HRESULT_FROM_WIN32(13);
					result = num;
					goto IL_5E3;
				}
				bool flag2;
				num = this.HrFindXmlElement(xmlDocument, ComponentInformation.VersionStamp, ref flag2);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrFindXmlElement failed with {0:X8}", num);
					return num;
				}
				if (!flag2)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrFindXmlElement failed with {0:X8}", num);
					object[] messageArgs = new object[0];
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterMissingVersionStampError.LogEvent(string.Empty, messageArgs);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Missing version stamp from backup metadata. Possible legacy backup set.");
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.VersionStamp, ref text);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				if (string.Compare(text, ComponentInformation.SupportedVersion) < 0)
				{
					object[] messageArgs2 = new object[]
					{
						text
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterBadVersionStampError.LogEvent(string.Empty, messageArgs2);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Backup set version not supported {0}.", text);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				num = this.HrRetrieveXmlGuid(xmlDocument, ComponentInformation.DatabaseGuid, ref componentInformation.m_guidDBOriginal);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlGuid for {0} failed with {1:X8}", ComponentInformation.DatabaseGuid, num);
					return num;
				}
				componentInformation.m_guidDBTarget = componentInformation.m_guidDBOriginal;
				uint random;
				num = this.HrRetrieveXmlUInt32(xmlDocument, ComponentInformation.LogSignatureId, ref random);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlUInt32 for {0} failed with {1:X8}", ComponentInformation.LogSignatureId, num);
					return num;
				}
				ulong ui64Time;
				num = this.HrRetrieveXmlUInt64(xmlDocument, ComponentInformation.LogSignatureTimestamp, ref ui64Time);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlDateTime for {0} failed with {1:X8}", ComponentInformation.LogSignatureTimestamp, num);
					return num;
				}
				JET_LOGTIME jet_LOGTIME = new JET_LOGTIME(ui64Time);
				DateTime? time = jet_LOGTIME.ToDateTime();
				JET_SIGNATURE signLog = new JET_SIGNATURE((int)random, time, null);
				componentInformation.m_signLog = signLog;
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.LogBaseName, ref logBaseName);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_logBaseName = logBaseName;
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.LogPathOriginal, ref text2);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_logPathOriginal = text2;
				componentInformation.m_logPathTarget = text2;
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.SystemPathOriginal, ref text3);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_systemPathOriginal = text3;
				componentInformation.m_systemPathTarget = text3;
				bool fCircularLoggingInBackupSet;
				num = this.HrRetrieveXmlBool(xmlDocument, ComponentInformation.CircularLogging, ref fCircularLoggingInBackupSet);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlBool failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_fCircularLoggingInBackupSet = fCircularLoggingInBackupSet;
				int fDatabaseFileSelectedForRestore;
				if (!componentInformation.m_fIncrementalBackupSet && !componentInformation.m_fSubComponentsExplicitlySelected)
				{
					fDatabaseFileSelectedForRestore = 1;
				}
				else
				{
					fDatabaseFileSelectedForRestore = 0;
				}
				componentInformation.m_fDatabaseFileSelectedForRestore = (fDatabaseFileSelectedForRestore != 0);
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbLocationOriginal, ref text4);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_edbLocationOriginal = text4;
				componentInformation.m_edbLocationTarget = text4;
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbFilenameOriginal, ref text5);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_edbFilenameOriginal = text5;
				componentInformation.m_edbFilenameTarget = text5;
				bool fPrivateMdb;
				num = this.HrRetrieveXmlBool(xmlDocument, ComponentInformation.PrivateMdb, ref fPrivateMdb);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: HrRetrieveXmlBool failed with {0:X8}", num);
					return num;
				}
				componentInformation.m_fPrivateMdb = fPrivateMdb;
				goto IL_5F9;
				IL_5E3:
				return result;
			}
			finally
			{
				<Module>.SysFreeString(ptr);
				ptr = null;
			}
			IL_5F9:
			if (componentInformation.m_fSubComponentsExplicitlySelected)
			{
				new object[0];
				for (uint num3 = 0U; num3 < num2; num3 += 1U)
				{
					ushort* ptr2 = null;
					ushort* ptr3 = null;
					try
					{
						bool flag3;
						num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,System.UInt16**,System.UInt16**,System.Boolean*), pComponent, num3, ref ptr2, ref ptr3, ref flag3, *(*(long*)pComponent + 280L));
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: GetRestoreSubcomponent failed with {0:X8}", num);
							return num;
						}
						if (null == ptr3)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: GetRestoreSubcomponent failed to return subcomponent name.");
							num = <Module>.HRESULT_FROM_WIN32(1610);
							return num;
						}
						string text7 = new string((char*)ptr3);
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<uint, string>((long)this.GetHashCode(), "Restoring subcomponent {0}:{1}.", num2, text7);
						if (this.StringEquals(text7, ComponentInformation.Logs))
						{
							object[] array7 = new object[0];
							componentInformation.m_fLogsSelectedForRestore = true;
						}
						else
						{
							if (!this.StringEquals(text7, ComponentInformation.File))
							{
								ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo: Could not find selected subcomponents for {0}.", text7);
								num = <Module>.HRESULT_FROM_WIN32(1610);
								return num;
							}
							object[] array8 = new object[0];
							componentInformation.m_fDatabaseFileSelectedForRestore = true;
						}
					}
					finally
					{
						<Module>.SysFreeString(ptr2);
						ptr2 = null;
						<Module>.SysFreeString(ptr3);
						ptr3 = null;
					}
				}
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGetComponentRestoreInfo -- {0:X8}", num);
			return num;
		}

		private unsafe int HrDetermineRestoreScenario(IVssComponent* pComponent, ComponentInformation componentInformation)
		{
			ValueType valueType = null;
			ValueType valueType2 = null;
			new object[0];
			new object[0];
			ushort* ptr = null;
			string text = null;
			new object[0];
			new object[0];
			new object[0];
			new object[0];
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario");
			try
			{
				int num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), pComponent, ref ptr, *(*(long*)pComponent + 264L));
				if (1 == num)
				{
					object[] array = new object[0];
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenarioNo restore options.");
					componentInformation.m_fLegacyRequestor = true;
					componentInformation.m_guidDBTarget = componentInformation.m_guidDBOriginal;
					componentInformation.m_rstscen = VssRestoreScenario.rstscenOriginalDB;
					return 0;
				}
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: GetRestoreOptions failed with {0:X8}", num);
					return num;
				}
				if (null == ptr)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: GetRestoreOptions failed to return restore options.");
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				text = new string((char*)ptr);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Parsing restore options {0}.", text);
				if (!componentInformation.m_fPostRestore)
				{
					object[] messageArgs = new object[]
					{
						text
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreOptionsString.LogEvent(string.Empty, messageArgs);
				}
			}
			finally
			{
				<Module>.SysFreeString(ptr);
				ptr = null;
			}
			new object[0];
			text != null;
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(text);
			}
			catch (XmlException)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: Invalid XML.");
				return <Module>.HRESULT_FROM_WIN32(13);
			}
			int num2 = this.HrRetrieveXmlGuid(xmlDocument, ComponentInformation.DatabaseGuidOriginal, ref valueType);
			if (num2 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: HrRetrieveXmlGuid for {0} failed with {1:X8}", ComponentInformation.DatabaseGuidOriginal, num2);
				return num2;
			}
			int num3 = this.HrRetrieveXmlGuid(xmlDocument, ComponentInformation.DatabaseGuidTarget, ref valueType2);
			if (num3 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: HrRetrieveXmlGuid for {0} failed with {1:X8}", ComponentInformation.DatabaseGuidTarget, num3);
				return num3;
			}
			if (componentInformation.m_fSelectedForRestore && ((Guid)componentInformation.m_guidDBOriginal).Equals(valueType))
			{
				if (((Guid)valueType).Equals(valueType2))
				{
					componentInformation.m_rstscen = VssRestoreScenario.rstscenOriginalDB;
				}
				else if (((Guid)valueType2).Equals(Guid.Empty))
				{
					componentInformation.m_rstscen = VssRestoreScenario.rstscenAlternateLoc;
				}
				else
				{
					componentInformation.m_rstscen = VssRestoreScenario.rstscenAlternateDB;
				}
				componentInformation.m_guidDBTarget = valueType2;
				componentInformation.m_fRemappedGuid = true;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<VssRestoreScenario>((long)this.GetHashCode(), "Restore scenario: {0}", componentInformation.m_rstscen);
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario -- {0:X8}", num3);
				return num3;
			}
			object[] args = new object[]
			{
				componentInformation.m_guidDBOriginal
			};
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrDetermineRestoreScenario: Database mapping speficied Guid {0} for a database which was not selected for restore.", args);
			int result = <Module>.HRESULT_FROM_WIN32(1610);
			object[] messageArgs2 = new object[]
			{
				componentInformation.m_guidDBOriginal
			};
			ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterDbGuidMappingMismatchError.LogEvent(string.Empty, messageArgs2);
			return result;
		}

		private unsafe int HrLoadAlternateRestoreTargets(IVssComponent* pComponent, ComponentInformation componentInformation)
		{
			new object[0];
			new object[0];
			IVssWMFiledesc* ptr = null;
			ushort* ptr2 = null;
			ushort* ptr3 = null;
			ushort* ptr4 = null;
			uint num = 0U;
			new object[0];
			new object[0];
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets");
			int num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32*), pComponent, ref num, *(*(long*)pComponent + 128L));
			if (num2 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetNewTargetCount failed with {0:X8}.", num2);
				return num2;
			}
			object[] args = new object[]
			{
				componentInformation.m_logBaseName + ComponentInformation.LogsWildCard,
				componentInformation.m_logBaseName + ComponentInformation.CheckpointExtension,
				componentInformation.m_edbFilenameOriginal,
				num
			};
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Component: log filename {0}, checkpoint filename {1}, edb filename {2}, restore targets {3}", args);
			for (uint num3 = 0U; num3 < num; num3 += 1U)
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				try
				{
					object[] array = new object[0];
					num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32,IVssWMFiledesc**), pComponent, num3, ref ptr, *(*(long*)pComponent + 136L));
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetNewTarget failed with {0:X8}.", num2);
						return num2;
					}
					if (null == ptr)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetNewTarget failed to return FileDesc.");
						num2 = <Module>.HRESULT_FROM_WIN32(1610);
						return num2;
					}
					bool flag = false;
					num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.Boolean*), ptr, ref flag, *(*(long*)ptr + 40L));
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetRecursive failed with {0:X8}.", num2);
						return num2;
					}
					if (flag)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Recursive retargetting not supported.");
						num2 = <Module>.HRESULT_FROM_WIN32(1610);
						return num2;
					}
					object[] array2 = new object[0];
					num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr, ref ptr2, *(*(long*)ptr + 32L));
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetFilespec failed with {0:X8}.", num2);
						return num2;
					}
					if (null == ptr2)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetFilespec failed to return the filename of a new restore target.");
						num2 = <Module>.HRESULT_FROM_WIN32(1610);
						return num2;
					}
					object[] array3 = new object[0];
					num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr, ref ptr3, *(*(long*)ptr + 24L));
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetPath failed with {0:X8}.", num2);
						return num2;
					}
					if (null == ptr3)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetPath failed to return the original location of a new restore target.");
						num2 = <Module>.HRESULT_FROM_WIN32(1610);
						return num2;
					}
					object[] array4 = new object[0];
					num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt16**), ptr, ref ptr4, *(*(long*)ptr + 48L));
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetAlternateLocation failed with {0:X8}.", num2);
						return num2;
					}
					if (null == ptr4)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: GetPath failed to return the target location of a new restore target.");
						num2 = <Module>.HRESULT_FROM_WIN32(1610);
						return num2;
					}
					text = new string((char*)ptr2);
					text2 = new string((char*)ptr3);
					text3 = new string((char*)ptr4);
				}
				finally
				{
					<Module>.SysFreeString(ptr2);
					ptr2 = null;
					<Module>.SysFreeString(ptr3);
					ptr3 = null;
					<Module>.SysFreeString(ptr4);
					ptr4 = null;
					IVssWMFiledesc* ptr5 = ptr;
					object obj = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ptr5, *(*(long*)ptr5 + 16L));
					ptr = null;
				}
				new object[0];
				null != text;
				new object[0];
				null != text2;
				new object[0];
				null != text3;
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: filename {0}, original location {1}, target location {2}", text, text2, text3);
				string b = componentInformation.m_logBaseName + ComponentInformation.LogsWildCard;
				if (string.Equals(text, b, StringComparison.InvariantCultureIgnoreCase))
				{
					if (componentInformation.m_fLogsRelocated)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Duplicate alternate log path specified.");
						object[] messageArgs = new object[]
						{
							text,
							text2
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterMultipleRetargettingError.LogEvent(string.Empty, messageArgs);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					char[] trimChars = new char[]
					{
						' ',
						'\\'
					};
					char[] trimChars2 = new char[]
					{
						' ',
						'\\'
					};
					string b2 = componentInformation.m_logPathOriginal.TrimEnd(trimChars);
					if (!string.Equals(text2.TrimEnd(trimChars2), b2, StringComparison.InvariantCultureIgnoreCase))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Log path original from VSS component {0}, from compoenent information {1} mismatch.", text2, componentInformation.m_logPathOriginal);
						object[] messageArgs2 = new object[]
						{
							text2,
							componentInformation.m_logPathOriginal
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterOriginalLogfilePathMismatchError.LogEvent(string.Empty, messageArgs2);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					componentInformation.m_fLogsRelocated = true;
					componentInformation.m_logPathTarget = text3;
				}
				else
				{
					string b3 = componentInformation.m_logBaseName + ComponentInformation.CheckpointExtension;
					if (string.Equals(text, b3, StringComparison.InvariantCultureIgnoreCase))
					{
						if (componentInformation.m_fCheckpointRelocated)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Duplicate alternate log path specified.");
							object[] messageArgs3 = new object[]
							{
								text,
								componentInformation.m_systemPathOriginal
							};
							ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterMultipleRetargettingError.LogEvent(string.Empty, messageArgs3);
							return <Module>.HRESULT_FROM_WIN32(1610);
						}
						char[] trimChars3 = new char[]
						{
							' ',
							'\\'
						};
						char[] trimChars4 = new char[]
						{
							' ',
							'\\'
						};
						string b4 = componentInformation.m_logPathOriginal.TrimEnd(trimChars3);
						if (!string.Equals(text2.TrimEnd(trimChars4), b4, StringComparison.InvariantCultureIgnoreCase))
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Checkpoint path original from VSS component {0}, from component information {1} mismatch.", text2, componentInformation.m_logPathOriginal);
							object[] messageArgs4 = new object[]
							{
								text,
								componentInformation.m_systemPathOriginal
							};
							ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterOriginalSystemPathMismatchError.LogEvent(string.Empty, messageArgs4);
							return <Module>.HRESULT_FROM_WIN32(1610);
						}
						componentInformation.m_fCheckpointRelocated = true;
						componentInformation.m_systemPathTarget = text3;
					}
					else
					{
						string edbFilenameOriginal = componentInformation.m_edbFilenameOriginal;
						if (!string.Equals(text, edbFilenameOriginal, StringComparison.InvariantCultureIgnoreCase))
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Couldn't match restore target specification with any component.");
							return <Module>.HRESULT_FROM_WIN32(1610);
						}
						if (!componentInformation.m_fDatabaseFileSelectedForRestore)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Restore target specification match for EDB but database file not selected for restore.");
							object[] messageArgs5 = new object[]
							{
								text,
								text2
							};
							ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterDbRetargetMismatchError.LogEvent(string.Empty, messageArgs5);
							return <Module>.HRESULT_FROM_WIN32(1610);
						}
						if (componentInformation.m_fEDBRelocated)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: Duplicate alternate EDB path specified.");
							object[] messageArgs6 = new object[]
							{
								text,
								text2
							};
							ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterMultipleRetargettingError.LogEvent(string.Empty, messageArgs6);
							return <Module>.HRESULT_FROM_WIN32(1610);
						}
						componentInformation.m_fEDBRelocated = true;
						componentInformation.m_edbLocationTarget = text3;
					}
				}
			}
			object[] args2 = new object[]
			{
				componentInformation.m_fLogsRelocated,
				componentInformation.m_logPathTarget,
				componentInformation.m_fCheckpointRelocated,
				componentInformation.m_systemPathTarget,
				componentInformation.m_fEDBRelocated,
				componentInformation.m_edbLocationTarget
			};
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets: logs relocated {0}, logs target path {1}, checkpoint relocated {2}, system target path {3}, edb relocated {4}, edb target path {5}", args2);
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrLoadAlternateRestoreTargets -- {0:X8}", num2);
			return num2;
		}

		private int HrCompareRestoreTargetsWithAD(ComponentInformation componentInformation)
		{
			string text = null;
			new object[0];
			int num = 0;
			ReplayConfiguration replayConfiguration = null;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD");
			List<ReplayConfiguration> allLocalCopyConfigurationsForVss = ReplayConfigurationHelper.GetAllLocalCopyConfigurationsForVss();
			object[] args = new object[]
			{
				componentInformation.m_displayNameTarget,
				componentInformation.m_guidDBTarget
			};
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Try to find database {0}:{1} in AD.", args);
			List<ReplayConfiguration>.Enumerator enumerator = allLocalCopyConfigurationsForVss.GetEnumerator();
			if (enumerator.MoveNext())
			{
				ReplayConfiguration replayConfiguration2;
				do
				{
					replayConfiguration2 = enumerator.Current;
					Guid identityGuid = replayConfiguration2.IdentityGuid;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Found database {0}:{1} .", replayConfiguration2.Name, identityGuid);
					Guid identityGuid2 = replayConfiguration2.IdentityGuid;
					if (((Guid)componentInformation.m_guidDBTarget).Equals(identityGuid2))
					{
						goto IL_C6;
					}
				}
				while (enumerator.MoveNext());
				goto IL_C9;
				IL_C6:
				replayConfiguration = replayConfiguration2;
			}
			IL_C9:
			if (null == replayConfiguration)
			{
				if (!componentInformation.m_fLegacyRequestor)
				{
					object[] args2 = new object[]
					{
						componentInformation.m_guidDBTarget
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Could not find database {1} in AD.", args2);
					object[] messageArgs = new object[]
					{
						componentInformation.m_displayNameTarget,
						componentInformation.m_guidDBTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetSGLookupError.LogEvent(string.Empty, messageArgs);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				componentInformation.m_fRemappedGuid = true;
				componentInformation.m_rstscen = VssRestoreScenario.rstscenAlternateLoc;
				componentInformation.m_guidDBTarget = Guid.Empty;
				num = this.HrVerifyAlternateLoc(componentInformation);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: HrVerifyAlternateLoc failed with {0:X8}.", num);
					return num;
				}
				return num;
			}
			else
			{
				object[] args3 = new object[]
				{
					replayConfiguration.DisplayName,
					replayConfiguration.Database,
					replayConfiguration.DestinationEdbPath,
					replayConfiguration.DestinationSystemPath,
					replayConfiguration.DestinationLogPath,
					replayConfiguration.SourceEdbPath,
					replayConfiguration.SourceSystemPath,
					replayConfiguration.SourceLogPath
				};
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "DisplayName {0}, Database {1}, DestinationEdbPath {2}, DestinationSystemPath {3}, DestinationLogPath {4}, SourceEdbPath {5}, SourceSystemPath {6}, SourceLogPath {7}", args3);
				componentInformation.m_displayNameTarget = replayConfiguration.DisplayName;
				Guid identityGuid3 = replayConfiguration.IdentityGuid;
				CopyStatusEnum copyStatus = this.m_replicaInstanceManager.GetCopyStatus(identityGuid3);
				if (CopyStatusEnum.Dismounted != copyStatus)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Target database {0} is in state '{1}'.", replayConfiguration.DisplayName, (int)copyStatus);
					object[] messageArgs2 = new object[]
					{
						replayConfiguration.DisplayName
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetSGOnline.LogEvent(string.Empty, messageArgs2);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				if (!componentInformation.m_fPostRestore)
				{
					if (VssRestoreScenario.rstscenOriginalDB == componentInformation.m_rstscen)
					{
						object[] messageArgs3 = new object[]
						{
							componentInformation.m_displayNameTarget
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreToOriginalSG.LogEvent(string.Empty, messageArgs3);
					}
					else
					{
						object[] messageArgs4 = new object[]
						{
							componentInformation.m_displayNameTarget
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreToAlternateSG.LogEvent(string.Empty, messageArgs4);
					}
				}
				if (VssRestoreScenario.rstscenAlternateDB == componentInformation.m_rstscen)
				{
					try
					{
						text = Path.Combine(replayConfiguration.DestinationLogPath, ComponentInformation.RestoreLogs);
					}
					catch (ArgumentException)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Destination log path {0} contains invalid path characters.", replayConfiguration.DestinationLogPath);
						num = <Module>.HRESULT_FROM_WIN32(87);
						return num;
					}
					if (text.Length > 260)
					{
						Trace replicaVssWriterInteropTracer = ExTraceGlobals.ReplicaVssWriterInteropTracer;
						long id = (long)this.GetHashCode();
						string formatString = "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Target path {0} is {1} long > {2}";
						string text2 = text;
						replicaVssWriterInteropTracer.TraceError<string, int, int>(id, formatString, text2, text2.Length, 260);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					char[] trimChars = new char[]
					{
						' ',
						'\\'
					};
					string text3 = componentInformation.m_logPathTarget.TrimEnd(trimChars);
					string text4 = text.TrimEnd(new char[]
					{
						' ',
						'\\'
					});
					if (!string.Equals(text3, text4, StringComparison.InvariantCultureIgnoreCase))
					{
						object[] args4 = new object[]
						{
							text3,
							componentInformation.m_logPathTarget,
							text4,
							text
						};
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Log target path {0} ( {1} ) mismatch expected {2} ( {3} ) .", args4);
						object[] messageArgs5 = new object[]
						{
							text3,
							text4,
							componentInformation.m_displayNameTarget
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetLogfilePathMismatchError.LogEvent(string.Empty, messageArgs5);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					char[] trimChars2 = new char[]
					{
						' ',
						'\\'
					};
					string text5 = componentInformation.m_systemPathTarget.TrimEnd(trimChars2);
					if (!string.Equals(text5, text4, StringComparison.InvariantCultureIgnoreCase))
					{
						object[] args5 = new object[]
						{
							text5,
							componentInformation.m_systemPathTarget,
							text4,
							text
						};
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: System target path {0} ( {1} ) mismatch expected {2} ( {3} ) .", args5);
						object[] messageArgs6 = new object[]
						{
							text5,
							text4,
							componentInformation.m_displayNameTarget
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetSystemPathMismatchError.LogEvent(string.Empty, messageArgs6);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					goto IL_70E;
				}
				char[] trimChars3 = new char[]
				{
					' ',
					'\\'
				};
				string text6 = componentInformation.m_logPathTarget.TrimEnd(trimChars3);
				char[] trimChars4 = new char[]
				{
					' ',
					'\\'
				};
				string text7 = replayConfiguration.DestinationLogPath.TrimEnd(trimChars4);
				if (!string.Equals(text6, text7, StringComparison.InvariantCultureIgnoreCase))
				{
					object[] args6 = new object[]
					{
						text6,
						componentInformation.m_logPathTarget,
						text7,
						replayConfiguration.DestinationLogPath
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Log target path {0} ( {1} ) mismatch expected {2} ( {3} ) .", args6);
					object[] messageArgs7 = new object[]
					{
						text6,
						text7,
						componentInformation.m_displayNameTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetLogfilePathMismatchError.LogEvent(string.Empty, messageArgs7);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				string logFilePrefix = replayConfiguration.LogFilePrefix;
				if (!string.Equals(componentInformation.m_logBaseName, logFilePrefix, StringComparison.InvariantCultureIgnoreCase))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Log file prefix (base name) {0} mismatch expected {2} .", componentInformation.m_logBaseName, replayConfiguration.LogFilePrefix);
					object[] messageArgs8 = new object[]
					{
						componentInformation.m_logBaseName,
						replayConfiguration.LogFilePrefix,
						componentInformation.m_displayNameTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetLogfileBaseNameMismatchError.LogEvent(string.Empty, messageArgs8);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				char[] trimChars5 = new char[]
				{
					' ',
					'\\'
				};
				string text8 = componentInformation.m_systemPathTarget.TrimEnd(trimChars5);
				char[] trimChars6 = new char[]
				{
					' ',
					'\\'
				};
				string text9 = replayConfiguration.DestinationSystemPath.TrimEnd(trimChars6);
				if (!string.Equals(text8, text9, StringComparison.InvariantCultureIgnoreCase))
				{
					object[] args7 = new object[]
					{
						text8,
						componentInformation.m_systemPathTarget,
						text9,
						replayConfiguration.DestinationSystemPath
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Log file prefix (base name) {0} ( {1} ) mismatch expected {2} ( {3} ) .", args7);
					object[] messageArgs9 = new object[]
					{
						text8,
						text9,
						componentInformation.m_displayNameTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetSystemPathMismatchError.LogEvent(string.Empty, messageArgs9);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				IL_70E:
				if (this.FRestoreInProgress(componentInformation))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Component {0} restore in progress.", componentInformation.m_displayNameTarget);
					object[] messageArgs10 = new object[]
					{
						componentInformation.m_displayNameTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterSGRestoreInProgressError.LogEvent(string.Empty, messageArgs10);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				if (!componentInformation.m_fPostRestore)
				{
					num = this.HrVerifyExistingLogFiles(componentInformation);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: HrVerifyExistingLogFiles failed with {0:X8}.", num);
						object[] messageArgs11 = new object[]
						{
							componentInformation.m_displayNameTarget,
							componentInformation.m_logPathTarget,
							num
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterFindLogFilesError.LogEvent(string.Empty, messageArgs11);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
				}
				byte circularLoggingEnabled = replayConfiguration.CircularLoggingEnabled ? 1 : 0;
				componentInformation.m_fCircularLoggingInDBTarget = (circularLoggingEnabled != 0);
				if (VssRestoreScenario.rstscenOriginalDB == componentInformation.m_rstscen && componentInformation.m_fSubComponentsExplicitlySelected && componentInformation.m_fLogsSelectedForRestore && (componentInformation.m_fCircularLoggingInBackupSet || circularLoggingEnabled != 0))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Circular logging enabled for restoring database {0}.", componentInformation.m_displayNameTarget);
					object[] messageArgs12 = new object[]
					{
						componentInformation.m_displayNameTarget
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterCircularLogDBRestore.LogEvent(string.Empty, messageArgs12);
					return <Module>.HRESULT_FROM_WIN32(1610);
				}
				if (componentInformation.m_fDatabaseFileSelectedForRestore)
				{
					string fileName = Path.GetFileName(replayConfiguration.SourceEdbPath);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string>((long)this.GetHashCode(), "replica edb filename {0}, componenent edb filename target {1}", fileName, componentInformation.m_edbFilenameTarget);
					string edbFilenameTarget = componentInformation.m_edbFilenameTarget;
					if (!string.Equals(fileName, edbFilenameTarget, StringComparison.InvariantCultureIgnoreCase))
					{
						componentInformation.m_edbFilenameTarget = fileName;
						componentInformation.m_fEDBRenamed = true;
					}
					char[] trimChars7 = new char[]
					{
						' ',
						'\\'
					};
					string text10 = componentInformation.m_edbLocationTarget.TrimEnd(trimChars7);
					char[] trimChars8 = new char[]
					{
						' ',
						'\\'
					};
					string text11 = Path.GetDirectoryName(replayConfiguration.DestinationEdbPath).TrimEnd(trimChars8);
					if (!string.Equals(text10, text11, StringComparison.InvariantCultureIgnoreCase))
					{
						object[] args8 = new object[]
						{
							text10,
							componentInformation.m_edbLocationTarget,
							text11,
							Path.GetDirectoryName(replayConfiguration.DestinationEdbPath)
						};
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: EDB location mismatch detected. Expected {0} ( {1} ), actual {2} ( {3} ) .", args8);
						object[] messageArgs13 = new object[]
						{
							componentInformation.m_edbFilenameOriginal,
							text10,
							componentInformation.m_edbFilenameTarget,
							componentInformation.m_displayNameTarget,
							text11
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetDbMismatchError.LogEvent(string.Empty, messageArgs13);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
					if (!replayConfiguration.AllowFileRestore)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD: Database {0} cannot be overwritten.", replayConfiguration.DatabaseName);
						object[] messageArgs14 = new object[]
						{
							componentInformation.m_edbFilenameTarget,
							componentInformation.m_edbLocationTarget,
							componentInformation.m_displayNameTarget
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterCannotOverwriteError.LogEvent(string.Empty, messageArgs14);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCompareRestoreTargetsWithAD -- {0:X8}", num);
				return num;
			}
		}

		private int HrVerifyExistingLogFiles(ComponentInformation componentInformation)
		{
			string fileNameFilter = null;
			DirectoryInfo directoryInfo = null;
			FileInfo[] array = null;
			string text = null;
			JET_LOGINFOMISC jet_LOGINFOMISC = null;
			new object[0];
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles");
			try
			{
				fileNameFilter = Path.GetFileName(componentInformation.m_logBaseName + ComponentInformation.LogsWildCard);
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: {0} Target log file filter name {1} contains invalid path characters.", arg, componentInformation.m_logBaseName);
				return <Module>.HRESULT_FROM_WIN32(87);
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles log path target {0}", componentInformation.m_logPathTarget);
			int num = this.HrGetDirectoryInfo(componentInformation.m_logPathTarget, ref directoryInfo);
			if (num < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: HrGetDirectoryInfo failed with {0:X8} ", num);
				return num;
			}
			int num2 = this.HrGetFileInfos(directoryInfo, fileNameFilter, ref array);
			if (<Module>.HRESULT_FROM_WIN32(1168) == num2)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles log path target {0} not found. No logfile to verify.", componentInformation.m_logPathTarget);
				return 0;
			}
			if (num2 < 0)
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: HrGetFileInfos failed with {0:X8}", num2);
				return num2;
			}
			string b = componentInformation.m_logBaseName + ComponentInformation.TempLogfile;
			foreach (FileInfo fileInfo in array)
			{
				if (fileInfo.Exists)
				{
					try
					{
						text = fileInfo.FullName;
					}
					catch (PathTooLongException arg2)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<PathTooLongException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: Target log path too long {0}", arg2);
						num2 = <Module>.HRESULT_FROM_WIN32(24);
						return num2;
					}
					if (string.Equals(fileInfo.Name, b, StringComparison.InvariantCultureIgnoreCase))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles Skip the temporary logfile {0}", text);
						goto IL_265;
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles Check the log signature for the file with filename {0}", text);
					try
					{
						UnpublishedApi.JetGetLogFileInfo(text, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
					}
					catch (EsentErrorException arg3)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<EsentErrorException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: JetGetLogFileInfo failed with {0:X8}.", arg3);
						num2 = <Module>.HRESULT_FROM_WIN32(13);
						return num2;
					}
					if (jet_LOGINFOMISC.signLog.ulRandom != componentInformation.m_signLog.ulRandom || !(jet_LOGINFOMISC.signLog.logtimeCreate == componentInformation.m_signLog.logtimeCreate))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles: Log signature mismatch.");
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
				}
				IL_265:;
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyExistingLogFiles -- {0:X8}", num2);
			return num2;
		}

		private int HrVerifyAlternateLoc(ComponentInformation componentInformation)
		{
			string text = null;
			new object[0];
			int num = 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc");
			if (this.FRestoreInProgress(componentInformation))
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc: Component {0} restore in progress.", componentInformation.m_displayNameTarget);
				object[] messageArgs = new object[]
				{
					componentInformation.m_logPathTarget
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterLocationRestoreInProgressError.LogEvent(string.Empty, messageArgs);
				return <Module>.HRESULT_FROM_WIN32(1610);
			}
			List<ReplayConfiguration>.Enumerator enumerator = ReplayConfigurationHelper.GetAllLocalCopyConfigurationsForVss().GetEnumerator();
			if (enumerator.MoveNext())
			{
				ReplayConfiguration replayConfiguration;
				do
				{
					replayConfiguration = enumerator.Current;
					string destinationLogPath = replayConfiguration.DestinationLogPath;
					if (string.Equals(componentInformation.m_logPathTarget, destinationLogPath, StringComparison.InvariantCultureIgnoreCase))
					{
						goto IL_D3;
					}
					string destinationSystemPath = replayConfiguration.DestinationSystemPath;
					if (string.Equals(componentInformation.m_logPathTarget, destinationSystemPath, StringComparison.InvariantCultureIgnoreCase))
					{
						goto IL_D3;
					}
				}
				while (enumerator.MoveNext());
				goto IL_123;
				IL_D3:
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc: Target log path {0} is in use.", componentInformation.m_logPathTarget);
				object[] messageArgs2 = new object[]
				{
					componentInformation.m_logPathTarget,
					replayConfiguration.DisplayName
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterTargetLogfilePathInUseError.LogEvent(string.Empty, messageArgs2);
				return <Module>.HRESULT_FROM_WIN32(1610);
			}
			IL_123:
			if (!componentInformation.m_fPostRestore)
			{
				if (componentInformation.m_fDatabaseFileSelectedForRestore)
				{
					try
					{
						text = Path.Combine(componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameTarget);
					}
					catch (ArgumentException arg)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc: {0} Target EDB file name {1} {2} contains invalid path characters.", arg, componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameTarget);
						return <Module>.HRESULT_FROM_WIN32(87);
					}
					if (File.Exists(text))
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc: EDB file {0} exists.", text);
						return <Module>.HRESULT_FROM_WIN32(1610);
					}
				}
				num = this.HrVerifyExistingLogFiles(componentInformation);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc: HrVerifyExistingLogFiles failed with {0:X8} ");
					object[] messageArgs3 = new object[]
					{
						componentInformation.m_displayNameTarget,
						componentInformation.m_logPathTarget,
						num
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterFindLogFilesError.LogEvent(string.Empty, messageArgs3);
					return num;
				}
			}
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyAlternateLoc -- {0:X8}", num);
			return num;
		}

		private int HrCheckRestoreEnv(ComponentInformation componentInformation)
		{
			new object[0];
			int num = 0;
			bool flag = false;
			int result;
			try
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCheckRestoreEnv");
				int num2;
				try
				{
					object[] array = new object[0];
					null == componentInformation.m_restoreEnv;
					componentInformation.m_restoreEnv = Path.Combine(componentInformation.m_logPathTarget, componentInformation.m_logBaseName + ComponentInformation.RestoreEnv);
				}
				catch (ArgumentException ex)
				{
					object[] args = new object[]
					{
						ex,
						componentInformation.m_edbLocationTarget,
						componentInformation.m_edbFilenameTarget,
						ComponentInformation.RestoreEnv
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCheckRestoreEnv: {0} Restore file name {1} {2} {3} contains invalid path characters.", args);
					num = <Module>.HRESULT_FROM_WIN32(87);
					num2 = num;
					goto IL_136;
				}
				object[] array2 = new object[0];
				null != componentInformation.m_restoreEnv;
				if (File.Exists(componentInformation.m_restoreEnv))
				{
					num = this.HrVerifyRestoreEnvironment(componentInformation);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCheckRestoreEnv: HrVerifyRestoreEnvironment failed with {0:X8}", num);
						return num;
					}
				}
				else if (componentInformation.m_fPostRestore)
				{
					num = this.HrGenerateRestoreEnvironment(componentInformation);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCheckRestoreEnv: HrGenerateRestoreEnvironment failed with {0:X8} ");
						return num;
					}
				}
				flag = true;
				return num;
				IL_136:
				result = num2;
			}
			finally
			{
				if (!flag)
				{
					componentInformation.m_fRunRecovery = false;
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrCheckRestoreEnv -- {0:X8}", num);
			}
			return result;
		}

		private int HrAdditionalPostRestoreTasks(ComponentInformation componentInformation)
		{
			string text = null;
			string text2 = null;
			string name = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			string text6 = null;
			string text7 = null;
			string text8 = null;
			string text9 = null;
			new object[0];
			new object[0];
			int num = 0;
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks");
			object[] args = new object[]
			{
				componentInformation.m_displayNameTarget,
				componentInformation.m_rstscen,
				componentInformation.m_fSubComponentsExplicitlySelected,
				componentInformation.m_fDatabaseFileSelectedForRestore,
				componentInformation.m_fLogsSelectedForRestore,
				componentInformation.m_fEDBRenamed,
				componentInformation.m_fRunRecovery
			};
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Target {0}, restore scenario {1}, fSubComponentsExplicitlySelected {2}, fDatabaseFileSelectedForRestore {3}, fLogsSelectedForRestore {4}, fEDBRenamed {5}, fRunRecovery {6}.", args);
			VssRestoreScenario rstscen = componentInformation.m_rstscen;
			if (VssRestoreScenario.rstscenOriginalDB != rstscen && VssRestoreScenario.rstscenAlternateDB != rstscen)
			{
				if (VssRestoreScenario.rstscenAlternateLoc == rstscen)
				{
					new object[0];
					string.IsNullOrEmpty(componentInformation.m_displayNameTarget);
					string restoreEnv = componentInformation.m_restoreEnv;
					name = restoreEnv;
					text3 = restoreEnv;
				}
				else
				{
					new object[0];
				}
			}
			else
			{
				if (componentInformation.m_fDatabaseFileSelectedForRestore)
				{
					if (componentInformation.m_fEDBRenamed)
					{
						new object[0];
						string edbFilenameTarget = componentInformation.m_edbFilenameTarget;
						string.Equals(componentInformation.m_edbFilenameOriginal, edbFilenameTarget, StringComparison.InvariantCultureIgnoreCase);
						try
						{
							text = Path.Combine(componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameOriginal);
						}
						catch (ArgumentException arg)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Original EDB path {1} {2} contains invalid path characters.", arg, componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameOriginal);
							return <Module>.HRESULT_FROM_WIN32(87);
						}
						try
						{
							text2 = Path.Combine(componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameTarget);
						}
						catch (ArgumentException arg2)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Original EDB path {1} {2} contains invalid path characters.", arg2, componentInformation.m_edbLocationTarget, componentInformation.m_edbFilenameTarget);
							return <Module>.HRESULT_FROM_WIN32(87);
						}
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string>((long)this.GetHashCode(), "EDB path original {0}, EDB path target {1} .", text, text2);
						num = this.HrDeleteFile(text2);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteFile failed with {0:X8}", num);
							return num;
						}
						num = this.HrMoveFile(text, text2);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrMoveFile failed with {0:X8}", num);
							object[] messageArgs = new object[]
							{
								num,
								text,
								text2
							};
							ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRenameDbError.LogEvent(string.Empty, messageArgs);
							return num;
						}
						goto IL_2B2;
					}
					new object[0];
					string edbFilenameTarget2 = componentInformation.m_edbFilenameTarget;
					string.Equals(componentInformation.m_edbFilenameOriginal, edbFilenameTarget2, StringComparison.InvariantCultureIgnoreCase);
				}
				IL_2B2:
				name = ((Guid)componentInformation.m_guidDBTarget).ToString();
				text3 = componentInformation.m_displayNameTarget;
			}
			if (componentInformation.m_fRunRecovery)
			{
				object[] messageArgs2 = new object[]
				{
					text3,
					componentInformation.m_restoreEnv
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRecoveryAfterRestore.LogEvent(string.Empty, messageArgs2);
				JET_RSTMAP[] array = new JET_RSTMAP[]
				{
					new JET_RSTMAP()
				};
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Restore environment for {0}: {1}.", componentInformation.m_displayNameTarget, componentInformation.m_restoreEnvXml);
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					object[] array2 = new object[0];
					null != componentInformation.m_restoreEnvXml;
					xmlDocument.LoadXml(componentInformation.m_restoreEnvXml);
				}
				catch (XmlException)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: Invalid restore environment information {0}.", componentInformation.m_restoreEnv);
					return <Module>.HRESULT_FROM_WIN32(13);
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbLocationOriginal, ref text4);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbFilenameOriginal, ref text5);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbLocationTarget, ref text6);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.EdbFilenameTarget, ref text7);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				object[] messageArgs3 = new object[]
				{
					text3,
					text7
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterDbToRecover.LogEvent(string.Empty, messageArgs3);
				try
				{
					array[0].szDatabaseName = Path.Combine(text4, text5);
				}
				catch (ArgumentException arg3)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Original EDB path {1} {2} contains invalid path characters.", arg3, text4, text5);
					return <Module>.HRESULT_FROM_WIN32(87);
				}
				try
				{
					array[0].szNewDatabaseName = Path.Combine(text6, text7);
				}
				catch (ArgumentException arg4)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Original EDB path {1} {2} contains invalid path characters.", arg4, text6, text7);
					return <Module>.HRESULT_FROM_WIN32(87);
				}
				try
				{
					text8 = Path.Combine(componentInformation.m_systemPathTarget, componentInformation.m_logBaseName + ComponentInformation.CheckpointExtension);
				}
				catch (ArgumentException ex)
				{
					object[] args2 = new object[]
					{
						ex,
						componentInformation.m_systemPathTarget,
						componentInformation.m_logBaseName,
						ComponentInformation.CheckpointExtension
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Check point file  path {1} {2} {3} contains invalid path characters.", args2);
					return <Module>.HRESULT_FROM_WIN32(87);
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Edb location original {0}, Edb location target {1}, chpckpoint file {2}.", array[0].szDatabaseName, array[0].szNewDatabaseName, text8);
				if (componentInformation.m_fIncrementalRestore || VssRestoreScenario.rstscenOriginalDB != componentInformation.m_rstscen || (componentInformation.m_fSubComponentsExplicitlySelected && !componentInformation.m_fLogsSelectedForRestore))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Deleting checkpoint file {0} .", text8);
					num = this.HrDeleteFile(text8);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteFile failed with {0:X8}", num);
						object[] messageArgs4 = new object[]
						{
							text8,
							num
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterChkptNotDeleted.LogEvent(string.Empty, messageArgs4);
						return num;
					}
				}
				if (!componentInformation.m_fCircularLoggingInBackupSet && !componentInformation.m_fCircularLoggingInDBTarget)
				{
					try
					{
						text9 = Path.Combine(componentInformation.m_logPathTarget, componentInformation.m_logBaseName);
					}
					catch (ArgumentException arg5)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<ArgumentException, string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Target log path {1} {2} contains invalid path characters.", arg5, componentInformation.m_logPathTarget, componentInformation.m_logBaseName);
						return <Module>.HRESULT_FROM_WIN32(87);
					}
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Perform logfile integrity check on {0} .", text9);
					string displayName = "Logfile Integrity-Check " + text9;
					bool flag = false;
					JET_INSTANCE instance = default(JET_INSTANCE);
					try
					{
						int result;
						try
						{
							Api.JetCreateInstance2(out instance, name, displayName, CreateInstanceGrbit.None);
							InstanceParameters instanceParameters = new InstanceParameters(instance);
							instanceParameters.Recovery = false;
							instanceParameters.MaxTemporaryTables = 0;
							instanceParameters.BaseName = componentInformation.m_logBaseName;
							Api.JetInit(ref instance);
							flag = true;
							JET_SESID sesid = default(JET_SESID);
							Api.JetBeginSession(instance, out sesid, null, null);
							UnpublishedApi.JetDBUtilities(new JET_DBUTIL
							{
								sesid = sesid,
								op = DBUTIL_OP.DumpLogfile,
								szDatabase = text9
							});
						}
						catch (EsentErrorException arg6)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<EsentErrorException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Log file integrity check failed.", arg6);
							num = <Module>.HRESULT_FROM_WIN32(13);
							result = num;
							goto IL_736;
						}
						goto IL_751;
						IL_736:
						return result;
					}
					finally
					{
						if (flag)
						{
							Api.JetTerm2(instance, TermGrbit.Complete);
						}
					}
					int result2;
					return result2;
				}
				IL_751:
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Perform recovery on {0} .", array[0].szNewDatabaseName);
				bool flag2 = false;
				bool flag3 = false;
				JET_INSTANCE instance2 = default(JET_INSTANCE);
				try
				{
					int result3;
					try
					{
						Api.JetCreateInstance2(out instance2, name, text3, CreateInstanceGrbit.None);
						InstanceParameters instanceParameters2 = new InstanceParameters(instance2);
						instanceParameters2.Recovery = true;
						instanceParameters2.SystemDirectory = componentInformation.m_systemPathTarget;
						instanceParameters2.LogFileDirectory = componentInformation.m_logPathTarget;
						instanceParameters2.BaseName = componentInformation.m_logBaseName;
						instanceParameters2.MaxTemporaryTables = 0;
						JET_SESID nil = JET_SESID.Nil;
						Api.JetSetSystemParameter(instance2, nil, (JET_param)168, 1, null);
						if (componentInformation.m_fCircularLoggingInBackupSet || componentInformation.m_fCircularLoggingInDBTarget)
						{
							JET_SESID nil2 = JET_SESID.Nil;
							Api.JetSetSystemParameter(instance2, nil2, JET_param.DeleteOutOfRangeLogs, 1, null);
						}
						JET_SESID nil3 = JET_SESID.Nil;
						Api.JetSetSystemParameter(instance2, nil3, JET_param.CacheSizeMax, 8000, null);
						this.AddToRestoreInProgress(componentInformation);
						<Module>.CReplicaVssWriter.UnLockObj(this.m_replicaWriter);
						flag3 = true;
						VistaApi.JetInit3(ref instance2, new JET_RSTINFO
						{
							rgrstmap = array,
							crstmap = 1
						}, InitGrbit.None);
						flag2 = true;
					}
					catch (EsentErrorException arg7)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<EsentErrorException>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: {0} Database recovery failed.", arg7);
						num = <Module>.HRESULT_FROM_WIN32(13);
						result3 = num;
						goto IL_884;
					}
					goto IL_8B3;
					IL_884:
					return result3;
				}
				finally
				{
					if (flag3)
					{
						<Module>.CReplicaVssWriter.LockObj(this.m_replicaWriter);
						this.RemoveFromRestoreInProgress(componentInformation);
					}
					if (flag2)
					{
						Api.JetTerm2(instance2, TermGrbit.Complete);
					}
				}
				IL_8B3:
				bool flag4 = false;
				if (VssRestoreScenario.rstscenOriginalDB != componentInformation.m_rstscen)
				{
					new object[0];
					string.IsNullOrEmpty(text8);
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Delete checkpoint file {0} .", text8);
					int num2 = this.HrDeleteFile(text8);
					if (num2 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteFile failed with {0:X8}", num2);
						object[] messageArgs5 = new object[]
						{
							text8,
							num2
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterChkptNotDeleted.LogEvent(string.Empty, messageArgs5);
						flag4 = true;
					}
					string text10 = componentInformation.m_logBaseName + ComponentInformation.LogsWildCard;
					int num3 = this.HrDeleteTargetLogFiles(componentInformation, text10);
					if (num3 < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteLogFiles failed to delete the files {0} with {1:X8}", text10, num3);
						object[] messageArgs6 = new object[]
						{
							text10,
							componentInformation.m_logPathTarget,
							num3
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterLogsNotDeleted.LogEvent(string.Empty, messageArgs6);
						flag4 = true;
					}
					string text11 = componentInformation.m_logBaseName + ComponentInformation.ReservedLogsWildCard;
					num = this.HrDeleteTargetLogFiles(componentInformation, text11);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteLogFiles failed to delete the files {0} with {1:X8}", text11, num);
						object[] messageArgs7 = new object[]
						{
							text11,
							componentInformation.m_logPathTarget,
							num
						};
						ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterLogsNotDeleted.LogEvent(string.Empty, messageArgs7);
						num = 0;
						goto IL_AF2;
					}
					if (flag4)
					{
						goto IL_AF2;
					}
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<string>((long)this.GetHashCode(), "Delete restore environment file {0} .", componentInformation.m_restoreEnv);
				num = this.HrDeleteFile(componentInformation.m_restoreEnv);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks: HrDeleteFile failed with {0:X8}", num);
					object[] messageArgs8 = new object[]
					{
						componentInformation.m_restoreEnv,
						num
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvNotDeleted.LogEvent(string.Empty, messageArgs8);
					num = 0;
				}
				goto IL_AF2;
			}
			if (componentInformation.m_fAdditionalRestores)
			{
				object[] messageArgs9 = new object[]
				{
					text3
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterAdditionalRestoresPending.LogEvent(string.Empty, messageArgs9);
			}
			else
			{
				object[] messageArgs10 = new object[]
				{
					text3
				};
				ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterNoDatabasesToRecover.LogEvent(string.Empty, messageArgs10);
			}
			IL_AF2:
			ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrAdditionalPostRestoreTasks -- {0:X8}", num);
			return num;
		}

		private int HrVerifyRestoreEnvironment(ComponentInformation componentInformation)
		{
			ValueType valueType = null;
			string text = null;
			string text2 = null;
			string text3 = null;
			new object[0];
			new object[0];
			int num = 0;
			bool flag = false;
			int result;
			try
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment");
				componentInformation.m_fIncrementalRestore = true;
				XmlDocument xmlDocument = new XmlDocument();
				int num2;
				try
				{
					object[] array = new object[0];
					null != componentInformation.m_restoreEnv;
					xmlDocument.Load(componentInformation.m_restoreEnv);
				}
				catch (XmlException)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Invalid restore environment file {0}.", componentInformation.m_restoreEnv);
					num = <Module>.HRESULT_FROM_WIN32(13);
					num2 = num;
					goto IL_807;
				}
				num = this.HrRetrieveXmlGuid(xmlDocument, ComponentInformation.DatabaseGuidTarget, ref valueType);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlGuid for {0} failed with {1:X8}", ComponentInformation.DatabaseGuidTarget, num);
					return num;
				}
				if (!((Guid)componentInformation.m_guidDBTarget).Equals(valueType))
				{
					object[] args = new object[]
					{
						valueType,
						componentInformation.m_guidDBTarget
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Restore environment target database Guid mismatch. Restore environment value {0}, component information value {1}.", args);
					object[] messageArgs = new object[]
					{
						componentInformation.m_guidDBTarget,
						valueType,
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvSGMismatchError.LogEvent(string.Empty, messageArgs);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.LogPathTarget, ref text);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				char[] trimChars = new char[]
				{
					' ',
					'\\'
				};
				text = text.TrimEnd(trimChars);
				char[] trimChars2 = new char[]
				{
					' ',
					'\\'
				};
				string text4 = componentInformation.m_logPathTarget.TrimEnd(trimChars2);
				if (!this.StringEquals(text4, text))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Restore environment target log path mismatch. Restore environment value {0}, component information value {1}.", text, text4);
					object[] messageArgs2 = new object[]
					{
						text4,
						text,
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvLogfilePathMismatchError.LogEvent(string.Empty, messageArgs2);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.LogBaseName, ref text2);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				if (!this.StringEquals(componentInformation.m_logBaseName, text2))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Restore environment log base name mismatch. Restore environment value {0}, component information value {1}.", text2, componentInformation.m_logBaseName);
					object[] messageArgs3 = new object[]
					{
						componentInformation.m_logBaseName,
						text2,
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvLogfileBaseNameMismatchError.LogEvent(string.Empty, messageArgs3);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				uint num3;
				num = this.HrRetrieveXmlUInt32(xmlDocument, ComponentInformation.LogSignatureId, ref num3);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlUInt32 for {0} failed with {1:X8}", ComponentInformation.LogSignatureId, num);
					return num;
				}
				ulong num4;
				num = this.HrRetrieveXmlUInt64(xmlDocument, ComponentInformation.LogSignatureTimestamp, ref num4);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlDateTime for {0} failed with {1:X8}", ComponentInformation.LogSignatureTimestamp, num);
					return num;
				}
				JET_LOGTIME logtimeCreate = componentInformation.m_signLog.logtimeCreate;
				if (componentInformation.m_signLog.ulRandom != num3 || logtimeCreate.ToUint64() != num4)
				{
					object[] args2 = new object[]
					{
						num3,
						num4,
						componentInformation.m_signLog.ulRandom,
						componentInformation.m_signLog.logtimeCreate
					};
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Restore environment log signature mismatch. Restore environment value {0}:{1} component information value {2}:{3}", args2);
					object[] messageArgs4 = new object[]
					{
						componentInformation.m_signLog.ulRandom,
						componentInformation.m_signLog.logtimeCreate,
						num3,
						num4,
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvLogfileSignatureMismatchError.LogEvent(string.Empty, messageArgs4);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				num = this.HrRetrieveXmlString(xmlDocument, ComponentInformation.SystemPathTarget, ref text3);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlString failed with {0:X8}", num);
					return num;
				}
				char[] trimChars3 = new char[]
				{
					' ',
					'\\'
				};
				text3 = text3.TrimEnd(trimChars3);
				char[] trimChars4 = new char[]
				{
					' ',
					'\\'
				};
				string text5 = componentInformation.m_systemPathTarget.TrimEnd(trimChars4);
				if (!this.StringEquals(text5, text3))
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Restore environment destination system path mismatch. Restore environment value {0} , component information value {1} .", text3, text5);
					object[] messageArgs5 = new object[]
					{
						text5,
						text3,
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvSystemPathMismatchError.LogEvent(string.Empty, messageArgs5);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				bool flag2;
				num = this.HrRetrieveXmlBool(xmlDocument, ComponentInformation.CircularLogging, ref flag2);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlBool failed with {0:X8}", num);
					return num;
				}
				if (flag2)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: Cannot add additional restores to a restored backup set where circular logging was enabled.", text3, componentInformation.m_systemPathTarget);
					object[] messageArgs6 = new object[]
					{
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvCircularLogEnabledError.LogEvent(string.Empty, messageArgs6);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				bool flag3;
				num = this.HrRetrieveXmlBool(xmlDocument, ComponentInformation.Recovery, ref flag3);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrRetrieveXmlBool failed with {0:X8}", num);
					return num;
				}
				if (flag3)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<string, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment:  Cannot add additional restore after recovery has been run on {0} .", text3, componentInformation.m_systemPathTarget);
					object[] messageArgs7 = new object[]
					{
						componentInformation.m_restoreEnv
					};
					ReplayEventLogConstants.Tuple_ReplayServiceVSSWriterRestoreEnvAlreadyRecoveredError.LogEvent(string.Empty, messageArgs7);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					return num;
				}
				if (componentInformation.m_fPostRestore)
				{
					string newValue;
					if (componentInformation.m_fRunRecovery)
					{
						newValue = ComponentInformation.YES;
					}
					else
					{
						newValue = ComponentInformation.NO;
					}
					num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.Recovery, newValue);
					if (num < 0)
					{
						ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
						return num;
					}
					if (componentInformation.m_fDatabaseFileSelectedForRestore)
					{
						num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.EdbLocationOriginal, componentInformation.m_edbLocationOriginal);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
							return num;
						}
						num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.EdbLocationTarget, componentInformation.m_edbLocationTarget);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
							return num;
						}
						num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.EdbFilenameOriginal, componentInformation.m_edbFilenameOriginal);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
							return num;
						}
						num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.EdbFilenameTarget, componentInformation.m_edbFilenameTarget);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
							return num;
						}
						string newValue2;
						if (componentInformation.m_fPrivateMdb)
						{
							newValue2 = ComponentInformation.YES;
						}
						else
						{
							newValue2 = ComponentInformation.NO;
						}
						num = this.HrUpdateXmlNode(xmlDocument, ComponentInformation.PrivateMdb, newValue2);
						if (num < 0)
						{
							ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrUpdateXmlNode failed with {0:X8}", num);
							return num;
						}
					}
				}
				num = this.HrSaveRestoreEnvironmentXml(componentInformation, xmlDocument);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment: HrSaveRestoreEnvironmentXml failed with {0:X8}", num);
					return num;
				}
				flag = true;
				return num;
				IL_807:
				result = num2;
			}
			finally
			{
				if (!flag)
				{
					componentInformation.m_fRunRecovery = false;
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrVerifyRestoreEnvironment -- {0:X8}", num);
			}
			return result;
		}

		private int HrGenerateRestoreEnvironment(ComponentInformation componentInformation)
		{
			XmlDocument xmlDocument = null;
			new object[0];
			new object[0];
			null != componentInformation.m_restoreEnv;
			int num = 0;
			bool flag = false;
			int result;
			try
			{
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug((long)this.GetHashCode(), "Enter Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGenerateRestoreEnvironment");
				int num2;
				try
				{
					xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(ComponentInformation.RestoreEnvironment);
					XmlElement newChild = xmlDocument.CreateElement(ComponentInformation.VersionStamp);
					XmlText newChild2 = xmlDocument.CreateTextNode("15.00.1497.015");
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.DatabaseGuidOriginal);
					newChild2 = xmlDocument.CreateTextNode(((Guid)componentInformation.m_guidDBOriginal).ToString());
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.DatabaseGuidTarget);
					newChild2 = xmlDocument.CreateTextNode(((Guid)componentInformation.m_guidDBTarget).ToString());
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					if (componentInformation.m_fDatabaseFileSelectedForRestore)
					{
						newChild = xmlDocument.CreateElement(ComponentInformation.EdbLocationOriginal);
						newChild2 = xmlDocument.CreateTextNode(componentInformation.m_edbLocationOriginal);
						xmlDocument.DocumentElement.AppendChild(newChild);
						xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
						newChild = xmlDocument.CreateElement(ComponentInformation.EdbLocationTarget);
						newChild2 = xmlDocument.CreateTextNode(componentInformation.m_edbLocationTarget);
						xmlDocument.DocumentElement.AppendChild(newChild);
						xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
						newChild = xmlDocument.CreateElement(ComponentInformation.EdbFilenameOriginal);
						newChild2 = xmlDocument.CreateTextNode(componentInformation.m_edbFilenameOriginal);
						xmlDocument.DocumentElement.AppendChild(newChild);
						xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
						newChild = xmlDocument.CreateElement(ComponentInformation.EdbFilenameTarget);
						newChild2 = xmlDocument.CreateTextNode(componentInformation.m_edbFilenameTarget);
						xmlDocument.DocumentElement.AppendChild(newChild);
						xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
						newChild = xmlDocument.CreateElement(ComponentInformation.PrivateMdb);
						string text;
						if (componentInformation.m_fPrivateMdb)
						{
							text = ComponentInformation.YES;
						}
						else
						{
							text = ComponentInformation.NO;
						}
						newChild2 = xmlDocument.CreateTextNode(text);
						xmlDocument.DocumentElement.AppendChild(newChild);
						xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					}
					else
					{
						componentInformation.m_fRunRecovery = false;
					}
					newChild = xmlDocument.CreateElement(ComponentInformation.LogSignatureId);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_signLog.ulRandom.ToString());
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.LogSignatureTimestamp);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_signLog.logtimeCreate.ToUint64().ToString());
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.LogBaseName);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_logBaseName);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.LogPathOriginal);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_logPathOriginal);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.LogPathTarget);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_logPathTarget);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.SystemPathOriginal);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_systemPathOriginal);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.SystemPathTarget);
					newChild2 = xmlDocument.CreateTextNode(componentInformation.m_systemPathTarget);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.CircularLogging);
					string text2;
					if (componentInformation.m_fCircularLoggingInBackupSet)
					{
						text2 = ComponentInformation.YES;
					}
					else
					{
						text2 = ComponentInformation.NO;
					}
					newChild2 = xmlDocument.CreateTextNode(text2);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
					newChild = xmlDocument.CreateElement(ComponentInformation.Recovery);
					string text3;
					if (componentInformation.m_fRunRecovery)
					{
						text3 = ComponentInformation.YES;
					}
					else
					{
						text3 = ComponentInformation.NO;
					}
					newChild2 = xmlDocument.CreateTextNode(text3);
					xmlDocument.DocumentElement.AppendChild(newChild);
					xmlDocument.DocumentElement.LastChild.AppendChild(newChild2);
				}
				catch (InvalidOperationException arg)
				{
					componentInformation.m_fRunRecovery = false;
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<InvalidOperationException, string>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGenerateRestoreEnvironment: {0} Cannot generate restore environment file {1}.", arg, componentInformation.m_restoreEnv);
					num = <Module>.HRESULT_FROM_WIN32(1610);
					num2 = num;
					goto IL_4FF;
				}
				num = this.HrSaveRestoreEnvironmentXml(componentInformation, xmlDocument);
				if (num < 0)
				{
					ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceError<int>((long)this.GetHashCode(), "Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGenerateRestoreEnvironment: HrSaveRestoreEnvironmentXml failed with {0:X8}", num);
					return num;
				}
				flag = true;
				return num;
				IL_4FF:
				result = num2;
			}
			finally
			{
				if (!flag)
				{
					componentInformation.m_fRunRecovery = false;
				}
				ExTraceGlobals.ReplicaVssWriterInteropTracer.TraceDebug<int>((long)this.GetHashCode(), "Leave Microsoft::Exchange::Cluster::ReplicaVssWriter::CReplicaVssWriterInterop::HrGenerateRestoreEnvironment -- {0:X8}", num);
			}
			return result;
		}

		private static void Assert([MarshalAs(UnmanagedType.U1)] bool condition, string formatString, params object[] variableArgs)
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CReplicaVssWriterInterop()
		{
			CReplicaVssWriterInterop.E_EXCEPTION = -3;
		}

		private static int E_EXCEPTION = -3;

		private static int E_BACKUPFAILED = -4;

		private bool m_fValidObj;

		private unsafe CReplicaVssWriter* m_replicaWriter;

		private IReplicaInstanceManager m_replicaInstanceManager;

		private IdentifyDelegate m_populateMetadataDelegate;

		private PrepareBackupDelegate m_prepareBackupDelegate;

		private PrepareSnapshotDelegate m_prepareSnapshotDelegate;

		private FreezeOrThawDelegate m_freezeOrThawDelegate;

		private AbortDelegate m_abortDelegate;

		private PostSnapshotDelegate m_postSnapshotDelegate;

		private BackupCompleteDelegate m_backupCompleteDelegate;

		private ShutdownBackupDelegate m_shutdownBackupDelegate;

		private PreRestoreDelegate m_preRestoreDelegate;

		private PostRestoreDelegate m_postRestoreDelegate;

		private method m_pfnIdentify;

		private method m_pfnPrepareBackup;

		private method m_pfnPrepareSnapshot;

		private method m_pfnFreezeOrThaw;

		private method m_pfnAbort;

		private method m_pfnPostSnapshot;

		private method m_pfnBackupComplete;

		private method m_pfnShutdownBackup;

		private method m_pfnPreRestore;

		private method m_pfnPostRestore;

		private List<ComponentInformation> m_restoreInProgressComponents;

		private Dictionary<Guid, BackupInstance> m_backupInstances;
	}
}
