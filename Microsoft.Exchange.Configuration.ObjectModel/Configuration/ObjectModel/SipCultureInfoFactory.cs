using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal static class SipCultureInfoFactory
	{
		internal static SipCultureInfo CreateInstance(CultureInfo parentCulture, string languageCode)
		{
			if (parentCulture is SipCultureInfoBase)
			{
				throw new ArgumentException(Strings.SipCultureInfoArgumentCheckFailure);
			}
			SipCulture key = new SipCulture(parentCulture, languageCode);
			SipCultureInfo sipCultureInfo = null;
			lock (SipCultureInfoFactory.sipCultureMap)
			{
				if (SipCultureInfoFactory.sipCultureMap.ContainsKey(key))
				{
					sipCultureInfo = SipCultureInfoFactory.sipCultureMap[key];
				}
				else
				{
					sipCultureInfo = new SipCultureInfo(parentCulture, languageCode);
					SipCultureInfoFactory.sipCultureMap.Add(key, sipCultureInfo);
				}
			}
			return sipCultureInfo;
		}

		private static Dictionary<SipCulture, SipCultureInfo> sipCultureMap = new Dictionary<SipCulture, SipCultureInfo>(1);
	}
}
