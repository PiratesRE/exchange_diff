using System;

namespace Microsoft.Exchange.Net
{
	public class PswsAppSettings : AutoLoadAppSettings, IAppSettings
	{
		private PswsAppSettings()
		{
		}

		public new static PswsAppSettings Instance
		{
			get
			{
				PswsAppSettings result;
				if ((result = PswsAppSettings.instance) == null)
				{
					result = (PswsAppSettings.instance = new PswsAppSettings());
				}
				return result;
			}
		}

		bool IAppSettings.FailFastEnabled
		{
			get
			{
				throw new NotSupportedException("FailFastEnabled is not supported to be used in PswsAppSettings.");
			}
		}

		private static PswsAppSettings instance;
	}
}
