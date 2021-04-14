using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class FolderBasedClassificationStrategy : IDeliveryClassificationStrategy
	{
		public void ApplyClassification(StoreDriverDeliveryEventArgsImpl argsImpl, InferenceClassificationResult result)
		{
			argsImpl.PropertiesForAllMessageCopies[ItemSchema.IsClutter] = result.HasFlag(InferenceClassificationResult.IsClutterFinal);
			if ((result & InferenceClassificationResult.IsOverridden) != InferenceClassificationResult.None)
			{
				argsImpl.PropertiesForAllMessageCopies[MessageItemSchema.IsClutterOverridden] = true;
			}
			if (result.HasFlag(InferenceClassificationResult.IsClutterFinal))
			{
				if (argsImpl.MailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter) == null)
				{
					argsImpl.MailboxSession.CreateDefaultFolder(DefaultFolderType.Clutter);
				}
				using (Folder folder = Folder.Bind(argsImpl.MailboxSession, DefaultFolderType.Clutter))
				{
					argsImpl.SetDeliveryFolder(folder);
				}
			}
		}
	}
}
