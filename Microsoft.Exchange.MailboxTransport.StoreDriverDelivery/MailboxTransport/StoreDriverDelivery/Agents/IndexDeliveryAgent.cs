using System;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class IndexDeliveryAgent : StoreDriverDeliveryAgent
	{
		public IndexDeliveryAgent()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("IndexDeliveryAgent", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexDeliveryAgentTracer, (long)this.GetHashCode());
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)e;
			Header header = storeDriverDeliveryEventArgsImpl.MailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Network-Message-Id");
			this.diagnosticsSession.TraceDebug<string>("Processing message {0}", (header != null) ? header.Value : "unknown");
			if (this.mimeHeaders != null)
			{
				this.diagnosticsSession.TraceDebug("Already processed this message", new object[0]);
				return;
			}
			this.mimeHeaders = storeDriverDeliveryEventArgsImpl.MailItem.Message.MimeDocument.RootPart.Headers;
			string property = this.GetProperty("X-MS-Exchange-Forest-IndexAgent");
			if (property == null || !XHeaderStream.IsVersionSupported(property))
			{
				this.diagnosticsSession.TraceDebug("X-header not present.", new object[0]);
				return;
			}
			bool invalidAnnotationToken = false;
			EmailMessageHash emailMessageHash;
			if (EmailMessageHash.TryGetFromHeader(this.mimeHeaders, out emailMessageHash))
			{
				EmailMessageHash emailMessageHash2 = new EmailMessageHash(storeDriverDeliveryEventArgsImpl.MailItem.Message);
				this.diagnosticsSession.TraceDebug<EmailMessageHash>("Computed hash value for current message: {0}.", emailMessageHash2);
				if (!StringComparer.OrdinalIgnoreCase.Equals(emailMessageHash.ToString(), emailMessageHash2.ToString()))
				{
					this.diagnosticsSession.TraceDebug<EmailMessageHash, EmailMessageHash>("The current hash value ({0}) doesn't match the original({1})", emailMessageHash2, emailMessageHash);
					invalidAnnotationToken = true;
				}
			}
			try
			{
				using (XHeaderStream xheaderStream = new XHeaderStream(new Func<string, string>(this.GetProperty)))
				{
					this.PromoteProperties(xheaderStream, storeDriverDeliveryEventArgsImpl.ReplayItem, invalidAnnotationToken);
				}
			}
			catch (Exception arg)
			{
				this.diagnosticsSession.TraceError<Exception>("Exception during PromoteProperties: {0}", arg);
			}
		}

		private string GetProperty(string name)
		{
			return XHeaderUtils.GetProperty(this.mimeHeaders, name);
		}

		private void PromoteProperties(Stream streamResult, MessageItem messageItem, bool invalidAnnotationToken)
		{
			foreach (ISerializableProperty serializableProperty in SerializableProperties.DeserializeFrom(streamResult))
			{
				this.diagnosticsSession.TraceDebug<SerializablePropertyId>("Setting property {0}", serializableProperty.Id);
				switch (serializableProperty.Id)
				{
				case SerializablePropertyId.AnnotationToken:
				{
					SerializableStreamProperty serializableStreamProperty = (SerializableStreamProperty)serializableProperty;
					if (invalidAnnotationToken)
					{
						this.diagnosticsSession.TraceDebug("Skip promoting invalid annotation token", new object[0]);
						serializableStreamProperty.CopyTo(Stream.Null);
						continue;
					}
					using (Stream stream = messageItem.OpenPropertyStream(ItemSchema.AnnotationToken, PropertyOpenMode.Create))
					{
						serializableStreamProperty.CopyTo(stream);
						this.diagnosticsSession.TraceDebug<long>("Bytes written: {0}", stream.Position);
						continue;
					}
					break;
				}
				case SerializablePropertyId.Tasks:
					break;
				case (SerializablePropertyId)3:
				case (SerializablePropertyId)7:
				case (SerializablePropertyId)8:
					continue;
				case SerializablePropertyId.Meetings:
				{
					string text = messageItem.TryGetProperty(StoreObjectSchema.ItemClass) as string;
					if (text == null || !ObjectClass.IsMeetingMessage(text))
					{
						messageItem.SafeSetProperty(ItemSchema.XmlExtractedMeetings, serializableProperty.Value);
						continue;
					}
					continue;
				}
				case SerializablePropertyId.Addresses:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedAddresses, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Keywords:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedKeywords, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Phones:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedPhones, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Emails:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedEmails, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Urls:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedUrls, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Contacts:
					messageItem.SafeSetProperty(ItemSchema.XmlExtractedContacts, serializableProperty.Value);
					continue;
				case SerializablePropertyId.Language:
					this.diagnosticsSession.TraceDebug("Detected language: {0}", new object[]
					{
						serializableProperty.Value
					});
					messageItem.SafeSetProperty(ItemSchema.DetectedLanguage, serializableProperty.Value);
					continue;
				default:
					continue;
				}
				messageItem.SafeSetProperty(ItemSchema.XmlExtractedTasks, serializableProperty.Value);
			}
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private HeaderList mimeHeaders;
	}
}
