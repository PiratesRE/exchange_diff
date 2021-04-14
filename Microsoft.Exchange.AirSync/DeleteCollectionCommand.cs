using System;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class DeleteCollectionCommand : CollectionCommand
	{
		internal DeleteCollectionCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfDeleteCollections;
		}

		protected override void ProcessCommand(MailboxSession mailboxSession, XmlDocument doc)
		{
			if (base.CollectionRequest.CollectionId == null)
			{
				base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidURLParameters");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
			}
			mailboxSession.Delete(DeleteItemFlags.SoftDelete, new StoreObjectId[]
			{
				base.CollectionRequest.CollectionId
			});
			FolderIdMapping folderIdMapping = (FolderIdMapping)base.SyncState[CustomStateDatumType.IdMapping];
			folderIdMapping.Delete(new ISyncItemId[]
			{
				MailboxSyncItemId.CreateForNewItem(base.CollectionRequest.CollectionId)
			});
			XmlNode xmlNode = doc.CreateElement("Response", "FolderHierarchy:");
			XmlNode xmlNode2 = doc.CreateElement("Status", "FolderHierarchy:");
			xmlNode2.InnerText = "1";
			doc.AppendChild(xmlNode);
			xmlNode.AppendChild(xmlNode2);
		}
	}
}
