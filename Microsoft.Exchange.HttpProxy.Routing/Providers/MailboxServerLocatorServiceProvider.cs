using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ServerLocator;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	internal class MailboxServerLocatorServiceProvider : IDatabaseLocationProvider
	{
		public BackEndServer GetBackEndServerForDatabase(Guid databaseGuid, string domainName, string resourceForest, IRoutingDiagnostics diagnostics)
		{
			Exception ex = null;
			BackEndServer result;
			using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.Create(databaseGuid, domainName, resourceForest))
			{
				BackEndServer backEndServer = null;
				try
				{
					backEndServer = mailboxServerLocator.GetServer();
				}
				catch (ServerLocatorClientException ex2)
				{
					ex = ex2;
				}
				catch (ServerLocatorClientTransientException ex3)
				{
					ex = ex3;
				}
				catch (MailboxServerLocatorException ex4)
				{
					ex = ex4;
				}
				catch (AmServerTransientException ex5)
				{
					ex = ex5;
				}
				catch (AmServerException ex6)
				{
					ex = ex6;
				}
				catch (DatabaseNotFoundException ex7)
				{
					ex = ex7;
				}
				catch (ADTransientException ex8)
				{
					ex = ex8;
				}
				catch (DataValidationException ex9)
				{
					ex = ex9;
				}
				catch (DataSourceOperationException ex10)
				{
					ex = ex10;
				}
				foreach (long num in mailboxServerLocator.DirectoryLatencies)
				{
					diagnostics.AddResourceForestLatency(TimeSpan.FromMilliseconds((double)num));
				}
				foreach (long num2 in mailboxServerLocator.GlsLatencies)
				{
					diagnostics.AddGlobalLocatorLatency(TimeSpan.FromMilliseconds((double)num2));
				}
				if (ex != null)
				{
					throw new DatabaseLocationProviderException("MailboxServerLocator.GetServer failed", ex);
				}
				result = backEndServer;
			}
			return result;
		}
	}
}
