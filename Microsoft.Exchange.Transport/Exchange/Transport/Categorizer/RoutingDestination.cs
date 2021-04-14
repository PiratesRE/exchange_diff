using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class RoutingDestination
	{
		protected RoutingDestination(RouteInfo routeInfo)
		{
			RoutingUtils.ThrowIfNull(routeInfo, "routeInfo");
			this.routeInfo = routeInfo;
		}

		public abstract MailRecipientType DestinationType { get; }

		public abstract string StringIdentity { get; }

		public RouteInfo RouteInfo
		{
			get
			{
				return this.routeInfo;
			}
		}

		public virtual RoutingNextHop GetNextHop(RoutingContext context)
		{
			if (context.CurrentRecipient.RoutingOverride == null && RoutingDestination.ShouldMakeMailItemShadowed(context))
			{
				context.RegisterCurrentRecipientForAction(RoutingDestination.TrackRedirectForShadowRedundancyActionId, new RoutingContext.ActionOnRecipients(RoutingDestination.TrackRedirectForShadowRedundancy));
				return context.RoutingTables.DatabaseMap.LocalDeliveryGroup;
			}
			RoutingNextHop result;
			if (this.ViolatesRouteRestrictions(context, out result))
			{
				return result;
			}
			return this.routeInfo.NextHop;
		}

		private static bool ShouldMakeMailItemShadowed(RoutingContext context)
		{
			bool? shouldMakeMailItemShadowed = context.ShouldMakeMailItemShadowed;
			if (shouldMakeMailItemShadowed != null)
			{
				return shouldMakeMailItemShadowed.Value;
			}
			if (RoutingDestination.RoutedForShadowRedundancyOnOtherHub(context))
			{
				shouldMakeMailItemShadowed = new bool?(false);
			}
			else
			{
				shouldMakeMailItemShadowed = new bool?(context.Core.MailboxDeliveryQueuesSupported && context.MailItem.RouteForHighAvailability && context.Core.Dependencies.ShadowRedundancyPromotionEnabled && !context.MailItem.IsShadowedByXShadow() && context.Core.Dependencies.ShouldShadowMailItem(context.MailItem) && context.RoutingTables.DatabaseMap.LocalDeliveryGroup != null);
			}
			context.ShouldMakeMailItemShadowed = shouldMakeMailItemShadowed;
			if (shouldMakeMailItemShadowed.Value)
			{
				RoutingDestination.StampRoutedForShadowRedundancyHeader(context);
			}
			return shouldMakeMailItemShadowed.Value;
		}

		private static bool RoutedForShadowRedundancyOnOtherHub(RoutingContext context)
		{
			Header header = context.MailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Forest-RoutedForHighAvailability");
			return header != null && !string.IsNullOrEmpty(header.Value) && !header.Value.Equals(context.Core.Dependencies.LocalComputerFqdn, StringComparison.OrdinalIgnoreCase);
		}

		private static void StampRoutedForShadowRedundancyHeader(RoutingContext context)
		{
			Header header = context.MailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Forest-RoutedForHighAvailability");
			if (header != null)
			{
				if (string.IsNullOrEmpty(header.Value))
				{
					context.MailItem.RootPart.Headers.RemoveAll("X-MS-Exchange-Forest-RoutedForHighAvailability");
				}
				else if (!header.Value.Equals(context.Core.Dependencies.LocalComputerFqdn, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("Header already stamped on other server: " + header.Value);
				}
			}
			Header header2 = Header.Create("X-MS-Exchange-Forest-RoutedForHighAvailability");
			header2.Value = context.Core.Dependencies.LocalComputerFqdn;
			context.MailItem.RootPart.Headers.AppendChild(header2);
		}

		private static void TrackRedirectForShadowRedundancy(Guid actionId, ICollection<MailRecipient> recipients, RoutingContext context)
		{
			MessageTrackingLog.TrackHighAvailabilityRedirect(MessageTrackingSource.ROUTING, context.MailItem, recipients, "NoShadowServer");
		}

		private bool ViolatesRouteRestrictions(RoutingContext context, out RoutingNextHop restrictionViolationHop)
		{
			restrictionViolationHop = null;
			if (context.MessageSize > this.routeInfo.MaxMessageSize)
			{
				RoutingDiag.Tracer.TraceError((long)this.GetHashCode(), "[{0}] Recipient '{1}' failed to be routed because the message size {2} is over the limit of {3} for route {4}", new object[]
				{
					context.Timestamp,
					context.CurrentRecipient.Email.ToString(),
					context.MessageSize,
					this.routeInfo.MaxMessageSize,
					this.routeInfo.DestinationName
				});
				restrictionViolationHop = NdrNextHop.MessageTooLargeForRoute;
				return true;
			}
			return false;
		}

		private static readonly Guid TrackRedirectForShadowRedundancyActionId = Guid.NewGuid();

		private RouteInfo routeInfo;
	}
}
