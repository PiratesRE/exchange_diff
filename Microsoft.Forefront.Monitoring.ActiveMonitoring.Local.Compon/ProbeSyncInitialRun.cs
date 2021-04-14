using System;
using System.Collections.Concurrent;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class ProbeSyncInitialRun
	{
		public ProbeSyncInitialRun(ProbeWorkItem currentProbe)
		{
			this.currentProbe = currentProbe;
			this.ParseWorkContext(currentProbe.Definition.ExtensionAttributes);
		}

		public string ProducerProbeName { get; private set; }

		public void MarkCompleted()
		{
			if (this.role == ProbeSyncInitialRun.SyncRole.Producer)
			{
				ProbeSyncInitialRun.completedProbes.TryAdd(this.ProducerProbeName, true);
			}
		}

		public bool CanRun()
		{
			return this.role != ProbeSyncInitialRun.SyncRole.Consumer || ProbeSyncInitialRun.completedProbes.ContainsKey(this.ProducerProbeName);
		}

		private void ParseWorkContext(string workContextXml)
		{
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentException("Work Definition XML is null", "workContextXml");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(workContextXml);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//WorkContext/ProbeSyncInitialRun");
			if (xmlNode != null)
			{
				Utils.CheckXmlElement(xmlNode, "ProbeSyncInitialRun");
				this.role = Utils.GetMandatoryXmlEnumAttribute<ProbeSyncInitialRun.SyncRole>(xmlNode, "Role");
				if (this.role == ProbeSyncInitialRun.SyncRole.Consumer)
				{
					this.ProducerProbeName = Utils.GetMandatoryXmlAttribute<string>(xmlNode, "ProducerProbeName");
					if (string.IsNullOrWhiteSpace(this.ProducerProbeName))
					{
						throw new ArgumentNullException("ProbeSyncInitialRun/ProducerName");
					}
					if (this.ProducerProbeName == this.currentProbe.Definition.Name)
					{
						throw new ArgumentException(string.Format("Producer and consumer cannot be a same probe, current probe: {0}", this.currentProbe.Definition.Name));
					}
				}
				else if (this.role == ProbeSyncInitialRun.SyncRole.Producer)
				{
					this.ProducerProbeName = this.currentProbe.Definition.Name;
					return;
				}
			}
			else
			{
				this.role = ProbeSyncInitialRun.SyncRole.None;
			}
		}

		private static ConcurrentDictionary<string, bool> completedProbes = new ConcurrentDictionary<string, bool>();

		private ProbeSyncInitialRun.SyncRole role;

		private ProbeWorkItem currentProbe;

		private enum SyncRole
		{
			None,
			Producer,
			Consumer
		}
	}
}
