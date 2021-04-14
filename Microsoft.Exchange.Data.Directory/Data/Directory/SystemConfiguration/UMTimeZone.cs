using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ImmutableObject(true)]
	public class UMTimeZone
	{
		internal UMTimeZone(string id)
		{
			ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(id, out this.TimeZone);
		}

		internal UMTimeZone(ExTimeZone etz)
		{
			this.TimeZone = etz;
		}

		public static UMTimeZone Parse(string timeZoneName)
		{
			if (string.IsNullOrEmpty(timeZoneName))
			{
				throw new ArgumentNullException(timeZoneName);
			}
			ExTimeZone exTimeZone = null;
			foreach (ExTimeZone exTimeZone2 in ExTimeZoneEnumerator.Instance)
			{
				string text = exTimeZone2.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture);
				if (string.Compare(timeZoneName, text, CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase) == 0)
				{
					exTimeZone = exTimeZone2;
					break;
				}
				if (text.IndexOf(timeZoneName, StringComparison.OrdinalIgnoreCase) != -1)
				{
					if (exTimeZone != null)
					{
						throw new InvalidTimeZoneNameException(DirectoryStrings.AmbiguousTimeZoneNameError(timeZoneName));
					}
					exTimeZone = exTimeZone2;
				}
			}
			if (exTimeZone == null)
			{
				throw new InvalidTimeZoneNameException(DirectoryStrings.NonexistentTimeZoneError(timeZoneName));
			}
			return new UMTimeZone(exTimeZone);
		}

		public override string ToString()
		{
			if (this.TimeZone == null)
			{
				return string.Empty;
			}
			return this.TimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture);
		}

		public string Name
		{
			get
			{
				return this.ToString();
			}
		}

		internal string Id
		{
			get
			{
				if (this.TimeZone == null)
				{
					return string.Empty;
				}
				return this.TimeZone.Id;
			}
		}

		internal ExTimeZone TimeZone;
	}
}
