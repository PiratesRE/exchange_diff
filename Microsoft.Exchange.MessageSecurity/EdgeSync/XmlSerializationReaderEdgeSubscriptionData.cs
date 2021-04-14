using System;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class XmlSerializationReaderEdgeSubscriptionData : XmlSerializationReader
	{
		public object Read3_EdgeSubscriptionData()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_EdgeSubscriptionData || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read2_EdgeSubscriptionData(true);
			}
			else
			{
				base.UnknownNode(null, ":EdgeSubscriptionData");
			}
			return result;
		}

		private EdgeSubscriptionData Read2_EdgeSubscriptionData(bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_EdgeSubscriptionData || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			EdgeSubscriptionData edgeSubscriptionData;
			try
			{
				edgeSubscriptionData = (EdgeSubscriptionData)Activator.CreateInstance(typeof(EdgeSubscriptionData), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[0], null);
			}
			catch (MissingMethodException)
			{
				throw base.CreateInaccessibleConstructorException("global::Microsoft.Exchange.MessageSecurity.EdgeSync.EdgeSubscriptionData");
			}
			catch (SecurityException)
			{
				throw base.CreateCtorHasSecurityException("global::Microsoft.Exchange.MessageSecurity.EdgeSync.EdgeSubscriptionData");
			}
			bool[] array = new bool[13];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(edgeSubscriptionData);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return edgeSubscriptionData;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id3_EdgeServerName && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.EdgeServerName = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id4_EdgeServerFQDN && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.EdgeServerFQDN = base.Reader.ReadElementString();
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id5_EdgeCertificateBlob && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.EdgeCertificateBlob = base.ToByteArrayBase64(false);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id6_PfxKPKCertificateBlob && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.PfxKPKCertificateBlob = base.ToByteArrayBase64(false);
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id7_ESRAUsername && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.ESRAUsername = base.Reader.ReadElementString();
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id8_ESRAPassword && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.ESRAPassword = base.Reader.ReadElementString();
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id9_EffectiveDate && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.EffectiveDate = XmlConvert.ToInt64(base.Reader.ReadElementString());
						array[6] = true;
					}
					else if (!array[7] && base.Reader.LocalName == this.id10_Duration && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.Duration = XmlConvert.ToInt64(base.Reader.ReadElementString());
						array[7] = true;
					}
					else if (!array[8] && base.Reader.LocalName == this.id11_AdamSslPort && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.AdamSslPort = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[8] = true;
					}
					else if (!array[9] && base.Reader.LocalName == this.id12_ServerType && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.ServerType = base.Reader.ReadElementString();
						array[9] = true;
					}
					else if (!array[10] && base.Reader.LocalName == this.id13_ProductID && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.ProductID = base.Reader.ReadElementString();
						array[10] = true;
					}
					else if (!array[11] && base.Reader.LocalName == this.id14_VersionNumber && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.VersionNumber = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[11] = true;
					}
					else if (!array[12] && base.Reader.LocalName == this.id15_SerialNumber && base.Reader.NamespaceURI == this.id2_Item)
					{
						edgeSubscriptionData.SerialNumber = base.Reader.ReadElementString();
						array[12] = true;
					}
					else
					{
						base.UnknownNode(edgeSubscriptionData, ":EdgeServerName, :EdgeServerFQDN, :EdgeCertificateBlob, :PfxKPKCertificateBlob, :ESRAUsername, :ESRAPassword, :EffectiveDate, :Duration, :AdamSslPort, :ServerType, :ProductID, :VersionNumber, :SerialNumber");
					}
				}
				else
				{
					base.UnknownNode(edgeSubscriptionData, ":EdgeServerName, :EdgeServerFQDN, :EdgeCertificateBlob, :PfxKPKCertificateBlob, :ESRAUsername, :ESRAPassword, :EffectiveDate, :Duration, :AdamSslPort, :ServerType, :ProductID, :VersionNumber, :SerialNumber");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return edgeSubscriptionData;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id13_ProductID = base.Reader.NameTable.Add("ProductID");
			this.id15_SerialNumber = base.Reader.NameTable.Add("SerialNumber");
			this.id8_ESRAPassword = base.Reader.NameTable.Add("ESRAPassword");
			this.id4_EdgeServerFQDN = base.Reader.NameTable.Add("EdgeServerFQDN");
			this.id12_ServerType = base.Reader.NameTable.Add("ServerType");
			this.id3_EdgeServerName = base.Reader.NameTable.Add("EdgeServerName");
			this.id9_EffectiveDate = base.Reader.NameTable.Add("EffectiveDate");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id5_EdgeCertificateBlob = base.Reader.NameTable.Add("EdgeCertificateBlob");
			this.id14_VersionNumber = base.Reader.NameTable.Add("VersionNumber");
			this.id10_Duration = base.Reader.NameTable.Add("Duration");
			this.id1_EdgeSubscriptionData = base.Reader.NameTable.Add("EdgeSubscriptionData");
			this.id7_ESRAUsername = base.Reader.NameTable.Add("ESRAUsername");
			this.id11_AdamSslPort = base.Reader.NameTable.Add("AdamSslPort");
			this.id6_PfxKPKCertificateBlob = base.Reader.NameTable.Add("PfxKPKCertificateBlob");
		}

		private string id13_ProductID;

		private string id15_SerialNumber;

		private string id8_ESRAPassword;

		private string id4_EdgeServerFQDN;

		private string id12_ServerType;

		private string id3_EdgeServerName;

		private string id9_EffectiveDate;

		private string id2_Item;

		private string id5_EdgeCertificateBlob;

		private string id14_VersionNumber;

		private string id10_Duration;

		private string id1_EdgeSubscriptionData;

		private string id7_ESRAUsername;

		private string id11_AdamSslPort;

		private string id6_PfxKPKCertificateBlob;
	}
}
