using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class NewPopImapConfiguration<TDataObject> : NewFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : PopImapAdConfiguration, new()
	{
		public NewPopImapConfiguration()
		{
			TDataObject dataObject = this.DataObject;
			dataObject.Name = "1";
			TDataObject dataObject2 = this.DataObject;
			dataObject2.LogPerFileSizeQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.Zero);
		}

		[Parameter(Mandatory = true)]
		public string ExchangePath
		{
			get
			{
				return (string)base.Fields["ExchangePath"];
			}
			set
			{
				base.Fields["ExchangePath"] = value;
			}
		}

		protected abstract string ProtocolName { get; }

		protected override ObjectId RootId
		{
			get
			{
				ServerIdParameter serverIdParameter = ServerIdParameter.Parse(Environment.MachineName);
				Server server = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				Server server2 = server;
				TDataObject dataObject = this.DataObject;
				return PopImapAdConfiguration.GetRootId(server2, dataObject.ProtocolName);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			ADObjectId adobjectId = this.RootId as ADObjectId;
			tdataObject.SetId(adobjectId.GetChildId(tdataObject.Name));
			tdataObject.LogFileLocation = Path.Combine(this.GetLoggingPath(), this.ProtocolName);
			return tdataObject;
		}

		protected override void InternalProcessRecord()
		{
			IConfigDataProvider dataSession = base.DataSession;
			TDataObject dataObject = this.DataObject;
			if (dataSession.Read<TDataObject>(dataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
		}

		internal TDataObject DefaultConfiguration
		{
			get
			{
				return this.DataObject;
			}
		}

		protected string GetLoggingPath()
		{
			return Path.Combine(this.ExchangePath, "Logging");
		}

		private const string LoggingSubDirectory = "Logging";
	}
}
