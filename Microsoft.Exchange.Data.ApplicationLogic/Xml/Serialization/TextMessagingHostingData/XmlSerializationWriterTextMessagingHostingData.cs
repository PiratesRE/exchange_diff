using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingHostingData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationWriterTextMessagingHostingData : XmlSerializationWriter
	{
		public void Write20_TextMessagingHostingData(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("TextMessagingHostingData", "");
				return;
			}
			base.TopLevelElement();
			this.Write19_TextMessagingHostingData("TextMessagingHostingData", "", (TextMessagingHostingData)o, false, false);
		}

		private void Write19_TextMessagingHostingData(string n, string ns, TextMessagingHostingData o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingData)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteAttribute("Version", "", o.Version);
			this.Write2_Item("_locDefinition", "", o._locDefinition, false, false);
			this.Write4_Item("Regions", "", o.Regions, false, false);
			this.Write7_Item("Carriers", "", o.Carriers, false, false);
			this.Write18_Item("Services", "", o.Services, false, false);
			base.WriteEndElement(o);
		}

		private void Write18_Item(string n, string ns, TextMessagingHostingDataServices o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServices)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			TextMessagingHostingDataServicesService[] service = o.Service;
			if (service != null)
			{
				for (int i = 0; i < service.Length; i++)
				{
					this.Write17_Item("Service", "", service[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write17_Item(string n, string ns, TextMessagingHostingDataServicesService o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesService)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteElementString("RegionIso2", "", o.RegionIso2);
			base.WriteElementStringRaw("CarrierIdentity", "", XmlConvert.ToString(o.CarrierIdentity));
			base.WriteElementString("Type", "", this.Write8_Item(o.Type));
			this.Write10_Item("VoiceCallForwarding", "", o.VoiceCallForwarding, false, false);
			this.Write16_Item("SmtpToSmsGateway", "", o.SmtpToSmsGateway, false, false);
			base.WriteEndElement(o);
		}

		private void Write16_Item(string n, string ns, TextMessagingHostingDataServicesServiceSmtpToSmsGateway o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGateway)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			this.Write11_Item("RecipientAddressing", "", o.RecipientAddressing, false, false);
			this.Write15_Item("MessageRendering", "", o.MessageRendering, false, false);
			base.WriteEndElement(o);
		}

		private void Write15_Item(string n, string ns, TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			if (o.ContainerSpecified)
			{
				base.WriteAttribute("Container", "", this.Write14_Item(o.Container));
			}
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[] capacity = o.Capacity;
			if (capacity != null)
			{
				for (int i = 0; i < capacity.Length; i++)
				{
					this.Write13_Item("Capacity", "", capacity[i], true, false);
				}
			}
			bool containerSpecified = o.ContainerSpecified;
			base.WriteEndElement(o);
		}

		private void Write13_Item(string n, string ns, TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteAttribute("CodingScheme", "", this.Write12_Item(o.CodingScheme));
			base.WriteValue(XmlConvert.ToString(o.Value));
			base.WriteEndElement(o);
		}

		private string Write12_Item(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme v)
		{
			string result;
			switch (v)
			{
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.GsmDefault:
				result = "GsmDefault";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Unicode:
				result = "Unicode";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.UsAscii:
				result = "UsAscii";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Ia5:
				result = "Ia5";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Iso_8859_1:
				result = "Iso_8859_1";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Iso_8859_8:
				result = "Iso_8859_8";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.ShiftJis:
				result = "ShiftJis";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.EucKr:
				result = "EucKr";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme");
			}
			return result;
		}

		private string Write14_Item(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer v)
		{
			string result;
			switch (v)
			{
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Body:
				result = "Body";
				break;
			case TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Subject:
				result = "Subject";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer");
			}
			return result;
		}

		private void Write11_Item(string n, string ns, TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteElementString("SmtpAddress", "", o.SmtpAddress);
			base.WriteEndElement(o);
		}

		private void Write10_Item(string n, string ns, TextMessagingHostingDataServicesServiceVoiceCallForwarding o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataServicesServiceVoiceCallForwarding)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			if (o.TypeSpecified)
			{
				base.WriteAttribute("Type", "", this.Write9_Item(o.Type));
			}
			base.WriteElementString("Enable", "", o.Enable);
			base.WriteElementString("Disable", "", o.Disable);
			bool typeSpecified = o.TypeSpecified;
			base.WriteEndElement(o);
		}

		private string Write9_Item(TextMessagingHostingDataServicesServiceVoiceCallForwardingType v)
		{
			string result;
			switch (v)
			{
			case TextMessagingHostingDataServicesServiceVoiceCallForwardingType.Conditional:
				result = "Conditional";
				break;
			case TextMessagingHostingDataServicesServiceVoiceCallForwardingType.Busy:
				result = "Busy";
				break;
			case TextMessagingHostingDataServicesServiceVoiceCallForwardingType.NoAnswer:
				result = "NoAnswer";
				break;
			case TextMessagingHostingDataServicesServiceVoiceCallForwardingType.OutOfReach:
				result = "OutOfReach";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingDataServicesServiceVoiceCallForwardingType");
			}
			return result;
		}

		private string Write8_Item(TextMessagingHostingDataServicesServiceType v)
		{
			string result;
			switch (v)
			{
			case TextMessagingHostingDataServicesServiceType.VoiceCallForwarding:
				result = "VoiceCallForwarding";
				break;
			case TextMessagingHostingDataServicesServiceType.SmtpToSmsGateway:
				result = "SmtpToSmsGateway";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingDataServicesServiceType");
			}
			return result;
		}

		private void Write7_Item(string n, string ns, TextMessagingHostingDataCarriers o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataCarriers)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			TextMessagingHostingDataCarriersCarrier[] carrier = o.Carrier;
			if (carrier != null)
			{
				for (int i = 0; i < carrier.Length; i++)
				{
					this.Write6_Item("Carrier", "", carrier[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write6_Item(string n, string ns, TextMessagingHostingDataCarriersCarrier o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataCarriersCarrier)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteAttribute("Identity", "", XmlConvert.ToString(o.Identity));
			TextMessagingHostingDataCarriersCarrierLocalizedInfo[] localizedInfo = o.LocalizedInfo;
			if (localizedInfo != null)
			{
				for (int i = 0; i < localizedInfo.Length; i++)
				{
					this.Write5_Item("LocalizedInfo", "", localizedInfo[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write5_Item(string n, string ns, TextMessagingHostingDataCarriersCarrierLocalizedInfo o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataCarriersCarrierLocalizedInfo)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteAttribute("Culture", "", o.Culture);
			base.WriteElementString("DisplayName", "", o.DisplayName);
			base.WriteEndElement(o);
		}

		private void Write4_Item(string n, string ns, TextMessagingHostingDataRegions o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataRegions)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			TextMessagingHostingDataRegionsRegion[] region = o.Region;
			if (region != null)
			{
				for (int i = 0; i < region.Length; i++)
				{
					this.Write3_Item("Region", "", region[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write3_Item(string n, string ns, TextMessagingHostingDataRegionsRegion o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingDataRegionsRegion)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteAttribute("Iso2", "", o.Iso2);
			base.WriteElementString("CountryCode", "", o.CountryCode);
			base.WriteElementString("PhoneNumberExample", "", o.PhoneNumberExample);
			base.WriteEndElement(o);
		}

		private void Write2_Item(string n, string ns, TextMessagingHostingData_locDefinition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(TextMessagingHostingData_locDefinition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "");
			}
			base.WriteElementString("_locDefault_loc", "", o._locDefault_loc);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
