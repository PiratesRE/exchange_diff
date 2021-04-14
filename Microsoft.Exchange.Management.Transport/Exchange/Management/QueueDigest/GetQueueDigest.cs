using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueDigest;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Management.QueueDigest
{
	[Cmdlet("Get", "QueueDigest", DefaultParameterSetName = "DagParameterSet")]
	[OutputType(new Type[]
	{
		typeof(QueueDigestPresentationObject)
	})]
	public sealed class GetQueueDigest : DataAccessTask<QueueDigestPresentationObject>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ServerParameterSet")]
		public MultiValuedProperty<ServerIdParameter> Server
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "DagParameterSet")]
		public MultiValuedProperty<DatabaseAvailabilityGroupIdParameter> Dag
		{
			get
			{
				return (MultiValuedProperty<DatabaseAvailabilityGroupIdParameter>)base.Fields["Dag"];
			}
			set
			{
				base.Fields["Dag"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "SiteParameterSet")]
		public MultiValuedProperty<AdSiteIdParameter> Site
		{
			get
			{
				return (MultiValuedProperty<AdSiteIdParameter>)base.Fields["Site"];
			}
			set
			{
				base.Fields["Site"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ForestParameterSet")]
		public SwitchParameter Forest
		{
			get
			{
				return (SwitchParameter)(base.Fields["Forest"] ?? false);
			}
			set
			{
				base.Fields["Forest"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public QueueDigestGroupBy GroupBy
		{
			get
			{
				return (QueueDigestGroupBy)(base.Fields["GroupBy"] ?? QueueDigestGroupBy.NextHopDomain);
			}
			set
			{
				base.Fields["GroupBy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DetailsLevel DetailsLevel
		{
			get
			{
				return (DetailsLevel)(base.Fields["DetailsLevel"] ?? DetailsLevel.Normal);
			}
			set
			{
				base.Fields["DetailsLevel"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				this.queryFilter = new MonadFilter(value, this, ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>()).InnerFilter;
				DateTimeConverter.ConvertQueryFilter(this.queryFilter);
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return this.resultSize;
			}
			set
			{
				this.resultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Mtrt
		{
			get
			{
				return (SwitchParameter)(base.Fields["Mtrt"] ?? false);
			}
			set
			{
				base.Fields["Mtrt"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeE14Servers
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeE14Servers"] ?? false);
			}
			set
			{
				base.Fields["IncludeE14Servers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		internal QueryFilter QueryFilter
		{
			get
			{
				return this.queryFilter;
			}
		}

		internal bool IsVerbose
		{
			get
			{
				return object.Equals(base.UserSpecifiedParameters["Verbose"], true);
			}
		}

		protected override void InternalValidate()
		{
			try
			{
				if (base.ParameterSetName == "ForestParameterSet" && !this.Forest.ToBool())
				{
					base.WriteError(new LocalizedException(Strings.GetQueueDigestForestParameterCannotBeFalse), ErrorCategory.InvalidArgument, null);
				}
				this.ResolveParameters();
				base.InternalValidate();
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (this.IncludeE14Servers.ToBool() || !this.Mtrt.ToBool())
			{
				this.session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 330, "InternalStateReset", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\Queueviewer\\GetQueueDigest.cs");
				TransportConfigContainer transportConfigContainer = this.session.FindSingletonConfigurationObject<TransportConfigContainer>();
				Server server = this.session.FindLocalServer();
				this.impl = new GetQueueDigestWebServiceImpl(new GetQueueDigestCmdletAdapter(this), this.session, server.ServerSite, transportConfigContainer.DiagnosticsAggregationServicePort);
				return;
			}
			this.impl = new GetQueueDigestMtrtImpl(this);
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.WriteDebug("InternalProcessRecord called");
				if (this.ResultSize.IsUnlimited || this.ResultSize.Value != 0U)
				{
					base.InternalProcessRecord();
					this.impl.ProcessRecord();
				}
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return null;
		}

		private void ResolveParameters()
		{
			if (this.Server != null)
			{
				this.ResolveServer();
				return;
			}
			if (this.Dag != null)
			{
				this.ResolveDag();
				return;
			}
			if (this.Site != null)
			{
				this.ResolveAdSite();
				return;
			}
			this.ResolveForForest();
		}

		private void ResolveForForest()
		{
			this.impl.ResolveForForest();
		}

		private void ResolveDag()
		{
			foreach (DatabaseAvailabilityGroupIdParameter databaseAvailabilityGroupIdParameter in this.Dag)
			{
				DatabaseAvailabilityGroup dag = base.GetDataObject<DatabaseAvailabilityGroup>(databaseAvailabilityGroupIdParameter, this.session, null, new LocalizedString?(Strings.ErrorDagNotFound(databaseAvailabilityGroupIdParameter.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(databaseAvailabilityGroupIdParameter.ToString()))) as DatabaseAvailabilityGroup;
				this.impl.ResolveDag(dag);
			}
		}

		private void ResolveAdSite()
		{
			foreach (AdSiteIdParameter adSiteIdParameter in this.Site)
			{
				ADSite adSite = base.GetDataObject<ADSite>(adSiteIdParameter, this.session, null, new LocalizedString?(Strings.GetQueueDigestSiteNotFound(adSiteIdParameter)), new LocalizedString?(Strings.GetQueueDigestAmbiguosSite(adSiteIdParameter))) as ADSite;
				this.impl.ResolveAdSite(adSite);
			}
		}

		private void ResolveServer()
		{
			foreach (ServerIdParameter serverIdParameter in this.Server)
			{
				Server server = base.GetDataObject<Server>(serverIdParameter, this.session, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString()))) as Server;
				this.impl.ResolveServer(server);
			}
		}

		internal const string ServerParameterSetName = "ServerParameterSet";

		internal const string DagParameterSetName = "DagParameterSet";

		internal const string SiteParameterSetName = "SiteParameterSet";

		internal const string ForestParameterSetName = "ForestParameterSet";

		private const string ForestParameterNameKey = "Forest";

		private const string ServerParameterNameKey = "Server";

		private const string DagParameterNameKey = "Dag";

		private const string SiteParameterNameKey = "Site";

		private const string GroupByParameterNameKey = "GroupBy";

		private const string DetailsLevelParameterNameKey = "DetailsLevel";

		private const string FilterParameterNameKey = "Filter";

		private const string MtrtParemeterNameKey = "Mtrt";

		private const string IncludeE14ServersParameterNameKey = "IncludeE14Servers";

		private static readonly uint DefaultResultSize = 100U;

		private Unlimited<uint> resultSize = new Unlimited<uint>(GetQueueDigest.DefaultResultSize);

		private ITopologyConfigurationSession session;

		private QueryFilter queryFilter;

		private EnhancedTimeSpan timeout = EnhancedTimeSpan.FromSeconds(8.0);

		private GetQueueDigestImpl impl;
	}
}
