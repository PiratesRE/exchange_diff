using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ReceiveConnector", DefaultParameterSetName = "Identity")]
	public sealed class GetReceiveConnector : GetSystemConfigurationObjectTask<ReceiveConnectorIdParameter, ReceiveConnector>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Server", ValueFromPipeline = true)]
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

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ReceiveConnector receiveConnector = dataObject as ReceiveConnector;
			if (receiveConnector != null && !receiveConnector.IsReadOnly)
			{
				Server permissionGroupsBasedOnSecurityDescriptor = (Server)base.GetDataObject<Server>(new ServerIdParameter(receiveConnector.Server), base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(receiveConnector.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(receiveConnector.Server.ToString())));
				if (base.HasErrors)
				{
					return;
				}
				try
				{
					receiveConnector.SetPermissionGroupsBasedOnSecurityDescriptor(permissionGroupsBasedOnSecurityDescriptor);
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidOperation, dataObject);
					return;
				}
				receiveConnector.ResetChangeTracking();
			}
			base.WriteResult(dataObject);
		}

		protected override void InternalValidate()
		{
			if (this.Server != null)
			{
				Server server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				if (base.HasErrors)
				{
					return;
				}
				this.rootId = (ADObjectId)server.Identity;
			}
			base.InternalValidate();
		}

		private ADObjectId rootId;
	}
}
