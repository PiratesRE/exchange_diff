using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[LocDescription(Strings.IDs.SetAppConfigValue)]
	[Cmdlet("Set", "AppConfigValue", DefaultParameterSetName = "Attribute")]
	public class SetAppConfigValue : Task
	{
		public SetAppConfigValue()
		{
			TaskLogger.LogEnter();
			this.OldValue = string.Empty;
			TaskLogger.LogExit();
		}

		[Parameter(Mandatory = true)]
		public string ConfigFileFullPath
		{
			get
			{
				return (string)base.Fields["ConfigFileFullPath"];
			}
			set
			{
				base.Fields["ConfigFileFullPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string Element
		{
			get
			{
				return (string)base.Fields["Element"];
			}
			set
			{
				base.Fields["Element"] = value;
			}
		}

		[Parameter(ParameterSetName = "Attribute", Mandatory = true)]
		public string Attribute
		{
			get
			{
				return (string)base.Fields["Attribute"];
			}
			set
			{
				base.Fields["Attribute"] = value;
			}
		}

		[Parameter(ParameterSetName = "Remove", Mandatory = false)]
		[Parameter(ParameterSetName = "AppSettingKey", Mandatory = true)]
		public string AppSettingKey
		{
			get
			{
				return (string)base.Fields["AppSettingKey"];
			}
			set
			{
				base.Fields["AppSettingKey"] = value;
			}
		}

		[AllowEmptyString]
		[Parameter(ParameterSetName = "AppSettingKey", Mandatory = true)]
		[Parameter(ParameterSetName = "Attribute", Mandatory = true)]
		public string NewValue
		{
			get
			{
				return (string)base.Fields["NewValue"];
			}
			set
			{
				base.Fields["NewValue"] = value;
			}
		}

		[Parameter(ParameterSetName = "AppSettingKey", Mandatory = false)]
		[Parameter(ParameterSetName = "Attribute", Mandatory = false)]
		public string OldValue
		{
			get
			{
				return (string)base.Fields["OldValue"];
			}
			set
			{
				base.Fields["OldValue"] = value;
			}
		}

		[Parameter(ParameterSetName = "ListValues", Mandatory = true)]
		public MultiValuedProperty<string> ListValues
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ListValues"];
			}
			set
			{
				base.Fields["ListValues"] = value;
			}
		}

		[Parameter(ParameterSetName = "Attribute", Mandatory = false)]
		[Parameter(ParameterSetName = "XmlNode", Mandatory = false)]
		public SwitchParameter InsertAsFirst
		{
			get
			{
				return (SwitchParameter)(base.Fields["InsertAsFirst"] ?? false);
			}
			set
			{
				base.Fields["InsertAsFirst"] = value;
			}
		}

		[Parameter(ParameterSetName = "Remove", Mandatory = true)]
		public SwitchParameter Remove
		{
			get
			{
				return (SwitchParameter)(base.Fields["Remove"] ?? false);
			}
			set
			{
				base.Fields["Remove"] = value;
			}
		}

		[Parameter(ParameterSetName = "XmlNode", Mandatory = true)]
		public XmlNode XmlNode
		{
			get
			{
				return (XmlNode)base.Fields["XmlNode"];
			}
			set
			{
				base.Fields["XmlNode"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			try
			{
				int num = 0;
				try
				{
					IL_0D:
					safeXmlDocument.Load(this.ConfigFileFullPath);
					XmlElement xmlElement = this.FindOrCreateElement(safeXmlDocument, base.ParameterSetName != "Remove");
					if (xmlElement == null)
					{
						if (base.ParameterSetName == "Remove")
						{
							this.WriteWarning(Strings.ElementNotFound(this.Element));
						}
						return;
					}
					string parameterSetName;
					if ((parameterSetName = base.ParameterSetName) != null)
					{
						if (parameterSetName == "AppSettingKey")
						{
							this.AddAppSettingEntry(xmlElement);
							goto IL_F3;
						}
						if (parameterSetName == "Attribute")
						{
							this.AddAttribute(xmlElement);
							goto IL_F3;
						}
						if (parameterSetName == "ListValues")
						{
							this.AddListValues(xmlElement);
							goto IL_F3;
						}
						if (parameterSetName == "Remove")
						{
							this.RemoveElementOrAppSettingEntry(xmlElement);
							goto IL_F3;
						}
						if (parameterSetName == "XmlNode")
						{
							this.SetXmlNode(safeXmlDocument, xmlElement);
							goto IL_F3;
						}
					}
					base.WriteError(new InvalidOperationException("invalid parameter set name"), (ErrorCategory)1003, null);
					IL_F3:
					safeXmlDocument.Save(this.ConfigFileFullPath);
				}
				catch (IOException exception)
				{
					num++;
					if (num > 3)
					{
						base.WriteError(exception, (ErrorCategory)1003, null);
					}
					else
					{
						this.WriteWarning(Strings.AppConfigIOException(this.ConfigFileFullPath, 3, 3));
						Thread.Sleep(3000);
					}
					goto IL_0D;
				}
			}
			catch (XmlException exception2)
			{
				base.WriteError(exception2, (ErrorCategory)1003, null);
			}
			catch (ArgumentException exception3)
			{
				base.WriteError(exception3, (ErrorCategory)1003, null);
			}
			catch (UnauthorizedAccessException exception4)
			{
				base.WriteError(exception4, (ErrorCategory)1003, null);
			}
			catch (NotSupportedException exception5)
			{
				base.WriteError(exception5, (ErrorCategory)1003, null);
			}
			catch (SecurityException exception6)
			{
				base.WriteError(exception6, (ErrorCategory)1003, null);
			}
			TaskLogger.LogExit();
		}

		private XmlElement FindOrCreateElement(SafeXmlDocument xmlDoc, bool create)
		{
			string[] array = this.Element.Split(new char[]
			{
				'\\',
				'/'
			});
			XmlNode xmlNode = null;
			bool flag = true;
			string text = array[0];
			using (XmlNodeList xmlNodeList = xmlDoc.SelectNodes(text))
			{
				if (xmlNodeList.Count != 1)
				{
					base.WriteError(new LocalizedException(Strings.RootNodeDoesNotMatch(text, this.ConfigFileFullPath)), (ErrorCategory)1003, null);
				}
				XmlNode xmlNode2;
				xmlNode = (xmlNode2 = xmlNodeList[0]);
				int i = 1;
				while (i < array.GetLength(0))
				{
					text = array[i];
					if (flag)
					{
						using (XmlNodeList xmlNodeList2 = xmlNode2.SelectNodes(text))
						{
							if (xmlNodeList2.Count > 1)
							{
								base.WriteError(new LocalizedException(Strings.NodeNotUnique(text, this.ConfigFileFullPath)), (ErrorCategory)1003, null);
							}
							else if (xmlNodeList2.Count == 1)
							{
								xmlNode = xmlNodeList2[0];
							}
							else
							{
								if (!create)
								{
									return null;
								}
								xmlNode = this.CreateChild(xmlDoc, xmlNode2, text);
								flag = false;
							}
							goto IL_F9;
						}
						goto IL_E5;
					}
					goto IL_E5;
					IL_F9:
					xmlNode2 = xmlNode;
					i++;
					continue;
					IL_E5:
					if (create)
					{
						xmlNode = this.CreateChild(xmlDoc, xmlNode2, text);
						goto IL_F9;
					}
					return null;
				}
			}
			XmlElement xmlElement = xmlNode as XmlElement;
			if (xmlElement == null)
			{
				base.WriteError(new LocalizedException(Strings.NodeNotElement(xmlNode.Name, this.ConfigFileFullPath)), (ErrorCategory)1003, null);
			}
			return xmlElement;
		}

		private void AddAppSettingEntry(XmlElement currElement)
		{
			XmlDocument ownerDocument = currElement.OwnerDocument;
			XmlElement xmlElement;
			if (this.TryFindAppSettingEntry(currElement, out xmlElement))
			{
				XmlAttribute xmlAttribute = xmlElement.GetAttributeNode("value");
				if (this.ShouldUpdate(this.OldValue, this.NewValue, xmlAttribute.Value, StringComparison.OrdinalIgnoreCase))
				{
					string value = xmlAttribute.Value;
					xmlAttribute.Value = this.NewValue;
					XmlComment newChild = ownerDocument.CreateComment(string.Format("Exchange cmdlet Set-AppConfigValue updated value for dictionary key {0} from {1} to {2} at {3}", new object[]
					{
						this.AppSettingKey,
						value,
						this.NewValue,
						DateTime.UtcNow.ToLocalTime()
					}));
					currElement.InsertBefore(newChild, xmlElement);
					return;
				}
			}
			else
			{
				XmlElement xmlElement2 = ownerDocument.CreateElement("add");
				currElement.AppendChild(xmlElement2);
				XmlAttribute xmlAttribute2 = ownerDocument.CreateAttribute("key");
				xmlAttribute2.Value = this.AppSettingKey;
				xmlElement2.SetAttributeNode(xmlAttribute2);
				XmlAttribute xmlAttribute = ownerDocument.CreateAttribute("value");
				xmlAttribute.Value = this.NewValue;
				xmlElement2.SetAttributeNode(xmlAttribute);
			}
		}

		private bool TryFindAppSettingEntry(XmlElement currElement, out XmlElement foundEntry)
		{
			using (XmlNodeList xmlNodeList = currElement.SelectNodes("add"))
			{
				foundEntry = null;
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlElement xmlElement = xmlNode as XmlElement;
					if (xmlElement == null)
					{
						base.WriteError(new LocalizedException(Strings.NodeNotElement("add", this.ConfigFileFullPath)), (ErrorCategory)1003, null);
					}
					if (xmlElement.HasAttribute("key") && xmlElement.HasAttribute("value"))
					{
						XmlAttribute attributeNode = xmlElement.GetAttributeNode("key");
						if (this.AppSettingKey.Equals(attributeNode.Value))
						{
							foundEntry = xmlElement;
							return true;
						}
					}
				}
			}
			return false;
		}

		private void AddAttribute(XmlElement currElement)
		{
			XmlAttribute xmlAttribute = currElement.GetAttributeNode(this.Attribute);
			XmlDocument ownerDocument = currElement.OwnerDocument;
			if (xmlAttribute == null)
			{
				xmlAttribute = ownerDocument.CreateAttribute(this.Attribute);
				xmlAttribute.Value = this.NewValue;
				currElement.SetAttributeNode(xmlAttribute);
				return;
			}
			if (this.ShouldUpdate(this.OldValue, this.NewValue, xmlAttribute.Value, StringComparison.OrdinalIgnoreCase))
			{
				string value = xmlAttribute.Value;
				xmlAttribute.Value = this.NewValue;
				XmlComment newChild = ownerDocument.CreateComment(string.Format("Exchange cmdlet Set-AppConfigValue updated value of attribute {0} from {1} to {2} at {3}", new object[]
				{
					this.Attribute,
					value,
					this.NewValue,
					DateTime.UtcNow.ToLocalTime()
				}));
				currElement.ParentNode.InsertBefore(newChild, currElement);
			}
		}

		private void RemoveElementOrAppSettingEntry(XmlElement currElement)
		{
			if (this.AppSettingKey != null)
			{
				this.RemoveAppSettingEntry(currElement);
				return;
			}
			this.RemoveElement(currElement);
		}

		private void SetXmlNode(XmlDocument xmlDoc, XmlElement currElement)
		{
			XmlNode xmlNode = xmlDoc.ImportNode(this.XmlNode, true);
			if (!string.Equals(currElement.Name, xmlNode.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException(Strings.ImportNodeNameDoesNotMatch(xmlNode.Name, currElement.Name));
			}
			currElement.ParentNode.AppendChild(xmlNode);
			currElement.ParentNode.RemoveChild(currElement);
		}

		private void RemoveElement(XmlElement currElement)
		{
			if (currElement.ParentNode is XmlDocument)
			{
				base.WriteError(new LocalizedException(Strings.RootNodeCannotBeRemoved(currElement.Name, this.ConfigFileFullPath)), (ErrorCategory)1003, null);
			}
			currElement.ParentNode.RemoveChild(currElement);
		}

		private void RemoveAppSettingEntry(XmlElement currElement)
		{
			XmlDocument ownerDocument = currElement.OwnerDocument;
			XmlElement xmlElement = null;
			if (this.TryFindAppSettingEntry(currElement, out xmlElement))
			{
				XmlComment newChild = ownerDocument.CreateComment(string.Format("Exchange cmdlet Set-AppConfigValue removed dictionary key {0} at {1}", this.AppSettingKey, DateTime.UtcNow.ToLocalTime()));
				currElement.InsertBefore(newChild, xmlElement);
				currElement.RemoveChild(xmlElement);
				return;
			}
			this.WriteWarning(Strings.AppSettingKeyNotFound(this.AppSettingKey));
		}

		private bool ShouldUpdate(string expectedValue, string newValue, string actualValue, StringComparison stringComparison)
		{
			return (string.IsNullOrEmpty(expectedValue) || actualValue.Equals(expectedValue, stringComparison)) && !actualValue.Equals(newValue, stringComparison);
		}

		private XmlNode CreateChild(SafeXmlDocument xmlDoc, XmlNode parentNode, string childName)
		{
			string name = childName;
			int num = childName.IndexOf("[");
			if (num >= 0)
			{
				name = childName.Substring(0, num);
			}
			XmlNode xmlNode = xmlDoc.CreateElement(name);
			XmlNode firstChild = parentNode.FirstChild;
			if (this.InsertAsFirst && firstChild != null)
			{
				parentNode.InsertBefore(xmlNode, firstChild);
			}
			else
			{
				parentNode.AppendChild(xmlNode);
			}
			return xmlNode;
		}

		private void AddListValues(XmlElement currElement)
		{
			XmlDocument ownerDocument = currElement.OwnerDocument;
			currElement.RemoveAll();
			foreach (string value in this.ListValues)
			{
				XmlElement xmlElement = ownerDocument.CreateElement("add");
				currElement.AppendChild(xmlElement);
				XmlAttribute xmlAttribute = ownerDocument.CreateAttribute("value");
				xmlAttribute.Value = value;
				xmlElement.SetAttributeNode(xmlAttribute);
			}
		}

		private const string AttributeParameterSetName = "Attribute";

		private const string AppSettingKeyParameterSetName = "AppSettingKey";

		private const string ListValuesParameterSetName = "ListValues";

		private const string RemoveParameterSetName = "Remove";

		private const string XmlNodeParameterSetName = "XmlNode";
	}
}
