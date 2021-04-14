using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MExConfiguration
	{
		static MExConfiguration()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			MExConfiguration.Schemas = new XmlSchemaSet();
			MExConfiguration.InternalSchemas = new XmlSchemaSet();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("MExRuntimeConfig.xsd"))
			{
				MExConfiguration.Schemas.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(manifestResourceStream));
				manifestResourceStream.Position = 0L;
				MExConfiguration.InternalSchemas.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(manifestResourceStream));
			}
			using (Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream("InternalMExRuntimeConfig.xsd"))
			{
				MExConfiguration.InternalSchemas.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(manifestResourceStream2));
			}
			MExConfiguration.Schemas.Compile();
			MExConfiguration.InternalSchemas.Compile();
		}

		internal MExConfiguration(ProcessTransportRole transportProcessRole, string installPath) : this(MExConfiguration.GetExchangeSku(), transportProcessRole, installPath)
		{
		}

		internal MExConfiguration(Datacenter.ExchangeSku exchangeSku, ProcessTransportRole transportProcessRole, string installPath)
		{
			this.monitoringOptions = new MonitoringOptions();
			this.agentList = new List<AgentInfo>();
			this.exchangeSku = exchangeSku;
			this.transportProcessRole = transportProcessRole;
			this.installPath = installPath;
		}

		internal MonitoringOptions MonitoringOptions
		{
			get
			{
				return this.monitoringOptions;
			}
		}

		internal IList<AgentInfo> AgentList
		{
			get
			{
				return this.agentList;
			}
		}

		internal bool DisposeAgents
		{
			get
			{
				return this.disposeAgents;
			}
		}

		internal static bool ValidateFile(string filePath)
		{
			bool isValid = true;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			xmlReaderSettings.Schemas = MExConfiguration.Schemas;
			xmlReaderSettings.ValidationEventHandler += delegate(object param0, ValidationEventArgs param1)
			{
				isValid = false;
			};
			xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
			xmlReaderSettings.XmlResolver = null;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(filePath, xmlReaderSettings))
				{
					while (xmlReader.Read())
					{
					}
				}
			}
			catch (IOException)
			{
				isValid = false;
			}
			catch (UnauthorizedAccessException)
			{
				isValid = false;
			}
			return isValid;
		}

		internal List<AgentInfo> GetPublicAgentList()
		{
			List<AgentInfo> list = new List<AgentInfo>();
			foreach (AgentInfo agentInfo in this.agentList)
			{
				if (!agentInfo.IsInternal)
				{
					list.Add(agentInfo);
				}
			}
			return list;
		}

		internal List<AgentInfo> GetPreExecutionInternalAgents()
		{
			List<AgentInfo> list = new List<AgentInfo>();
			foreach (AgentInfo agentInfo in this.agentList)
			{
				if (!agentInfo.IsInternal)
				{
					break;
				}
				list.Add(agentInfo);
			}
			return list;
		}

		internal bool Validate()
		{
			XmlDocument xmlDocument = this.CreateXmlDocument();
			xmlDocument.Schemas = MExConfiguration.Schemas;
			bool isValid = true;
			xmlDocument.Validate(delegate(object param0, ValidationEventArgs param1)
			{
				isValid = false;
			});
			return isValid;
		}

		internal void Load(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.MissingConfigurationFile(filePath), new FileNotFoundException());
			}
			List<AgentInfo> preExecutionInternalAgents;
			List<AgentInfo> postExecutionInternalAgents;
			this.LoadInternalAgents(out preExecutionInternalAgents, out postExecutionInternalAgents);
			List<AgentInfo> publicAgents;
			this.LoadPublicAgents(filePath, out publicAgents);
			this.agentList = this.CreateFinalAgentsList(publicAgents, preExecutionInternalAgents, postExecutionInternalAgents);
		}

		internal AgentInfo[] GetEnabledAgentsByType(string type)
		{
			return this.GetEnabledAgentsByType(type, false);
		}

		internal AgentInfo[] GetEnaledPublicAgentsByType(string type)
		{
			return this.GetEnabledAgentsByType(type, true);
		}

		internal void Save(string filePath)
		{
			XmlDocument xmlDocument = this.CreateXmlDocument();
			xmlDocument.Schemas = MExConfiguration.Schemas;
			xmlDocument.Validate(delegate(object sender, ValidationEventArgs args)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfiguration, args.Exception);
			});
			int num = 20;
			try
			{
				IL_38:
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					xmlDocument.Save(fileStream);
				}
			}
			catch (XmlException innerException)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException2);
			}
			catch (IOException innerException3)
			{
				if (num <= 0)
				{
					throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException3);
				}
				num--;
				Thread.Sleep(50);
				goto IL_38;
			}
		}

		private static Datacenter.ExchangeSku GetExchangeSku()
		{
			Datacenter.ExchangeSku result;
			try
			{
				if (Datacenter.IsForefrontForOfficeDatacenter())
				{
					result = Datacenter.ExchangeSku.ForefrontForOfficeDatacenter;
				}
				else
				{
					result = Datacenter.GetExchangeSku();
				}
			}
			catch (CannotDetermineExchangeModeException innerException)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.FailedToReadDataCenterMode, innerException);
			}
			return result;
		}

		private List<AgentInfo> CreateFinalAgentsList(List<AgentInfo> publicAgents, List<AgentInfo> preExecutionInternalAgents, List<AgentInfo> postExecutionInternalAgents)
		{
			List<AgentInfo> list = new List<AgentInfo>();
			list.AddRange(preExecutionInternalAgents);
			list.AddRange(publicAgents);
			list.AddRange(postExecutionInternalAgents);
			return list;
		}

		private void LoadPublicAgents(string filePath, out List<AgentInfo> publicAgents)
		{
			int num = 20;
			if (string.IsNullOrEmpty(filePath))
			{
				publicAgents = new List<AgentInfo>();
				return;
			}
			for (;;)
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.Schemas = MExConfiguration.Schemas;
				try
				{
					using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						xmlDocument.Load(fileStream);
						xmlDocument.Validate(delegate(object sender, ValidationEventArgs args)
						{
							throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), args.Exception);
						});
						this.LoadSettings(xmlDocument.SelectSingleNode("/configuration/mexRuntime/settings"));
						this.LoadMonitoringOptions(xmlDocument.SelectSingleNode("/configuration/mexRuntime/monitoring"));
						publicAgents = this.LoadAgentList(xmlDocument.SelectSingleNode("/configuration/mexRuntime/agentList"), false);
					}
				}
				catch (XmlException innerException)
				{
					throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException);
				}
				catch (FormatException innerException2)
				{
					throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException2);
				}
				catch (UnauthorizedAccessException innerException3)
				{
					throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException3);
				}
				catch (IOException innerException4)
				{
					if (num <= 0)
					{
						throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile(filePath), innerException4);
					}
					num--;
					Thread.Sleep(50);
					continue;
				}
				break;
			}
		}

		private void LoadSettings(XmlNode node)
		{
			if (node == null)
			{
				return;
			}
			foreach (object obj in node)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "disposeAgents")
				{
					this.disposeAgents = bool.Parse(xmlNode.InnerText);
				}
			}
		}

		private void LoadInternalAgents(out List<AgentInfo> preExecutionInternalAgents, out List<AgentInfo> postExecutionInternalAgents)
		{
			preExecutionInternalAgents = new List<AgentInfo>();
			postExecutionInternalAgents = new List<AgentInfo>();
			try
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.Schemas = MExConfiguration.InternalSchemas;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("internalAgents.config"))
				{
					xmlDocument.Load(manifestResourceStream);
					xmlDocument.Validate(delegate(object sender, ValidationEventArgs args)
					{
						throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile("internalAgents.config"), args.Exception);
					});
					string xmlMarkups = this.GetXmlMarkups(this.exchangeSku, this.transportProcessRole);
					if (xmlMarkups != null)
					{
						preExecutionInternalAgents = this.LoadAgentList(xmlDocument.SelectSingleNode("/internalConfiguration/internalMexRuntime/" + xmlMarkups + "/preExecution"), true);
						postExecutionInternalAgents = this.LoadAgentList(xmlDocument.SelectSingleNode("/internalConfiguration/internalMexRuntime/" + xmlMarkups + "/postExecution"), true);
					}
				}
			}
			catch (XmlException innerException)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile("internalAgents.config"), innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile("internalAgents.config"), innerException2);
			}
			catch (IOException innerException3)
			{
				throw new ExchangeConfigurationException(MExRuntimeStrings.InvalidConfigurationFile("internalAgents.config"), innerException3);
			}
		}

		private string GetXmlMarkups(Datacenter.ExchangeSku exchangeSku, ProcessTransportRole transportProcessRole)
		{
			string result = null;
			switch (exchangeSku)
			{
			case Datacenter.ExchangeSku.Enterprise:
			case Datacenter.ExchangeSku.DatacenterDedicated:
				switch (transportProcessRole)
				{
				case ProcessTransportRole.Hub:
					result = "enterpriseBridgehead";
					break;
				case ProcessTransportRole.Edge:
					result = "enterpriseGateway";
					break;
				case ProcessTransportRole.FrontEnd:
					result = "enterpriseFrontend";
					break;
				case ProcessTransportRole.MailboxSubmission:
					result = "enterpriseMailboxSubmission";
					break;
				case ProcessTransportRole.MailboxDelivery:
					result = "enterpriseMailboxDelivery";
					break;
				}
				break;
			case Datacenter.ExchangeSku.ExchangeDatacenter:
				switch (transportProcessRole)
				{
				case ProcessTransportRole.Hub:
					result = "exchangeDatacenterBridgehead";
					break;
				case ProcessTransportRole.FrontEnd:
					result = "exchangeDatacenterFrontend";
					break;
				case ProcessTransportRole.MailboxSubmission:
					result = "exchangeDatacenterMailboxSubmission";
					break;
				case ProcessTransportRole.MailboxDelivery:
					result = "exchangeDatacenterMailboxDelivery";
					break;
				}
				break;
			case Datacenter.ExchangeSku.PartnerHosted:
				switch (transportProcessRole)
				{
				case ProcessTransportRole.Hub:
					result = "partnerHostedBridgehead";
					break;
				case ProcessTransportRole.Edge:
					result = "partnerHostedGateway";
					break;
				case ProcessTransportRole.MailboxSubmission:
					result = "partnerHostedMailboxSubmission";
					break;
				case ProcessTransportRole.MailboxDelivery:
					result = "partnerHostedMailboxDelivery";
					break;
				}
				break;
			case Datacenter.ExchangeSku.ForefrontForOfficeDatacenter:
				switch (transportProcessRole)
				{
				case ProcessTransportRole.Hub:
					result = "forefrontForOfficeBridgehead";
					break;
				case ProcessTransportRole.FrontEnd:
					result = "forefrontForOfficeFrontend";
					break;
				}
				break;
			}
			return result;
		}

		private AgentInfo[] GetEnabledAgentsByType(string type, bool onlyPublic)
		{
			List<AgentInfo> list = new List<AgentInfo>();
			foreach (AgentInfo agentInfo in this.agentList)
			{
				if (agentInfo.Enabled && agentInfo.BaseTypeName == type && (!onlyPublic || !agentInfo.IsInternal))
				{
					list.Add(agentInfo);
				}
			}
			AgentInfo[] array = new AgentInfo[list.Count];
			list.CopyTo(array);
			return array;
		}

		private void LoadMonitoringOptions(XmlNode node)
		{
			foreach (object obj in node)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "agentExecution")
				{
					this.monitoringOptions.AgentExecutionLimitInMilliseconds = int.Parse(xmlNode.Attributes.GetNamedItem("timeLimitInMilliseconds").Value, CultureInfo.InvariantCulture);
				}
				else if (xmlNode.Name == "messageSnapshot")
				{
					this.monitoringOptions.MessageSnapshotEnabled = bool.Parse(xmlNode.Attributes.GetNamedItem("enabled").Value);
				}
			}
		}

		private List<AgentInfo> LoadAgentList(XmlNode node, bool isInternal)
		{
			List<AgentInfo> list = new List<AgentInfo>();
			foreach (object obj in node)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode.Name != "agent"))
				{
					XmlAttributeCollection attributes = xmlNode.Attributes;
					string value = attributes.GetNamedItem("assemblyPath").Value;
					string path = isInternal ? Path.Combine(this.installPath, value) : value;
					list.Add(new AgentInfo(attributes.GetNamedItem("name").Value, attributes.GetNamedItem("baseType").Value, attributes.GetNamedItem("classFactory").Value, path, bool.Parse(attributes.GetNamedItem("enabled").Value), isInternal));
				}
			}
			return list;
		}

		private XmlDocument CreateXmlDocument()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.InsertBefore(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null), xmlDocument.DocumentElement);
			xmlDocument.AppendChild(xmlDocument.CreateElement("configuration"));
			XmlElement xmlElement = xmlDocument.CreateElement("mexRuntime");
			xmlDocument.DocumentElement.PrependChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("monitoring");
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xmlDocument.CreateElement("agentExecution");
			xmlElement3.SetAttribute("timeLimitInMilliseconds", this.monitoringOptions.AgentExecutionLimitInMilliseconds.ToString(CultureInfo.InvariantCulture));
			xmlElement2.AppendChild(xmlElement3);
			if (!this.monitoringOptions.MessageSnapshotEnabled)
			{
				XmlElement xmlElement4 = xmlDocument.CreateElement("messageSnapshot");
				xmlElement4.SetAttribute("enabled", "false");
				xmlElement2.AppendChild(xmlElement4);
			}
			XmlElement xmlElement5 = xmlDocument.CreateElement("agentList");
			xmlElement.AppendChild(xmlElement5);
			foreach (AgentInfo agentInfo in this.agentList)
			{
				if (!agentInfo.IsInternal)
				{
					XmlElement xmlElement6 = xmlDocument.CreateElement("agent");
					xmlElement6.SetAttribute("name", agentInfo.AgentName);
					xmlElement6.SetAttribute("baseType", agentInfo.BaseTypeName);
					xmlElement6.SetAttribute("classFactory", agentInfo.FactoryTypeName);
					xmlElement6.SetAttribute("assemblyPath", agentInfo.FactoryAssemblyPath);
					xmlElement6.SetAttribute("enabled", agentInfo.Enabled.ToString().ToLower(CultureInfo.InvariantCulture));
					xmlElement5.AppendChild(xmlElement6);
				}
			}
			XmlElement xmlElement7 = xmlDocument.CreateElement("settings");
			if (!this.disposeAgents)
			{
				XmlElement xmlElement8 = xmlDocument.CreateElement("disposeAgents");
				xmlElement8.InnerText = "false";
				xmlElement7.AppendChild(xmlElement8);
			}
			xmlElement.AppendChild(xmlElement7);
			return xmlDocument;
		}

		private static readonly XmlSchemaSet Schemas;

		private static readonly XmlSchemaSet InternalSchemas;

		private MonitoringOptions monitoringOptions;

		private List<AgentInfo> agentList;

		private Datacenter.ExchangeSku exchangeSku;

		private ProcessTransportRole transportProcessRole;

		private string installPath;

		private bool disposeAgents = true;

		private static class InternalAgentConstants
		{
			public const string ExchangeDatacenterHubTransport = "exchangeDatacenterBridgehead";

			public const string PartnerHostedHubTransport = "partnerHostedBridgehead";

			public const string PartnerHostedGateway = "partnerHostedGateway";

			public const string EnterpriseHubTransport = "enterpriseBridgehead";

			public const string EnterpriseGateway = "enterpriseGateway";

			public const string ForefrontForOfficeFrontend = "forefrontForOfficeFrontend";

			public const string ForefrontForOfficeBridgehead = "forefrontForOfficeBridgehead";

			public const string ExchangeDatacenterFrontend = "exchangeDatacenterFrontend";

			public const string EnterpriseFrontend = "enterpriseFrontend";

			public const string ExchangeDatacenterMailboxSubmission = "exchangeDatacenterMailboxSubmission";

			public const string PartnerHostedMailboxSubmission = "partnerHostedMailboxSubmission";

			public const string EnterpriseMailboxSubmission = "enterpriseMailboxSubmission";

			public const string ExchangeDatacenterMailboxDelivery = "exchangeDatacenterMailboxDelivery";

			public const string PartnerHostedMailboxDelivery = "partnerHostedMailboxDelivery";

			public const string EnterpriseMailboxDelivery = "enterpriseMailboxDelivery";

			public const string InternalAgentsConfigurationFileName = "internalAgents.config";
		}
	}
}
