using System;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MoveCollectionCommand : CollectionCommand
	{
		internal MoveCollectionCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfMoveCollections;
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		protected override void ProcessCommand(MailboxSession mailboxSession, XmlDocument doc)
		{
			if (base.CollectionRequest.CollectionName == null || base.CollectionRequest.ParentId == null || base.CollectionRequest.CollectionId == null)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidURLParameters");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
			}
			using (Folder folder = Folder.Bind(mailboxSession, base.CollectionRequest.CollectionId, null))
			{
				if (folder.DisplayName == base.CollectionRequest.CollectionName && folder.ParentId.Equals(base.CollectionRequest.ParentId))
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FolderExistsInCollectionMove");
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
				}
				if (base.CollectionRequest.CollectionName != null)
				{
					folder.DisplayName = base.CollectionRequest.CollectionName;
					folder.Save();
					folder.Load();
				}
				if (!base.CollectionRequest.ParentId.Equals(folder.ParentId))
				{
					OperationResult operationResult = mailboxSession.Move(base.CollectionRequest.ParentId, new StoreObjectId[]
					{
						base.CollectionRequest.CollectionId
					}).OperationResult;
					if (operationResult != OperationResult.Succeeded)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FolderNotFoundInCollectionMove");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ProtocolError), null, false);
					}
				}
			}
			XmlNode xmlNode = doc.CreateElement("Response", "FolderHierarchy:");
			XmlNode xmlNode2 = doc.CreateElement("Status", "FolderHierarchy:");
			xmlNode2.InnerText = "1";
			doc.AppendChild(xmlNode);
			xmlNode.AppendChild(xmlNode2);
		}
	}
}
