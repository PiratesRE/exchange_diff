using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ArchiveItem : MultiStepServiceCommand<ArchiveItemRequest, Microsoft.Exchange.Services.Core.Types.ItemType>
	{
		public ArchiveItem(CallContext callContext, ArchiveItemRequest request) : base(callContext, request)
		{
			this.objectIds = request.Ids;
			this.sourceFolderId = request.SourceFolderId.BaseFolderId;
		}

		internal override int StepCount
		{
			get
			{
				return this.objectIds.Length;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ArchiveItemResponse archiveItemResponse = new ArchiveItemResponse();
			archiveItemResponse.BuildForResults<Microsoft.Exchange.Services.Core.Types.ItemType>(base.Results);
			return archiveItemResponse;
		}

		internal override void PreExecuteCommand()
		{
			this.ValidateItems();
			this.destinationFolderId = this.GetArchiveDestinationFolder();
			ServiceCommandBase serviceCommand = new MoveItemRequest
			{
				ToFolderId = new TargetFolderId(this.destinationFolderId),
				Ids = this.objectIds,
				ReturnNewItemIds = true
			}.GetServiceCommand(base.CallContext);
			if (serviceCommand is MoveItem)
			{
				this.moveItemCommand = (serviceCommand as MoveItem);
				this.moveItemCommand.PreExecuteCommand();
				return;
			}
			this.moveItemBatchCommand = (serviceCommand as MoveItemBatch);
			this.moveItemBatchCommand.PreExecuteCommand();
		}

		internal override ServiceResult<Microsoft.Exchange.Services.Core.Types.ItemType> Execute()
		{
			if (this.moveItemCommand != null)
			{
				this.moveItemCommand.CurrentStep = base.CurrentStep;
				return this.moveItemCommand.Execute();
			}
			this.moveItemBatchCommand.CurrentStep = base.CurrentStep;
			return this.moveItemBatchCommand.Execute();
		}

		internal override void PostExecuteCommand()
		{
			if (this.moveItemCommand != null)
			{
				this.moveItemCommand.PostExecuteCommand();
				return;
			}
			this.moveItemBatchCommand.PostExecuteCommand();
		}

		private static DistinguishedFolderId MapDefaultFolderTypeToDistinguishedFolderId(DefaultFolderType defaultFolderType)
		{
			ArchiveItem.ValidateDefaultFolderType(defaultFolderType);
			DistinguishedFolderId distinguishedFolderId = new DistinguishedFolderId();
			if (defaultFolderType == DefaultFolderType.Inbox)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.inbox;
				return distinguishedFolderId;
			}
			if (defaultFolderType == DefaultFolderType.Root)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.root;
				return distinguishedFolderId;
			}
			if (defaultFolderType == DefaultFolderType.Drafts)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.drafts;
				return distinguishedFolderId;
			}
			if (defaultFolderType == DefaultFolderType.DeletedItems)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.deleteditems;
				return distinguishedFolderId;
			}
			if (defaultFolderType == DefaultFolderType.SentItems)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.sentitems;
				return distinguishedFolderId;
			}
			if (defaultFolderType == DefaultFolderType.CommunicatorHistory)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.conversationhistory;
			}
			return null;
		}

		private static void ValidateDefaultFolderType(DefaultFolderType defaultFolderType)
		{
			if (defaultFolderType != DefaultFolderType.None && defaultFolderType != DefaultFolderType.Inbox && defaultFolderType != DefaultFolderType.Root && defaultFolderType != DefaultFolderType.Drafts && defaultFolderType != DefaultFolderType.DeletedItems && defaultFolderType != DefaultFolderType.SentItems && defaultFolderType != DefaultFolderType.CommunicatorHistory)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2786380669U);
			}
		}

		private static bool IsFolderDistinguished(IdConverter idConverter, BaseFolderId folderId, out DistinguishedFolderId archiveFolder)
		{
			archiveFolder = null;
			IdAndSession idAndSession = idConverter.ConvertFolderIdToIdAndSession(folderId, IdConverter.ConvertOption.IgnoreChangeKey);
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(idAndSession.Id);
			if (defaultFolderType == DefaultFolderType.Root)
			{
				archiveFolder = new DistinguishedFolderId();
				archiveFolder.Id = DistinguishedFolderIdName.archivemsgfolderroot;
				return true;
			}
			if (defaultFolderType == DefaultFolderType.DeletedItems)
			{
				archiveFolder = new DistinguishedFolderId();
				archiveFolder.Id = DistinguishedFolderIdName.deleteditems;
				return true;
			}
			ArchiveItem.ValidateDefaultFolderType(defaultFolderType);
			return false;
		}

		private static DistinguishedFolderId GetArchiveDistinguishedFolder(DistinguishedFolderId primaryFolderId)
		{
			DistinguishedFolderId distinguishedFolderId = new DistinguishedFolderId();
			if (primaryFolderId.Id == DistinguishedFolderIdName.root)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.archivemsgfolderroot;
				return distinguishedFolderId;
			}
			if (primaryFolderId.Id == DistinguishedFolderIdName.deleteditems)
			{
				distinguishedFolderId.Id = DistinguishedFolderIdName.archivedeleteditems;
				return distinguishedFolderId;
			}
			return null;
		}

		private static StoreObjectType GetFolderStoreObjectType(object containerClassValue)
		{
			if (containerClassValue is PropertyError)
			{
				return StoreObjectType.Folder;
			}
			return ObjectClass.GetObjectType(containerClassValue as string);
		}

		private CreateFolderPathRequest GetFolderPathRequest()
		{
			List<Microsoft.Exchange.Services.Core.Types.BaseFolderType> list = new List<Microsoft.Exchange.Services.Core.Types.BaseFolderType>();
			BaseFolderId baseFolderId = this.sourceFolderId;
			for (;;)
			{
				IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(baseFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
				MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
				using (Folder folder = Folder.Bind(mailboxSession, idAndSession.Id, new PropertyDefinition[]
				{
					StoreObjectSchema.ParentItemId,
					FolderSchema.DisplayName,
					StoreObjectSchema.ContainerClass
				}))
				{
					object obj = folder.TryGetProperty(StoreObjectSchema.ParentItemId);
					StoreId storeId = obj as StoreId;
					if (storeId == null)
					{
						throw new InvalidFolderIdException((CoreResources.IDs)2940401781U);
					}
					object containerClassValue = folder.TryGetProperty(StoreObjectSchema.ContainerClass);
					StoreObjectType folderStoreObjectType = ArchiveItem.GetFolderStoreObjectType(containerClassValue);
					if (folderStoreObjectType == StoreObjectType.SearchFolder || folderStoreObjectType == StoreObjectType.OutlookSearchFolder)
					{
						throw new InvalidFolderTypeForOperationException((CoreResources.IDs)3848937923U);
					}
					Microsoft.Exchange.Services.Core.Types.BaseFolderType baseFolderType = Microsoft.Exchange.Services.Core.Types.BaseFolderType.CreateFromStoreObjectType(folderStoreObjectType);
					baseFolderType.DisplayName = folder.DisplayName;
					list.Insert(0, baseFolderType);
					DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(storeId);
					BaseFolderId baseFolderId2;
					if (defaultFolderType == DefaultFolderType.None)
					{
						baseFolderId2 = IdConverter.GetFolderIdFromStoreId(storeId, new MailboxId(mailboxSession));
					}
					else
					{
						baseFolderId2 = ArchiveItem.MapDefaultFolderTypeToDistinguishedFolderId(defaultFolderType);
					}
					baseFolderId = baseFolderId2;
					if (defaultFolderType != DefaultFolderType.Root && defaultFolderType != DefaultFolderType.DeletedItems)
					{
						continue;
					}
				}
				break;
			}
			return new CreateFolderPathRequest
			{
				ParentFolderId = new TargetFolderId(),
				ParentFolderId = 
				{
					BaseFolderId = ArchiveItem.GetArchiveDistinguishedFolder(baseFolderId as DistinguishedFolderId)
				},
				RelativeFolderPath = list.ToArray()
			};
		}

		private BaseFolderId GetLocalArchiveDestinationFolder(CreateFolderPathRequest request)
		{
			CreateFolderPath createFolderPath = new CreateFolderPath(base.CallContext, request);
			List<Microsoft.Exchange.Services.Core.Types.BaseFolderType> list = new List<Microsoft.Exchange.Services.Core.Types.BaseFolderType>();
			createFolderPath.PreExecuteCommand();
			for (int i = 0; i < createFolderPath.StepCount; i++)
			{
				ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> serviceResult = createFolderPath.Execute();
				if (serviceResult.Error != null)
				{
					throw new ArchiveItemException((CoreResources.IDs)2565659540U);
				}
				list.Add(serviceResult.Value);
				createFolderPath.CurrentStep++;
			}
			createFolderPath.PostExecuteCommand();
			return list.Last<Microsoft.Exchange.Services.Core.Types.BaseFolderType>().FolderId;
		}

		private void ValidateItems()
		{
			IdConverter idConverter = new IdConverter(base.CallContext);
			Guid? guid = null;
			foreach (BaseItemId itemId in this.objectIds)
			{
				StoreId storeId;
				Guid value;
				bool flag = idConverter.TryGetStoreIdAndMailboxGuidFromItemId(itemId, out storeId, out value);
				if (guid == null)
				{
					guid = new Guid?(value);
				}
				if (!flag || !value.Equals(guid.Value))
				{
					throw new InvalidItemForOperationException(typeof(ArchiveItem).Name);
				}
			}
			if (!idConverter.GetMailboxGuidFromFolderId(this.sourceFolderId).Equals(guid.Value))
			{
				throw new InvalidItemForOperationException(typeof(ArchiveItem).Name);
			}
			IdAndSession idAndSession = idConverter.ConvertFolderIdToIdAndSession(this.sourceFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession.MailboxOwner.MailboxInfo.IsArchive)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2225772284U);
			}
		}

		private BaseFolderId GetRemoteArchiveDestinationFolder(IMailboxInfo archiveMailbox, CreateFolderPathRequest request)
		{
			string text = EwsClientHelper.DiscoverEwsUrl(archiveMailbox);
			if (string.IsNullOrEmpty(text))
			{
				throw new ArchiveItemException((CoreResources.IDs)3156121664U);
			}
			ExchangeServiceBinding serviceBinding = EwsClientHelper.CreateBinding(base.CallContext.EffectiveCaller, text);
			CreateFolderPathType ewsClientRequest = EwsClientHelper.Convert<CreateFolderPathRequest, CreateFolderPathType>(request);
			Exception ex = null;
			CreateFolderPathResponseType ewsClientCreateFolderPathResponse = null;
			bool flag = EwsClientHelper.ExecuteEwsCall(delegate
			{
				ewsClientCreateFolderPathResponse = serviceBinding.CreateFolderPath(ewsClientRequest);
			}, out ex);
			if (!flag)
			{
				throw new ArchiveItemException((CoreResources.IDs)2565659540U);
			}
			CreateFolderPathResponse createFolderPathResponse = EwsClientHelper.Convert<CreateFolderPathResponseType, CreateFolderPathResponse>(ewsClientCreateFolderPathResponse);
			ResponseMessage[] items = createFolderPathResponse.ResponseMessages.Items;
			ResponseMessage responseMessage = items.Last<ResponseMessage>();
			if (responseMessage.ResponseClass == ResponseClass.Success)
			{
				return ((FolderInfoResponseMessage)responseMessage).Folders[0].FolderId;
			}
			throw new ArchiveItemException((CoreResources.IDs)2565659540U);
		}

		private BaseFolderId GetArchiveDestinationFolder()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.sourceFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3558192788U);
			}
			DistinguishedFolderId result = null;
			if (ArchiveItem.IsFolderDistinguished(base.IdConverter, this.sourceFolderId, out result))
			{
				return result;
			}
			CreateFolderPathRequest folderPathRequest = this.GetFolderPathRequest();
			IMailboxInfo archiveMailbox = mailboxSession.MailboxOwner.GetArchiveMailbox();
			switch ((archiveMailbox != null) ? archiveMailbox.ArchiveState : ArchiveState.None)
			{
			case ArchiveState.None:
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorArchiveMailboxNotEnabled);
			case ArchiveState.Local:
				if (archiveMailbox != null && mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn != archiveMailbox.Location.ServerFqdn)
				{
					return this.GetRemoteArchiveDestinationFolder(archiveMailbox, folderPathRequest);
				}
				return this.GetLocalArchiveDestinationFolder(folderPathRequest);
			default:
				throw new NotSupportedException();
			}
		}

		private BaseItemId[] objectIds;

		private BaseFolderId sourceFolderId;

		private BaseFolderId destinationFolderId;

		private MoveItemBatch moveItemBatchCommand;

		private MoveItem moveItemCommand;
	}
}
