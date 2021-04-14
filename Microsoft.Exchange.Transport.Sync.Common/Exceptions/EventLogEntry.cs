using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class EventLogEntry
	{
		public EventLogEntry(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			this.tuple = tuple;
			this.periodicKey = periodicKey;
			this.messageArgs = messageArgs;
		}

		public ExEventLog.EventTuple Tuple
		{
			get
			{
				return this.tuple;
			}
		}

		public string PeriodicKey
		{
			get
			{
				return this.periodicKey;
			}
		}

		public object[] MessageArgs
		{
			get
			{
				return this.messageArgs;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			EventLogEntry eventLogEntry = obj as EventLogEntry;
			if (eventLogEntry == null)
			{
				return false;
			}
			if (this.Tuple.EventId != eventLogEntry.Tuple.EventId || this.Tuple.CategoryId != eventLogEntry.Tuple.CategoryId || this.Tuple.EntryType != eventLogEntry.Tuple.EntryType || this.Tuple.Level != eventLogEntry.Tuple.Level || this.Tuple.Period != eventLogEntry.Tuple.Period)
			{
				return false;
			}
			if (this.PeriodicKey != eventLogEntry.PeriodicKey)
			{
				return false;
			}
			if ((this.MessageArgs == null && eventLogEntry.MessageArgs != null) || (this.MessageArgs != null && eventLogEntry.MessageArgs == null))
			{
				return false;
			}
			if (this.MessageArgs != null && eventLogEntry.MessageArgs != null)
			{
				if (this.MessageArgs.Length != eventLogEntry.MessageArgs.Length)
				{
					return false;
				}
				for (int i = 0; i < this.MessageArgs.Length; i++)
				{
					object objA = this.MessageArgs[i];
					object objB = eventLogEntry.MessageArgs[i];
					if (!object.Equals(objA, objB))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EventId:");
			stringBuilder.Append(this.Tuple.EventId);
			stringBuilder.Append(",");
			stringBuilder.Append("CategoryId:");
			stringBuilder.Append(this.Tuple.CategoryId);
			stringBuilder.Append(",");
			stringBuilder.Append("EntryType:");
			stringBuilder.Append(this.Tuple.EntryType);
			stringBuilder.Append(",");
			stringBuilder.Append("Level:");
			stringBuilder.Append(this.Tuple.Level);
			stringBuilder.Append(",");
			stringBuilder.Append("Period:");
			stringBuilder.Append(this.Tuple.Period);
			stringBuilder.Append(",");
			stringBuilder.Append("PeriodicKey:");
			stringBuilder.Append(this.PeriodicKey);
			stringBuilder.Append(",");
			stringBuilder.Append("MessageArgs:");
			if (this.MessageArgs != null)
			{
				foreach (object value in this.MessageArgs)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}

		private ExEventLog.EventTuple tuple;

		private string periodicKey;

		private object[] messageArgs;
	}
}
