using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class TenantOutboundConnector : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantOutboundConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TenantOutboundConnector.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return TenantOutboundConnector.RootId;
			}
		}

		internal static object RecipientDomainsGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = null;
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[TenantOutboundConnectorSchema.RecipientDomains];
			if (multiValuedProperty2 != null)
			{
				multiValuedProperty = new MultiValuedProperty<SmtpDomainWithSubdomains>();
				foreach (string text in multiValuedProperty2)
				{
					SmtpDomainWithSubdomains smtpDomainWithSubdomains = null;
					if (!SmtpDomainWithSubdomains.TryParse(text, out smtpDomainWithSubdomains))
					{
						AddressSpace addressSpace = null;
						if (AddressSpace.TryParse(text, out addressSpace) && addressSpace.IsSmtpType)
						{
							smtpDomainWithSubdomains = addressSpace.DomainWithSubdomains;
						}
					}
					if (smtpDomainWithSubdomains != null)
					{
						multiValuedProperty.Add(smtpDomainWithSubdomains);
					}
				}
			}
			return multiValuedProperty;
		}

		internal static void RecipientDomainsSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = null;
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty2 = (MultiValuedProperty<SmtpDomainWithSubdomains>)value;
			if (multiValuedProperty2 != null)
			{
				multiValuedProperty = new MultiValuedProperty<string>();
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in multiValuedProperty2)
				{
					multiValuedProperty.Add(smtpDomainWithSubdomains.ToString());
				}
			}
			propertyBag[TenantOutboundConnectorSchema.RecipientDomains] = multiValuedProperty;
		}

		internal static object TlsAuthLevelGetter(IPropertyBag propertyBag)
		{
			return SmtpSendConnectorConfig.TlsAuthLevelGetter(propertyBag, TenantOutboundConnectorSchema.OutboundConnectorFlags);
		}

		internal static void TlsAuthLevelSetter(object value, IPropertyBag propertyBag)
		{
			SmtpSendConnectorConfig.TlsAuthLevelSetter(value, propertyBag, TenantOutboundConnectorSchema.OutboundConnectorFlags);
		}

		internal static object SmartHostsGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[TenantOutboundConnectorSchema.SmartHostsString];
			if (string.IsNullOrEmpty(text))
			{
				return new MultiValuedProperty<SmartHost>(false, TenantOutboundConnectorSchema.SmartHosts, new SmartHost[0]);
			}
			List<SmartHost> routingHostsFromString = RoutingHost.GetRoutingHostsFromString<SmartHost>(text, (RoutingHost routingHost) => new SmartHost(routingHost));
			return new MultiValuedProperty<SmartHost>(false, TenantOutboundConnectorSchema.SmartHosts, routingHostsFromString);
		}

		internal static void SmartHostsSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				propertyBag[TenantOutboundConnectorSchema.SmartHostsString] = string.Empty;
				return;
			}
			MultiValuedProperty<SmartHost> routingHostWrappers = (MultiValuedProperty<SmartHost>)value;
			string value2 = RoutingHost.ConvertRoutingHostsToString<SmartHost>(routingHostWrappers, (SmartHost host) => host.InnerRoutingHost);
			propertyBag[TenantOutboundConnectorSchema.SmartHostsString] = value2;
		}

		internal override void InitializeSchema()
		{
		}

		public TenantOutboundConnector()
		{
		}

		internal TenantOutboundConnector(IConfigurationSession session, string tenantId)
		{
			this.m_Session = session;
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal TenantOutboundConnector(string tenantId)
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.Enabled];
			}
			set
			{
				this[TenantOutboundConnectorSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseMXRecord
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.UseMxRecord];
			}
			set
			{
				this[TenantOutboundConnectorSchema.UseMxRecord] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)this[TenantOutboundConnectorSchema.Comment];
			}
			set
			{
				this[TenantOutboundConnectorSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorType ConnectorType
		{
			get
			{
				return (TenantConnectorType)this[TenantOutboundConnectorSchema.ConnectorType];
			}
			set
			{
				this[TenantOutboundConnectorSchema.ConnectorType] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorSource ConnectorSource
		{
			get
			{
				return (TenantConnectorSource)this[TenantOutboundConnectorSchema.ConnectorSourceFlags];
			}
			set
			{
				this[TenantOutboundConnectorSchema.ConnectorSourceFlags] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomainWithSubdomains> RecipientDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomainWithSubdomains>)this[TenantOutboundConnectorSchema.RecipientDomainsEx];
			}
			set
			{
				this[TenantOutboundConnectorSchema.RecipientDomainsEx] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmartHost> SmartHosts
		{
			get
			{
				return (MultiValuedProperty<SmartHost>)this[TenantOutboundConnectorSchema.SmartHosts];
			}
			set
			{
				this[TenantOutboundConnectorSchema.SmartHosts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomainWithSubdomains TlsDomain
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[TenantOutboundConnectorSchema.TlsDomain];
			}
			set
			{
				this[TenantOutboundConnectorSchema.TlsDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsAuthLevel? TlsSettings
		{
			get
			{
				return (TlsAuthLevel?)this[TenantOutboundConnectorSchema.TlsSettings];
			}
			set
			{
				this[TenantOutboundConnectorSchema.TlsSettings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsTransportRuleScoped
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.IsTransportRuleScoped];
			}
			set
			{
				this[TenantOutboundConnectorSchema.IsTransportRuleScoped] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RouteAllMessagesViaOnPremises
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.RouteAllMessagesViaOnPremises];
			}
			set
			{
				this[TenantOutboundConnectorSchema.RouteAllMessagesViaOnPremises] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.CloudServicesMailEnabled];
			}
			set
			{
				this[TenantOutboundConnectorSchema.CloudServicesMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllAcceptedDomains
		{
			get
			{
				return (bool)this[TenantOutboundConnectorSchema.AllAcceptedDomains];
			}
			set
			{
				this[TenantOutboundConnectorSchema.AllAcceptedDomains] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			if (!(this.IsTransportRuleScoped ^ (!MultiValuedPropertyBase.IsNullOrEmpty(this.RecipientDomains) || this.AllAcceptedDomains)))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorIncorrectTransportRuleScopedParameters, TenantOutboundConnectorSchema.IsTransportRuleScoped, this));
			}
			if (this.TlsDomain != null && this.TlsSettings != TlsAuthLevel.DomainValidation)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorTlsSettingsInvalidTlsDomainWithoutDomainValidation, TenantOutboundConnectorSchema.TlsSettings, this));
			}
			if (this.TlsDomain == null && this.TlsSettings == TlsAuthLevel.DomainValidation)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorTlsSettingsInvalidDomainValidationWithoutTlsDomain, TenantOutboundConnectorSchema.TlsSettings, this));
			}
			this.ValidateSmartHosts(errors);
			if (this.ConnectorType != TenantConnectorType.OnPremises && this.RouteAllMessagesViaOnPremises)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorIncorrectRouteAllMessagesViaOnPremises, TenantOutboundConnectorSchema.RouteAllMessagesViaOnPremises, this));
			}
			if (this.ConnectorType != TenantConnectorType.OnPremises && this.CloudServicesMailEnabled)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorIncorrectCloudServicesMailEnabled, TenantOutboundConnectorSchema.CloudServicesMailEnabled, this));
			}
			if (this.ConnectorType != TenantConnectorType.OnPremises && this.AllAcceptedDomains)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorIncorrectAllAcceptedDomains, TenantOutboundConnectorSchema.AllAcceptedDomains, this));
			}
		}

		private void ValidateSmartHosts(List<ValidationError> errors)
		{
			if (this.UseMXRecord && !MultiValuedPropertyBase.IsNullOrEmpty(this.SmartHosts))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorUseMXRecordShouldBeFalseIfSmartHostsIsPresent, TenantOutboundConnectorSchema.UseMxRecord, this));
			}
			if (!this.UseMXRecord && MultiValuedPropertyBase.IsNullOrEmpty(this.SmartHosts))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OutboundConnectorSmartHostShouldBePresentIfUseMXRecordFalse, TenantOutboundConnectorSchema.SmartHostsString, this));
			}
		}

		private static readonly ADObjectId RootId = new ADObjectId("CN=Transport Settings");

		private static readonly TenantOutboundConnectorSchema schema = ObjectSchema.GetInstance<TenantOutboundConnectorSchema>();

		private static string mostDerivedClass = "msExchSMTPOutboundConnector";
	}
}
