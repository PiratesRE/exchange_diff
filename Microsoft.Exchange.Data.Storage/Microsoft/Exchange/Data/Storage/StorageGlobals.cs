using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct StorageGlobals
	{
		internal static ExEventLog EventLogger
		{
			get
			{
				if (StorageGlobals.eventLogger == null)
				{
					StorageGlobals.eventLogger = new ExEventLog(new Guid("{8E4F12B2-E72A-42b4-816C-30462241203A}"), "MSExchange Mid-Tier Storage");
				}
				return StorageGlobals.eventLogger;
			}
		}

		public static LocalizedException TranslateMapiException(LocalizedString exceptionMessage, LocalizedException exception, StoreSession session, object thisObject, string traceMessage, params object[] traceMessageParameters)
		{
			if (ExTraceGlobals.MapiConnectivityTracer.IsTraceEnabled(TraceType.ErrorTrace) && (exception is MapiExceptionBackupInProgress || exception is MapiExceptionNetworkError || exception is MapiExceptionEndOfSession || exception is MapiExceptionLogonFailed || exception is MapiExceptionExiting || exception is MapiExceptionRpcServerTooBusy || exception is MapiExceptionBusy))
			{
				Guid guid = (session == null) ? Guid.Empty : session.MdbGuid;
				ExTraceGlobals.MapiConnectivityTracer.TraceError((long)((thisObject != null) ? thisObject.GetHashCode() : 0), "MAPI exception: {0}\n\rClient: {1}\n\rServer: {2}\n\rMDB: {3}", new object[]
				{
					exception.ToString(),
					(session != null) ? session.ClientInfoString : null,
					(session != null && !session.IsRemote) ? session.ServerFullyQualifiedDomainName : null,
					guid
				});
			}
			LocalizedException ex = null;
			if (session != null && session.IsRemote)
			{
				if (exception is MapiExceptionBackupInProgress || exception is MapiExceptionNetworkError || exception is MapiExceptionEndOfSession || exception is MapiExceptionLogonFailed || exception is MapiExceptionExiting || exception is MapiExceptionRpcServerTooBusy || exception is MapiExceptionBusy || exception is MapiExceptionMailboxInTransit || exception is MapiExceptionNotEnoughDisk || exception is MapiExceptionNotEnoughResources || exception is MapiExceptionMdbOffline || exception is MapiExceptionServerPaused || exception is MapiExceptionMailboxDisabled || exception is MapiExceptionAccountDisabled || exception is MapiExceptionWrongMailbox || exception is MapiExceptionCorruptStore || exception is MapiExceptionNoAccess || exception is MapiExceptionNoSupport || exception is MapiExceptionNotAuthorized || exception is MapiExceptionPasswordChangeRequired || exception is MapiExceptionPasswordExpired || exception is MapiExceptionNoMoreConnections || exception is MapiExceptionWrongServer || exception is MapiExceptionSessionLimit || exception is MapiExceptionUnconfigured || exception is MapiExceptionUnknownUser || exception is MapiExceptionCallFailed)
				{
					MailboxSession mailboxSession = session as MailboxSession;
					string text = mailboxSession.MailboxOwner.MailboxInfo.RemoteIdentity.Value.ToString();
					string text2 = exception.ToString();
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_XtcMapiError, text, new object[]
					{
						text,
						text2
					});
					ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "The remote connection threw exception for user {0}. Exception: {1}", text, text2);
				}
			}
			else
			{
				ex = StorageGlobals.CheckHAState(exceptionMessage, exception, session);
			}
			if (ex == null)
			{
				if (exception is MapiExceptionBackupInProgress || exception is MapiExceptionNetworkError || exception is MapiExceptionEndOfSession || exception is MapiExceptionLogonFailed || exception is MapiExceptionExiting)
				{
					ex = new ConnectionFailedTransientException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMailboxInTransit)
				{
					ex = new MailboxInTransitException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNotEnoughDisk || exception is MapiExceptionNotEnoughResources || exception is MapiExceptionBusy)
				{
					ex = new ResourcesException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionObjectChanged)
				{
					ex = new SaveConflictException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionCanNotComplete)
				{
					ex = new CannotCompleteOperationException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionJetErrorCheckpointDepthTooDeep)
				{
					ex = new CheckpointTooDeepException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMdbOffline)
				{
					ex = new MailboxOfflineException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionVirusScanInProgress)
				{
					ex = new VirusScanInProgressException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionServerPaused)
				{
					ex = new ServerPausedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionSearchEvaluationInProgress)
				{
					ex = new QueryInProgressException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionGlobalCounterRangeExceeded)
				{
					ex = new GlobalCounterRangeExceededException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionCorruptMidsetDeleted)
				{
					ex = new CorruptMidsetDeletedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMailboxDisabled || exception is MapiExceptionAccountDisabled || exception is MapiExceptionMailboxSoftDeleted)
				{
					ex = new AccountDisabledException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionWrongMailbox || exception is MapiExceptionCorruptStore)
				{
					ex = new ConnectionFailedPermanentException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNoAccess || exception is MapiExceptionNotAuthorized || exception is MapiExceptionPasswordChangeRequired || exception is MapiExceptionPasswordExpired || exception is MapiExceptionNoCreateRight || exception is MapiExceptionNoCreateSubfolderRight)
				{
					ex = new AccessDeniedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionCorruptData)
				{
					ex = new CorruptDataException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionQuotaExceeded || exception is MapiExceptionNamedPropsQuotaExceeded || exception is MapiExceptionShutoffQuotaExceeded)
				{
					ex = new QuotaExceededException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionCollision)
				{
					ex = new ObjectExistedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNotFound || exception is MapiExceptionInvalidEntryId || exception is MapiExceptionJetErrorRecordDeleted || exception is MapiExceptionObjectDeleted)
				{
					ex = new ObjectNotFoundException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNoMoreConnections)
				{
					ex = new NoMoreConnectionsException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionPartialCompletion)
				{
					ex = new PartialCompletionException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionFolderCycle)
				{
					ex = new FolderCycleException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionWrongServer)
				{
					ex = new WrongServerException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionIllegalCrossServerConnection)
				{
					ex = new IllegalCrossServerConnectionException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionInvalidRecipients)
				{
					ex = new InvalidRecipientsException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMessageTooBig)
				{
					ex = new MessageTooBigException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMaxSubmissionExceeded)
				{
					ex = new MessageSubmissionExceededException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionMaxAttachmentExceeded)
				{
					ex = new AttachmentExceededException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionVirusDetected)
				{
					ex = new VirusDetectedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionVirusMessageDeleted)
				{
					ex = new VirusMessageDeletedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNotInitialized)
				{
					ex = new ObjectNotInitializedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionSendAsDenied)
				{
					ex = new SendAsDeniedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionSessionLimit)
				{
					ex = new TooManyObjectsOpenedException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut)
				{
					ex = new ServerCleanupTimedOutException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionJetErrorInvalidLanguageId)
				{
					ex = new InvalidFolderLanguageIdException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionEventsDeleted)
				{
					ex = new EventNotFoundException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionUnconfigured)
				{
					ex = new MailboxUnavailableException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionUnknownUser)
				{
					ex = new MailboxUnavailableException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNoReplicaHere)
				{
					ex = new NoReplicaHereException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNoReplicaAvailable)
				{
					ex = new NoReplicaException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionConditionViolation)
				{
					ex = new FolderSaveConditionViolationException(exceptionMessage, exception);
				}
				else if (exception is MapiExceptionNoSupport)
				{
					ex = new NoSupportException(exceptionMessage, exception);
				}
				else if (exception is MapiPermanentException)
				{
					ex = new StoragePermanentException(exceptionMessage, exception);
				}
				else if (exception is MapiRetryableException)
				{
					ex = new StorageTransientException(exceptionMessage, exception);
				}
				else
				{
					if (!(exception is MapiExceptionCallFailed))
					{
						throw new ArgumentException("Exception is not of type MapiException");
					}
					ex = new StoragePermanentException(exceptionMessage, exception);
				}
			}
			if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string arg = string.Format(traceMessage, traceMessageParameters);
				ExTraceGlobals.StorageTracer.TraceError<string, LocalizedException>((long)((thisObject != null) ? thisObject.GetHashCode() : 0), "{0}. Throwing exception: {1}.", arg, ex);
			}
			return ex;
		}

		internal static bool IsDiskFullException(IOException e)
		{
			uint hrforException = (uint)Marshal.GetHRForException(e);
			return hrforException == 2147942512U;
		}

		internal static void SetCallMapiTestHook(StorageGlobals.MapiTestHook beforeCall, StorageGlobals.MapiTestHook afterCall)
		{
			StorageGlobals.MapiTestHookBeforeCall = beforeCall;
			StorageGlobals.MapiTestHookAfterCall = afterCall;
		}

		static StorageGlobals()
		{
			StorageGlobals.buildVersion = StorageGlobals.ObtainBuildVersion();
		}

		internal static TResult ProtectedADCall<TResult>(Func<TResult> activeDirectoryCall)
		{
			TResult result;
			try
			{
				result = activeDirectoryCall();
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "StorageGlobals::{0} failed due to directory exception {1}.", new object[]
				{
					activeDirectoryCall,
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "StorageGlobals::{0} failed due to directory exception {1}.", new object[]
				{
					activeDirectoryCall,
					ex2
				});
			}
			catch (DataValidationException ex3)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex3, null, "StorageGlobals::{0} failed due to directory exception {1}.", new object[]
				{
					activeDirectoryCall,
					ex3
				});
			}
			return result;
		}

		internal static long BuildVersion
		{
			get
			{
				return StorageGlobals.buildVersion;
			}
		}

		internal static string BuildVersionString
		{
			get
			{
				return "15.00.1497.012";
			}
		}

		internal static string ExchangeVersion
		{
			get
			{
				if (StorageGlobals.exchangeVersion == null)
				{
					StorageGlobals.exchangeVersion = ((ushort)(StorageGlobals.BuildVersion >> 48)).ToString(NumberFormatInfo.InvariantInfo) + "." + ((ushort)(StorageGlobals.BuildVersion >> 32)).ToString(NumberFormatInfo.InvariantInfo) + "Ex";
				}
				return StorageGlobals.exchangeVersion;
			}
		}

		private static long ObtainBuildVersion()
		{
			return 4222124748767244L;
		}

		public static void TraceConstructIDisposable(object obj)
		{
			if (ExTraceGlobals.DisposeTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int hashCode = obj.GetHashCode();
				ExTraceGlobals.DisposeTracer.TraceDebug<string, int>((long)hashCode, "{0}::Constructor. Hashcode = {1}.", obj.GetType().Name, hashCode);
			}
		}

		public static void TraceDispose(object obj, bool isDisposed, bool disposing)
		{
			if (ExTraceGlobals.DisposeTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int hashCode = obj.GetHashCode();
				ExTraceGlobals.DisposeTracer.TraceDebug((long)hashCode, "{0}::Dispose. Hashcode = {1}. IsDisposed = {2}. Disposing = {3}.", new object[]
				{
					obj.GetType().Name,
					hashCode,
					isDisposed,
					disposing
				});
			}
		}

		public static void TraceFailedCheckDisposed(object obj, string methodName)
		{
			if (ExTraceGlobals.DisposeTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int hashCode = obj.GetHashCode();
				ExTraceGlobals.DisposeTracer.TraceDebug<string, string, int>((long)hashCode, "{0}::{1}. Attempted to use disposed object. Hashcode = {2}.", obj.GetType().Name, methodName, hashCode);
			}
		}

		public static void TraceFailedCheckDead(object obj, string methodName)
		{
			if (ExTraceGlobals.DisposeTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int hashCode = obj.GetHashCode();
				ExTraceGlobals.DisposeTracer.TraceDebug<string, string, int>((long)hashCode, "{0}::{1}. Attempted to use object on dead session. Hashcode = {2}.", obj.GetType().Name, methodName, hashCode);
			}
		}

		private static Stack<StorageGlobals.TraceContextDescriptor> TraceContextStack
		{
			get
			{
				if (StorageGlobals.traceContextStack == null)
				{
					StorageGlobals.traceContextStack = new Stack<StorageGlobals.TraceContextDescriptor>();
				}
				return StorageGlobals.traceContextStack;
			}
		}

		private static Random TraceContextRandom
		{
			get
			{
				if (StorageGlobals.traceContextRandom == null)
				{
					StorageGlobals.traceContextRandom = new Random();
				}
				return StorageGlobals.traceContextRandom;
			}
		}

		private static int GetTraceContextCode(bool isTraceEnabled)
		{
			if (StorageGlobals.TraceContextStack.Count > 0)
			{
				StorageGlobals.TraceContextDescriptor traceContextDescriptor = StorageGlobals.TraceContextStack.Peek();
				return traceContextDescriptor.GetContextCode(isTraceEnabled);
			}
			return 0;
		}

		public static IDisposable SetTraceContext(object context)
		{
			return new StorageGlobals.TraceContextDescriptor(context);
		}

		public static void ContextTracePfd(Trace tracer, string message)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.PfdTrace);
			tracer.TracePfd((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), message);
		}

		public static void ContextTracePfd(Trace tracer, string formatString, params object[] args)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.PfdTrace);
			tracer.TracePfd((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, args);
		}

		public static void ContextTracePfd<T>(Trace tracer, string formatString, T arg0)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.PfdTrace);
			tracer.TracePfd<T>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0);
		}

		public static void ContextTracePfd<T0, T1>(Trace tracer, string formatString, T0 arg0, T1 arg1)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.PfdTrace);
			tracer.TracePfd<T0, T1>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1);
		}

		public static void ContextTracePfd<T0, T1, T2>(Trace tracer, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.PfdTrace);
			tracer.TracePfd<T0, T1, T2>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1, arg2);
		}

		public static void ContextTraceInformation(Trace tracer, string message)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.InfoTrace);
			tracer.Information((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), message);
		}

		public static void ContextTraceInformation(Trace tracer, string formatString, params object[] args)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.InfoTrace);
			tracer.Information((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, args);
		}

		public static void ContextTraceInformation<T>(Trace tracer, string formatString, T arg0)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.InfoTrace);
			tracer.Information<T>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0);
		}

		public static void ContextTraceInformation<T0, T1>(Trace tracer, string formatString, T0 arg0, T1 arg1)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.InfoTrace);
			tracer.Information<T0, T1>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1);
		}

		public static void ContextTraceInformation<T0, T1, T2>(Trace tracer, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.InfoTrace);
			tracer.Information<T0, T1, T2>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1, arg2);
		}

		public static void ContextTraceDebug(Trace tracer, string message)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.DebugTrace);
			tracer.TraceDebug((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), message);
		}

		public static void ContextTraceDebug(Trace tracer, string formatString, params object[] args)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.DebugTrace);
			tracer.TraceDebug((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, args);
		}

		public static void ContextTraceDebug<T>(Trace tracer, string formatString, T arg0)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.DebugTrace);
			tracer.TraceDebug<T>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0);
		}

		public static void ContextTraceDebug<T0, T1>(Trace tracer, string formatString, T0 arg0, T1 arg1)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.DebugTrace);
			tracer.TraceDebug<T0, T1>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1);
		}

		public static void ContextTraceDebug<T0, T1, T2>(Trace tracer, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.DebugTrace);
			tracer.TraceDebug<T0, T1, T2>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1, arg2);
		}

		public static void ContextTraceError(Trace tracer, string message)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.ErrorTrace);
			tracer.TraceError((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), message);
		}

		public static void ContextTraceError(Trace tracer, string formatString, params object[] args)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.ErrorTrace);
			tracer.TraceError((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, args);
		}

		public static void ContextTraceError<T0>(Trace tracer, string formatString, T0 arg0)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.ErrorTrace);
			tracer.TraceError<T0>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0);
		}

		public static void ContextTraceError<T0, T1>(Trace tracer, string formatString, T0 arg0, T1 arg1)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.ErrorTrace);
			tracer.TraceError<T0, T1>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1);
		}

		public static void ContextTraceError<T0, T1, T2>(Trace tracer, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			bool isTraceEnabled = tracer.IsTraceEnabled(TraceType.ErrorTrace);
			tracer.TraceError<T0, T1, T2>((long)StorageGlobals.GetTraceContextCode(isTraceEnabled), formatString, arg0, arg1, arg2);
		}

		private static LocalizedException CheckHAState(LocalizedString exceptionMessage, LocalizedException mapiException, StoreSession session)
		{
			if (session != null && (mapiException is MapiExceptionBackupInProgress || mapiException is MapiExceptionEndOfSession || mapiException is MapiExceptionLogonFailed || mapiException is MapiExceptionExiting || mapiException is MapiExceptionMailboxInTransit || mapiException is MapiExceptionCanNotComplete || mapiException is MapiExceptionMdbOffline || mapiException is MapiExceptionWrongServer || mapiException is MapiExceptionUnconfigured || mapiException is MapiExceptionUnknownUser || mapiException is MapiExceptionConditionViolation || mapiException is MapiExceptionServerPaused || mapiException is MapiExceptionCallFailed))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<LocalizedException>((long)session.GetHashCode(), "StorageGlobals::CheckHAState. Translating exception {0}.", mapiException);
				DatabaseLocationInfo databaseLocationInfoOnOperationFailure = session.GetDatabaseLocationInfoOnOperationFailure();
				ExTraceGlobals.SessionTracer.TraceDebug<DatabaseLocationInfoResult>((long)session.GetHashCode(), "StorageGlobals::CheckHAState. AM result {0}.", databaseLocationInfoOnOperationFailure.RequestResult);
				switch (databaseLocationInfoOnOperationFailure.RequestResult)
				{
				case DatabaseLocationInfoResult.Success:
				{
					IExchangePrincipal mailboxOwner = session.MailboxOwner;
					if (mailboxOwner != null)
					{
						IMailboxInfo mailboxInfo = mailboxOwner.MailboxInfo;
						if (mailboxInfo != null)
						{
							IMailboxLocation location = mailboxInfo.Location;
							if (location != null)
							{
								if (string.Equals(location.ServerFqdn, databaseLocationInfoOnOperationFailure.ServerFqdn, StringComparison.OrdinalIgnoreCase))
								{
									if (mapiException is MapiExceptionWrongServer)
									{
										return new WrongServerException(exceptionMessage, mapiException);
									}
									return null;
								}
								else
								{
									if (location.ServerSite == null || databaseLocationInfoOnOperationFailure.ServerSite == null)
									{
										return new MailboxInSiteFailoverException(exceptionMessage, mapiException);
									}
									if (ADObjectId.Equals(location.ServerSite, databaseLocationInfoOnOperationFailure.ServerSite))
									{
										return new MailboxInSiteFailoverException(exceptionMessage, mapiException);
									}
									return new MailboxCrossSiteFailoverException(exceptionMessage, mapiException, databaseLocationInfoOnOperationFailure);
								}
							}
						}
					}
					break;
				}
				case DatabaseLocationInfoResult.Unknown:
					return new MailboxOfflineException(exceptionMessage, mapiException);
				case DatabaseLocationInfoResult.InTransitSameSite:
					return new MailboxInSiteFailoverException(exceptionMessage, mapiException);
				case DatabaseLocationInfoResult.InTransitCrossSite:
					return new MailboxCrossSiteFailoverException(exceptionMessage, mapiException, databaseLocationInfoOnOperationFailure);
				case DatabaseLocationInfoResult.SiteViolation:
					return new WrongServerException(exceptionMessage, mapiException);
				default:
					throw new NotSupportedException(string.Format("DatabaseLocationInfoResult.{0} is not supported", databaseLocationInfoOnOperationFailure));
				}
			}
			return null;
		}

		internal static LocalizedException TranslateDirectoryException(LocalizedString exceptionMessage, LocalizedException exception, object thisObject, string traceMessage = "", params object[] traceMessageParameters)
		{
			LocalizedException ex;
			if (exception is ADNoSuchObjectException)
			{
				ex = new ObjectNotFoundException(exceptionMessage, exception);
			}
			else if (exception is DataValidationException || exception is DataConversionException || exception is NonUniqueRecipientException)
			{
				ex = new StoragePermanentException(exceptionMessage, exception);
			}
			else if (exception is ADTransientException || exception is ComputerNameNotCurrentlyAvailableException || exception is ADPossibleOperationException || exception is RusServerUnavailableException || exception is DataSourceTransientException)
			{
				ex = new StorageTransientException(exceptionMessage, exception);
			}
			else if (exception is ADOperationException || exception is ADObjectAlreadyExistsException || exception is ADFilterException || exception is ServerRoleOperationException || exception is RusOperationException || exception is NameConversionException || exception is GenerateUniqueLegacyDnException)
			{
				ex = new StoragePermanentException(exceptionMessage, exception);
			}
			else
			{
				if (!(exception is ADExternalException) && !(exception is CannotGetComputerNameException) && !(exception is CannotGetDomainInfoException) && !(exception is CannotGetSiteInfoException) && !(exception is CannotGetForestInfoException) && !(exception is LocalServerNotFoundException) && !(exception is DataSourceOperationException))
				{
					throw new ArgumentException("exception is not a directory exception");
				}
				ex = new StoragePermanentException(exceptionMessage, exception);
			}
			if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string arg = string.Format(CultureInfo.InvariantCulture, traceMessage, traceMessageParameters);
				ExTraceGlobals.StorageTracer.TraceError<string, LocalizedException>((long)((thisObject != null) ? thisObject.GetHashCode() : 0), "{0}. Throwing exception: {1}.", arg, ex);
			}
			return ex;
		}

		private static ExEventLog eventLogger;

		private static long buildVersion;

		private static string exchangeVersion;

		[ThreadStatic]
		public static bool RPCBudgetStarted = false;

		[ThreadStatic]
		public static StorageGlobals.MapiTestHook MapiTestHookBeforeCall = null;

		public static StorageGlobals.MapiTestHook MapiTestHookAfterCall = null;

		[ThreadStatic]
		private static Stack<StorageGlobals.TraceContextDescriptor> traceContextStack;

		[ThreadStatic]
		private static Random traceContextRandom;

		internal delegate void MapiCall();

		internal delegate T MapiCallWithReturnValue<T>();

		internal delegate void MapiTestHook(object caller);

		private class TraceContextDescriptor : IDisposable
		{
			public TraceContextDescriptor(object context)
			{
				this.context = context;
				this.isWritten = false;
				this.isWrittenShadow = false;
				this.isDisposed = false;
				this.contextCode = StorageGlobals.TraceContextRandom.Next();
				StorageGlobals.TraceContextStack.Push(this);
			}

			private int GetParentCode(bool isTraceEnabled)
			{
				StorageGlobals.TraceContextStack.Pop();
				int traceContextCode = StorageGlobals.GetTraceContextCode(isTraceEnabled);
				StorageGlobals.TraceContextStack.Push(this);
				return traceContextCode;
			}

			public int GetContextCode(bool isTraceEnabled)
			{
				bool flag = ExTraceGlobals.ContextTracer.IsTraceEnabled(TraceType.InfoTrace);
				bool flag2 = ExTraceGlobals.ContextShadowTracer.IsTraceEnabled(TraceType.InfoTrace);
				if (isTraceEnabled && flag)
				{
					if (!this.isWritten)
					{
						int parentCode = this.GetParentCode(isTraceEnabled);
						ExTraceGlobals.ContextTracer.Information<int, int, object>((long)parentCode, "Code: {0}, Parent: {1}, \n{2}", this.contextCode, parentCode, this.context);
						this.isWritten = true;
						this.isWrittenShadow = true;
					}
				}
				else if (flag2 && !this.isWrittenShadow)
				{
					int parentCode2 = this.GetParentCode(isTraceEnabled);
					ExTraceGlobals.ContextShadowTracer.Information<int, int, object>((long)parentCode2, "Code: {0}, Parent: {1}, \n{2}", this.contextCode, parentCode2, this.context);
					this.isWrittenShadow = true;
				}
				return this.contextCode;
			}

			protected void Dispose(bool disposing)
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					this.InternalDispose(disposing);
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					StorageGlobals.TraceContextStack.Pop();
				}
			}

			private object context;

			private bool isWritten;

			private bool isWrittenShadow;

			private int contextCode;

			private bool isDisposed;
		}
	}
}
