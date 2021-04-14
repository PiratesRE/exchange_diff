using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class ScriptingAgentClassFactory : IProvisioningAgent
	{
		public ScriptingAgentClassFactory()
		{
			this.xmlConfigPath = Path.Combine(CmdletExtensionAgentsGlobalConfig.CmdletExtensionAgentsFolder, "ScriptingAgentConfig.xml");
		}

		private ScriptingAgentConfiguration Configuration
		{
			get
			{
				if (this.configuration == null || !File.Exists(this.xmlConfigPath) || this.configFileLastWriteTime < File.GetLastWriteTimeUtc(this.xmlConfigPath))
				{
					this.configuration = new ScriptingAgentConfiguration(this.xmlConfigPath);
					this.configFileLastWriteTime = DateTime.UtcNow;
				}
				return this.configuration;
			}
		}

		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.Configuration.GetAllSupportedCmdlets();
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			return new ScriptingAgentHandler(this.Configuration);
		}

		private const string ScriptingAgentConfig = "ScriptingAgentConfig.xml";

		private ScriptingAgentConfiguration configuration;

		private string xmlConfigPath;

		private DateTime configFileLastWriteTime;
	}
}
