using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Serializable]
	public class ImportContactListResult : ConfigurableObject
	{
		public ImportContactListResult(ObjectId mbxIdentity) : base(new SimpleProviderPropertyBag())
		{
			SyncUtilities.ThrowIfArgumentNull("mbxIdentity", mbxIdentity);
			this[SimpleProviderObjectSchema.Identity] = mbxIdentity;
			base.ResetChangeTracking();
		}

		public int ContactsImported
		{
			get
			{
				return (int)this[ImportContactListResultSchema.ContactsImported];
			}
			internal set
			{
				this[ImportContactListResultSchema.ContactsImported] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ImportContactListResult.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ImportContactListResultSchema schema = ObjectSchema.GetInstance<ImportContactListResultSchema>();
	}
}
