using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class FederationInformation : ConfigurableObject
	{
		[Parameter]
		public Uri TargetApplicationUri
		{
			get
			{
				return (Uri)this.propertyBag[FederationInformationSchema.TargetApplicationUri];
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpDomain> DomainNames
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this.propertyBag[FederationInformationSchema.DomainNames];
			}
		}

		[Parameter]
		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return (Uri)this.propertyBag[FederationInformationSchema.TargetAutodiscoverEpr];
			}
		}

		[Parameter]
		public MultiValuedProperty<Uri> TokenIssuerUris
		{
			get
			{
				return (MultiValuedProperty<Uri>)this.propertyBag[FederationInformationSchema.TokenIssuerUris];
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal FederationInformation(SmtpDomain identity, Uri targetApplicationUri, ICollection tokenIssuers, ICollection domainNames, Uri targetAutodiscoverEpr) : base(new FederationInformationPropertyBag())
		{
			this.propertyBag[FederationInformationSchema.Identity] = identity;
			this.propertyBag[FederationInformationSchema.TargetApplicationUri] = targetApplicationUri;
			this.propertyBag[FederationInformationSchema.DomainNames] = new MultiValuedProperty<SmtpDomain>(true, null, domainNames);
			this.propertyBag[FederationInformationSchema.TargetAutodiscoverEpr] = targetAutodiscoverEpr;
			if (tokenIssuers != null)
			{
				this.propertyBag[FederationInformationSchema.TokenIssuerUris] = new MultiValuedProperty<Uri>(true, null, tokenIssuers);
			}
			base.ResetChangeTracking(true);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return FederationInformation.schema;
			}
		}

		private static ObjectSchema schema = new FederationInformationSchema();
	}
}
