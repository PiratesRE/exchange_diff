using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MailFlowPartner : ADConfigurationObject
	{
		public int? InboundConnectorId
		{
			get
			{
				return (int?)this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.InboundConnectorId];
			}
			set
			{
				this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.InboundConnectorId] = value;
			}
		}

		public int? OutboundConnectorId
		{
			get
			{
				return (int?)this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.OutboundConnectorId];
			}
			set
			{
				this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.OutboundConnectorId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailFlowPartnerInternalMailContentType InternalMailContentType
		{
			get
			{
				return (MailFlowPartnerInternalMailContentType)this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.InternalMailContentType];
			}
			set
			{
				this.propertyBag[MailFlowPartner.MailFlowPartnerSchema.InternalMailContentType] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MailFlowPartner.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchTransportResellerSettings";
			}
		}

		internal static object InboundConnectorIdGetter(IPropertyBag propertyBag)
		{
			return MailFlowPartner.ConnectorIdGetter(propertyBag, MailFlowPartner.MailFlowPartnerSchema.InboundConnectorIdString);
		}

		internal static void InboundConnectorIdSetter(object value, IPropertyBag propertyBag)
		{
			MailFlowPartner.ConnectorIdSetter(value, propertyBag, MailFlowPartner.MailFlowPartnerSchema.InboundConnectorIdString);
		}

		internal static object OutboundConnectorIdGetter(IPropertyBag propertyBag)
		{
			return MailFlowPartner.ConnectorIdGetter(propertyBag, MailFlowPartner.MailFlowPartnerSchema.OutboundConnectorIdString);
		}

		internal static void OutboundConnectorIdSetter(object value, IPropertyBag propertyBag)
		{
			MailFlowPartner.ConnectorIdSetter(value, propertyBag, MailFlowPartner.MailFlowPartnerSchema.OutboundConnectorIdString);
		}

		private static void ConnectorIdSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition connectorIdProperty)
		{
			if (value != null)
			{
				propertyBag[connectorIdProperty] = value.ToString();
				return;
			}
			propertyBag[connectorIdProperty] = null;
		}

		private static object ConnectorIdGetter(IPropertyBag propertyBag, ADPropertyDefinition connectorIdProperty)
		{
			string text = (string)propertyBag[connectorIdProperty];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			int num;
			if (!int.TryParse(text, out num))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ConnectorIdIsNotAnInteger, connectorIdProperty, text));
			}
			return num;
		}

		public const string MostDerivedClass = "msExchTransportResellerSettings";

		private static MailFlowPartner.MailFlowPartnerSchema schema = ObjectSchema.GetInstance<MailFlowPartner.MailFlowPartnerSchema>();

		internal class MailFlowPartnerSchema : ADConfigurationObjectSchema
		{
			public static readonly ADPropertyDefinition InboundConnectorIdString = new ADPropertyDefinition("InboundConnectorIdString", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchTransportResellerSettingsInboundGatewayID", ADPropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new Int32ParsableNullableStringConstraint()
			}, null, null);

			public static readonly ADPropertyDefinition OutboundConnectorIdString = new ADPropertyDefinition("OutboundConnectorIdString", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchTransportResellerSettingsOutboundGatewayID", ADPropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new Int32ParsableNullableStringConstraint()
			}, null, null);

			public static readonly ADPropertyDefinition InboundConnectorId = new ADPropertyDefinition("InboundConnectorId", ExchangeObjectVersion.Exchange2003, typeof(int?), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				MailFlowPartner.MailFlowPartnerSchema.InboundConnectorIdString
			}, null, new GetterDelegate(MailFlowPartner.InboundConnectorIdGetter), new SetterDelegate(MailFlowPartner.InboundConnectorIdSetter), null, null);

			public static readonly ADPropertyDefinition OutboundConnectorId = new ADPropertyDefinition("OutboundConnectorId", ExchangeObjectVersion.Exchange2003, typeof(int?), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				MailFlowPartner.MailFlowPartnerSchema.OutboundConnectorIdString
			}, null, new GetterDelegate(MailFlowPartner.OutboundConnectorIdGetter), new SetterDelegate(MailFlowPartner.OutboundConnectorIdSetter), null, null);

			public static readonly ADPropertyDefinition InternalMailContentType = new ADPropertyDefinition("InternalMailContentType", ExchangeObjectVersion.Exchange2003, typeof(MailFlowPartnerInternalMailContentType), "msExchTransportResellerIntraTenantMailContentType", ADPropertyDefinitionFlags.None, MailFlowPartnerInternalMailContentType.None, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new EnumValueDefinedConstraint(typeof(MailFlowPartnerInternalMailContentType))
			}, null, null);
		}
	}
}
