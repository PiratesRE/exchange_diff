using System;
using System.Collections;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class TaskRecurrenceData : INestedData
	{
		public TaskRecurrenceData()
		{
			this.subProperties = new Hashtable();
		}

		public bool IsRecurring
		{
			get
			{
				return this.subProperties["IsRecurring"] != null;
			}
			set
			{
				this.subProperties["IsRecurring"] = value.ToString();
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public void Clear()
		{
			this.subProperties.Clear();
		}

		private IDictionary subProperties;
	}
}
