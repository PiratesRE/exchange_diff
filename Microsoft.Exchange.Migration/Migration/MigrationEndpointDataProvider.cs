using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationEndpointDataProvider : XsoMailboxDataProviderBase
	{
		internal MigrationEndpointDataProvider(IMigrationDataProvider dataProvider)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			this.diagnosticEnabled = false;
			this.dataProvider = dataProvider;
		}

		private MigrationEndpointDataProvider(MigrationDataProvider dataProvider) : base(dataProvider.MailboxSession)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			this.dataProvider = dataProvider;
		}

		public static MigrationEndpointDataProvider CreateDataProvider(string action, IRecipientSession recipientSession, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationEndpointDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationDataProvider disposable = MigrationDataProvider.CreateProviderForMigrationMailbox(action, recipientSession, partitionMailbox);
				disposeGuard.Add<MigrationDataProvider>(disposable);
				MigrationEndpointDataProvider migrationEndpointDataProvider = new MigrationEndpointDataProvider(disposable);
				disposeGuard.Success();
				result = migrationEndpointDataProvider;
			}
			return result;
		}

		public static QueryFilter GetFilterFromEndpointType(MigrationType type)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.MigrationEndpointType, type);
		}

		public static QueryFilter GetFilterFromConnectionSettings(ExchangeConnectionSettings connectionSettings)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			MigrationType type = connectionSettings.Type;
			if (type != MigrationType.ExchangeOutlookAnywhere)
			{
				if (type != MigrationType.ExchangeRemoteMove)
				{
					if (type != MigrationType.PublicFolder)
					{
						throw new NotSupportedException(string.Format("The connection settings of type '{0}' is not for a well-known Exchange endpoint.", connectionSettings.Type));
					}
					list.Add(MigrationEndpointDataProvider.GetFilterFromEndpointType(MigrationType.PublicFolder));
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.RemoteHostName, connectionSettings.IncomingRPCProxyServer));
				}
				else
				{
					list.Add(MigrationEndpointDataProvider.GetFilterFromEndpointType(MigrationType.ExchangeRemoteMove));
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.RemoteHostName, connectionSettings.IncomingRPCProxyServer));
				}
			}
			else
			{
				list.Add(MigrationEndpointDataProvider.GetFilterFromEndpointType(MigrationType.ExchangeOutlookAnywhere));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.ExchangeServer, connectionSettings.IncomingExchangeServer));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.RemoteHostName, connectionSettings.IncomingRPCProxyServer));
			}
			return new AndFilter(list.ToArray());
		}

		public void EnableDiagnostics(string argument)
		{
			this.diagnosticEnabled = true;
			this.diagnosticArgument = new MigrationDiagnosticArgument(argument);
		}

		internal void CreateEndpoint(MigrationEndpoint connector)
		{
			MigrationEndpointBase migrationEndpointBase = MigrationEndpointBase.CreateFrom(connector);
			MigrationEndpointBase.Create(this.dataProvider, migrationEndpointBase);
			connector.Identity = migrationEndpointBase.Identity;
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			MigrationEndpointId endpointRoot = rootId as MigrationEndpointId;
			IEnumerable<MigrationEndpointBase> endpoints;
			if (endpointRoot != null)
			{
				endpoints = MigrationEndpointBase.Get(endpointRoot, this.dataProvider, true);
			}
			else
			{
				endpoints = MigrationEndpointBase.Get(filter, this.dataProvider, true);
			}
			foreach (MigrationEndpointBase item in endpoints)
			{
				if (typeof(MigrationEndpoint).IsAssignableFrom(typeof(T)))
				{
					MigrationEndpoint endpoint = item.ToMigrationEndpoint();
					if (this.diagnosticEnabled)
					{
						XElement diagnosticInfo = item.GetDiagnosticInfo(this.dataProvider, this.diagnosticArgument);
						if (diagnosticInfo != null)
						{
							endpoint.DiagnosticInfo = diagnosticInfo.ToString();
						}
					}
					yield return (T)((object)endpoint);
				}
				else
				{
					yield return (T)((object)item);
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			MigrationEndpoint migrationEndpoint = instance as MigrationEndpoint;
			if (migrationEndpoint == null)
			{
				throw new ArgumentException("Instance is not from an expected class.", "instance");
			}
			migrationEndpoint.LastModifiedTime = (DateTime)ExDateTime.UtcNow;
			switch (instance.ObjectState)
			{
			case ObjectState.New:
				this.CreateEndpoint(migrationEndpoint);
				return;
			case ObjectState.Unchanged:
			case ObjectState.Changed:
				MigrationEndpointBase.UpdateEndpoint(migrationEndpoint, this.dataProvider);
				return;
			case ObjectState.Deleted:
				return;
			default:
				throw new NotImplementedException("Support for action " + instance.ObjectState + " is not yet implemented");
			}
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			MigrationEndpoint migrationEndpoint = instance as MigrationEndpoint;
			if (migrationEndpoint == null)
			{
				throw new ArgumentException("Instance is not from an expected class.", "instance");
			}
			switch (instance.ObjectState)
			{
			case ObjectState.Unchanged:
			case ObjectState.Deleted:
				MigrationEndpointBase.Delete(migrationEndpoint.Identity, this.dataProvider);
				return;
			}
			throw new NotImplementedException("Support for action " + instance.ObjectState + " is not yet implemented");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationEndpointDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.dataProvider != null)
					{
						this.dataProvider.Dispose();
					}
					this.dataProvider = null;
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		private IMigrationDataProvider dataProvider;

		private bool diagnosticEnabled;

		private MigrationDiagnosticArgument diagnosticArgument;
	}
}
