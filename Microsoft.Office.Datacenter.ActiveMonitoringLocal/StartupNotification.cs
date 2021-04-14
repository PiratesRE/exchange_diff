using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public sealed class StartupNotification : IPersistence
	{
		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		public string NotificationId { get; internal set; }

		public void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData)
		{
			this.LocalDataAccessMetaData = metaData;
			this.SetProperties(propertyBag);
		}

		public void SetProperties(Dictionary<string, string> propertyBag)
		{
			string inputStr = null;
			if (propertyBag.TryGetValue("NotificationId", out inputStr))
			{
				this.NotificationId = CrimsonHelper.NullDecode(inputStr);
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			StartupNotification.InsertStartupNotification(this.NotificationId);
		}

		public static void SetStartupNotificationDefinition(WorkDefinition definition, string notificationId, int maxStartWaitInSeconds)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			if (string.IsNullOrWhiteSpace(notificationId))
			{
				throw new ArgumentException("notificationId cannot be null or empty.");
			}
			if (maxStartWaitInSeconds <= 0)
			{
				throw new ArgumentOutOfRangeException("maxStartWaitInSeconds", maxStartWaitInSeconds, "Must be greater than 0.");
			}
			definition.Attributes["StartupNotificationId"] = notificationId;
			definition.Attributes["StartupNotificationMaxStartWaitInSeconds"] = maxStartWaitInSeconds.ToString();
		}

		public static void InsertStartupNotification(string notificationId)
		{
			ManagedAvailabilityCrimsonEvents.StartupNotification.Log<string>(notificationId);
		}

		public static DateTime GetSystemBootTime(bool isRestrictPrecisionToSeconds = true)
		{
			long tickCount = NativeMethods.GetTickCount64();
			DateTime result = DateTime.UtcNow - TimeSpan.FromMilliseconds((double)tickCount);
			if (isRestrictPrecisionToSeconds)
			{
				result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second, result.Kind);
			}
			return result;
		}
	}
}
