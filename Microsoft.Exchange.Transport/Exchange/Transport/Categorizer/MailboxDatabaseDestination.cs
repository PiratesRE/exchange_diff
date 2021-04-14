using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailboxDatabaseDestination : RoutingDestination
	{
		public MailboxDatabaseDestination(ADObjectId databaseId, RouteInfo routeInfo, DateTime? databaseCreationTime) : base(routeInfo)
		{
			RoutingUtils.ThrowIfNullOrEmpty(databaseId, "databaseId");
			this.databaseId = databaseId;
			this.databaseCreationTime = databaseCreationTime;
		}

		public override MailRecipientType DestinationType
		{
			get
			{
				return MailRecipientType.Mailbox;
			}
		}

		public override string StringIdentity
		{
			get
			{
				return this.databaseId.DistinguishedName;
			}
		}

		public DateTime? DatabaseCreationTime
		{
			get
			{
				return this.databaseCreationTime;
			}
		}

		public static RoutingDestination GetRoutingDestination(ADObjectId databaseId, RoutingContext context)
		{
			if (databaseId == null)
			{
				context.RegisterCurrentRecipientForAction(MailboxDatabaseDestination.ForkAndDeferRecipientWithNoMdbActionId, new RoutingContext.ActionOnRecipients(MailboxDatabaseDestination.ForkAndDeferRecipientWithNoMdb));
				return MailboxDatabaseDestination.NoDatabaseDestination;
			}
			MailboxDatabaseDestination result;
			if (!context.RoutingTables.DatabaseMap.TryGetDatabaseDestination(databaseId, out result))
			{
				return new UnroutableDestination(MailRecipientType.Mailbox, databaseId.ToString(), UnreachableNextHop.NoRouteToDatabase);
			}
			return result;
		}

		public static bool TryGetNextHop(NextHopSolutionKey nextHopKey, RoutingTables routingTables, out RoutingNextHop nextHop)
		{
			nextHop = null;
			if (nextHopKey.NextHopType.DeliveryType == DeliveryType.SmtpRelayToDag)
			{
				DagDeliveryGroup dagDeliveryGroup;
				if (!routingTables.DatabaseMap.TryGetDagDeliveryGroup(nextHopKey.NextHopConnector, out dagDeliveryGroup))
				{
					RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] No DAG delivery group for next hop key <{1}>", routingTables.WhenCreated, nextHopKey);
					return false;
				}
				nextHop = dagDeliveryGroup;
				return true;
			}
			else
			{
				if (nextHopKey.NextHopType.DeliveryType != DeliveryType.SmtpRelayToMailboxDeliveryGroup)
				{
					throw new ArgumentOutOfRangeException("nextHopKey", nextHopKey.NextHopType.DeliveryType, "Unexpected nextHopKey.NextHopType.DeliveryType: " + nextHopKey.NextHopType.DeliveryType);
				}
				MailboxDeliveryGroup mailboxDeliveryGroup;
				if (!routingTables.DatabaseMap.TryGetMailboxDeliveryGroup(new MailboxDeliveryGroupId(nextHopKey.NextHopDomain), out mailboxDeliveryGroup))
				{
					RoutingDiag.Tracer.TraceError<DateTime, NextHopSolutionKey>(0L, "[{0}] No mailbox delivery group for next hop key <{1}>", routingTables.WhenCreated, nextHopKey);
					return false;
				}
				nextHop = mailboxDeliveryGroup;
				return true;
			}
		}

		public bool Match(MailboxDatabaseDestination other)
		{
			return base.RouteInfo.Match(other.RouteInfo, NextHopMatch.GuidOnly);
		}

		private static void ForkAndDeferRecipientWithNoMdb(Guid actionId, ICollection<MailRecipient> recipients, RoutingContext context)
		{
			context.Core.Dependencies.BifurcateRecipientsAndDefer(context.MailItem, recipients, context.TaskContext, SmtpResponse.RecipientDeferredNoMdb, context.Core.Settings.DeferralTimeForNoMdb, DeferReason.RecipientHasNoMdb);
		}

		private static readonly UnroutableDestination NoDatabaseDestination = new UnroutableDestination(MailRecipientType.Mailbox, "<No Home Database>", UnreachableNextHop.NoDatabase);

		private static readonly Guid ForkAndDeferRecipientWithNoMdbActionId = Guid.NewGuid();

		private ADObjectId databaseId;

		private DateTime? databaseCreationTime;
	}
}
