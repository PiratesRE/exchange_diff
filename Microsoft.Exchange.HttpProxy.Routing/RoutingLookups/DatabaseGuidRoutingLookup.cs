using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class DatabaseGuidRoutingLookup : IRoutingLookup
	{
		public DatabaseGuidRoutingLookup(IDatabaseLocationProvider databaseLocationProvider)
		{
			if (databaseLocationProvider == null)
			{
				throw new ArgumentNullException("databaseLocationProvider");
			}
			this.databaseLocationProvider = databaseLocationProvider;
		}

		IRoutingEntry IRoutingLookup.GetRoutingEntry(IRoutingKey routingKey, IRoutingDiagnostics diagnostics)
		{
			if (routingKey == null)
			{
				throw new ArgumentNullException("routingKey");
			}
			if (diagnostics == null)
			{
				throw new ArgumentNullException("diagnostics");
			}
			DatabaseGuidRoutingKey databaseGuidRoutingKey = routingKey as DatabaseGuidRoutingKey;
			if (databaseGuidRoutingKey == null)
			{
				string message = string.Format("Routing key type {0} is not supported", routingKey.GetType());
				throw new ArgumentException(message, "routingKey");
			}
			return this.GetDatabaseGuidRoutingEntry(databaseGuidRoutingKey, diagnostics);
		}

		public DatabaseGuidRoutingEntry GetDatabaseGuidRoutingEntry(DatabaseGuidRoutingKey databaseGuidRoutingKey, IRoutingDiagnostics diagnostics)
		{
			if (databaseGuidRoutingKey == null)
			{
				throw new ArgumentNullException("databaseGuidRoutingKey");
			}
			DatabaseGuidRoutingEntry result;
			try
			{
				BackEndServer backEndServerForDatabase = this.databaseLocationProvider.GetBackEndServerForDatabase(databaseGuidRoutingKey.DatabaseGuid, databaseGuidRoutingKey.DomainName, databaseGuidRoutingKey.ResourceForest, diagnostics);
				if (backEndServerForDatabase == null)
				{
					result = this.CreateFailedEntry(databaseGuidRoutingKey, "Could not find database");
				}
				else
				{
					result = new SuccessfulDatabaseGuidRoutingEntry(databaseGuidRoutingKey, new ServerRoutingDestination(backEndServerForDatabase.Fqdn, new int?(backEndServerForDatabase.Version)), 0L);
				}
			}
			catch (DatabaseLocationProviderException ex)
			{
				result = this.CreateFailedEntry(databaseGuidRoutingKey, ex.Message);
			}
			return result;
		}

		private FailedDatabaseGuidRoutingEntry CreateFailedEntry(DatabaseGuidRoutingKey databaseGuidRoutingKey, string message)
		{
			ErrorRoutingDestination destination = new ErrorRoutingDestination(message);
			return new FailedDatabaseGuidRoutingEntry(databaseGuidRoutingKey, destination, DateTime.UtcNow.ToFileTimeUtc());
		}

		private readonly IDatabaseLocationProvider databaseLocationProvider;
	}
}
