using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities
{
	public class DateTimeHelper
	{
		public virtual ExTimeZone GetTimeZoneOrDefault(string id, ExTimeZone defaultTimeZone)
		{
			ExTimeZone result;
			if (!this.TryParseTimeZoneId(id, out result))
			{
				return defaultTimeZone;
			}
			return result;
		}

		public virtual bool TryParseTimeZoneId(string id, out ExTimeZone timeZone)
		{
			if (string.IsNullOrEmpty(id))
			{
				timeZone = null;
			}
			else if (id == "tzone://Microsoft/Utc")
			{
				timeZone = ExTimeZone.UtcTimeZone;
			}
			else
			{
				this.EnumeratorTryGetTimeZoneByName(id, out timeZone);
			}
			return timeZone != null;
		}

		public virtual ExDateTime ChangeTimeZone(ExDateTime time, ExTimeZone targetTimeZone, bool applyBias = true)
		{
			if (!applyBias)
			{
				return targetTimeZone.Assign(time);
			}
			return targetTimeZone.ConvertDateTime(time);
		}

		protected virtual bool EnumeratorTryGetTimeZoneByName(string name, out ExTimeZone timeZone)
		{
			ExTimeZoneEnumerator instance = ExTimeZoneEnumerator.Instance;
			return instance.TryGetTimeZoneByName(name, out timeZone);
		}
	}
}
