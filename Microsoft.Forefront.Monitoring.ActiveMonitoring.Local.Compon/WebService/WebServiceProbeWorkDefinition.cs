using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class WebServiceProbeWorkDefinition
	{
		public WebServiceProbeWorkDefinition(string xml)
		{
			this.macAddress = WebServiceProbeWorkDefinition.GetMacAddress();
			this.LoadFromContext(xml);
		}

		internal WebServiceConfiguration WebServiceConfiguration
		{
			get
			{
				return this.webServiceConfiguration;
			}
		}

		internal List<Operation> Operations
		{
			get
			{
				return this.operations;
			}
		}

		internal PhysicalAddress MachineMacAddress
		{
			get
			{
				return this.macAddress;
			}
		}

		private static Dictionary<string, Tuple<Type, string>> GetMachineUniqueParameters(XmlElement unqiueParamsElement)
		{
			Dictionary<string, Tuple<Type, string>> dictionary = new Dictionary<string, Tuple<Type, string>>();
			foreach (object obj in unqiueParamsElement.ChildNodes)
			{
				XmlNode node = (XmlNode)obj;
				XmlElement xmlElement = Utils.CheckNode(node, "in Parameters") as XmlElement;
				if (xmlElement != null)
				{
					string mandatoryXmlAttribute = Utils.GetMandatoryXmlAttribute<string>(xmlElement, "Name");
					string optionalXmlAttribute = Utils.GetOptionalXmlAttribute<string>(xmlElement, "Type", "System.string");
					string mandatoryXmlAttribute2 = Utils.GetMandatoryXmlAttribute<string>(xmlElement, "Namespace");
					dictionary[mandatoryXmlAttribute] = new Tuple<Type, string>(Type.GetType(optionalXmlAttribute, true, true), mandatoryXmlAttribute2);
				}
			}
			return dictionary;
		}

		private static PhysicalAddress GetMacAddress()
		{
			PhysicalAddress physicalAddress = null;
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					physicalAddress = networkInterface.GetPhysicalAddress();
					if ((from value in physicalAddress.GetAddressBytes()
					where value > 0
					select value).Count<byte>() > 0)
					{
						break;
					}
				}
			}
			if (physicalAddress == null)
			{
				throw new Exception(string.Format("Could not determine the mac address of the machine:{0}", Environment.MachineName));
			}
			return physicalAddress;
		}

		private void LoadFromContext(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("Work Definition XML is null");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			XmlElement configNode = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//WorkContext/WebServiceConfiguration"), "WebServiceConfiguration");
			this.webServiceConfiguration = this.GetWebServiceConfiguration(configNode);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//WorkContext/MachineUniqueParameters");
			if (xmlNode != null)
			{
				XmlElement unqiueParamsElement = Utils.CheckXmlElement(xmlNode, "MachineUniqueParameters");
				this.machineUniqueParameters = WebServiceProbeWorkDefinition.GetMachineUniqueParameters(unqiueParamsElement);
			}
			XmlElement operationsNode = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//WorkContext/Operations"), "Operations");
			this.operations = this.GetOperations(operationsNode);
		}

		private WebServiceConfiguration GetWebServiceConfiguration(XmlElement configNode)
		{
			WebServiceConfiguration webServiceConfiguration = new WebServiceConfiguration();
			XmlElement xmlElement = Utils.CheckXmlElement(configNode.SelectSingleNode("//Uri"), "Uri");
			webServiceConfiguration.Uri = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Address"), "Address");
			this.CheckSslCertificate(xmlElement);
			xmlElement = (configNode.SelectSingleNode("//Binding") as XmlElement);
			if (xmlElement != null)
			{
				string attribute = xmlElement.GetAttribute("Type");
				if (!string.IsNullOrWhiteSpace(attribute))
				{
					webServiceConfiguration.BindingType = attribute;
				}
				webServiceConfiguration.BindingName = xmlElement.GetAttribute("Name");
				webServiceConfiguration.BindingNamespace = xmlElement.GetAttribute("BindingNamespace");
				webServiceConfiguration.ClientBaseAddress = xmlElement.GetAttribute("ClientBaseAddress");
				webServiceConfiguration.CloseTimeout = Utils.GetTimeSpan(xmlElement.GetAttribute("CloseTimeout"), "CloseTimeout", TimeSpan.MaxValue);
				webServiceConfiguration.OpenTimeout = Utils.GetTimeSpan(xmlElement.GetAttribute("OpenTimeout"), "OpenTimeout", TimeSpan.MaxValue);
				webServiceConfiguration.ReceiveTimeout = Utils.GetTimeSpan(xmlElement.GetAttribute("ReceiveTimeout"), "ReceiveTimeout", TimeSpan.MaxValue);
				webServiceConfiguration.SendTimeout = Utils.GetTimeSpan(xmlElement.GetAttribute("SendTimeout"), "SendTimeout", TimeSpan.MaxValue);
				webServiceConfiguration.AllowCookies = xmlElement.GetAttribute("AllowCookies");
				webServiceConfiguration.BypassProxyOnLocal = xmlElement.GetAttribute("BypassProxyOnLocal");
				webServiceConfiguration.HostNameComparisonMode = xmlElement.GetAttribute("HostNameComparisonMode");
				webServiceConfiguration.ListenBacklog = Utils.GetPositiveInteger(xmlElement.GetAttribute("ListenBacklog"), "ListenBacklog");
				webServiceConfiguration.TransactionFlow = xmlElement.GetAttribute("TransactionFlow");
				webServiceConfiguration.TransferMode = xmlElement.GetAttribute("TransferMode");
				webServiceConfiguration.TransactionProtocol = xmlElement.GetAttribute("TransactionProtocol");
				webServiceConfiguration.MaxConnections = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxConnections"), "MaxConnections");
				long num;
				if (long.TryParse(xmlElement.GetAttribute("MaxReceivedMessageSize"), out num) && num > 0L)
				{
					webServiceConfiguration.MaxReceivedMessageSize = num;
				}
				webServiceConfiguration.MaxBufferPoolSize = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxBufferPoolSize"), "MaxBufferPoolSize");
				webServiceConfiguration.MaxBufferSize = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxBufferSize"), "MaxBufferSize");
				webServiceConfiguration.MessageEncoding = xmlElement.GetAttribute("MessageEncoding");
				webServiceConfiguration.TextEncoding = xmlElement.GetAttribute("TextEncoding");
				xmlElement = (configNode.SelectSingleNode("Binding/ReaderQuotas") as XmlElement);
				if (xmlElement != null)
				{
					webServiceConfiguration.MaxDepth = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxDepth"), "MaxDepth");
					webServiceConfiguration.MaxStringContentLength = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxStringContentLength"), "MaxStringContentLength");
					webServiceConfiguration.MaxArrayLength = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxArrayLength"), "MaxArrayLength");
					webServiceConfiguration.MaxBytesPerRead = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxBytesPerRead"), "MaxBytesPerRead");
					webServiceConfiguration.MaxNameTableCharCount = Utils.GetPositiveInteger(xmlElement.GetAttribute("MaxNameTableCharCount"), "MaxNameTableCharCount");
				}
				xmlElement = (configNode.SelectSingleNode("Binding/ReliableSession") as XmlElement);
				if (xmlElement != null)
				{
					webServiceConfiguration.Ordered = xmlElement.GetAttribute("Ordered");
					webServiceConfiguration.InactivityTimeout = Utils.GetTimeSpan(xmlElement.GetAttribute("InactivityTimeout"), "InactivityTimeout", TimeSpan.MaxValue);
					webServiceConfiguration.ReliableSessionEnabled = new bool?(Utils.GetBoolean(xmlElement.GetAttribute("Enabled"), "Enabled in ReliableSession"));
				}
				xmlElement = (configNode.SelectSingleNode("Binding/Security") as XmlElement);
				if (xmlElement != null)
				{
					webServiceConfiguration.SecurityMode = xmlElement.GetAttribute("Mode");
					xmlElement = (configNode.SelectSingleNode("Binding/Security/Transport") as XmlElement);
					if (xmlElement != null)
					{
						webServiceConfiguration.TransportCredentialType = xmlElement.GetAttribute("ClientCredentialType");
						webServiceConfiguration.Realm = xmlElement.GetAttribute("Realm");
						webServiceConfiguration.ProtectionLevel = xmlElement.GetAttribute("ProtectionLevel");
					}
					xmlElement = (configNode.SelectSingleNode("Binding/Security/Message") as XmlElement);
					if (xmlElement != null)
					{
						webServiceConfiguration.MessageCredentialType = xmlElement.GetAttribute("ClientCredentialType");
						webServiceConfiguration.AlgorithmSuite = xmlElement.GetAttribute("AlgorithmSuite");
						webServiceConfiguration.NegotiateServiceCredential = xmlElement.GetAttribute("NegotiateServiceCredential");
						webServiceConfiguration.EstablishSecurityContext = xmlElement.GetAttribute("EstablishSecurityContext");
					}
				}
			}
			xmlElement = Utils.CheckXmlElement(configNode.SelectSingleNode("Proxy"), "Proxy");
			webServiceConfiguration.ProxyClassName = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("ClassName"), "ClassName in node ProxyClass");
			webServiceConfiguration.ProxyAssembly = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Assembly"), "Assembly");
			webServiceConfiguration.ProxyGenerated = Utils.GetBoolean(xmlElement.GetAttribute("Generated"), "Generated", true);
			webServiceConfiguration.ProxyValidatorClassName = xmlElement.GetAttribute("ValidatorClassName");
			if (!string.IsNullOrWhiteSpace(webServiceConfiguration.ProxyValidatorClassName))
			{
				webServiceConfiguration.ProxyValidatorMethodName = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("ValidatorMethodName"), "ValidatorMethodName in node ProxyClass");
				webServiceConfiguration.ProxyDiagnosticsInfoMethodName = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("DiagnosticsInfoMethodName"), "DiagnosticsInfoMethodName in node ProxyClass");
				webServiceConfiguration.DumpDiagnosticsInfoOnSuccess = Utils.GetBoolean(xmlElement.GetAttribute("DumpDiagnosticsInfoOnSuccess"), "DumpDiagnosticsInfoOnSuccess", false);
			}
			if (!webServiceConfiguration.ProxyGenerated)
			{
				xmlElement = Utils.CheckXmlElement(configNode.SelectSingleNode("Proxy/ProxyInstanceMethod"), "ProxyInstanceMethod");
				webServiceConfiguration.ProxyInstanceMethodIsStatic = Utils.GetBoolean(xmlElement.GetAttribute("Static"), "Static", false);
				Operation operation = new Operation();
				operation.Name = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Name"), "Name");
				operation.Parameters = new List<Parameter>();
				XmlElement xmlElement2 = xmlElement.SelectSingleNode("Parameters") as XmlElement;
				if (xmlElement2 != null)
				{
					using (XmlNodeList childNodes = xmlElement2.ChildNodes)
					{
						foreach (object obj in childNodes)
						{
							XmlNode node = (XmlNode)obj;
							XmlElement xmlElement3 = Utils.CheckNode(node, "in Parameters") as XmlElement;
							if (xmlElement3 != null)
							{
								Parameter parameter = new Parameter();
								parameter.Name = xmlElement3.Name;
								parameter.IsNull = Utils.GetBoolean(xmlElement3.GetAttribute("IsNull"), "IsNull", false);
								if (!parameter.IsNull)
								{
									parameter.Type = xmlElement3.GetAttribute("Type");
									parameter.Value = xmlElement3.CloneNode(true);
								}
								operation.Parameters.Add(parameter);
							}
						}
					}
				}
				webServiceConfiguration.ProxyInstanceMethod = operation;
			}
			if (webServiceConfiguration.UseCertificateAuthentication)
			{
				xmlElement = Utils.CheckXmlElement(configNode.SelectSingleNode("Proxy/Credentials"), "Credentials");
				webServiceConfiguration.StoreLocation = Utils.GetEnumValue<StoreLocation>(xmlElement.GetAttribute("StoreLocation"), "StoreLocation");
				webServiceConfiguration.StoreName = Utils.GetEnumValue<StoreName>(xmlElement.GetAttribute("StoreName"), "StoreName");
				webServiceConfiguration.X509FindType = Utils.GetEnumValue<X509FindType>(xmlElement.GetAttribute("X509FindType"), "X509FindType");
				webServiceConfiguration.FindValue = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("FindValue"), "FindValue");
				webServiceConfiguration.ServiceCertificateValidationMode = xmlElement.GetAttribute("ServiceCertificateValidationMode");
			}
			else if (webServiceConfiguration.UseUserNameAuthentication)
			{
				xmlElement = Utils.CheckXmlElement(configNode.SelectSingleNode("Proxy/Credentials"), "Credentials");
				webServiceConfiguration.Username = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Username"), "Username");
				webServiceConfiguration.Password = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Password"), "Password");
			}
			xmlElement = (configNode.SelectSingleNode("FwLink") as XmlElement);
			if (xmlElement != null)
			{
				webServiceConfiguration.FwLinkEnabled = Utils.GetBoolean(xmlElement.GetAttribute("FwLinkEnabled"), "FwLinkEnabled", false);
				if (webServiceConfiguration.FwLinkEnabled)
				{
					webServiceConfiguration.FwLinkUri = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("FwLinkUri"), "FwLinkUri");
				}
			}
			xmlElement = (configNode.SelectSingleNode("HttpProxy") as XmlElement);
			if (xmlElement != null)
			{
				webServiceConfiguration.WebProxyEnabled = Utils.GetBoolean(xmlElement.GetAttribute("Enabled"), "Enabled in WebProxy", false);
				if (webServiceConfiguration.WebProxyEnabled)
				{
					webServiceConfiguration.UseDefaultWebProxy = Utils.GetBoolean(xmlElement.GetAttribute("UseDefaultWebProxy"), "UseDefaultWebProxy", true);
					if (!webServiceConfiguration.UseDefaultWebProxy)
					{
						webServiceConfiguration.WebProxyServerUri = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("ProxyServerUri"), "ProxyServerUri");
						webServiceConfiguration.WebProxyPort = Utils.GetInteger(xmlElement.GetAttribute("ProxyPort"), "ProxyPort", 80, 1);
					}
					webServiceConfiguration.WebProxyCredentialsRequired = Utils.GetBoolean(xmlElement.GetAttribute("CredentialsRequired"), "CredentialsRequired", false);
					webServiceConfiguration.WebProxyUsername = xmlElement.GetAttribute("ProxyUsername");
					webServiceConfiguration.WebProxyPassword = xmlElement.GetAttribute("ProxyPassword");
					webServiceConfiguration.WebProxyDomain = xmlElement.GetAttribute("ProxyDomain");
					if (webServiceConfiguration.WebProxyCredentialsRequired && (string.IsNullOrWhiteSpace(webServiceConfiguration.WebProxyUsername) || string.IsNullOrWhiteSpace(webServiceConfiguration.WebProxyPassword)))
					{
						throw new ArgumentException("Work definition error - WebProxy credentials (username and password) missing in node 'WebProxy'.");
					}
				}
			}
			return webServiceConfiguration;
		}

		private List<Operation> GetOperations(XmlElement operationsNode)
		{
			if (operationsNode.ChildNodes.Count == 0)
			{
				throw new ArgumentException("Work definition error - node 'Operation' missing.");
			}
			this.UpdateMachineUniqueParameters(operationsNode);
			List<Operation> list = new List<Operation>();
			for (int i = 0; i < operationsNode.ChildNodes.Count; i++)
			{
				Operation op = new Operation();
				XmlElement xmlElement = Utils.CheckNode(operationsNode.ChildNodes.Item(i), "Operation") as XmlElement;
				if (xmlElement != null)
				{
					op.Name = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Name"), "Name");
					op.Sla = Utils.GetTimeSpan(xmlElement.GetAttribute("Sla"), "Sla", TimeSpan.Parse("00:00:00.200"));
					op.PauseTime = Utils.GetTimeSpan(xmlElement.GetAttribute("PauseTime"), "PauseTime", TimeSpan.Zero);
					op.MaxRetryAttempts = Utils.GetInteger(Utils.GetOptionalXmlAttribute<string>(xmlElement, "MaxRetryAttempts", null), "MaxRetryAttempts", 0, 0);
					op.Id = xmlElement.GetAttribute("Id");
					Operation operation = list.Find((Operation item) => !string.IsNullOrWhiteSpace(item.Id) && item.Id == op.Id);
					if (operation != null)
					{
						throw new ArgumentException(string.Format("Work definition error - value '{0}' of attribute 'Id' in operation '{1}' is already used in operation '{2}'.", op.Id, op.Name, operation.Name));
					}
					List<Parameter> list2 = new List<Parameter>();
					XmlElement xmlElement2 = xmlElement.SelectSingleNode("Parameters") as XmlElement;
					if (xmlElement2 != null)
					{
						using (XmlNodeList childNodes = xmlElement2.ChildNodes)
						{
							foreach (object obj in childNodes)
							{
								XmlNode node = (XmlNode)obj;
								XmlElement xmlElement3 = Utils.CheckNode(node, "in Parameters") as XmlElement;
								if (xmlElement3 != null)
								{
									Parameter parameter = this.GetParameter(xmlElement3, list, op);
									list2.Add(parameter);
								}
							}
						}
					}
					op.Parameters = list2;
					List<ResultItem> list3 = new List<ResultItem>();
					XmlElement xmlElement4 = xmlElement.SelectSingleNode("ExpectedResult") as XmlElement;
					if (xmlElement4 != null)
					{
						using (XmlNodeList childNodes2 = xmlElement4.ChildNodes)
						{
							for (int j = 0; j < childNodes2.Count; j++)
							{
								XmlElement xmlElement5 = Utils.CheckNode(childNodes2[j], "in ExpectedResult") as XmlElement;
								if (xmlElement5 != null)
								{
									ResultItem expectedResultItem = this.GetExpectedResultItem(xmlElement5, op);
									list3.Add(expectedResultItem);
								}
							}
						}
					}
					op.ExpectedResultItems = list3;
					list.Add(op);
				}
			}
			return list;
		}

		private void UpdateMachineUniqueParameters(XmlElement operationsNode)
		{
			if (this.machineUniqueParameters == null || this.machineUniqueParameters.Count == 0)
			{
				return;
			}
			foreach (string text in this.machineUniqueParameters.Keys)
			{
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(operationsNode.OwnerDocument.NameTable);
				xmlNamespaceManager.AddNamespace("n", this.machineUniqueParameters[text].Item2);
				string xpath = "Operation//n:" + text;
				foreach (object obj in operationsNode.SelectNodes(xpath, xmlNamespaceManager))
				{
					XmlNode paramNode = (XmlNode)obj;
					this.UpdateMachineUniqueValue(paramNode, this.machineUniqueParameters[text].Item1);
				}
			}
		}

		private void UpdateMachineUniqueValue(XmlNode paramNode, Type type)
		{
			if (type == typeof(string))
			{
				paramNode.InnerText = Environment.MachineName + paramNode.InnerText;
				return;
			}
			if (type == typeof(Guid))
			{
				byte[] array = new Guid(paramNode.InnerText).ToByteArray();
				Array.Copy(this.macAddress.GetAddressBytes(), 0, array, 10, 6);
				paramNode.InnerText = new Guid(array).ToString();
				return;
			}
			if (type == typeof(string[]))
			{
				using (IEnumerator enumerator = paramNode.ChildNodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode xmlNode = (XmlNode)obj;
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement != null)
						{
							this.UpdateMachineUniqueValue(xmlElement, typeof(string));
						}
					}
					return;
				}
			}
			throw new NotImplementedException(string.Format("UpdateMachineUniqueValue not implemented for type:{0}", type.Name));
		}

		private Parameter GetParameter(XmlElement paramNode, List<Operation> operations, Operation op)
		{
			Parameter param = new Parameter();
			param.Name = paramNode.Name;
			param.IsNull = Utils.GetBoolean(paramNode.GetAttribute("IsNull"), "IsNull", false);
			if (!param.IsNull)
			{
				param.UseResultFromOperationId = paramNode.GetAttribute("UseResultFromOperationId");
				if (!string.IsNullOrWhiteSpace(param.UseResultFromOperationId))
				{
					if (operations.Find((Operation item) => !string.IsNullOrWhiteSpace(item.Id) && item.Id.Equals(param.UseResultFromOperationId, StringComparison.OrdinalIgnoreCase)) == null)
					{
						throw new ArgumentException(string.Format("Work definition error - value '{0}' of attribute 'UseResultFromOperationId' invalid in node '{1}' in operation '{2}'.", param.UseResultFromOperationId, param.Name, op.Name));
					}
					param.PropertyName = paramNode.GetAttribute("PropertyName");
					param.Index = Utils.GetInteger(paramNode.GetAttribute("Index"), "Index", -1, 0);
				}
				if (string.IsNullOrWhiteSpace(param.UseResultFromOperationId) && string.IsNullOrWhiteSpace(paramNode.InnerXml))
				{
					throw new ArgumentException(string.Format("Work definition error - node '{0}' and attribute 'UseResultFromOperationId' both empty in operation '{1}'.", param.Name, op.Name));
				}
				if (!string.IsNullOrWhiteSpace(param.UseResultFromOperationId) && !string.IsNullOrWhiteSpace(paramNode.InnerXml))
				{
					throw new ArgumentException(string.Format("Work definition error - node '{0}' and attribute 'UseResultFromOperationId' both non-empty in operation '{1}'.", param.Name, op.Name));
				}
				param.UseFile = Utils.GetBoolean(paramNode.GetAttribute("UseFile"), "UseFile", false);
				if (!string.IsNullOrWhiteSpace(param.UseResultFromOperationId) && param.UseFile)
				{
					throw new ArgumentException(string.Format("Work definition error - attribute 'UseResultFromOperationId' is not null or empty and attribute 'UseFile' is set to true in node '{0}' in operation '{1}'.", param.Name, op.Name));
				}
				if (param.UseFile)
				{
					param.Type = Utils.CheckNullOrWhiteSpace(paramNode.GetAttribute("Type"), "Type");
					Utils.CheckNullOrWhiteSpace(paramNode.InnerText, param.Name);
					XElement.Load(paramNode.InnerText);
				}
				else
				{
					param.Type = paramNode.GetAttribute("Type");
				}
				param.Value = paramNode.CloneNode(true);
			}
			return param;
		}

		private ResultItem GetExpectedResultItem(XmlElement itemNode, Operation op)
		{
			ResultItem resultItem = new ResultItem();
			ResultVerifyMethod resultVerifyMethod;
			if (!Enum.TryParse<ResultVerifyMethod>(itemNode.Name, true, out resultVerifyMethod))
			{
				throw new ArgumentException(string.Format("Work definition error - node '{0}' invalid in operation '{1}'.", itemNode.Name, op.Name));
			}
			resultItem.VerifyMethod = resultVerifyMethod;
			switch (resultVerifyMethod)
			{
			case ResultVerifyMethod.ReturnType:
				resultItem.Value = Utils.CheckNullOrWhiteSpace(itemNode.InnerXml, resultVerifyMethod.ToString());
				return resultItem;
			case ResultVerifyMethod.ReturnValue:
			case ResultVerifyMethod.ReturnValueRegex:
			case ResultVerifyMethod.ReturnValueContains:
				resultItem.Value = Utils.CheckNullOrWhiteSpace(itemNode.InnerXml, resultVerifyMethod.ToString());
				resultItem.Index = Utils.GetInteger(itemNode.GetAttribute("Index"), "Index", -1, 0);
				return resultItem;
			case ResultVerifyMethod.ReturnValueXml:
			case ResultVerifyMethod.PropertyValueXml:
				resultItem.Value = Utils.CheckNullOrWhiteSpace(itemNode.InnerXml, resultVerifyMethod.ToString());
				resultItem.Index = Utils.GetInteger(itemNode.GetAttribute("Index"), "Index", -1, 0);
				if (resultVerifyMethod == ResultVerifyMethod.PropertyValueXml)
				{
					resultItem.PropertyName = Utils.CheckNullOrWhiteSpace(itemNode.GetAttribute("PropertyName"), "PropertyName");
				}
				try
				{
					resultItem.UseFile = Utils.GetBoolean(itemNode.GetAttribute("UseFile"), "UseFile", false);
					if (resultItem.UseFile)
					{
						Utils.CheckNullOrWhiteSpace(resultItem.Value, resultVerifyMethod.ToString());
						XElement.Load(resultItem.Value);
					}
					else
					{
						XElement.Parse(resultItem.Value);
					}
					return resultItem;
				}
				catch (Exception innerException)
				{
					throw new ArgumentException(string.Format("Work definition error - failed to load an XElement in node '{0}' in operation '{1}' with string value:\r\n{2}", resultVerifyMethod.ToString(), op.Name, resultItem.Value), innerException);
				}
				break;
			case ResultVerifyMethod.ReturnValueIsNull:
			case ResultVerifyMethod.ReturnValueIsNotNull:
				return resultItem;
			case ResultVerifyMethod.ReturnValueUseValidator:
				goto IL_207;
			case ResultVerifyMethod.PropertyValue:
			case ResultVerifyMethod.PropertyValueRegex:
			case ResultVerifyMethod.PropertyValueContains:
				break;
			default:
				goto IL_207;
			}
			resultItem.Value = Utils.CheckNullOrWhiteSpace(itemNode.InnerXml, resultVerifyMethod.ToString());
			resultItem.PropertyName = Utils.CheckNullOrWhiteSpace(itemNode.GetAttribute("PropertyName"), "PropertyName");
			resultItem.Index = Utils.GetInteger(itemNode.GetAttribute("Index"), "Index", -1, 0);
			return resultItem;
			IL_207:
			resultItem.Value = Utils.CheckNullOrWhiteSpace(itemNode.InnerXml, resultVerifyMethod.ToString());
			return resultItem;
		}

		private void CheckSslCertificate(XmlElement node)
		{
			if (Utils.GetBoolean(node.GetAttribute("IgnoreInvalidSslCertificateError"), "IgnoreInvalidSslCertificateError", false))
			{
				ServicePointManager.ServerCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
			}
		}

		private WebServiceConfiguration webServiceConfiguration = new WebServiceConfiguration();

		private Dictionary<string, Tuple<Type, string>> machineUniqueParameters;

		private List<Operation> operations;

		private PhysicalAddress macAddress;
	}
}
