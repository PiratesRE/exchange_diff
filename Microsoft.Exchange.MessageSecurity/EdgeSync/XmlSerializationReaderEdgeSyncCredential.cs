using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class XmlSerializationReaderEdgeSyncCredential : XmlSerializationReader
	{
		public object Read3_EdgeSyncCredential()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_EdgeSyncCredential || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read2_EdgeSyncCredential(true, true);
			}
			else
			{
				base.UnknownNode(null, ":EdgeSyncCredential");
			}
			return result;
		}

		private EdgeSyncCredential Read2_EdgeSyncCredential(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_EdgeSyncCredential || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			EdgeSyncCredential edgeSyncCredential = new EdgeSyncCredential();
			bool[] array = new bool[7];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(edgeSyncCredential);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return edgeSyncCredential;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id3_EdgeServerFQDN && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.EdgeServerFQDN = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id4_ESRAUsername && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.ESRAUsername = base.Reader.ReadElementString();
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id5_EncryptedESRAPassword && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.EncryptedESRAPassword = base.ToByteArrayBase64(false);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id6_EdgeEncryptedESRAPassword && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.EdgeEncryptedESRAPassword = base.ToByteArrayBase64(false);
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id7_EffectiveDate && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.EffectiveDate = XmlConvert.ToInt64(base.Reader.ReadElementString());
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id8_Duration && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.Duration = XmlConvert.ToInt64(base.Reader.ReadElementString());
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id9_IsBootStrapAccount && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSyncCredential.IsBootStrapAccount = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[6] = true;
					}
					else
					{
						base.UnknownNode(edgeSyncCredential, ":EdgeServerFQDN, :ESRAUsername, :EncryptedESRAPassword, :EdgeEncryptedESRAPassword, :EffectiveDate, :Duration, :IsBootStrapAccount");
					}
				}
				else
				{
					base.UnknownNode(edgeSyncCredential, ":EdgeServerFQDN, :ESRAUsername, :EncryptedESRAPassword, :EdgeEncryptedESRAPassword, :EffectiveDate, :Duration, :IsBootStrapAccount");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return edgeSyncCredential;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id9_IsBootStrapAccount = base.Reader.NameTable.Add("IsBootStrapAccount");
			this.id3_EdgeServerFQDN = base.Reader.NameTable.Add("EdgeServerFQDN");
			this.id8_Duration = base.Reader.NameTable.Add("Duration");
			this.id7_EffectiveDate = base.Reader.NameTable.Add("EffectiveDate");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id6_EdgeEncryptedESRAPassword = base.Reader.NameTable.Add("EdgeEncryptedESRAPassword");
			this.id5_EncryptedESRAPassword = base.Reader.NameTable.Add("EncryptedESRAPassword");
			this.id4_ESRAUsername = base.Reader.NameTable.Add("ESRAUsername");
			this.id1_EdgeSyncCredential = base.Reader.NameTable.Add("EdgeSyncCredential");
		}

		private string id9_IsBootStrapAccount;

		private string id3_EdgeServerFQDN;

		private string id8_Duration;

		private string id7_EffectiveDate;

		private string id2_Item;

		private string id6_EdgeEncryptedESRAPassword;

		private string id5_EncryptedESRAPassword;

		private string id4_ESRAUsername;

		private string id1_EdgeSyncCredential;
	}
}
