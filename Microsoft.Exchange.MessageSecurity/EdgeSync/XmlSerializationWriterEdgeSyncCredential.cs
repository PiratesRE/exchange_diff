using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class XmlSerializationWriterEdgeSyncCredential : XmlSerializationWriter
	{
		public void Write3_EdgeSyncCredential(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("EdgeSyncCredential", "");
				return;
			}
			base.TopLevelElement();
			this.Write2_EdgeSyncCredential("EdgeSyncCredential", "", (EdgeSyncCredential)o, true, false);
		}

		private void Write2_EdgeSyncCredential(string n, string ns, EdgeSyncCredential o, bool isNullable, bool needType)
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
				if (!(type == typeof(EdgeSyncCredential)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("EdgeSyncCredential", "");
			}
			base.WriteElementString("EdgeServerFQDN", "", o.EdgeServerFQDN);
			base.WriteElementString("ESRAUsername", "", o.ESRAUsername);
			base.WriteElementStringRaw("EncryptedESRAPassword", "", XmlSerializationWriter.FromByteArrayBase64(o.EncryptedESRAPassword));
			base.WriteElementStringRaw("EdgeEncryptedESRAPassword", "", XmlSerializationWriter.FromByteArrayBase64(o.EdgeEncryptedESRAPassword));
			base.WriteElementStringRaw("EffectiveDate", "", XmlConvert.ToString(o.EffectiveDate));
			base.WriteElementStringRaw("Duration", "", XmlConvert.ToString(o.Duration));
			base.WriteElementStringRaw("IsBootStrapAccount", "", XmlConvert.ToString(o.IsBootStrapAccount));
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
