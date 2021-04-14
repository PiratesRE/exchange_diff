using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal sealed class RecipientEventData
	{
		public RecipientEventData(List<RecipientTrackingEvent> mainResult)
		{
			this.mainResult = mainResult;
		}

		public RecipientEventData(List<RecipientTrackingEvent> mainResult, List<List<RecipientTrackingEvent>> handoffPaths) : this(mainResult)
		{
			this.handoffPaths = handoffPaths;
		}

		public List<RecipientTrackingEvent> Events
		{
			get
			{
				return this.mainResult;
			}
		}

		public List<List<RecipientTrackingEvent>> HandoffPaths
		{
			get
			{
				return this.handoffPaths;
			}
		}

		public static Dictionary<string, RecipientEventData> DeserializeMultiple(List<RecipientTrackingEvent> combinedReports)
		{
			Dictionary<string, List<RecipientTrackingEvent>> dictionary = new Dictionary<string, List<RecipientTrackingEvent>>();
			foreach (RecipientTrackingEvent recipientTrackingEvent in combinedReports)
			{
				string key = recipientTrackingEvent.ExtendedProperties.MessageTrackingReportId ?? string.Empty;
				List<RecipientTrackingEvent> list;
				if (!dictionary.TryGetValue(key, out list))
				{
					list = new List<RecipientTrackingEvent>();
					dictionary[key] = list;
				}
				list.Add(recipientTrackingEvent);
			}
			Dictionary<string, RecipientEventData> dictionary2 = new Dictionary<string, RecipientEventData>(dictionary.Count);
			foreach (string key2 in dictionary.Keys)
			{
				dictionary2[key2] = RecipientEventData.Deserialize(dictionary[key2]);
			}
			return dictionary2;
		}

		public List<RecipientTrackingEvent> Serialize()
		{
			if (this.mainResult != null)
			{
				foreach (RecipientTrackingEvent recipientTrackingEvent in this.mainResult)
				{
					recipientTrackingEvent.UniquePathId = "0";
				}
				return this.mainResult;
			}
			if (this.handoffPaths == null)
			{
				return null;
			}
			int num = 0;
			foreach (List<RecipientTrackingEvent> list in this.handoffPaths)
			{
				num += list.Count;
			}
			List<RecipientTrackingEvent> list2 = new List<RecipientTrackingEvent>(num);
			int num2 = 1;
			foreach (List<RecipientTrackingEvent> list3 in this.handoffPaths)
			{
				foreach (RecipientTrackingEvent recipientTrackingEvent2 in list3)
				{
					recipientTrackingEvent2.UniquePathId = num2.ToString();
				}
				list2.AddRange(list3);
				num2++;
			}
			return list2;
		}

		private static RecipientEventData Deserialize(List<RecipientTrackingEvent> combinedPaths)
		{
			Dictionary<string, List<RecipientTrackingEvent>> dictionary = new Dictionary<string, List<RecipientTrackingEvent>>();
			foreach (RecipientTrackingEvent recipientTrackingEvent in combinedPaths)
			{
				List<RecipientTrackingEvent> list = null;
				if (!dictionary.TryGetValue(recipientTrackingEvent.UniquePathId, out list))
				{
					list = new List<RecipientTrackingEvent>();
					dictionary[recipientTrackingEvent.UniquePathId] = list;
				}
				list.Add(recipientTrackingEvent);
			}
			if (dictionary.Keys.Count == 1)
			{
				using (Dictionary<string, List<RecipientTrackingEvent>>.KeyCollection.Enumerator enumerator2 = dictionary.Keys.GetEnumerator())
				{
					enumerator2.MoveNext();
					if (enumerator2.Current.Equals("0"))
					{
						return new RecipientEventData(dictionary[enumerator2.Current]);
					}
				}
			}
			List<List<RecipientTrackingEvent>> list2 = new List<List<RecipientTrackingEvent>>();
			foreach (KeyValuePair<string, List<RecipientTrackingEvent>> keyValuePair in dictionary)
			{
				list2.Add(keyValuePair.Value);
			}
			return new RecipientEventData(null, list2);
		}

		private const string MainResultUniquePathId = "0";

		private List<RecipientTrackingEvent> mainResult;

		private List<List<RecipientTrackingEvent>> handoffPaths;
	}
}
