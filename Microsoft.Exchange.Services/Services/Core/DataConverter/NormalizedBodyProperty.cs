using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class NormalizedBodyProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		public NormalizedBodyProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static NormalizedBodyProperty CreateCommand(CommandContext commandContext)
		{
			return new NormalizedBodyProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			ResponseShape responseShape = commandSettings.ResponseShape;
			this.xsoItem = (storeObject as Item);
			this.itemResponseShape = (responseShape as ItemResponseShape);
			this.idAndSession = idAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			if (this.itemResponseShape.CanCreateNormalizedBodyServiceObject)
			{
				serviceObject.PropertyBag[propertyInformation] = this.CreateNormalizedBodyServiceObject();
				return;
			}
			this.itemResponseShape.BodyType = BodyResponseType.HTML;
			BodyProperty bodyProperty = BodyProperty.CreateCommand(this.commandContext);
			bodyProperty.CharBuffer = new char[32768];
			bodyProperty.InlineAttachmentAction = delegate(string value)
			{
				IDictionary<string, bool> inlineImagesInNormalizedBody = EWSSettings.InlineImagesInNormalizedBody;
				inlineImagesInNormalizedBody[value] = true;
			};
			((IToServiceObjectCommand)bodyProperty).ToServiceObject();
		}

		private BodyContentType CreateNormalizedBodyServiceObject()
		{
			BodyType bodyType = BodyProperty.GetBodyType(this.itemResponseShape.NormalizedBodyType);
			BodyContentType bodyContentType = new BodyContentType
			{
				BodyType = bodyType,
				Value = string.Empty
			};
			switch (bodyType)
			{
			case BodyType.Text:
				this.FillTextBody(bodyContentType);
				return bodyContentType;
			}
			this.FillNormalizedHtmlBody(bodyContentType);
			return bodyContentType;
		}

		private void FillTextBody(BodyContentType bodyContent)
		{
			Body effectiveBody = Util.GetEffectiveBody(this.xsoItem);
			using (TextReader textReader = effectiveBody.OpenTextReader(BodyFormat.TextPlain))
			{
				UniqueBodyProperty.GetTruncatedString(textReader.ReadToEnd(), this.itemResponseShape.MaximumBodySize, bodyContent);
			}
		}

		private void FillNormalizedHtmlBody(BodyContentType bodyContent)
		{
			try
			{
				bool itemBlockStatus = Util.GetItemBlockStatus(this.xsoItem, this.itemResponseShape.BlockExternalImages, this.itemResponseShape.BlockExternalImagesIfSenderUntrusted);
				HtmlBodyCallback htmlBodyCallback = new HtmlBodyCallback(this.xsoItem, this.idAndSession, false);
				htmlBodyCallback.AddBlankTargetToLinks = this.itemResponseShape.AddBlankTargetToLinks;
				htmlBodyCallback.InlineImageUrlTemplate = this.itemResponseShape.InlineImageUrlTemplate;
				htmlBodyCallback.InlineImageUrlOnLoadTemplate = this.itemResponseShape.InlineImageUrlOnLoadTemplate;
				htmlBodyCallback.InlineImageCustomDataTemplate = this.itemResponseShape.InlineImageCustomDataTemplate;
				htmlBodyCallback.IsBodyFragment = false;
				htmlBodyCallback.BlockExternalImages = itemBlockStatus;
				htmlBodyCallback.CalculateAttachmentInlineProps = this.itemResponseShape.CalculateAttachmentInlineProps;
				htmlBodyCallback.HasBlockedImagesAction = delegate(bool value)
				{
					EWSSettings.ItemHasBlockedImages = new bool?(value);
				};
				htmlBodyCallback.InlineAttachmentIdAction = delegate(string value)
				{
					IDictionary<string, bool> inlineImagesInNormalizedBody = EWSSettings.InlineImagesInNormalizedBody;
					inlineImagesInNormalizedBody[value] = true;
				};
				HtmlBodyCallback callback = htmlBodyCallback;
				long num = 0L;
				Body effectiveBody = Util.GetEffectiveBody(this.xsoItem);
				ConversationBodyScanner conversationBodyScanner = effectiveBody.GetConversationBodyScanner(callback, -1L, 0L, true, this.itemResponseShape.FilterHtmlContent, out num);
				StringBuilder stringBuilder = new StringBuilder((int)num);
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					using (HtmlWriter htmlWriter = new HtmlWriter(stringWriter))
					{
						conversationBodyScanner.WriteAll(htmlWriter);
						htmlWriter.Flush();
					}
					stringWriter.Flush();
				}
				UniqueBodyProperty.GetTruncatedString(stringBuilder.ToString(), this.itemResponseShape.MaximumBodySize, bodyContent);
				FaultInjection.GenerateFault((FaultInjection.LIDs)2231774525U);
			}
			catch (TextConvertersException innerException)
			{
				throw new PropertyRequestFailedException(CoreResources.IDs.ErrorItemPropertyRequestFailed, this.commandContext.PropertyInformation.PropertyPath, innerException);
			}
		}

		public void ToXml()
		{
			throw new InvalidOperationException("NormalizedBodyProperty.ToXml should not be called.");
		}

		private Item xsoItem;

		private ItemResponseShape itemResponseShape;

		private IdAndSession idAndSession;
	}
}
