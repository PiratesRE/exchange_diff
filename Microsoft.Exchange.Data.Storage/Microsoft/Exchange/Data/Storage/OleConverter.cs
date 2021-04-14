using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OleConverter : ComProcessManager<IOleConverter>
	{
		internal static OleConverter Instance
		{
			get
			{
				return OleConverter.InstanceCreator.Instance;
			}
		}

		internal OleConverter() : base(OleConverter.MaxWorkerNumber, OleConverter.WorkerConfigurationObject, ExTraceGlobals.CcOleTracer)
		{
			this.nQueueSize = 0;
			this.cleanupLock = new object();
			this.cleanupTimer = new Timer(new TimerCallback(OleConverter.DirectoryCleanup), this.cleanupLock, new TimeSpan(0, 0, 0), OleConverter.CleanupTimespan);
			this.CreateWorkerCallback = (ComProcessManager<IOleConverter>.OnCreateWorker)Delegate.Combine(this.CreateWorkerCallback, new ComProcessManager<IOleConverter>.OnCreateWorker(OleConverter.OnNewConverterCreatedCallback));
			this.DestroyWorkerCallback = (ComProcessManager<IOleConverter>.OnDestroyWorker)Delegate.Combine(this.DestroyWorkerCallback, new ComProcessManager<IOleConverter>.OnDestroyWorker(OleConverter.OnConverterDestroyedCallback));
			this.ExecuteRequestCallback = (ComProcessManager<IOleConverter>.OnExecuteRequest)Delegate.Combine(this.ExecuteRequestCallback, new ComProcessManager<IOleConverter>.OnExecuteRequest(OleConverter.OnExecuteRequestCallback));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OleConverter>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.cleanupTimer.Dispose();
			}
			base.InternalDispose(isDisposing);
		}

		private static void OnNewConverterCreatedCallback(IComWorker<IOleConverter> converterProcess, object requestParameters)
		{
			IOleConverter worker = converterProcess.Worker;
			uint num = 0U;
			string conversionsDirectory = OleConverter.GetConversionsDirectory(true);
			if (conversionsDirectory == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleConverter::OnNewConverterCreatedCallback: conversions directory full or inaccessible.");
				throw new OleConversionFailedException(ServerStrings.OleConversionFailed);
			}
			int num2;
			worker.ConfigureConverter(out num2, 4194304U, 262144U, conversionsDirectory, out num);
			if (num2 != 0)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleConverter::OnNewConverterCreatedCallback: failed to configure converter.");
				throw new OleConversionFailedException(ServerStrings.OleConversionFailed, new COMException("HRESULT =", num2));
			}
			if (num != (uint)converterProcess.ProcessId)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleConverter::OnNewConverterCreatedCallback: process id mismatch.");
				throw new OleConversionFailedException(ServerStrings.OleConversionInitError(Process.GetCurrentProcess().ProcessName, converterProcess.ProcessId, (int)num));
			}
		}

		private static void OnConverterDestroyedCallback(IComWorker<IOleConverter> converterProcess, object requestParameters, bool isForcedTermination)
		{
		}

		private static bool OnExecuteRequestCallback(IComWorker<IOleConverter> converterProcess, object requestParameters)
		{
			OleConverter.ConversionRequestParameters conversionRequestParameters = (OleConverter.ConversionRequestParameters)requestParameters;
			object responseData = null;
			int num;
			converterProcess.Worker.OleConvertToBmp(out num, conversionRequestParameters.RequestData, out responseData);
			if (num != 0)
			{
				throw new OleConversionFailedException(ServerStrings.OleConversionFailed, new COMException("HRESULT =", num));
			}
			conversionRequestParameters.ResponseData = responseData;
			return true;
		}

		private static string GetConversionsDirectory(bool checkIfFull)
		{
			return ConvertUtils.GetOleConversionsDirectory("Working\\OleConverter", checkIfFull);
		}

		internal Stream ConvertToBitmap(Stream oleDataStream)
		{
			object obj = null;
			int num = Interlocked.Increment(ref this.nQueueSize);
			Stream result;
			try
			{
				try
				{
					bool canCacheInMemory = num <= 30;
					obj = OleConverter.CreateOleObjectData(oleDataStream, canCacheInMemory);
					OleConverter.ConversionRequestParameters conversionRequestParameters = new OleConverter.ConversionRequestParameters(obj, null);
					base.ExecuteRequest(conversionRequestParameters);
					result = OleConverter.CreateResultStream(conversionRequestParameters.ResponseData);
				}
				catch (ComInterfaceInitializeException ex)
				{
					StorageGlobals.ContextTraceError<ComInterfaceInitializeException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex);
					throw new OleConversionServerBusyException(ServerStrings.OleConversionFailed, ex);
				}
				catch (ComProcessBusyException ex2)
				{
					StorageGlobals.ContextTraceError<ComProcessBusyException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex2);
					throw new OleConversionServerBusyException(ServerStrings.OleConversionFailed, ex2);
				}
				catch (ComProcessTimeoutException ex3)
				{
					StorageGlobals.ContextTraceError<ComProcessTimeoutException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex3);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex3);
				}
				catch (ComProcessBeyondMemoryLimitException ex4)
				{
					StorageGlobals.ContextTraceError<ComProcessBeyondMemoryLimitException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex4);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex4);
				}
				catch (COMException ex5)
				{
					StorageGlobals.ContextTraceError<COMException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex5);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex5);
				}
				catch (InvalidComObjectException ex6)
				{
					StorageGlobals.ContextTraceError<InvalidComObjectException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex6);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex6);
				}
				catch (InvalidCastException ex7)
				{
					StorageGlobals.ContextTraceError<InvalidCastException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex7);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex7);
				}
				catch (NoSupportException ex8)
				{
					StorageGlobals.ContextTraceError<NoSupportException>(ExTraceGlobals.CcOleTracer, "OleConverter::ConvertToBitmap: ole conversion failed. Exception:\n {0}", ex8);
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, ex8);
				}
			}
			catch (StoragePermanentException exc)
			{
				OleConverter.SaveFailedConversionData(obj, exc, null);
				throw;
			}
			catch (StorageTransientException exc2)
			{
				OleConverter.SaveFailedConversionData(obj, exc2, null);
				throw;
			}
			finally
			{
				Interlocked.Decrement(ref this.nQueueSize);
				if (obj != null)
				{
					OleConverter.DestroyOleObjectData(obj);
				}
			}
			return result;
		}

		internal void ForceRunDirectoryCleanup()
		{
			OleConverter.DirectoryCleanup(this.cleanupLock, true);
		}

		private static object CreateOleObjectData(Stream oleDataStream, bool canCacheInMemory)
		{
			if (canCacheInMemory && oleDataStream.CanSeek)
			{
				long length = oleDataStream.Length;
				if (length <= 262144L)
				{
					byte[] array = new byte[length];
					int num;
					for (int i = 0; i < array.Length; i += num)
					{
						num = oleDataStream.Read(array, i, (int)length - i);
						if (num == 0)
						{
							StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleConverter::CreateOleObjectData: unable to load full stream.");
							throw new OleConversionFailedException(ServerStrings.OleUnableToReadAttachment);
						}
					}
					return array;
				}
			}
			return OleConverter.SaveStreamToTempFile(oleDataStream);
		}

		private static void DestroyOleObjectData(object oleObjectData)
		{
			string text = oleObjectData as string;
			if (text != null)
			{
				OleConverter.DeleteTempFile(text);
			}
		}

		internal static void DeleteTempFile(string filename)
		{
			NativeMethods.DeleteFile(filename);
		}

		private static string SaveStreamToTempFile(Stream stream)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			string conversionsDirectory = OleConverter.GetConversionsDirectory(true);
			if (NativeMethods.GetTempFileName(conversionsDirectory, "ole", 0U, stringBuilder) == 0)
			{
				StorageGlobals.ContextTraceError<string, int>(ExTraceGlobals.CcOleTracer, "OleConverter::SaveStreamToTempFile: failed to create temp file name, directory = {0}. Error {1}", conversionsDirectory, Marshal.GetLastWin32Error());
				throw new OleConversionFailedException(ServerStrings.OleConversionPrepareFailed);
			}
			try
			{
				using (FileStream fileStream = new FileStream(stringBuilder.ToString(), FileMode.OpenOrCreate, FileAccess.Write))
				{
					Util.StreamHandler.CopyStreamData(stream, fileStream);
					fileStream.Close();
				}
			}
			catch (IOException arg)
			{
				OleConverter.DeleteTempFile(stringBuilder.ToString());
				StorageGlobals.ContextTraceError<IOException>(ExTraceGlobals.CcOleTracer, "OleConverter::SaveStreamToTempFile: IOException caught. Exception:\n {0}", arg);
				throw new OleConversionFailedException(ServerStrings.OleConversionPrepareFailed);
			}
			return stringBuilder.ToString();
		}

		private static Stream CreateResultStream(object result)
		{
			string text = result as string;
			if (text != null)
			{
				try
				{
					return new OleConverter.ConversionResultFileStream(text);
				}
				catch (IOException arg)
				{
					OleConverter.DeleteTempFile(text);
					StorageGlobals.ContextTraceError<IOException>(ExTraceGlobals.CcOleTracer, "OleConverter::CreateResultStream: IOException caught. Exception:\n {0}", arg);
					throw new OleConversionFailedException(ServerStrings.OleConversionResultFailed);
				}
			}
			byte[] array = result as byte[];
			if (array != null)
			{
				return new MemoryStream(array);
			}
			StorageGlobals.ContextTraceError<Type>(ExTraceGlobals.CcOleTracer, "OleConverter::CreateResultStream: result type is invalid, {0}.", result.GetType());
			throw new OleConversionFailedException(ServerStrings.OleConversionInvalidResultType);
		}

		private static ExDateTime GetFileCreateTimeUtc(ref NativeMethods.WIN32_FIND_DATA findData)
		{
			long fileTime = ((long)findData.CreationTime.dwHighDateTime << 32) + (long)findData.CreationTime.dwLowDateTime;
			return ExDateTime.FromFileTimeUtc(fileTime);
		}

		private static void DirectoryCleanup(object cleanupLock)
		{
			OleConverter.DirectoryCleanup(cleanupLock, false);
		}

		private static void DirectoryCleanup(object cleanupLock, bool isForced)
		{
			string conversionsDirectory = OleConverter.GetConversionsDirectory(false);
			if (conversionsDirectory == null)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(cleanupLock) || (isForced && Monitor.TryEnter(cleanupLock, 30000)))
				{
					NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
					using (SafeFindHandle safeFindHandle = NativeMethods.FindFirstFile(Path.Combine(conversionsDirectory, "*"), out win32_FIND_DATA))
					{
						if (!safeFindHandle.IsInvalid)
						{
							do
							{
								if ((win32_FIND_DATA.FileAttributes & NativeMethods.FileAttributes.Directory) != NativeMethods.FileAttributes.Directory)
								{
									ExDateTime fileCreateTimeUtc = OleConverter.GetFileCreateTimeUtc(ref win32_FIND_DATA);
									TimeSpan t = ExDateTime.UtcNow.Subtract(fileCreateTimeUtc);
									if (t > OleConverter.MaxFileLifetime)
									{
										NativeMethods.DeleteFile(Path.Combine(conversionsDirectory, win32_FIND_DATA.FileName));
									}
								}
							}
							while (NativeMethods.FindNextFile(safeFindHandle, out win32_FIND_DATA));
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(cleanupLock))
				{
					Monitor.Exit(cleanupLock);
				}
			}
		}

		private static bool RunAsLocalService()
		{
			return string.Compare(Environment.UserName, "SYSTEM", StringComparison.OrdinalIgnoreCase) == 0;
		}

		private static Mutex CreateWorkerLaunchMutex()
		{
			try
			{
				return Mutex.OpenExisting("OleConverterProcessStartMutex", MutexRights.Modify | MutexRights.Synchronize);
			}
			catch (WaitHandleCannotBeOpenedException)
			{
			}
			SecurityIdentifier[] array = new SecurityIdentifier[]
			{
				new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
				new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null),
				new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null),
				new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
				new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null)
			};
			MutexSecurity mutexSecurity = new MutexSecurity();
			foreach (SecurityIdentifier identity in array)
			{
				MutexAccessRule rule = new MutexAccessRule(identity, MutexRights.Modify | MutexRights.Synchronize, AccessControlType.Allow);
				mutexSecurity.AddAccessRule(rule);
			}
			bool flag;
			return new Mutex(false, "OleConverterProcessStartMutex", ref flag, mutexSecurity);
		}

		private static void SaveFailedConversionData(object conversionData, Exception exc, string logDirectoryPath)
		{
			string failedOutboundConversionsDirectory = ConvertUtils.GetFailedOutboundConversionsDirectory(logDirectoryPath);
			if (failedOutboundConversionsDirectory != null)
			{
				try
				{
					string str = Path.Combine(failedOutboundConversionsDirectory, Guid.NewGuid().ToString());
					string path = str + ".txt";
					string path2 = str + ".ole";
					using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(exc.ToString());
							streamWriter.Close();
						}
						fileStream.Close();
					}
					if (conversionData != null)
					{
						using (FileStream fileStream2 = new FileStream(path2, FileMode.CreateNew, FileAccess.Write))
						{
							string text = conversionData as string;
							if (text != null)
							{
								using (FileStream fileStream3 = new FileStream(text, FileMode.Open, FileAccess.Read))
								{
									Util.StreamHandler.CopyStreamData(fileStream3, fileStream2);
									goto IL_DC;
								}
							}
							byte[] array = (byte[])conversionData;
							fileStream2.Write(array, 0, array.Length);
							IL_DC:;
						}
					}
				}
				catch (IOException)
				{
				}
			}
		}

		private const string OleFileExtension = ".ole";

		private const string ErrorInfoExtension = ".txt";

		private const int ImageSizeConversionThresholdBytes = 4194304;

		private const int ImageSizeMarshallingThresholdBytes = 262144;

		private const string ConversionsSubdir = "Working\\OleConverter";

		private const string TempFilenamePrefix = "ole";

		private const int TempFilenameBufferLength = 256;

		private const int WorkerProcessMemoryLimit = 20971520;

		private const string WorkerProcessPath = "bin\\OleConverter.exe";

		private const int WorkerProcessTransactionsLimit = 512;

		private const int WorkerProcessTransactionTimeout = 15000;

		private const int WorkerProcessAllocationTimeout = 10000;

		private const int WorkerProcessLifetimeLimit = 600000;

		private const int WorkerProcessIdleTimeout = 120000;

		private const string WorkerLaunchMutexName = "OleConverterProcessStartMutex";

		private const int MemoryCacheQueueSizeLimit = 30;

		private static readonly int MaxWorkerNumber = Environment.ProcessorCount * 3;

		private static readonly Mutex WorkerLaunchMutex = OleConverter.CreateWorkerLaunchMutex();

		private static Guid oleConverterClassId = OleConverter.RunAsLocalService() ? new Guid("{B5D1252D-4EE6-47CF-AE46-9D1223806F8E}") : new Guid("{B5D12274-5222-418D-8D7B-7D7F674FC111}");

		private static string extraParams = OleConverter.RunAsLocalService() ? "-RunAs LocalService" : null;

		private static readonly ComWorkerConfiguration.RunAsFlag RunAsFlag = ComWorkerConfiguration.RunAsFlag.MayRunUnderAnotherJobObject | (OleConverter.RunAsLocalService() ? ComWorkerConfiguration.RunAsFlag.RunAsLocalService : ComWorkerConfiguration.RunAsFlag.None);

		private static readonly ComWorkerConfiguration WorkerConfigurationObject = new ComWorkerConfiguration(Path.Combine(ConvertUtils.ExchangeSetupPath, "bin\\OleConverter.exe"), OleConverter.extraParams, OleConverter.oleConverterClassId, OleConverter.RunAsFlag, OleConverter.WorkerLaunchMutex, 20971520, 600000, 10000, 512, 15000, 120000);

		private int nQueueSize;

		private static readonly TimeSpan CleanupTimespan = new TimeSpan(0, 30, 0);

		private static readonly TimeSpan MaxFileLifetime = new TimeSpan(0, 5, 0);

		private Timer cleanupTimer;

		private object cleanupLock;

		internal class ConversionRequestParameters
		{
			internal ConversionRequestParameters(object requestData, object responseData)
			{
				this.requestData = requestData;
				this.responseData = responseData;
			}

			internal object RequestData
			{
				get
				{
					return this.requestData;
				}
				set
				{
					this.requestData = value;
				}
			}

			internal object ResponseData
			{
				get
				{
					return this.responseData;
				}
				set
				{
					this.responseData = value;
				}
			}

			private object requestData;

			private object responseData;
		}

		private static class InstanceCreator
		{
			public static OleConverter Instance = new OleConverter();
		}

		private class ConversionResultFileStream : FileStream
		{
			internal ConversionResultFileStream(string filename) : base(filename, FileMode.Open, FileAccess.Read)
			{
				this.filename = filename;
				this.isDeleted = false;
			}

			public override void Close()
			{
				base.Close();
				if (!this.isDeleted)
				{
					OleConverter.DeleteTempFile(this.filename);
					this.isDeleted = true;
				}
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (!this.isDeleted && this.filename != null)
				{
					OleConverter.DeleteTempFile(this.filename);
					this.isDeleted = true;
				}
			}

			private readonly string filename;

			private bool isDeleted;
		}
	}
}
