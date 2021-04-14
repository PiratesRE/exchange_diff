using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[Serializable]
	public class RecoveryActionEntry : IPersistence
	{
		public RecoveryActionEntry()
		{
		}

		internal RecoveryActionEntry(RecoveryActionHelper.RecoveryActionEntrySerializable entry)
		{
			this.Id = entry.Id;
			this.InstanceId = entry.InstanceId;
			this.ResourceName = entry.ResourceName;
			this.StartTime = entry.StartTime;
			this.EndTime = entry.EndTime;
			this.State = entry.State;
			this.Result = entry.Result;
			this.RequestorName = entry.RequestorName;
			this.ExceptionName = entry.ExceptionName;
			this.ExceptionMessage = entry.ExceptionMessage;
			this.Context = entry.Context;
			this.CustomArg1 = entry.CustomArg1;
			this.CustomArg2 = entry.CustomArg2;
			this.CustomArg3 = entry.CustomArg3;
			this.LamProcessStartTime = entry.LamProcessStartTime;
			this.ThrottleIdentity = entry.ThrottleIdentity;
			this.ThrottleParametersXml = entry.ThrottleParametersXml;
			this.TotalLocalActionsInOneHour = entry.TotalLocalActionsInOneHour;
			this.TotalLocalActionsInOneDay = entry.TotalLocalActionsInOneDay;
			this.TotalGroupActionsInOneDay = entry.TotalGroupActionsInOneDay;
		}

		public bool IsReadFromPersistentStore { get; set; }

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; set; }

		public RecoveryActionId Id { get; set; }

		public string InstanceId { get; set; }

		public string ResourceName { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public RecoveryActionState State { get; set; }

		public RecoveryActionResult Result { get; set; }

		public string RequestorName { get; set; }

		public string ExceptionName { get; set; }

		public string ExceptionMessage { get; set; }

		public string Context { get; set; }

		public string CustomArg1 { get; set; }

		public string CustomArg2 { get; set; }

		public string CustomArg3 { get; set; }

		public DateTime LamProcessStartTime { get; set; }

		public int WriteRecordDelayInMilliSeconds { get; set; }

		public string ThrottleIdentity { get; set; }

		public string ThrottleParametersXml { get; set; }

		public int TotalLocalActionsInOneHour { get; set; }

		public int TotalLocalActionsInOneDay { get; set; }

		public int TotalGroupActionsInOneDay { get; set; }

		public string GetName()
		{
			return RecoveryActionHelper.ConstructActionName(this.Id, this.ResourceName);
		}

		public void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData)
		{
			this.IsReadFromPersistentStore = true;
			this.LocalDataAccessMetaData = metaData;
			this.SetProperties(propertyBag);
		}

		public void SetProperties(Dictionary<string, string> propertyBag)
		{
			this.Id = this.GetEnumProperty<RecoveryActionId>(propertyBag, "Id");
			this.InstanceId = this.GetStringProperty(propertyBag, "InstanceId");
			this.ResourceName = this.GetStringProperty(propertyBag, "ResourceName");
			this.StartTime = this.GetDateTimeProperty(propertyBag, "StartTime");
			this.EndTime = this.GetDateTimeProperty(propertyBag, "EndTime");
			this.State = this.GetEnumProperty<RecoveryActionState>(propertyBag, "State");
			this.Result = this.GetEnumProperty<RecoveryActionResult>(propertyBag, "Result");
			this.RequestorName = this.GetStringProperty(propertyBag, "RequestorName");
			this.ExceptionName = this.GetStringProperty(propertyBag, "ExceptionName");
			this.ExceptionMessage = this.GetStringProperty(propertyBag, "ExceptionMessage");
			this.Context = this.GetStringProperty(propertyBag, "Context");
			this.CustomArg1 = this.GetStringProperty(propertyBag, "CustomArg1");
			this.CustomArg2 = this.GetStringProperty(propertyBag, "CustomArg2");
			this.CustomArg3 = this.GetStringProperty(propertyBag, "CustomArg3");
			this.LamProcessStartTime = this.GetDateTimeProperty(propertyBag, "LamProcessStartTime");
			this.ThrottleIdentity = this.GetStringProperty(propertyBag, "ThrottleIdentity");
			this.ThrottleParametersXml = this.GetStringProperty(propertyBag, "ThrottleParametersXml");
			this.TotalLocalActionsInOneHour = this.GetIntProperty(propertyBag, "TotalLocalActionsInOneHour");
			this.TotalLocalActionsInOneDay = this.GetIntProperty(propertyBag, "TotalLocalActionsInOneDay");
			this.TotalGroupActionsInOneDay = this.GetIntProperty(propertyBag, "TotalGroupActionsInOneDay");
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			if (this.IsReadFromPersistentStore)
			{
				throw new InvalidOperationException();
			}
			if (this.WriteRecordDelayInMilliSeconds > 0)
			{
				Thread.Sleep(this.WriteRecordDelayInMilliSeconds);
			}
			ManagedAvailabilityCrimsonEvent crimsonEventToUse = this.GetCrimsonEventToUse();
			crimsonEventToUse.LogGeneric(new object[]
			{
				this.Id,
				this.InstanceId,
				CrimsonHelper.NullCode(this.ResourceName),
				this.ToUniversalSortableTimeString(this.StartTime),
				this.ToUniversalSortableTimeString(this.EndTime),
				this.State,
				this.Result,
				CrimsonHelper.NullCode(this.RequestorName),
				CrimsonHelper.NullCode(this.ExceptionName),
				CrimsonHelper.NullCode(this.ExceptionMessage),
				CrimsonHelper.NullCode(this.Context),
				CrimsonHelper.NullCode(this.CustomArg1),
				CrimsonHelper.NullCode(this.CustomArg2),
				CrimsonHelper.NullCode(this.CustomArg3),
				this.LamProcessStartTime,
				CrimsonHelper.NullCode(this.ThrottleIdentity),
				CrimsonHelper.NullCode(this.ThrottleParametersXml),
				this.TotalLocalActionsInOneHour,
				this.TotalLocalActionsInOneDay,
				this.TotalGroupActionsInOneDay
			});
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Id"] = this.Id;
			dictionary["InstanceId"] = this.InstanceId;
			dictionary["ResourceName"] = this.ResourceName;
			dictionary["StartTime"] = this.StartTime;
			dictionary["EndTime"] = this.EndTime;
			dictionary["State"] = this.State;
			dictionary["Result"] = this.Result;
			dictionary["RequestorName"] = this.RequestorName;
			dictionary["ExceptionName"] = this.ExceptionName;
			dictionary["ExceptionMessage"] = this.ExceptionMessage;
			dictionary["Context"] = this.Context;
			dictionary["CustomArg1"] = this.CustomArg1;
			dictionary["CustomArg2"] = this.CustomArg2;
			dictionary["CustomArg3"] = this.CustomArg3;
			dictionary["LamProcessStartTime"] = this.LamProcessStartTime;
			dictionary["ThrottleIdentity"] = this.ThrottleIdentity;
			dictionary["ThrottleParametersXml"] = this.ThrottleParametersXml;
			dictionary["TotalLocalActionsInOneHour"] = this.TotalLocalActionsInOneHour;
			dictionary["TotalLocalActionsInOneDay"] = this.TotalLocalActionsInOneDay;
			dictionary["TotalGroupActionsInOneDay"] = this.TotalGroupActionsInOneDay;
			return dictionary;
		}

		private ManagedAvailabilityCrimsonEvent GetCrimsonEventToUse()
		{
			ManagedAvailabilityCrimsonEvent result = null;
			if (this.State == RecoveryActionState.Started)
			{
				result = ManagedAvailabilityCrimsonEvents.RecoveryStarted;
			}
			else if (this.State == RecoveryActionState.Finished)
			{
				if (this.Result == RecoveryActionResult.Succeeded)
				{
					result = ManagedAvailabilityCrimsonEvents.RecoverySucceeded;
				}
				else
				{
					result = ManagedAvailabilityCrimsonEvents.RecoveryFailed;
				}
			}
			return result;
		}

		private string ToUniversalSortableTimeString(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("o");
		}

		private string GetStringProperty(Dictionary<string, string> propertyBag, string propertyName)
		{
			string text = null;
			propertyBag.TryGetValue(propertyName, out text);
			text = CrimsonHelper.NullDecode(text);
			return text;
		}

		private DateTime GetDateTimeProperty(Dictionary<string, string> propertyBag, string propertyName)
		{
			DateTime result = DateTime.MinValue;
			string text;
			if (propertyBag.TryGetValue(propertyName, out text) && !string.IsNullOrEmpty(text))
			{
				result = DateTime.Parse(text);
			}
			return result;
		}

		private T GetEnumProperty<T>(Dictionary<string, string> propertyBag, string propertyName)
		{
			T result = default(T);
			string value;
			if (propertyBag.TryGetValue(propertyName, out value) && !string.IsNullOrEmpty(value))
			{
				result = (T)((object)Enum.Parse(typeof(T), value));
			}
			return result;
		}

		private int GetIntProperty(Dictionary<string, string> propertyBag, string propertyName)
		{
			int result = 0;
			string text;
			if (propertyBag.TryGetValue(propertyName, out text) && !string.IsNullOrEmpty(text))
			{
				result = int.Parse(text);
			}
			return result;
		}

		public const int MaximumAllowedElementsPerQuery = 4096;
	}
}
