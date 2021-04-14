using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Serializable]
	public class BposServiceInstanceInfo : ConfigurableObject
	{
		internal BposServiceInstanceInfo(ServiceInstanceId serviceInstanceId, string endpointName, Uri backSyncUrl, bool authorityTransferIsSupported) : this()
		{
			this.Identity = serviceInstanceId;
			this.EndpointName = endpointName;
			this.BackSyncUrl = backSyncUrl;
			this.AuthorityTransferIsSupported = authorityTransferIsSupported;
		}

		internal BposServiceInstanceInfo() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public new ServiceInstanceId Identity
		{
			get
			{
				return (ServiceInstanceId)this[BposServiceInstanceInfoSchema.Identity];
			}
			internal set
			{
				this[BposServiceInstanceInfoSchema.Identity] = value;
			}
		}

		public Uri BackSyncUrl
		{
			get
			{
				return (Uri)base[BposServiceInstanceInfoSchema.BackSyncUrl];
			}
			private set
			{
				base[BposServiceInstanceInfoSchema.BackSyncUrl] = value;
			}
		}

		public string EndpointName
		{
			get
			{
				return (string)base[BposServiceInstanceInfoSchema.EndpointName];
			}
			private set
			{
				base[BposServiceInstanceInfoSchema.EndpointName] = value;
			}
		}

		public bool AuthorityTransferIsSupported
		{
			get
			{
				return (bool)base[BposServiceInstanceInfoSchema.AuthorityTransferIsSupported];
			}
			private set
			{
				base[BposServiceInstanceInfoSchema.AuthorityTransferIsSupported] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return BposServiceInstanceInfo.schema;
			}
		}

		public const string BackSyncUrlEndpointName = "BackSyncPSConnectionURI";

		public const string SupportsAuthorityTransfer = "SupportsAuthorityTransfer";

		private static BposServiceInstanceInfoSchema schema = ObjectSchema.GetInstance<BposServiceInstanceInfoSchema>();
	}
}
