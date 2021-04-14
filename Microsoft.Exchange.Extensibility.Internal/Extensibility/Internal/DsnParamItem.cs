using System;
using System.Globalization;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class DsnParamItem
	{
		public DsnParamItem(string[] paramNames, DsnParamItem.DsnParamItemGetStringDelegate getStringDelegate)
		{
			if (paramNames == null)
			{
				throw new ArgumentNullException("paramNames");
			}
			if (getStringDelegate == null)
			{
				throw new ArgumentNullException("getStringDelegate");
			}
			this.paramNames = paramNames;
			this.getStringDelegate = getStringDelegate;
		}

		public string GetString(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
		{
			if (!this.AllParametersAvailable(dsnParameters))
			{
				overwriteDefault = false;
				return null;
			}
			return this.getStringDelegate(dsnParameters, culture, out overwriteDefault);
		}

		private bool AllParametersAvailable(DsnParameters dsnParameters)
		{
			foreach (string key in this.paramNames)
			{
				if (!dsnParameters.ContainsKey(key) || dsnParameters[key] == null)
				{
					return false;
				}
			}
			return true;
		}

		private string[] paramNames;

		private DsnParamItem.DsnParamItemGetStringDelegate getStringDelegate;

		public delegate string DsnParamItemGetStringDelegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault);
	}
}
