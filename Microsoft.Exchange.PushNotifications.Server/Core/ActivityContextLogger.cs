using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal sealed class ActivityContextLogger : ExtensibleLogger
	{
		public ActivityContextLogger() : base(new ActivityContextLogConfig())
		{
		}

		public static void Initialize()
		{
			if (ActivityContextLogger.instance == null)
			{
				ActivityContext.RegisterMetadata(typeof(ServiceCommonMetadata));
				ActivityContext.RegisterMetadata(typeof(ServiceLatencyMetadata));
				ActivityContext.RegisterMetadata(typeof(BudgetMetadata));
				ActivityContext.RegisterMetadata(typeof(ActivityContextLogger.PushNotificationData));
				ActivityContextLogger.instance = new ActivityContextLogger();
			}
		}

		protected override ICollection<KeyValuePair<string, object>> GetComponentSpecificData(IActivityScope activityScope, string eventId)
		{
			Dictionary<string, object> dictionary = null;
			if (activityScope != null)
			{
				dictionary = new Dictionary<string, object>(ActivityContextLogger.EnumToShortKeyMapping.Count);
				ExtensibleLogger.CopyProperties(activityScope, dictionary, ActivityContextLogger.EnumToShortKeyMapping);
			}
			return dictionary;
		}

		public static readonly Dictionary<Enum, string> EnumToShortKeyMapping = new Dictionary<Enum, string>
		{
			{
				ActivityStandardMetadata.Action,
				"Cmd"
			},
			{
				ActivityContextLogger.PushNotificationData.ServiceCommandError,
				"CmdErr"
			}
		};

		private static ActivityContextLogger instance;

		public enum PushNotificationData
		{
			ServiceCommandError
		}
	}
}
