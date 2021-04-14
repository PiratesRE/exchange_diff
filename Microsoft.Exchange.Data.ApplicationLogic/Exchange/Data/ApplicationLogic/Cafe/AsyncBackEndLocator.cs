using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncBackEndLocator
	{
		public IAsyncResult BeginGetBackEndServerList(MiniRecipient miniRecipient, int maxServers, AsyncCallback callback, object state)
		{
			if (miniRecipient == null)
			{
				throw new ArgumentNullException("miniRecipient");
			}
			if (maxServers <= 0)
			{
				throw new ArgumentException("maxServers needs to be greater than zero");
			}
			this.database = miniRecipient.Database;
			this.maxServers = maxServers;
			OrganizationId organizationId = miniRecipient.OrganizationId;
			SmtpAddress primarySmtpAddress = miniRecipient.PrimarySmtpAddress;
			if (this.database == null)
			{
				ADUser defaultOrganizationMailbox = HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(organizationId, null);
				if (defaultOrganizationMailbox == null || defaultOrganizationMailbox.Database == null)
				{
					ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[BackEndLocator.BeginGetBackEndServerList] Cannot find organization mailbox for organization {1}", organizationId);
					throw new AdUserNotFoundException(ServerStrings.ADUserNotFound);
				}
				this.database = defaultOrganizationMailbox.Database;
				primarySmtpAddress = defaultOrganizationMailbox.PrimarySmtpAddress;
			}
			string domainName = null;
			if (organizationId != null && !organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				domainName = primarySmtpAddress.Domain;
			}
			this.serverLocator = MailboxServerLocator.Create(this.database.ObjectGuid, domainName, this.database.PartitionFQDN);
			bool flag = true;
			IAsyncResult result;
			try
			{
				result = this.serverLocator.BeginGetServer(callback, state);
				flag = false;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CafeTracer.TraceError<Exception>(0L, "[AsyncBackEndLocator.BeginGetBackEndServerList] Caught exception {0}.", ex);
				if (BackEndLocator.ShouldWrapInBackendLocatorException(ex))
				{
					throw new BackEndLocatorException(ex);
				}
				throw;
			}
			finally
			{
				if (flag)
				{
					this.serverLocator.Dispose();
					this.serverLocator = null;
				}
			}
			return result;
		}

		public IList<BackEndServer> EndGetBackEndServerList(IAsyncResult asyncResult)
		{
			IList<BackEndServer> result;
			try
			{
				BackEndServer backEndServer = this.serverLocator.EndGetServer(asyncResult);
				ExTraceGlobals.CafeTracer.TraceDebug<BackEndServer, ADObjectId>(0L, "[MailboxServerLocator.EndGetServer] called inside [AsyncBackEndLocator.EndGetBackEndServerList] returned back end server {0} for database {1}", backEndServer, this.database);
				IList<BackEndServer> list = new List<BackEndServer>();
				list.Add(backEndServer);
				int num = 1;
				Random localRandom = new Random(AsyncBackEndLocator.GetRandomNumber(int.MaxValue));
				IEnumerable<KeyValuePair<Guid, BackEndServer>> enumerable = from x in this.serverLocator.AvailabilityGroupServers
				orderby localRandom.Next()
				select x;
				foreach (KeyValuePair<Guid, BackEndServer> keyValuePair in enumerable)
				{
					if (num >= this.maxServers)
					{
						break;
					}
					if (!this.IsServerInBackendList(list, keyValuePair.Value))
					{
						list.Add(keyValuePair.Value);
						num++;
					}
				}
				result = list;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CafeTracer.TraceError<Exception>(0L, "[AsyncBackEndLocator.EndGetBackEndServerList] Caught exception {0}.", ex);
				if (BackEndLocator.ShouldWrapInBackendLocatorException(ex))
				{
					throw new BackEndLocatorException(ex);
				}
				throw;
			}
			finally
			{
				this.serverLocator.Dispose();
				this.serverLocator = null;
			}
			return result;
		}

		private static int GetRandomNumber(int maxValue)
		{
			int result;
			lock (AsyncBackEndLocator.RandomSeeder)
			{
				result = AsyncBackEndLocator.RandomSeeder.Next(maxValue);
			}
			return result;
		}

		private bool IsServerInBackendList(IEnumerable<BackEndServer> serverList, BackEndServer backendServer)
		{
			return serverList.Any((BackEndServer server) => string.Equals(server.Fqdn, backendServer.Fqdn, StringComparison.OrdinalIgnoreCase));
		}

		private static readonly Random RandomSeeder = new Random((int)DateTime.UtcNow.Ticks);

		private MailboxServerLocator serverLocator;

		private ADObjectId database;

		private int maxServers;
	}
}
