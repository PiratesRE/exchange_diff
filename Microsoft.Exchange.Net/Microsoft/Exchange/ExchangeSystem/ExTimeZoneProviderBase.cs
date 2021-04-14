using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	public class ExTimeZoneProviderBase
	{
		public ExTimeZoneProviderBase(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == string.Empty)
			{
				throw new ArgumentException("id");
			}
			this.Id = id;
			this.TimeZoneByIdDictionary = new Dictionary<string, ExTimeZone>(StringComparer.OrdinalIgnoreCase);
			this.TimeZoneRuleByIdDictionary = new Dictionary<string, ExTimeZoneRule>();
		}

		public bool TryGetTimeZoneByName(string name, out ExTimeZone timeZone)
		{
			return this.TimeZoneByIdDictionary.TryGetValue(name, out timeZone);
		}

		public bool TryGetTimeZoneById(string timeZoneId, out ExTimeZone timeZone)
		{
			return this.TimeZoneByIdDictionary.TryGetValue(timeZoneId, out timeZone);
		}

		public bool TryGetTimeZoneRuleById(string timeZoneRuleId, out ExTimeZoneRule timeZoneRule)
		{
			return this.TimeZoneRuleByIdDictionary.TryGetValue(timeZoneRuleId, out timeZoneRule);
		}

		public IEnumerable<ExTimeZone> GetTimeZones()
		{
			return this.TimeZoneByIdDictionary.Values;
		}

		public void AddTimeZone(ExTimeZone timeZone)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (this.TimeZoneByIdDictionary.ContainsKey(timeZone.Id))
			{
				throw new InvalidTimeZoneException("Time zone id already exists");
			}
			if (this.TimeZoneByIdDictionary.ContainsKey(timeZone.Id))
			{
				throw new InvalidTimeZoneException("Time zone name already exists");
			}
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup in timeZone.TimeZoneInformation.Groups)
			{
				foreach (ExTimeZoneRule exTimeZoneRule in exTimeZoneRuleGroup.Rules)
				{
					if (this.TimeZoneRuleByIdDictionary.ContainsKey(exTimeZoneRule.Id))
					{
						throw new InvalidTimeZoneException("Time zone rule id already exists: Id={0}" + exTimeZoneRule.Id);
					}
					this.TimeZoneRuleByIdDictionary[exTimeZoneRule.Id] = exTimeZoneRule;
				}
			}
			this.TimeZoneByIdDictionary[timeZone.Id] = timeZone;
		}

		private protected Dictionary<string, ExTimeZone> TimeZoneByIdDictionary { protected get; private set; }

		private protected Dictionary<string, ExTimeZoneRule> TimeZoneRuleByIdDictionary { protected get; private set; }

		public readonly string Id;
	}
}
