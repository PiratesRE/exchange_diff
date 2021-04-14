using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Interop;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentFilter;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	internal static class Util
	{
		public static void LogContentFilterInitialized()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitialized, null, null);
		}

		public static void LogContentFilterNotInitialized(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterNotInitialized, null, new object[]
			{
				e
			});
		}

		public static void LogFailedWithUnauthorizedAccess(string path, Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitFailedUnauthorizedAccess, null, new object[]
			{
				path,
				e
			});
		}

		public static void LogFailedWithBadImageFormat(string path, Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitFailedBadImageFormat, null, new object[]
			{
				path,
				e
			});
		}

		public static void LogFailedFSWatcherAlreadyInitialized(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitFailedFSWatcherAlreadyInitialized, null, new object[]
			{
				e
			});
		}

		public static void LogFailedInsufficientBuffer(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitFailedInsufficientBuffer, null, new object[]
			{
				e
			});
		}

		public static void LogQuarantineMailboxIsInvalid()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterQuarantineMailboxIsInvalid, null, null);
		}

		public static void LogWrapperNotResponding()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperNotResponding, null, null);
		}

		public static void LogWrapperBeingRecycled()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperBeingRecycled, null, null);
		}

		public static void LogWrapperSuccessfullyRecycled()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperSuccessfullyRecycled, null, null);
		}

		public static void LogWrapperRecycleTimedout()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperRecycleTimedout, null, null);
		}

		public static void LogWrapperRecycleError(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperRecycleError, null, new object[]
			{
				e
			});
		}

		public static void LogWrapperSendingPingRequest(int numberOfMessages)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperSendingPingRequest, null, new object[]
			{
				numberOfMessages
			});
		}

		public static void LogErrorSubmittingMessage(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterWrapperErrorSubmittingMessage, null, new object[]
			{
				e
			});
		}

		public static void LogExSMimeFailedToInitialize(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ExSMimeFailedToInitialize, null, new object[]
			{
				e
			});
		}

		public static void LogUnexpectedFailureScanningMessage(string messageID, int hresult, string details)
		{
			string periodicKey = hresult.ToString("X", CultureInfo.InvariantCulture);
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_UnexpectedFailureScanningMessage, periodicKey, new object[]
			{
				messageID,
				details
			});
		}

		public static void LogUpdateModeChangedReinitializingSmartScreen()
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_AntispamUpdateModeChanged, null, null);
		}

		public static void LogFailedToReadAntispamUpdateMode(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_FailedToReadAntispamUpdateMode, e.Message, new object[]
			{
				e
			});
		}

		public static void LogContentFilterInitFailedFileNotFound(Exception e)
		{
			Util.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ContentFilterInitFailedFileNotFound, null, new object[]
			{
				e
			});
		}

		public static void InvokeExLapi(ComProxy comProxy, ComProxy.AsyncCompletionCallback callback, ComArguments comArguments, MailItem mailItem, byte[] requestType)
		{
			if (comArguments == null)
			{
				comArguments = new ComArguments();
			}
			comArguments[1] = requestType;
			try
			{
				comProxy.Invoke(callback, comArguments, mailItem);
			}
			catch (COMException ex)
			{
				ExTraceGlobals.InitializationTracer.TraceError<int>(0L, "ComProxy.Invoke() threw a COMException with the following HRESULT: {0}", ex.ErrorCode);
				throw;
			}
		}

		public static bool IsUnexpectedMessageFailure(uint failureHResult)
		{
			return Array.BinarySearch<uint>(Constants.ExpectedMessageFailureHResults, failureHResult) < 0;
		}

		public static void InitializeFilter(ComProxy comProxy, ComArguments comArguments)
		{
			Util.InvokeExLapi(comProxy, null, comArguments, null, Constants.RequestTypes.Initialize);
		}

		public static byte[] SerializeByteArrays(int totalLength, params byte[][][] byteArrays)
		{
			if (byteArrays.Length == 0)
			{
				throw new InvalidOperationException("You must pass in at least one byte[][] parameter");
			}
			int num = byteArrays[0].Length;
			byte[] bytes = BitConverter.GetBytes(num);
			byte[] array = new byte[totalLength + bytes.Length];
			int num2 = 0;
			Util.WriteToBuffer(array, ref num2, bytes);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < byteArrays.Length; j++)
				{
					Util.WriteToBuffer(array, ref num2, byteArrays[j][i]);
				}
			}
			return array;
		}

		private static void WriteToBuffer(byte[] buffer, ref int bufferIndex, byte[] value)
		{
			value.CopyTo(buffer, bufferIndex);
			bufferIndex += value.Length;
		}

		internal static readonly ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.ScanMessageTracer.Category, "MSExchange Antispam");

		public static class PerformanceCounters
		{
			public static void MessageScanned()
			{
				ContentFilterPerfCounters.TotalMessagesScanned.Increment();
			}

			public static void MessageNotScanned()
			{
				ContentFilterPerfCounters.TotalMessagesNotScanned.Increment();
			}

			public static void PreExistingSCL()
			{
				ContentFilterPerfCounters.TotalMessagesWithPreExistingSCL.Increment();
			}

			public static void FilterFailure()
			{
				ContentFilterPerfCounters.TotalMessagesThatCauseFilterFailure.Increment();
			}

			public static void MessageIsAtSCL(int scl)
			{
				if (scl < 0 || scl > 9)
				{
					throw new ArgumentOutOfRangeException("scl", scl, "SCL must be within 0-9");
				}
				Util.PerformanceCounters.SclCounters[scl].Increment();
			}

			public static void MessageDeleted()
			{
				ContentFilterPerfCounters.TotalMessagesDeleted.Increment();
			}

			public static void MessageRejected()
			{
				ContentFilterPerfCounters.TotalMessagesRejected.Increment();
			}

			public static void MessageQuarantined()
			{
				ContentFilterPerfCounters.TotalMessagesQuarantined.Increment();
			}

			public static void MessageNotScannedDueToOrgSafeSender()
			{
				ContentFilterPerfCounters.TotalMessagesNotScannedDueToOrgSafeSender.Increment();
			}

			public static void BypassedRecipientDueToPerRecipientSafeSender()
			{
				ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeSender.Increment();
			}

			public static void BypassedRecipientDueToPerRecipientSafeRecipient()
			{
				ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeRecipient.Increment();
			}

			public static void MessagesWithInvalidPostmarks()
			{
				ContentFilterPerfCounters.TotalMessagesWithInvalidPostmarks.Increment();
			}

			public static void MessagesWithValidPostmarks()
			{
				ContentFilterPerfCounters.TotalMessagesWithValidPostmarks.Increment();
			}

			public static void RemoveCounters()
			{
				ContentFilterPerfCounters.TotalMessagesScanned.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesNotScanned.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesWithPreExistingSCL.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesThatCauseFilterFailure.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesDeleted.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesRejected.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesQuarantined.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesNotScannedDueToOrgSafeSender.RawValue = 0L;
				ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeSender.RawValue = 0L;
				ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeRecipient.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesWithValidPostmarks.RawValue = 0L;
				ContentFilterPerfCounters.TotalMessagesWithInvalidPostmarks.RawValue = 0L;
				foreach (ExPerformanceCounter exPerformanceCounter in Util.PerformanceCounters.SclCounters)
				{
					exPerformanceCounter.RawValue = 0L;
				}
			}

			private static readonly ExPerformanceCounter[] SclCounters = new ExPerformanceCounter[]
			{
				ContentFilterPerfCounters.MessagesAtSCL0,
				ContentFilterPerfCounters.MessagesAtSCL1,
				ContentFilterPerfCounters.MessagesAtSCL2,
				ContentFilterPerfCounters.MessagesAtSCL3,
				ContentFilterPerfCounters.MessagesAtSCL4,
				ContentFilterPerfCounters.MessagesAtSCL5,
				ContentFilterPerfCounters.MessagesAtSCL6,
				ContentFilterPerfCounters.MessagesAtSCL7,
				ContentFilterPerfCounters.MessagesAtSCL8,
				ContentFilterPerfCounters.MessagesAtSCL9
			};
		}
	}
}
