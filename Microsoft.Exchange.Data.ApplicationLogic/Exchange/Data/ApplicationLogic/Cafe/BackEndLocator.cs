using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class BackEndLocator
	{
		public static BackEndServer GetAnyBackEndServer()
		{
			return BackEndLocator.CallWithExceptionHandling<BackEndServer>(() => HttpProxyBackEndHelper.GetAnyBackEndServer());
		}

		public static BackEndServer GetBackEndServer(ADUser aduser)
		{
			if (aduser == null)
			{
				throw new ArgumentNullException("aduser");
			}
			return BackEndLocator.CallWithExceptionHandling<BackEndServer>(() => BackEndLocator.GetBackEndServerByDatabase(aduser.Database, aduser.OrganizationId, aduser.PrimarySmtpAddress));
		}

		public static BackEndServer GetBackEndServer(ADObjectId database)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			return BackEndLocator.CallWithExceptionHandling<BackEndServer>(() => BackEndLocator.GetBackEndServerByDatabase(database, null, default(SmtpAddress)));
		}

		public static BackEndServer GetBackEndServer(MiniRecipient miniRecipient)
		{
			if (miniRecipient == null)
			{
				throw new ArgumentNullException("miniRecipient");
			}
			return BackEndLocator.CallWithExceptionHandling<BackEndServer>(() => BackEndLocator.GetBackEndServerByDatabase(miniRecipient.Database, miniRecipient.OrganizationId, miniRecipient.PrimarySmtpAddress));
		}

		public static IList<BackEndServer> GetBackEndServerList(MiniRecipient miniRecipient, int maxServers)
		{
			if (miniRecipient == null)
			{
				throw new ArgumentNullException("miniRecipient");
			}
			return BackEndLocator.CallWithExceptionHandling<IList<BackEndServer>>(() => BackEndLocator.GetBackEndServerListForDatabase(miniRecipient.Database, miniRecipient.OrganizationId, miniRecipient.PrimarySmtpAddress, maxServers));
		}

		public static BackEndServer GetBackEndServer(IMailboxInfo mailbox)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			return BackEndLocator.CallWithExceptionHandling<BackEndServer>(delegate
			{
				if (mailbox.Location != null)
				{
					BackEndServer backEndServer = new BackEndServer(mailbox.Location.ServerFqdn, mailbox.Location.ServerVersion);
					ExTraceGlobals.CafeTracer.TraceDebug<BackEndServer, IMailboxInfo>(0L, "[BackEndLocator.GetBackEndServer] Returns back end server {0} for Mailbox {1}", backEndServer, mailbox);
					return backEndServer;
				}
				return BackEndLocator.GetBackEndServerByDatabase(mailbox.MailboxDatabase, mailbox.OrganizationId, mailbox.PrimarySmtpAddress);
			});
		}

		public static Uri GetBackEndWebServicesUrl(ADUser aduser)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<WebServicesService>(aduser);
		}

		public static Uri GetBackEndWebServicesUrl(MiniRecipient miniRecipient)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<WebServicesService>(miniRecipient);
		}

		public static Uri GetBackEndWebServicesUrl(IMailboxInfo mailbox)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<WebServicesService>(mailbox);
		}

		public static Uri GetBackEndWebServicesUrl(BackEndServer backEndServer)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<WebServicesService>(backEndServer);
		}

		public static Uri GetBackEndOwaUrl(IMailboxInfo mailbox)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<OwaService>(mailbox);
		}

		public static Uri GetBackEndEcpUrl(IMailboxInfo mailbox)
		{
			return BackEndLocator.GetBackEndHttpServiceUrl<EcpService>(mailbox);
		}

		public static bool ShouldWrapInBackendLocatorException(Exception exception)
		{
			return exception is ObjectNotFoundException || exception is ServerLocatorClientException || exception is ServerLocatorClientTransientException || exception is ServiceDiscoveryPermanentException || exception is ServiceDiscoveryTransientException || (exception is InsufficientMemoryException && exception.InnerException is SocketException);
		}

		private static Uri GetBackEndHttpServiceUrl<ServiceType>(ADUser aduser) where ServiceType : HttpService
		{
			if (aduser == null)
			{
				throw new ArgumentNullException("aduser");
			}
			return BackEndLocator.CallWithExceptionHandling<Uri>(delegate
			{
				BackEndServer backEndServer = BackEndLocator.GetBackEndServer(aduser);
				return HttpProxyBackEndHelper.GetBackEndServiceUrlByServer<ServiceType>(backEndServer);
			});
		}

		private static Uri GetBackEndHttpServiceUrl<ServiceType>(MiniRecipient miniRecipient) where ServiceType : HttpService
		{
			if (miniRecipient == null)
			{
				throw new ArgumentNullException("miniRecipient");
			}
			return BackEndLocator.CallWithExceptionHandling<Uri>(delegate
			{
				BackEndServer backEndServer = BackEndLocator.GetBackEndServer(miniRecipient);
				return HttpProxyBackEndHelper.GetBackEndServiceUrlByServer<ServiceType>(backEndServer);
			});
		}

		private static Uri GetBackEndHttpServiceUrl<ServiceType>(IMailboxInfo mailbox) where ServiceType : HttpService
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			return BackEndLocator.CallWithExceptionHandling<Uri>(delegate
			{
				BackEndServer backEndServer = BackEndLocator.GetBackEndServer(mailbox);
				return HttpProxyBackEndHelper.GetBackEndServiceUrlByServer<ServiceType>(backEndServer);
			});
		}

		private static Uri GetBackEndHttpServiceUrl<ServiceType>(BackEndServer backEndServer) where ServiceType : HttpService
		{
			if (backEndServer == null)
			{
				throw new ArgumentNullException("backEndServer");
			}
			return BackEndLocator.CallWithExceptionHandling<Uri>(() => HttpProxyBackEndHelper.GetBackEndServiceUrlByServer<ServiceType>(backEndServer));
		}

		private static BackEndServer GetBackEndServerByDatabase(ADObjectId database, OrganizationId organizationId, SmtpAddress primarySmtpAddress)
		{
			if (database == null)
			{
				return BackEndLocator.GetBackEndServerByOrganization(organizationId);
			}
			string domainName = null;
			if (organizationId != null && !organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				domainName = primarySmtpAddress.Domain;
			}
			BackEndServer result;
			using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.Create(database.ObjectGuid, domainName, database.PartitionFQDN))
			{
				BackEndServer server = mailboxServerLocator.GetServer();
				ExTraceGlobals.CafeTracer.TraceDebug<BackEndServer, ADObjectId>(0L, "[BackEndLocator.GetBackEndServerByDatabase] Returns back end server {0} for database {1}", server, database);
				result = server;
			}
			return result;
		}

		private static IList<BackEndServer> GetBackEndServerListForDatabase(ADObjectId database, OrganizationId organizationId, SmtpAddress primarySmtpAddress, int maxServers)
		{
			if (maxServers == 0)
			{
				return new BackEndServer[0];
			}
			if (database == null)
			{
				return BackEndLocator.GetBackEndServerListForOrganization(organizationId, maxServers);
			}
			string domainName = null;
			if (organizationId != null && !organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				domainName = primarySmtpAddress.Domain;
			}
			IList<BackEndServer> result;
			using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.Create(database.ObjectGuid, domainName, database.PartitionFQDN))
			{
				BackEndServer server = mailboxServerLocator.GetServer();
				ExTraceGlobals.CafeTracer.TraceDebug<BackEndServer, ADObjectId>(0L, "[BackEndLocator.GetBackEndServerByDatabase] Returns back end server {0} for database {1}", server, database);
				IList<BackEndServer> list = new List<BackEndServer>();
				list.Add(server);
				int num = 1;
				foreach (KeyValuePair<Guid, BackEndServer> keyValuePair in mailboxServerLocator.AvailabilityGroupServers)
				{
					if (num >= maxServers)
					{
						break;
					}
					if (!string.Equals(keyValuePair.Value.Fqdn, server.Fqdn, StringComparison.OrdinalIgnoreCase))
					{
						list.Add(keyValuePair.Value);
						num++;
					}
				}
				result = list;
			}
			return result;
		}

		private static IList<BackEndServer> GetBackEndServerListForOrganization(OrganizationId organizationId, int maxServers)
		{
			ADUser defaultOrganizationMailbox = HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(organizationId, null);
			if (defaultOrganizationMailbox == null || defaultOrganizationMailbox.Database == null)
			{
				ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[BackEndLocator.GetBackEndServerByOrganization] Cannot find organization mailbox for organization {1}", organizationId);
				throw new AdUserNotFoundException(ServerStrings.ADUserNotFound);
			}
			return BackEndLocator.GetBackEndServerListForDatabase(defaultOrganizationMailbox.Database, organizationId, defaultOrganizationMailbox.PrimarySmtpAddress, maxServers);
		}

		private static BackEndServer GetBackEndServerByOrganization(OrganizationId organizationId)
		{
			ADUser defaultOrganizationMailbox = HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(organizationId, null);
			if (defaultOrganizationMailbox == null || defaultOrganizationMailbox.Database == null)
			{
				ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[BackEndLocator.GetBackEndServerByOrganization] Cannot find organization mailbox for organization {1}", organizationId);
				throw new AdUserNotFoundException(ServerStrings.ADUserNotFound);
			}
			return BackEndLocator.GetBackEndServerByDatabase(defaultOrganizationMailbox.Database, organizationId, defaultOrganizationMailbox.PrimarySmtpAddress);
		}

		private static T CallWithExceptionHandling<T>(Func<T> actualCall)
		{
			T result;
			try
			{
				result = actualCall();
			}
			catch (BackEndLocatorException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CafeTracer.TraceError<Exception>(0L, "[BackEndLocator.CallWithExceptionHandling] Caught exception {0}.", ex);
				if (BackEndLocator.ShouldWrapInBackendLocatorException(ex))
				{
					throw new BackEndLocatorException(ex);
				}
				throw;
			}
			return result;
		}
	}
}
