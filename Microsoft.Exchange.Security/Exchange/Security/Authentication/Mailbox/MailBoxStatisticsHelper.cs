using System;
using System.Diagnostics;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;

namespace Microsoft.Exchange.Security.Authentication.Mailbox
{
	internal static class MailBoxStatisticsHelper
	{
		public static DateTime? GetLastLogonTimestampFromCache(string upn)
		{
			if (MailBoxStatisticsHelper.cachedDateTime == null)
			{
				return null;
			}
			DateTime value;
			if (MailBoxStatisticsHelper.cachedDateTime.TryGetValue(upn, out value))
			{
				return new DateTime?(value);
			}
			return null;
		}

		public static DateTime GetLastLogonTimestamp(string upn, Guid mailboxGuid, ADObjectId databaseId, string dbName, out int mailboxFound)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Entering GetLastLogonTimestamp()");
			mailboxFound = 0;
			MailBoxStatisticsHelper.counters.NumberOfMailboxAccess.Increment();
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			try
			{
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(databaseId.ObjectGuid);
				using (MapiAdministrationSession mapiAdministrationSession = new MapiAdministrationSession(serverForDatabase.ServerLegacyDN, Fqdn.Parse(serverForDatabase.ServerFqdn)))
				{
					QueryFilter filter = new MailboxContextFilter(mailboxGuid, 0UL, false);
					DatabaseId rootId = new DatabaseId(null, null, dbName, databaseId.ObjectGuid);
					try
					{
						MailboxStatistics[] array = (MailboxStatistics[])((IConfigDataProvider)mapiAdministrationSession).Find<MailboxStatistics>(filter, rootId, true, null);
						mailboxFound = array.Length;
						if (array.Length == 1)
						{
							if (array[0].LastLogonTime != null)
							{
								MailBoxStatisticsHelper.CreateCacheIfNecessary();
								MailBoxStatisticsHelper.cachedDateTime.Add(upn, array[0].LastLogonTime.Value);
								ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Leaving GetLastLogonTimestamp 1");
								return array[0].LastLogonTime.Value;
							}
							ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Leaving GetLastLogonTimestamp 2");
							return DateTime.UtcNow;
						}
					}
					catch (MapiObjectNotFoundException ex)
					{
						ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>(0L, "GetLastLogonTimestamp for database {0} failed. Error: {1}", dbName, ex.ToString());
					}
				}
			}
			catch (DatabaseNotFoundException ex2)
			{
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>(0L, "GetLastLogonTimestamp for database {0} failed. Error: {1}", dbName, ex2.ToString());
			}
			catch (DatabaseUnavailableException ex3)
			{
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>(0L, "GetLastLogonTimestamp for database {0} failed. Error: {1}", dbName, ex3.ToString());
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Leaving GetLastLogonTimestamp 3");
			return DateTime.MinValue;
		}

		private static void CreateCacheIfNecessary()
		{
			if (MailBoxStatisticsHelper.cachedDateTime == null)
			{
				lock (MailBoxStatisticsHelper.lockObj)
				{
					if (MailBoxStatisticsHelper.cachedDateTime == null)
					{
						MailBoxStatisticsHelper.cachedDateTime = new MruDictionary<string, DateTime>(AuthServiceStaticConfig.Config.MaxCacheSizeOfLastLogonTime, StringComparer.OrdinalIgnoreCase, null);
					}
				}
			}
		}

		private static MruDictionary<string, DateTime> cachedDateTime = null;

		private static object lockObj = new object();

		private static readonly LiveIdBasicAuthenticationCountersInstance counters = LiveIdBasicAuthenticationCounters.GetInstance(Process.GetCurrentProcess().ProcessName);
	}
}
