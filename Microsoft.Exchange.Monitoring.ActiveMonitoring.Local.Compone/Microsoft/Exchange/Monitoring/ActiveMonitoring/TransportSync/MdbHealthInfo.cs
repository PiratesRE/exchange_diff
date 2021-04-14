using System;
using System.Text;
using System.Xml;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public class MdbHealthInfo
	{
		internal MdbHealthInfo(XmlReader xmlReader)
		{
			if (xmlReader.Name != "Database")
			{
				throw new ArgumentException("Invalid Xml Node was passed in", "Database");
			}
			while (xmlReader.Read())
			{
				string name = xmlReader.Name;
				string a;
				if ((a = name) != null)
				{
					if (!(a == "databaseId"))
					{
						if (a == "enabled")
						{
							this.Enabled = bool.Parse(xmlReader.ReadString());
						}
					}
					else
					{
						this.MdbGuid = new Guid(xmlReader.ReadString());
					}
				}
				if (name == "Database")
				{
					return;
				}
			}
		}

		public Guid MdbGuid { get; private set; }

		public bool Enabled { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("MDBGuid={0} Enabled={1}", this.MdbGuid, this.Enabled);
			return stringBuilder.ToString();
		}
	}
}
