using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ClientAccessArray", DefaultParameterSetName = "Identity")]
	public sealed class GetClientAccessArray : GetSystemConfigurationObjectTask<ClientAccessArrayIdParameter, ClientAccessArray>
	{
		public GetClientAccessArray()
		{
			this.arrayTaskCommon = new ClientAccessArrayTaskHelper(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		[Parameter(Mandatory = false)]
		public AdSiteIdParameter Site
		{
			get
			{
				return (AdSiteIdParameter)base.Fields[ClientAccessArraySchema.Site];
			}
			set
			{
				base.Fields[ClientAccessArraySchema.Site] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ClientAccessArray.GetParentContainer((ITopologyConfigurationSession)this.ConfigurationSession);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.siteId = ((this.Site != null) ? this.arrayTaskCommon.GetADSite(this.Site, (ITopologyConfigurationSession)this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADSite>)).Id : null);
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ClientAccessArray clientAccessArray = (ClientAccessArray)dataObject;
			if (clientAccessArray.IsPriorTo15ExchangeObjectVersion)
			{
				Server[] cachedServers = ExchangeRpcClientAccess.GetAllPossibleServers((ITopologyConfigurationSession)this.ConfigurationSession, this.siteId).ToArray<Server>();
				ExchangeRpcClientAccess[] all = ExchangeRpcClientAccess.GetAll((ITopologyConfigurationSession)this.ConfigurationSession);
				clientAccessArray.CompleteAllCalculatedProperties(cachedServers, all);
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					this.arrayTaskCommon.GetSiteFilter(this.siteId)
				});
			}
		}

		private readonly ClientAccessArrayTaskHelper arrayTaskCommon;

		private ADObjectId siteId;
	}
}
