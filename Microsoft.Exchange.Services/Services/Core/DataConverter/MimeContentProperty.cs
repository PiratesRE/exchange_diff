using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MimeContentProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public MimeContentProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static MimeContentProperty CreateCommand(CommandContext commandContext)
		{
			return new MimeContentProperty(commandContext);
		}

		public static void ConvertMimeContentServiceObjectToMessageItem(MimeContentType mimeContentServiceObject, MessageItem messageItem, IdAndSession storeIdAndSession)
		{
			IRecipientSession adrecipientSession = storeIdAndSession.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			InboundConversionOptions inboundConversionOptions = MimeContentProperty.CreateInboundConversionOptions(adrecipientSession);
			Charset preferredCharset = null;
			bool flag = Charset.TryGetCharset("UTF-8", out preferredCharset);
			if (flag)
			{
				inboundConversionOptions.DetectionOptions.PreferredCharset = preferredCharset;
			}
			byte[] buffer = null;
			try
			{
				buffer = Convert.FromBase64String(mimeContentServiceObject.Value);
			}
			catch (FormatException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ItemConversion.ConvertAnyMimeToItem(messageItem, memoryStream, inboundConversionOptions);
			}
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			Item item = (Item)storeObject;
			IRecipientSession adrecipientSession = storeObject.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			bool allowUTF8Headers = false;
			if (this.commandContext.PropertyInformation.ToString().Equals(ItemSchema.MimeContentUTF8.ToString()))
			{
				allowUTF8Headers = true;
			}
			OutboundConversionOptions outboundConversionOptions = MimeContentProperty.CreateOutboundConversionOptions(adrecipientSession, allowUTF8Headers);
			Charset preferredCharset = null;
			bool flag = Charset.TryGetCharset("UTF-8", out preferredCharset);
			if (flag)
			{
				outboundConversionOptions.DetectionOptions.PreferredCharset = preferredCharset;
			}
			MimeContentType mimeContentType = new MimeContentType();
			mimeContentType.CharacterSet = "UTF-8";
			using (TextWriter textWriter = new StringWriter())
			{
				this.WriteMimeContent(textWriter, item, allowUTF8Headers);
				mimeContentType.Value = textWriter.ToString();
			}
			serviceObject[this.commandContext.PropertyInformation] = mimeContentType;
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.ClearItemPropertiesForMimeImport((Item)storeObject);
			this.SetProperty(setPropertyUpdate.ServiceObject, storeObject);
		}

		internal static CalendarItem CreateCalendarItemFromICAL(MimeContentType mimeContent, StoreSession session, StoreObjectId parentFolderId)
		{
			byte[] mimeContentInBytes = MimeContentProperty.GetMimeContentInBytes(mimeContent.Value);
			CalendarItem result = null;
			if (mimeContentInBytes != null && mimeContentInBytes.Length > 0)
			{
				using (MemoryStream memoryStream = new MemoryStream(mimeContentInBytes))
				{
					using (CalendarFolder calendarFolder = CalendarFolder.Bind(session, parentFolderId))
					{
						result = MimeContentProperty.ImportICalAsCalendarItem(memoryStream, calendarFolder);
					}
				}
			}
			return result;
		}

		internal void WriteMimeContent(TextWriter writer, Item item, bool allowUTF8Headers)
		{
			using (MimeContentDecodingStream mimeContentDecodingStream = new MimeContentDecodingStream(writer))
			{
				IRecipientSession adrecipientSession = item.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				OutboundConversionOptions outboundConversionOptions = MimeContentProperty.CreateOutboundConversionOptions(adrecipientSession, allowUTF8Headers);
				Charset preferredCharset;
				if (Charset.TryGetCharset("UTF-8", out preferredCharset))
				{
					outboundConversionOptions.DetectionOptions.PreferredCharset = preferredCharset;
				}
				this.ConvertItemToMime(item, mimeContentDecodingStream, outboundConversionOptions);
			}
		}

		private static OutboundConversionOptions CreateOutboundConversionOptions(IRecipientSession adRecipientSession, bool allowUTF8Headers)
		{
			CallContext callContext = CallContext.Current;
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(callContext.DefaultDomain.DomainName.Domain);
			outboundConversionOptions.ClearCategories = false;
			outboundConversionOptions.DemoteBcc = true;
			outboundConversionOptions.UserADSession = adRecipientSession;
			outboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(outboundConversionOptions.UserADSession.SessionSettings.CurrentOrganizationId);
			outboundConversionOptions.EwsOutboundMimeConversion = true;
			if (allowUTF8Headers)
			{
				outboundConversionOptions.AllowUTF8Headers = true;
			}
			return outboundConversionOptions;
		}

		private static InboundConversionOptions CreateInboundConversionOptions(IRecipientSession adRecipientSession)
		{
			CallContext callContext = CallContext.Current;
			InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(callContext.DefaultDomain.DomainName.Domain);
			inboundConversionOptions.ClearCategories = false;
			inboundConversionOptions.UserADSession = adRecipientSession;
			inboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(inboundConversionOptions.UserADSession.SessionSettings.CurrentOrganizationId);
			return inboundConversionOptions;
		}

		private static byte[] GetMimeContentInBytes(string base64MimeContent)
		{
			if (string.IsNullOrEmpty(base64MimeContent))
			{
				throw new InvalidMimeContentException((CoreResources.IDs)3907819958U);
			}
			if (base64MimeContent.Length > 2147483647)
			{
				throw new DataSizeLimitException(new PropertyUri(PropertyUriEnum.MimeContent));
			}
			byte[] result = null;
			try
			{
				result = Convert.FromBase64String(base64MimeContent);
			}
			catch (FormatException innerException)
			{
				throw new InvalidMimeContentException((CoreResources.IDs)3907819958U, innerException);
			}
			return result;
		}

		private static CalendarItem ImportICalAsCalendarItem(MemoryStream mimeContentStream, CalendarFolder calendarFolder)
		{
			CalendarItem result;
			try
			{
				CallContext callContext = CallContext.Current;
				result = calendarFolder.ImportICAL(mimeContentStream, "UTF-8", new InboundConversionOptions(callContext.DefaultDomain.DomainName.Domain)
				{
					IsSenderTrusted = true,
					UserADSession = calendarFolder.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)
				});
			}
			catch (ConversionFailedException innerException)
			{
				throw new InvalidMimeContentException((CoreResources.IDs)3846347532U, innerException);
			}
			return result;
		}

		private void ConvertItemToMime(Item item, Stream mimeContentStream, OutboundConversionOptions outboundConversionOptions)
		{
			try
			{
				item.Load(StoreObjectSchema.ContentConversionProperties);
				if (item is CalendarItemBase)
				{
					CalendarItemBase calendarItemBase = (CalendarItemBase)item;
					calendarItemBase.ExportAsICAL(mimeContentStream, "UTF-8", outboundConversionOptions);
				}
				else if (item is Contact)
				{
					Contact contact = (Contact)item;
					Contact.ExportVCard(contact, mimeContentStream, outboundConversionOptions);
				}
				else
				{
					ItemConversion.ConvertItemToMime(item, mimeContentStream, outboundConversionOptions);
				}
			}
			catch (ConversionFailedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[MimeContentProperty::ConvertItemToMime] encountered exception - Class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				throw new InvalidMimeContentException((CoreResources.IDs)3846347532U, ex);
			}
			catch (PropertyErrorException ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[MimeContentProperty::ConvertItemToMime] encountered exception - Class: {0}, Message: {1}", ex2.GetType().FullName, ex2.Message);
				throw new InvalidMimeContentException((CoreResources.IDs)3846347532U, ex2);
			}
		}

		private void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			MimeContentType valueOrDefault = serviceObject.GetValueOrDefault<MimeContentType>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				byte[] mimeContentInBytes = MimeContentProperty.GetMimeContentInBytes(valueOrDefault.Value);
				this.SetProperty(storeObject, mimeContentInBytes);
			}
		}

		private void SetProperty(StoreObject storeObject, byte[] mimeContentBytes)
		{
			Item item = (Item)storeObject;
			if (item is CalendarItem)
			{
				return;
			}
			if (!(item is PostItem) && !(item is MessageItem))
			{
				throw new UnsupportedMimeConversionException();
			}
			if (mimeContentBytes != null && mimeContentBytes.Length > 0)
			{
				CallContext callContext = CallContext.Current;
				InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(callContext.DefaultDomain.DomainName.Domain);
				inboundConversionOptions.ClearCategories = false;
				inboundConversionOptions.IsSenderTrusted = true;
				inboundConversionOptions.UserADSession = storeObject.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				inboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(inboundConversionOptions.UserADSession.SessionSettings.CurrentOrganizationId);
				using (MemoryStream memoryStream = new MemoryStream(mimeContentBytes))
				{
					try
					{
						ItemConversion.CleanItemForMimeConversion(item);
						ItemConversion.ConvertAnyMimeToItem(item, memoryStream, inboundConversionOptions);
					}
					catch (StoragePermanentException innerException)
					{
						throw new InvalidMimeContentException(CoreResources.IDs.ErrorMimeContentInvalid, innerException);
					}
					catch (StorageTransientException innerException2)
					{
						throw new InvalidMimeContentException(CoreResources.IDs.ErrorMimeContentInvalid, innerException2);
					}
				}
			}
		}

		private void ClearItemPropertiesForMimeImport(Item item)
		{
			if (item.AttachmentCollection != null && item.AttachmentCollection.Count > 0)
			{
				item.AttachmentCollection.RemoveAll();
			}
			using (TextWriter textWriter = item.Body.OpenTextWriter(BodyFormat.TextPlain))
			{
				textWriter.Write(string.Empty);
			}
			MessageItem messageItem = item as MessageItem;
			if (messageItem != null)
			{
				messageItem.Recipients.Clear();
				messageItem.IsReadReceiptRequested = false;
				messageItem.IsDeliveryReceiptRequested = false;
			}
			item.Sensitivity = Sensitivity.Normal;
			item.Importance = Importance.Normal;
			item.DeleteProperties(new PropertyDefinition[]
			{
				ItemSchema.IconIndex
			});
		}

		public void ToXml()
		{
			throw new InvalidOperationException("Not supported");
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceProperty = commandSettings.ServiceProperty;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (serviceObject != null)
			{
				this.SetProperty(serviceObject, storeObject);
				return;
			}
			this.SetProperty(serviceProperty, storeObject);
		}

		private static byte[] GetMimeContentInBytes(XmlElement mimeElement)
		{
			return MimeContentProperty.GetMimeContentInBytes(ServiceXml.GetXmlTextNodeValue(mimeElement));
		}

		private void SetProperty(XmlElement serviceProperty, StoreObject storeObject)
		{
			byte[] mimeContentInBytes = MimeContentProperty.GetMimeContentInBytes(serviceProperty);
			this.SetProperty(storeObject, mimeContentInBytes);
		}

		private const string PreferredCharSetUnicode = "UTF-8";
	}
}
