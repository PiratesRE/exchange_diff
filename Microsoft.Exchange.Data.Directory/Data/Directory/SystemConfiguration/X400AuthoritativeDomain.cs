using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class X400AuthoritativeDomain : ADConfigurationObject
	{
		public X400AuthoritativeDomain()
		{
			this[AcceptedDomainSchema.X400AddressType] = true;
		}

		[Parameter]
		public X400Domain X400DomainName
		{
			get
			{
				return (X400Domain)this[X400AuthoritativeDomainSchema.DomainName];
			}
			set
			{
				this[X400AuthoritativeDomainSchema.DomainName] = value;
			}
		}

		[Parameter]
		public bool X400ExternalRelay
		{
			get
			{
				return 1 == (int)this[AcceptedDomainSchema.AcceptedDomainType];
			}
			set
			{
				this[AcceptedDomainSchema.AcceptedDomainType] = (value ? 1 : 0);
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return X400AuthoritativeDomain.SchemaObject;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return AcceptedDomain.AcceptedDomainContainer;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchAcceptedDomain";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					base.ImplicitFilter,
					X400AuthoritativeDomain.X400DomainsFilter
				});
			}
		}

		private static readonly X400AuthoritativeDomainSchema SchemaObject = ObjectSchema.GetInstance<X400AuthoritativeDomainSchema>();

		private static readonly QueryFilter AuthoritativeFilter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.AcceptedDomainFlags, 16);

		private static readonly QueryFilter RelayFilter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.AcceptedDomainFlags, 17);

		private static readonly QueryFilter X400DomainsFilter = new OrFilter(new QueryFilter[]
		{
			X400AuthoritativeDomain.AuthoritativeFilter,
			X400AuthoritativeDomain.RelayFilter
		});

		internal static readonly QueryFilter NonX400DomainsFilter = new NotFilter(X400AuthoritativeDomain.X400DomainsFilter);
	}
}
