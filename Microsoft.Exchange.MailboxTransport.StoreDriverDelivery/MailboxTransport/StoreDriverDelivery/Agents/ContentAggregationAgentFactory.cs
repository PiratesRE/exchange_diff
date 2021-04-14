using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class ContentAggregationAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new ContentAggregationAgentFactory.ContentAggregationAgent();
		}

		private sealed class ContentAggregationAgent : StoreDriverDeliveryAgent
		{
			public ContentAggregationAgent()
			{
				base.OnPromotedMessage += this.OnPromotedMessageHandler;
			}

			public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
			{
				StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
				MbxTransportMailItem mbxTransportMailItem = storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem;
				if (storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient || storeDriverDeliveryEventArgsImpl.IsJournalReport)
				{
					return;
				}
				Guid? guidHeaderValue = ContentAggregationAgentFactory.ContentAggregationAgent.GetGuidHeaderValue(mbxTransportMailItem, "X-MS-Exchange-Organization-Sharing-Instance-Guid");
				if (guidHeaderValue == null)
				{
					return;
				}
				storeDriverDeliveryEventArgsImpl.ShouldRunMailboxRulesBasedOnDeliveryFolder = true;
				if (storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies == null)
				{
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies = new Dictionary<PropertyDefinition, object>(3);
				}
				storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies.Add(MessageItemSchema.SharingInstanceGuid, guidHeaderValue.Value);
				ExTraceGlobals.DeliveryAgentTracer.TraceDebug<Guid?>(mbxTransportMailItem.RecordId, "Sharing Instance Guid: {0}", guidHeaderValue);
				string textHeaderValue = ContentAggregationAgentFactory.ContentAggregationAgent.GetTextHeaderValue(mbxTransportMailItem, "X-MS-Exchange-Organization-Cloud-Id");
				if (!string.IsNullOrEmpty(textHeaderValue))
				{
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies.Add(ItemSchema.CloudId, textHeaderValue);
				}
				ExTraceGlobals.DeliveryAgentTracer.TraceDebug<string>(mbxTransportMailItem.RecordId, "Cloud Id: {0}", textHeaderValue);
				string textHeaderValue2 = ContentAggregationAgentFactory.ContentAggregationAgent.GetTextHeaderValue(mbxTransportMailItem, "X-MS-Exchange-Organization-Cloud-Version");
				if (!string.IsNullOrEmpty(textHeaderValue2))
				{
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies.Add(ItemSchema.CloudVersion, textHeaderValue2);
				}
				ExTraceGlobals.DeliveryAgentTracer.TraceDebug<string>(mbxTransportMailItem.RecordId, "Cloud Version: {0}", textHeaderValue2);
				StoreId storeId;
				if (this.TryGetDeliveryFolderId(mbxTransportMailItem, storeDriverDeliveryEventArgsImpl.MailboxSession, out storeId))
				{
					storeDriverDeliveryEventArgsImpl.DeliverToFolder = storeId;
				}
				ExTraceGlobals.DeliveryAgentTracer.TraceDebug<StoreId>(mbxTransportMailItem.RecordId, "Delivery Folder: {0}", storeId);
			}

			private static Guid? GetGuidHeaderValue(MbxTransportMailItem rmi, string headerName)
			{
				string textHeaderValue = ContentAggregationAgentFactory.ContentAggregationAgent.GetTextHeaderValue(rmi, headerName);
				if (!string.IsNullOrEmpty(textHeaderValue))
				{
					try
					{
						return new Guid?(new Guid(textHeaderValue));
					}
					catch (FormatException arg)
					{
						ExTraceGlobals.DeliveryAgentTracer.TraceError<FormatException>(rmi.RecordId, "Could not make into Guid due to FormatException: {0}", arg);
					}
					catch (OverflowException arg2)
					{
						ExTraceGlobals.DeliveryAgentTracer.TraceError<OverflowException>(rmi.RecordId, "Could not make into Guid due to OverflowException: {0}", arg2);
					}
				}
				ExTraceGlobals.DeliveryAgentTracer.TraceDebug<string>(rmi.RecordId, "Header not found: {0}", headerName);
				return null;
			}

			private static string GetTextHeaderValue(MbxTransportMailItem rmi, string headerName)
			{
				TextHeader textHeader = rmi.RootPart.Headers.FindFirst(headerName) as TextHeader;
				if (textHeader == null)
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceDebug<string>(rmi.RecordId, "Header not found: {0}", headerName);
					return null;
				}
				DecodingResults decodingResults;
				string result;
				if (!textHeader.TryGetValue(DecodingOptions.None, out decodingResults, out result))
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceDebug<string>(rmi.RecordId, "Could not decode the header value for header: {0}", headerName);
					return null;
				}
				return result;
			}

			private bool TryGetDeliveryFolderId(MbxTransportMailItem rmi, MailboxSession mailBoxSession, out StoreId deliveryFolderId)
			{
				deliveryFolderId = null;
				string textHeaderValue = ContentAggregationAgentFactory.ContentAggregationAgent.GetTextHeaderValue(rmi, "X-MS-Exchange-Organization-DeliveryFolder");
				if (string.IsNullOrEmpty(textHeaderValue))
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceDebug(rmi.RecordId, "Delivery Folder ID property is empty.");
					return false;
				}
				byte[] array = null;
				try
				{
					array = HexConverter.HexStringToByteArray(textHeaderValue);
				}
				catch (FormatException arg)
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceError<FormatException>(rmi.RecordId, "Could not get Delivery Folder ID due to an invalid Mail Item header - Exception: {0}", arg);
					return false;
				}
				if (!IdConverter.IsFolderId(array))
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceError<string>(rmi.RecordId, "Can't open folder, id {0} is not a valid Exchange folder entry id.", array.ToString());
					return false;
				}
				try
				{
					deliveryFolderId = StoreObjectId.FromProviderSpecificId(array);
				}
				catch (ArgumentException arg2)
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceError<ArgumentException>(rmi.RecordId, "Could not get Delivery Folder ID. Can't convert header value to StoreObjectId - Exception: {0}", arg2);
					return false;
				}
				bool result;
				try
				{
					using (Folder.Bind(mailBoxSession, deliveryFolderId))
					{
						result = true;
					}
				}
				catch (ObjectNotFoundException arg3)
				{
					ExTraceGlobals.DeliveryAgentTracer.TraceError<StoreId, ObjectNotFoundException>(rmi.RecordId, "Can't open folder. Folder id {0} does not exist - Exception: {1}", deliveryFolderId, arg3);
					result = false;
				}
				return result;
			}

			private const int DefaultCapacityForPropertyBag = 3;

			private const string DeliveryFolderHeaderName = "X-MS-Exchange-Organization-DeliveryFolder";

			private const string SharingInstanceGuidHeaderName = "X-MS-Exchange-Organization-Sharing-Instance-Guid";

			private const string CloudIdHeaderName = "X-MS-Exchange-Organization-Cloud-Id";

			private const string CloudVersionHeaderName = "X-MS-Exchange-Organization-Cloud-Version";

			private static readonly SmtpResponse MissingDeliveryFolder = new SmtpResponse("550", "5.2.0", new string[]
			{
				"STOREDRV.ContentAggregationAgent; Missing delivery folder"
			});
		}
	}
}
