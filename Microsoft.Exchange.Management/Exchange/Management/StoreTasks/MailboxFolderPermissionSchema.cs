using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal sealed class MailboxFolderPermissionSchema : ObjectSchema
	{
		public static readonly SimplePropertyDefinition ExchangeVersion = new SimplePropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.None, ExchangeObjectVersion.Exchange2003, ExchangeObjectVersion.Exchange2003, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition ObjectState = new SimplePropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2003, typeof(ObjectState), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.ObjectState.New, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition FolderName = new SimplePropertyDefinition("FolderName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition User = new SimplePropertyDefinition("User", ExchangeObjectVersion.Exchange2010, typeof(MailboxFolderUserId), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition AccessRights = new SimplePropertyDefinition("AccessRights", ExchangeObjectVersion.Exchange2010, typeof(Collection<MailboxFolderAccessRight>), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition Identity = new SimplePropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(ObjectId), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
