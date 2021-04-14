using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactFolderProvider : ExchangeServiceProvider
	{
		public ContactFolderProvider(IExchangeService exchangeService) : base(exchangeService)
		{
		}

		public static ContactFolder FolderTypeToEntity(ContactsFolderType itemType, IList<PropertyDefinition> properties)
		{
			ArgumentValidator.ThrowIfNull("itemType", itemType);
			ArgumentValidator.ThrowIfNull("properties", properties);
			ContactFolder contactFolder = EwsServiceObjectFactory.CreateEntity<ContactFolder>(itemType);
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				propertyDefinition.EwsPropertyProvider.GetPropertyFromDataSource(contactFolder, propertyDefinition, itemType);
			}
			return contactFolder;
		}

		public ContactFolder Read(string id, ContactFolderQueryAdapter queryAdapter = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("id", id);
			string ewsId = EwsIdConverter.ODataIdToEwsId(id);
			return this.InternalRead(ewsId, queryAdapter);
		}

		public IFindEntitiesResult<ContactFolder> Find(string parentFolderId, ContactFolderQueryAdapter queryAdapter = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("parentFolderId", parentFolderId);
			string id = EwsIdConverter.ODataIdToEwsId(parentFolderId);
			BaseFolderId baseFolderId = EwsIdConverter.CreateFolderIdFromEwsId(id);
			queryAdapter = (queryAdapter ?? ContactFolderQueryAdapter.Default);
			FindFolderRequest findFolderRequest = new FindFolderRequest();
			findFolderRequest.ParentFolderIds = new BaseFolderId[]
			{
				baseFolderId
			};
			findFolderRequest.Traversal = FolderQueryTraversal.Shallow;
			findFolderRequest.Paging = queryAdapter.GetPaging();
			findFolderRequest.Restriction = queryAdapter.GetRestriction();
			findFolderRequest.FolderShape = queryAdapter.GetResponseShape(false);
			FindFolderResponse findFolderResponse = base.ExchangeService.FindFolder(findFolderRequest, null);
			FindFolderResponseMessage findFolderResponseMessage = findFolderResponse.ResponseMessages.Items[0] as FindFolderResponseMessage;
			List<ContactFolder> list = new List<ContactFolder>();
			for (int i = 0; i < findFolderResponseMessage.RootFolder.Folders.Length; i++)
			{
				ContactsFolderType contactsFolderType = findFolderResponseMessage.RootFolder.Folders[i] as ContactsFolderType;
				if (contactsFolderType != null)
				{
					ContactFolder item = ContactFolderProvider.FolderTypeToEntity(contactsFolderType, queryAdapter.RequestedProperties);
					list.Add(item);
				}
			}
			return new FindEntitiesResult<ContactFolder>(list, findFolderResponseMessage.RootFolder.TotalItemsInView);
		}

		private ContactFolder InternalRead(string ewsId, ContactFolderQueryAdapter queryAdapter = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ewsId", ewsId);
			BaseFolderId baseFolderId = EwsIdConverter.CreateFolderIdFromEwsId(ewsId);
			queryAdapter = (queryAdapter ?? ContactFolderQueryAdapter.Default);
			GetFolderRequest request = new GetFolderRequest
			{
				Ids = new BaseFolderId[]
				{
					baseFolderId
				},
				FolderShape = queryAdapter.GetResponseShape(false)
			};
			GetFolderResponse folder = base.ExchangeService.GetFolder(request, null);
			FolderInfoResponseMessage folderInfoResponseMessage = folder.ResponseMessages.Items[0] as FolderInfoResponseMessage;
			ContactsFolderType contactsFolderType = folderInfoResponseMessage.Folders[0] as ContactsFolderType;
			if (contactsFolderType == null)
			{
				throw new InvalidIdException();
			}
			return ContactFolderProvider.FolderTypeToEntity(contactsFolderType, queryAdapter.RequestedProperties);
		}
	}
}
