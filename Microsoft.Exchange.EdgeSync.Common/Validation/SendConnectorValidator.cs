using System;
using System.DirectoryServices.Protocols;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class SendConnectorValidator : ConfigValidator
	{
		public SendConnectorValidator(ReplicationTopology topology) : base(topology, "Send Connector")
		{
			base.ConfigDirectoryPath = "CN=" + AdministrativeGroup.DefaultName + ",CN=Administrative Groups";
			base.LdapQuery = Schema.Query.QuerySendConnectors;
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				return Schema.SendConnector.PayloadAttributes;
			}
		}

		protected override string[] ReadAttributes
		{
			get
			{
				return new string[]
				{
					"msExchSourceBridgeheadServersDN"
				};
			}
		}

		protected override bool CompareAttributes(ExSearchResultEntry edgeEntry, ExSearchResultEntry hubEntry, string[] copyAttributes)
		{
			return this.Filter(hubEntry) && base.CompareAttributes(edgeEntry, hubEntry, copyAttributes);
		}

		protected override bool Filter(ExSearchResultEntry entry)
		{
			if (entry.Attributes.ContainsKey("msExchSourceBridgeheadServersDN"))
			{
				DirectoryAttribute directoryAttribute = entry.Attributes["msExchSourceBridgeheadServersDN"];
				string value = string.Format("cn={0},", base.CurrentEdgeConnection.EdgeServer.Name.ToLower());
				foreach (object obj in directoryAttribute)
				{
					byte[] bytes = (byte[])obj;
					string @string = Encoding.UTF8.GetString(bytes);
					if (@string.ToLowerInvariant().Contains(value))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
	}
}
