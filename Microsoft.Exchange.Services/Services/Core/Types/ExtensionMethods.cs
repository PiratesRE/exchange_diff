using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ExtensionMethods
	{
		public static StoreObjectId GetRefreshedDefaultFolderId(this MailboxSession mailboxSession, DefaultFolderType defaultFolderType)
		{
			return mailboxSession.GetRefreshedDefaultFolderId(defaultFolderType, false);
		}

		public static StoreObjectId GetRefreshedDefaultFolderId(this MailboxSession mailboxSession, DefaultFolderType defaultFolderType, bool unifiedSession)
		{
			CallContext callContext = CallContext.Current;
			if (callContext == null || callContext.SessionCache == null)
			{
				throw new InvalidOperationException("Method GetRefreshedDefaultFolderId requires a valid CallContext.SessionCache.");
			}
			SessionAndAuthZ sessionAndAuthZ = callContext.SessionCache.GetSessionAndAuthZ(mailboxSession.MailboxGuid, unifiedSession);
			return sessionAndAuthZ.GetRefreshedDefaultFolderId(defaultFolderType);
		}

		public static DelegateSessionHandle GetDelegateSessionHandleForEWS(this MailboxSession mailboxSession, IExchangePrincipal exchangePrincipal)
		{
			DelegateSessionHandle delegateSessionHandle;
			try
			{
				delegateSessionHandle = mailboxSession.GetDelegateSessionHandle(exchangePrincipal);
			}
			catch (NotSupportedWithServerVersionException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<NotSupportedWithServerVersionException>(0L, "mailboxSession.GetDelegateSessionHandle failed. Exception '{0}'.", ex);
				throw new WrongServerVersionDelegateException(ex);
			}
			return delegateSessionHandle;
		}

		public static CalendarItemBase GetCorrelatedItemForEWS(this MeetingMessage meetingMessage)
		{
			try
			{
				return meetingMessage.GetCorrelatedItem();
			}
			catch (CorrelationFailedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<CorrelationFailedException>(0L, "CalendarItem associated with MeetingMessage could not be found. Exception '{0}'.", ex);
				if (ex.InnerException is NotSupportedWithServerVersionException)
				{
					throw new WrongServerVersionDelegateException(ex);
				}
			}
			catch (CorruptDataException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<CorruptDataException>(0L, "CalendarItem associated with MeetingMessage could not be found. Exception '{0}'.", arg);
			}
			catch (VirusException arg2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<VirusException>(0L, "CalendarItem associated with MeetingMessage could not be found. Exception '{0}'.", arg2);
			}
			catch (RecurrenceException arg3)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<RecurrenceException>(0L, "CalendarItem associated with MeetingMessage could not be found. Exception '{0}'.", arg3);
			}
			return null;
		}
	}
}
