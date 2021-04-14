using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class IMAPClient
	{
		internal static IAsyncResult BeginConnectAndAuthenticate(IMAPClientState clientState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogDebugging((TSLID)813UL, IMAPClient.Tracer, "Beginning connect-and-authenticate process", new object[0]);
			AsyncResult<IMAPClientState, DBNull> asyncResult = new AsyncResult<IMAPClientState, DBNull>(clientState, clientState, callback, callbackState, syncPoisonContext);
			AsyncCallback callback2;
			switch (clientState.IMAPSecurityMechanism)
			{
			case IMAPSecurityMechanism.None:
				callback2 = new AsyncCallback(IMAPClient.OnEndConnectToAuthenticate);
				break;
			case IMAPSecurityMechanism.Ssl:
				callback2 = new AsyncCallback(IMAPClient.OnEndConnectToAuthenticate);
				break;
			case IMAPSecurityMechanism.Tls:
				callback2 = new AsyncCallback(IMAPClient.OnEndConnectToStarttls);
				break;
			default:
				throw new InvalidOperationException("Unexpected security mechanism " + clientState.IMAPSecurityMechanism);
			}
			asyncResult.PendingAsyncResult = clientState.CommClient.BeginConnect(clientState, callback2, asyncResult, syncPoisonContext);
			return asyncResult;
		}

		internal static AsyncOperationResult<DBNull> EndConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginCapabilities(IMAPClientState clientState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)814UL, IMAPClient.Tracer, "Getting server capabilities", new object[0]);
			clientState.CachedCommand.ResetAsCapability(clientState.UniqueCommandId);
			return IMAPClient.CreateAsyncResultAndBeginCommand<IList<string>>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndCapabilityInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<IList<string>> EndCapabilities(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IList<string>> asyncResult2 = (AsyncResult<IMAPClientState, IList<string>>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginExpunge(IMAPClientState clientState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)815UL, IMAPClient.Tracer, "Expunging messages.", new object[0]);
			clientState.CachedCommand.ResetAsExpunge(clientState.UniqueCommandId);
			return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndExpungeInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<DBNull> EndExpunge(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginSelectImapMailbox(IMAPClientState clientState, IMAPMailbox imapMailbox, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)816UL, IMAPClient.Tracer, "Selecting mailbox {0}", new object[]
			{
				imapMailbox.Name
			});
			clientState.CachedCommand.ResetAsSelect(clientState.UniqueCommandId, imapMailbox);
			return IMAPClient.CreateAsyncResultAndBeginCommand<IMAPMailbox>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndSelectImapMailboxCallStatusIfNeeded), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<IMAPMailbox> EndSelectImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IMAPMailbox> asyncResult2 = (AsyncResult<IMAPClientState, IMAPMailbox>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginGetMessageInfoByRange(IMAPClientState clientState, string start, string end, bool uidFetch, IList<string> messageDataItems, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			AsyncResult<IMAPClientState, IMAPResultData> asyncResult = new AsyncResult<IMAPClientState, IMAPResultData>(clientState, clientState, callback, callbackState, syncPoisonContext);
			clientState.CachedCommand.ResetAsFetch(clientState.UniqueCommandId, start, end, uidFetch, messageDataItems);
			clientState.Log.LogRawData((TSLID)817UL, IMAPClient.Tracer, "IMAP Fetch Message Headers in range.  Start={0}.  End={1}", new object[]
			{
				start,
				end
			});
			asyncResult.PendingAsyncResult = clientState.CommClient.BeginCommand(clientState.CachedCommand, clientState, new AsyncCallback(IMAPClient.OnEndGetMessageInfoByRangeInternal), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		internal static AsyncOperationResult<IMAPResultData> EndGetMessageInfoByRange(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IMAPResultData> asyncResult2 = (AsyncResult<IMAPClientState, IMAPResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginGetMessageItemByUid(IMAPClientState clientState, string uid, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)818UL, IMAPClient.Tracer, "IMAP Fetch Message Body.  Uid={0}.", new object[]
			{
				uid
			});
			clientState.CachedCommand.ResetAsFetch(clientState.UniqueCommandId, uid, null, true, IMAPClient.messageBodyDataItems);
			return IMAPClient.CreateAsyncResultAndBeginCommand<IMAPResultData>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndGetMessageBodyInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<IMAPResultData> EndGetMessageItemByUid(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IMAPResultData> asyncResult2 = (AsyncResult<IMAPClientState, IMAPResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginAppendMessageToIMAPMailbox(IMAPClientState clientState, string mailboxName, IMAPMailFlags messageFlags, Stream messageMimeStream, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogDebugging((TSLID)819UL, IMAPClient.Tracer, "Appending message to mailbox {0}", new object[]
			{
				mailboxName
			});
			clientState.CachedCommand.ResetAsAppend(clientState.UniqueCommandId, mailboxName, messageFlags, messageMimeStream);
			return IMAPClient.CreateAsyncResultAndBeginCommand<string>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndAppendMessageInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<string> EndAppendMessageToIMAPMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, string> asyncResult2 = (AsyncResult<IMAPClientState, string>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginSearchForMessageByMessageId(IMAPClientState clientState, string messageId, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)820UL, IMAPClient.Tracer, "Searching for message by ID.  ID = {0}", new object[]
			{
				messageId
			});
			clientState.CachedCommand.ResetAsSearch(clientState.UniqueCommandId, new string[]
			{
				"HEADER Message-Id",
				messageId
			});
			return IMAPClient.CreateAsyncResultAndBeginCommand<IList<string>>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndSearchForMessageInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<IList<string>> EndSearchForMessageByMessageId(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IList<string>> asyncResult2 = (AsyncResult<IMAPClientState, IList<string>>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginStoreMessageFlags(IMAPClientState clientState, string uid, IMAPMailFlags flagsToStore, IMAPMailFlags previousFlags, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)821UL, IMAPClient.Tracer, "Storing flags for message.  UID = {0}", new object[]
			{
				uid
			});
			IMAPMailFlags imapmailFlags = flagsToStore & ~previousFlags;
			IMAPMailFlags imapmailFlags2 = ~flagsToStore & previousFlags;
			if (imapmailFlags != IMAPMailFlags.None)
			{
				clientState.CachedCommand.ResetAsUidStore(clientState.UniqueCommandId, uid, imapmailFlags, true);
				clientState.FlagsToRemove = imapmailFlags2;
				return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndStoreMessageFlagsInternal), callback, callbackState, syncPoisonContext);
			}
			if (imapmailFlags2 != IMAPMailFlags.None)
			{
				clientState.CachedCommand.ResetAsUidStore(clientState.UniqueCommandId, uid, imapmailFlags2, false);
				clientState.FlagsToRemove = IMAPMailFlags.None;
				return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndStoreMessageFlagsInternal), callback, callbackState, syncPoisonContext);
			}
			clientState.Log.LogError((TSLID)822UL, IMAPClient.Tracer, "Attempt to store the same flags that were already set.", new object[0]);
			return null;
		}

		internal static AsyncOperationResult<DBNull> EndStoreMessageFlags(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginCreateImapMailbox(IMAPClientState clientState, string mailboxName, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)823UL, IMAPClient.Tracer, "Creating new IMAP Mailbox.  Name = {0}", new object[]
			{
				mailboxName
			});
			clientState.CachedCommand.ResetAsCreate(clientState.UniqueCommandId, mailboxName);
			return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndCreateImapMailboxInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<DBNull> EndCreateImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginDeleteImapMailbox(IMAPClientState clientState, string mailboxName, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)824UL, IMAPClient.Tracer, "Deleting IMAP Mailbox.  Name = {0}", new object[]
			{
				mailboxName
			});
			clientState.CachedCommand.ResetAsDelete(clientState.UniqueCommandId, mailboxName);
			return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndDeleteImapMailboxInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<DBNull> EndDeleteImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginRenameImapMailbox(IMAPClientState clientState, string oldMailboxName, string newMailboxName, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)825UL, IMAPClient.Tracer, "Renaming IMAP Mailbox.  {0} => {1}", new object[]
			{
				oldMailboxName,
				newMailboxName
			});
			clientState.CachedCommand.ResetAsRename(clientState.UniqueCommandId, oldMailboxName, newMailboxName);
			return IMAPClient.CreateAsyncResultAndBeginCommand<DBNull>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndRenameImapMailboxInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<DBNull> EndRenameImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginListImapMailboxesByLevel(IMAPClientState clientState, int level, char separator, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			clientState.Log.LogRawData((TSLID)826UL, IMAPClient.Tracer, "Listing mailboxes at level {0}", new object[]
			{
				level
			});
			clientState.InitializeRootPathProcessingFlags(level, separator);
			clientState.CachedCommand.ResetAsList(clientState.UniqueCommandId, separator, new int?(level), clientState.RootFolderPath);
			return IMAPClient.CreateAsyncResultAndBeginCommand<IList<IMAPMailbox>>(clientState, clientState.CachedCommand, true, new AsyncCallback(IMAPClient.OnEndListImapMailboxesInternal), callback, callbackState, syncPoisonContext);
		}

		internal static AsyncOperationResult<IList<IMAPMailbox>> EndListImapMailboxes(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IList<IMAPMailbox>> asyncResult2 = (AsyncResult<IMAPClientState, IList<IMAPMailbox>>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static IAsyncResult BeginLogOff(IMAPClientState clientState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult = new AsyncResult<IMAPClientState, DBNull>(clientState, clientState, callback, callbackState, syncPoisonContext);
			clientState.Log.LogRawData((TSLID)827UL, "Logging out", new object[0]);
			clientState.CachedCommand.ResetAsLogout(clientState.UniqueCommandId);
			try
			{
				asyncResult.PendingAsyncResult = clientState.CommClient.BeginCommand(clientState.CachedCommand, false, clientState, new AsyncCallback(IMAPClient.OnEndLogOffInternal), asyncResult, syncPoisonContext);
			}
			catch (InvalidOperationException ex)
			{
				string message = "BUG: BeginLogOff : should never throw InvalidOperationException.";
				clientState.Log.LogError((TSLID)828UL, IMAPClient.Tracer, "Caught InvalidOperationException while logging off.  Ignoring.  Exception = {0}", new object[]
				{
					ex
				});
				clientState.Log.ReportWatson(message, ex);
			}
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted(DBNull.Value);
			return asyncResult;
		}

		internal static AsyncOperationResult<DBNull> EndLogOff(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		internal static void Cancel(IMAPClientState clientState)
		{
			clientState.CommClient.Cancel();
		}

		private static void OnEndConnectToStarttls(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = state.CommClient.EndConnect(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, "Connecting", asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			state.CachedCommand.ResetAsStarttls(state.UniqueCommandId);
			asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndStarttlsToBeginTlsNegotiation), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndStarttlsToBeginTlsNegotiation(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = state.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			if (data.Status != IMAPStatus.Ok)
			{
				data.FailureException = IMAPClient.BuildFailureException(state.CachedCommand, data.Status);
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(null, data.FailureException);
				return;
			}
			asyncResult2.PendingAsyncResult = state.CommClient.BeginNegotiateTlsAsClient(state, new AsyncCallback(IMAPClient.OnEndConnectToAuthenticate), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndConnectToAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			switch (state.IMAPSecurityMechanism)
			{
			case IMAPSecurityMechanism.None:
			case IMAPSecurityMechanism.Ssl:
			{
				AsyncOperationResult<IMAPResultData> asyncOperationResult = state.CommClient.EndConnect(asyncResult);
				if (!asyncOperationResult.IsSucceeded)
				{
					IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, "Connecting", asyncOperationResult.Exception);
					asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
					return;
				}
				break;
			}
			case IMAPSecurityMechanism.Tls:
			{
				AsyncOperationResult<IMAPResultData> asyncOperationResult2 = state.CommClient.EndNegotiateTlsAsClient(asyncResult);
				if (!asyncOperationResult2.IsSucceeded)
				{
					IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, "Tls negotiation", asyncOperationResult2.Exception);
					asyncResult2.ProcessCompleted(null, asyncOperationResult2.Exception);
					return;
				}
				break;
			}
			default:
				throw new InvalidOperationException("Unexpected security mechanism " + state.IMAPSecurityMechanism);
			}
			IMAPAuthenticationMechanism imapauthenticationMechanism = state.IMAPAuthenticationMechanism;
			if (imapauthenticationMechanism == IMAPAuthenticationMechanism.Basic)
			{
				state.CachedCommand.ResetAsLogin(state.UniqueCommandId, state.LogonName, state.LogonPassword);
				asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndLoginFallbackToAuthenticatePlainIfNeeded), asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
			if (imapauthenticationMechanism != IMAPAuthenticationMechanism.Ntlm)
			{
				throw new InvalidOperationException("Unexpected authentication mechanism" + state.IMAPAuthenticationMechanism);
			}
			AuthenticationContext authContext = null;
			state.CachedCommand.ResetAsAuthenticate(state.UniqueCommandId, IMAPAuthenticationMechanism.Ntlm, state.LogonName, state.LogonPassword, authContext);
			asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndCompleteConnectAndAuthenticate), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndCompleteConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = asyncResult2.State.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			Exception ex = null;
			if (data.Status == IMAPStatus.No)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new IMAPAuthenticationException(), true);
			}
			else if (data.Status == IMAPStatus.Bad)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, IMAPClient.BuildBadResponseInnerException(asyncResult2.State.CachedCommand), true);
			}
			else if (data.Status != IMAPStatus.Ok)
			{
				ex = IMAPClient.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
			}
			if (ex == null)
			{
				asyncResult2.State.Log.LogRawData((TSLID)829UL, "{0}: Command completed successfully.", new object[]
				{
					asyncResult2.State.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(DBNull.Value, ex);
		}

		private static void OnEndLoginFallbackToAuthenticatePlainIfNeeded(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = state.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			Exception ex = null;
			if (data.Status == IMAPStatus.No || data.Status == IMAPStatus.Bad)
			{
				AuthenticationContext authContext = null;
				state.CachedCommand.ResetAsAuthenticate(state.UniqueCommandId, IMAPAuthenticationMechanism.Basic, state.LogonName, state.LogonPassword, authContext);
				asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndCompleteConnectAndAuthenticate), asyncResult2, asyncResult2.SyncPoisonContext);
				return;
			}
			if (data.Status != IMAPStatus.Ok)
			{
				ex = IMAPClient.BuildFailureException(state.CachedCommand, data.Status);
			}
			if (ex == null)
			{
				state.Log.LogRawData((TSLID)1240UL, "{0}: Command completed successfully.", new object[]
				{
					state.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(DBNull.Value, ex);
		}

		private static void OnEndCapabilityInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<IList<string>>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState clientState)
			{
				if (resultData.IsParseSuccessful)
				{
					clientState.Log.LogRawData((TSLID)830UL, IMAPClient.Tracer, "Found capabilities from server", new object[0]);
					return resultData.Capabilities;
				}
				return null;
			});
		}

		private static void OnEndExpungeInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (IMAPResultData resultData, IMAPClientState clientState) => DBNull.Value);
		}

		private static void OnEndSelectImapMailboxCallStatusIfNeeded(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IMAPMailbox> asyncResult2 = (AsyncResult<IMAPClientState, IMAPMailbox>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			Exception ex = null;
			IMAPMailbox imapmailbox = null;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = state.CommClient.EndCommand(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				IMAPResultData data = asyncOperationResult.Data;
				if (data.IsParseSuccessful && data.Mailboxes.Count == 1)
				{
					if (data.Mailboxes[0].NumberOfMessages == null)
					{
						ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException("Mailbox is missing EXISTS data."), true);
					}
					else
					{
						imapmailbox = data.Mailboxes[0];
						if (data.Mailboxes[0].UidNext == null)
						{
							state.CachedCommand.ResetAsStatus(state.UniqueCommandId, imapmailbox);
							asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndSelectFollowedByStatus), asyncResult2, asyncResult2.SyncPoisonContext);
							return;
						}
						state.Log.LogRawData((TSLID)831UL, IMAPClient.Tracer, "Selected mailbox {0}", new object[]
						{
							imapmailbox.Name
						});
					}
				}
				else
				{
					ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException("Failed to select mailbox"), true);
				}
			}
			else
			{
				ex = asyncOperationResult.Exception;
			}
			if (ex == null)
			{
				state.Log.LogRawData((TSLID)1160UL, "{0}: Command completed successfully.", new object[]
				{
					state.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(imapmailbox, ex);
		}

		private static void OnEndSelectFollowedByStatus(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<IMAPMailbox>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState clientState)
			{
				if (resultData.IsParseSuccessful && resultData.Mailboxes.Count == 1 && resultData.Mailboxes[0].UidNext != null)
				{
					clientState.Log.LogRawData((TSLID)1161UL, IMAPClient.Tracer, "Selected mailbox {0} after succesful STATUS command", new object[]
					{
						resultData.Mailboxes[0].Name
					});
					return resultData.Mailboxes[0];
				}
				resultData.FailureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(string.Format("Failed to get STATUS for mailbox. {0}", resultData.IsParseSuccessful ? ((resultData.Mailboxes.Count == 1) ? "Missing UIDNEXT" : "No mailbox returned") : " Parsing unsuccessful.")), true);
				return null;
			});
		}

		private static void OnEndGetMessageInfoByRangeInternal(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, IMAPResultData> asyncResult2 = (AsyncResult<IMAPClientState, IMAPResultData>)asyncResult.AsyncState;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = asyncResult2.State.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			if (data.Status != IMAPStatus.Ok)
			{
				data.FailureException = IMAPClient.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(null, data.FailureException);
				return;
			}
			if (!data.IsParseSuccessful)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, data.FailureException);
			}
			asyncResult2.ProcessCompleted(data, data.FailureException);
		}

		private static void OnEndGetMessageBodyInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<IMAPResultData>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState clientState)
			{
				if (resultData.IsParseSuccessful)
				{
					clientState.Log.LogRawData((TSLID)832UL, IMAPClient.Tracer, "Successful UID FETCH of message body", new object[0]);
					IList<string> messageUids = resultData.MessageUids;
					if (messageUids.Count != 1)
					{
						clientState.Log.LogRawData((TSLID)833UL, IMAPClient.Tracer, "Unexpected number of UIDs returned during single message fetch: {0}", new object[]
						{
							messageUids.Count
						});
					}
				}
				clientState.ActivatePerfMsgDownloadEvent(clientState, null);
				return resultData;
			});
		}

		private static void OnEndAppendMessageInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<string>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState clientState)
			{
				clientState.ActivatePerfMsgUploadEvent(clientState, null);
				if (resultData.MessageUids != null && resultData.MessageUids.Count > 0)
				{
					return resultData.MessageUids[0];
				}
				return null;
			});
		}

		private static void OnEndSearchForMessageInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<IList<string>>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState state)
			{
				if (resultData.IsParseSuccessful)
				{
					return resultData.MessageUids;
				}
				return null;
			});
		}

		private static void OnEndStoreMessageFlagsInternal(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			if (state.FlagsToRemove == IMAPMailFlags.None)
			{
				IMAPClient.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (IMAPResultData resultData, IMAPClientState clientState) => DBNull.Value);
				return;
			}
			IMAPMailFlags flagsToRemove = state.FlagsToRemove;
			state.FlagsToRemove = IMAPMailFlags.None;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = asyncResult2.State.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(DBNull.Value, asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			if (data.Status != IMAPStatus.Ok)
			{
				data.FailureException = IMAPClient.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(DBNull.Value, data.FailureException);
				return;
			}
			if (!data.IsParseSuccessful)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(DBNull.Value, data.FailureException);
				return;
			}
			state.CachedCommand.ResetAsUidStore(state.UniqueCommandId, (string)state.CachedCommand.CommandParameters[0], flagsToRemove, false);
			asyncResult2.PendingAsyncResult = state.CommClient.BeginCommand(state.CachedCommand, state, new AsyncCallback(IMAPClient.OnEndStoreMessageFlagsInternal), asyncResult2, asyncResult2.SyncPoisonContext);
		}

		private static void OnEndCreateImapMailboxInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (IMAPResultData resultData, IMAPClientState clientState) => DBNull.Value);
		}

		private static void OnEndDeleteImapMailboxInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (IMAPResultData resultData, IMAPClientState state) => DBNull.Value);
		}

		private static void OnEndRenameImapMailboxInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (IMAPResultData resultData, IMAPClientState state) => DBNull.Value);
		}

		private static void OnEndListImapMailboxesInternal(IAsyncResult asyncResult)
		{
			IMAPClient.ProcessResultAndCompleteRequest<IList<IMAPMailbox>>(asyncResult, delegate(IMAPResultData resultData, IMAPClientState state)
			{
				if (!resultData.IsParseSuccessful || resultData.FailureException != null)
				{
					return null;
				}
				if (!string.IsNullOrEmpty(state.RootFolderPath) && (int?)state.CachedCommand.CommandParameters[0] != null)
				{
					state.Log.LogDebugging((TSLID)834UL, IMAPClient.Tracer, "Need to post-process the LIST results, with RootFolderPath: {0}", new object[]
					{
						state.RootFolderPath
					});
					if ((state.RootPathProcessingFlags & RootPathProcessingFlags.UnableToProcess) == RootPathProcessingFlags.UnableToProcess)
					{
						resultData.FailureException = IMAPResponse.Fail("Failed to process LIST command with root folder path", state.CachedCommand, state.RootFolderPath);
						IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, resultData.FailureException);
						resultData.Mailboxes.Clear();
						return null;
					}
					int value = ((int?)state.CachedCommand.CommandParameters[0]).Value;
					char c = (char)state.CachedCommand.CommandParameters[1];
					bool flag = false;
					if (resultData.Mailboxes.Count > 0)
					{
						if (resultData.Mailboxes[0].Separator != null)
						{
							c = resultData.Mailboxes[0].Separator.Value;
						}
						state.UpdateRootPathProcessingFlags(IMAPClient.Tracer, resultData.Mailboxes[0].NameOnTheWire, c, new int?(value), resultData.Mailboxes.Count);
						if (value == 1 && (state.RootPathProcessingFlags & RootPathProcessingFlags.FolderPathPrefixIsInbox) == RootPathProcessingFlags.FolderPathPrefixIsInbox)
						{
							resultData.Mailboxes[0].Name = IMAPMailbox.Inbox;
							flag = true;
							state.Log.LogVerbose((TSLID)1408UL, IMAPClient.Tracer, "The mailbox {0} is the only level 1 folder and it's the same as the path prefix. It will be treated as being the user's Inbox", new object[]
							{
								resultData.Mailboxes[0].NameOnTheWire
							});
						}
						else
						{
							foreach (IMAPMailbox imapmailbox in resultData.Mailboxes)
							{
								string imapmailboxNameFromWireName = IMAPClient.GetIMAPMailboxNameFromWireName(state, imapmailbox.NameOnTheWire, c);
								if (imapmailboxNameFromWireName == null)
								{
									resultData.FailureException = IMAPResponse.Fail("Failed to process LIST command with root folder path", state.CachedCommand, state.RootFolderPath);
									IMAPUtils.LogExceptionDetails(state.Log, IMAPClient.Tracer, state.CachedCommand, resultData.FailureException);
									resultData.Mailboxes.Clear();
									return null;
								}
								imapmailbox.Name = imapmailboxNameFromWireName;
								if (value == 1 && string.Equals(imapmailbox.Name, IMAPMailbox.Inbox, StringComparison.InvariantCultureIgnoreCase))
								{
									flag = true;
								}
							}
						}
					}
					if (value == 1 && !flag)
					{
						foreach (IMAPMailbox imapmailbox2 in resultData.Mailboxes)
						{
							imapmailbox2.Name = IMAPMailbox.Inbox + c.ToString() + imapmailbox2.Name;
						}
						IMAPMailbox imapmailbox3 = new IMAPMailbox(state.RootFolderPath.TrimEnd(new char[]
						{
							c
						}));
						imapmailbox3.Name = IMAPMailbox.Inbox;
						resultData.Mailboxes.Insert(0, imapmailbox3);
						state.UpdateRootPathProcessingFlags(IMAPClient.Tracer, RootPathProcessingFlags.FolderPathPrefixIsInbox);
						state.Log.LogVerbose((TSLID)835UL, IMAPClient.Tracer, "Treating the RootFolderPath prefix {0} as being the user's Inbox", new object[]
						{
							imapmailbox3.NameOnTheWire
						});
					}
				}
				return resultData.Mailboxes;
			});
		}

		private static void OnEndLogOffInternal(IAsyncResult asyncResult)
		{
		}

		private static IAsyncResult CreateAsyncResultAndBeginCommand<TOutput>(IMAPClientState clientState, IMAPCommand commandToExecute, bool processResponse, AsyncCallback commandCompletionCallback, AsyncCallback externalCallback, object externalCallbackState, object syncPoisonContext) where TOutput : class
		{
			AsyncResult<IMAPClientState, TOutput> asyncResult = new AsyncResult<IMAPClientState, TOutput>(clientState, clientState, externalCallback, externalCallbackState, syncPoisonContext);
			asyncResult.PendingAsyncResult = clientState.CommClient.BeginCommand(commandToExecute, processResponse, clientState, commandCompletionCallback, asyncResult, syncPoisonContext);
			return asyncResult;
		}

		private static void ProcessResultAndCompleteRequest<TOutput>(IAsyncResult asyncResult, IMAPClient.ResultConverter<TOutput> resultConverter) where TOutput : class
		{
			AsyncResult<IMAPClientState, TOutput> asyncResult2 = (AsyncResult<IMAPClientState, TOutput>)asyncResult.AsyncState;
			AsyncOperationResult<IMAPResultData> asyncOperationResult = asyncResult2.State.CommClient.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			IMAPResultData data = asyncOperationResult.Data;
			TOutput result = default(TOutput);
			if (data.Status == IMAPStatus.Ok)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2271620413U);
				result = resultConverter(data, asyncResult2.State);
			}
			else
			{
				data.FailureException = IMAPClient.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
			}
			if (data.FailureException == null)
			{
				asyncResult2.State.Log.LogRawData((TSLID)836UL, IMAPClient.Tracer, "{0}: Command completed successfully.", new object[]
				{
					asyncResult2.State.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				IMAPUtils.LogExceptionDetails(asyncResult2.State.Log, IMAPClient.Tracer, asyncResult2.State.CachedCommand, data.FailureException);
			}
			asyncResult2.ProcessCompleted(result, data.FailureException);
		}

		private static string GetIMAPMailboxNameFromWireName(IMAPClientState state, string mailboxName, char separator)
		{
			if (string.IsNullOrEmpty(state.RootFolderPath))
			{
				return mailboxName;
			}
			if ((state.RootPathProcessingFlags & RootPathProcessingFlags.UnableToProcess) == RootPathProcessingFlags.UnableToProcess)
			{
				state.Log.LogDebugging((TSLID)837UL, IMAPClient.Tracer, "Wire name {0} not translated. Unable to process server responses.", new object[]
				{
					mailboxName
				});
				return null;
			}
			string text = null;
			if ((state.RootPathProcessingFlags & RootPathProcessingFlags.FolderPathPrefixIsInbox) == RootPathProcessingFlags.FolderPathPrefixIsInbox)
			{
				if ((state.RootPathProcessingFlags & RootPathProcessingFlags.ResponseIncludesRootPathPrefix) == RootPathProcessingFlags.ResponseIncludesRootPathPrefix)
				{
					if (!mailboxName.StartsWith(state.RootFolderPath))
					{
						state.Log.LogDebugging((TSLID)838UL, IMAPClient.Tracer, "Wire name {0} not translated. It does not begin with expected prefix {1}", new object[]
						{
							mailboxName,
							state.RootFolderPath
						});
						return null;
					}
					text = IMAPMailbox.Inbox + separator.ToString() + mailboxName.Substring(state.RootFolderPath.Length);
				}
				else
				{
					text = IMAPMailbox.Inbox + separator.ToString() + mailboxName;
				}
			}
			else if ((state.RootPathProcessingFlags & RootPathProcessingFlags.ResponseIncludesRootPathPrefix) == RootPathProcessingFlags.ResponseIncludesRootPathPrefix)
			{
				if (!mailboxName.StartsWith(state.RootFolderPath))
				{
					state.Log.LogDebugging((TSLID)840UL, IMAPClient.Tracer, "Wire name {0} not translated. It does not begin with expected prefix {1}", new object[]
					{
						mailboxName,
						state.RootFolderPath
					});
					return null;
				}
				text = mailboxName.Substring(state.RootFolderPath.Length);
				if (string.IsNullOrEmpty(text))
				{
					state.Log.LogDebugging((TSLID)839UL, IMAPClient.Tracer, "Wire name {0} not translated. Removing the prefix {1} makes it an empty string", new object[]
					{
						mailboxName,
						state.RootFolderPath
					});
					return null;
				}
			}
			state.Log.LogDebugging((TSLID)841UL, IMAPClient.Tracer, "Wire name {0} translated to actual name {1}", new object[]
			{
				mailboxName,
				text
			});
			return text;
		}

		private static Exception BuildFailureException(IMAPCommand command, IMAPStatus completionCode)
		{
			switch (completionCode)
			{
			case IMAPStatus.No:
				return SyncTransientException.CreateItemLevelException(new IMAPException(string.Format(CultureInfo.InvariantCulture, "Error while executing [{0}]", new object[]
				{
					(command == null) ? "Initial handshake" : command.ToPiiCleanString()
				})));
			case IMAPStatus.Bad:
				return SyncPermanentException.CreateItemLevelException(IMAPClient.BuildBadResponseInnerException(command));
			case IMAPStatus.Bye:
				return SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, new IMAPException("IMAP Server disconnected."), true);
			default:
				throw new InvalidOperationException("Unknown response failure code: " + completionCode);
			}
		}

		private static Exception BuildBadResponseInnerException(IMAPCommand command)
		{
			return new IMAPException(string.Format(CultureInfo.InvariantCulture, "Error while executing [{0}]", new object[]
			{
				(command == null) ? "Initial handshake" : command.ToPiiCleanString()
			}));
		}

		internal static readonly Trace Tracer = ExTraceGlobals.IMAPClientTracer;

		internal static readonly string ImapComponentId = "IMAP";

		internal static IList<string> MessageInfoDataItemsForChangesOnly = new List<string>(new string[]
		{
			"UID",
			"FLAGS"
		}).AsReadOnly();

		internal static IList<string> MessageInfoDataItemsForUidValidityRecovery = new List<string>(new string[]
		{
			"UID",
			"BODY.PEEK[HEADER.FIELDS (Message-ID)]"
		}).AsReadOnly();

		internal static IList<string> MessageInfoDataItemsForNewMessages = new List<string>(new string[]
		{
			"UID",
			"FLAGS",
			"BODY.PEEK[HEADER.FIELDS (Message-ID)]",
			"RFC822.SIZE"
		}).AsReadOnly();

		private static IList<string> messageBodyDataItems = new List<string>(new string[]
		{
			"UID",
			"INTERNALDATE",
			"BODY.PEEK[]"
		}).AsReadOnly();

		private delegate TOutput ResultConverter<TOutput>(IMAPResultData resultData, IMAPClientState state);
	}
}
