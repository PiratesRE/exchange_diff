using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class FolderManagementHelper
	{
		public FolderManagementHelper(OwaContext owaContext)
		{
			this.owaContext = owaContext;
		}

		public bool Move(StoreObjectId sourceId, StoreObjectId destinationId)
		{
			OperationResult operationResult = OperationResult.Succeeded;
			string arg = string.Empty;
			AggregateOperationResult aggregateOperationResult;
			try
			{
				using (Folder folder = Folder.Bind(this.owaContext.UserContext.MailboxSession, sourceId, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				}))
				{
					arg = folder.DisplayName;
				}
				aggregateOperationResult = this.owaContext.UserContext.MailboxSession.Move(destinationId, new StoreId[]
				{
					sourceId
				});
				operationResult = aggregateOperationResult.OperationResult;
			}
			catch (ObjectNotFoundException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2106120183, InfobarMessageType.Error);
				return false;
			}
			catch (InvalidOperationException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(442250020, InfobarMessageType.Error);
				return false;
			}
			catch (StoragePermanentException e)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e, this.owaContext);
				return false;
			}
			catch (StorageTransientException e2)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e2, this.owaContext);
				return false;
			}
			if (operationResult == OperationResult.Failed)
			{
				int i = 0;
				while (i < aggregateOperationResult.GroupOperationResults.Length)
				{
					GroupOperationResult groupOperationResult = aggregateOperationResult.GroupOperationResults[i];
					if (groupOperationResult.OperationResult != OperationResult.Succeeded)
					{
						if (groupOperationResult.Exception is CannotMoveDefaultFolderException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(442250020, InfobarMessageType.Error);
							return false;
						}
						if (groupOperationResult.Exception is ObjectExistedException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(896702607, InfobarMessageType.Error);
							return false;
						}
						InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(aggregateOperationResult.GroupOperationResults[i].Exception, this.owaContext);
						return false;
					}
					else
					{
						i++;
					}
				}
			}
			else if (operationResult == OperationResult.PartiallySucceeded)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-1093060331, InfobarMessageType.Error);
				return false;
			}
			this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(2053728082), arg), InfobarMessageType.Informational);
			return true;
		}

		public bool Create(StoreObjectId destinationId, string folderClass, string folderName)
		{
			if (destinationId == null)
			{
				throw new ArgumentNullException("destinationId");
			}
			if (folderName == null)
			{
				throw new ArgumentNullException("folderName");
			}
			folderName = folderName.Trim();
			if (folderName.Length == 0)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-41080803, InfobarMessageType.Error);
				return false;
			}
			if (folderName.Length > 256)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-1198351885, InfobarMessageType.Error);
				return false;
			}
			StoreObjectType objectType = ObjectClass.GetObjectType(folderClass);
			try
			{
				using (Folder folder = Folder.Create(this.owaContext.UserContext.MailboxSession, destinationId, objectType, folderName, CreateMode.CreateNew))
				{
					folder.ClassName = folderClass;
					folder.Save();
				}
			}
			catch (ObjectExistedException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-1399945920, InfobarMessageType.Error);
				return false;
			}
			catch (StoragePermanentException e)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e, this.owaContext);
				return false;
			}
			catch (StorageTransientException e2)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e2, this.owaContext);
				return false;
			}
			this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(-22521321), folderName), InfobarMessageType.Informational);
			return true;
		}

		public bool Rename(StoreObjectId destinationId, string folderName)
		{
			if (destinationId == null)
			{
				throw new ArgumentNullException("destinationId");
			}
			if (folderName == null)
			{
				throw new ArgumentNullException("folderName");
			}
			folderName = folderName.Trim();
			if (folderName.Length == 0)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(615663605, InfobarMessageType.Error);
				return false;
			}
			if (folderName.Length > 256)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-1198351885, InfobarMessageType.Error);
				return false;
			}
			StoreObjectType objectType = destinationId.ObjectType;
			FolderSaveResult folderSaveResult;
			try
			{
				Folder folder2;
				Folder folder = folder2 = Folder.Bind(this.owaContext.UserContext.MailboxSession, destinationId);
				try
				{
					folder.DisplayName = folderName;
					folderSaveResult = folder.Save();
				}
				finally
				{
					if (folder2 != null)
					{
						((IDisposable)folder2).Dispose();
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2106120183, InfobarMessageType.Error);
				return false;
			}
			catch (ObjectValidationException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2103130667, InfobarMessageType.Error);
				return false;
			}
			catch (InvalidOperationException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2103130667, InfobarMessageType.Error);
				return false;
			}
			catch (StoragePermanentException e)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e, this.owaContext);
				return false;
			}
			catch (StorageTransientException e2)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e2, this.owaContext);
				return false;
			}
			if (folderSaveResult.OperationResult != OperationResult.Succeeded)
			{
				if (folderSaveResult.PropertyErrors.Length > 0)
				{
					switch (folderSaveResult.PropertyErrors[0].PropertyErrorCode)
					{
					case PropertyErrorCode.MapiCallFailed:
						this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2103130667, InfobarMessageType.Error);
						return false;
					case PropertyErrorCode.FolderNameConflict:
						this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(1602494619, InfobarMessageType.Error);
						return false;
					}
				}
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2103130667, InfobarMessageType.Error);
				return false;
			}
			this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(-889243793), folderName), InfobarMessageType.Informational);
			return true;
		}

		public bool Delete(StoreObjectId folderId, bool isPermanentDelete)
		{
			OperationResult operationResult = OperationResult.Succeeded;
			AggregateOperationResult aggregateOperationResult;
			try
			{
				aggregateOperationResult = this.owaContext.UserContext.MailboxSession.Delete(isPermanentDelete ? DeleteItemFlags.HardDelete : DeleteItemFlags.MoveToDeletedItems, new StoreId[]
				{
					folderId
				});
				operationResult = aggregateOperationResult.OperationResult;
			}
			catch (ObjectNotFoundException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2106120183, InfobarMessageType.Error);
				return false;
			}
			catch (InvalidOperationException)
			{
				this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(1752827486, InfobarMessageType.Error);
				return false;
			}
			catch (StoragePermanentException e)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e, this.owaContext);
				return false;
			}
			catch (StorageTransientException e2)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(e2, this.owaContext);
				return false;
			}
			if (operationResult != OperationResult.Succeeded)
			{
				int i = 0;
				while (i < aggregateOperationResult.GroupOperationResults.Length)
				{
					GroupOperationResult groupOperationResult = aggregateOperationResult.GroupOperationResults[i];
					if (groupOperationResult.OperationResult != OperationResult.Succeeded)
					{
						if (groupOperationResult.Exception is ObjectNotFoundException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(2106120183, InfobarMessageType.Error);
							return false;
						}
						if (groupOperationResult.Exception is ObjectExistedException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(1041829989, InfobarMessageType.Error);
							return false;
						}
						if (groupOperationResult.Exception is CannotMoveDefaultFolderException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(1752827486, InfobarMessageType.Error);
							return false;
						}
						if (groupOperationResult.Exception is PartialCompletionException)
						{
							this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(1752827486, InfobarMessageType.Error);
							return false;
						}
						InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(aggregateOperationResult.GroupOperationResults[i].Exception, this.owaContext);
						return false;
					}
					else
					{
						i++;
					}
				}
			}
			this.owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateLocalized(-572096464, InfobarMessageType.Informational);
			return true;
		}

		private OwaContext owaContext;
	}
}
