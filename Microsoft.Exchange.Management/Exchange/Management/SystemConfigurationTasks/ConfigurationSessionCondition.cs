using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class ConfigurationSessionCondition : Condition
	{
		protected ConfigurationSessionCondition(IConfigurationSession session)
		{
			this.session = session;
		}

		protected IConfigurationSession Session
		{
			get
			{
				return this.session;
			}
			set
			{
				this.session = value;
			}
		}

		private IConfigurationSession session;
	}
}
