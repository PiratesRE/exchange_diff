using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class XmlSerializationWriterEdgeSubscriptionData : XmlSerializationWriter
	{
		public void Write3_EdgeSubscriptionData(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("EdgeSubscriptionData", "");
				return;
			}
			this.Write2_EdgeSubscriptionData("EdgeSubscriptionData", "", (EdgeSubscriptionData)o, false);
		}

		private void Write2_EdgeSubscriptionData(string n, string ns, EdgeSubscriptionData o, bool needType)
		{
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(EdgeSubscriptionData)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("EdgeSubscriptionData", "");
			}
			base.WriteElementString("EdgeServerName", "", o.EdgeServerName);
			base.WriteElementString("EdgeServerFQDN", "", o.EdgeServerFQDN);
			base.WriteElementStringRaw("EdgeCertificateBlob", "", XmlSerializationWriter.FromByteArrayBase64(o.EdgeCertificateBlob));
			base.WriteElementStringRaw("PfxKPKCertificateBlob", "", XmlSerializationWriter.FromByteArrayBase64(o.PfxKPKCertificateBlob));
			base.WriteElementString("ESRAUsername", "", o.ESRAUsername);
			base.WriteElementString("ESRAPassword", "", o.ESRAPassword);
			base.WriteElementStringRaw("EffectiveDate", "", XmlConvert.ToString(o.EffectiveDate));
			base.WriteElementStringRaw("Duration", "", XmlConvert.ToString(o.Duration));
			base.WriteElementStringRaw("AdamSslPort", "", XmlConvert.ToString(o.AdamSslPort));
			base.WriteElementString("ServerType", "", o.ServerType);
			base.WriteElementString("ProductID", "", o.ProductID);
			base.WriteElementStringRaw("VersionNumber", "", XmlConvert.ToString(o.VersionNumber));
			base.WriteElementString("SerialNumber", "", o.SerialNumber);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
