using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class Connectors
	{
		public static void GetObjectInboundPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["SenderIPAddresses"]))
			{
				List<IPAddressEntry> list = new List<IPAddressEntry>();
				foreach (IPRange range in ((MultiValuedProperty<IPRange>)dataRow["SenderIPAddresses"]))
				{
					list.Add(new IPAddressEntry(range));
				}
				dataRow["senderIPAddressList"] = list;
			}
			if (!DBNull.Value.Equals(dataRow["SenderDomains"]))
			{
				List<DomainEntry> list2 = new List<DomainEntry>();
				foreach (AddressSpace addressSpace in ((MultiValuedProperty<AddressSpace>)dataRow["SenderDomains"]))
				{
					list2.Add(new DomainEntry(addressSpace.Address));
				}
				dataRow["senderDomainsList"] = list2;
			}
		}

		public static ADMultiValuedProperty<AcceptedDomainIdParameter> GetAcceptedDomains(object data)
		{
			if (DBNull.Value.Equals(data))
			{
				return new ADMultiValuedProperty<AcceptedDomainIdParameter>();
			}
			return new ADMultiValuedProperty<AcceptedDomainIdParameter>(from r in (object[])data
			select new AcceptedDomainIdParameter((Identity)r));
		}

		public static void GetObjectOutboundPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["RecipientDomains"]))
			{
				List<DomainEntry> list = new List<DomainEntry>();
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in ((MultiValuedProperty<SmtpDomainWithSubdomains>)dataRow["RecipientDomains"]))
				{
					list.Add(new DomainEntry(smtpDomainWithSubdomains.Address));
				}
				dataRow["RecipientDomainsList"] = list;
			}
		}

		public static void GetForInboundSDOPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["SenderIPAddresses"] != null)
			{
				List<IPAddressEntry> list = new List<IPAddressEntry>();
				foreach (IPRange range in ((MultiValuedProperty<IPRange>)dataRow["SenderIPAddresses"]))
				{
					list.Add(new IPAddressEntry(range));
				}
				dataRow["senderIPAddressList"] = list;
			}
			List<DomainEntry> list2 = new List<DomainEntry>();
			foreach (AddressSpace addressSpace in ((MultiValuedProperty<AddressSpace>)dataRow["SenderDomains"]))
			{
				list2.Add(new DomainEntry(addressSpace.Address));
			}
			dataRow["senderDomainsList"] = list2;
			if ((bool)dataRow["Enabled"])
			{
				dataRow["EnabledDisplay"] = Strings.ConnectorEnabled;
				return;
			}
			dataRow["EnabledDisplay"] = Strings.ConnectorCaptionDisabled;
		}

		public static void GetForOutboundSDOPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<DomainEntry> list = new List<DomainEntry>();
			foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in ((MultiValuedProperty<SmtpDomainWithSubdomains>)dataRow["RecipientDomains"]))
			{
				list.Add(new DomainEntry(smtpDomainWithSubdomains.Address));
			}
			dataRow["RecipientDomainsList"] = list;
			if ((bool)dataRow["UseMXRecord"])
			{
				dataRow["DeliveryTypeDisplay"] = Strings.ConnectorSDOMXBased;
			}
			else
			{
				dataRow["DeliveryTypeDisplay"] = Strings.ConnectorSDOSmartHost;
			}
			string value = string.Empty;
			string value2 = string.Empty;
			if (string.IsNullOrEmpty(dataRow["TlsSettings"].ToString()))
			{
				value = Strings.ConnectorSecurityOpportunistic;
			}
			else
			{
				switch ((TlsAuthLevel)Enum.Parse(typeof(TlsAuthLevel), dataRow["TlsSettings"].ToString(), true))
				{
				case TlsAuthLevel.EncryptionOnly:
					value = Strings.ConnectorSecuritySelfSigned;
					break;
				case TlsAuthLevel.CertificateValidation:
					value = Strings.ConnectorSecurityTrusted;
					break;
				case TlsAuthLevel.DomainValidation:
					value = Strings.ConnectorSecurityCertDomain;
					value2 = dataRow["TlsDomain"].ToString();
					break;
				}
			}
			dataRow["TlsSettingDisplayValue"] = value;
			dataRow["TlsDomainDisplayValue"] = value2;
			if ((bool)dataRow["Enabled"])
			{
				dataRow["EnabledDisplay"] = Strings.ConnectorEnabled;
				return;
			}
			dataRow["EnabledDisplay"] = Strings.ConnectorCaptionDisabled;
		}
	}
}
