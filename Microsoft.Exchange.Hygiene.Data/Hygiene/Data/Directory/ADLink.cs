using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal class ADLink : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		internal ADObjectId SourceId
		{
			get
			{
				return this[ADLinkSchema.SourceIdProperty] as ADObjectId;
			}
			set
			{
				this[ADLinkSchema.SourceIdProperty] = value;
			}
		}

		internal ADObjectId DestinationId
		{
			get
			{
				return this[ADLinkSchema.DestinationIdProperty] as ADObjectId;
			}
			set
			{
				this[ADLinkSchema.DestinationIdProperty] = value;
			}
		}

		internal DirectoryObjectClass SourceType
		{
			get
			{
				return (DirectoryObjectClass)this[ADLinkSchema.SourceTypeProperty];
			}
			set
			{
				this[ADLinkSchema.SourceTypeProperty] = value;
			}
		}

		internal DirectoryObjectClass DestinationType
		{
			get
			{
				return (DirectoryObjectClass)this[ADLinkSchema.DestinationTypeProperty];
			}
			set
			{
				this[ADLinkSchema.DestinationTypeProperty] = value;
			}
		}

		internal LinkType LinkType
		{
			get
			{
				return (LinkType)this[ADLinkSchema.LinkTypeProperty];
			}
			set
			{
				this[ADLinkSchema.LinkTypeProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADLink.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADLink.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ADLinkSchema schema = ObjectSchema.GetInstance<ADLinkSchema>();

		private static string mostDerivedClass = "ADLink";
	}
}
