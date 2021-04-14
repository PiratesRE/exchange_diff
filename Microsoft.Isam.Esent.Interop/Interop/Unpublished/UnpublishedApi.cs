using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Isam.Esent.Interop.Implementation;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public static class UnpublishedApi
	{
		public static JET_DBID AllDatabases
		{
			get
			{
				return new JET_DBID
				{
					Value = 2147483647U
				};
			}
		}

		public static void JetTracing(JET_traceop operation, JET_tracetag tag, bool value)
		{
			Api.Check(Api.Impl.JetTracing(operation, tag, value));
		}

		public static void JetTracing(JET_traceop operation, JET_tracetag tag, JET_DBID value)
		{
			Api.Check(Api.Impl.JetTracing(operation, tag, value));
		}

		public static void JetTracing(JET_traceop operation, JET_tracetag tag, int value)
		{
			Api.Check(Api.Impl.JetTracing(operation, tag, value));
		}

		public static void JetTracing(JET_traceop operation, JET_tracetag tag, JET_PFNTRACEREGISTER callback)
		{
			Api.Check(Api.Impl.JetTracing(operation, tag, callback));
		}

		public static void JetTracing(JET_traceop operation, JET_tracetag tag, JET_PFNTRACEEMIT callback)
		{
			Api.Check(Api.Impl.JetTracing(operation, tag, callback));
		}

		public static void JetBeginDatabaseIncrementalReseed(JET_INSTANCE instance, string wszDatabase, BeginDatabaseIncrementalReseedGrbit grbit)
		{
			Api.Check(Api.Impl.JetBeginDatabaseIncrementalReseed(instance, wszDatabase, grbit));
		}

		public static void JetEndDatabaseIncrementalReseed(JET_INSTANCE instance, string wszDatabase, int genMinRequired, int genFirstDivergedLog, int genMaxRequired, EndDatabaseIncrementalReseedGrbit grbit)
		{
			Api.Check(Api.Impl.JetEndDatabaseIncrementalReseed(instance, wszDatabase, genMinRequired, genFirstDivergedLog, genMaxRequired, grbit));
		}

		public static void JetPatchDatabasePages(JET_INSTANCE instance, string databaseFileName, int pgnoStart, int pageCount, byte[] inputData, int dataLength, PatchDatabasePagesGrbit grbit)
		{
			Api.Check(Api.Impl.JetPatchDatabasePages(instance, databaseFileName, pgnoStart, pageCount, inputData, dataLength, grbit));
		}

		public static void JetRemoveLogfile(string databaseFileName, string logFileName, RemoveLogfileGrbit grbit)
		{
			Api.Check(Api.Impl.JetRemoveLogfile(databaseFileName, logFileName, grbit));
		}

		public static void JetGetDatabasePages(JET_SESID sesid, JET_DBID dbid, int pgnoStart, int countPages, byte[] pageBytes, int pageBytesLength, out int pageBytesRead, GetDatabasePagesGrbit grbit)
		{
			Api.Check(Api.Impl.JetGetDatabasePages(sesid, dbid, pgnoStart, countPages, pageBytes, pageBytesLength, out pageBytesRead, grbit));
		}

		public static void JetSetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, long longParameter)
		{
			Api.Check(Api.Impl.JetSetResourceParam(instance, resoper, resid, longParameter));
		}

		public static void JetGetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, out long longParameter)
		{
			Api.Check(Api.Impl.JetGetResourceParam(instance, resoper, resid, out longParameter));
		}

		public static void JetConsumeLogData(JET_INSTANCE instance, JET_EMITDATACTX emitLogDataCtx, byte[] logDataBuf, int logDataStartOffset, int logDataLength, ShadowLogConsumeGrbit grbit)
		{
			Api.Check(Api.Impl.JetConsumeLogData(instance, emitLogDataCtx, logDataBuf, logDataStartOffset, logDataLength, grbit));
		}

		public static void JetGetLogFileInfo(string logFileName, out JET_LOGINFOMISC info, JET_LogInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetLogFileInfo(logFileName, out info, infoLevel));
		}

		public static void JetGetPageInfo2(byte[] rawPageData, int pageDataLength, JET_PAGEINFO[] pageInfos, PageInfoGrbit grbit, JET_PageInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetPageInfo2(rawPageData, pageDataLength, pageInfos, grbit, infoLevel));
		}

		public static void JetGetInstanceMiscInfo(JET_INSTANCE instance, out JET_CHECKPOINTINFO checkpointInfo, JET_InstanceMiscInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetInstanceMiscInfo(instance, out checkpointInfo, infoLevel));
		}

		public static void JetDBUtilities(JET_DBUTIL dbutil)
		{
			Api.Check(Api.Impl.JetDBUtilities(dbutil));
		}

		public static void JetTestHook(TestHookOp opcode, TestHookNegativeTestingFlags flags)
		{
			uint num = (uint)flags;
			Api.Check(Api.Impl.JetTestHook((int)opcode, ref num));
		}

		public static void JetTestHook(TestHookOp opcode, JET_TESTHOOKTESTINJECTION testInjection)
		{
			NATIVE_TESTHOOKTESTINJECTION nativeTestHookInjection = testInjection.GetNativeTestHookInjection();
			Api.Check(Api.Impl.JetTestHook((int)opcode, ref nativeTestHookInjection));
		}

		public static void JetTestHook(TestHookOp opcode, JET_TESTHOOKEVICTCACHE testEvictCachedPage)
		{
			Api.Check(Api.Impl.JetTestHook((int)opcode, testEvictCachedPage));
		}

		public static void JetTestHook(TestHookOp opcode, ref IntPtr pv)
		{
			Api.Check(Api.Impl.JetTestHook((int)opcode, ref pv));
		}

		public static void JetTestHook(TestHookOp opcode, ref uint pv)
		{
			Api.Check(Api.Impl.JetTestHook((int)opcode, ref pv));
		}

		public static void JetTestHook(TestHookOp opcode, JET_TESTHOOKTRACETESTMARKER traceTestMarker)
		{
			GCHandleCollection gchandleCollection = default(GCHandleCollection);
			try
			{
				NATIVE_TESTHOOKTRACETESTMARKER nativeTraceTestMarker = traceTestMarker.GetNativeTraceTestMarker(ref gchandleCollection);
				Api.Check(Api.Impl.JetTestHook((int)opcode, ref nativeTraceTestMarker));
			}
			finally
			{
				gchandleCollection.Dispose();
			}
		}

		public static void JetTestHook(TestHookOp opcode, JET_TESTHOOKCORRUPT corruptData)
		{
			GCHandleCollection gchandleCollection = default(GCHandleCollection);
			try
			{
				NATIVE_TESTHOOKCORRUPT_DATABASEFILE nativeCorruptDatabaseFile = corruptData.GetNativeCorruptDatabaseFile(ref gchandleCollection);
				Api.Check(Api.Impl.JetTestHook((int)opcode, ref nativeCorruptDatabaseFile));
			}
			finally
			{
				gchandleCollection.Dispose();
			}
		}

		public static void JetPrereadTables(JET_SESID sesid, JET_DBID dbid, string[] rgsz, PrereadTablesGrbit grbit)
		{
			Api.Check(Api.Impl.JetPrereadTables(sesid, dbid, rgsz, grbit));
		}

		public static JET_wrn JetDatabaseScan(JET_SESID sesid, JET_DBID dbid, ref int seconds, int sleepInMsec, JET_CALLBACK callback, DatabaseScanGrbit grbit)
		{
			return Api.Check(Api.Impl.JetDatabaseScan(sesid, dbid, ref seconds, sleepInMsec, callback, grbit));
		}

		public static void JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, int value)
		{
			Api.Check(Api.Impl.JetSetSessionParameter(sesid, sesparamid, value));
		}

		public static void JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, JET_OPERATIONCONTEXT operationContext)
		{
			NATIVE_OPERATIONCONTEXT nativeOperationContext = operationContext.GetNativeOperationContext();
			Api.Check(Api.Impl.JetSetSessionParameter(sesid, sesparamid, ref nativeOperationContext));
		}

		public static void TestHookEnableConfigOverrideInjection(int testHookIdentifier, IntPtr testHookValue, int probability, TestInjectionGrbit grbit)
		{
			UnpublishedApi.JetTestHook(TestHookOp.TestInjection, new JET_TESTHOOKTESTINJECTION
			{
				type = TestHookInjectionType.ConfigOverride,
				pv = testHookValue,
				ulID = (uint)testHookIdentifier,
				ulProbability = (uint)probability,
				grbit = grbit
			});
		}

		public static void TestHookEnableFaultInjection(int testHookIdentifier, JET_err testHookValue, int probability, TestInjectionGrbit grbit)
		{
			UnpublishedApi.JetTestHook(TestHookOp.TestInjection, new JET_TESTHOOKTESTINJECTION
			{
				type = TestHookInjectionType.Fault,
				pv = new IntPtr((int)testHookValue),
				ulID = (uint)testHookIdentifier,
				ulProbability = (uint)probability,
				grbit = grbit
			});
		}

		public static void TestHookEvictCachedPage(JET_DBID dbid, int pgnoToEvict)
		{
			UnpublishedApi.JetTestHook(TestHookOp.EvictCache, new JET_TESTHOOKEVICTCACHE
			{
				ulTargetContext = dbid,
				ulTargetData = pgnoToEvict,
				grbit = EvictCacheGrbit.EvictDataByPgno
			});
		}

		public static int GetLogGenFromCheckpointFileInfo(string checkpointFile)
		{
			return UnpublishedApi.GetCheckpointGeneration(checkpointFile);
		}

		public static void ChecksumLogFromMemory(JET_SESID sesid, string logFile, string basename, byte[] logBytes)
		{
			UnpublishedApi.CheckNotNull(logFile, "logFile");
			UnpublishedApi.CheckNotNull(basename, "basename");
			UnpublishedApi.CheckNotNull(logBytes, "logBytes");
			JET_DBUTIL dbutil = new JET_DBUTIL
			{
				op = DBUTIL_OP.ChecksumLogFromMemory,
				sesid = sesid,
				szLog = logFile,
				szBase = basename,
				pvBuffer = logBytes,
				cbBuffer = logBytes.Length
			};
			UnpublishedApi.JetDBUtilities(dbutil);
		}

		private static int GetCheckpointGeneration(string checkpointFile)
		{
			UnpublishedApi.CheckNotNull(checkpointFile, "checkpointFile");
			byte[] array = new byte[4096];
			using (FileStream fileStream = new FileStream(checkpointFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				fileStream.Read(array, 0, 4096);
			}
			UnpublishedApi.VerifyCheckpoint(array);
			return BitConverter.ToInt32(array, 16);
		}

		private static void VerifyCheckpoint(byte[] checkpointBytes)
		{
			if (4096 != checkpointBytes.Length)
			{
				throw new ArgumentException("VerifyCheckpoint() needs a 4k input buffer.");
			}
			uint num = BitConverter.ToUInt32(checkpointBytes, 0);
			uint num2 = UnpublishedApi.CalculateHeaderChecksum(checkpointBytes);
			if (num != num2)
			{
				throw new EsentCheckpointCorruptException();
			}
		}

		private static uint CalculateHeaderChecksum(byte[] headerBytes)
		{
			if (headerBytes.Length % 4096 != 0)
			{
				throw new ArgumentException("CalculateHeaderChecksum() needs an input buffer of a 4k-multiple.");
			}
			int num = headerBytes.Length;
			int num2 = 0;
			uint num3 = BitConverter.ToUInt32(headerBytes, num2);
			uint num4 = 2309737967U ^ num3;
			do
			{
				num4 ^= (BitConverter.ToUInt32(headerBytes, num2) ^ BitConverter.ToUInt32(headerBytes, num2 + 4) ^ BitConverter.ToUInt32(headerBytes, num2 + 8) ^ BitConverter.ToUInt32(headerBytes, num2 + 12) ^ BitConverter.ToUInt32(headerBytes, num2 + 16) ^ BitConverter.ToUInt32(headerBytes, num2 + 20) ^ BitConverter.ToUInt32(headerBytes, num2 + 24) ^ BitConverter.ToUInt32(headerBytes, num2 + 28));
				num -= 32;
				num2 += 32;
			}
			while (num > 0);
			return num4;
		}

		private static void CheckNotNull(object o, string paramName)
		{
			if (o == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT P/Invoke", "P/Invoke calls to ESENT");
	}
}
