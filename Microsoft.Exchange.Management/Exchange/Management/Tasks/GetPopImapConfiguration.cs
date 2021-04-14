using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class GetPopImapConfiguration<TDataObject> : GetSingletonSystemConfigurationObjectTask<TDataObject> where TDataObject : PopImapAdConfiguration, new()
	{
		public GetPopImapConfiguration()
		{
			TDataObject tdataObject = Activator.CreateInstance<TDataObject>();
			this.protocolName = tdataObject.ProtocolName;
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ServerIdParameter serverIdParameter = this.Server ?? ServerIdParameter.Parse(Environment.MachineName);
				Server server = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				return PopImapAdConfiguration.GetRootId(server, this.protocolName);
			}
		}

		private readonly string protocolName;
	}
}
