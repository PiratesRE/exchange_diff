using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionRequest
{
	internal class XmlSerializationWriterProvision : XmlSerializationWriter
	{
		public void Write5_Provision(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("Provision", "DeltaSyncV2:");
				return;
			}
			base.TopLevelElement();
			this.Write4_Provision("Provision", "DeltaSyncV2:", (Provision)o, false, false);
		}

		private void Write4_Provision(string n, string ns, Provision o, bool isNullable, bool needType)
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
				if (!(type == typeof(Provision)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "DeltaSyncV2:");
			}
			AccountType[] add = o.Add;
			if (add != null)
			{
				base.WriteStartElement("Add", "DeltaSyncV2:", null, false);
				for (int i = 0; i < add.Length; i++)
				{
					this.Write3_AccountType("Account", "DeltaSyncV2:", add[i], false, false);
				}
				base.WriteEndElement();
			}
			AccountType[] delete = o.Delete;
			if (delete != null)
			{
				base.WriteStartElement("Delete", "DeltaSyncV2:", null, false);
				for (int j = 0; j < delete.Length; j++)
				{
					this.Write3_AccountType("Account", "DeltaSyncV2:", delete[j], false, false);
				}
				base.WriteEndElement();
			}
			AccountType[] read = o.Read;
			if (read != null)
			{
				base.WriteStartElement("Read", "DeltaSyncV2:", null, false);
				for (int k = 0; k < read.Length; k++)
				{
					this.Write3_AccountType("Account", "DeltaSyncV2:", read[k], false, false);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write3_AccountType(string n, string ns, AccountType o, bool isNullable, bool needType)
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
				if (!(type == typeof(AccountType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("AccountType", "DeltaSyncV2:");
			}
			base.WriteElementString("Name", "DeltaSyncV2:", o.Name);
			base.WriteElementString("Type", "DeltaSyncV2:", this.Write1_AccountTypeType(o.Type));
			base.WriteElementString("PartnerID", "DeltaSyncV2:", o.PartnerID);
			base.WriteEndElement(o);
		}

		private string Write1_AccountTypeType(AccountTypeType v)
		{
			if (v == AccountTypeType.MailRelay)
			{
				return "MailRelay";
			}
			throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountTypeType");
		}

		protected override void InitCallbacks()
		{
		}
	}
}
