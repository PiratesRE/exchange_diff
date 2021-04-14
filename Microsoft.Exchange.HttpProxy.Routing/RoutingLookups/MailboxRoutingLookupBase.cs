using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal abstract class MailboxRoutingLookupBase<T> : IRoutingLookup where T : class, IRoutingKey
	{
		protected MailboxRoutingLookupBase(IUserProvider userProvider)
		{
			if (userProvider == null)
			{
				throw new ArgumentNullException("userProvider");
			}
			this.userProvider = userProvider;
		}

		protected IUserProvider UserProvider
		{
			get
			{
				return this.userProvider;
			}
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
			T t = routingKey as T;
			if (t == null)
			{
				string message = string.Format("Routing key type {0} is not supported", routingKey.GetType());
				throw new ArgumentException(message, "routingKey");
			}
			return this.GetMailboxRoutingEntry(t, diagnostics);
		}

		public MailboxRoutingEntry GetMailboxRoutingEntry(T routingKey, IRoutingDiagnostics diagnostics)
		{
			if (routingKey == null)
			{
				throw new ArgumentNullException("routingKey");
			}
			MailboxRoutingEntry result;
			try
			{
				User user = this.FindUser(routingKey, diagnostics);
				if (user == null)
				{
					result = this.CreateFailedEntry(routingKey, "Unable to find user");
				}
				else
				{
					Guid? guid = null;
					string resourceForest = null;
					this.SelectDatabaseGuidResourceForest(routingKey, user, out guid, out resourceForest);
					if (guid == null)
					{
						result = this.CreateFailedEntry(routingKey, "User object missing database GUID");
					}
					else
					{
						long timestamp = (user.LastModifiedTime != null) ? user.LastModifiedTime.Value.ToFileTimeUtc() : DateTime.UtcNow.ToFileTimeUtc();
						string domainName = this.GetDomainName(routingKey);
						result = new SuccessfulMailboxRoutingEntry(routingKey, new DatabaseGuidRoutingDestination(guid.Value, domainName, resourceForest), timestamp);
					}
				}
			}
			catch (UserProviderException ex)
			{
				ErrorRoutingDestination destination = new ErrorRoutingDestination(ex.Message);
				result = new FailedMailboxRoutingEntry(routingKey, destination, DateTime.UtcNow.ToFileTimeUtc());
			}
			return result;
		}

		protected abstract User FindUser(T routingKey, IRoutingDiagnostics diagnostics);

		protected abstract string GetDomainName(T routingKey);

		protected virtual void SelectDatabaseGuidResourceForest(T routingKey, User user, out Guid? databaseGuid, out string resourceForest)
		{
			databaseGuid = user.DatabaseGuid;
			resourceForest = user.DatabaseResourceForest;
		}

		private FailedMailboxRoutingEntry CreateFailedEntry(IRoutingKey smtpRoutingKey, string message)
		{
			ErrorRoutingDestination destination = new ErrorRoutingDestination(message);
			return new FailedMailboxRoutingEntry(smtpRoutingKey, destination, DateTime.UtcNow.ToFileTimeUtc());
		}

		private readonly IUserProvider userProvider;
	}
}
