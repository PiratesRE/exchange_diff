using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve
{
	public class XmlSerializationWriterRecipientSyncState : XmlSerializationWriter
	{
		public void Write3_RecipientSyncState(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("RecipientSyncState", "");
				return;
			}
			base.TopLevelElement();
			this.Write2_RecipientSyncState("RecipientSyncState", "", (RecipientSyncState)o, true, false);
		}

		private void Write2_RecipientSyncState(string n, string ns, RecipientSyncState o, bool isNullable, bool needType)
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
				if (!(type == typeof(RecipientSyncState)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("RecipientSyncState", "");
			}
			base.WriteElementString("ProxyAddresses", "", o.ProxyAddresses);
			base.WriteElementString("SignupAddresses", "", o.SignupAddresses);
			base.WriteElementStringRaw("PartnerId", "", XmlConvert.ToString(o.PartnerId));
			base.WriteElementString("UMProxyAddresses", "", o.UMProxyAddresses);
			base.WriteElementString("ArchiveAddress", "", o.ArchiveAddress);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
