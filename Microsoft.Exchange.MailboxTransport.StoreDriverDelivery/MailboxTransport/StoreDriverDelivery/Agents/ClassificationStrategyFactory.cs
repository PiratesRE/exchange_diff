using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ClassificationStrategyFactory
	{
		public static IDeliveryClassificationStrategy Create(MailboxSession session, VariantConfigurationSnapshot snapshot)
		{
			IDeliveryClassificationStrategy result = null;
			if (snapshot != null && ClutterUtilities.IsClassificationEnabled(session, snapshot))
			{
				result = new FolderBasedClassificationStrategy();
			}
			return result;
		}
	}
}
