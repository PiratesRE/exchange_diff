using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ClutterCountProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IToXmlForPropertyBagCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public ClutterCountProperty(CommandContext commandContext) : base(commandContext)
		{
			this.linkedSearchFolderIdProperty = this.propertyDefinitions[0];
			this.folderCountProperty = this.propertyDefinitions[1];
		}

		public static ClutterCountProperty CreateCommand(CommandContext commandContext)
		{
			return new ClutterCountProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ClutterCountProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("ClutterCountProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			byte[] array = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(commandSettings.PropertyBag, this.linkedSearchFolderIdProperty, out array) && array != null)
			{
				int num = -1;
				if (this.TryGetFolderCount(commandSettings.IdAndSession, array, out num))
				{
					serviceObject.PropertyBag[this.commandContext.PropertyInformation] = num;
				}
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, this.linkedSearchFolderIdProperty))
			{
				byte[] array = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.linkedSearchFolderIdProperty) as byte[];
				if (array != null)
				{
					int num = -1;
					if (this.TryGetFolderCount(commandSettings.IdAndSession, array, out num))
					{
						ServiceObject serviceObject = commandSettings.ServiceObject;
						serviceObject.PropertyBag[this.commandContext.PropertyInformation] = num;
					}
				}
			}
		}

		private bool TryGetFolderCount(IdAndSession idAndSession, byte[] entryId, out int folderCount)
		{
			bool result = false;
			folderCount = 0;
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession != null)
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
				using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId))
				{
					List<object[]> list = new List<object[]>();
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.EntryId, entryId);
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, queryFilter, null, new PropertyDefinition[]
					{
						this.folderCountProperty
					}))
					{
						list = SearchUtil.FetchRowsFromQueryResult(queryResult, 10000);
						if (list.Count > 0)
						{
							folderCount = (int)list[0][0];
						}
						result = true;
					}
				}
			}
			return result;
		}

		private readonly PropertyDefinition linkedSearchFolderIdProperty;

		private readonly PropertyDefinition folderCountProperty;
	}
}
