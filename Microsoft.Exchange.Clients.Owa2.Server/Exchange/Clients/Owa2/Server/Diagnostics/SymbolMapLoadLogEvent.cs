using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class SymbolMapLoadLogEvent : ILogEvent
	{
		private SymbolMapLoadLogEvent()
		{
		}

		public string EventId
		{
			get
			{
				return "ClientWatsonSymbolsMapLoad";
			}
		}

		public static SymbolMapLoadLogEvent CreateForError(Exception e)
		{
			return new SymbolMapLoadLogEvent
			{
				exception = e
			};
		}

		public static SymbolMapLoadLogEvent CreateForError(string filePath, Exception e, TimeSpan elapsedTime)
		{
			return new SymbolMapLoadLogEvent
			{
				fileName = Path.GetFileNameWithoutExtension(filePath),
				exception = e,
				elapsedTimeInMilliseconds = elapsedTime.TotalMilliseconds
			};
		}

		public static SymbolMapLoadLogEvent CreateForSuccess(string filePath, TimeSpan elapsedTime)
		{
			return new SymbolMapLoadLogEvent
			{
				fileName = Path.GetFileNameWithoutExtension(filePath),
				elapsedTimeInMilliseconds = elapsedTime.TotalMilliseconds
			};
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"IS",
					(this.exception == null) ? 1 : 0
				},
				{
					"N",
					this.fileName
				},
				{
					"LT",
					this.elapsedTimeInMilliseconds
				}
			};
			if (this.exception != null)
			{
				dictionary.Add("ET", this.exception.GetType().Name);
				dictionary.Add("EM", this.exception.Message);
			}
			return dictionary;
		}

		private const string LogEventId = "ClientWatsonSymbolsMapLoad";

		private const string IsSuccessfulKey = "IS";

		private const string FileNameKey = "N";

		private const string ExceptionTypeKey = "ET";

		private const string ExceptionMessageKey = "EM";

		private const string LoadTimeKey = "LT";

		private Exception exception;

		private string fileName;

		private double elapsedTimeInMilliseconds;
	}
}
