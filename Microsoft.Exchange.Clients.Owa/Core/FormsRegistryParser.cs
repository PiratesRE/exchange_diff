using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FormsRegistryParser
	{
		internal FormsRegistry Registry
		{
			get
			{
				return this.registry;
			}
		}

		internal ArrayList ClientMappings
		{
			get
			{
				return this.clientMappings;
			}
		}

		internal ArrayList BaseClientMappings
		{
			get
			{
				return this.baseClientMappings;
			}
		}

		internal FormsRegistryParser()
		{
			if (FormsRegistryParser.nameTable == null)
			{
				FormsRegistryParser.BuildNameTable();
			}
		}

		private static void BuildNameTable()
		{
			FormsRegistryParser.nameTable = new NameTable();
			FormsRegistryParser.nameTableValues = new string[19];
			FormsRegistryParser.nameTableValues[0] = FormsRegistryParser.nameTable.Add("Registry");
			FormsRegistryParser.nameTableValues[1] = FormsRegistryParser.nameTable.Add("Name");
			FormsRegistryParser.nameTableValues[2] = FormsRegistryParser.nameTable.Add("BaseExperience");
			FormsRegistryParser.nameTableValues[3] = FormsRegistryParser.nameTable.Add("InheritsFrom");
			FormsRegistryParser.nameTableValues[4] = FormsRegistryParser.nameTable.Add("IsRichClient");
			FormsRegistryParser.nameTableValues[5] = FormsRegistryParser.nameTable.Add("Experience");
			FormsRegistryParser.nameTableValues[6] = FormsRegistryParser.nameTable.Add("Client");
			FormsRegistryParser.nameTableValues[7] = FormsRegistryParser.nameTable.Add("Application");
			FormsRegistryParser.nameTableValues[8] = FormsRegistryParser.nameTable.Add("MinimumVersion");
			FormsRegistryParser.nameTableValues[9] = FormsRegistryParser.nameTable.Add("Platform");
			FormsRegistryParser.nameTableValues[10] = FormsRegistryParser.nameTable.Add("Control");
			FormsRegistryParser.nameTableValues[11] = FormsRegistryParser.nameTable.Add("ApplicationElement");
			FormsRegistryParser.nameTableValues[12] = FormsRegistryParser.nameTable.Add("ElementClass");
			FormsRegistryParser.nameTableValues[13] = FormsRegistryParser.nameTable.Add("Mapping");
			FormsRegistryParser.nameTableValues[14] = FormsRegistryParser.nameTable.Add("Value");
			FormsRegistryParser.nameTableValues[15] = FormsRegistryParser.nameTable.Add("Form");
			FormsRegistryParser.nameTableValues[16] = FormsRegistryParser.nameTable.Add("Action");
			FormsRegistryParser.nameTableValues[17] = FormsRegistryParser.nameTable.Add("State");
			FormsRegistryParser.nameTableValues[18] = FormsRegistryParser.nameTable.Add("RequiredFeatures");
		}

		internal void Load(string registryFile, string folder)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "FormsRegistryParser.Load  registry file = {0}, folder = {1}", registryFile, folder);
			this.registryFile = registryFile;
			bool flag = false;
			string name = string.Empty;
			string inheritsFrom = string.Empty;
			string baseExperience = string.Empty;
			bool isRichClient = false;
			try
			{
				this.reader = SafeXmlFactory.CreateSafeXmlTextReader(registryFile);
				this.reader.WhitespaceHandling = WhitespaceHandling.None;
				this.registry = new FormsRegistry();
				this.reader.Read();
				if (this.reader.Name != FormsRegistryParser.nameTableValues[0])
				{
					this.ThrowExpectedElementException(FormsRegistryParser.NameTableValues.Registry);
				}
				if (!this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[1]))
				{
					this.ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues.Name);
				}
				name = this.reader.Value;
				if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[3]))
				{
					inheritsFrom = this.reader.Value;
					if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[2]))
					{
						this.ThrowParserException("BaseExperience is not valid when inheriting from another registry. The BaseExperience from the inherited registry is used instead", ClientsEventLogConstants.Tuple_FormsRegistryInvalidUserOfBaseExperience, new object[]
						{
							registryFile,
							this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
							this.reader.LinePosition.ToString(CultureInfo.InvariantCulture)
						});
					}
				}
				else if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[2]))
				{
					baseExperience = this.reader.Value;
				}
				else
				{
					this.ThrowParserException("Expected BaseExperience or InheritsFrom attribute", ClientsEventLogConstants.Tuple_FormsRegistryExpectedBaseExperienceOrInheritsFrom, new object[]
					{
						registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture)
					});
				}
				if (!this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[4]))
				{
					this.ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues.IsRichClient);
				}
				try
				{
					isRichClient = bool.Parse(this.reader.Value);
				}
				catch (FormatException)
				{
					this.ThrowParserException("Expected a valid boolean value in IsRichClient property", ClientsEventLogConstants.Tuple_FormsRegistryInvalidUserOfIsRichClient, new object[]
					{
						registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
						this.reader.Value
					});
				}
				try
				{
					this.registry.Initialize(name, baseExperience, inheritsFrom, folder, isRichClient);
					goto IL_33E;
				}
				catch (OwaInvalidInputException ex)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_FormsRegistryParseError, ex.Message, new object[]
					{
						registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
						ex.Message
					});
					throw ex;
				}
				IL_2D4:
				XmlNodeType nodeType = this.reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (this.reader.Name == FormsRegistryParser.nameTableValues[0])
						{
							flag = true;
						}
					}
				}
				else if (this.reader.Name == FormsRegistryParser.nameTableValues[5])
				{
					this.clientMappings.AddRange(this.ParseExperience());
				}
				else
				{
					this.ThrowExpectedElementException(FormsRegistryParser.NameTableValues.Experience);
				}
				IL_33E:
				if (!flag && this.reader.Read())
				{
					goto IL_2D4;
				}
			}
			catch (XmlException ex2)
			{
				this.ThrowParserException(ex2.Message, ClientsEventLogConstants.Tuple_FormsRegistryParseError, new object[]
				{
					registryFile,
					this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
					this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
					ex2.Message
				});
			}
			finally
			{
				if (this.reader != null)
				{
					this.reader.Close();
				}
			}
		}

		private ArrayList ParseExperience()
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistryParser.ParseExperience");
			ArrayList arrayList = new ArrayList();
			bool flag = false;
			string text = string.Empty;
			if (!this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[1]))
			{
				this.ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues.Name);
			}
			text = this.reader.Value;
			ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<string>((long)this.GetHashCode(), "experience = {0}", text);
			bool flag2 = this.registry.BaseExperience == text;
			while (!flag && this.reader.Read())
			{
				XmlNodeType nodeType = this.reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (this.reader.Name == FormsRegistryParser.nameTableValues[5])
						{
							flag = true;
						}
					}
				}
				else if (this.reader.Name == FormsRegistryParser.nameTableValues[6])
				{
					ClientMapping clientMapping = this.ParseClientMapping();
					clientMapping.Experience = new Experience(text, this.registry);
					ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<ClientMapping>((long)this.GetHashCode(), "Parsed ClientMapping - {0}", clientMapping);
					arrayList.Add(clientMapping);
					if (flag2)
					{
						this.baseClientMappings.Add(clientMapping);
					}
				}
				else if (this.reader.Name == FormsRegistryParser.nameTableValues[11])
				{
					this.ParseApplicationElement(text);
				}
				else
				{
					this.ThrowParserException("Expected Client or ApplicationElement element", ClientsEventLogConstants.Tuple_FormsRegistryExpectedClientOrApplicationElement, new object[]
					{
						this.registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture)
					});
				}
			}
			return arrayList;
		}

		private ClientMapping ParseClientMapping()
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistryParser.ParseClientMapping");
			ClientMapping clientMapping = new ClientMapping();
			UserAgentParser.UserAgentVersion minimumVersion = default(UserAgentParser.UserAgentVersion);
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[7]))
			{
				clientMapping.Application = this.reader.Value;
			}
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[9]))
			{
				clientMapping.Platform = this.reader.Value;
			}
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[8]))
			{
				try
				{
					minimumVersion = new UserAgentParser.UserAgentVersion(this.reader.Value);
				}
				catch (ArgumentException)
				{
					this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Unable to parse MinimumVersion attribute value = {0}", new object[]
					{
						this.reader.Value
					}), ClientsEventLogConstants.Tuple_FormsRegistryInvalidMinimumVersion, new object[]
					{
						this.registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
						this.reader.Value
					});
				}
			}
			clientMapping.MinimumVersion = minimumVersion;
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[10]))
			{
				object obj = FormsRegistry.ClientControlParser.Parse(this.reader.Value);
				if (obj == null)
				{
					try
					{
						minimumVersion = new UserAgentParser.UserAgentVersion(this.reader.Value);
					}
					catch (ArgumentException)
					{
						this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Invalid ClientControl: {0}", new object[]
						{
							this.reader.Value
						}), ClientsEventLogConstants.Tuple_FormsRegistryInvalidClientControl, new object[]
						{
							this.registryFile,
							this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
							this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
							this.reader.Value
						});
					}
				}
				clientMapping.Control = (ClientControl)obj;
			}
			return clientMapping;
		}

		private void ParseApplicationElement(string experience)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistryParser.ParseApplicationElement");
			bool flag = false;
			if (!this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[1]))
			{
				this.ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues.Name);
			}
			object obj = FormsRegistry.ApplicationElementParser.Parse(this.reader.Value);
			if (obj == null)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Invalid ApplicationElement: {0}", new object[]
				{
					this.reader.Value
				}), ClientsEventLogConstants.Tuple_FormsRegistryInvalidApplicationElement, new object[]
				{
					this.registryFile,
					this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
					this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
					this.reader.Value
				});
			}
			ApplicationElement applicationElement = (ApplicationElement)obj;
			while (!flag && this.reader.Read())
			{
				XmlNodeType nodeType = this.reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (this.reader.Name == FormsRegistryParser.nameTableValues[11])
						{
							flag = true;
						}
					}
				}
				else if (this.reader.Name == FormsRegistryParser.nameTableValues[12])
				{
					this.ParseElementClass(experience, applicationElement);
				}
				else
				{
					this.ThrowExpectedElementException(FormsRegistryParser.NameTableValues.ElementClass);
				}
			}
		}

		private void ParseElementClass(string experience, ApplicationElement applicationElement)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "FormsRegistryParser.ParseElementClass");
			bool flag = false;
			string itemClass = string.Empty;
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[14]))
			{
				itemClass = this.reader.Value;
			}
			ulong num = 0UL;
			if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[18]))
			{
				num = this.ParseRequiredFeatures(this.reader.Value);
			}
			while (!flag && this.reader.Read())
			{
				XmlNodeType nodeType = this.reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (this.reader.Name == FormsRegistryParser.nameTableValues[12])
						{
							flag = true;
						}
					}
				}
				else
				{
					if (this.reader.Name == FormsRegistryParser.nameTableValues[13])
					{
						string text = string.Empty;
						string action = string.Empty;
						string state = string.Empty;
						ulong segmentationFlags = num;
						if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[16]))
						{
							action = this.reader.Value;
						}
						if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[17]))
						{
							state = this.reader.Value;
						}
						if (this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[18]))
						{
							segmentationFlags = this.ParseRequiredFeatures(this.reader.Value);
						}
						if (!this.reader.MoveToAttribute(FormsRegistryParser.nameTableValues[15]))
						{
							this.ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues.Form);
						}
						text = this.reader.Value;
						FormKey formKey = new FormKey(experience, applicationElement, itemClass, action, state);
						ExTraceGlobals.FormsRegistryDataTracer.TraceDebug<FormKey, string>((long)this.GetHashCode(), "Parsed Mapping - key = ({0}), form = {1}", formKey, text);
						try
						{
							if (applicationElement == ApplicationElement.PreFormAction)
							{
								this.registry.AddPreForm(formKey, text, segmentationFlags);
							}
							else
							{
								this.registry.AddForm(formKey, text, segmentationFlags);
							}
							continue;
						}
						catch (OwaInvalidInputException ex)
						{
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_FormsRegistryParseError, ex.Message, new object[]
							{
								this.registryFile,
								this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
								this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
								ex.Message
							});
							throw ex;
						}
					}
					this.ThrowExpectedElementException(FormsRegistryParser.NameTableValues.Mapping);
				}
			}
		}

		private ulong ParseRequiredFeatures(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 0UL;
			}
			string[] array = value.Split(FormsRegistryParser.featureSeparators, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				return 0UL;
			}
			ulong num = 0UL;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				object obj = FormsRegistryParser.featureEnumParser.Parse(array[i]);
				if (obj == null)
				{
					this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Invalid RequiredFeatures: {0}", new object[]
					{
						this.reader.Value
					}), ClientsEventLogConstants.Tuple_FormsRegistryInvalidRequiredFeatures, new object[]
					{
						this.registryFile,
						this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
						this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
						this.reader.Value
					});
				}
				num |= (ulong)obj;
			}
			return num;
		}

		private void ThrowParserException(string description, ExEventLog.EventTuple tuple, params object[] eventMessageArgs)
		{
			OwaDiagnostics.LogEvent(tuple, string.Empty, eventMessageArgs);
			throw new OwaInvalidInputException(string.Format(CultureInfo.InvariantCulture, "Invalid forms registry file {0}. Line Number: {1}. Position: {2}.{3}", new object[]
			{
				this.registryFile,
				this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}), null, this);
		}

		private void ThrowExpectedElementException(FormsRegistryParser.NameTableValues elementName)
		{
			this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Expected element: {0}", new object[]
			{
				FormsRegistryParser.nameTableValues[(int)elementName]
			}), ClientsEventLogConstants.Tuple_FormsRegistryExpectedElement, new object[]
			{
				this.registryFile,
				this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				FormsRegistryParser.nameTableValues[(int)elementName]
			});
		}

		private void ThrowExpectedAttributeException(FormsRegistryParser.NameTableValues attributeName)
		{
			this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Expected attribute: {0}", new object[]
			{
				FormsRegistryParser.nameTableValues[(int)attributeName]
			}), ClientsEventLogConstants.Tuple_FormsRegistryExpectedAttribute, new object[]
			{
				this.registryFile,
				this.reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				this.reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				FormsRegistryParser.nameTableValues[(int)attributeName]
			});
		}

		private static readonly char[] featureSeparators = new char[]
		{
			',',
			' ',
			';'
		};

		private static FastEnumParser featureEnumParser = new FastEnumParser(typeof(Feature));

		private static NameTable nameTable;

		private static string[] nameTableValues;

		private string registryFile = string.Empty;

		private XmlTextReader reader;

		private FormsRegistry registry;

		private ArrayList clientMappings = new ArrayList();

		private ArrayList baseClientMappings = new ArrayList();

		private enum NameTableValues
		{
			Registry,
			Name,
			BaseExperience,
			InheritsFrom,
			IsRichClient,
			Experience,
			Client,
			Application,
			MinimumVersion,
			Platform,
			Control,
			ApplicationElement,
			ElementClass,
			Mapping,
			Value,
			Form,
			Action,
			State,
			RequiredFeatures,
			Max
		}
	}
}
