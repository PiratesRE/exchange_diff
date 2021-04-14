using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve
{
	public class XmlSerializationReaderRecipientSyncState : XmlSerializationReader
	{
		public object Read3_RecipientSyncState()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_RecipientSyncState || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read2_RecipientSyncState(true, true);
			}
			else
			{
				base.UnknownNode(null, ":RecipientSyncState");
			}
			return result;
		}

		private RecipientSyncState Read2_RecipientSyncState(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_RecipientSyncState || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			RecipientSyncState recipientSyncState = new RecipientSyncState();
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(recipientSyncState);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return recipientSyncState;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id3_ProxyAddresses && base.Reader.NamespaceURI == this.id2_Item)
					{
						recipientSyncState.ProxyAddresses = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id4_SignupAddresses && base.Reader.NamespaceURI == this.id2_Item)
					{
						recipientSyncState.SignupAddresses = base.Reader.ReadElementString();
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id5_PartnerId && base.Reader.NamespaceURI == this.id2_Item)
					{
						recipientSyncState.PartnerId = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id6_UMProxyAddresses && base.Reader.NamespaceURI == this.id2_Item)
					{
						recipientSyncState.UMProxyAddresses = base.Reader.ReadElementString();
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id7_ArchiveAddress && base.Reader.NamespaceURI == this.id2_Item)
					{
						recipientSyncState.ArchiveAddress = base.Reader.ReadElementString();
						array[4] = true;
					}
					else
					{
						base.UnknownNode(recipientSyncState, ":ProxyAddresses, :SignupAddresses, :PartnerId, :UMProxyAddresses, :ArchiveAddress");
					}
				}
				else
				{
					base.UnknownNode(recipientSyncState, ":ProxyAddresses, :SignupAddresses, :PartnerId, :UMProxyAddresses, :ArchiveAddress");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return recipientSyncState;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id6_UMProxyAddresses = base.Reader.NameTable.Add("UMProxyAddresses");
			this.id3_ProxyAddresses = base.Reader.NameTable.Add("ProxyAddresses");
			this.id4_SignupAddresses = base.Reader.NameTable.Add("SignupAddresses");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id7_ArchiveAddress = base.Reader.NameTable.Add("ArchiveAddress");
			this.id5_PartnerId = base.Reader.NameTable.Add("PartnerId");
			this.id1_RecipientSyncState = base.Reader.NameTable.Add("RecipientSyncState");
		}

		private string id6_UMProxyAddresses;

		private string id3_ProxyAddresses;

		private string id4_SignupAddresses;

		private string id2_Item;

		private string id7_ArchiveAddress;

		private string id5_PartnerId;

		private string id1_RecipientSyncState;
	}
}
