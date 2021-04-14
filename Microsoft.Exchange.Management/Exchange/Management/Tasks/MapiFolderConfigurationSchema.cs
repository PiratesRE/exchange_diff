using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MapiFolderConfigurationSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(MailboxFolderId), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectState = new SimpleProviderPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2003, typeof(ObjectState), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeVersion = new SimpleProviderPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.None, ExchangeObjectVersion.Exchange2010, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Date = new SimpleProviderPropertyDefinition("Date", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderPath = new SimpleProviderPropertyDefinition("FolderPath", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderId = new SimpleProviderPropertyDefinition("FolderId", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderType = new SimpleProviderPropertyDefinition("FolderType", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ItemsInFolder = new SimpleProviderPropertyDefinition("ItemsInFolder", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeletedItemsInFolder = new SimpleProviderPropertyDefinition("DeleteItemsInFolder", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderSize = new SimpleProviderPropertyDefinition("FolderSize", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ItemsInFolderAndSubfolders = new SimpleProviderPropertyDefinition("ItemsInFolderAndSubfolders", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeletedItemsInFolderAndSubfolders = new SimpleProviderPropertyDefinition("DeletedItemsInFolderAndSubfolders", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderAndSubfolderSize = new SimpleProviderPropertyDefinition("FolderAndSubfolderSize", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OldestItemReceivedDate = new SimpleProviderPropertyDefinition("OldestItemReceivedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NewestItemReceivedDate = new SimpleProviderPropertyDefinition("NewestItemReceivedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OldestDeletedItemReceivedDate = new SimpleProviderPropertyDefinition("OldestDeletedItemReceivedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NewestDeletedItemReceivedDate = new SimpleProviderPropertyDefinition("NewestDeletedItemReceivedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OldestItemLastModifiedDate = new SimpleProviderPropertyDefinition("OldestItemLastModifiedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NewestItemLastModifiedDate = new SimpleProviderPropertyDefinition("NewestItemLastModifiedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OldestDeletedItemLastModifiedDate = new SimpleProviderPropertyDefinition("OldestDeletedItemLastModifiedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NewestDeletedItemLastModifiedDate = new SimpleProviderPropertyDefinition("NewestDeletedItemLastModifiedDate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ManagedFolder = new SimpleProviderPropertyDefinition("ManagedFolder", ExchangeObjectVersion.Exchange2003, typeof(ELCFolderIdParameter), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeletePolicy = new SimpleProviderPropertyDefinition("DeletePolicy", ExchangeObjectVersion.Exchange2003, typeof(RetentionPolicyTagIdParameter), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ArchivePolicy = new SimpleProviderPropertyDefinition("ArchivePolicy", ExchangeObjectVersion.Exchange2003, typeof(RetentionPolicyTagIdParameter), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubject = new SimpleProviderPropertyDefinition("TopSubject", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectSize = new SimpleProviderPropertyDefinition("TopSubjectSize", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectCount = new SimpleProviderPropertyDefinition("TopSubjectCount", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectClass = new SimpleProviderPropertyDefinition("TopSubjectClass", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectPath = new SimpleProviderPropertyDefinition("TopSubjectPath", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectReceivedTime = new SimpleProviderPropertyDefinition("TopSubjectReceivedTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopSubjectFrom = new SimpleProviderPropertyDefinition("TopSubjectFrom", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopClientInfoForSubject = new SimpleProviderPropertyDefinition("TopClientInfoForSubject", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TopClientInfoCountForSubject = new SimpleProviderPropertyDefinition("TopClientInfoCountForSubject", ExchangeObjectVersion.Exchange2003, typeof(int?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SearchFolders = new SimpleProviderPropertyDefinition("SearchFolders", ExchangeObjectVersion.Exchange2003, typeof(string[]), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
