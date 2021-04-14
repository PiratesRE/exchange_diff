using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TeamMailboxExecutionHelper
	{
		public static GroupOperationResult RunGroupOperationsWithExecutionLimitHandler(Func<GroupOperationResult> method, string actionName)
		{
			GroupOperationResult result;
			try
			{
				if (Interlocked.Increment(ref TeamMailboxExecutionHelper.concurrentOperationCount) > Configuration.ServiceConfiguration.TMPublishConcurrentOperationLimit)
				{
					throw new RopExecutionException(string.Format("{0} failed because concurrent SharePoint operations limit inside MoMT has been exceeded at {1}", actionName, Configuration.ServiceConfiguration.TMPublishConcurrentOperationLimit), (ErrorCode)2147500037U);
				}
				result = method();
			}
			finally
			{
				Interlocked.Decrement(ref TeamMailboxExecutionHelper.concurrentOperationCount);
			}
			return result;
		}

		public static void RunOperationWithExceptionAndExecutionLimitHandler(Action action, string actionName, Action<SharePointException> exceptionHandler)
		{
			try
			{
				if (Interlocked.Increment(ref TeamMailboxExecutionHelper.concurrentOperationCount) > Configuration.ServiceConfiguration.TMPublishConcurrentOperationLimit)
				{
					throw new RopExecutionException(string.Format("{0} failed because concurrent SharePoint operations limit inside MoMT has been exceeded at {1}", actionName, Configuration.ServiceConfiguration.TMPublishConcurrentOperationLimit), (ErrorCode)2147500037U);
				}
				action();
			}
			catch (SharePointException ex)
			{
				if (exceptionHandler == null)
				{
					throw new RopExecutionException(string.Format("{0} failed with SharePoint Exception {1}", actionName, ex.Message), (ErrorCode)2147500037U, ex);
				}
				exceptionHandler(ex);
			}
			finally
			{
				Interlocked.Decrement(ref TeamMailboxExecutionHelper.concurrentOperationCount);
			}
		}

		public static TeamMailboxClientOperations GetTeamMailboxClientOperations(IConnection connection)
		{
			if (connection == null)
			{
				throw new RopExecutionException("GetTeamMailboxClientOperations failed because connection is null", (ErrorCode)2147500037U);
			}
			bool isOAuthCredentials = false;
			ICredentials credential = TeamMailboxExecutionHelper.GetCredential(connection, out isOAuthCredentials);
			return TeamMailboxClientOperations.CreateInstance(credential, isOAuthCredentials, connection.MiniRecipient, Configuration.ServiceConfiguration.TMPublishRequestTimeout, Configuration.ServiceConfiguration.TMPublishHttpDebugEnabled, Configuration.ServiceConfiguration.TMUseMockSharePointOperation);
		}

		public static bool SaveChangesToLinkedDocumentLibraryIfNecessary(CoreItem item, Logon logonObject)
		{
			if (!Utils.IsTeamMailbox(item.Session))
			{
				return false;
			}
			if (TeamMailboxClientOperations.IsLinkedItem(item))
			{
				return false;
			}
			byte[] array = item.PropertyBag.TryGetProperty(CoreObjectSchema.ParentEntryId) as byte[];
			if (array != null)
			{
				StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Message);
				using (Folder folder = Folder.Bind(item.Session, folderId))
				{
					if (TeamMailboxClientOperations.IsLinkedFolder(folder))
					{
						if (!Configuration.ServiceConfiguration.TMPublishEnabled)
						{
							throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
						}
						TeamMailboxClientOperations teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(logonObject.Connection);
						string responseUrl = null;
						TeamMailboxExecutionHelper.RunOperationWithExceptionAndExecutionLimitHandler(delegate
						{
							teamMailboxClientOperations.OnSaveMessage(item, folder, out responseUrl);
						}, "TeamMailboxClientOperations.OnSaveMessage", delegate(SharePointException e)
						{
							teamMailboxClientOperations.HandleSharePointPublishingError(e, item);
						});
						return true;
					}
				}
				return false;
			}
			throw new RopExecutionException("Message doesn't have parent folder ID set", (ErrorCode)2147746075U);
		}

		public static void LogServerFailures(CoreItem item, Logon logonObject, Exception e)
		{
			string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.NormalizedSubject, string.Empty);
			LoggingContext loggingContext = new LoggingContext(item.Session.MailboxGuid, valueOrDefault, logonObject.Connection.ActAsLegacyDN, null);
			ProtocolLog.LogError(ProtocolLog.Component.MoMT, loggingContext, string.Format("SaveChangesToLinkedDocumentLibraryIfNecessary failed for user {0} against site mailbox guid {1}", logonObject.Connection.ActAsLegacyDN, item.Session.MailboxGuid), e);
		}

		private static ICredentials GetCredential(IConnection connection, out bool isOauthCredential)
		{
			ICredentials result = Configuration.ServiceConfiguration.TMPublishCredential;
			isOauthCredential = false;
			if (connection.MiniRecipient == null)
			{
				throw new RopExecutionException(string.Format("TeamMailboxClientOperations is unable to obtain outbound credential for {0} because the actAs user is null", connection.ActAsLegacyDN), (ErrorCode)2147500037U);
			}
			if (connection.MiniRecipient.PrimarySmtpAddress == SmtpAddress.Empty)
			{
				throw new RopExecutionException(string.Format("TeamMailboxClientOperations is unable to obtain outbound OAuth token for {0} because PrimarySmtpAddress is empty for the user object", connection.ActAsLegacyDN), (ErrorCode)2147500037U);
			}
			if (Configuration.ServiceConfiguration.TMOAuthEnabled)
			{
				result = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(connection.MiniRecipient.OrganizationId, connection.MiniRecipient, null);
				isOauthCredential = true;
			}
			return result;
		}

		private static int concurrentOperationCount;
	}
}
