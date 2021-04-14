using System;
using System.Text;
using System.Xml;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ServerHistoryEntryData
	{
		public ServerHistoryEntryData(ServerHistoryEntry entry)
		{
			this.FromBinary(entry.Data);
		}

		public ServerHistoryEntryData(string serverName, DateTime activeTimestamp, DateTime passiveTimestamp, string passiveReason)
		{
			this.ServerName = serverName;
			this.ActiveTimestamp = activeTimestamp;
			this.PassiveTimestamp = passiveTimestamp;
			this.PassiveReason = passiveReason;
		}

		public string ServerName { get; set; }

		public DateTime ActiveTimestamp { get; set; }

		public DateTime PassiveTimestamp { get; set; }

		public string PassiveReason { get; set; }

		public override string ToString()
		{
			return string.Format("ServerName: {0}, ActiveTimestamp: {1}, PassiveTimestamp: {2}, PassiveReason: {3}", new object[]
			{
				this.ServerName,
				this.ActiveTimestamp,
				this.PassiveTimestamp,
				this.PassiveReason
			});
		}

		public byte[] ToBinary()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("ServerHistoryEntryData");
			xmlDocument.AppendChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("ServerName");
			xmlElement2.InnerText = this.ServerName;
			XmlElement xmlElement3 = xmlDocument.CreateElement("ActiveTimestamp");
			xmlElement3.InnerText = this.ActiveTimestamp.ToString();
			XmlElement xmlElement4 = xmlDocument.CreateElement("PassiveTimestamp");
			xmlElement4.InnerText = this.PassiveTimestamp.ToString();
			XmlElement xmlElement5 = xmlDocument.CreateElement("PassiveReason");
			xmlElement5.InnerText = this.PassiveReason;
			xmlElement.AppendChild(xmlElement2);
			xmlElement.AppendChild(xmlElement3);
			xmlElement.AppendChild(xmlElement4);
			xmlElement.AppendChild(xmlElement5);
			return new UTF8Encoding().GetBytes(xmlDocument.OuterXml);
		}

		public void FromBinary(byte[] data)
		{
			string innerText;
			DateTime activeTimestamp;
			DateTime passiveTimestamp;
			string innerText2;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(new UTF8Encoding().GetString(data));
				string str = "/ServerHistoryEntryData/";
				innerText = xmlDocument.DocumentElement.SelectSingleNode(str + "ServerName").InnerText;
				activeTimestamp = DateTime.Parse(xmlDocument.DocumentElement.SelectSingleNode(str + "ActiveTimestamp").InnerText);
				passiveTimestamp = DateTime.Parse(xmlDocument.DocumentElement.SelectSingleNode(str + "PassiveTimestamp").InnerText);
				innerText2 = xmlDocument.DocumentElement.SelectSingleNode(str + "PassiveReason").InnerText;
			}
			catch (Exception)
			{
				return;
			}
			this.ServerName = innerText;
			this.ActiveTimestamp = activeTimestamp;
			this.PassiveTimestamp = passiveTimestamp;
			this.PassiveReason = innerText2;
		}

		private const string ServerHistoryEntryDataElement = "ServerHistoryEntryData";

		private const string ServerNameElement = "ServerName";

		private const string ActiveTimestampElement = "ActiveTimestamp";

		private const string PassiveTimestampElement = "PassiveTimestamp";

		private const string PassiveReasonElement = "PassiveReason";
	}
}
