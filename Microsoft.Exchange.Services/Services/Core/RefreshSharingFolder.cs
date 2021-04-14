using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class RefreshSharingFolder : SingleStepServiceCommand<RefreshSharingFolderRequest, bool>
	{
		public RefreshSharingFolder(CallContext callContext, RefreshSharingFolderRequest request) : base(callContext, request)
		{
		}

		private static XmlNode CreateSyncInProgressXmlNode()
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateNode(XmlNodeType.Element, "SynchronizationStatus", "http://schemas.microsoft.com/exchange/services/2006/messages");
			xmlNode.InnerText = "InProgress";
			return xmlNode;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			RefreshSharingFolderResponseMessage refreshSharingFolderResponseMessage = new RefreshSharingFolderResponseMessage(base.Result.Code, base.Result.Error);
			if (base.Result.Value && base.Result.Error == null)
			{
				if (refreshSharingFolderResponseMessage.MessageXml == null)
				{
					refreshSharingFolderResponseMessage.MessageXml = new XmlNodeArray();
				}
				refreshSharingFolderResponseMessage.MessageXml.Nodes.Add(RefreshSharingFolder.SyncInProgressXmlNode);
			}
			return refreshSharingFolderResponseMessage;
		}

		internal override ServiceResult<bool> Execute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertXmlToIdAndSessionReadOnly(base.Request.SharingFolderId, BasicTypes.Folder);
			MailboxSession mailboxSession = (MailboxSession)idAndSession.Session;
			if (SyncAssistantInvoker.MailboxServerSupportsSync(mailboxSession))
			{
				SharingEngine.ValidateFolder(mailboxSession, idAndSession.Id);
				SyncAssistantInvoker.SyncFolder(mailboxSession, StoreId.GetStoreObjectId(idAndSession.Id));
				return new ServiceResult<bool>(true);
			}
			bool value = SharingEngine.SyncFolder(mailboxSession, idAndSession.Id);
			return new ServiceResult<bool>(value);
		}

		private static readonly XmlNode SyncInProgressXmlNode = RefreshSharingFolder.CreateSyncInProgressXmlNode();
	}
}
