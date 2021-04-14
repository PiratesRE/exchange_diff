using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ImapConnectionCore
	{
		public static IAsyncResult BeginConnectAndAuthenticate(ImapConnectionContext context, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Beginning connect-and-authenticate process", new object[0]);
			AsyncResult<ImapConnectionContext, DBNull> asyncResult = new AsyncResult<ImapConnectionContext, DBNull>(context, context, callback, callbackState);
			AsyncCallback callback2;
			switch (context.ImapSecurityMechanism)
			{
			case ImapSecurityMechanism.None:
			case ImapSecurityMechanism.Ssl:
				callback2 = new AsyncCallback(ImapConnectionCore.OnEndConnectToAuthenticate);
				break;
			case ImapSecurityMechanism.Tls:
				callback2 = new AsyncCallback(ImapConnectionCore.OnEndConnectToStarttls);
				break;
			default:
				throw new InvalidOperationException("Unexpected security mechanism " + context.ImapSecurityMechanism);
			}
			asyncResult.PendingAsyncResult = context.NetworkFacade.BeginConnect(context, callback2, asyncResult);
			return asyncResult;
		}

		public static AsyncOperationResult<DBNull> EndConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginCapabilities(ImapConnectionContext context, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Getting server capabilities", new object[0]);
			context.CachedCommand.ResetAsCapability(context.UniqueCommandId());
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<ImapServerCapabilities>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndCapabilityInternal), callback, callbackState);
		}

		public static AsyncOperationResult<ImapServerCapabilities> EndCapabilities(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapServerCapabilities> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapServerCapabilities>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginExpunge(ImapConnectionContext context, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Expunging messages.", new object[0]);
			context.CachedCommand.ResetAsExpunge(context.UniqueCommandId());
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndExpungeInternal), callback, callbackState);
		}

		public static AsyncOperationResult<DBNull> EndExpunge(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginSelectImapMailbox(ImapConnectionContext context, ImapMailbox imapMailbox, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Selecting mailbox {0}", new object[]
			{
				imapMailbox.Name
			});
			context.CachedCommand.ResetAsSelect(context.UniqueCommandId(), imapMailbox);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<ImapMailbox>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndSelectImapMailboxCallStatusIfNeeded), callback, callbackState);
		}

		public static AsyncOperationResult<ImapMailbox> EndSelectImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapMailbox> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapMailbox>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginGetMessageInfoByRange(ImapConnectionContext context, string start, string end, bool uidFetch, IList<string> messageDataItems, AsyncCallback callback, object callbackState)
		{
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult = new AsyncResult<ImapConnectionContext, ImapResultData>(context, context, callback, callbackState);
			context.CachedCommand.ResetAsFetch(context.UniqueCommandId(), start, end, uidFetch, messageDataItems);
			context.Log.Debug("IMAP Fetch Message Headers in range.  Start={0}.  End={1}", new object[]
			{
				start,
				end
			});
			asyncResult.PendingAsyncResult = context.NetworkFacade.BeginCommand(context.CachedCommand, context, new AsyncCallback(ImapConnectionCore.OnEndGetMessageInfoByRangeInternal), asyncResult);
			return asyncResult;
		}

		public static AsyncOperationResult<ImapResultData> EndGetMessageInfoByRange(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginGetMessageItemByUid(ImapConnectionContext context, string uid, IList<string> messageBodyDataItems, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("IMAP Fetch Message Body.  Uid={0}.", new object[]
			{
				uid
			});
			context.CachedCommand.ResetAsFetch(context.UniqueCommandId(), uid, null, true, messageBodyDataItems);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<ImapResultData>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndGetMessageBodyInternal), callback, callbackState);
		}

		public static AsyncOperationResult<ImapResultData> EndGetMessageItemByUid(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginAppendMessageToImapMailbox(ImapConnectionContext context, string mailboxName, ImapMailFlags messageFlags, Stream messageMimeStream, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Appending message to mailbox {0}", new object[]
			{
				mailboxName
			});
			context.CachedCommand.ResetAsAppend(context.UniqueCommandId(), mailboxName, messageFlags, messageMimeStream);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<string>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndAppendMessageInternal), callback, callbackState);
		}

		public static AsyncOperationResult<string> EndAppendMessageToImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, string> asyncResult2 = (AsyncResult<ImapConnectionContext, string>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginSearchForMessageByMessageId(ImapConnectionContext context, string messageId, AsyncCallback callback = null, object callbackState = null)
		{
			context.Log.Debug("Searching for message by ID.  ID = {0}", new object[]
			{
				messageId
			});
			context.CachedCommand.ResetAsSearch(context.UniqueCommandId(), new string[]
			{
				"HEADER Message-Id",
				messageId
			});
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<IList<string>>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndSearchForMessageInternal), callback, callbackState);
		}

		public static AsyncOperationResult<IList<string>> EndSearchForMessageByMessageId(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, IList<string>> asyncResult2 = (AsyncResult<ImapConnectionContext, IList<string>>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginStoreMessageFlags(ImapConnectionContext context, string uid, ImapMailFlags flagsToStore, ImapMailFlags previousFlags, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Storing flags for message.  UID = {0}", new object[]
			{
				uid
			});
			ImapMailFlags imapMailFlags = flagsToStore & ~previousFlags;
			ImapMailFlags imapMailFlags2 = ~flagsToStore & previousFlags;
			if (imapMailFlags != ImapMailFlags.None)
			{
				context.CachedCommand.ResetAsUidStore(context.UniqueCommandId(), uid, imapMailFlags, true);
				context.FlagsToRemove = imapMailFlags2;
				return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndStoreMessageFlagsInternal), callback, callbackState);
			}
			if (imapMailFlags2 != ImapMailFlags.None)
			{
				context.CachedCommand.ResetAsUidStore(context.UniqueCommandId(), uid, imapMailFlags2, false);
				context.FlagsToRemove = ImapMailFlags.None;
				return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndStoreMessageFlagsInternal), callback, callbackState);
			}
			context.Log.Error("Attempt to store the same flags that were already set.", new object[0]);
			return null;
		}

		public static AsyncOperationResult<DBNull> EndStoreMessageFlags(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginCreateImapMailbox(ImapConnectionContext context, string mailboxName, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Creating new IMAP mailbox.  Name = {0}", new object[]
			{
				mailboxName
			});
			context.CachedCommand.ResetAsCreate(context.UniqueCommandId(), mailboxName);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndCreateImapMailboxInternal), callback, callbackState);
		}

		public static AsyncOperationResult<DBNull> EndCreateImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginDeleteImapMailbox(ImapConnectionContext context, string mailboxName, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Deleting IMAP mailbox.  Name = {0}", new object[]
			{
				mailboxName
			});
			context.CachedCommand.ResetAsDelete(context.UniqueCommandId(), mailboxName);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndDeleteImapMailboxInternal), callback, callbackState);
		}

		public static AsyncOperationResult<DBNull> EndDeleteImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginRenameImapMailbox(ImapConnectionContext context, string oldMailboxName, string newMailboxName, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Renaming IMAP mailbox.  {0} => {1}", new object[]
			{
				oldMailboxName,
				newMailboxName
			});
			context.CachedCommand.ResetAsRename(context.UniqueCommandId(), oldMailboxName, newMailboxName);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<DBNull>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndRenameImapMailboxInternal), callback, callbackState);
		}

		public static AsyncOperationResult<DBNull> EndRenameImapMailbox(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginListImapMailboxesByLevel(ImapConnectionContext context, int level, char separator, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Listing mailboxes at level {0}", new object[]
			{
				level
			});
			context.InitializeRootPathProcessingFlags(level, separator);
			context.CachedCommand.ResetAsList(context.UniqueCommandId(), separator, new int?(level), context.RootFolderPath);
			return ImapConnectionCore.CreateAsyncResultAndBeginCommand<IList<ImapMailbox>>(context, context.CachedCommand, true, new AsyncCallback(ImapConnectionCore.OnEndListImapMailboxesInternal), callback, callbackState);
		}

		public static AsyncOperationResult<IList<ImapMailbox>> EndListImapMailboxesByLevel(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, IList<ImapMailbox>> asyncResult2 = (AsyncResult<ImapConnectionContext, IList<ImapMailbox>>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static IAsyncResult BeginLogOff(ImapConnectionContext context, AsyncCallback callback, object callbackState)
		{
			context.Log.Debug("Logging out", new object[0]);
			AsyncResult<ImapConnectionContext, DBNull> asyncResult = new AsyncResult<ImapConnectionContext, DBNull>(context, context, callback, callbackState);
			context.CachedCommand.ResetAsLogout(context.UniqueCommandId());
			try
			{
				asyncResult.PendingAsyncResult = context.NetworkFacade.BeginCommand(context.CachedCommand, false, context, new AsyncCallback(ImapConnectionCore.OnEndLogOffInternal), asyncResult);
			}
			catch (InvalidOperationException ex)
			{
				context.Log.Error("Caught InvalidOperationException while logging off. Exception = {0}", new object[]
				{
					ex
				});
				context.Log.Fatal(ex, "BUG: BeginLogOff : should never throw InvalidOperationException.");
			}
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted(DBNull.Value);
			return asyncResult;
		}

		public static AsyncOperationResult<DBNull> EndLogOff(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public static void Cancel(ImapConnectionContext context)
		{
			context.NetworkFacade.Cancel();
		}

		private static void OnEndConnectToStarttls(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			AsyncOperationResult<ImapResultData> asyncOperationResult = state.NetworkFacade.EndConnect(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(state.Log, "Connecting", asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			state.CachedCommand.ResetAsStarttls(state.UniqueCommandId());
			asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndStarttlsToBeginTlsNegotiation), asyncResult2);
		}

		private static void OnEndStarttlsToBeginTlsNegotiation(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			AsyncOperationResult<ImapResultData> asyncOperationResult = state.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(state.Log, state.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			if (data.Status != ImapStatus.Ok)
			{
				data.FailureException = ImapConnectionCore.BuildFailureException(state.CachedCommand, data.Status);
				ImapUtilities.LogExceptionDetails(state.Log, state.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(null, data.FailureException);
				return;
			}
			asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginNegotiateTlsAsClient(state, new AsyncCallback(ImapConnectionCore.OnEndConnectToAuthenticate), asyncResult2);
		}

		private static void OnEndConnectToAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			switch (state.ImapSecurityMechanism)
			{
			case ImapSecurityMechanism.None:
			case ImapSecurityMechanism.Ssl:
			{
				AsyncOperationResult<ImapResultData> asyncOperationResult = state.NetworkFacade.EndConnect(asyncResult);
				if (!asyncOperationResult.IsSucceeded)
				{
					ImapUtilities.LogExceptionDetails(state.Log, "Connecting", asyncOperationResult.Exception);
					asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
					return;
				}
				break;
			}
			case ImapSecurityMechanism.Tls:
			{
				AsyncOperationResult<ImapResultData> asyncOperationResult2 = state.NetworkFacade.EndNegotiateTlsAsClient(asyncResult);
				if (!asyncOperationResult2.IsSucceeded)
				{
					ImapUtilities.LogExceptionDetails(state.Log, "Tls negotiation", asyncOperationResult2.Exception);
					asyncResult2.ProcessCompleted(null, asyncOperationResult2.Exception);
					return;
				}
				break;
			}
			default:
				throw new InvalidOperationException("Unexpected security mechanism " + state.ImapSecurityMechanism);
			}
			NetworkCredential networkCredential = state.AuthenticationParameters.NetworkCredential;
			ImapAuthenticationMechanism imapAuthenticationMechanism = state.ImapAuthenticationMechanism;
			if (imapAuthenticationMechanism == ImapAuthenticationMechanism.Basic)
			{
				state.CachedCommand.ResetAsLogin(state.UniqueCommandId(), networkCredential.UserName, networkCredential.SecurePassword);
				asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndLoginFallbackToAuthenticatePlainIfNeeded), asyncResult2);
				return;
			}
			if (imapAuthenticationMechanism != ImapAuthenticationMechanism.Ntlm)
			{
				throw new InvalidOperationException("Unexpected authentication mechanism" + state.ImapAuthenticationMechanism);
			}
			AuthenticationContext authContext = null;
			state.CachedCommand.ResetAsAuthenticate(state.UniqueCommandId(), ImapAuthenticationMechanism.Ntlm, networkCredential.UserName, networkCredential.SecurePassword, authContext);
			asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndCompleteConnectAndAuthenticate), asyncResult2);
		}

		private static void OnEndCompleteConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			AsyncOperationResult<ImapResultData> asyncOperationResult = asyncResult2.State.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			Exception ex = null;
			if (data.Status == ImapStatus.No)
			{
				ImapAuthenticationMechanism imapAuthenticationMechanism = asyncResult2.State.ImapAuthenticationMechanism;
				string imapAuthenticationErrorMsg = asyncResult2.State.ServerParameters.Server.ToString();
				ex = new ImapAuthenticationException(imapAuthenticationErrorMsg, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
			}
			else if (data.Status == ImapStatus.Bad)
			{
				ImapAuthenticationMechanism imapAuthenticationMechanism2 = asyncResult2.State.ImapAuthenticationMechanism;
				string imapAuthenticationErrorMsg2 = asyncResult2.State.ServerParameters.Server.ToString();
				Exception innerException = ImapConnectionCore.BuildBadResponseException(asyncResult2.State.CachedCommand);
				ex = new ImapAuthenticationException(imapAuthenticationErrorMsg2, imapAuthenticationMechanism2.ToString(), RetryPolicy.Backoff, innerException);
			}
			else if (data.Status != ImapStatus.Ok)
			{
				ex = ImapConnectionCore.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
			}
			if (ex == null)
			{
				asyncResult2.State.Log.Debug("{0}: Command completed successfully.", new object[]
				{
					asyncResult2.State.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(DBNull.Value, ex);
		}

		private static void OnEndLoginFallbackToAuthenticatePlainIfNeeded(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			AsyncOperationResult<ImapResultData> asyncOperationResult = state.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(state.Log, state.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			Exception ex = null;
			if (data.Status == ImapStatus.No || data.Status == ImapStatus.Bad)
			{
				AuthenticationContext authContext = null;
				NetworkCredential networkCredential = state.AuthenticationParameters.NetworkCredential;
				state.CachedCommand.ResetAsAuthenticate(state.UniqueCommandId(), ImapAuthenticationMechanism.Basic, networkCredential.UserName, networkCredential.SecurePassword, authContext);
				asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndCompleteConnectAndAuthenticate), asyncResult2);
				return;
			}
			if (data.Status != ImapStatus.Ok)
			{
				ex = ImapConnectionCore.BuildFailureException(state.CachedCommand, data.Status);
			}
			if (ex == null)
			{
				state.Log.Debug("{0}: Command completed successfully.", new object[]
				{
					state.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				ImapUtilities.LogExceptionDetails(state.Log, state.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(DBNull.Value, ex);
		}

		private static void OnEndCapabilityInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<ImapServerCapabilities>(asyncResult, new ImapConnectionCore.ResultConverter<ImapServerCapabilities>(ImapConnectionCore.EndCapabilityResultConverter));
		}

		private static ImapServerCapabilities EndCapabilityResultConverter(ImapResultData resultData, ImapConnectionContext context)
		{
			if (resultData.IsParseSuccessful)
			{
				context.Log.Debug("Found capabilities from server", new object[0]);
				return resultData.Capabilities;
			}
			return null;
		}

		private static void OnEndExpungeInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (ImapResultData resultData, ImapConnectionContext context) => DBNull.Value);
		}

		private static void OnEndSelectImapMailboxCallStatusIfNeeded(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapMailbox> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapMailbox>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			Exception ex = null;
			ImapMailbox imapMailbox = null;
			AsyncOperationResult<ImapResultData> asyncOperationResult = state.NetworkFacade.EndCommand(asyncResult);
			if (asyncOperationResult.IsSucceeded)
			{
				ImapResultData data = asyncOperationResult.Data;
				if (data.IsParseSuccessful && data.Mailboxes.Count == 1)
				{
					if (data.Mailboxes[0].NumberOfMessages == null)
					{
						ex = new ImapCommunicationException(CXStrings.ImapNoExistsData, RetryPolicy.Backoff);
					}
					else
					{
						imapMailbox = data.Mailboxes[0];
						if (data.Mailboxes[0].UidNext == null)
						{
							state.CachedCommand.ResetAsStatus(state.UniqueCommandId(), imapMailbox);
							asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndSelectFollowedByStatus), asyncResult2);
							return;
						}
						state.Log.Debug("Selected mailbox {0}", new object[]
						{
							imapMailbox.Name
						});
					}
				}
				else
				{
					ex = new ImapCommunicationException(CXStrings.ImapSelectMailboxFailed, RetryPolicy.Backoff);
				}
			}
			else
			{
				ex = asyncOperationResult.Exception;
			}
			if (ex == null)
			{
				state.Log.Debug("{0}: Command completed successfully.", new object[]
				{
					state.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				ImapUtilities.LogExceptionDetails(state.Log, state.CachedCommand, ex);
			}
			asyncResult2.ProcessCompleted(imapMailbox, ex);
		}

		private static void OnEndSelectFollowedByStatus(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<ImapMailbox>(asyncResult, new ImapConnectionCore.ResultConverter<ImapMailbox>(ImapConnectionCore.EndSelectFollowedByStatusResultConverter));
		}

		private static ImapMailbox EndSelectFollowedByStatusResultConverter(ImapResultData resultData, ImapConnectionContext context)
		{
			if (resultData.IsParseSuccessful && resultData.Mailboxes.Count == 1 && resultData.Mailboxes[0].UidNext != null)
			{
				context.Log.Debug("Selected mailbox {0} after succesful STATUS command", new object[]
				{
					resultData.Mailboxes[0].Name
				});
				return resultData.Mailboxes[0];
			}
			string imapCommunicationErrorMsg = string.Format("Failed to get STATUS for mailbox. {0}", resultData.IsParseSuccessful ? ((resultData.Mailboxes.Count == 1) ? "Missing UIDNEXT" : "No mailbox returned") : " Parsing unsuccessful.");
			resultData.FailureException = new ImapCommunicationException(imapCommunicationErrorMsg, RetryPolicy.Backoff);
			return null;
		}

		private static void OnEndGetMessageInfoByRangeInternal(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
			AsyncOperationResult<ImapResultData> asyncOperationResult = asyncResult2.State.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			if (data.Status != ImapStatus.Ok)
			{
				data.FailureException = ImapConnectionCore.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(null, data.FailureException);
				return;
			}
			if (!data.IsParseSuccessful)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, data.FailureException);
			}
			asyncResult2.ProcessCompleted(data, data.FailureException);
		}

		private static void OnEndGetMessageBodyInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<ImapResultData>(asyncResult, new ImapConnectionCore.ResultConverter<ImapResultData>(ImapConnectionCore.EndGetMessageBodyResultConverter));
		}

		private static ImapResultData EndGetMessageBodyResultConverter(ImapResultData resultData, ImapConnectionContext context)
		{
			if (resultData.IsParseSuccessful)
			{
				context.Log.Debug("Successful UID FETCH of message body", new object[0]);
				IList<string> messageUids = resultData.MessageUids;
				if (messageUids.Count != 1)
				{
					context.Log.Debug("Unexpected number of UIDs returned during single message fetch: {0}", new object[]
					{
						messageUids.Count
					});
				}
			}
			context.ActivatePerfMsgDownloadEvent(context, null);
			return resultData;
		}

		private static void OnEndAppendMessageInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<string>(asyncResult, new ImapConnectionCore.ResultConverter<string>(ImapConnectionCore.EndAppendMessageResultConverter));
		}

		private static string EndAppendMessageResultConverter(ImapResultData resultData, ImapConnectionContext context)
		{
			context.ActivatePerfMsgUploadEvent(context, null);
			if (resultData.MessageUids != null && resultData.MessageUids.Count > 0)
			{
				return resultData.MessageUids[0];
			}
			return null;
		}

		private static void OnEndSearchForMessageInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<IList<string>>(asyncResult, new ImapConnectionCore.ResultConverter<IList<string>>(ImapConnectionCore.EndSearchForMessageResultConverter));
		}

		private static IList<string> EndSearchForMessageResultConverter(ImapResultData resultData, ImapConnectionContext state)
		{
			if (resultData.IsParseSuccessful)
			{
				return resultData.MessageUids;
			}
			return null;
		}

		private static void OnEndStoreMessageFlagsInternal(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, DBNull> asyncResult2 = (AsyncResult<ImapConnectionContext, DBNull>)asyncResult.AsyncState;
			ImapConnectionContext state = asyncResult2.State;
			if (state.FlagsToRemove == ImapMailFlags.None)
			{
				ImapConnectionCore.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (ImapResultData resultData, ImapConnectionContext context) => DBNull.Value);
				return;
			}
			ImapMailFlags flagsToRemove = state.FlagsToRemove;
			state.FlagsToRemove = ImapMailFlags.None;
			AsyncOperationResult<ImapResultData> asyncOperationResult = asyncResult2.State.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(DBNull.Value, asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			if (data.Status != ImapStatus.Ok)
			{
				data.FailureException = ImapConnectionCore.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(DBNull.Value, data.FailureException);
				return;
			}
			if (!data.IsParseSuccessful)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, data.FailureException);
				asyncResult2.ProcessCompleted(DBNull.Value, data.FailureException);
				return;
			}
			state.CachedCommand.ResetAsUidStore(state.UniqueCommandId(), (string)state.CachedCommand.CommandParameters[0], flagsToRemove, false);
			asyncResult2.PendingAsyncResult = state.NetworkFacade.BeginCommand(state.CachedCommand, state, new AsyncCallback(ImapConnectionCore.OnEndStoreMessageFlagsInternal), asyncResult2);
		}

		private static void OnEndCreateImapMailboxInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (ImapResultData resultData, ImapConnectionContext context) => DBNull.Value);
		}

		private static void OnEndDeleteImapMailboxInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (ImapResultData resultData, ImapConnectionContext state) => DBNull.Value);
		}

		private static void OnEndRenameImapMailboxInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<DBNull>(asyncResult, (ImapResultData resultData, ImapConnectionContext state) => DBNull.Value);
		}

		private static void OnEndListImapMailboxesInternal(IAsyncResult asyncResult)
		{
			ImapConnectionCore.ProcessResultAndCompleteRequest<IList<ImapMailbox>>(asyncResult, new ImapConnectionCore.ResultConverter<IList<ImapMailbox>>(ImapConnectionCore.EndListImapMailboxesResultConverter));
		}

		private static IList<ImapMailbox> EndListImapMailboxesResultConverter(ImapResultData resultData, ImapConnectionContext context)
		{
			if (!resultData.IsParseSuccessful || resultData.FailureException != null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(context.RootFolderPath) && (int?)context.CachedCommand.CommandParameters[0] != null)
			{
				context.Log.Debug("Need to post-process the LIST results, with RootFolderPath: {0}", new object[]
				{
					context.RootFolderPath
				});
				if ((context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.UnableToProcess) == ImapRootPathProcessingFlags.UnableToProcess)
				{
					resultData.FailureException = ImapResponse.Fail("Failed to process LIST command with root folder path", context.CachedCommand, context.RootFolderPath);
					ImapUtilities.LogExceptionDetails(context.Log, context.CachedCommand, resultData.FailureException);
					resultData.Mailboxes.Clear();
					return null;
				}
				int value = ((int?)context.CachedCommand.CommandParameters[0]).Value;
				char c = (char)context.CachedCommand.CommandParameters[1];
				bool flag = false;
				if (resultData.Mailboxes.Count > 0)
				{
					if (resultData.Mailboxes[0].Separator != null)
					{
						c = resultData.Mailboxes[0].Separator.Value;
					}
					context.UpdateRootPathProcessingFlags(resultData.Mailboxes[0].NameOnTheWire, c, new int?(value), resultData.Mailboxes.Count);
					if (value == 1 && (context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.FolderPathPrefixIsInbox) == ImapRootPathProcessingFlags.FolderPathPrefixIsInbox)
					{
						context.Log.Assert(resultData.Mailboxes.Count == 1 && (context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix) == ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix, "INBOX decision incorrect", new object[0]);
						resultData.Mailboxes[0].Name = ImapMailbox.Inbox;
						flag = true;
						context.Log.Debug("The mailbox {0} is the only level 1 folder and it's the same as the path prefix. It will be treated as being the user's Inbox", new object[]
						{
							resultData.Mailboxes[0].NameOnTheWire
						});
					}
					else
					{
						foreach (ImapMailbox imapMailbox in resultData.Mailboxes)
						{
							string imapMailboxNameFromWireName = ImapConnectionCore.GetImapMailboxNameFromWireName(context, imapMailbox.NameOnTheWire, c);
							if (imapMailboxNameFromWireName == null)
							{
								resultData.FailureException = ImapResponse.Fail("Failed to process LIST command with root folder path", context.CachedCommand, context.RootFolderPath);
								ImapUtilities.LogExceptionDetails(context.Log, context.CachedCommand, resultData.FailureException);
								resultData.Mailboxes.Clear();
								return null;
							}
							context.Log.Assert(!string.IsNullOrEmpty(imapMailboxNameFromWireName), "GetImapMailboxNameFromWireName should either return null for failure or non-emty string for success", new object[0]);
							imapMailbox.Name = imapMailboxNameFromWireName;
							if (value == 1 && string.Equals(imapMailbox.Name, ImapMailbox.Inbox, StringComparison.InvariantCultureIgnoreCase))
							{
								flag = true;
							}
						}
					}
				}
				if (value == 1 && !flag)
				{
					foreach (ImapMailbox imapMailbox2 in resultData.Mailboxes)
					{
						imapMailbox2.Name = ImapMailbox.Inbox + c.ToString() + imapMailbox2.Name;
					}
					ImapMailbox imapMailbox3 = new ImapMailbox(context.RootFolderPath.TrimEnd(new char[]
					{
						c
					}));
					imapMailbox3.Name = ImapMailbox.Inbox;
					resultData.Mailboxes.Insert(0, imapMailbox3);
					context.UpdateRootPathProcessingFlags(ImapRootPathProcessingFlags.FolderPathPrefixIsInbox);
					context.Log.Debug("Treating the RootFolderPath prefix {0} as being the user's Inbox", new object[]
					{
						imapMailbox3.NameOnTheWire
					});
				}
			}
			return resultData.Mailboxes;
		}

		private static void OnEndLogOffInternal(IAsyncResult asyncResult)
		{
		}

		private static IAsyncResult CreateAsyncResultAndBeginCommand<TOutput>(ImapConnectionContext context, ImapCommand commandToExecute, bool processResponse, AsyncCallback commandCompletionCallback, AsyncCallback externalCallback, object externalCallbackState) where TOutput : class
		{
			AsyncResult<ImapConnectionContext, TOutput> asyncResult = new AsyncResult<ImapConnectionContext, TOutput>(context, context, externalCallback, externalCallbackState);
			asyncResult.PendingAsyncResult = context.NetworkFacade.BeginCommand(commandToExecute, processResponse, context, commandCompletionCallback, asyncResult);
			return asyncResult;
		}

		private static void ProcessResultAndCompleteRequest<TOutput>(IAsyncResult asyncResult, ImapConnectionCore.ResultConverter<TOutput> resultConverter) where TOutput : class
		{
			AsyncResult<ImapConnectionContext, TOutput> asyncResult2 = (AsyncResult<ImapConnectionContext, TOutput>)asyncResult.AsyncState;
			AsyncOperationResult<ImapResultData> asyncOperationResult = asyncResult2.State.NetworkFacade.EndCommand(asyncResult);
			if (!asyncOperationResult.IsSucceeded)
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, asyncOperationResult.Exception);
				asyncResult2.ProcessCompleted(asyncOperationResult.Exception);
				return;
			}
			ImapResultData data = asyncOperationResult.Data;
			TOutput result = default(TOutput);
			if (data.Status == ImapStatus.Ok)
			{
				result = resultConverter(data, asyncResult2.State);
			}
			else
			{
				data.FailureException = ImapConnectionCore.BuildFailureException(asyncResult2.State.CachedCommand, data.Status);
			}
			if (data.FailureException == null)
			{
				asyncResult2.State.Log.Debug("{0}: Command completed successfully.", new object[]
				{
					asyncResult2.State.CachedCommand.ToPiiCleanString()
				});
			}
			else
			{
				ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, asyncResult2.State.CachedCommand, data.FailureException);
			}
			asyncResult2.ProcessCompleted(result, data.FailureException);
		}

		private static string GetImapMailboxNameFromWireName(ImapConnectionContext context, string mailboxName, char separator)
		{
			if (string.IsNullOrEmpty(context.RootFolderPath))
			{
				return mailboxName;
			}
			if ((context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.UnableToProcess) == ImapRootPathProcessingFlags.UnableToProcess)
			{
				context.Log.Debug("Wire name {0} not translated. Unable to process server responses.", new object[]
				{
					mailboxName
				});
				return null;
			}
			context.Log.Assert((context.ImapRootPathProcessingFlags & (ImapRootPathProcessingFlags.FlagsInitialized | ImapRootPathProcessingFlags.FlagsDetermined)) == (ImapRootPathProcessingFlags.FlagsInitialized | ImapRootPathProcessingFlags.FlagsDetermined), "The ImapRootPathProcessingFlags should already be initialized and determined when processing a mailbox name", new object[0]);
			string text = null;
			if ((context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.FolderPathPrefixIsInbox) == ImapRootPathProcessingFlags.FolderPathPrefixIsInbox)
			{
				if ((context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix) == ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix)
				{
					if (!mailboxName.StartsWith(context.RootFolderPath))
					{
						context.Log.Debug("Wire name {0} not translated. It does not begin with expected prefix {1}", new object[]
						{
							mailboxName,
							context.RootFolderPath
						});
						return null;
					}
					text = ImapMailbox.Inbox + separator.ToString() + mailboxName.Substring(context.RootFolderPath.Length);
				}
				else
				{
					text = ImapMailbox.Inbox + separator.ToString() + mailboxName;
				}
			}
			else if ((context.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix) == ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix)
			{
				if (!mailboxName.StartsWith(context.RootFolderPath))
				{
					context.Log.Debug("Wire name {0} not translated. It does not begin with expected prefix {1}", new object[]
					{
						mailboxName,
						context.RootFolderPath
					});
					return null;
				}
				text = mailboxName.Substring(context.RootFolderPath.Length);
				if (string.IsNullOrEmpty(text))
				{
					context.Log.Debug("Wire name {0} not translated. Removing the prefix {1} makes it an empty string", new object[]
					{
						mailboxName,
						context.RootFolderPath
					});
					return null;
				}
			}
			context.Log.Debug("Wire name {0} translated to actual name {1}", new object[]
			{
				mailboxName,
				text
			});
			return text;
		}

		private static Exception BuildFailureException(ImapCommand command, ImapStatus completionCode)
		{
			switch (completionCode)
			{
			case ImapStatus.No:
			{
				string msg = string.Format(CultureInfo.InvariantCulture, "Error while executing [{0}]", new object[]
				{
					(command == null) ? "Initial handshake" : command.ToPiiCleanString()
				});
				return new ItemLevelTransientException(msg);
			}
			case ImapStatus.Bad:
			{
				Exception innerException = ImapConnectionCore.BuildBadResponseException(command);
				return new ItemLevelPermanentException(LocalizedString.Empty, innerException);
			}
			case ImapStatus.Bye:
				return new ImapConnectionException(CXStrings.ImapServerDisconnected, RetryPolicy.Backoff);
			default:
			{
				string message = "Unknown response failure code: " + completionCode;
				throw new InvalidOperationException(message);
			}
			}
		}

		private static Exception BuildBadResponseException(ImapCommand command)
		{
			string failureReason = string.Format(CultureInfo.InvariantCulture, "Error while executing [{0}]", new object[]
			{
				(command == null) ? "Initial handshake" : command.ToPiiCleanString()
			});
			return new ImapBadResponseException(failureReason);
		}

		public static AsyncOperationResult<DBNull> ConnectAndAuthenticate(ImapConnectionContext connectionContext, IServerCapabilities requiredCapabilities = null, AsyncCallback callback = null, object callbackState = null)
		{
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.ConnectAndAuthenticate(connectionContext, callback, callbackState);
			if (!asyncOperationResult.IsSucceeded)
			{
				connectionContext.Log.Debug("Imap.ConnectAndAuthenticate failed, ex: {0}.", new object[]
				{
					asyncOperationResult.Exception
				});
				throw asyncOperationResult.Exception;
			}
			if (requiredCapabilities == null)
			{
				return asyncOperationResult;
			}
			AsyncOperationResult<ImapServerCapabilities> asyncOperationResult2 = ImapConnectionCore.Capabilities(connectionContext, callback, callbackState);
			if (!asyncOperationResult2.IsSucceeded)
			{
				connectionContext.Log.Debug("ImapConnectionCore.ConnectAndAuthenticate.Capabilities ex: ex: {0}", new object[]
				{
					asyncOperationResult2.Exception
				});
				throw asyncOperationResult2.Exception;
			}
			ImapServerCapabilities data = asyncOperationResult2.Data;
			if (data.Supports(requiredCapabilities))
			{
				return asyncOperationResult;
			}
			IEnumerable<string> values = requiredCapabilities.NotIn(data);
			string text = string.Format("Missing capabilities: {0}", string.Join(", ", values));
			connectionContext.Log.Debug("ImapConnectionCore.ConnectAndAuthenticate, missing capabilities: {0}", new object[]
			{
				text
			});
			throw new MissingCapabilitiesException(text);
		}

		public static AsyncOperationResult<DBNull> ConnectAndAuthenticate(ImapConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndConnectAndAuthenticate(ImapConnectionCore.BeginConnectAndAuthenticate(connectionContext, callback, callbackState));
		}

		public static AsyncOperationResult<ImapServerCapabilities> Capabilities(ImapConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndCapabilities(ImapConnectionCore.BeginCapabilities(connectionContext, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> Expunge(ImapConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndExpunge(ImapConnectionCore.BeginExpunge(connectionContext, callback, callbackState));
		}

		public static AsyncOperationResult<ImapMailbox> SelectImapMailbox(ImapConnectionContext connectionContext, ImapMailbox imapMailbox, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndSelectImapMailbox(ImapConnectionCore.BeginSelectImapMailbox(connectionContext, imapMailbox, callback, callbackState));
		}

		public static AsyncOperationResult<ImapResultData> GetMessageInfoByRange(ImapConnectionContext connectionContext, string start, string end, bool uidFetch, IList<string> messageDataItems, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndGetMessageInfoByRange(ImapConnectionCore.BeginGetMessageInfoByRange(connectionContext, start, end, uidFetch, messageDataItems, callback, callbackState));
		}

		public static AsyncOperationResult<ImapResultData> GetMessageItemByUid(ImapConnectionContext connectionContext, string uid, IList<string> messageBodyDataItems, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndGetMessageItemByUid(ImapConnectionCore.BeginGetMessageItemByUid(connectionContext, uid, messageBodyDataItems, callback, callbackState));
		}

		public static AsyncOperationResult<string> AppendMessageToImapMailbox(ImapConnectionContext connectionContext, string mailboxName, ImapMailFlags messageFlags, Stream messageMimeStream, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndAppendMessageToImapMailbox(ImapConnectionCore.BeginAppendMessageToImapMailbox(connectionContext, mailboxName, messageFlags, messageMimeStream, callback, callbackState));
		}

		public static AsyncOperationResult<IList<string>> SearchForMessageByMessageId(ImapConnectionContext connectionContext, string messageId, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndSearchForMessageByMessageId(ImapConnectionCore.BeginSearchForMessageByMessageId(connectionContext, messageId, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> StoreMessageFlags(ImapConnectionContext connectionContext, string uid, ImapMailFlags flagsToStore, ImapMailFlags previousFlags, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndStoreMessageFlags(ImapConnectionCore.BeginStoreMessageFlags(connectionContext, uid, flagsToStore, previousFlags, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> CreateImapMailbox(ImapConnectionContext connectionContext, string mailboxName, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndCreateImapMailbox(ImapConnectionCore.BeginCreateImapMailbox(connectionContext, mailboxName, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> DeleteImapMailbox(ImapConnectionContext connectionContext, string mailboxName, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndDeleteImapMailbox(ImapConnectionCore.BeginDeleteImapMailbox(connectionContext, mailboxName, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> RenameImapMailbox(ImapConnectionContext connectionContext, string oldMailboxName, string newMailboxName, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndRenameImapMailbox(ImapConnectionCore.BeginRenameImapMailbox(connectionContext, oldMailboxName, newMailboxName, callback, callbackState));
		}

		public static AsyncOperationResult<IList<ImapMailbox>> ListImapMailboxesByLevel(ImapConnectionContext connectionContext, int level, char separator, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndListImapMailboxesByLevel(ImapConnectionCore.BeginListImapMailboxesByLevel(connectionContext, level, separator, callback, callbackState));
		}

		public static AsyncOperationResult<DBNull> LogOff(ImapConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return ImapConnectionCore.EndLogOff(ImapConnectionCore.BeginLogOff(connectionContext, callback, callbackState));
		}

		private delegate TOutput ResultConverter<TOutput>(ImapResultData resultData, ImapConnectionContext context);
	}
}
