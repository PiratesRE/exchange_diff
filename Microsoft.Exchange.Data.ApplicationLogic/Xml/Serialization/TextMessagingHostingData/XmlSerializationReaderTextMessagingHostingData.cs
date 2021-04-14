using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingHostingData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationReaderTextMessagingHostingData : XmlSerializationReader
	{
		public object Read20_TextMessagingHostingData()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_TextMessagingHostingData || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read19_TextMessagingHostingData(false, true);
			}
			else
			{
				base.UnknownNode(null, ":TextMessagingHostingData");
			}
			return result;
		}

		private TextMessagingHostingData Read19_TextMessagingHostingData(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[4] && base.Reader.LocalName == this.id3_Version && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingData.Version = base.Reader.Value;
					array[4] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingData, ":Version");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingData;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id4__locDefinition && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingData._locDefinition = this.Read2_Item(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id5_Regions && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingData.Regions = this.Read4_Item(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id6_Carriers && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingData.Carriers = this.Read7_Item(false, true);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id7_Services && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingData.Services = this.Read18_Item(false, true);
						array[3] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingData, ":_locDefinition, :Regions, :Carriers, :Services");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingData, ":_locDefinition, :Regions, :Carriers, :Services");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingData;
		}

		private TextMessagingHostingDataServices Read18_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServices textMessagingHostingDataServices = new TextMessagingHostingDataServices();
			TextMessagingHostingDataServicesService[] array = null;
			int num = 0;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServices);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				textMessagingHostingDataServices.Service = (TextMessagingHostingDataServicesService[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataServicesService), true);
				return textMessagingHostingDataServices;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num2 = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id8_Service && base.Reader.NamespaceURI == this.id2_Item)
					{
						array = (TextMessagingHostingDataServicesService[])base.EnsureArrayIndex(array, num, typeof(TextMessagingHostingDataServicesService));
						array[num++] = this.Read17_Item(false, true);
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServices, ":Service");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServices, ":Service");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num2, ref readerCount);
			}
			textMessagingHostingDataServices.Service = (TextMessagingHostingDataServicesService[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataServicesService), true);
			base.ReadEndElement();
			return textMessagingHostingDataServices;
		}

		private TextMessagingHostingDataServicesService Read17_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesService textMessagingHostingDataServicesService = new TextMessagingHostingDataServicesService();
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesService);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataServicesService;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_RegionIso2 && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesService.RegionIso2 = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id10_CarrierIdentity && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesService.CarrierIdentity = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id11_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesService.Type = this.Read8_Item(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id12_VoiceCallForwarding && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesService.VoiceCallForwarding = this.Read10_Item(false, true);
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id13_SmtpToSmsGateway && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesService.SmtpToSmsGateway = this.Read16_Item(false, true);
						array[4] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServicesService, ":RegionIso2, :CarrierIdentity, :Type, :VoiceCallForwarding, :SmtpToSmsGateway");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesService, ":RegionIso2, :CarrierIdentity, :Type, :VoiceCallForwarding, :SmtpToSmsGateway");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataServicesService;
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGateway Read16_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesServiceSmtpToSmsGateway textMessagingHostingDataServicesServiceSmtpToSmsGateway = new TextMessagingHostingDataServicesServiceSmtpToSmsGateway();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGateway);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataServicesServiceSmtpToSmsGateway;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id14_RecipientAddressing && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesServiceSmtpToSmsGateway.RecipientAddressing = this.Read11_Item(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id15_MessageRendering && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesServiceSmtpToSmsGateway.MessageRendering = this.Read15_Item(false, true);
						array[1] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGateway, ":RecipientAddressing, :MessageRendering");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGateway, ":RecipientAddressing, :MessageRendering");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataServicesServiceSmtpToSmsGateway;
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering Read15_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering = new TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering();
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[] array = null;
			int num = 0;
			bool[] array2 = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array2[1] && base.Reader.LocalName == this.id16_Container && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering.Container = this.Read14_Item(base.Reader.Value);
					textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering.ContainerSpecified = true;
					array2[1] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering, ":Container");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering.Capacity = (TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity), true);
				return textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num2 = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id17_Capacity && base.Reader.NamespaceURI == this.id2_Item)
					{
						array = (TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[])base.EnsureArrayIndex(array, num, typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity));
						array[num++] = this.Read13_Item(true, true);
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering, ":Capacity");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering, ":Capacity");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num2, ref readerCount);
			}
			textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering.Capacity = (TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity), true);
			base.ReadEndElement();
			return textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering;
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity Read13_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity = new TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id18_CodingScheme && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.CodingScheme = this.Read12_Item(base.Reader.Value);
					array[0] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity, ":CodingScheme");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity, "");
				}
				else if (base.Reader.NodeType == XmlNodeType.Text || base.Reader.NodeType == XmlNodeType.CDATA || base.Reader.NodeType == XmlNodeType.Whitespace || base.Reader.NodeType == XmlNodeType.SignificantWhitespace)
				{
					textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.Value = XmlConvert.ToInt32(base.Reader.ReadString());
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity, "");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity;
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme Read12_Item(string s)
		{
			switch (s)
			{
			case "GsmDefault":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.GsmDefault;
			case "Unicode":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Unicode;
			case "UsAscii":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.UsAscii;
			case "Ia5":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Ia5;
			case "Iso_8859_1":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Iso_8859_1;
			case "Iso_8859_8":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.Iso_8859_8;
			case "ShiftJis":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.ShiftJis;
			case "EucKr":
				return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme.EucKr;
			}
			throw base.CreateUnknownConstantException(s, typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme));
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer Read14_Item(string s)
		{
			if (s != null)
			{
				if (s == "Body")
				{
					return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Body;
				}
				if (s == "Subject")
				{
					return TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Subject;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer));
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing Read11_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing = new TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id19_SmtpAddress && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing.SmtpAddress = base.Reader.ReadElementString();
						array[0] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing, ":SmtpAddress");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing, ":SmtpAddress");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing;
		}

		private TextMessagingHostingDataServicesServiceVoiceCallForwarding Read10_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataServicesServiceVoiceCallForwarding textMessagingHostingDataServicesServiceVoiceCallForwarding = new TextMessagingHostingDataServicesServiceVoiceCallForwarding();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[2] && base.Reader.LocalName == this.id11_Type && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataServicesServiceVoiceCallForwarding.Type = this.Read9_Item(base.Reader.Value);
					textMessagingHostingDataServicesServiceVoiceCallForwarding.TypeSpecified = true;
					array[2] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceVoiceCallForwarding, ":Type");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataServicesServiceVoiceCallForwarding;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id20_Enable && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesServiceVoiceCallForwarding.Enable = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id21_Disable && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataServicesServiceVoiceCallForwarding.Disable = base.Reader.ReadElementString();
						array[1] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataServicesServiceVoiceCallForwarding, ":Enable, :Disable");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataServicesServiceVoiceCallForwarding, ":Enable, :Disable");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataServicesServiceVoiceCallForwarding;
		}

		private TextMessagingHostingDataServicesServiceVoiceCallForwardingType Read9_Item(string s)
		{
			if (s != null)
			{
				if (s == "Conditional")
				{
					return TextMessagingHostingDataServicesServiceVoiceCallForwardingType.Conditional;
				}
				if (s == "Busy")
				{
					return TextMessagingHostingDataServicesServiceVoiceCallForwardingType.Busy;
				}
				if (s == "NoAnswer")
				{
					return TextMessagingHostingDataServicesServiceVoiceCallForwardingType.NoAnswer;
				}
				if (s == "OutOfReach")
				{
					return TextMessagingHostingDataServicesServiceVoiceCallForwardingType.OutOfReach;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(TextMessagingHostingDataServicesServiceVoiceCallForwardingType));
		}

		private TextMessagingHostingDataServicesServiceType Read8_Item(string s)
		{
			if (s != null)
			{
				if (s == "VoiceCallForwarding")
				{
					return TextMessagingHostingDataServicesServiceType.VoiceCallForwarding;
				}
				if (s == "SmtpToSmsGateway")
				{
					return TextMessagingHostingDataServicesServiceType.SmtpToSmsGateway;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(TextMessagingHostingDataServicesServiceType));
		}

		private TextMessagingHostingDataCarriers Read7_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataCarriers textMessagingHostingDataCarriers = new TextMessagingHostingDataCarriers();
			TextMessagingHostingDataCarriersCarrier[] array = null;
			int num = 0;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataCarriers);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				textMessagingHostingDataCarriers.Carrier = (TextMessagingHostingDataCarriersCarrier[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataCarriersCarrier), true);
				return textMessagingHostingDataCarriers;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num2 = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id22_Carrier && base.Reader.NamespaceURI == this.id2_Item)
					{
						array = (TextMessagingHostingDataCarriersCarrier[])base.EnsureArrayIndex(array, num, typeof(TextMessagingHostingDataCarriersCarrier));
						array[num++] = this.Read6_Item(false, true);
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataCarriers, ":Carrier");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataCarriers, ":Carrier");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num2, ref readerCount);
			}
			textMessagingHostingDataCarriers.Carrier = (TextMessagingHostingDataCarriersCarrier[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataCarriersCarrier), true);
			base.ReadEndElement();
			return textMessagingHostingDataCarriers;
		}

		private TextMessagingHostingDataCarriersCarrier Read6_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataCarriersCarrier textMessagingHostingDataCarriersCarrier = new TextMessagingHostingDataCarriersCarrier();
			TextMessagingHostingDataCarriersCarrierLocalizedInfo[] array = null;
			int num = 0;
			bool[] array2 = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array2[1] && base.Reader.LocalName == this.id23_Identity && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataCarriersCarrier.Identity = XmlConvert.ToInt32(base.Reader.Value);
					array2[1] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataCarriersCarrier, ":Identity");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				textMessagingHostingDataCarriersCarrier.LocalizedInfo = (TextMessagingHostingDataCarriersCarrierLocalizedInfo[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataCarriersCarrierLocalizedInfo), true);
				return textMessagingHostingDataCarriersCarrier;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num2 = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id24_LocalizedInfo && base.Reader.NamespaceURI == this.id2_Item)
					{
						array = (TextMessagingHostingDataCarriersCarrierLocalizedInfo[])base.EnsureArrayIndex(array, num, typeof(TextMessagingHostingDataCarriersCarrierLocalizedInfo));
						array[num++] = this.Read5_Item(false, true);
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataCarriersCarrier, ":LocalizedInfo");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataCarriersCarrier, ":LocalizedInfo");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num2, ref readerCount);
			}
			textMessagingHostingDataCarriersCarrier.LocalizedInfo = (TextMessagingHostingDataCarriersCarrierLocalizedInfo[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataCarriersCarrierLocalizedInfo), true);
			base.ReadEndElement();
			return textMessagingHostingDataCarriersCarrier;
		}

		private TextMessagingHostingDataCarriersCarrierLocalizedInfo Read5_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataCarriersCarrierLocalizedInfo textMessagingHostingDataCarriersCarrierLocalizedInfo = new TextMessagingHostingDataCarriersCarrierLocalizedInfo();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[1] && base.Reader.LocalName == this.id25_Culture && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataCarriersCarrierLocalizedInfo.Culture = base.Reader.Value;
					array[1] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataCarriersCarrierLocalizedInfo, ":Culture");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataCarriersCarrierLocalizedInfo;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id26_DisplayName && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataCarriersCarrierLocalizedInfo.DisplayName = base.Reader.ReadElementString();
						array[0] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataCarriersCarrierLocalizedInfo, ":DisplayName");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataCarriersCarrierLocalizedInfo, ":DisplayName");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataCarriersCarrierLocalizedInfo;
		}

		private TextMessagingHostingDataRegions Read4_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataRegions textMessagingHostingDataRegions = new TextMessagingHostingDataRegions();
			TextMessagingHostingDataRegionsRegion[] array = null;
			int num = 0;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataRegions);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				textMessagingHostingDataRegions.Region = (TextMessagingHostingDataRegionsRegion[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataRegionsRegion), true);
				return textMessagingHostingDataRegions;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num2 = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id27_Region && base.Reader.NamespaceURI == this.id2_Item)
					{
						array = (TextMessagingHostingDataRegionsRegion[])base.EnsureArrayIndex(array, num, typeof(TextMessagingHostingDataRegionsRegion));
						array[num++] = this.Read3_Item(false, true);
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataRegions, ":Region");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataRegions, ":Region");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num2, ref readerCount);
			}
			textMessagingHostingDataRegions.Region = (TextMessagingHostingDataRegionsRegion[])base.ShrinkArray(array, num, typeof(TextMessagingHostingDataRegionsRegion), true);
			base.ReadEndElement();
			return textMessagingHostingDataRegions;
		}

		private TextMessagingHostingDataRegionsRegion Read3_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingDataRegionsRegion textMessagingHostingDataRegionsRegion = new TextMessagingHostingDataRegionsRegion();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[2] && base.Reader.LocalName == this.id28_Iso2 && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingHostingDataRegionsRegion.Iso2 = base.Reader.Value;
					array[2] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingDataRegionsRegion, ":Iso2");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingDataRegionsRegion;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id29_CountryCode && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataRegionsRegion.CountryCode = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id30_PhoneNumberExample && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingDataRegionsRegion.PhoneNumberExample = base.Reader.ReadElementString();
						array[1] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingDataRegionsRegion, ":CountryCode, :PhoneNumberExample");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingDataRegionsRegion, ":CountryCode, :PhoneNumberExample");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingDataRegionsRegion;
		}

		private TextMessagingHostingData_locDefinition Read2_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id2_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingHostingData_locDefinition textMessagingHostingData_locDefinition = new TextMessagingHostingData_locDefinition();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingHostingData_locDefinition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingHostingData_locDefinition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id31__locDefault_loc && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingHostingData_locDefinition._locDefault_loc = base.Reader.ReadElementString();
						array[0] = true;
					}
					else
					{
						base.UnknownNode(textMessagingHostingData_locDefinition, ":_locDefault_loc");
					}
				}
				else
				{
					base.UnknownNode(textMessagingHostingData_locDefinition, ":_locDefault_loc");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingHostingData_locDefinition;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id9_RegionIso2 = base.Reader.NameTable.Add("RegionIso2");
			this.id21_Disable = base.Reader.NameTable.Add("Disable");
			this.id28_Iso2 = base.Reader.NameTable.Add("Iso2");
			this.id17_Capacity = base.Reader.NameTable.Add("Capacity");
			this.id3_Version = base.Reader.NameTable.Add("Version");
			this.id27_Region = base.Reader.NameTable.Add("Region");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id25_Culture = base.Reader.NameTable.Add("Culture");
			this.id23_Identity = base.Reader.NameTable.Add("Identity");
			this.id18_CodingScheme = base.Reader.NameTable.Add("CodingScheme");
			this.id22_Carrier = base.Reader.NameTable.Add("Carrier");
			this.id16_Container = base.Reader.NameTable.Add("Container");
			this.id11_Type = base.Reader.NameTable.Add("Type");
			this.id29_CountryCode = base.Reader.NameTable.Add("CountryCode");
			this.id24_LocalizedInfo = base.Reader.NameTable.Add("LocalizedInfo");
			this.id15_MessageRendering = base.Reader.NameTable.Add("MessageRendering");
			this.id8_Service = base.Reader.NameTable.Add("Service");
			this.id14_RecipientAddressing = base.Reader.NameTable.Add("RecipientAddressing");
			this.id5_Regions = base.Reader.NameTable.Add("Regions");
			this.id20_Enable = base.Reader.NameTable.Add("Enable");
			this.id6_Carriers = base.Reader.NameTable.Add("Carriers");
			this.id4__locDefinition = base.Reader.NameTable.Add("_locDefinition");
			this.id10_CarrierIdentity = base.Reader.NameTable.Add("CarrierIdentity");
			this.id26_DisplayName = base.Reader.NameTable.Add("DisplayName");
			this.id1_TextMessagingHostingData = base.Reader.NameTable.Add("TextMessagingHostingData");
			this.id13_SmtpToSmsGateway = base.Reader.NameTable.Add("SmtpToSmsGateway");
			this.id7_Services = base.Reader.NameTable.Add("Services");
			this.id31__locDefault_loc = base.Reader.NameTable.Add("_locDefault_loc");
			this.id30_PhoneNumberExample = base.Reader.NameTable.Add("PhoneNumberExample");
			this.id12_VoiceCallForwarding = base.Reader.NameTable.Add("VoiceCallForwarding");
			this.id19_SmtpAddress = base.Reader.NameTable.Add("SmtpAddress");
		}

		private string id9_RegionIso2;

		private string id21_Disable;

		private string id28_Iso2;

		private string id17_Capacity;

		private string id3_Version;

		private string id27_Region;

		private string id2_Item;

		private string id25_Culture;

		private string id23_Identity;

		private string id18_CodingScheme;

		private string id22_Carrier;

		private string id16_Container;

		private string id11_Type;

		private string id29_CountryCode;

		private string id24_LocalizedInfo;

		private string id15_MessageRendering;

		private string id8_Service;

		private string id14_RecipientAddressing;

		private string id5_Regions;

		private string id20_Enable;

		private string id6_Carriers;

		private string id4__locDefinition;

		private string id10_CarrierIdentity;

		private string id26_DisplayName;

		private string id1_TextMessagingHostingData;

		private string id13_SmtpToSmsGateway;

		private string id7_Services;

		private string id31__locDefault_loc;

		private string id30_PhoneNumberExample;

		private string id12_VoiceCallForwarding;

		private string id19_SmtpAddress;
	}
}
