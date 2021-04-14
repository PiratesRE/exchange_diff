using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Exchange;

namespace Microsoft.Exchange.Transport
{
	internal class ResourceManagerEventLogger
	{
		public virtual void LogResourcePressureChangedEvent(ResourceUses previousResourceUses, ResourceUses currentResourceUses, string currentState)
		{
			ResourceManagerEventLogger.eventLogger.LogEvent((currentResourceUses > previousResourceUses) ? TransportEventLogConstants.Tuple_ResourceUtilizationUp : TransportEventLogConstants.Tuple_ResourceUtilizationDown, null, new object[]
			{
				ResourceManager.MapToLocalizedString(previousResourceUses),
				ResourceManager.MapToLocalizedString(currentResourceUses),
				currentState
			});
		}

		public virtual void LogResourcePressureChangedEvent(ResourceUse aggregateResourceUse, string currentState)
		{
			if (aggregateResourceUse.CurrentUseLevel != aggregateResourceUse.PreviousUseLevel)
			{
				ResourceManagerEventLogger.eventLogger.LogEvent((aggregateResourceUse.CurrentUseLevel > aggregateResourceUse.PreviousUseLevel) ? TransportEventLogConstants.Tuple_ResourceUtilizationUp : TransportEventLogConstants.Tuple_ResourceUtilizationDown, null, new object[]
				{
					this.localizedUseLevel[aggregateResourceUse.PreviousUseLevel],
					this.localizedUseLevel[aggregateResourceUse.CurrentUseLevel],
					currentState
				});
			}
		}

		public virtual void LogLowOnDiskSpaceEvent(ResourceUses resourceUses, string currentState)
		{
			ResourceManagerEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DiskSpaceLow, resourceUses.ToString(), new object[]
			{
				currentState
			});
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DiskSpaceLow", null, "Microsoft Exchange Transport is rejecting message submissions because the available disk space has dropped below the configured threshold.", ResultSeverityLevel.Warning, false);
		}

		public virtual void LogLowOnDiskSpaceEvent(UseLevel resourceUse, string currentState)
		{
			ResourceManagerEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DiskSpaceLow, resourceUse.ToString(), new object[]
			{
				currentState
			});
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DiskSpaceLow", null, "Microsoft Exchange Transport is rejecting message submissions because the available disk space has dropped below the configured threshold.", ResultSeverityLevel.Warning, false);
		}

		public virtual void LogPrivateBytesHighEvent(string currentState)
		{
			ResourceManagerEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PrivateBytesHigh, string.Empty, new object[]
			{
				currentState
			});
			this.RaiseActiveMonitoringErrorNotification(ResourceManagerEventLogger.transportRejectingMessageSubmissionsNotification, currentState);
		}

		private void RaiseActiveMonitoringErrorNotification(string notificationEvent, string message)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ResourceManagerEventLogger.transportNotificationServiceName, notificationEvent, message, ResultSeverityLevel.Error);
			eventNotificationItem.Publish(false);
		}

		private static readonly string transportNotificationServiceName = ExchangeComponent.Transport.Name;

		private static readonly string transportRejectingMessageSubmissionsNotification = TransportNotificationEvent.TransportRejectingMessageSubmissions.ToString();

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ResourceManagerTracer.Category, TransportEventLog.GetEventSource());

		private readonly Dictionary<UseLevel, string> localizedUseLevel = new Dictionary<UseLevel, string>
		{
			{
				UseLevel.Low,
				Strings.LowResourceUses
			},
			{
				UseLevel.Medium,
				Strings.MediumResourceUses
			},
			{
				UseLevel.High,
				Strings.HighResourceUses
			}
		};
	}
}
