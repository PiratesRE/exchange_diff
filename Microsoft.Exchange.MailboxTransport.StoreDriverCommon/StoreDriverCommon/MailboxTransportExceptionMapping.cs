using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal static class MailboxTransportExceptionMapping
	{
		internal static Dictionary<Type, Handler> Delivery
		{
			get
			{
				return MailboxTransportExceptionMapping.delivery;
			}
		}

		internal static Dictionary<Type, Handler> Submission
		{
			get
			{
				return MailboxTransportExceptionMapping.submission;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MailboxTransportExceptionMapping()
		{
			Dictionary<Type, Handler> dictionary = new Dictionary<Type, Handler>();
			Dictionary<Type, Handler> dictionary2 = dictionary;
			Type typeFromHandle = typeof(ConversionFailedException);
			Handler handler = new Handler();
			handler.Action = MessageAction.NDR;
			handler.Category = Category.Permanent;
			handler.AppliesToAllRecipients = true;
			handler.PrimaryStatusTextCallback = ((Exception e) => StorageExceptionHandler.GetReasonDescription(((ConversionFailedException)e).ConversionFailureReason));
			handler.EnhancedStatusCodeCallback = delegate(Exception e)
			{
				if (ConversionFailureReason.ExceedsLimit != ((ConversionFailedException)e).ConversionFailureReason)
				{
					return "5.6.0";
				}
				return "5.3.4";
			};
			dictionary2.Add(typeFromHandle, handler);
			Dictionary<Type, Handler> dictionary3 = dictionary;
			Type typeFromHandle2 = typeof(IOException);
			Handler handler2 = new Handler();
			handler2.Action = MessageAction.Throw;
			handler2.Category = Category.Transient;
			handler2.Response = AckReason.DiskFull;
			handler2.CustomizeStatusCallback = delegate(Exception e, IMessageConverter mb, MessageStatus ms)
			{
				if (ExceptionHelper.IsDiskFullException((IOException)e))
				{
					ms.Action = MessageAction.RetryQueue;
				}
			};
			dictionary3.Add(typeFromHandle2, handler2);
			dictionary.Add(typeof(ExchangeDataException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary.Add(typeof(DataSourceOperationException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary.Add(typeof(ADInvalidCredentialException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(ADTransientException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(DataValidationException), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				Response = AckReason.InboundInvalidDirectoryData
			});
			dictionary.Add(typeof(MailboxInSiteFailoverException), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MailboxCrossSiteFailoverException), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MailboxInfoStaleException), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(UnableToFindServerForDatabaseException), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(IllegalCrossServerConnectionException), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(CannotGetSiteInfoException), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				ProcessInnerException = true
			});
			dictionary.Add(typeof(MapiExceptionJetErrorLogDiskFull), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary.Add(typeof(MapiExceptionADDuplicateEntry), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				PrimaryStatusText = "Duplicated AD entries found"
			});
			dictionary.Add(typeof(MapiExceptionNoMoreConnections), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = SmtpResponse.TooManyConnectionsPerSource,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionUnknownUser), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionNoReplicaHere), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionWrongServer), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionWrongMailbox), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionClientVersionDisallowed), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionDuplicateDelivery), new Handler
			{
				Action = MessageAction.LogDuplicate,
				Category = Category.Permanent
			});
			dictionary.Add(typeof(MapiExceptionMaxTimeExpired), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.4.1",
				PrimaryStatusText = "connection timed out"
			});
			dictionary.Add(typeof(MapiExceptionUnableToComplete), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.4.1",
				PrimaryStatusText = "connection timed out"
			});
			dictionary.Add(typeof(MapiExceptionMessageTooBig), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary.Add(typeof(MapiExceptionTooComplex), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary.Add(typeof(MapiExceptionTooBig), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary.Add(typeof(MapiExceptionMaxAttachmentExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary.Add(typeof(MapiExceptionTooManyRecips), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.3",
				PrimaryStatusText = "too many recipients"
			});
			dictionary.Add(typeof(MapiExceptionQuotaExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.2",
				PrimaryStatusText = "mailbox full"
			});
			dictionary.Add(typeof(MapiExceptionShutoffQuotaExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.2",
				PrimaryStatusText = "mailbox full"
			});
			dictionary.Add(typeof(MapiExceptionNoReplicaAvailable), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary.Add(typeof(MapiExceptionFolderDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary.Add(typeof(MapiExceptionMailboxDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary.Add(typeof(MapiExceptionAccountDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary.Add(typeof(MapiExceptionMdbOffline), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MDBOffline
			});
			dictionary.Add(typeof(MapiExceptionMaxObjsExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.3.2",
				PrimaryStatusText = "mailbox busy"
			});
			dictionary.Add(typeof(MapiExceptionRpcBufferTooSmall), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.3.2",
				PrimaryStatusText = "mailbox busy"
			});
			dictionary.Add(typeof(MapiExceptionNoAccess), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.MapiNoAccessFailure
			});
			dictionary.Add(typeof(MapiExceptionJetErrorPageNotInitialized), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				PrimaryStatusText = "Database page not initialized"
			});
			dictionary.Add(typeof(MapiExceptionOutOfMemory), MailboxTransportExceptionMapping.NdrHandler);
			dictionary.Add(typeof(MapiExceptionUnconfigured), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionMailboxInTransit), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionServerPaused), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionDismountInProgress), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary.Add(typeof(MapiExceptionLogonFailed), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.LogonFailure
			});
			dictionary.Add(typeof(MapiExceptionJetErrorLogWriteFail), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary.Add(typeof(MapiExceptionJetErrorDiskIO), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary.Add(typeof(MapiExceptionJetErrorCheckpointDepthTooDeep), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary.Add(typeof(MapiExceptionJetErrorInstanceUnavailable), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary.Add(typeof(MapiExceptionNetworkError), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerOffline,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionRpcServerTooBusy), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerTooBusy,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionSessionLimit), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxMapiSessionLimit,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionNotEnoughMemory), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerNotEnoughMemory,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionMaxThreadsPerMdbExceeded), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerMaxThreadsPerMdbExceeded,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionMaxThreadsPerSCTExceeded), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MapiExceptionMaxThreadsPerSCTExceeded,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary.Add(typeof(MapiExceptionRpcOutOfMemory), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(MapiExceptionRpcOutOfResources), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(MapiExceptionRpcServerOutOfMemory), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(MapiExceptionVersionStoreBusy), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(MapiExceptionSubsystemStopping), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary.Add(typeof(MapiExceptionLowDatabaseDiskSpace), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary.Add(typeof(MapiExceptionLowDatabaseLogDiskSpace), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary.Add(typeof(MapiExceptionADNotFound), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary.Add(typeof(MapiExceptionMailboxQuarantined), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				Response = AckReason.RecipientMailboxQuarantined
			});
			dictionary.Add(typeof(MapiExceptionRetryableImportFailure), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionBusy), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionDiskError), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorOutOfBuffers), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorOutOfCursors), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorOutOfSessions), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorTooManyOpenTables), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorVersionStoreOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionNoFreeJses), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionNotEnoughDisk), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionTimeout), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionWait), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionCollision), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(StoreDriverAgentTransientException), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.DeliverAgentTransientFailure,
				IncludeDiagnosticStatusText = false
			});
			dictionary.Add(typeof(MapiPermanentException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary.Add(typeof(StoragePermanentException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary.Add(typeof(StorageTransientException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiRetryableException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut), MailboxTransportExceptionMapping.RetryHandler);
			dictionary.Add(typeof(MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut), MailboxTransportExceptionMapping.RetryHandler);
			MailboxTransportExceptionMapping.delivery = dictionary;
			Dictionary<Type, Handler> dictionary4 = new Dictionary<Type, Handler>();
			Dictionary<Type, Handler> dictionary5 = dictionary4;
			Type typeFromHandle3 = typeof(ConversionFailedException);
			Handler handler3 = new Handler();
			handler3.Action = MessageAction.NDR;
			handler3.Category = Category.Permanent;
			handler3.AppliesToAllRecipients = true;
			handler3.PrimaryStatusTextCallback = ((Exception e) => StorageExceptionHandler.GetReasonDescription(((ConversionFailedException)e).ConversionFailureReason));
			handler3.EnhancedStatusCodeCallback = delegate(Exception e)
			{
				if (ConversionFailureReason.ExceedsLimit != ((ConversionFailedException)e).ConversionFailureReason)
				{
					return "5.6.0";
				}
				return "5.3.4";
			};
			dictionary5.Add(typeFromHandle3, handler3);
			dictionary4.Add(typeof(NonNdrItemToTransportItemCopyException), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				AppliesToAllRecipients = true,
				EnhancedStatusCode = "5.6.0",
				PrimaryStatusText = "Permanent error while processing message",
				IncludeDiagnosticStatusText = false
			});
			dictionary4.Add(typeof(NdrItemToTransportItemCopyException), new Handler
			{
				Action = MessageAction.Skip,
				Category = Category.Permanent,
				AppliesToAllRecipients = true
			});
			dictionary4.Add(typeof(PoisonHandlerNdrGenerationErrorException), new Handler
			{
				Action = MessageAction.Skip,
				Category = Category.Permanent,
				AppliesToAllRecipients = true
			});
			Dictionary<Type, Handler> dictionary6 = dictionary4;
			Type typeFromHandle4 = typeof(IOException);
			Handler handler4 = new Handler();
			handler4.Action = MessageAction.Throw;
			handler4.Category = Category.Transient;
			handler4.Response = AckReason.DiskFull;
			handler4.CustomizeStatusCallback = delegate(Exception e, IMessageConverter mb, MessageStatus ms)
			{
				if (ExceptionHelper.IsDiskFullException((IOException)e))
				{
					ms.Action = MessageAction.RetryMailboxServer;
				}
			};
			dictionary6.Add(typeFromHandle4, handler4);
			dictionary4.Add(typeof(ExchangeDataException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary4.Add(typeof(DataSourceOperationException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary4.Add(typeof(ADInvalidCredentialException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(ADTransientException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(DataValidationException), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				Response = AckReason.OutboundInvalidDirectoryData
			});
			dictionary4.Add(typeof(MailboxInSiteFailoverException), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MailboxCrossSiteFailoverException), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MailboxInfoStaleException), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(UnableToFindServerForDatabaseException), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(IllegalCrossServerConnectionException), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(CannotGetSiteInfoException), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				ProcessInnerException = true
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorLogDiskFull), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary4.Add(typeof(MapiExceptionADDuplicateEntry), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				PrimaryStatusText = "Duplicated AD entries found"
			});
			dictionary4.Add(typeof(MapiExceptionNoMoreConnections), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				Response = SmtpResponse.TooManyConnectionsPerSource,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionUnknownUser), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionNoReplicaHere), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionWrongServer), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionWrongMailbox), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionClientVersionDisallowed), MailboxTransportExceptionMapping.RerouteHandler);
			dictionary4.Add(typeof(MapiExceptionDuplicateDelivery), new Handler
			{
				Action = MessageAction.LogDuplicate,
				Category = Category.Permanent
			});
			dictionary4.Add(typeof(MapiExceptionMaxTimeExpired), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.4.1",
				PrimaryStatusText = "connection timed out"
			});
			dictionary4.Add(typeof(MapiExceptionUnableToComplete), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.4.1",
				PrimaryStatusText = "connection timed out"
			});
			dictionary4.Add(typeof(MapiExceptionMessageTooBig), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary4.Add(typeof(MapiExceptionTooComplex), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary4.Add(typeof(MapiExceptionTooBig), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary4.Add(typeof(MapiExceptionMaxAttachmentExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.4",
				PrimaryStatusText = "message exceeds fixed system limits"
			});
			dictionary4.Add(typeof(MapiExceptionTooManyRecips), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.3.3",
				PrimaryStatusText = "too many recipients"
			});
			dictionary4.Add(typeof(MapiExceptionQuotaExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.2",
				PrimaryStatusText = "mailbox full"
			});
			dictionary4.Add(typeof(MapiExceptionShutoffQuotaExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.2",
				PrimaryStatusText = "mailbox full"
			});
			dictionary4.Add(typeof(MapiExceptionNoReplicaAvailable), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary4.Add(typeof(MapiExceptionFolderDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary4.Add(typeof(MapiExceptionMailboxDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary4.Add(typeof(MapiExceptionAccountDisabled), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "5.2.1",
				PrimaryStatusText = "mailbox disabled"
			});
			dictionary4.Add(typeof(MapiExceptionMdbOffline), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MDBOffline
			});
			dictionary4.Add(typeof(MapiExceptionMaxObjsExceeded), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.3.2",
				PrimaryStatusText = "mailbox busy"
			});
			dictionary4.Add(typeof(MapiExceptionRpcBufferTooSmall), new Handler
			{
				Action = MessageAction.NDR,
				Category = Category.Permanent,
				EnhancedStatusCode = "4.3.2",
				PrimaryStatusText = "mailbox busy"
			});
			dictionary4.Add(typeof(MapiExceptionNoAccess), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.MapiNoAccessFailure
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorPageNotInitialized), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				PrimaryStatusText = "Database page not initialized"
			});
			dictionary4.Add(typeof(MapiExceptionOutOfMemory), MailboxTransportExceptionMapping.NdrHandler);
			dictionary4.Add(typeof(MapiExceptionUnconfigured), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionMailboxInTransit), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionServerPaused), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionDismountInProgress), MailboxTransportExceptionMapping.RetryQueueHandler);
			dictionary4.Add(typeof(MapiExceptionLogonFailed), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.LogonFailure
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorLogWriteFail), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorDiskIO), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorCheckpointDepthTooDeep), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary4.Add(typeof(MapiExceptionJetErrorInstanceUnavailable), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxIOError
			});
			dictionary4.Add(typeof(MapiExceptionNetworkError), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerOffline,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionRpcServerTooBusy), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				Response = AckReason.MailboxServerTooBusy,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionSessionLimit), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				Response = AckReason.MailboxMapiSessionLimit,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionNotEnoughMemory), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				Response = AckReason.MailboxServerNotEnoughMemory,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionMaxThreadsPerMdbExceeded), new Handler
			{
				Action = MessageAction.RetryQueue,
				Category = Category.Transient,
				Response = AckReason.MailboxServerMaxThreadsPerMdbExceeded,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionMaxThreadsPerSCTExceeded), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.MapiExceptionMaxThreadsPerSCTExceeded,
				RetryInterval = RetryInterval.FastRetry
			});
			dictionary4.Add(typeof(MapiExceptionRpcOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionRpcOutOfResources), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionRpcServerOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionVersionStoreBusy), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionSubsystemStopping), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient
			});
			dictionary4.Add(typeof(MapiExceptionLowDatabaseDiskSpace), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary4.Add(typeof(MapiExceptionLowDatabaseLogDiskSpace), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary4.Add(typeof(MapiExceptionADNotFound), new Handler
			{
				Action = MessageAction.RetryMailboxServer,
				Category = Category.Transient,
				Response = AckReason.MailboxDiskFull
			});
			dictionary4.Add(typeof(MapiExceptionMailboxQuarantined), MailboxTransportExceptionMapping.SkipHandler);
			dictionary4.Add(typeof(MapiExceptionRetryableImportFailure), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionBusy), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionDiskError), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorOutOfBuffers), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorOutOfCursors), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorOutOfSessions), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorTooManyOpenTables), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorVersionStoreOutOfMemory), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionNoFreeJses), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionNotEnoughDisk), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionTimeout), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionWait), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionCollision), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(StoreDriverAgentTransientException), new Handler
			{
				Action = MessageAction.Retry,
				Category = Category.Transient,
				Response = AckReason.DeliverAgentTransientFailure,
				IncludeDiagnosticStatusText = false
			});
			dictionary4.Add(typeof(MapiPermanentException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary4.Add(typeof(StoragePermanentException), MailboxTransportExceptionMapping.NdrHandler);
			dictionary4.Add(typeof(StorageTransientException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiRetryableException), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut), MailboxTransportExceptionMapping.RetryHandler);
			dictionary4.Add(typeof(MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut), MailboxTransportExceptionMapping.RetryHandler);
			MailboxTransportExceptionMapping.submission = dictionary4;
		}

		internal const string DeliveryComponentName = "Delivery";

		internal const string SubmissionComponentName = "Submission";

		private static readonly Handler RetryHandler = new Handler
		{
			Action = MessageAction.Retry,
			Category = Category.Transient
		};

		private static readonly Handler NdrHandler = new Handler
		{
			Action = MessageAction.NDR,
			Category = Category.Permanent
		};

		private static readonly Handler RerouteHandler = new Handler
		{
			Action = MessageAction.Reroute,
			Category = Category.Transient
		};

		private static readonly Handler RetryQueueHandler = new Handler
		{
			Action = MessageAction.RetryQueue,
			Category = Category.Transient
		};

		private static readonly Handler SkipHandler = new Handler
		{
			Action = MessageAction.Skip,
			Category = Category.Transient
		};

		private static Dictionary<Type, Handler> delivery;

		private static Dictionary<Type, Handler> submission;
	}
}
