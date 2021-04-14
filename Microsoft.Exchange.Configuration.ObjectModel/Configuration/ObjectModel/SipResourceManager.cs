using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class SipResourceManager : ResourceManager
	{
		internal SipResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
		{
		}

		public override string GetString(string name)
		{
			return this.GetString(name, CultureInfo.CurrentUICulture);
		}

		public override string GetString(string name, CultureInfo culture)
		{
			SipCultureInfoBase sipCultureInfoBase = culture as SipCultureInfoBase;
			if (sipCultureInfoBase != null)
			{
				try
				{
					sipCultureInfoBase.UseSipName = true;
					return base.GetString(name, sipCultureInfoBase);
				}
				finally
				{
					sipCultureInfoBase.UseSipName = false;
				}
			}
			return base.GetString(name, culture);
		}
	}
}
