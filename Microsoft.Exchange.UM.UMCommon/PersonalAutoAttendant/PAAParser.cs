using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PAAParser : IPAAParser
	{
		internal static PAAParser Instance
		{
			get
			{
				return PAAParser.instance;
			}
		}

		public virtual void SerializeTo(PersonalAutoAttendant paa, XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public virtual PersonalAutoAttendant DeserializeFrom(XmlNode node)
		{
			throw new NotImplementedException();
		}

		internal IPAAParser Create(XmlNode node)
		{
			Version version;
			if (!this.TryParseAttributeAsVersion(node.Attributes, "Version", null, out version))
			{
				this.TraceError("PAA has a missing or corrupted Version attribute", new object[0]);
				throw new CorruptedPAAStoreException();
			}
			if (PAAUtils.IsCompatible(version))
			{
				return new PAAParser.PAA_V14_Parser();
			}
			return new PAAParser.IncompatiblePAAParser();
		}

		internal void Parse(IList<PersonalAutoAttendant> parsedlist, Stream stream)
		{
			if (parsedlist == null)
			{
				throw new ArgumentNullException("parsedlist");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.CloseInput = false;
			bool flag = false;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.Load(xmlReader);
					XmlElement documentElement = xmlDocument.DocumentElement;
					if (string.Compare(documentElement.Name, "PersonalAutoAttendants", StringComparison.Ordinal) == 0)
					{
						this.ParseAutoAttendantsNode(documentElement, parsedlist);
					}
					else
					{
						this.TraceError("PAAParser::Parse() got unknown element '{0}'. Aborting the parse", new object[]
						{
							documentElement.Name
						});
						flag = true;
						parsedlist.Clear();
					}
				}
			}
			catch (XmlException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAParser::Parse() Exception = {0}", new object[]
				{
					ex
				});
				parsedlist.Clear();
				throw new CorruptedPAAStoreException(ex);
			}
			catch (CorruptedPAAStoreException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAParser::Parse() Exception = {0}", new object[]
				{
					ex2
				});
				parsedlist.Clear();
				throw;
			}
			if (flag)
			{
				throw new CorruptedPAAStoreException();
			}
		}

		internal void Serialize(IList<PersonalAutoAttendant> autoAttendants, Stream stream)
		{
			if (autoAttendants == null)
			{
				throw new ArgumentNullException("autoAttendants");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
			{
				CloseOutput = false,
				Encoding = Encoding.Unicode
			}))
			{
				xmlWriter.WriteStartDocument(true);
				xmlWriter.WriteStartElement("PersonalAutoAttendants");
				foreach (PersonalAutoAttendant paa in autoAttendants)
				{
					IPAAParser ipaaparser = this.Create(paa);
					ipaaparser.SerializeTo(paa, xmlWriter);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
				xmlWriter.Flush();
			}
		}

		internal IPAAParser Create(PersonalAutoAttendant paa)
		{
			if (PAAUtils.IsCompatible(paa.Version))
			{
				return new PAAParser.PAA_V14_Parser();
			}
			return new PAAParser.IncompatiblePAAParser();
		}

		protected void WritePAAHeader(PersonalAutoAttendant paa, XmlWriter writer)
		{
			writer.WriteAttributeString("Version", paa.Version.ToString());
			writer.WriteAttributeString("Identity", paa.Identity.ToString());
			writer.WriteAttributeString("Name", paa.Name);
			writer.WriteAttributeString("Enabled", paa.Enabled.ToString());
		}

		protected bool TryParseAttributeAsString(XmlAttributeCollection attributes, string attributeName, string defaultValue, out string actualValue)
		{
			XmlAttribute xmlAttribute = attributes[attributeName];
			if (xmlAttribute == null)
			{
				actualValue = defaultValue;
				return false;
			}
			actualValue = xmlAttribute.Value;
			return true;
		}

		protected bool TryParseAttributeAsBase64String(XmlAttributeCollection attributes, string attributeName, string defaultValue, out string actualValue)
		{
			bool result = false;
			actualValue = defaultValue;
			string text = null;
			if (!this.TryParseAttributeAsString(attributes, attributeName, null, out text))
			{
				return false;
			}
			actualValue = text;
			try
			{
				Convert.FromBase64String(text);
				result = true;
			}
			catch (FormatException ex)
			{
				this.TraceError("TryParseAttributeAsBase64String: Invalid Base64 String {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex
				});
			}
			catch (ArgumentNullException ex2)
			{
				this.TraceError("TryParseAttributeAsBase64String: Invalid Base64 String {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex2
				});
			}
			return result;
		}

		protected bool TryParseAttributeAsGuid(XmlAttributeCollection attributes, string attributeName, Guid defaultValue, out Guid actualValue)
		{
			bool result = false;
			actualValue = defaultValue;
			string text = null;
			if (!this.TryParseAttributeAsString(attributes, attributeName, null, out text))
			{
				return false;
			}
			try
			{
				Guid guid = new Guid(text);
				actualValue = guid;
				result = true;
			}
			catch (FormatException ex)
			{
				this.TraceError("TryParseAttributeAsGuid: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex
				});
			}
			catch (OverflowException ex2)
			{
				this.TraceError("TryParseAttributeAsGuid: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex2
				});
			}
			catch (ArgumentNullException ex3)
			{
				this.TraceError("TryParseAttributeAsGuid: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex3
				});
			}
			return result;
		}

		protected bool TryParseAttributeAsVersion(XmlAttributeCollection attributes, string attributeName, Version defaultValue, out Version actualValue)
		{
			bool result = false;
			actualValue = defaultValue;
			string text = null;
			if (!this.TryParseAttributeAsString(attributes, attributeName, null, out text))
			{
				return false;
			}
			try
			{
				Version version = new Version(text);
				actualValue = version;
				result = true;
			}
			catch (FormatException ex)
			{
				this.TraceError("ParseAutoAttendant: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex
				});
			}
			catch (OverflowException ex2)
			{
				this.TraceError("ParseAutoAttendant: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex2
				});
			}
			catch (ArgumentException ex3)
			{
				this.TraceError("ParseAutoAttendant: Invalid GUID {0}", new object[]
				{
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex3
				});
			}
			return result;
		}

		protected bool TryParseAttributeAsBool(XmlAttributeCollection attributes, string attributeName, bool defaultValue, out bool actualValue)
		{
			bool result = false;
			actualValue = defaultValue;
			string text = null;
			if (!this.TryParseAttributeAsString(attributes, attributeName, null, out text))
			{
				return false;
			}
			try
			{
				bool flag = bool.Parse(text);
				actualValue = flag;
				result = true;
			}
			catch (FormatException ex)
			{
				this.TraceError("ParseAutoAttendant: Invalid value for {0}[{1}]", new object[]
				{
					attributeName,
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex
				});
			}
			catch (ArgumentNullException ex2)
			{
				this.TraceError("ParseAutoAttendant: Invalid value for {0}[{1}]", new object[]
				{
					attributeName,
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex2
				});
			}
			return result;
		}

		protected bool TryParseAttributeAsInt(XmlAttributeCollection attributes, string attributeName, int defaultValue, out int actualValue)
		{
			bool result = false;
			actualValue = defaultValue;
			string text = null;
			if (!this.TryParseAttributeAsString(attributes, attributeName, null, out text))
			{
				return false;
			}
			try
			{
				int num = int.Parse(text, CultureInfo.InvariantCulture);
				actualValue = num;
				result = true;
			}
			catch (FormatException ex)
			{
				this.TraceError("ParseAutoAttendant: Invalid value for {0}[{1}]", new object[]
				{
					attributeName,
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex
				});
			}
			catch (ArgumentNullException ex2)
			{
				this.TraceError("ParseAutoAttendant: Invalid value for {0}[{1}]", new object[]
				{
					attributeName,
					text
				});
				this.TraceError("Exception : {0}", new object[]
				{
					ex2
				});
			}
			return result;
		}

		protected void TraceError(string format, params object[] args)
		{
			CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, format, args);
		}

		protected void TraceDebug(string format, params object[] args)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, format, args);
		}

		private void ParseAutoAttendantsNode(XmlNode parent, IList<PersonalAutoAttendant> parsedlist)
		{
			foreach (object obj in parent.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment))
				{
					bool flag = false;
					if (string.Compare(xmlNode.Name, "PersonalAutoAttendant", StringComparison.OrdinalIgnoreCase) != 0)
					{
						this.TraceError("Failed to parse element '{0}' with PAA parser. PAA is corrupted", new object[]
						{
							xmlNode.OuterXml
						});
						throw new CorruptedPAAStoreException();
					}
					PersonalAutoAttendant item = null;
					try
					{
						IPAAParser ipaaparser = this.Create(xmlNode);
						item = ipaaparser.DeserializeFrom(xmlNode);
						flag = true;
					}
					catch (PersonalAutoAttendantParseException ex)
					{
						this.TraceError("Exception parsing PAA: {0}", new object[]
						{
							ex
						});
					}
					if (!flag)
					{
						this.TraceDebug("Failed to parse PAA with V14 parser. Now parsing with IncompatiblePAAParser", new object[0]);
						IPAAParser ipaaparser2 = new PAAParser.IncompatiblePAAParser();
						item = ipaaparser2.DeserializeFrom(xmlNode);
					}
					parsedlist.Add(item);
				}
			}
		}

		internal const string PersonalAutoAttendants = "PersonalAutoAttendants";

		internal const string PersonalAutoAttendant = "PersonalAutoAttendant";

		internal const string Version = "Version";

		internal const string Identity = "Identity";

		internal const string Name = "Name";

		internal const string Enabled = "Enabled";

		internal const string OWAPreview = "OWAPreview";

		internal const string Extensions = "Extensions";

		internal const string Extension = "Extension";

		internal const string CallerIdList = "CallerIdList";

		internal const string CallerId = "CallerId";

		internal const string CallerIdType = "CallerIdType";

		internal const string Id = "Id";

		internal const string TimeOfDay = "TimeOfDay";

		internal const string TimeOfDayType = "TimeOfDayType";

		internal const string DaysOfWeek = "DaysOfWeek";

		internal const string StartTimeInMinutes = "StartTimeInMinutes";

		internal const string EndTimeInMinutes = "EndTimeInMinutes";

		internal const string FreeBusy = "FreeBusy";

		internal const string OutOfOffice = "OutOfOffice";

		internal const string EnableBargeIn = "EnableBargeIn";

		internal const string KeyMappings = "KeyMappings";

		internal const string AutoKeyMappings = "AutoKeyMappings";

		internal const string KeyMapping = "KeyMapping";

		internal const string KeyMappingType = "KeyMappingType";

		internal const string Key = "Key";

		internal const string Context = "Context";

		internal const string LegacyExchangeDN = "LegacyExchangeDN";

		internal const string Number = "Number";

		internal const string FindMe = "FindMe";

		internal const string Timeout = "Timeout";

		internal const string Label = "Label";

		private static PAAParser instance = new PAAParser();

		internal class IncompatiblePAAParser : PAAParser
		{
			public override void SerializeTo(PersonalAutoAttendant paa, XmlWriter writer)
			{
				writer.WriteStartElement("PersonalAutoAttendant");
				base.WritePAAHeader(paa, writer);
				for (int i = 0; i < paa.DocumentNodes.Count; i++)
				{
					writer.WriteRaw(paa.DocumentNodes[i].OuterXml);
				}
				writer.WriteEndElement();
			}

			public override PersonalAutoAttendant DeserializeFrom(XmlNode node)
			{
				PersonalAutoAttendant personalAutoAttendant = Microsoft.Exchange.UM.PersonalAutoAttendant.PersonalAutoAttendant.CreateUninitialized();
				Version version = null;
				if (!base.TryParseAttributeAsVersion(node.Attributes, "Version", null, out version))
				{
					base.TraceError("PAA does not have 'Version' attribute", new object[0]);
					base.TraceError(node.OuterXml, new object[0]);
					throw new CorruptedPAAStoreException();
				}
				Guid identity;
				if (!base.TryParseAttributeAsGuid(node.Attributes, "Identity", Guid.NewGuid(), out identity))
				{
					base.TraceError("PAA does not have 'Identity' attribute", new object[0]);
					base.TraceError(node.OuterXml, new object[0]);
					throw new CorruptedPAAStoreException();
				}
				string name;
				if (!base.TryParseAttributeAsString(node.Attributes, "Name", string.Empty, out name))
				{
					base.TraceError("PAA does not have 'Name' attribute", new object[0]);
					base.TraceError(node.OuterXml, new object[0]);
					throw new CorruptedPAAStoreException();
				}
				bool enabled = false;
				if (!base.TryParseAttributeAsBool(node.Attributes, "Enabled", false, out enabled))
				{
					base.TraceError("PAA does not have 'Enabled' attribute", new object[0]);
					base.TraceError(node.OuterXml, new object[0]);
					throw new CorruptedPAAStoreException();
				}
				personalAutoAttendant.Version = version;
				personalAutoAttendant.Identity = identity;
				personalAutoAttendant.Name = name;
				personalAutoAttendant.Enabled = enabled;
				personalAutoAttendant.IsCompatible = false;
				if (node.ChildNodes.Count > 0)
				{
					personalAutoAttendant.DocumentNodes = new List<XmlNode>();
					for (int i = 0; i < node.ChildNodes.Count; i++)
					{
						personalAutoAttendant.DocumentNodes.Add(node.ChildNodes[i]);
					}
				}
				return personalAutoAttendant;
			}
		}

		internal class PAA_V14_Parser : PAAParser
		{
			public override void SerializeTo(PersonalAutoAttendant paa, XmlWriter writer)
			{
				if (paa == null)
				{
					throw new ArgumentNullException("paa");
				}
				if (writer == null)
				{
					throw new ArgumentNullException("writer");
				}
				this.WritePAA(paa, writer);
			}

			public override PersonalAutoAttendant DeserializeFrom(XmlNode parent)
			{
				int num = -1;
				uint num2 = 0U;
				List<XmlNode> list = null;
				PersonalAutoAttendant personalAutoAttendant = Microsoft.Exchange.UM.PersonalAutoAttendant.PersonalAutoAttendant.CreateUninitialized();
				Version version = null;
				if (!base.TryParseAttributeAsVersion(parent.Attributes, "Version", null, out version))
				{
					throw new PersonalAutoAttendantParseException();
				}
				Guid identity;
				if (!base.TryParseAttributeAsGuid(parent.Attributes, "Identity", Guid.Empty, out identity))
				{
					throw new PersonalAutoAttendantParseException();
				}
				string empty = string.Empty;
				if (!base.TryParseAttributeAsString(parent.Attributes, "Name", empty, out empty))
				{
					throw new PersonalAutoAttendantParseException();
				}
				bool flag = false;
				if (!base.TryParseAttributeAsBool(parent.Attributes, "Enabled", flag, out flag))
				{
					throw new PersonalAutoAttendantParseException();
				}
				personalAutoAttendant.Version = version;
				personalAutoAttendant.Identity = identity;
				personalAutoAttendant.Name = empty;
				personalAutoAttendant.Enabled = flag;
				personalAutoAttendant.IsCompatible = true;
				foreach (object obj in parent.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment))
					{
						if (string.Compare(xmlNode.Name, "OWAPreview", StringComparison.Ordinal) == 0)
						{
							personalAutoAttendant.OwaPreview = xmlNode.InnerText;
						}
						else
						{
							if (string.Compare(xmlNode.Name, "Extensions", StringComparison.Ordinal) == 0)
							{
								using (IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										object obj2 = enumerator2.Current;
										XmlNode xmlNode2 = (XmlNode)obj2;
										if (!(xmlNode2 is XmlComment))
										{
											if (string.Compare(xmlNode2.Name, "Extension", StringComparison.Ordinal) != 0)
											{
												base.TraceError("Unexpected element: {0}. Expecting {1}", new object[]
												{
													xmlNode2.Name,
													"Extension"
												});
												throw new PersonalAutoAttendantParseException();
											}
											personalAutoAttendant.ExtensionList.Add(xmlNode2.InnerText);
										}
									}
									continue;
								}
							}
							if (string.Compare(xmlNode.Name, "CallerIdList", StringComparison.Ordinal) == 0)
							{
								string text = null;
								using (IEnumerator enumerator3 = xmlNode.ChildNodes.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										object obj3 = enumerator3.Current;
										XmlNode xmlNode3 = (XmlNode)obj3;
										if (!(xmlNode3 is XmlComment))
										{
											if (string.Compare(xmlNode3.Name, "CallerId", StringComparison.Ordinal) != 0)
											{
												base.TraceError("Unexpected element: {0}. Expecting {1}", new object[]
												{
													xmlNode3.Name,
													"CallerId"
												});
												throw new PersonalAutoAttendantParseException();
											}
											string text2 = null;
											if (!base.TryParseAttributeAsString(xmlNode3.Attributes, "CallerIdType", null, out text2))
											{
												base.TraceError("Did not find callerid type", new object[0]);
												throw new PersonalAutoAttendantParseException();
											}
											num = -1;
											if (!this.ValidateEnum(text2, 0, 5, out num))
											{
												base.TraceError("Unknown callerid type: {0}", new object[]
												{
													text2
												});
												throw new PersonalAutoAttendantParseException();
											}
											CallerIdTypeEnum callerIdTypeEnum = (CallerIdTypeEnum)num;
											switch (callerIdTypeEnum)
											{
											case CallerIdTypeEnum.Number:
												if (!base.TryParseAttributeAsString(xmlNode3.Attributes, "Id", null, out text))
												{
													base.TraceError("Did not find ID attribute for CallerId condition of type {0}", new object[]
													{
														text2
													});
													throw new PersonalAutoAttendantParseException();
												}
												if (string.IsNullOrEmpty(text))
												{
													base.TraceError("Found Null Or Empty value for ID attribute of PhoneNumberCallerId condition", new object[0]);
													throw new PersonalAutoAttendantParseException();
												}
												personalAutoAttendant.AddPhoneNumberCallerId(text);
												break;
											case CallerIdTypeEnum.ContactItem:
												if (!base.TryParseAttributeAsBase64String(xmlNode3.Attributes, "Id", null, out text))
												{
													base.TraceError("Did not find ID attribute for CallerId condition of type {0}", new object[]
													{
														text2
													});
													throw new PersonalAutoAttendantParseException();
												}
												if (string.IsNullOrEmpty(text))
												{
													base.TraceError("Found Null Or Empty value for ID attribute of ContactItemCallerId condition", new object[0]);
													throw new PersonalAutoAttendantParseException();
												}
												personalAutoAttendant.AddContactItemCallerId(ContactItemCallerId.Parse(text));
												break;
											case CallerIdTypeEnum.DefaultContactFolder:
												personalAutoAttendant.AddDefaultContactFolderCallerId();
												break;
											case CallerIdTypeEnum.ADContact:
												if (!base.TryParseAttributeAsString(xmlNode3.Attributes, "Id", null, out text))
												{
													base.TraceError("Did not find ID attribute for CallerId condition of type {0}", new object[]
													{
														text2
													});
													throw new PersonalAutoAttendantParseException();
												}
												if (string.IsNullOrEmpty(text))
												{
													base.TraceError("Found Null Or Empty value for ID attribute of ADContactCallerId condition", new object[0]);
													throw new PersonalAutoAttendantParseException();
												}
												personalAutoAttendant.AddADContactCallerId(text);
												break;
											case CallerIdTypeEnum.PersonaContact:
												if (!base.TryParseAttributeAsString(xmlNode3.Attributes, "Id", null, out text))
												{
													base.TraceError("Did not find ID attribute for CallerId condition of type {0}", new object[]
													{
														text2
													});
													throw new PersonalAutoAttendantParseException();
												}
												if (string.IsNullOrEmpty(text))
												{
													base.TraceError("Found Null Or Empty value for ID attribute of PersonaContactCallerId condition", new object[0]);
													throw new PersonalAutoAttendantParseException();
												}
												personalAutoAttendant.AddPersonaContactCallerId(text);
												break;
											default:
												base.TraceError("Invalid callerid type: {0}", new object[]
												{
													callerIdTypeEnum.ToString()
												});
												throw new PersonalAutoAttendantParseException();
											}
										}
									}
									continue;
								}
							}
							if (string.Compare(xmlNode.Name, "TimeOfDay", StringComparison.Ordinal) == 0)
							{
								string text3 = null;
								if (!base.TryParseAttributeAsString(xmlNode.Attributes, "TimeOfDayType", null, out text3))
								{
									base.TraceError("Did not find TimeOfDay type", new object[0]);
									throw new PersonalAutoAttendantParseException();
								}
								num = -1;
								if (string.IsNullOrEmpty(text3) || !this.ValidateEnum(text3, 0, 5, out num))
								{
									base.TraceError("Unknown timeofday type: {0}", new object[]
									{
										text3
									});
									throw new PersonalAutoAttendantParseException();
								}
								TimeOfDayEnum timeOfDayEnum = (TimeOfDayEnum)num;
								personalAutoAttendant.TimeOfDay = timeOfDayEnum;
								if (timeOfDayEnum == TimeOfDayEnum.Custom)
								{
									int startTimeInMinutes = -1;
									if (!base.TryParseAttributeAsInt(xmlNode.Attributes, "StartTimeInMinutes", -1, out startTimeInMinutes))
									{
										base.TraceError("Did not find StartTimeInMinutes type", new object[0]);
										throw new PersonalAutoAttendantParseException();
									}
									int endTimeInMinutes = -1;
									if (!base.TryParseAttributeAsInt(xmlNode.Attributes, "EndTimeInMinutes", -1, out endTimeInMinutes))
									{
										base.TraceError("Did not find EndTimeInMinutes type", new object[0]);
										throw new PersonalAutoAttendantParseException();
									}
									int dayOfWeek = -1;
									if (!base.TryParseAttributeAsInt(xmlNode.Attributes, "DaysOfWeek", -1, out dayOfWeek))
									{
										base.TraceError("Did not find DaysOfWeek type", new object[0]);
										throw new PersonalAutoAttendantParseException();
									}
									personalAutoAttendant.WorkingPeriod = new WorkingPeriod((DaysOfWeek)dayOfWeek, startTimeInMinutes, endTimeInMinutes);
								}
							}
							else if (string.Compare(xmlNode.Name, "FreeBusy", StringComparison.Ordinal) == 0)
							{
								string innerText = xmlNode.InnerText;
								uint mask = 15U;
								if (string.IsNullOrEmpty(innerText) || !this.ValidateFlagsEnum(innerText, mask, out num2))
								{
									base.TraceError("Unknown FreeBusyStatus value: {0}", new object[]
									{
										innerText
									});
									throw new PersonalAutoAttendantParseException();
								}
								FreeBusyStatusEnum freeBusy = (FreeBusyStatusEnum)num2;
								personalAutoAttendant.FreeBusy = freeBusy;
							}
							else if (string.Compare(xmlNode.Name, "OutOfOffice", StringComparison.Ordinal) == 0)
							{
								string innerText2 = xmlNode.InnerText;
								if (string.IsNullOrEmpty(innerText2) || !this.ValidateEnum(innerText2, 0, 2, out num))
								{
									base.TraceError("Unknown OutOfOffice value: {0}", new object[]
									{
										innerText2
									});
									throw new PersonalAutoAttendantParseException();
								}
								OutOfOfficeStatusEnum outOfOffice = (OutOfOfficeStatusEnum)num;
								personalAutoAttendant.OutOfOffice = outOfOffice;
							}
							else if (string.Compare(xmlNode.Name, "EnableBargeIn", StringComparison.Ordinal) == 0)
							{
								string innerText3 = xmlNode.InnerText;
								if (string.IsNullOrEmpty(innerText3))
								{
									base.TraceError("Unknown EnableBargeIn value: {0}", new object[]
									{
										innerText3
									});
									throw new PersonalAutoAttendantParseException();
								}
								bool enableBargeIn = false;
								if (!bool.TryParse(innerText3, out enableBargeIn))
								{
									base.TraceError("Unknown EnableBargeIn value: {0}", new object[]
									{
										innerText3
									});
									throw new PersonalAutoAttendantParseException();
								}
								personalAutoAttendant.EnableBargeIn = enableBargeIn;
							}
							else if (string.Compare(xmlNode.Name, "KeyMappings", StringComparison.Ordinal) == 0)
							{
								this.ParseKeyMappings(xmlNode, personalAutoAttendant.KeyMappingList);
							}
							else if (string.Compare(xmlNode.Name, "AutoKeyMappings", StringComparison.Ordinal) == 0)
							{
								this.ParseKeyMappings(xmlNode, personalAutoAttendant.AutoActionsList);
							}
							else
							{
								if (list == null)
								{
									list = new List<XmlNode>();
								}
								list.Add(xmlNode);
							}
						}
					}
				}
				if (list != null)
				{
					personalAutoAttendant.Unprocessed = list.ToArray();
				}
				return personalAutoAttendant;
			}

			private static void WriteCallerIdList(IList<CallerIdBase> callerIdList, XmlWriter writer)
			{
				writer.WriteStartElement("CallerIdList");
				foreach (CallerIdBase callerIdBase in callerIdList)
				{
					writer.WriteStartElement("CallerId");
					writer.WriteAttributeString("CallerIdType", ((int)callerIdBase.CallerIdType).ToString(CultureInfo.InvariantCulture));
					switch (callerIdBase.CallerIdType)
					{
					case CallerIdTypeEnum.Number:
						writer.WriteAttributeString("Id", ((PhoneNumberCallerId)callerIdBase).PhoneNumberString);
						break;
					case CallerIdTypeEnum.ContactItem:
						writer.WriteAttributeString("Id", ((ContactItemCallerId)callerIdBase).ToString());
						break;
					case CallerIdTypeEnum.ADContact:
						writer.WriteAttributeString("Id", ((ADContactCallerId)callerIdBase).LegacyExchangeDN);
						break;
					case CallerIdTypeEnum.PersonaContact:
						writer.WriteAttributeString("Id", ((PersonaContactCallerId)callerIdBase).ToString());
						break;
					}
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}

			private static void WriteKeyMappings(string tag, KeyMappingBase[] menu, XmlWriter writer)
			{
				writer.WriteStartElement(tag);
				foreach (KeyMappingBase keyMappingBase in menu)
				{
					writer.WriteStartElement("KeyMapping");
					writer.WriteAttributeString("KeyMappingType", ((int)keyMappingBase.KeyMappingType).ToString(CultureInfo.InvariantCulture));
					writer.WriteAttributeString("Key", keyMappingBase.Key.ToString(CultureInfo.InvariantCulture));
					writer.WriteAttributeString("Context", keyMappingBase.Context);
					switch (keyMappingBase.KeyMappingType)
					{
					case KeyMappingTypeEnum.TransferToNumber:
					{
						TransferToNumber transferToNumber = keyMappingBase as TransferToNumber;
						writer.WriteElementString("Number", transferToNumber.PhoneNumberString);
						break;
					}
					case KeyMappingTypeEnum.TransferToADContactMailbox:
					{
						TransferToADContactMailbox transferToADContactMailbox = keyMappingBase as TransferToADContactMailbox;
						writer.WriteElementString("LegacyExchangeDN", transferToADContactMailbox.LegacyExchangeDN);
						break;
					}
					case KeyMappingTypeEnum.TransferToADContactPhone:
					{
						TransferToADContactPhone transferToADContactPhone = keyMappingBase as TransferToADContactPhone;
						writer.WriteElementString("LegacyExchangeDN", transferToADContactPhone.LegacyExchangeDN);
						break;
					}
					case KeyMappingTypeEnum.FindMe:
					{
						TransferToFindMe transferToFindMe = keyMappingBase as TransferToFindMe;
						for (int j = 0; j < transferToFindMe.Numbers.Count; j++)
						{
							FindMe findMe = transferToFindMe.Numbers[j];
							writer.WriteStartElement("FindMe");
							writer.WriteAttributeString("Number", findMe.Number);
							writer.WriteAttributeString("Timeout", findMe.Timeout.ToString(CultureInfo.InvariantCulture));
							writer.WriteAttributeString("Label", findMe.Label);
							writer.WriteEndElement();
						}
						break;
					}
					}
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}

			private void ParseKeyMappings(XmlNode keyMappingsNode, KeyMappings km)
			{
				string input = null;
				string text = null;
				int num = -1;
				foreach (object obj in keyMappingsNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment))
					{
						if (string.Compare(xmlNode.Name, "KeyMapping", StringComparison.Ordinal) != 0)
						{
							base.TraceError("Invalid element: {0} Expected {1}", new object[]
							{
								xmlNode.Name,
								"KeyMapping"
							});
							throw new PersonalAutoAttendantParseException();
						}
						string text2 = null;
						base.TryParseAttributeAsString(xmlNode.Attributes, "KeyMappingType", null, out text2);
						string text3 = null;
						base.TryParseAttributeAsString(xmlNode.Attributes, "Key", null, out text3);
						string text4 = null;
						base.TryParseAttributeAsString(xmlNode.Attributes, "Context", null, out text4);
						if (string.IsNullOrEmpty(text2) || !this.ValidateEnum(text2, 0, 5, out num))
						{
							base.TraceError("Unknown keymapping type: '{0}' key='{1}' context='{2}'", new object[]
							{
								text2,
								text3,
								text4
							});
							throw new PersonalAutoAttendantParseException();
						}
						int key = -1;
						if (string.IsNullOrEmpty(text3) || !int.TryParse(text3, out key))
						{
							base.TraceError("Invalid key: {0}", new object[]
							{
								text3
							});
							throw new PersonalAutoAttendantParseException();
						}
						KeyMappingTypeEnum keyMappingTypeEnum = (KeyMappingTypeEnum)num;
						switch (keyMappingTypeEnum)
						{
						case KeyMappingTypeEnum.TransferToNumber:
						{
							this.ValidateKeyForTransfer(key);
							text = null;
							foreach (object obj2 in xmlNode.ChildNodes)
							{
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (!(xmlNode2 is XmlComment))
								{
									if (!string.IsNullOrEmpty(text))
									{
										base.TraceError("Found extra element {0} in the TransferToNumber action", new object[]
										{
											xmlNode2.Name
										});
										throw new PersonalAutoAttendantParseException();
									}
									if (string.Compare(xmlNode2.Name, "Number", StringComparison.Ordinal) != 0)
									{
										base.TraceError("Unknown element name: {0}", new object[]
										{
											xmlNode2.Name
										});
										throw new PersonalAutoAttendantParseException();
									}
									text = Utils.TrimSpaces(xmlNode2.InnerText);
								}
							}
							PhoneNumber phoneNumber = null;
							if (PhoneNumber.TryParse(text, out phoneNumber))
							{
								km.AddTransferToNumber(key, text4, text);
								continue;
							}
							base.TraceError("Found invalid phone number '{0}' in TransferToPhone", new object[]
							{
								text
							});
							throw new PersonalAutoAttendantParseException();
						}
						case KeyMappingTypeEnum.TransferToADContactMailbox:
						case KeyMappingTypeEnum.TransferToADContactPhone:
						{
							this.ValidateKeyForTransfer(key);
							string text5 = null;
							foreach (object obj3 in xmlNode.ChildNodes)
							{
								XmlNode xmlNode3 = (XmlNode)obj3;
								if (!(xmlNode3 is XmlComment))
								{
									if (!string.IsNullOrEmpty(text5))
									{
										base.TraceError("Found extra element {0} in the TransferToADContactMailbox/TransferToADContactPhone action", new object[]
										{
											xmlNode3.Name
										});
										throw new PersonalAutoAttendantParseException();
									}
									if (string.Compare(xmlNode3.Name, "LegacyExchangeDN", StringComparison.Ordinal) != 0)
									{
										base.TraceError("Unknown element '{0}' for TransferToADContact base type", new object[]
										{
											xmlNode3.Name
										});
										throw new PersonalAutoAttendantParseException();
									}
									text5 = Utils.TrimSpaces(xmlNode3.InnerText);
								}
							}
							if (text5 == null)
							{
								base.TraceError("LegacyExchangeDN parameter is NULL for TransferToADContact base type", new object[0]);
								throw new PersonalAutoAttendantParseException();
							}
							if (keyMappingTypeEnum == KeyMappingTypeEnum.TransferToADContactMailbox)
							{
								km.AddTransferToADContactMailbox(key, text4, text5);
								continue;
							}
							km.AddTransferToADContactPhone(key, text4, text5);
							continue;
						}
						case KeyMappingTypeEnum.TransferToVoicemail:
							this.ValidateKeyForTransferToVoicemail(key);
							km.AddTransferToVoicemail(text4);
							continue;
						case KeyMappingTypeEnum.FindMe:
							this.ValidateKeyForTransfer(key);
							using (IEnumerator enumerator4 = xmlNode.ChildNodes.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									object obj4 = enumerator4.Current;
									XmlNode xmlNode4 = (XmlNode)obj4;
									if (!(xmlNode4 is XmlComment))
									{
										text = null;
										string text6 = null;
										string empty = string.Empty;
										if (string.Compare(xmlNode4.Name, "FindMe", StringComparison.Ordinal) != 0)
										{
											base.TraceError("Unknown element name: {0}", new object[]
											{
												xmlNode4.Name
											});
											throw new PersonalAutoAttendantParseException();
										}
										input = null;
										if (!base.TryParseAttributeAsString(xmlNode4.Attributes, "Number", null, out input))
										{
											base.TraceError("Did not find attribute {0} for findme", new object[]
											{
												"Number"
											});
											throw new PersonalAutoAttendantParseException();
										}
										text = Utils.TrimSpaces(input);
										if (!base.TryParseAttributeAsString(xmlNode4.Attributes, "Timeout", null, out text6))
										{
											base.TraceError("Did not find attribute {0} for findme", new object[]
											{
												"Timeout"
											});
											throw new PersonalAutoAttendantParseException();
										}
										base.TryParseAttributeAsString(xmlNode4.Attributes, "Label", null, out empty);
										int num2 = -1;
										PhoneNumber phoneNumber2 = null;
										PhoneNumber.TryParse(text, out phoneNumber2);
										if (phoneNumber2 == null || text6 == null || !int.TryParse(text6, out num2) || num2 <= 0)
										{
											base.TraceError("Invalid phonenumber, or timeout value", new object[0]);
											base.TraceError("Phone: {0} Timeout: {1}", new object[]
											{
												text,
												text6
											});
											throw new PersonalAutoAttendantParseException();
										}
										km.AddFindMe(key, text4, text, num2, empty);
									}
								}
								continue;
							}
							break;
						}
						base.TraceError("Invalid KeyMapping type: {0}", new object[]
						{
							keyMappingTypeEnum.ToString()
						});
						throw new PersonalAutoAttendantParseException();
					}
				}
			}

			private void ValidateKeyForTransferToVoicemail(int key)
			{
				if (key < 1 || (key > 9 && key != 10))
				{
					base.TraceError("Invalid key. Key should be in the range 1..9 or #", new object[0]);
					throw new PersonalAutoAttendantParseException();
				}
			}

			private void ValidateKeyForTransfer(int key)
			{
				if (key < 1 || key > 9)
				{
					base.TraceError("Invalid key. Key should be in the range 1..9", new object[0]);
					throw new PersonalAutoAttendantParseException();
				}
			}

			private void WritePAA(PersonalAutoAttendant paa, XmlWriter writer)
			{
				if (PAAUtils.IsCompatible(paa.Version))
				{
					this.WriteCurrentVersionPAA(paa, writer);
					return;
				}
				writer.WriteStartElement("PersonalAutoAttendant");
				base.WritePAAHeader(paa, writer);
				for (int i = 0; i < paa.DocumentNodes.Count; i++)
				{
					writer.WriteRaw(paa.DocumentNodes[i].OuterXml);
				}
				writer.WriteEndElement();
			}

			private void WriteCurrentVersionPAA(PersonalAutoAttendant paa, XmlWriter writer)
			{
				writer.WriteStartElement("PersonalAutoAttendant");
				base.WritePAAHeader(paa, writer);
				writer.WriteElementString("OWAPreview", paa.OwaPreview);
				writer.WriteStartElement("Extensions");
				foreach (string value in paa.ExtensionList)
				{
					writer.WriteElementString("Extension", value);
				}
				writer.WriteEndElement();
				PAAParser.PAA_V14_Parser.WriteCallerIdList(paa.CallerIdList, writer);
				writer.WriteStartElement("TimeOfDay");
				writer.WriteAttributeString("TimeOfDayType", ((int)paa.TimeOfDay).ToString(CultureInfo.InvariantCulture));
				if (paa.TimeOfDay == TimeOfDayEnum.Custom)
				{
					writer.WriteStartAttribute("DaysOfWeek");
					writer.WriteValue((int)paa.WorkingPeriod.DayOfWeek);
					writer.WriteEndAttribute();
					writer.WriteStartAttribute("StartTimeInMinutes");
					writer.WriteValue(paa.WorkingPeriod.StartTimeInMinutes);
					writer.WriteEndAttribute();
					writer.WriteStartAttribute("EndTimeInMinutes");
					writer.WriteValue(paa.WorkingPeriod.EndTimeInMinutes);
					writer.WriteEndAttribute();
				}
				writer.WriteEndElement();
				int freeBusy = (int)paa.FreeBusy;
				writer.WriteStartElement("FreeBusy");
				writer.WriteValue(freeBusy);
				writer.WriteEndElement();
				writer.WriteElementString("OutOfOffice", ((int)paa.OutOfOffice).ToString(CultureInfo.InvariantCulture));
				writer.WriteElementString("EnableBargeIn", paa.EnableBargeIn.ToString(CultureInfo.InvariantCulture));
				PAAParser.PAA_V14_Parser.WriteKeyMappings("KeyMappings", paa.KeyMappingList.Menu, writer);
				PAAParser.PAA_V14_Parser.WriteKeyMappings("AutoKeyMappings", paa.AutoActionsList.Menu, writer);
				if (paa.Unprocessed != null)
				{
					for (int i = 0; i < paa.Unprocessed.Length; i++)
					{
						writer.WriteRaw(paa.Unprocessed[i].OuterXml);
					}
				}
				writer.WriteEndElement();
			}

			private bool ValidateEnum(string enumString, int min, int max, out int parsedVal)
			{
				parsedVal = -1;
				int num = -1;
				if (!int.TryParse(enumString, out num))
				{
					base.TraceError("ValidateEnum: [{0}] is not a valid integer", new object[]
					{
						enumString
					});
					return false;
				}
				if (num < min || num > max)
				{
					base.TraceError("enumVal: [{0}] is not in the range {1}-{2}", new object[]
					{
						enumString,
						min,
						max
					});
					return false;
				}
				parsedVal = num;
				return true;
			}

			private bool ValidateFlagsEnum(string enumString, uint mask, out uint parsedVal)
			{
				parsedVal = uint.MaxValue;
				int num = -1;
				if (!int.TryParse(enumString, out num))
				{
					base.TraceError("ValidateFlagsEnum: [{0}] is not a valid integer", new object[]
					{
						enumString
					});
					return false;
				}
				uint num2 = (uint)num;
				uint num3 = ~mask;
				if ((num2 & num3) != 0U)
				{
					base.TraceError("ValidateFlagsEnum: [{0:X}] is not in the range {1:X}", new object[]
					{
						num,
						mask
					});
					return false;
				}
				parsedVal = num2;
				return true;
			}
		}
	}
}
