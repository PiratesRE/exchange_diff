using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	public class ExTimeZoneEnumerator : IEnumerable<ExTimeZone>, IEnumerable
	{
		public static ExTimeZoneEnumerator Instance
		{
			get
			{
				if (ExTimeZoneEnumerator.instance == null)
				{
					lock (ExTimeZoneEnumerator.instanceLock)
					{
						if (ExTimeZoneEnumerator.instance == null)
						{
							ExTimeZoneEnumerator.instance = new ExTimeZoneEnumerator(ExRegistryTimeZoneProvider.Instance);
						}
					}
				}
				return ExTimeZoneEnumerator.instance;
			}
		}

		public static ExTimeZoneEnumerator InstanceWithReload
		{
			get
			{
				lock (ExTimeZoneEnumerator.instanceLock)
				{
					ExTimeZoneEnumerator.instance = null;
					ExTimeZoneEnumerator.instance = new ExTimeZoneEnumerator(ExRegistryTimeZoneProvider.InstanceWithReload);
				}
				return ExTimeZoneEnumerator.instance;
			}
		}

		private ExTimeZoneEnumerator(ExRegistryTimeZoneProvider provider)
		{
			this.provider = provider;
			this.gmtOrderTimeZones = new List<ExTimeZone>(provider.GetTimeZones());
			this.gmtOrderTimeZones.Sort(new ExTimeZoneComparator());
		}

		public bool TryGetTimeZoneByName(string timeZoneName, out ExTimeZone timeZone)
		{
			return this.provider.TryGetTimeZoneById(timeZoneName, out timeZone);
		}

		public IEnumerator<ExTimeZone> GetEnumerator()
		{
			return this.gmtOrderTimeZones.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static object instanceLock = new object();

		private static ExTimeZoneEnumerator instance = null;

		private readonly ExTimeZoneProviderBase provider;

		private readonly List<ExTimeZone> gmtOrderTimeZones;
	}
}
