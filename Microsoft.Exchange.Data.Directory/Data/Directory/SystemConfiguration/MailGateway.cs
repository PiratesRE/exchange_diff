using System;
using System.Management.Automation;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MailGateway : SendConnector
	{
		[Parameter]
		public MultiValuedProperty<AddressSpace> AddressSpaces
		{
			get
			{
				return (MultiValuedProperty<AddressSpace>)this[MailGatewaySchema.AddressSpaces];
			}
			set
			{
				this[MailGatewaySchema.AddressSpaces] = value;
			}
		}

		public MultiValuedProperty<ConnectedDomain> ConnectedDomains
		{
			get
			{
				return (MultiValuedProperty<ConnectedDomain>)this[MailGatewaySchema.ConnectedDomains];
			}
		}

		public virtual bool Enabled
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotSupportedException("This property cannot be set for this connector type");
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsScopedConnector
		{
			get
			{
				return this.GetScopedConnector();
			}
			set
			{
				this[MailGatewaySchema.IsScopedConnector] = value;
			}
		}

		public bool IsSmtpConnector
		{
			get
			{
				return (bool)this[MailGatewaySchema.IsSmtpConnector];
			}
		}

		[Parameter]
		public string Comment
		{
			get
			{
				return (string)this[MailGatewaySchema.Comment];
			}
			set
			{
				this[MailGatewaySchema.Comment] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ObjectSchema.GetInstance<MailGateway.AllMailGatewayProperties>();
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "mailGateway";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "mailGateway");
			}
		}

		internal static object IsSmtpConnectorGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			return multiValuedProperty.Contains(SmtpSendConnectorConfig.MostDerivedClass);
		}

		internal bool GetScopedConnector()
		{
			if (this[MailGatewaySchema.IsScopedConnector] == null)
			{
				MultiValuedProperty<AddressSpace> addressSpaces = this.AddressSpaces;
				using (MultiValuedProperty<AddressSpace>.Enumerator enumerator = addressSpaces.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						AddressSpace addressSpace = enumerator.Current;
						return addressSpace.IsLocal;
					}
				}
				return false;
			}
			return (bool)this[MailGatewaySchema.IsScopedConnector];
		}

		public const string MostDerivedClass = "mailGateway";

		private const string LocalScopePrefix = "Local:";

		private class AllMailGatewayProperties : ADPropertyUnionSchema
		{
			public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
			{
				get
				{
					return MailGateway.AllMailGatewayProperties.mailGatewaySchema;
				}
			}

			private static ReadOnlyCollection<ADObjectSchema> mailGatewaySchema = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
			{
				ObjectSchema.GetInstance<SmtpSendConnectorConfigSchema>(),
				ObjectSchema.GetInstance<LegacyGatewayConnectorSchema>(),
				ObjectSchema.GetInstance<ForeignConnectorSchema>(),
				ObjectSchema.GetInstance<DeliveryAgentConnectorSchema>()
			});
		}
	}
}
