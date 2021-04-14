using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ContentTypes.vCard;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class InboundVCardConverter
	{
		static InboundVCardConverter()
		{
			InboundVCardConverter.rootHandlersMap.Add("FN", new InboundVCardConverter.FormattedNamePropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("N", new InboundVCardConverter.NamePropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("NICKNAME", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Nickname));
			InboundVCardConverter.rootHandlersMap.Add("PHOTO", new InboundVCardConverter.PhotoPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("BDAY", new InboundVCardConverter.DatePropertyHandler(ContactSchema.Birthday));
			InboundVCardConverter.rootHandlersMap.Add("ADR", new InboundVCardConverter.AddressPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("TEL", new InboundVCardConverter.PhonePropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("EMAIL", new InboundVCardConverter.EmailPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("TITLE", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Title));
			InboundVCardConverter.rootHandlersMap.Add("ROLE", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Profession));
			InboundVCardConverter.rootHandlersMap.Add("AGENT", new InboundVCardConverter.AgentPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("ORG", new InboundVCardConverter.OrgPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("CATEGORIES", new InboundVCardConverter.MultiStringPropertyHandler(InternalSchema.Categories));
			InboundVCardConverter.rootHandlersMap.Add("NOTE", new InboundVCardConverter.NotePropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("REV", new InboundVCardConverter.DateTimePropertyHandler(StoreObjectSchema.LastModifiedTime));
			InboundVCardConverter.rootHandlersMap.Add("URL", new InboundVCardConverter.UrlPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("CLASS", new InboundVCardConverter.SensitivityPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-OL-DESIGN", new InboundVCardConverter.OutlookDesignPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-CHILD", new InboundVCardConverter.MultiStringPropertyHandler(InternalSchema.Children));
			InboundVCardConverter.rootHandlersMap.Add("X-CHILD", new InboundVCardConverter.MultiStringPropertyHandler(InternalSchema.Children));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-TEXT", new InboundVCardConverter.MsTextPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-CUSTOM", new InboundVCardConverter.MsTextPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-IMADDRESS", new InboundVCardConverter.ImAddressPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-RM-IMACCOUNT", new InboundVCardConverter.ImAddressPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-TEL", new InboundVCardConverter.MsTelPropertyHandler());
			InboundVCardConverter.rootHandlersMap.Add("X-MS-RM-COMPANYTELEPHONE", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.OrganizationMainPhone));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-ANNIVERSARY", new InboundVCardConverter.DatePropertyHandler(ContactSchema.WeddingAnniversary));
			InboundVCardConverter.rootHandlersMap.Add("X-ANNIVERSARY", new InboundVCardConverter.DatePropertyHandler(ContactSchema.WeddingAnniversary));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-SPOUSE", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.SpouseName));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-MANAGER", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Manager));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-ASSISTANT", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.AssistantName));
			InboundVCardConverter.rootHandlersMap.Add("X-ASSISTANT", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.AssistantName));
			InboundVCardConverter.rootHandlersMap.Add("FBURL", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.FreeBusyUrl));
			InboundVCardConverter.rootHandlersMap.Add("X-MS-INTERESTS", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Hobbies));
			InboundVCardConverter.rootHandlersMap.Add("X-INTERESTS", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.Hobbies));
			InboundVCardConverter.agentHandlersMap = new Dictionary<string, InboundVCardConverter.PropertyHandler>(StringComparer.OrdinalIgnoreCase);
			InboundVCardConverter.agentHandlersMap.Add("FN", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.AssistantName));
			InboundVCardConverter.agentHandlersMap.Add("TEL", new InboundVCardConverter.SimpleTextPropertyHandler(ContactSchema.AssistantPhoneNumber));
		}

		internal static void Convert(Stream dataStream, Encoding encoding, Contact contact, InboundConversionOptions options)
		{
			InboundVCardConverter.Convert(dataStream, encoding, contact, options, InboundVCardConverter.rootHandlersMap);
		}

		private static void Convert(Stream dataStream, Encoding encoding, Contact contact, InboundConversionOptions options, Dictionary<string, InboundVCardConverter.PropertyHandler> handlersMap)
		{
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			contact[InternalSchema.ItemClass] = "IPM.Contact";
			contact[InternalSchema.PostalAddressId] = PhysicalAddressType.None;
			contact[InternalSchema.ConversationIndex] = ConversationIndex.CreateNew().ToByteArray();
			contact.FileAs = FileAsMapping.LastCommaFirst;
			ContactReader contactReader = new ContactReader(dataStream, encoding, ContactComplianceMode.Loose);
			try
			{
				if (!contactReader.ReadNext())
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundVCardTracer, "InboundVCardConverter::Convert - vCard not found");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
				}
				ContactPropertyReader propertyReader = contactReader.PropertyReader;
				InboundVCardConverter.ProcessingContext context = new InboundVCardConverter.ProcessingContext(contact, handlersMap, encoding, options);
				while (propertyReader.ReadNextProperty())
				{
					InboundVCardConverter.ProcessProperty(propertyReader, context);
				}
			}
			catch (ExchangeDataException ex)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundVCardTracer, ex.ToString());
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ex);
			}
		}

		private static void ProcessProperty(ContactPropertyReader propertyReader, InboundVCardConverter.ProcessingContext context)
		{
			InboundVCardConverter.PropertyHandler propertyHandler = null;
			string text = propertyReader.Name;
			int num = text.IndexOf('.');
			if (num > 0 && num < text.Length - 1)
			{
				text = text.Substring(num + 1);
			}
			if (!context.HandlersMap.TryGetValue(text, out propertyHandler))
			{
				return;
			}
			propertyHandler.ProcessPropertyValue(context, propertyReader);
			context.UnnamedParameterValues.Clear();
			context.ApplicableTypes.Clear();
			context.Decoder = null;
			context.OverrideEncoding = null;
		}

		private static Encoding GetEncoding(string charset)
		{
			Encoding result = null;
			Charset.TryGetEncoding(charset, out result);
			return result;
		}

		private static ByteEncoder GetDecoder(string value)
		{
			if (string.Equals(value, "B", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "BASE64", StringComparison.OrdinalIgnoreCase))
			{
				return new Base64Decoder();
			}
			if (string.Equals(value, "QUOTED-PRINTABLE", StringComparison.OrdinalIgnoreCase))
			{
				return new QPDecoder();
			}
			return null;
		}

		private const int MaxPropertySize = 32768;

		private static Dictionary<string, InboundVCardConverter.PropertyHandler> rootHandlersMap = new Dictionary<string, InboundVCardConverter.PropertyHandler>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, InboundVCardConverter.PropertyHandler> agentHandlersMap;

		private class ProcessingContext
		{
			public ProcessingContext(Contact contact, Dictionary<string, InboundVCardConverter.PropertyHandler> handlersMap, Encoding encoding, InboundConversionOptions options)
			{
				this.Contact = contact;
				this.HandlersMap = handlersMap;
				this.Encoding = encoding;
				this.Options = options;
			}

			public Dictionary<string, InboundVCardConverter.PropertyHandler> HandlersMap;

			public Contact Contact;

			public List<string> UnnamedParameterValues = new List<string>();

			public List<string> ApplicableTypes = new List<string>();

			public ByteEncoder Decoder;

			public Encoding Encoding;

			public Encoding OverrideEncoding;

			public InboundConversionOptions Options;
		}

		private abstract class PropertyHandler
		{
			public InboundVCardConverter.PropertyHandler.HandlerOptions Options
			{
				get
				{
					return this.options;
				}
			}

			protected PropertyHandler(InboundVCardConverter.PropertyHandler.HandlerOptions options)
			{
				this.options = options;
			}

			protected void ProcessParameters(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				ContactParameterReader parameterReader = reader.ParameterReader;
				while (parameterReader.ReadNextParameter())
				{
					if (parameterReader.Name == null)
					{
						context.UnnamedParameterValues.Add(parameterReader.ReadValue());
					}
					else
					{
						switch (parameterReader.ParameterId)
						{
						case ParameterId.Type:
							while (parameterReader.ReadNextValue())
							{
								context.ApplicableTypes.Add(parameterReader.ReadValue());
							}
							break;
						case ParameterId.Encoding:
							if ((this.options & InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder) == InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder)
							{
								context.Decoder = InboundVCardConverter.GetDecoder(parameterReader.ReadValue());
							}
							break;
						case ParameterId.Charset:
							if ((this.options & InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset) == InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
							{
								context.OverrideEncoding = InboundVCardConverter.GetEncoding(parameterReader.ReadValue());
							}
							break;
						}
					}
				}
				if (this.options == InboundVCardConverter.PropertyHandler.HandlerOptions.None)
				{
					return;
				}
				List<string> list = new List<string>();
				foreach (string text in context.UnnamedParameterValues)
				{
					if ((this.options & InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder) == InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder && context.Decoder == null)
					{
						context.Decoder = InboundVCardConverter.GetDecoder(text);
						if (context.Decoder != null)
						{
							continue;
						}
					}
					if ((this.options & InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset) == InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset && context.OverrideEncoding == null)
					{
						context.OverrideEncoding = InboundVCardConverter.GetEncoding(text);
						if (context.OverrideEncoding != null)
						{
							continue;
						}
					}
					list.Add(text);
				}
				context.UnnamedParameterValues = list;
				if ((this.options & InboundVCardConverter.PropertyHandler.HandlerOptions.MustHaveDecoder) == InboundVCardConverter.PropertyHandler.HandlerOptions.MustHaveDecoder && context.Decoder == null)
				{
					context.Decoder = new Base64Decoder();
				}
				if (context.Decoder != null || context.OverrideEncoding != null)
				{
					reader.ApplyValueOverrides(context.OverrideEncoding, context.Decoder);
				}
			}

			public void ProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				this.ProcessParameters(context, reader);
				this.InternalProcessPropertyValue(context, reader);
			}

			protected abstract void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader);

			private InboundVCardConverter.PropertyHandler.HandlerOptions options;

			[Flags]
			public enum HandlerOptions
			{
				CanHaveDecoder = 1,
				CanOverrideCharset = 2,
				MustHaveDecoder = 5,
				None = 0
			}
		}

		private abstract class StructuredTextHandler : InboundVCardConverter.PropertyHandler
		{
			protected StructuredTextHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected static string[] ReadStructuredText(ContactPropertyReader reader, int expectedCount, InboundVCardConverter.ProcessingContext context)
			{
				string[] array = new string[expectedCount];
				for (int i = 0; i < array.Length; i++)
				{
					if (!reader.ReadNextValue())
					{
						while (i < array.Length)
						{
							array[i] = string.Empty;
							i++;
						}
						break;
					}
					array[i] = reader.ReadValueAsString(ContactValueSeparators.Semicolon).Trim();
				}
				return array;
			}
		}

		private abstract class MultiPropertyTextHandler : InboundVCardConverter.PropertyHandler
		{
			protected MultiPropertyTextHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected static void SetArrayPropertyValue(string value, Contact contact, params PropertyDefinition[] propertyList)
			{
				for (int i = 0; i < propertyList.Length; i++)
				{
					if (contact.TryGetProperty(propertyList[i]) is PropertyError)
					{
						contact[propertyList[i]] = value;
						return;
					}
				}
			}
		}

		private class SimpleTextPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public SimpleTextPropertyHandler(PropertyDefinition prop) : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
				this.prop = prop;
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				context.Contact[this.prop] = reader.ReadValueAsString();
			}

			private readonly PropertyDefinition prop;
		}

		private class DatePropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public DatePropertyHandler(PropertyDefinition prop) : base(InboundVCardConverter.PropertyHandler.HandlerOptions.None)
			{
				this.prop = prop;
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				ExTimeZone desiredTimeZone = ExTimeZone.CurrentTimeZone;
				DateTime value = reader.ReadValueAsDateTime(ContactValueType.Date);
				if (context.Contact.Session != null)
				{
					desiredTimeZone = context.Contact.Session.ExTimeZone;
				}
				context.Contact[this.prop] = new ExDateTime(desiredTimeZone, DateTime.SpecifyKind(value, DateTimeKind.Local).Date);
			}

			private readonly PropertyDefinition prop;
		}

		private class DateTimePropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public DateTimePropertyHandler(PropertyDefinition prop) : base(InboundVCardConverter.PropertyHandler.HandlerOptions.None)
			{
				this.prop = prop;
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				DateTime dateTime = reader.ReadValueAsDateTime(ContactValueType.Date);
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
				context.Contact[this.prop] = exDateTime;
			}

			private readonly PropertyDefinition prop;
		}

		private abstract class TextStreamPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			protected TextStreamPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected static Stream GetUnicodeReadStream(ContactPropertyReader reader, InboundVCardConverter.ProcessingContext context)
			{
				Stream valueReadStream = reader.GetValueReadStream();
				return new ConverterStream(valueReadStream, new TextToText
				{
					InputEncoding = (context.OverrideEncoding ?? context.Encoding),
					OutputEncoding = Encoding.Unicode
				}, ConverterStreamAccess.Read);
			}
		}

		private class FormattedNamePropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public FormattedNamePropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				string value = reader.ReadValueAsString();
				context.Contact[StoreObjectSchema.DisplayName] = value;
				context.Contact[ItemSchema.NormalizedSubject] = value;
				context.Contact[InternalSchema.ConversationTopic] = value;
			}
		}

		private class NamePropertyHandler : InboundVCardConverter.StructuredTextHandler
		{
			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				string[] array = InboundVCardConverter.StructuredTextHandler.ReadStructuredText(reader, 5, context);
				context.Contact[ContactSchema.Surname] = array[0];
				context.Contact[ContactSchema.GivenName] = array[1];
				context.Contact[ContactSchema.MiddleName] = array[2];
				context.Contact[ContactSchema.DisplayNamePrefix] = array[3];
				context.Contact[ContactSchema.Generation] = array[4];
			}
		}

		private class PhotoPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			static PhotoPropertyHandler()
			{
				InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.Add("GIF", "ContactPicture.gif");
				InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.Add("JPEG", "ContactPicture.jpg");
				InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.Add("BMP", "ContactPicture.bmp");
				InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.Add("PNG", "ContactPicture.png");
			}

			public PhotoPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.MustHaveDecoder)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				if (reader.ValueType != ContactValueType.Binary)
				{
					return;
				}
				string text = null;
				foreach (string key in context.ApplicableTypes)
				{
					if (InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.TryGetValue(key, out text))
					{
						break;
					}
				}
				if (text == null)
				{
					foreach (string key2 in context.UnnamedParameterValues)
					{
						if (InboundVCardConverter.PhotoPropertyHandler.typeToFilenameMap.TryGetValue(key2, out text))
						{
							break;
						}
					}
					if (text == null)
					{
						return;
					}
				}
				using (Stream valueReadStream = reader.GetValueReadStream())
				{
					using (StreamAttachment streamAttachment = context.Contact.AttachmentCollection.Create(AttachmentType.Stream) as StreamAttachment)
					{
						streamAttachment[AttachmentSchema.IsContactPhoto] = true;
						streamAttachment.FileName = text;
						using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
						{
							Util.StreamHandler.CopyStreamData(valueReadStream, contentStream);
						}
						streamAttachment.Save();
					}
				}
			}

			private static Dictionary<string, string> typeToFilenameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		private class AddressPropertyHandler : InboundVCardConverter.StructuredTextHandler
		{
			static AddressPropertyHandler()
			{
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("DOM", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Domestic);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("INTL", InboundVCardConverter.AddressPropertyHandler.AddressTypes.International);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("PREF", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Preferred);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("WORK", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Work);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("HOME", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Home);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("POSTAL", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Postal);
				InboundVCardConverter.AddressPropertyHandler.addressTypeMap.Add("PARCEL", InboundVCardConverter.AddressPropertyHandler.AddressTypes.Parcel);
			}

			private static string CombineStreetAddress(string value1, string value2)
			{
				if (!string.IsNullOrEmpty(value1) && !string.IsNullOrEmpty(value2))
				{
					return value1 + "\n" + value2;
				}
				if (!string.IsNullOrEmpty(value1))
				{
					return value1;
				}
				return value2;
			}

			private static InboundVCardConverter.AddressPropertyHandler.AddressTypes GetAddressType(List<string> values)
			{
				InboundVCardConverter.AddressPropertyHandler.AddressTypes addressTypes = InboundVCardConverter.AddressPropertyHandler.AddressTypes.None;
				foreach (string key in values)
				{
					InboundVCardConverter.AddressPropertyHandler.AddressTypes addressTypes2 = InboundVCardConverter.AddressPropertyHandler.AddressTypes.None;
					if (InboundVCardConverter.AddressPropertyHandler.addressTypeMap.TryGetValue(key, out addressTypes2))
					{
						addressTypes |= addressTypes2;
					}
				}
				return addressTypes;
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				InboundVCardConverter.AddressPropertyHandler.AddressTypes addressTypes = InboundVCardConverter.AddressPropertyHandler.GetAddressType(context.ApplicableTypes) | InboundVCardConverter.AddressPropertyHandler.GetAddressType(context.UnnamedParameterValues);
				if (addressTypes == InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
				{
					addressTypes = InboundVCardConverter.AddressPropertyHandler.AddressTypes.Default;
				}
				string[] array = InboundVCardConverter.StructuredTextHandler.ReadStructuredText(reader, 7, context);
				if ((addressTypes & InboundVCardConverter.AddressPropertyHandler.AddressTypes.Work) != InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
				{
					context.Contact[ContactSchema.WorkPostOfficeBox] = array[0];
					context.Contact[ContactSchema.OfficeLocation] = array[1];
					context.Contact[ContactSchema.WorkAddressStreet] = array[2];
					context.Contact[ContactSchema.WorkAddressCity] = array[3];
					context.Contact[ContactSchema.WorkAddressState] = array[4];
					context.Contact[ContactSchema.WorkAddressPostalCode] = array[5];
					context.Contact[ContactSchema.WorkAddressCountry] = array[6];
					if ((addressTypes & InboundVCardConverter.AddressPropertyHandler.AddressTypes.Preferred) != InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
					{
						context.Contact[ContactSchema.PostalAddressId] = PhysicalAddressType.Business;
					}
				}
				if ((addressTypes & InboundVCardConverter.AddressPropertyHandler.AddressTypes.Home) != InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
				{
					context.Contact[ContactSchema.HomePostOfficeBox] = array[0];
					context.Contact[ContactSchema.HomeStreet] = InboundVCardConverter.AddressPropertyHandler.CombineStreetAddress(array[1], array[2]);
					context.Contact[ContactSchema.HomeCity] = array[3];
					context.Contact[ContactSchema.HomeState] = array[4];
					context.Contact[ContactSchema.HomePostalCode] = array[5];
					context.Contact[ContactSchema.HomeCountry] = array[6];
					if ((addressTypes & InboundVCardConverter.AddressPropertyHandler.AddressTypes.Preferred) != InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
					{
						context.Contact[ContactSchema.PostalAddressId] = PhysicalAddressType.Home;
					}
				}
				if ((addressTypes & (InboundVCardConverter.AddressPropertyHandler.AddressTypes.Home | InboundVCardConverter.AddressPropertyHandler.AddressTypes.Work)) == InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
				{
					context.Contact[ContactSchema.OtherPostOfficeBox] = array[0];
					context.Contact[ContactSchema.OtherStreet] = InboundVCardConverter.AddressPropertyHandler.CombineStreetAddress(array[1], array[2]);
					context.Contact[ContactSchema.OtherCity] = array[3];
					context.Contact[ContactSchema.OtherState] = array[4];
					context.Contact[ContactSchema.OtherPostalCode] = array[5];
					context.Contact[ContactSchema.OtherCountry] = array[6];
					if ((addressTypes & InboundVCardConverter.AddressPropertyHandler.AddressTypes.Preferred) != InboundVCardConverter.AddressPropertyHandler.AddressTypes.None)
					{
						context.Contact[ContactSchema.PostalAddressId] = PhysicalAddressType.Other;
					}
				}
			}

			private static Dictionary<string, InboundVCardConverter.AddressPropertyHandler.AddressTypes> addressTypeMap = new Dictionary<string, InboundVCardConverter.AddressPropertyHandler.AddressTypes>(StringComparer.OrdinalIgnoreCase);

			[Flags]
			private enum AddressTypes
			{
				None = 0,
				Domestic = 1,
				International = 2,
				Home = 4,
				Work = 8,
				Preferred = 16,
				Postal = 32,
				Parcel = 64,
				Default = 106
			}
		}

		private class PhonePropertyHandler : InboundVCardConverter.MultiPropertyTextHandler
		{
			static PhonePropertyHandler()
			{
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("HOME", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Home);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("WORK", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Work);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("MSG", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.VoiceMessage);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("PREF", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Preferred);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("VOICE", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Voice);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("FAX", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Fax);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("CELL", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Cell);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("VIDEO", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Video);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("PAGER", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Pager);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("BBS", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.BBS);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("MODEM", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Modem);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("CAR", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Car);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("ISDN", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.ISDN);
				InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.Add("PCS", InboundVCardConverter.PhonePropertyHandler.PhoneTypes.PCS);
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				InboundVCardConverter.PhonePropertyHandler.PhoneTypes phoneTypes = InboundVCardConverter.PhonePropertyHandler.GetPhoneType(context.ApplicableTypes) | InboundVCardConverter.PhonePropertyHandler.GetPhoneType(context.UnnamedParameterValues);
				if (phoneTypes == InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					phoneTypes = InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Voice;
				}
				string value = reader.ReadValueAsString();
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Home) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Fax) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
					{
						context.Contact[ContactSchema.HomeFax] = value;
					}
					else
					{
						InboundVCardConverter.MultiPropertyTextHandler.SetArrayPropertyValue(value, context.Contact, new PropertyDefinition[]
						{
							ContactSchema.HomePhone,
							ContactSchema.HomePhone2
						});
					}
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Work) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Fax) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
					{
						context.Contact[ContactSchema.WorkFax] = value;
					}
					else
					{
						InboundVCardConverter.MultiPropertyTextHandler.SetArrayPropertyValue(value, context.Contact, new PropertyDefinition[]
						{
							ContactSchema.BusinessPhoneNumber,
							ContactSchema.BusinessPhoneNumber2
						});
					}
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Cell) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.MobilePhone] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Car) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.CarPhone] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.ISDN) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.InternationalIsdnNumber] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Preferred) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.PrimaryTelephoneNumber] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Pager) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.Pager] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Fax) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None && (phoneTypes & (InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Home | InboundVCardConverter.PhonePropertyHandler.PhoneTypes.Work)) == InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.OtherFax] = value;
				}
				if ((phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.SpecificPropertyPromotion) == InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None || (phoneTypes & InboundVCardConverter.PhonePropertyHandler.PhoneTypes.OtherPropertyPromotion) != InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None)
				{
					context.Contact[ContactSchema.OtherTelephone] = value;
				}
			}

			private static InboundVCardConverter.PhonePropertyHandler.PhoneTypes GetPhoneType(List<string> values)
			{
				InboundVCardConverter.PhonePropertyHandler.PhoneTypes phoneTypes = InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None;
				foreach (string key in values)
				{
					InboundVCardConverter.PhonePropertyHandler.PhoneTypes phoneTypes2 = InboundVCardConverter.PhonePropertyHandler.PhoneTypes.None;
					if (InboundVCardConverter.PhonePropertyHandler.phoneTypeMap.TryGetValue(key, out phoneTypes2))
					{
						phoneTypes |= phoneTypes2;
					}
				}
				return phoneTypes;
			}

			private static Dictionary<string, InboundVCardConverter.PhonePropertyHandler.PhoneTypes> phoneTypeMap = new Dictionary<string, InboundVCardConverter.PhonePropertyHandler.PhoneTypes>(StringComparer.OrdinalIgnoreCase);

			[Flags]
			private enum PhoneTypes
			{
				None = 0,
				Home = 1,
				Work = 2,
				VoiceMessage = 4,
				Preferred = 8,
				Voice = 16,
				Fax = 32,
				Cell = 64,
				Video = 128,
				Pager = 256,
				BBS = 512,
				Modem = 1024,
				Car = 2048,
				ISDN = 4096,
				PCS = 8192,
				Default = 16,
				SpecificPropertyPromotion = 6507,
				OtherPropertyPromotion = 1668
			}
		}

		private class EmailPropertyHandler : InboundVCardConverter.MultiPropertyTextHandler
		{
			static EmailPropertyHandler()
			{
				InboundVCardConverter.EmailPropertyHandler.typeToRoutingTypeMap.Add("INTERNET", InboundVCardConverter.EmailPropertyHandler.EmailType.SMTP);
				InboundVCardConverter.EmailPropertyHandler.typeToRoutingTypeMap.Add("X.400", InboundVCardConverter.EmailPropertyHandler.EmailType.X400);
				InboundVCardConverter.EmailPropertyHandler.typeToRoutingTypeMap.Add("IM", InboundVCardConverter.EmailPropertyHandler.EmailType.IM);
				InboundVCardConverter.EmailPropertyHandler.typeToRoutingTypeMap.Add("TLX", InboundVCardConverter.EmailPropertyHandler.EmailType.Telex);
				InboundVCardConverter.EmailPropertyHandler.emailSlots = new EmailAddressIndex[]
				{
					EmailAddressIndex.Email1,
					EmailAddressIndex.Email2,
					EmailAddressIndex.Email3
				};
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				bool flag = false;
				InboundVCardConverter.EmailPropertyHandler.EmailType emailType = InboundVCardConverter.EmailPropertyHandler.GetRoutingType(context.ApplicableTypes, ref flag);
				if (emailType == InboundVCardConverter.EmailPropertyHandler.EmailType.None)
				{
					emailType = InboundVCardConverter.EmailPropertyHandler.GetRoutingType(context.UnnamedParameterValues, ref flag);
				}
				if (emailType == InboundVCardConverter.EmailPropertyHandler.EmailType.None && !flag)
				{
					emailType = InboundVCardConverter.EmailPropertyHandler.EmailType.SMTP;
				}
				if (emailType == InboundVCardConverter.EmailPropertyHandler.EmailType.None || emailType == InboundVCardConverter.EmailPropertyHandler.EmailType.X400)
				{
					return;
				}
				string text = reader.ReadValueAsString();
				switch (emailType)
				{
				case InboundVCardConverter.EmailPropertyHandler.EmailType.IM:
					InboundVCardConverter.MultiPropertyTextHandler.SetArrayPropertyValue(text, context.Contact, new PropertyDefinition[]
					{
						ContactSchema.IMAddress,
						ContactSchema.IMAddress2,
						ContactSchema.IMAddress3
					});
					return;
				case InboundVCardConverter.EmailPropertyHandler.EmailType.Telex:
					context.Contact[ContactSchema.TelexNumber] = text;
					return;
				}
				for (int i = 0; i < InboundVCardConverter.EmailPropertyHandler.emailSlots.Length; i++)
				{
					if (context.Contact.EmailAddresses[InboundVCardConverter.EmailPropertyHandler.emailSlots[i]] == null)
					{
						context.Contact.EmailAddresses[InboundVCardConverter.EmailPropertyHandler.emailSlots[i]] = InboundMimeHeadersParser.CreateParticipantFromMime(null, text, context.Options, true);
						return;
					}
				}
			}

			private static InboundVCardConverter.EmailPropertyHandler.EmailType GetRoutingType(List<string> types, ref bool foundUnknownType)
			{
				foreach (string text in types)
				{
					InboundVCardConverter.EmailPropertyHandler.EmailType result;
					if (InboundVCardConverter.EmailPropertyHandler.typeToRoutingTypeMap.TryGetValue(text, out result))
					{
						return result;
					}
					if (!string.Equals(text, "PREF", StringComparison.OrdinalIgnoreCase))
					{
						foundUnknownType = true;
					}
				}
				return InboundVCardConverter.EmailPropertyHandler.EmailType.None;
			}

			private static Dictionary<string, InboundVCardConverter.EmailPropertyHandler.EmailType> typeToRoutingTypeMap = new Dictionary<string, InboundVCardConverter.EmailPropertyHandler.EmailType>(StringComparer.OrdinalIgnoreCase);

			private static EmailAddressIndex[] emailSlots;

			private enum EmailType
			{
				None,
				SMTP,
				X400,
				IM,
				Telex
			}
		}

		private class AgentPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public AgentPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				if (reader.ValueType != ContactValueType.VCard)
				{
					return;
				}
				using (Stream valueReadStream = reader.GetValueReadStream())
				{
					InboundVCardConverter.Convert(valueReadStream, context.OverrideEncoding ?? context.Encoding, context.Contact, context.Options, InboundVCardConverter.agentHandlersMap);
				}
			}
		}

		private class OrgPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public OrgPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				if (reader.ReadNextValue())
				{
					string value = reader.ReadValueAsString(ContactValueSeparators.Semicolon);
					context.Contact[ContactSchema.CompanyName] = value;
					if (reader.ReadNextValue())
					{
						string value2 = reader.ReadValueAsString(ContactValueSeparators.None);
						context.Contact[ContactSchema.Department] = value2;
					}
				}
			}
		}

		private class NotePropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public NotePropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				using (Stream valueReadStream = reader.GetValueReadStream())
				{
					Encoding encoding = context.OverrideEncoding ?? context.Encoding;
					BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextPlain, encoding.WebName);
					bodyWriteConfiguration.SetTargetFormat(BodyFormat.ApplicationRtf, encoding.WebName);
					using (Stream stream = context.Contact.Body.OpenWriteStream(bodyWriteConfiguration))
					{
						Util.StreamHandler.CopyStreamData(valueReadStream, stream);
					}
				}
			}
		}

		private class UrlPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public UrlPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder)
			{
			}

			private static void MarkWorkHomeUrl(List<string> types, ref bool isHomeUrl, ref bool isWorkUrl)
			{
				foreach (string a in types)
				{
					if (string.Equals(a, "HOME", StringComparison.OrdinalIgnoreCase))
					{
						isHomeUrl = true;
					}
					else if (string.Equals(a, "WORK", StringComparison.OrdinalIgnoreCase))
					{
						isWorkUrl = true;
					}
				}
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				bool flag = false;
				bool flag2 = false;
				InboundVCardConverter.UrlPropertyHandler.MarkWorkHomeUrl(context.ApplicableTypes, ref flag, ref flag2);
				InboundVCardConverter.UrlPropertyHandler.MarkWorkHomeUrl(context.UnnamedParameterValues, ref flag, ref flag2);
				if (!flag && !flag2)
				{
					if (!(context.Contact.TryGetProperty(InternalSchema.PersonalHomePage) is string))
					{
						flag = true;
					}
					else if (!(context.Contact.TryGetProperty(InternalSchema.BusinessHomePage) is string))
					{
						flag2 = true;
					}
				}
				string value = reader.ReadValueAsString();
				if (flag)
				{
					context.Contact[ContactSchema.PersonalHomePage] = value;
				}
				if (flag2)
				{
					context.Contact[ContactSchema.BusinessHomePage] = value;
				}
			}
		}

		private class SensitivityPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public SensitivityPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.None)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				string a = reader.ReadValueAsString();
				if (string.Equals(a, "PUBLIC", StringComparison.OrdinalIgnoreCase))
				{
					context.Contact[ItemSchema.Sensitivity] = Sensitivity.Normal;
					return;
				}
				if (string.Equals(a, "PRIVATE", StringComparison.OrdinalIgnoreCase))
				{
					context.Contact[ItemSchema.Sensitivity] = Sensitivity.Private;
					return;
				}
				if (string.Equals(a, "CONFIDENTIAL", StringComparison.OrdinalIgnoreCase))
				{
					context.Contact[ItemSchema.Sensitivity] = Sensitivity.CompanyConfidential;
				}
			}
		}

		private class KeyPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public KeyPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.MustHaveDecoder)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				if (reader.ValueType != ContactValueType.Binary)
				{
					return;
				}
				if (InboundVCardConverter.KeyPropertyHandler.IsX509Type(context.ApplicableTypes) || InboundVCardConverter.KeyPropertyHandler.IsX509Type(context.UnnamedParameterValues) || (context.ApplicableTypes.Count == 0 && context.UnnamedParameterValues.Count == 0))
				{
					using (reader.GetValueReadStream())
					{
					}
				}
			}

			private static bool IsX509Type(List<string> values)
			{
				foreach (string a in values)
				{
					if (string.Equals(a, "X509", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
		}

		private class OutlookDesignPropertyHandler : InboundVCardConverter.TextStreamPropertyHandler
		{
			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
			}
		}

		private class MsTextPropertyHandler : InboundVCardConverter.TextStreamPropertyHandler
		{
			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				using (Stream unicodeReadStream = InboundVCardConverter.TextStreamPropertyHandler.GetUnicodeReadStream(reader, context))
				{
					for (int i = 0; i < InboundVCardConverter.MsTextPropertyHandler.props.Length; i++)
					{
						PropertyError propertyError = context.Contact.TryGetProperty(InboundVCardConverter.MsTextPropertyHandler.props[i]) as PropertyError;
						if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
						{
							using (Stream stream = context.Contact.OpenPropertyStream(InboundVCardConverter.MsTextPropertyHandler.props[i], PropertyOpenMode.Create))
							{
								Util.StreamHandler.CopyStreamData(unicodeReadStream, stream);
								break;
							}
						}
					}
				}
			}

			private static PropertyDefinition[] props = new PropertyDefinition[]
			{
				ContactSchema.UserText1,
				ContactSchema.UserText2,
				ContactSchema.UserText3,
				ContactSchema.UserText4
			};
		}

		private class ImAddressPropertyHandler : InboundVCardConverter.MultiPropertyTextHandler
		{
			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				InboundVCardConverter.MultiPropertyTextHandler.SetArrayPropertyValue(reader.ReadValueAsString(), context.Contact, new PropertyDefinition[]
				{
					ContactSchema.IMAddress,
					ContactSchema.IMAddress2,
					ContactSchema.IMAddress3
				});
			}
		}

		private class MsTelPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			static MsTelPropertyHandler()
			{
				InboundVCardConverter.MsTelPropertyHandler.telTypesMap.Add("ASSISTANT", InboundVCardConverter.MsTelPropertyHandler.TelTypes.Assistant);
				InboundVCardConverter.MsTelPropertyHandler.telTypesMap.Add("TTYTDD", InboundVCardConverter.MsTelPropertyHandler.TelTypes.TTY);
				InboundVCardConverter.MsTelPropertyHandler.telTypesMap.Add("COMPANY", InboundVCardConverter.MsTelPropertyHandler.TelTypes.Company);
				InboundVCardConverter.MsTelPropertyHandler.telTypesMap.Add("CALLBACK", InboundVCardConverter.MsTelPropertyHandler.TelTypes.Callback);
				InboundVCardConverter.MsTelPropertyHandler.telTypesMap.Add("RADIO", InboundVCardConverter.MsTelPropertyHandler.TelTypes.Radio);
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap = new List<Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>>();
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap.Add(new Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>(InboundVCardConverter.MsTelPropertyHandler.TelTypes.Assistant, ContactSchema.AssistantPhoneNumber));
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap.Add(new Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>(InboundVCardConverter.MsTelPropertyHandler.TelTypes.Callback, ContactSchema.CallbackPhone));
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap.Add(new Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>(InboundVCardConverter.MsTelPropertyHandler.TelTypes.Company, ContactSchema.OrganizationMainPhone));
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap.Add(new Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>(InboundVCardConverter.MsTelPropertyHandler.TelTypes.Radio, ContactSchema.RadioPhone));
				InboundVCardConverter.MsTelPropertyHandler.typeToPropMap.Add(new Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>(InboundVCardConverter.MsTelPropertyHandler.TelTypes.TTY, ContactSchema.TtyTddPhoneNumber));
			}

			public MsTelPropertyHandler() : base(InboundVCardConverter.PropertyHandler.HandlerOptions.None)
			{
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				InboundVCardConverter.MsTelPropertyHandler.TelTypes telTypes = InboundVCardConverter.MsTelPropertyHandler.GetTelTypes(context.ApplicableTypes) | InboundVCardConverter.MsTelPropertyHandler.GetTelTypes(context.UnnamedParameterValues);
				string value = reader.ReadValueAsString();
				foreach (Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition> pair in InboundVCardConverter.MsTelPropertyHandler.typeToPropMap)
				{
					if ((telTypes & pair.First) != InboundVCardConverter.MsTelPropertyHandler.TelTypes.None)
					{
						context.Contact[pair.Second] = value;
					}
				}
			}

			private static InboundVCardConverter.MsTelPropertyHandler.TelTypes GetTelTypes(List<string> values)
			{
				InboundVCardConverter.MsTelPropertyHandler.TelTypes telTypes = InboundVCardConverter.MsTelPropertyHandler.TelTypes.None;
				foreach (string key in values)
				{
					InboundVCardConverter.MsTelPropertyHandler.TelTypes telTypes2 = InboundVCardConverter.MsTelPropertyHandler.TelTypes.None;
					if (InboundVCardConverter.MsTelPropertyHandler.telTypesMap.TryGetValue(key, out telTypes2))
					{
						telTypes |= telTypes2;
					}
				}
				return telTypes;
			}

			private static Dictionary<string, InboundVCardConverter.MsTelPropertyHandler.TelTypes> telTypesMap = new Dictionary<string, InboundVCardConverter.MsTelPropertyHandler.TelTypes>(StringComparer.OrdinalIgnoreCase);

			private static List<Pair<InboundVCardConverter.MsTelPropertyHandler.TelTypes, PropertyDefinition>> typeToPropMap;

			[Flags]
			private enum TelTypes
			{
				None = 0,
				Assistant = 1,
				TTY = 2,
				Company = 4,
				Callback = 8,
				Radio = 16
			}
		}

		private class MultiStringPropertyHandler : InboundVCardConverter.PropertyHandler
		{
			public MultiStringPropertyHandler(StorePropertyDefinition prop) : base(InboundVCardConverter.PropertyHandler.HandlerOptions.CanHaveDecoder | InboundVCardConverter.PropertyHandler.HandlerOptions.CanOverrideCharset)
			{
				this.prop = prop;
			}

			protected override void InternalProcessPropertyValue(InboundVCardConverter.ProcessingContext context, ContactPropertyReader reader)
			{
				string[] valueOrDefault = context.Contact.GetValueOrDefault<string[]>(this.prop, InboundVCardConverter.MultiStringPropertyHandler.emptyValue);
				List<string> list = new List<string>(valueOrDefault);
				while (reader.ReadNextValue())
				{
					list.Add(reader.ReadValueAsString(ContactValueSeparators.Comma | ContactValueSeparators.Semicolon));
				}
				context.Contact[this.prop] = list.ToArray();
			}

			private readonly StorePropertyDefinition prop;

			private static string[] emptyValue = Array<string>.Empty;
		}
	}
}
