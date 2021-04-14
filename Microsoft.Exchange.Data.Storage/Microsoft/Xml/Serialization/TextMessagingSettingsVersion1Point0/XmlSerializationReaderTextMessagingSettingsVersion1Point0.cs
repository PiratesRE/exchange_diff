using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationReaderTextMessagingSettingsVersion1Point0 : XmlSerializationReader
	{
		public object Read9_TextMessagingSettings()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_TextMessagingSettings || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read8_Item(true, true);
			}
			else
			{
				base.UnknownNode(null, ":TextMessagingSettings");
			}
			return result;
		}

		private TextMessagingSettingsVersion1Point0 Read8_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id3_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = new TextMessagingSettingsVersion1Point0();
			if (textMessagingSettingsVersion1Point.DeliveryPoints == null)
			{
				textMessagingSettingsVersion1Point.DeliveryPoints = new List<DeliveryPoint>();
			}
			List<DeliveryPoint> deliveryPoints = textMessagingSettingsVersion1Point.DeliveryPoints;
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id4_Version && base.Reader.NamespaceURI == this.id2_Item)
				{
					textMessagingSettingsVersion1Point.Version = base.Reader.Value;
					array[0] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(textMessagingSettingsVersion1Point, ":Version");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return textMessagingSettingsVersion1Point;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[1] && base.Reader.LocalName == this.id5_Item && base.Reader.NamespaceURI == this.id2_Item)
					{
						textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies = this.Read5_Item(false, true);
						array[1] = true;
					}
					else if (base.Reader.LocalName == this.id6_DeliveryPoint && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (deliveryPoints == null)
						{
							base.Reader.Skip();
						}
						else
						{
							deliveryPoints.Add(this.Read7_DeliveryPoint(false, true));
						}
					}
					else
					{
						base.UnknownNode(textMessagingSettingsVersion1Point, ":MachineToPersonMessagingPolicies, :DeliveryPoint");
					}
				}
				else
				{
					base.UnknownNode(textMessagingSettingsVersion1Point, ":MachineToPersonMessagingPolicies, :DeliveryPoint");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return textMessagingSettingsVersion1Point;
		}

		private DeliveryPoint Read7_DeliveryPoint(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id6_DeliveryPoint || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			DeliveryPoint deliveryPoint = new DeliveryPoint();
			bool[] array = new bool[9];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(deliveryPoint);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return deliveryPoint;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id7_Identity && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.Identity = XmlConvert.ToByte(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id8_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.Type = this.Read6_DeliveryPointType(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id9_PhoneNumber && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.PhoneNumber = (E164Number)base.ReadSerializable(new E164Number());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id10_Protocol && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.Protocol = base.Reader.ReadElementString();
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id11_DeviceType && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.DeviceType = base.Reader.ReadElementString();
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id12_DeviceId && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.DeviceId = base.Reader.ReadElementString();
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id13_DeviceFriendlyName && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.DeviceFriendlyName = base.Reader.ReadElementString();
						array[6] = true;
					}
					else if (!array[7] && base.Reader.LocalName == this.id14_P2pMessaginPriority && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.P2pMessagingPriority = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[7] = true;
					}
					else if (!array[8] && base.Reader.LocalName == this.id15_M2pMessagingPriority && base.Reader.NamespaceURI == this.id2_Item)
					{
						deliveryPoint.M2pMessagingPriority = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[8] = true;
					}
					else
					{
						base.UnknownNode(deliveryPoint, ":Identity, :Type, :PhoneNumber, :Protocol, :DeviceType, :DeviceId, :DeviceFriendlyName, :P2pMessaginPriority, :M2pMessagingPriority");
					}
				}
				else
				{
					base.UnknownNode(deliveryPoint, ":Identity, :Type, :PhoneNumber, :Protocol, :DeviceType, :DeviceId, :DeviceFriendlyName, :P2pMessaginPriority, :M2pMessagingPriority");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return deliveryPoint;
		}

		private DeliveryPointType Read6_DeliveryPointType(string s)
		{
			if (s != null)
			{
				if (s == "Unknown")
				{
					return DeliveryPointType.Unknown;
				}
				if (s == "ExchangeActiveSync")
				{
					return DeliveryPointType.ExchangeActiveSync;
				}
				if (s == "SmtpToSmsGateway")
				{
					return DeliveryPointType.SmtpToSmsGateway;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(DeliveryPointType));
		}

		private MachineToPersonMessagingPolicies Read5_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id5_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			MachineToPersonMessagingPolicies machineToPersonMessagingPolicies = new MachineToPersonMessagingPolicies();
			if (machineToPersonMessagingPolicies.PossibleRecipients == null)
			{
				machineToPersonMessagingPolicies.PossibleRecipients = new List<PossibleRecipient>();
			}
			List<PossibleRecipient> possibleRecipients = machineToPersonMessagingPolicies.PossibleRecipients;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(machineToPersonMessagingPolicies);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return machineToPersonMessagingPolicies;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id16_PossibleRecipient && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (possibleRecipients == null)
						{
							base.Reader.Skip();
						}
						else
						{
							possibleRecipients.Add(this.Read4_PossibleRecipient(false, true));
						}
					}
					else
					{
						base.UnknownNode(machineToPersonMessagingPolicies, ":PossibleRecipient");
					}
				}
				else
				{
					base.UnknownNode(machineToPersonMessagingPolicies, ":PossibleRecipient");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return machineToPersonMessagingPolicies;
		}

		private PossibleRecipient Read4_PossibleRecipient(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id16_PossibleRecipient || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			PossibleRecipient possibleRecipient = new PossibleRecipient();
			if (possibleRecipient.PasscodeSentTimeHistory == null)
			{
				possibleRecipient.PasscodeSentTimeHistory = new List<DateTime>();
			}
			List<DateTime> passcodeSentTimeHistory = possibleRecipient.PasscodeSentTimeHistory;
			if (possibleRecipient.PasscodeVerificationFailedTimeHistory == null)
			{
				possibleRecipient.PasscodeVerificationFailedTimeHistory = new List<DateTime>();
			}
			List<DateTime> passcodeVerificationFailedTimeHistory = possibleRecipient.PasscodeVerificationFailedTimeHistory;
			bool[] array = new bool[10];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(possibleRecipient);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return possibleRecipient;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id17_Effective && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.Effective = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id18_EffectiveLastModificationTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.EffectiveLastModificationTime = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id19_Region && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.Region = base.Reader.ReadElementString();
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id20_Carrier && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.Carrier = base.Reader.ReadElementString();
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id9_PhoneNumber && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.PhoneNumber = (E164Number)base.ReadSerializable(new E164Number());
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id21_PhoneNumberSetTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.PhoneNumberSetTime = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id22_Acknowledged && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.Acknowledged = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[6] = true;
					}
					else if (!array[7] && base.Reader.LocalName == this.id23_Passcode && base.Reader.NamespaceURI == this.id2_Item)
					{
						possibleRecipient.Passcode = base.Reader.ReadElementString();
						array[7] = true;
					}
					else if (base.Reader.LocalName == this.id24_PasscodeSentTimeHistory && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							if (possibleRecipient.PasscodeSentTimeHistory == null)
							{
								possibleRecipient.PasscodeSentTimeHistory = new List<DateTime>();
							}
							List<DateTime> passcodeSentTimeHistory2 = possibleRecipient.PasscodeSentTimeHistory;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num2 = 0;
								int readerCount2 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id25_SentTime && base.Reader.NamespaceURI == this.id2_Item)
										{
											passcodeSentTimeHistory2.Add(XmlSerializationReader.ToDateTime(base.Reader.ReadElementString()));
										}
										else
										{
											base.UnknownNode(null, ":SentTime");
										}
									}
									else
									{
										base.UnknownNode(null, ":SentTime");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num2, ref readerCount2);
								}
								base.ReadEndElement();
							}
						}
					}
					else if (base.Reader.LocalName == this.id26_Item && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							if (possibleRecipient.PasscodeVerificationFailedTimeHistory == null)
							{
								possibleRecipient.PasscodeVerificationFailedTimeHistory = new List<DateTime>();
							}
							List<DateTime> passcodeVerificationFailedTimeHistory2 = possibleRecipient.PasscodeVerificationFailedTimeHistory;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num3 = 0;
								int readerCount3 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id27_FailedTime && base.Reader.NamespaceURI == this.id2_Item)
										{
											passcodeVerificationFailedTimeHistory2.Add(XmlSerializationReader.ToDateTime(base.Reader.ReadElementString()));
										}
										else
										{
											base.UnknownNode(null, ":FailedTime");
										}
									}
									else
									{
										base.UnknownNode(null, ":FailedTime");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num3, ref readerCount3);
								}
								base.ReadEndElement();
							}
						}
					}
					else
					{
						base.UnknownNode(possibleRecipient, ":Effective, :EffectiveLastModificationTime, :Region, :Carrier, :PhoneNumber, :PhoneNumberSetTime, :Acknowledged, :Passcode, :PasscodeSentTimeHistory, :PasscodeVerificationFailedTimeHistory");
					}
				}
				else
				{
					base.UnknownNode(possibleRecipient, ":Effective, :EffectiveLastModificationTime, :Region, :Carrier, :PhoneNumber, :PhoneNumberSetTime, :Acknowledged, :Passcode, :PasscodeSentTimeHistory, :PasscodeVerificationFailedTimeHistory");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return possibleRecipient;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id16_PossibleRecipient = base.Reader.NameTable.Add("PossibleRecipient");
			this.id5_Item = base.Reader.NameTable.Add("MachineToPersonMessagingPolicies");
			this.id4_Version = base.Reader.NameTable.Add("Version");
			this.id27_FailedTime = base.Reader.NameTable.Add("FailedTime");
			this.id12_DeviceId = base.Reader.NameTable.Add("DeviceId");
			this.id3_Item = base.Reader.NameTable.Add("TextMessagingSettingsVersion1Point0");
			this.id7_Identity = base.Reader.NameTable.Add("Identity");
			this.id20_Carrier = base.Reader.NameTable.Add("Carrier");
			this.id19_Region = base.Reader.NameTable.Add("Region");
			this.id23_Passcode = base.Reader.NameTable.Add("Passcode");
			this.id8_Type = base.Reader.NameTable.Add("Type");
			this.id17_Effective = base.Reader.NameTable.Add("Effective");
			this.id15_M2pMessagingPriority = base.Reader.NameTable.Add("M2pMessagingPriority");
			this.id1_TextMessagingSettings = base.Reader.NameTable.Add("TextMessagingSettings");
			this.id22_Acknowledged = base.Reader.NameTable.Add("Acknowledged");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id14_P2pMessaginPriority = base.Reader.NameTable.Add("P2pMessaginPriority");
			this.id6_DeliveryPoint = base.Reader.NameTable.Add("DeliveryPoint");
			this.id24_PasscodeSentTimeHistory = base.Reader.NameTable.Add("PasscodeSentTimeHistory");
			this.id11_DeviceType = base.Reader.NameTable.Add("DeviceType");
			this.id26_Item = base.Reader.NameTable.Add("PasscodeVerificationFailedTimeHistory");
			this.id18_EffectiveLastModificationTime = base.Reader.NameTable.Add("EffectiveLastModificationTime");
			this.id13_DeviceFriendlyName = base.Reader.NameTable.Add("DeviceFriendlyName");
			this.id10_Protocol = base.Reader.NameTable.Add("Protocol");
			this.id21_PhoneNumberSetTime = base.Reader.NameTable.Add("PhoneNumberSetTime");
			this.id25_SentTime = base.Reader.NameTable.Add("SentTime");
			this.id9_PhoneNumber = base.Reader.NameTable.Add("PhoneNumber");
		}

		private string id16_PossibleRecipient;

		private string id5_Item;

		private string id4_Version;

		private string id27_FailedTime;

		private string id12_DeviceId;

		private string id3_Item;

		private string id7_Identity;

		private string id20_Carrier;

		private string id19_Region;

		private string id23_Passcode;

		private string id8_Type;

		private string id17_Effective;

		private string id15_M2pMessagingPriority;

		private string id1_TextMessagingSettings;

		private string id22_Acknowledged;

		private string id2_Item;

		private string id14_P2pMessaginPriority;

		private string id6_DeliveryPoint;

		private string id24_PasscodeSentTimeHistory;

		private string id11_DeviceType;

		private string id26_Item;

		private string id18_EffectiveLastModificationTime;

		private string id13_DeviceFriendlyName;

		private string id10_Protocol;

		private string id21_PhoneNumberSetTime;

		private string id25_SentTime;

		private string id9_PhoneNumber;
	}
}
