using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Client;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility
{
	internal static class MessageHelper
	{
		public static string GetRoutingKey(ComplianceMessage message)
		{
			if (message == null || message.MessageTarget == null)
			{
				return null;
			}
			if (message.MessageTarget.TargetType == Target.Type.Driver)
			{
				return string.Format("DRIVERROUTE", new object[0]);
			}
			return string.Format("DATABASEROUTE:{0}", message.MessageTarget.Database);
		}

		public static ClientType GetClientType(ComplianceMessage message)
		{
			if (message != null && message.MessageTarget != null && message.MessageTarget.TargetType == Target.Type.Driver)
			{
				return ClientType.DriverClient;
			}
			return ClientType.ExchangeWorkloadClient;
		}

		public static ClientType GetClientType(IEnumerable<ComplianceMessage> messages)
		{
			return MessageHelper.GetClientType(messages.First<ComplianceMessage>());
		}

		public static bool IsFromDriver(ComplianceMessage message)
		{
			return message.MessageSource != null && message.MessageSource.TargetType == Target.Type.Driver;
		}

		public static bool IsToDriver(ComplianceMessage message)
		{
			return message.MessageSource != null && message.MessageTarget.TargetType == Target.Type.Driver;
		}
	}
}
