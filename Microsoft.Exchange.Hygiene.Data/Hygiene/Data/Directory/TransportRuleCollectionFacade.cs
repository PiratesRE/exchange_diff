using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Transport;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TransportRuleCollectionFacade : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return null;
			}
		}

		public byte[] FileData
		{
			get
			{
				return (byte[])this[TransportRuleCollectionFacadeSchema.FileData];
			}
			set
			{
				this[TransportRuleCollectionFacadeSchema.FileData] = value;
			}
		}

		public MigrationSourceType MigrationSource
		{
			get
			{
				return (MigrationSourceType)this[TransportRuleCollectionFacadeSchema.MigrationSource];
			}
			set
			{
				this[TransportRuleCollectionFacadeSchema.MigrationSource] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TransportRuleCollectionFacade.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TransportRuleCollectionFacade.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TransportRuleCollectionFacadeSchema schema = ObjectSchema.GetInstance<TransportRuleCollectionFacadeSchema>();

		private static string mostDerivedClass = "TransportRuleCollectionFacade";
	}
}
