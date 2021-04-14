using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class XmlSerializationReaderUserOofSettingsSerializer : XmlSerializationReader
	{
		public object Read8_UserOofSettings()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_UserOofSettings || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read7_UserOofSettingsSerializer(true, true);
			}
			else
			{
				base.UnknownNode(null, ":UserOofSettings");
			}
			return result;
		}

		private UserOofSettingsSerializer Read7_UserOofSettingsSerializer(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id3_UserOofSettingsSerializer || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			UserOofSettingsSerializer userOofSettingsSerializer = new UserOofSettingsSerializer();
			bool[] array = new bool[7];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(userOofSettingsSerializer);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return userOofSettingsSerializer;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id4_Duration && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.Duration = this.Read2_Duration(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id5_OofState && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.OofState = this.Read3_OofState(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id6_ExternalAudience && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.ExternalAudience = this.Read4_ExternalAudience(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id7_InternalReply && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.InternalReply = this.Read5_ReplyBodySerializer(false, true);
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id8_ExternalReply && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.ExternalReply = this.Read5_ReplyBodySerializer(false, true);
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id9_SetByLegacyClient && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.SetByLegacyClient = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id10_UserChangeTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						userOofSettingsSerializer.UserChangeTime = this.Read6_NullableOfDateTime(true);
						array[6] = true;
					}
					else
					{
						base.UnknownNode(userOofSettingsSerializer, ":Duration, :OofState, :ExternalAudience, :InternalReply, :ExternalReply, :SetByLegacyClient, :UserChangeTime");
					}
				}
				else
				{
					base.UnknownNode(userOofSettingsSerializer, ":Duration, :OofState, :ExternalAudience, :InternalReply, :ExternalReply, :SetByLegacyClient, :UserChangeTime");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return userOofSettingsSerializer;
		}

		private DateTime? Read6_NullableOfDateTime(bool checkType)
		{
			DateTime? result = null;
			if (base.ReadNull())
			{
				return result;
			}
			result = new DateTime?(XmlSerializationReader.ToDateTime(base.Reader.ReadElementString()));
			return result;
		}

		private ReplyBodySerializer Read5_ReplyBodySerializer(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id11_ReplyBodySerializer || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ReplyBodySerializer replyBodySerializer = new ReplyBodySerializer();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(replyBodySerializer);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return replyBodySerializer;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id12_Message && base.Reader.NamespaceURI == this.id2_Item)
					{
						replyBodySerializer.Message = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id13_LanguageTag && base.Reader.NamespaceURI == this.id2_Item)
					{
						replyBodySerializer.LanguageTag = base.Reader.ReadElementString();
						array[1] = true;
					}
					else
					{
						base.UnknownNode(replyBodySerializer, ":Message, :LanguageTag");
					}
				}
				else
				{
					base.UnknownNode(replyBodySerializer, ":Message, :LanguageTag");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return replyBodySerializer;
		}

		private ExternalAudience Read4_ExternalAudience(string s)
		{
			if (s != null)
			{
				if (s == "None")
				{
					return ExternalAudience.None;
				}
				if (s == "Known")
				{
					return ExternalAudience.Known;
				}
				if (s == "All")
				{
					return ExternalAudience.All;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(ExternalAudience));
		}

		private OofState Read3_OofState(string s)
		{
			if (s != null)
			{
				if (s == "Disabled")
				{
					return OofState.Disabled;
				}
				if (s == "Enabled")
				{
					return OofState.Enabled;
				}
				if (s == "Scheduled")
				{
					return OofState.Scheduled;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(OofState));
		}

		private Duration Read2_Duration(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id4_Duration || xmlQualifiedName.Namespace != this.id14_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Duration duration = new Duration();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(duration);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return duration;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id15_StartTime && base.Reader.NamespaceURI == this.id14_Item)
					{
						duration.StartTime = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id16_EndTime && base.Reader.NamespaceURI == this.id14_Item)
					{
						duration.EndTime = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[1] = true;
					}
					else
					{
						base.UnknownNode(duration, "http://schemas.microsoft.com/exchange/services/2006/types:StartTime, http://schemas.microsoft.com/exchange/services/2006/types:EndTime");
					}
				}
				else
				{
					base.UnknownNode(duration, "http://schemas.microsoft.com/exchange/services/2006/types:StartTime, http://schemas.microsoft.com/exchange/services/2006/types:EndTime");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return duration;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id6_ExternalAudience = base.Reader.NameTable.Add("ExternalAudience");
			this.id15_StartTime = base.Reader.NameTable.Add("StartTime");
			this.id8_ExternalReply = base.Reader.NameTable.Add("ExternalReply");
			this.id5_OofState = base.Reader.NameTable.Add("OofState");
			this.id11_ReplyBodySerializer = base.Reader.NameTable.Add("ReplyBodySerializer");
			this.id3_UserOofSettingsSerializer = base.Reader.NameTable.Add("UserOofSettingsSerializer");
			this.id16_EndTime = base.Reader.NameTable.Add("EndTime");
			this.id9_SetByLegacyClient = base.Reader.NameTable.Add("SetByLegacyClient");
			this.id1_UserOofSettings = base.Reader.NameTable.Add("UserOofSettings");
			this.id7_InternalReply = base.Reader.NameTable.Add("InternalReply");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id10_UserChangeTime = base.Reader.NameTable.Add("UserChangeTime");
			this.id14_Item = base.Reader.NameTable.Add("http://schemas.microsoft.com/exchange/services/2006/types");
			this.id4_Duration = base.Reader.NameTable.Add("Duration");
			this.id13_LanguageTag = base.Reader.NameTable.Add("LanguageTag");
			this.id12_Message = base.Reader.NameTable.Add("Message");
		}

		private string id6_ExternalAudience;

		private string id15_StartTime;

		private string id8_ExternalReply;

		private string id5_OofState;

		private string id11_ReplyBodySerializer;

		private string id3_UserOofSettingsSerializer;

		private string id16_EndTime;

		private string id9_SetByLegacyClient;

		private string id1_UserOofSettings;

		private string id7_InternalReply;

		private string id2_Item;

		private string id10_UserChangeTime;

		private string id14_Item;

		private string id4_Duration;

		private string id13_LanguageTag;

		private string id12_Message;
	}
}
