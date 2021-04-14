using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class ScriptingAgentConfiguration
	{
		public ScriptingAgentConfiguration(string xmlConfigPath)
		{
			this.provisionDefaultPropertiesDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.provisionDefaultPropertiesDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.provisionPhysicalPropertiesDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.updateAffectedIConfigurableDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.validateDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.onCompleteDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.supportedCmdlets = new List<string>();
			Exception ex = null;
			try
			{
				this.Initialize(xmlConfigPath);
			}
			catch (FileNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				ex = ex3;
			}
			catch (XmlException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new ProvisioningException(Strings.ScriptingAgentInitializationFailed(ex.Message), ex);
			}
		}

		public Dictionary<string, string> ProvisionDefaultPropertiesDictionary
		{
			get
			{
				return this.provisionDefaultPropertiesDictionary;
			}
		}

		public Dictionary<string, string> ProvisionPhysicalPropertiesDictionary
		{
			get
			{
				return this.provisionPhysicalPropertiesDictionary;
			}
		}

		public Dictionary<string, string> UpdateAffectedIConfigurableDictionary
		{
			get
			{
				return this.updateAffectedIConfigurableDictionary;
			}
		}

		public Dictionary<string, string> ValidateDictionary
		{
			get
			{
				return this.validateDictionary;
			}
		}

		public Dictionary<string, string> OnCompleteDictionary
		{
			get
			{
				return this.onCompleteDictionary;
			}
		}

		public string CommonFunctions
		{
			get
			{
				return this.commonFunctionsBuffer;
			}
		}

		public void Initialize(string xmlConfigPath)
		{
			if (!File.Exists(xmlConfigPath))
			{
				throw new FileNotFoundException(Strings.ErrorFileIsNotFound(xmlConfigPath));
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(xmlConfigPath);
			XmlElement documentElement = safeXmlDocument.DocumentElement;
			if (documentElement == null)
			{
				throw new InvalidOperationException(Strings.XmlErrorMissingNode("xml", xmlConfigPath));
			}
			XmlNodeList xmlNodeList = documentElement.SelectNodes("/Configuration/Feature");
			if (xmlNodeList == null)
			{
				throw new InvalidOperationException(Strings.XmlErrorMissingNode("/Configuration/Feature", xmlConfigPath));
			}
			foreach (object obj in xmlNodeList)
			{
				XmlElement xmlElement = (XmlElement)obj;
				string attribute = xmlElement.GetAttribute("Cmdlets");
				if (attribute == null)
				{
					throw new InvalidOperationException(Strings.XmlErrorMissingAttribute("cmdlets", xmlConfigPath));
				}
				string[] array = attribute.Trim().Split(new char[]
				{
					','
				});
				XmlNodeList childNodes = xmlElement.ChildNodes;
				if (childNodes.Count == 0)
				{
					throw new InvalidOperationException(Strings.XmlErrorMissingNode("<ApiCall>", xmlConfigPath));
				}
				foreach (object obj2 in childNodes)
				{
					XmlElement xmlElement2 = (XmlElement)obj2;
					string innerText = xmlElement2.InnerText;
					if (innerText == null)
					{
						throw new XmlException(Strings.XmlErrorMissingInnerText(xmlConfigPath));
					}
					string text = xmlElement2.GetAttribute("Name");
					if (text == null)
					{
						throw new InvalidOperationException(Strings.XmlErrorMissingAttribute("Name", xmlConfigPath));
					}
					text = text.Trim();
					string[] array2 = array;
					int i = 0;
					while (i < array2.Length)
					{
						string cmdletName = array2[i];
						string a;
						if ((a = text) != null)
						{
							if (!(a == "Validate"))
							{
								if (!(a == "OnComplete"))
								{
									if (!(a == "ProvisionDefaultProperties"))
									{
										if (!(a == "ProvisionPhysicalProperties"))
										{
											if (!(a == "UpdateAffectedIConfigurable"))
											{
												goto IL_21B;
											}
											this.AddScriptToDictionary(innerText, cmdletName, this.UpdateAffectedIConfigurableDictionary);
										}
										else
										{
											this.AddScriptToDictionary(innerText, cmdletName, this.ProvisionPhysicalPropertiesDictionary);
										}
									}
									else
									{
										this.AddScriptToDictionary(innerText, cmdletName, this.ProvisionDefaultPropertiesDictionary);
									}
								}
								else
								{
									this.AddScriptToDictionary(innerText, cmdletName, this.OnCompleteDictionary);
								}
							}
							else
							{
								this.AddScriptToDictionary(innerText, cmdletName, this.ValidateDictionary);
							}
							i++;
							continue;
						}
						IL_21B:
						throw new InvalidOperationException(Strings.XmlErrorWrongAPI(xmlConfigPath, text));
					}
				}
			}
			XmlNode xmlNode = documentElement.SelectSingleNode("/Configuration/Common");
			if (xmlNode != null)
			{
				this.commonFunctionsBuffer = xmlNode.InnerText;
				return;
			}
			this.commonFunctionsBuffer = string.Empty;
		}

		private void AddScriptToDictionary(string scriptletCode, string cmdletName, Dictionary<string, string> dictionary)
		{
			if (!this.supportedCmdlets.Contains(cmdletName))
			{
				this.supportedCmdlets.Add(cmdletName);
			}
			string arg;
			if (dictionary.TryGetValue(cmdletName, out arg))
			{
				dictionary[cmdletName] = string.Format("{0};\r\n{1}", arg, scriptletCode);
				return;
			}
			dictionary[cmdletName] = scriptletCode;
		}

		public IEnumerable<string> GetAllSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		private Dictionary<string, string> provisionDefaultPropertiesDictionary;

		private Dictionary<string, string> provisionPhysicalPropertiesDictionary;

		private Dictionary<string, string> updateAffectedIConfigurableDictionary;

		private Dictionary<string, string> validateDictionary;

		private Dictionary<string, string> onCompleteDictionary;

		private List<string> supportedCmdlets;

		private string commonFunctionsBuffer;
	}
}
