using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Serializable]
	public class AmLastKnownConfigSerializable
	{
		public int Role { get; set; }

		public string AuthoritativeServer { get; set; }

		[XmlArray]
		public string[] Members { get; set; }
	}
}
