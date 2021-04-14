using System;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache;

namespace Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV160
{
	internal class RecipientInfoCacheSchemaState : AirSyncRecipientInfoCacheSchemaState
	{
		public RecipientInfoCacheSchemaState()
		{
			base.InitConversionTable(2);
			this.CreatePropertyConversionTable();
		}

		private void CreatePropertyConversionTable()
		{
			string xmlNodeNamespace = "Contacts:";
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Email1Address", false),
				new RecipientInfoCacheStringProperty(RecipientInfoCacheEntryElements.EmailAddress)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "FileAs", false),
				new RecipientInfoCacheStringProperty(RecipientInfoCacheEntryElements.DisplayName)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Alias", false),
				new RecipientInfoCacheStringProperty(RecipientInfoCacheEntryElements.Alias)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "WeightedRank", false),
				new RecipientInfoCacheIntProperty(RecipientInfoCacheEntryElements.WeightedRank)
			});
		}
	}
}
