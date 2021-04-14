using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class OlcTopology
	{
		public static OlcTopology Instance
		{
			get
			{
				return OlcTopology.instance.Value;
			}
		}

		private OlcTopology()
		{
			this.objLock = new object();
			this.LastRefreshed = DateTime.MinValue;
			this.rnd = new Random();
			this.xml = null;
		}

		private DateTime LastRefreshed { get; set; }

		private static XDocument Deserialize(string parsedXml)
		{
			Exception ex = null;
			XDocument result;
			try
			{
				result = XDocument.Parse(parsedXml);
			}
			catch (XmlException ex2)
			{
				ex = ex2;
				result = null;
			}
			catch (FormatException ex3)
			{
				ex = ex3;
				result = null;
			}
			catch (NotSupportedException ex4)
			{
				ex = ex4;
				result = null;
			}
			finally
			{
				if (ex != null)
				{
					CommonUtils.GenerateWatson(ex);
				}
			}
			return result;
		}

		public string FindServerByLocalDatacenter()
		{
			string result;
			lock (this.objLock)
			{
				this.CheckAndInitialize();
				if (this.xml == null)
				{
					result = null;
				}
				else
				{
					List<XElement> list = new List<XElement>(from el in this.xml.Descendants("cluster")
					select el);
					if (list.Count == 0)
					{
						throw new ClusterNotFoundPermanentException();
					}
					XElement cluster = list[this.rnd.Next(list.Count)];
					result = this.FindServerByCluster(cluster);
				}
			}
			return result;
		}

		public string FindServerByClusterIP(IPAddress clusterIp)
		{
			string result;
			lock (this.objLock)
			{
				this.CheckAndInitialize();
				if (this.xml == null)
				{
					throw new ClusterIPNotFoundPermanentException(clusterIp);
				}
				XElement xelement = (from el in this.xml.Descendants("cluster")
				where (string)el.Attribute("clusterIp") == clusterIp.ToString()
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					throw new ClusterIPNotFoundPermanentException(clusterIp);
				}
				result = this.FindServerByCluster(xelement);
			}
			return result;
		}

		private string FindServerByCluster(XElement cluster)
		{
			List<XElement> list = new List<XElement>(from el in cluster.Descendants("hostName")
			select el);
			if (list.Count == 0)
			{
				throw new ClusterNotFoundPermanentException();
			}
			XElement xelement = list[this.rnd.Next(list.Count)];
			return (string)xelement.Attribute("name");
		}

		private void CheckAndInitialize()
		{
			if (this.xml == null || ConfigBase<OlcConfigSchema>.Provider.LastUpdated > this.LastRefreshed)
			{
				this.InitializeFromConfig();
			}
		}

		private void InitializeFromConfig()
		{
			this.LastRefreshed = ConfigBase<OlcConfigSchema>.Provider.LastUpdated;
			string config = ConfigBase<OlcConfigSchema>.GetConfig<string>("OlcTopology");
			if (config == null)
			{
				this.xml = null;
				return;
			}
			XDocument xdocument = OlcTopology.Deserialize(config);
			if (xdocument != null)
			{
				Interlocked.Exchange<XDocument>(ref this.xml, xdocument);
			}
		}

		private static Lazy<OlcTopology> instance = new Lazy<OlcTopology>(() => new OlcTopology(), LazyThreadSafetyMode.PublicationOnly);

		private object objLock;

		private XDocument xml;

		private Random rnd;
	}
}
