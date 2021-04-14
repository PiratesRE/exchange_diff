using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.ThrottlingService.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SubmissionQuotaChecker
	{
		public static bool CheckSubmissionQuota(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			int count = sessionState.TransportMailItem.Recipients.Count;
			if (count == 0)
			{
				sessionState.Tracer.TraceDebug<SecurityIdentifier>(0L, "Bypassed submission quota check for remote identity {0} because the mail item has no recipients", sessionState.RemoteIdentity);
				return true;
			}
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(sessionState.CombinedPermissions))
			{
				sessionState.Tracer.TraceDebug<SecurityIdentifier>(0L, "Bypassed submission quota check for remote identity {0} because the SMTPBypassMessageSizeLimitPermission is granted", sessionState.RemoteIdentity);
				return true;
			}
			if (sessionState.AuthenticatedUser == null)
			{
				sessionState.Tracer.TraceDebug<SecurityIdentifier>(0L, "Bypassed submission quota check for remote identity {0} because the identity does not have a valid AD entry to get a quota from", sessionState.RemoteIdentity);
				return true;
			}
			Guid exchangeGuid = sessionState.AuthenticatedUser.ExchangeGuid;
			ADObjectId database = sessionState.AuthenticatedUser.Database;
			OrganizationId organizationId = sessionState.AuthenticatedUser.OrganizationId;
			if (exchangeGuid == Guid.Empty || database == null || database.ObjectGuid == Guid.Empty || organizationId == null)
			{
				sessionState.Tracer.TraceDebug(0L, "Bypassed submission quota check for remote identity {0} because the identity does not resolve to a valid mailbox; mailboxGuid={1}; homeMdb=<{2}>; organizationId=<{3}>", new object[]
				{
					sessionState.RemoteIdentity,
					exchangeGuid,
					database ?? "null",
					organizationId ?? "null"
				});
				return true;
			}
			ADObjectId throttlingPolicy = sessionState.AuthenticatedUser.ThrottlingPolicy;
			Unlimited<uint> userSubmissionQuota = MailboxThrottle.GetUserSubmissionQuota(throttlingPolicy, organizationId);
			if (userSubmissionQuota.IsUnlimited)
			{
				sessionState.Tracer.TraceDebug<SecurityIdentifier, Guid, OrganizationId>(0L, "Bypassed submission quota check for remote identity {0} because mailbox GUID {1} from organization <{2}> has unlimited submission quota", sessionState.RemoteIdentity, exchangeGuid, organizationId);
				return true;
			}
			string mailboxServer;
			int mailboxServerVersion;
			if (!SubmissionQuotaChecker.TryGetServerForMdb(sessionState, database.ObjectGuid, out mailboxServer, out mailboxServerVersion))
			{
				sessionState.Tracer.TraceError<SecurityIdentifier, Guid, Guid>(0L, "Bypassed submission quota check for remote identity {0} and mailbox GUID {1} because the mailbox server was not determined for MDB {2}", sessionState.RemoteIdentity, exchangeGuid, database.ObjectGuid);
				return true;
			}
			bool flag = MailboxThrottle.Instance.ObtainUserSubmissionTokens(mailboxServer, mailboxServerVersion, exchangeGuid, count, userSubmissionQuota, "SmtpClient");
			sessionState.Tracer.TraceDebug(0L, "Submission to {0} recipient(s) is {1} for remote identity {2}, mailbox GUID {3} and submission quota {4}", new object[]
			{
				count,
				flag ? "allowed" : "denied",
				sessionState.RemoteIdentity,
				exchangeGuid,
				userSubmissionQuota
			});
			return flag;
		}

		private static bool TryGetServerForMdb(SmtpInSessionState sessionState, Guid mdbGuid, out string serverFqdn, out int serverVersion)
		{
			serverFqdn = null;
			serverVersion = 0;
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			Exception ex = null;
			try
			{
				sessionState.Tracer.TraceDebug<Guid>(0L, "Invoking ActiveManager for MDB GUID {0}", mdbGuid);
				DatabaseLocationInfo serverForDatabase = cachingActiveManagerInstance.GetServerForDatabase(mdbGuid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary | GetServerForDatabaseFlags.BasicQuery);
				serverFqdn = serverForDatabase.ServerFqdn;
				serverVersion = serverForDatabase.ServerVersion;
			}
			catch (DatabaseNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			catch (StorageTransientException ex4)
			{
				ex = ex4;
			}
			if (string.IsNullOrEmpty(serverFqdn))
			{
				SubmissionQuotaChecker.HandleActiveManagerFailure(sessionState, ex, mdbGuid);
				sessionState.Tracer.TraceError<Guid>(0L, "Failed to get server for MDB GUID {0} from ActiveManager", mdbGuid);
				return false;
			}
			sessionState.Tracer.TraceDebug<string, Guid>(0L, "ActiveManager returned server {0} for MDB GUID {1}", serverFqdn, mdbGuid);
			return true;
		}

		private static void HandleActiveManagerFailure(SmtpInSessionState sessionState, Exception ex, Guid mdbGuid)
		{
			sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveActiveManagerFailure, mdbGuid.ToString(), new object[]
			{
				mdbGuid,
				sessionState.RemoteIdentity,
				sessionState.NetworkConnection.RemoteEndPoint.Address,
				ex ?? "<none>"
			});
			sessionState.Tracer.TraceDebug<Guid, object>(0L, "Failure occurred while querying ActiveManager for MDB {0}. Exception details: {1}", mdbGuid, ex ?? "<none>");
		}
	}
}
