using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateFolderPath : MultiStepServiceCommand<CreateFolderPathRequest, BaseFolderType>
	{
		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingRead;
			}
		}

		internal override void PreExecuteCommand()
		{
			try
			{
				this.currentParentIdAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(this.parentFolderId.BaseFolderId, true);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ParentFolderNotFoundException(innerException);
			}
		}

		public CreateFolderPath(CallContext callContext, CreateFolderPathRequest request) : base(callContext, request)
		{
			this.parentFolderId = base.Request.ParentFolderId;
			this.relativePath = base.Request.RelativeFolderPath;
			ServiceCommandBase.ThrowIfNull(this.parentFolderId, "this.parentFolderId", "CreateFolderPath::PreExecuteCommand");
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderType>(this.relativePath, "this.relativePath", "CreateFolderPath::PreExecuteCommand");
		}

		internal override ServiceResult<BaseFolderType> Execute()
		{
			BaseFolderType baseFolderType = null;
			try
			{
				if (this.currentParentIdAndSession == null)
				{
					throw new ParentFolderNotFoundException();
				}
				baseFolderType = this.CreateFolderFromRequestFolder(this.currentParentIdAndSession, this.relativePath[base.CurrentStep]);
			}
			finally
			{
				if (baseFolderType != null)
				{
					this.currentParentIdAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(baseFolderType.FolderId, true);
				}
				else
				{
					this.currentParentIdAndSession = null;
				}
			}
			this.objectsChanged++;
			return new ServiceResult<BaseFolderType>(baseFolderType);
		}

		private static StoreObjectType GetStoreObjectType(object containerClassValue)
		{
			if (containerClassValue is PropertyError)
			{
				return StoreObjectType.Folder;
			}
			return ObjectClass.GetObjectType(containerClassValue as string);
		}

		private static Folder CreateOrGetFolderBasedOnStoreObjectType(IdAndSession parentIdAndSession, StoreObjectType storeObjectType, BaseFolderType folder)
		{
			Folder result;
			if (folder.DistinguishedFolderIdSpecified)
			{
				if (!IdConverter.IsDefaultFolderCreateSupported(folder.DistinguishedFolderId))
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCreateDistinguishedFolder);
				}
				MailboxSession mailboxSession = parentIdAndSession.Session as MailboxSession;
				DefaultFolderType defaultFolderTypeFromDistinguishedFolderIdNameType = IdConverter.GetDefaultFolderTypeFromDistinguishedFolderIdNameType(folder.DistinguishedFolderId);
				StoreObjectId storeObjectId;
				if (DefaultFolderType.AdminAuditLogs == defaultFolderTypeFromDistinguishedFolderIdNameType)
				{
					storeObjectId = mailboxSession.GetAdminAuditLogsFolderId();
				}
				else
				{
					storeObjectId = mailboxSession.GetDefaultFolderId(defaultFolderTypeFromDistinguishedFolderIdNameType);
				}
				if (storeObjectId != null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorFolderExists);
				}
				StoreObjectId folderId = mailboxSession.CreateDefaultFolder(defaultFolderTypeFromDistinguishedFolderIdNameType);
				result = Folder.Bind(mailboxSession, folderId);
				folder.PropertyBag.Remove(BaseFolderSchema.DistinguishedFolderId);
				folder.PropertyBag.Remove(BaseFolderSchema.DisplayName);
			}
			else
			{
				VersionedId versionedId = null;
				using (Folder folder2 = Folder.Bind(parentIdAndSession.Session, parentIdAndSession.Id, null))
				{
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, folder.DisplayName);
					using (QueryResult queryResult = folder2.FolderQuery(FolderQueryFlags.None, queryFilter, null, new PropertyDefinition[]
					{
						FolderSchema.DisplayName,
						FolderSchema.Id,
						StoreObjectSchema.ContainerClass
					}))
					{
						for (;;)
						{
							object[][] rows = queryResult.GetRows(10);
							if (rows.Length <= 0)
							{
								break;
							}
							for (int i = 0; i < rows.Length; i++)
							{
								if (CreateFolderPath.GetStoreObjectType(rows[i][2]) == storeObjectType)
								{
									versionedId = (VersionedId)rows[i][1];
									break;
								}
							}
						}
					}
				}
				if (versionedId != null)
				{
					result = Folder.Bind(parentIdAndSession.Session, versionedId.ObjectId);
				}
				else
				{
					result = Folder.Create(parentIdAndSession.Session, parentIdAndSession.Id, storeObjectType);
				}
			}
			return result;
		}

		private BaseFolderType CreateFolderFromRequestFolder(IdAndSession parentIdAndSession, BaseFolderType folder)
		{
			BaseFolderType result = null;
			StoreObjectType storeObjectType = folder.StoreObjectType;
			this.ValidateCreate(storeObjectType, folder);
			using (Folder folder2 = CreateFolderPath.CreateOrGetFolderBasedOnStoreObjectType(parentIdAndSession, storeObjectType, folder))
			{
				result = this.UpdateNewFolder(parentIdAndSession, folder2, folder);
			}
			return result;
		}

		private void ValidateCreate(StoreObjectType storeObjectType, BaseFolderType folder)
		{
			if (storeObjectType != StoreObjectType.Folder)
			{
				this.ConfirmNoFolderClassOverride(folder);
			}
		}

		private void ConfirmNoFolderClassOverride(BaseFolderType folder)
		{
			if (!string.IsNullOrEmpty(folder.FolderClass))
			{
				throw new NoFolderClassOverrideException();
			}
		}

		internal override int StepCount
		{
			get
			{
				return this.relativePath.Length;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CreateFolderPathResponse createFolderPathResponse = new CreateFolderPathResponse();
			createFolderPathResponse.BuildForResults<BaseFolderType>(base.Results);
			return createFolderPathResponse;
		}

		private BaseFolderType UpdateNewFolder(IdAndSession parentIdAndSession, Folder xsoFolder, BaseFolderType folder)
		{
			base.SetProperties(xsoFolder, folder);
			this.SaveXsoFolder(xsoFolder);
			folder.Clear();
			base.LoadServiceObject(folder, xsoFolder, parentIdAndSession, this.responseShape);
			return folder;
		}

		private TargetFolderId parentFolderId;

		private BaseFolderType[] relativePath;

		private IdAndSession currentParentIdAndSession;

		private ResponseShape responseShape = new ResponseShape(ShapeEnum.Default, null);
	}
}
