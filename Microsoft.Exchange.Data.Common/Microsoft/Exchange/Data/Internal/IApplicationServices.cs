using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal interface IApplicationServices
	{
		Stream CreateTemporaryStorage();

		IList<CtsConfigurationSetting> GetConfiguration(string subSectionName);

		void RefreshConfiguration();

		void LogConfigurationErrorEvent();
	}
}
