using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarItemState
	{
		internal CalendarItemState()
		{
			this.stateData = new Dictionary<string, object>();
		}

		internal object this[string key]
		{
			get
			{
				return this.stateData[key];
			}
			set
			{
				this.stateData[key] = value;
			}
		}

		internal bool ContainsKey(string key)
		{
			return this.stateData.ContainsKey(key);
		}

		private Dictionary<string, object> stateData;
	}
}
