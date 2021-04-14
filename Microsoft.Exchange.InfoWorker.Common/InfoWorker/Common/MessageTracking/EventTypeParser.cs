using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class EventTypeParser
	{
		internal static bool TryParse(string rawString, out MessageTrackingEvent enumValue)
		{
			return EventTypeParser.dictionary.TryGetValue(rawString, out enumValue);
		}

		internal static Dictionary<string, MessageTrackingEvent> CreateTypeDictionary()
		{
			Dictionary<string, MessageTrackingEvent> dictionary = new Dictionary<string, MessageTrackingEvent>();
			string[] names = Enum.GetNames(typeof(MessageTrackingEvent));
			Array values = Enum.GetValues(typeof(MessageTrackingEvent));
			int num = names.Length;
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(names[i], (MessageTrackingEvent)values.GetValue(i));
			}
			return dictionary;
		}

		private static Dictionary<string, MessageTrackingEvent> dictionary = EventTypeParser.CreateTypeDictionary();
	}
}
