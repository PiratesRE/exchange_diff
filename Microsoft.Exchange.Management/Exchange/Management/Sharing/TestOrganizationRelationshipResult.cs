using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Sharing
{
	[Serializable]
	public sealed class TestOrganizationRelationshipResult : ConfigurableObject
	{
		public TestOrganizationRelationshipResult() : base(new SimpleProviderPropertyBag())
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return this.propertyBag[SimpleProviderObjectSchema.Identity] as ObjectId;
			}
		}

		public TestOrganizationRelationshipResultId Id
		{
			get
			{
				return (TestOrganizationRelationshipResultId)this.propertyBag[TestOrganizationRelationshipResultSchema.Id];
			}
			set
			{
				this.propertyBag[TestOrganizationRelationshipResultSchema.Id] = value;
			}
		}

		public string Status
		{
			get
			{
				return this.propertyBag[TestOrganizationRelationshipResultSchema.Status] as string;
			}
			set
			{
				this.propertyBag[TestOrganizationRelationshipResultSchema.Status] = value;
			}
		}

		public LocalizedString Description
		{
			get
			{
				return (LocalizedString)this.propertyBag[TestOrganizationRelationshipResultSchema.Description];
			}
			set
			{
				this.propertyBag[TestOrganizationRelationshipResultSchema.Description] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TestOrganizationRelationshipResult.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<TestOrganizationRelationshipResultSchema>();
	}
}
