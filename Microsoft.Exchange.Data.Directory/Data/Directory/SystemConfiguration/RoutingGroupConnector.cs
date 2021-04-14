using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class RoutingGroupConnector : SendConnector
	{
		public ADObjectId TargetRoutingGroup
		{
			get
			{
				return (ADObjectId)this[RoutingGroupConnectorSchema.TargetRoutingGroup];
			}
			internal set
			{
				this[RoutingGroupConnectorSchema.TargetRoutingGroup] = value;
			}
		}

		[Parameter]
		public int Cost
		{
			get
			{
				return (int)this[RoutingGroupConnectorSchema.Cost];
			}
			set
			{
				this[RoutingGroupConnectorSchema.Cost] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> TargetTransportServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoutingGroupConnectorSchema.TargetTransportServers];
			}
			set
			{
				this[RoutingGroupConnectorSchema.TargetTransportServers] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[RoutingGroupConnectorSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[RoutingGroupConnectorSchema.ExchangeLegacyDN] = value;
			}
		}

		[Parameter]
		public bool PublicFolderReferralsEnabled
		{
			get
			{
				return (bool)this[RoutingGroupConnectorSchema.PublicFolderReferralsEnabled];
			}
			set
			{
				this[RoutingGroupConnectorSchema.PublicFolderReferralsEnabled] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RoutingGroupConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RoutingGroupConnector.mostDerivedClass;
			}
		}

		internal static object TargetTransportServersGetter(IPropertyBag propertyBag)
		{
			return SendConnector.VsiIdsToServerIds(propertyBag, RoutingGroupConnectorSchema.TargetTransportServerVsis, RoutingGroupConnectorSchema.TargetTransportServers);
		}

		internal static void TargetTransportServersSetter(object value, IPropertyBag propertyBag)
		{
			SendConnector.ServerIdsToVsiIds(propertyBag, value, RoutingGroupConnectorSchema.TargetTransportServerVsis, RoutingGroupConnectorSchema.TargetTransportServers);
		}

		internal static object PFReferralsEnabledGetter(IPropertyBag propertyBag)
		{
			return !(bool)propertyBag[RoutingGroupConnectorSchema.PublicFolderReferralsDisabled];
		}

		internal static void PFReferralsEnabledSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[RoutingGroupConnectorSchema.PublicFolderReferralsDisabled] = !(bool)value;
		}

		private static RoutingGroupConnectorSchema schema = ObjectSchema.GetInstance<RoutingGroupConnectorSchema>();

		private static string mostDerivedClass = "msExchRoutingGroupConnector";
	}
}
