using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ContentTypes.vCard;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class OutboundVCardConverter
	{
		static OutboundVCardConverter()
		{
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.GenericExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.FormattedNameExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.NameExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.PhotoExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.EmailExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.NoteExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.OrgExporter.Instance);
			OutboundVCardConverter.exporters.Add(OutboundVCardConverter.ClassExporter.Instance);
			OutboundVCardConverter.AddressExporter.RegisterAll(OutboundVCardConverter.exporters);
			OutboundVCardConverter.TypedPropertyStringExporter.RegisterAll(OutboundVCardConverter.exporters);
			OutboundVCardConverter.MultiStringExporter.RegisterAll(OutboundVCardConverter.exporters);
			OutboundVCardConverter.DatePropExporter.RegisterAll(OutboundVCardConverter.exporters);
			OutboundVCardConverter.StreamTextExporter.RegisterAll(OutboundVCardConverter.exporters);
		}

		internal static void Convert(Stream dataStream, Encoding encoding, Contact contact, OutboundConversionOptions options, ConversionLimitsTracker limitsTracker)
		{
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			ContactWriter contactWriter = new ContactWriter(dataStream, encoding);
			contactWriter.StartVCard();
			OutboundVCardConverter.PropertyExporter.Context context = new OutboundVCardConverter.PropertyExporter.Context(encoding, options, limitsTracker);
			foreach (OutboundVCardConverter.PropertyExporter propertyExporter in OutboundVCardConverter.exporters)
			{
				propertyExporter.Export(contactWriter, contact, context);
			}
			contactWriter.EndVCard();
		}

		private static readonly List<OutboundVCardConverter.PropertyExporter> exporters = new List<OutboundVCardConverter.PropertyExporter>();

		private abstract class PropertyExporter
		{
			public abstract void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context);

			internal class Context
			{
				internal Context(Encoding encoding, OutboundConversionOptions options, ConversionLimitsTracker limitsTracker)
				{
					this.Encoding = encoding;
					this.AddressCache = new OutboundAddressCache(options, limitsTracker);
					this.Options = options;
				}

				internal readonly OutboundAddressCache AddressCache;

				internal readonly Encoding Encoding;

				internal readonly OutboundConversionOptions Options;
			}
		}

		private class GenericExporter : OutboundVCardConverter.PropertyExporter
		{
			private GenericExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				writer.WriteProperty(PropertyId.Profile, "VCARD");
				writer.WriteProperty(PropertyId.Version, "3.0");
				writer.WriteProperty(PropertyId.Mailer, "Microsoft Exchange");
				writer.WriteProperty(PropertyId.ProductId, "Microsoft Exchange");
			}

			public static readonly OutboundVCardConverter.GenericExporter Instance = new OutboundVCardConverter.GenericExporter();
		}

		private class FormattedNameExporter : OutboundVCardConverter.PropertyExporter
		{
			private FormattedNameExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				string text = contact.TryGetProperty(ContactSchema.FullName) as string;
				if (text == null)
				{
					text = (contact.TryGetProperty(ItemSchema.NormalizedSubject) as string);
					if (text == null)
					{
						text = string.Empty;
					}
				}
				writer.WriteProperty(PropertyId.CommonName, text);
			}

			public static readonly OutboundVCardConverter.FormattedNameExporter Instance = new OutboundVCardConverter.FormattedNameExporter();
		}

		private class NameExporter : OutboundVCardConverter.PropertyExporter
		{
			private NameExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				writer.StartProperty(PropertyId.StructuredName);
				for (int i = 0; i < OutboundVCardConverter.NameExporter.sourceProperties.Length; i++)
				{
					string text = contact.TryGetProperty(OutboundVCardConverter.NameExporter.sourceProperties[i]) as string;
					if (text == null)
					{
						text = string.Empty;
					}
					writer.WritePropertyValue(text, ContactValueSeparators.Semicolon);
				}
			}

			public static readonly OutboundVCardConverter.NameExporter Instance = new OutboundVCardConverter.NameExporter();

			private static readonly PropertyDefinition[] sourceProperties = new PropertyDefinition[]
			{
				ContactSchema.Surname,
				ContactSchema.GivenName,
				ContactSchema.MiddleName,
				ContactSchema.DisplayNamePrefix,
				ContactSchema.Generation
			};
		}

		private class PhotoExporter : OutboundVCardConverter.PropertyExporter
		{
			static PhotoExporter()
			{
				OutboundVCardConverter.PhotoExporter.contentTypeToType.Add("image/jpeg", "JPEG");
				OutboundVCardConverter.PhotoExporter.contentTypeToType.Add("image/gif", "GIF");
				OutboundVCardConverter.PhotoExporter.contentTypeToType.Add("image/png", "PNG");
				OutboundVCardConverter.PhotoExporter.contentTypeToType.Add("image/bmp", "BMP");
			}

			private PhotoExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				foreach (AttachmentHandle handle in contact.AttachmentCollection)
				{
					using (Attachment attachment = contact.AttachmentCollection.Open(handle, null))
					{
						if (attachment.IsContactPhoto)
						{
							StreamAttachment streamAttachment = attachment as StreamAttachment;
							if (streamAttachment != null)
							{
								string text = streamAttachment.ContentType;
								if (string.IsNullOrEmpty(text))
								{
									text = streamAttachment.CalculatedContentType;
								}
								string value = null;
								if (!string.IsNullOrEmpty(text) && OutboundVCardConverter.PhotoExporter.contentTypeToType.TryGetValue(text, out value))
								{
									using (Stream stream = streamAttachment.TryGetContentStream(PropertyOpenMode.ReadOnly))
									{
										if (stream != null)
										{
											writer.StartProperty(PropertyId.Photo);
											writer.WriteParameter(ParameterId.Type, value);
											writer.WriteParameter(ParameterId.Encoding, "B");
											using (Stream stream2 = new EncoderStream(stream, new Base64Encoder(0), EncoderStreamAccess.Read))
											{
												writer.WritePropertyValue(stream2);
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			public static readonly OutboundVCardConverter.PhotoExporter Instance = new OutboundVCardConverter.PhotoExporter();

			private static readonly Dictionary<string, string> contentTypeToType = new Dictionary<string, string>();
		}

		private class EmailExporter : OutboundVCardConverter.PropertyExporter
		{
			private EmailExporter()
			{
			}

			private void WriteParticipant(ContactWriter writer, Participant addr, OutboundConversionOptions options)
			{
				if (addr == null)
				{
					return;
				}
				string participantSmtpAddress = ItemToMimeConverter.GetParticipantSmtpAddress(addr, options);
				if (!string.IsNullOrEmpty(participantSmtpAddress))
				{
					writer.StartProperty(PropertyId.EMail);
					writer.WriteParameter(ParameterId.Type, "INTERNET");
					writer.WritePropertyValue(participantSmtpAddress);
				}
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				context.AddressCache.CopyDataFromItem(contact);
				context.AddressCache.Resolve();
				this.WriteParticipant(writer, context.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ContactEmail1], context.Options);
				this.WriteParticipant(writer, context.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ContactEmail2], context.Options);
				this.WriteParticipant(writer, context.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ContactEmail3], context.Options);
			}

			public static readonly OutboundVCardConverter.EmailExporter Instance = new OutboundVCardConverter.EmailExporter();
		}

		private class MultiStringExporter : OutboundVCardConverter.PropertyExporter
		{
			static MultiStringExporter()
			{
				OutboundVCardConverter.MultiStringExporter.Instances.Add(new OutboundVCardConverter.MultiStringExporter(InternalSchema.Categories, "CATEGORIES"));
				OutboundVCardConverter.MultiStringExporter.Instances.Add(new OutboundVCardConverter.MultiStringExporter(InternalSchema.Children, "X-MS-CHILD"));
			}

			public static void RegisterAll(List<OutboundVCardConverter.PropertyExporter> list)
			{
				foreach (OutboundVCardConverter.MultiStringExporter item in OutboundVCardConverter.MultiStringExporter.Instances)
				{
					list.Add(item);
				}
			}

			private MultiStringExporter(NativeStorePropertyDefinition prop, string propName)
			{
				this.prop = prop;
				this.propName = propName;
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				string[] array = contact.TryGetProperty(this.prop) as string[];
				if (array != null && array.Length > 0)
				{
					writer.StartProperty(this.propName);
					foreach (string value in array)
					{
						writer.WritePropertyValue(value, ContactValueSeparators.Comma);
					}
				}
			}

			public static readonly List<OutboundVCardConverter.MultiStringExporter> Instances = new List<OutboundVCardConverter.MultiStringExporter>();

			private readonly NativeStorePropertyDefinition prop;

			private readonly string propName;
		}

		private class DatePropExporter : OutboundVCardConverter.PropertyExporter
		{
			static DatePropExporter()
			{
				OutboundVCardConverter.DatePropExporter.Instances.Add(new OutboundVCardConverter.DatePropExporter(InternalSchema.Birthday, ContactValueType.Date, "BDAY"));
				OutboundVCardConverter.DatePropExporter.Instances.Add(new OutboundVCardConverter.DatePropExporter(InternalSchema.LastModifiedTime, ContactValueType.DateTime, "REV"));
				OutboundVCardConverter.DatePropExporter.Instances.Add(new OutboundVCardConverter.DatePropExporter(InternalSchema.WeddingAnniversary, ContactValueType.Date, "X-MS-ANNIVERSARY"));
			}

			public static void RegisterAll(List<OutboundVCardConverter.PropertyExporter> list)
			{
				foreach (OutboundVCardConverter.DatePropExporter item in OutboundVCardConverter.DatePropExporter.Instances)
				{
					list.Add(item);
				}
			}

			private DatePropExporter(NativeStorePropertyDefinition prop, ContactValueType type, string propName)
			{
				this.prop = prop;
				this.propName = propName;
				this.type = type;
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				ExDateTime? exDateTime = contact.TryGetProperty(this.prop) as ExDateTime?;
				if (exDateTime != null)
				{
					writer.StartProperty(this.propName);
					writer.WriteValueTypeParameter(this.type);
					writer.WritePropertyValue((DateTime)exDateTime.Value.ToUtc());
				}
			}

			public static readonly List<OutboundVCardConverter.DatePropExporter> Instances = new List<OutboundVCardConverter.DatePropExporter>();

			private readonly NativeStorePropertyDefinition prop;

			private readonly ContactValueType type;

			private readonly string propName;
		}

		private class AddressExporter : OutboundVCardConverter.PropertyExporter
		{
			static AddressExporter()
			{
				OutboundVCardConverter.AddressExporter.instances.Add(new OutboundVCardConverter.AddressExporter(new NativeStorePropertyDefinition[]
				{
					InternalSchema.WorkPostOfficeBox,
					InternalSchema.OfficeLocation,
					InternalSchema.WorkAddressStreet,
					InternalSchema.WorkAddressCity,
					InternalSchema.WorkAddressState,
					InternalSchema.WorkAddressPostalCode,
					InternalSchema.WorkAddressCountry
				}, InternalSchema.BusinessAddress, PhysicalAddressType.Business, "WORK"));
				OutboundVCardConverter.AddressExporter.instances.Add(new OutboundVCardConverter.AddressExporter(new NativeStorePropertyDefinition[]
				{
					InternalSchema.HomePostOfficeBox,
					null,
					InternalSchema.HomeStreet,
					InternalSchema.HomeCity,
					InternalSchema.HomeState,
					InternalSchema.HomePostalCode,
					InternalSchema.HomeCountry
				}, InternalSchema.HomeAddress, PhysicalAddressType.Home, "HOME"));
				OutboundVCardConverter.AddressExporter.instances.Add(new OutboundVCardConverter.AddressExporter(new NativeStorePropertyDefinition[]
				{
					InternalSchema.OtherPostOfficeBox,
					null,
					InternalSchema.OtherStreet,
					InternalSchema.OtherCity,
					InternalSchema.OtherState,
					InternalSchema.OtherPostalCode,
					InternalSchema.OtherCountry
				}, InternalSchema.OtherAddress, PhysicalAddressType.Other, "POSTAL"));
			}

			public static void RegisterAll(List<OutboundVCardConverter.PropertyExporter> list)
			{
				foreach (OutboundVCardConverter.AddressExporter item in OutboundVCardConverter.AddressExporter.instances)
				{
					list.Add(item);
				}
			}

			private AddressExporter(NativeStorePropertyDefinition[] props, PropertyDefinition labelProp, PhysicalAddressType addrType, string type)
			{
				this.props = props;
				this.labelProp = labelProp;
				this.addrType = addrType;
				this.type = type;
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				writer.StartProperty(PropertyId.Address);
				writer.StartParameter(ParameterId.Type);
				writer.WriteParameterValue(this.type);
				PhysicalAddressType valueOrDefault = contact.GetValueOrDefault<PhysicalAddressType>(ContactSchema.PostalAddressId);
				if (valueOrDefault == this.addrType)
				{
					writer.WriteParameterValue("PREF");
				}
				for (int i = 0; i < this.props.Length; i++)
				{
					string text = string.Empty;
					if (this.props[i] != null)
					{
						text = (contact.TryGetProperty(this.props[i]) as string);
						if (text == null)
						{
							text = string.Empty;
						}
					}
					writer.WritePropertyValue(text, ContactValueSeparators.Semicolon);
				}
				string text2 = contact.TryGetProperty(this.labelProp) as string;
				if (text2 != null)
				{
					writer.StartProperty(PropertyId.Label);
					writer.StartParameter(ParameterId.Type);
					writer.WriteParameterValue(this.type);
					if (valueOrDefault == this.addrType)
					{
						writer.WriteParameterValue("PREF");
					}
					writer.WritePropertyValue(text2);
				}
			}

			private static readonly List<OutboundVCardConverter.AddressExporter> instances = new List<OutboundVCardConverter.AddressExporter>();

			private readonly NativeStorePropertyDefinition[] props;

			private readonly PhysicalAddressType addrType;

			private readonly string type;

			private readonly PropertyDefinition labelProp;
		}

		private class TypedPropertyStringExporter : OutboundVCardConverter.PropertyExporter
		{
			static TypedPropertyStringExporter()
			{
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.BusinessPhoneNumber, "TEL", new string[]
				{
					"WORK"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.BusinessPhoneNumber2, "TEL", new string[]
				{
					"WORK"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.HomePhone, "TEL", new string[]
				{
					"HOME"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.HomePhone2, "TEL", new string[]
				{
					"HOME"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.OtherFax, "TEL", new string[]
				{
					"FAX"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.MobilePhone, "TEL", new string[]
				{
					"CELL"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Pager, "TEL", new string[]
				{
					"PAGER"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.CarPhone, "TEL", new string[]
				{
					"CAR"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.InternationalIsdnNumber, "TEL", new string[]
				{
					"ISDN"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.HomeFax, "TEL", new string[]
				{
					"HOME",
					"FAX"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.FaxNumber, "TEL", new string[]
				{
					"WORK",
					"FAX"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.PrimaryTelephoneNumber, "TEL", new string[]
				{
					"PREF"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.OtherTelephone, "TEL", new string[]
				{
					"VOICE"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.AssistantPhoneNumber, "X-MS-TEL", new string[]
				{
					"ASSISTANT"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.TtyTddPhoneNumber, "X-MS-TEL", new string[]
				{
					"TTYTDD"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.CallbackPhone, "X-MS-TEL", new string[]
				{
					"CALLBACK"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.RadioPhone, "X-MS-TEL", new string[]
				{
					"RADIO"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.OrganizationMainPhone, "X-MS-TEL", new string[]
				{
					"COMPANY"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.SpouseName, "X-MS-SPOUSE", new string[]
				{
					"N"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Manager, "X-MS-MANAGER", new string[]
				{
					"N"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.AssistantName, "X-MS-ASSISTANT", new string[]
				{
					"N"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.TelexNumber, "EMAIL", new string[]
				{
					"TLX"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Nickname, "NICKNAME", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Title, "TITLE", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Profession, "ROLE", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.IMAddress, "X-MS-IMADDRESS", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.IMAddress2, "X-MS-IMADDRESS", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.IMAddress3, "X-MS-IMADDRESS", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.FreeBusyUrl, "FBURL", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.Hobbies, "X-MS-INTERESTS", new string[0]));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.PersonalHomePage, "URL", new string[]
				{
					"HOME"
				}));
				OutboundVCardConverter.TypedPropertyStringExporter.instances.Add(new OutboundVCardConverter.TypedPropertyStringExporter(InternalSchema.BusinessHomePage, "URL", new string[]
				{
					"WORK"
				}));
			}

			public static void RegisterAll(List<OutboundVCardConverter.PropertyExporter> list)
			{
				foreach (OutboundVCardConverter.TypedPropertyStringExporter item in OutboundVCardConverter.TypedPropertyStringExporter.instances)
				{
					list.Add(item);
				}
			}

			private TypedPropertyStringExporter(NativeStorePropertyDefinition prop, string propName, params string[] types)
			{
				this.prop = prop;
				this.types = types;
				this.propName = propName;
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				string text = contact.TryGetProperty(this.prop) as string;
				if (text != null)
				{
					writer.StartProperty(this.propName);
					if (this.types != null && this.types.Length > 0)
					{
						writer.StartParameter(ParameterId.Type);
						foreach (string value in this.types)
						{
							writer.WriteParameterValue(value);
						}
					}
					writer.WritePropertyValue(text);
				}
			}

			private static readonly List<OutboundVCardConverter.TypedPropertyStringExporter> instances = new List<OutboundVCardConverter.TypedPropertyStringExporter>();

			private readonly NativeStorePropertyDefinition prop;

			private readonly string propName;

			private readonly string[] types;
		}

		private class OrgExporter : OutboundVCardConverter.PropertyExporter
		{
			private OrgExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				writer.StartProperty(PropertyId.Organization);
				string valueOrDefault = contact.GetValueOrDefault<string>(InternalSchema.CompanyName, string.Empty);
				writer.WritePropertyValue(valueOrDefault, ContactValueSeparators.Semicolon);
				valueOrDefault = contact.GetValueOrDefault<string>(InternalSchema.Department, string.Empty);
				writer.WritePropertyValue(valueOrDefault, ContactValueSeparators.Semicolon);
			}

			public static readonly OutboundVCardConverter.OrgExporter Instance = new OutboundVCardConverter.OrgExporter();
		}

		private class NoteExporter : OutboundVCardConverter.PropertyExporter
		{
			private NoteExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				if (contact.Body.IsBodyDefined)
				{
					BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain, context.Encoding.WebName);
					using (Stream stream = contact.Body.OpenReadStream(configuration))
					{
						writer.StartProperty(PropertyId.Note);
						writer.WritePropertyValue(stream);
					}
				}
			}

			public static readonly OutboundVCardConverter.NoteExporter Instance = new OutboundVCardConverter.NoteExporter();
		}

		private class ClassExporter : OutboundVCardConverter.PropertyExporter
		{
			private ClassExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				Sensitivity? sensitivity = contact.TryGetProperty(InternalSchema.Sensitivity) as Sensitivity?;
				if (sensitivity != null)
				{
					switch (sensitivity.Value)
					{
					case Sensitivity.Normal:
						writer.WriteProperty(PropertyId.Class, "PUBLIC");
						break;
					case Sensitivity.Personal:
					case Sensitivity.Private:
						writer.WriteProperty(PropertyId.Class, "PRIVATE");
						return;
					case Sensitivity.CompanyConfidential:
						writer.WriteProperty(PropertyId.Class, "CONFIDENTIAL");
						return;
					default:
						return;
					}
				}
			}

			public static readonly OutboundVCardConverter.ClassExporter Instance = new OutboundVCardConverter.ClassExporter();
		}

		private class KeyExporter : OutboundVCardConverter.PropertyExporter
		{
			private KeyExporter()
			{
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				byte[][] array = contact.TryGetProperty(InternalSchema.UserX509Certificates) as byte[][];
				if (array != null)
				{
					foreach (byte[] array3 in array)
					{
						if (array3 != null)
						{
							writer.StartProperty(PropertyId.Key);
							writer.WriteParameter(ParameterId.Type, "X509");
							writer.WriteParameter(ParameterId.Encoding, "B");
							using (MemoryStream memoryStream = new MemoryStream())
							{
								using (Stream stream = new EncoderStream(new StreamWrapper(memoryStream, false), new Base64Encoder(0), EncoderStreamAccess.Write))
								{
									stream.Write(array3, 0, array3.Length);
								}
								memoryStream.Position = 0L;
								writer.WritePropertyValue(memoryStream);
							}
						}
					}
				}
			}

			public static readonly OutboundVCardConverter.KeyExporter Instance = new OutboundVCardConverter.KeyExporter();
		}

		private class StreamTextExporter : OutboundVCardConverter.PropertyExporter
		{
			static StreamTextExporter()
			{
				OutboundVCardConverter.StreamTextExporter.Instances.Add(new OutboundVCardConverter.StreamTextExporter(InternalSchema.UserText1, "X-MS-TEXT"));
				OutboundVCardConverter.StreamTextExporter.Instances.Add(new OutboundVCardConverter.StreamTextExporter(InternalSchema.UserText2, "X-MS-TEXT"));
				OutboundVCardConverter.StreamTextExporter.Instances.Add(new OutboundVCardConverter.StreamTextExporter(InternalSchema.UserText3, "X-MS-TEXT"));
				OutboundVCardConverter.StreamTextExporter.Instances.Add(new OutboundVCardConverter.StreamTextExporter(InternalSchema.UserText4, "X-MS-TEXT"));
			}

			public static void RegisterAll(List<OutboundVCardConverter.PropertyExporter> list)
			{
				foreach (OutboundVCardConverter.StreamTextExporter item in OutboundVCardConverter.StreamTextExporter.Instances)
				{
					list.Add(item);
				}
			}

			private StreamTextExporter(NativeStorePropertyDefinition prop, string propName)
			{
				this.prop = prop;
				this.propName = propName;
			}

			public override void Export(ContactWriter writer, Contact contact, OutboundVCardConverter.PropertyExporter.Context context)
			{
				object obj = contact.TryGetProperty(this.prop);
				if (obj is string)
				{
					writer.WriteProperty(this.propName, obj as string);
					return;
				}
				if (PropertyError.IsPropertyValueTooBig(obj))
				{
					writer.StartProperty(this.propName);
					using (Stream stream = contact.OpenPropertyStream(this.prop, PropertyOpenMode.ReadOnly))
					{
						using (Stream stream2 = new ConverterStream(stream, new TextToText
						{
							InputEncoding = Encoding.Unicode,
							OutputEncoding = context.Encoding
						}, ConverterStreamAccess.Read))
						{
							writer.WritePropertyValue(stream2);
						}
					}
				}
			}

			public static readonly List<OutboundVCardConverter.StreamTextExporter> Instances = new List<OutboundVCardConverter.StreamTextExporter>();

			private readonly NativeStorePropertyDefinition prop;

			private readonly string propName;
		}
	}
}
