using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class DatabaseRouteMap
	{
		public DatabaseRouteMap(RouteCalculatorContext context, ITenantDagQuota tenantDagQuota, bool forcedReload)
		{
			this.whenCreated = context.Timestamp;
			this.PopulateDeliveryGroups(context, tenantDagQuota);
			this.PopulateDatabaseDestinations(context, forcedReload);
			this.PopulatePublicFolderTreeDestinations(context);
		}

		public DeliveryGroup LocalDeliveryGroup
		{
			get
			{
				return this.localDeliveryGroup;
			}
			private set
			{
				if (this.localDeliveryGroup != null)
				{
					throw new InvalidOperationException("localDeliveryGroup already has a value");
				}
				this.localDeliveryGroup = value;
			}
		}

		public bool TryGetDatabaseDestination(ADObjectId databaseId, out MailboxDatabaseDestination databaseDestination)
		{
			RoutingUtils.ThrowIfNullOrEmptyObjectGuid(databaseId, "databaseId");
			return this.databaseDestinations.TryGetValue(databaseId.ObjectGuid, out databaseDestination);
		}

		public bool TryGetDagDeliveryGroup(Guid dagGuid, out DagDeliveryGroup deliveryGroup)
		{
			return this.dagDeliveryGroups.TryGetValue(dagGuid, out deliveryGroup);
		}

		public bool TryGetDagDeliveryGroupUsingDagSelector(Guid externalOrganizationId, out DeliveryGroup deliveryGroup)
		{
			DeliveryGroup deliveryGroup2;
			if (this.dagSelector.TryGetDagDeliveryGroup(externalOrganizationId, out deliveryGroup2))
			{
				deliveryGroup = deliveryGroup2;
				return true;
			}
			deliveryGroup = null;
			return false;
		}

		public void UpdateDagSelectorWithMessagesToDagDeliveryGroup(Guid externalOrganizationId, DeliveryGroup deliveryGroup)
		{
			DagDeliveryGroup dagDeliveryGroup = deliveryGroup as DagDeliveryGroup;
			if (dagDeliveryGroup != null)
			{
				this.dagSelector.IncrementMessagesDeliveredBasedOnMailbox(dagDeliveryGroup.NextHopGuid, externalOrganizationId);
			}
		}

		public bool TryGetMailboxDeliveryGroup(MailboxDeliveryGroupId id, out MailboxDeliveryGroup deliveryGroup)
		{
			return this.mailboxDeliveryGroups.TryGetValue(id, out deliveryGroup);
		}

		public bool QuickMatch(DatabaseRouteMap other)
		{
			return this.databaseDestinations.Count == other.databaseDestinations.Count && this.dagDeliveryGroups.Count == other.dagDeliveryGroups.Count && this.mailboxDeliveryGroups.Count == other.mailboxDeliveryGroups.Count;
		}

		public bool FullMatch(DatabaseRouteMap other)
		{
			if (!RoutingUtils.MatchDictionaries<Guid, MailboxDatabaseDestination>(this.databaseDestinations, other.databaseDestinations, (MailboxDatabaseDestination dest1, MailboxDatabaseDestination dest2) => dest1.Match(dest2)))
			{
				return false;
			}
			if (RoutingUtils.MatchDictionaries<Guid, DagDeliveryGroup>(this.dagDeliveryGroups, other.dagDeliveryGroups, (DagDeliveryGroup group1, DagDeliveryGroup group2) => group1.Match(group2)))
			{
				if (RoutingUtils.MatchDictionaries<MailboxDeliveryGroupId, MailboxDeliveryGroup>(this.mailboxDeliveryGroups, other.mailboxDeliveryGroups, (MailboxDeliveryGroup group1, MailboxDeliveryGroup group2) => group1.Match(group2)))
				{
					return RoutingUtils.NullMatch(this.localDeliveryGroup, other.localDeliveryGroup) && (this.localDeliveryGroup == null || this.localDeliveryGroup.Match(other.localDeliveryGroup));
				}
			}
			return false;
		}

		public bool TryGetDiagnosticInfo(bool verbose, DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			if (this.dagSelector == null)
			{
				diagnosticInfo = null;
				return false;
			}
			return this.dagSelector.TryGetDiagnosticInfo(verbose, parameters, out diagnosticInfo);
		}

		public void LogDagSelectorInfo()
		{
			if (this.dagSelector == null)
			{
				return;
			}
			this.dagSelector.LogDiagnosticInfo();
		}

		private static MailboxDeliveryGroupId CreateMailboxDeliveryGroupId(RoutingServerInfo serverInfo)
		{
			return new MailboxDeliveryGroupId(serverInfo.ADSite.Name, serverInfo.MajorVersion);
		}

		private static bool MailboxDeliveryToServerEnabled(RoutingServerInfo mailboxServerInfo, RouteCalculatorContext context)
		{
			return context.Core.MailboxDeliveryQueuesSupported && DatabaseRouteMap.IsServerInLocalDeliveryGroup(mailboxServerInfo, context);
		}

		private static bool IsServerInLocalDeliveryGroup(RoutingServerInfo serverInfo, RouteCalculatorContext context)
		{
			if (!context.Core.DeliveryGroupMembershipSupported)
			{
				return false;
			}
			if (context.Core.Settings.DagRoutingEnabled)
			{
				bool dagRoutingEnabled = serverInfo.DagRoutingEnabled;
				if (dagRoutingEnabled && context.ServerMap.IsInLocalDag(serverInfo))
				{
					return true;
				}
				if (dagRoutingEnabled || context.ServerMap.LocalDagExists)
				{
					return false;
				}
			}
			return serverInfo.IsSameSiteAndVersionAs(context.TopologyConfig.LocalServer);
		}

		private static void AddToLocalSiteFallbackIfNecessary(RoutingServerInfo serverInfo, RouteCalculatorContext context, ref RoutedServerCollection fallbackServerCollection)
		{
			if (!context.Core.ProxyRoutingSupported || context.Core.DeliveryGroupMembershipSupported)
			{
				return;
			}
			if (!context.ServerMap.IsInLocalSite(serverInfo))
			{
				return;
			}
			RouteInfo routeInfo = context.ServerMap.IsLocalServer(serverInfo) ? RouteInfo.LocalServerRoute : RouteInfo.LocalSiteRoute;
			if (fallbackServerCollection == null)
			{
				fallbackServerCollection = new RoutedServerCollection(routeInfo, serverInfo, context.Core);
				return;
			}
			fallbackServerCollection.AddServerForRoute(routeInfo, serverInfo, context.Core);
		}

		private void PopulateDeliveryGroups(RouteCalculatorContext context, ITenantDagQuota tenantDagQuota)
		{
			this.dagDeliveryGroups = new Dictionary<Guid, DagDeliveryGroup>(16);
			this.mailboxDeliveryGroups = new Dictionary<MailboxDeliveryGroupId, MailboxDeliveryGroup>(8);
			RoutedServerCollection routedServerCollection = null;
			foreach (RoutingServerInfo routingServerInfo in context.ServerMap.GetHubTransportServers())
			{
				if (!context.Core.MailboxDeliveryQueuesSupported || !context.ServerMap.IsLocalServer(routingServerInfo))
				{
					if (context.Core.Settings.DagRoutingEnabled && routingServerInfo.DagRoutingEnabled)
					{
						this.AddDagServer(routingServerInfo, context);
					}
					else
					{
						this.AddNonDagServer(routingServerInfo, context);
					}
					if (context.Core.VerifyHubComponentStateRestriction(routingServerInfo))
					{
						DatabaseRouteMap.AddToLocalSiteFallbackIfNecessary(routingServerInfo, context, ref routedServerCollection);
					}
				}
			}
			foreach (DagDeliveryGroup dagDeliveryGroup in this.dagDeliveryGroups.Values)
			{
				dagDeliveryGroup.UpdateIfGroupIsActiveAndRemoveInactiveServers(context.Core);
			}
			foreach (MailboxDeliveryGroup mailboxDeliveryGroup in this.mailboxDeliveryGroups.Values)
			{
				mailboxDeliveryGroup.UpdateIfGroupIsActiveAndRemoveInactiveServers(context.Core);
			}
			if (routedServerCollection != null)
			{
				this.LocalDeliveryGroup = new ProxyFallbackDeliveryGroup(routedServerCollection);
			}
			if (context.Core.Settings.DagSelectorEnabled)
			{
				this.dagSelector = new DagSelector(context.Core.Settings.DagSelectorMessageThresholdPerServer, context.Core.Settings.DagSelectorIncrementMessageThresholdFactor, context.Core.Settings.DagSelectorActiveServersForDagToBeRoutable, context.Core.Settings.DagSelectorMinimumSitesForDagToBeRoutable, context.Core.Settings.DagSelectorLogDiagnosticInfo, tenantDagQuota, this.dagDeliveryGroups.Values);
			}
		}

		private void AddDagServer(RoutingServerInfo serverInfo, RouteCalculatorContext context)
		{
			RouteInfo routeInfo;
			if (!context.ServerMap.TryGetServerRoute(serverInfo, out routeInfo))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No route information for server '{1}'. Skipping the server for DAGs.", this.whenCreated, serverInfo.Id.DistinguishedName);
				return;
			}
			ADObjectId databaseAvailabilityGroup = serverInfo.DatabaseAvailabilityGroup;
			DagDeliveryGroup dagDeliveryGroup;
			if (this.dagDeliveryGroups.TryGetValue(databaseAvailabilityGroup.ObjectGuid, out dagDeliveryGroup))
			{
				dagDeliveryGroup.AddServer(routeInfo, serverInfo, context.Core);
			}
			else
			{
				bool flag = context.Core.DeliveryGroupMembershipSupported && context.ServerMap.IsInLocalDag(serverInfo);
				dagDeliveryGroup = new DagDeliveryGroup(databaseAvailabilityGroup, routeInfo, serverInfo, flag, context.Core);
				if (TransportHelpers.AttemptAddToDictionary<Guid, DagDeliveryGroup>(this.dagDeliveryGroups, databaseAvailabilityGroup.ObjectGuid, dagDeliveryGroup, new TransportHelpers.DiagnosticsHandler<Guid, DagDeliveryGroup>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, DagDeliveryGroup>)) && flag)
				{
					this.LocalDeliveryGroup = dagDeliveryGroup;
				}
			}
			this.ProcessAddedServer(serverInfo, dagDeliveryGroup);
		}

		private void AddNonDagServer(RoutingServerInfo serverInfo, RouteCalculatorContext context)
		{
			MailboxDeliveryGroupId mailboxDeliveryGroupId = DatabaseRouteMap.CreateMailboxDeliveryGroupId(serverInfo);
			MailboxDeliveryGroup mailboxDeliveryGroup;
			if (this.mailboxDeliveryGroups.TryGetValue(mailboxDeliveryGroupId, out mailboxDeliveryGroup))
			{
				mailboxDeliveryGroup.AddHubServer(serverInfo, context.Core);
			}
			else
			{
				RouteInfo siteRouteInfo;
				if (!context.ServerMap.TryGetServerRoute(serverInfo, out siteRouteInfo))
				{
					RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No route information for server '{1}'. Skipping the server for Mailbox Delivery Groups.", this.whenCreated, serverInfo.Id.DistinguishedName);
					return;
				}
				bool flag = DatabaseRouteMap.IsServerInLocalDeliveryGroup(serverInfo, context);
				mailboxDeliveryGroup = new MailboxDeliveryGroup(mailboxDeliveryGroupId, siteRouteInfo, serverInfo, flag, context.Core);
				if (TransportHelpers.AttemptAddToDictionary<MailboxDeliveryGroupId, MailboxDeliveryGroup>(this.mailboxDeliveryGroups, mailboxDeliveryGroupId, mailboxDeliveryGroup, new TransportHelpers.DiagnosticsHandler<MailboxDeliveryGroupId, MailboxDeliveryGroup>(RoutingUtils.LogErrorWhenAddToDictionaryFails<MailboxDeliveryGroupId, MailboxDeliveryGroup>)) && flag)
				{
					this.LocalDeliveryGroup = mailboxDeliveryGroup;
				}
			}
			this.ProcessAddedServer(serverInfo, mailboxDeliveryGroup);
		}

		private void ProcessAddedServer(RoutingServerInfo serverInfo, MailboxDeliveryGroupBase deliveryGroup)
		{
			RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Added server '{1}' to mailbox delivery group '{2}'", this.whenCreated, serverInfo.Id.DistinguishedName, deliveryGroup.Name);
		}

		private void PopulateDatabaseDestinations(RouteCalculatorContext context, bool forcedReload)
		{
			this.databaseDestinations = new Dictionary<Guid, MailboxDatabaseDestination>(256);
			foreach (MiniDatabase miniDatabase in context.TopologyConfig.GetDatabases(forcedReload))
			{
				ADObjectId server = miniDatabase.Server;
				RoutingServerInfo routingServerInfo = null;
				if (!context.ServerMap.TryGetServerInfo(server, out routingServerInfo))
				{
					RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] No information about server '{1}' for Database '{2}'. Skipping the Database.", this.whenCreated, server.DistinguishedName, miniDatabase.DistinguishedName);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoOwningServerForMdb, null, new object[]
					{
						server.DistinguishedName,
						miniDatabase.DistinguishedName,
						this.whenCreated
					});
				}
				else
				{
					RouteInfo routeInfo = null;
					if (DatabaseRouteMap.MailboxDeliveryToServerEnabled(routingServerInfo, context))
					{
						routeInfo = this.CreateRouteForMailboxDelivery(miniDatabase, context);
					}
					else if (context.Core.Settings.DagRoutingEnabled && routingServerInfo.DagRoutingEnabled)
					{
						if (!this.TryGetDagDeliveryGroupRoute(miniDatabase, routingServerInfo, context, out routeInfo))
						{
							continue;
						}
					}
					else if (routingServerInfo.IsExchange2007OrLater)
					{
						if (!this.TryGetMailboxDeliveryGroupRoute(miniDatabase, routingServerInfo, context, out routeInfo))
						{
							continue;
						}
					}
					else if (!this.TryGetRGRoute(miniDatabase, routingServerInfo, context, out routeInfo))
					{
						continue;
					}
					MailboxDatabaseDestination valueToAdd = new MailboxDatabaseDestination(miniDatabase.Id, routeInfo, miniDatabase.WhenCreatedUTC);
					TransportHelpers.AttemptAddToDictionary<Guid, MailboxDatabaseDestination>(this.databaseDestinations, miniDatabase.Guid, valueToAdd, new TransportHelpers.DiagnosticsHandler<Guid, MailboxDatabaseDestination>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, MailboxDatabaseDestination>));
				}
			}
		}

		private bool TryGetDagDeliveryGroupRoute(MiniDatabase database, RoutingServerInfo mailboxServerInfo, RouteCalculatorContext context, out RouteInfo routeInfo)
		{
			routeInfo = null;
			ADObjectId databaseAvailabilityGroup = mailboxServerInfo.DatabaseAvailabilityGroup;
			RoutingUtils.ThrowIfNullOrEmpty(databaseAvailabilityGroup, "mailboxServerInfo.DatabaseAvailabilityGroup");
			DagDeliveryGroup dagDeliveryGroup = null;
			if (!this.dagDeliveryGroups.TryGetValue(databaseAvailabilityGroup.ObjectGuid, out dagDeliveryGroup))
			{
				RoutingDiag.Tracer.TraceError((long)this.GetHashCode(), "[{0}] No DAG '{1}' for Database '{2}' and owning server '{3}'. Skipping the Database.", new object[]
				{
					this.whenCreated,
					databaseAvailabilityGroup,
					database.DistinguishedName,
					mailboxServerInfo.Id.DistinguishedName
				});
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoDagForDatabase, null, new object[]
				{
					databaseAvailabilityGroup,
					database.DistinguishedName,
					this.whenCreated
				});
				return false;
			}
			return dagDeliveryGroup.TryGetDatabaseRouteInfo(database, mailboxServerInfo, context, out routeInfo);
		}

		private bool TryGetMailboxDeliveryGroupRoute(MiniDatabase database, RoutingServerInfo mailboxServerInfo, RouteCalculatorContext context, out RouteInfo routeInfo)
		{
			routeInfo = null;
			MailboxDeliveryGroupId mailboxDeliveryGroupId = DatabaseRouteMap.CreateMailboxDeliveryGroupId(mailboxServerInfo);
			MailboxDeliveryGroup mailboxDeliveryGroup = null;
			if (!this.mailboxDeliveryGroups.TryGetValue(mailboxDeliveryGroupId, out mailboxDeliveryGroup))
			{
				RoutingDiag.Tracer.TraceError((long)this.GetHashCode(), "[{0}] No Mailbox Delivery Group '{1}' for Database '{2}' and owning server '{3}'. Skipping the Database.", new object[]
				{
					this.whenCreated,
					mailboxDeliveryGroupId,
					database.DistinguishedName,
					mailboxServerInfo.Id.DistinguishedName
				});
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoDeliveryGroupForDatabase, null, new object[]
				{
					mailboxDeliveryGroupId,
					database.DistinguishedName,
					this.whenCreated
				});
				return false;
			}
			return mailboxDeliveryGroup.TryGetDatabaseRouteInfo(database, mailboxServerInfo, context, out routeInfo);
		}

		private bool TryGetRGRoute(MiniDatabase database, RoutingServerInfo serverInfo, RouteCalculatorContext context, out RouteInfo routeInfo)
		{
			routeInfo = null;
			if (!context.ServerMap.RoutingGroupRelayMap.TryGetRouteInfo(serverInfo.HomeRoutingGroup, out routeInfo))
			{
				RoutingDiag.Tracer.TraceError((long)this.GetHashCode(), "[{0}] No route for RG '{1}' for Database '{2}' and owning server '{3}'. Skipping the Database.", new object[]
				{
					this.whenCreated,
					serverInfo.HomeRoutingGroup.DistinguishedName,
					database.DistinguishedName,
					serverInfo.Id.DistinguishedName
				});
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoRoutingGroupForDatabase, null, new object[]
				{
					serverInfo.HomeRoutingGroup.DistinguishedName,
					database.DistinguishedName,
					this.whenCreated
				});
				return false;
			}
			return true;
		}

		private RouteInfo CreateRouteForMailboxDelivery(MiniDatabase database, RouteCalculatorContext context)
		{
			DeliveryType deliveryType = context.Core.Settings.SmtpDeliveryToMailboxEnabled ? DeliveryType.SmtpDeliveryToMailbox : DeliveryType.MapiDelivery;
			return RouteInfo.CreateForLocalServer(database.Name, new MailboxDeliveryHop(database.Id, deliveryType));
		}

		private void PopulatePublicFolderTreeDestinations(RouteCalculatorContext context)
		{
			RoutingDiag.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[{0}] Calculating PF Tree routes", this.whenCreated);
			TimeSpan pfReplicaAgeThreshold = context.Core.Settings.PfReplicaAgeThreshold;
			DateTime utcNow = DateTime.UtcNow;
			foreach (PublicFolderTree publicFolderTree in context.TopologyConfig.PublicFolderTrees)
			{
				if (MultiValuedPropertyBase.IsNullOrEmpty(publicFolderTree.PublicDatabases))
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] PF Tree '{1}' is not linked to any PFDBs; skipping it", this.whenCreated, publicFolderTree.DistinguishedName);
				}
				else
				{
					MailboxDatabaseDestination mailboxDatabaseDestination = null;
					DatabaseRouteMap.DatabaseAgeRanking databaseAgeRanking = DatabaseRouteMap.DatabaseAgeRanking.Unknown;
					foreach (ADObjectId adobjectId in publicFolderTree.PublicDatabases)
					{
						MailboxDatabaseDestination mailboxDatabaseDestination2;
						if (!this.databaseDestinations.TryGetValue(adobjectId.ObjectGuid, out mailboxDatabaseDestination2))
						{
							RoutingDiag.Tracer.TraceError<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Route for PFDB '{1}' not found for PF Tree '{2}'", this.whenCreated, adobjectId.DistinguishedName, publicFolderTree.DistinguishedName);
							RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoPfTreeMdbRoute, null, new object[]
							{
								adobjectId.DistinguishedName,
								publicFolderTree.DistinguishedName,
								this.whenCreated
							});
						}
						else
						{
							DatabaseRouteMap.DatabaseAgeRanking databaseAgeRanking2;
							if (mailboxDatabaseDestination2.DatabaseCreationTime == null)
							{
								databaseAgeRanking2 = DatabaseRouteMap.DatabaseAgeRanking.Unknown;
							}
							else
							{
								databaseAgeRanking2 = ((utcNow - mailboxDatabaseDestination2.DatabaseCreationTime.Value >= pfReplicaAgeThreshold) ? DatabaseRouteMap.DatabaseAgeRanking.Old : DatabaseRouteMap.DatabaseAgeRanking.New);
							}
							if (databaseAgeRanking2 >= databaseAgeRanking && (databaseAgeRanking2 != databaseAgeRanking || mailboxDatabaseDestination == null || mailboxDatabaseDestination.RouteInfo.CompareTo(mailboxDatabaseDestination2.RouteInfo, RouteComparison.IgnoreRGCosts) > 0))
							{
								mailboxDatabaseDestination = mailboxDatabaseDestination2;
								databaseAgeRanking = databaseAgeRanking2;
								if (mailboxDatabaseDestination2.RouteInfo.DestinationProximity == Proximity.LocalServer && databaseAgeRanking2 == DatabaseRouteMap.DatabaseAgeRanking.Old)
								{
									break;
								}
							}
						}
					}
					if (mailboxDatabaseDestination == null)
					{
						RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] No route created for PF Tree '{1}'", this.whenCreated, publicFolderTree.DistinguishedName);
						RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingNoPfTreeRoute, null, new object[]
						{
							publicFolderTree.DistinguishedName,
							this.whenCreated
						});
					}
					else if (TransportHelpers.AttemptAddToDictionary<Guid, MailboxDatabaseDestination>(this.databaseDestinations, publicFolderTree.Guid, mailboxDatabaseDestination, new TransportHelpers.DiagnosticsHandler<Guid, MailboxDatabaseDestination>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, MailboxDatabaseDestination>)))
					{
						RoutingDiag.Tracer.TraceDebug<DateTime, string, string>((long)this.GetHashCode(), "[{0}] Selected PF replica database '{1}' for PF Tree '{2}'", this.whenCreated, mailboxDatabaseDestination.StringIdentity, publicFolderTree.DistinguishedName);
					}
				}
			}
			RoutingDiag.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[{0}] PF Tree routes successfully calculated", this.whenCreated);
		}

		private readonly DateTime whenCreated;

		private Dictionary<Guid, MailboxDatabaseDestination> databaseDestinations;

		private Dictionary<Guid, DagDeliveryGroup> dagDeliveryGroups;

		private DagSelector dagSelector;

		private Dictionary<MailboxDeliveryGroupId, MailboxDeliveryGroup> mailboxDeliveryGroups;

		private DeliveryGroup localDeliveryGroup;

		private enum DatabaseAgeRanking
		{
			Unknown,
			New,
			Old
		}
	}
}
