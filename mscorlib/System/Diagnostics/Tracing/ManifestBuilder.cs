using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.Reflection;

namespace System.Diagnostics.Tracing
{
	internal class ManifestBuilder
	{
		public ManifestBuilder(string providerName, Guid providerGuid, string dllName, ResourceManager resources, EventManifestOptions flags)
		{
			this.providerName = providerName;
			this.flags = flags;
			this.resources = resources;
			this.sb = new StringBuilder();
			this.events = new StringBuilder();
			this.templates = new StringBuilder();
			this.opcodeTab = new Dictionary<int, string>();
			this.stringTab = new Dictionary<string, string>();
			this.errors = new List<string>();
			this.perEventByteArrayArgIndices = new Dictionary<string, List<int>>();
			this.sb.AppendLine("<instrumentationManifest xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
			this.sb.AppendLine(" <instrumentation xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:win=\"http://manifests.microsoft.com/win/2004/08/windows/events\">");
			this.sb.AppendLine("  <events xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
			this.sb.Append("<provider name=\"").Append(providerName).Append("\" guid=\"{").Append(providerGuid.ToString()).Append("}");
			if (dllName != null)
			{
				this.sb.Append("\" resourceFileName=\"").Append(dllName).Append("\" messageFileName=\"").Append(dllName);
			}
			string value = providerName.Replace("-", "").Replace(".", "_");
			this.sb.Append("\" symbol=\"").Append(value);
			this.sb.Append("\">").AppendLine();
		}

		public void AddOpcode(string name, int value)
		{
			if ((this.flags & EventManifestOptions.Strict) != EventManifestOptions.None)
			{
				if (value <= 10 || value >= 239)
				{
					this.ManifestError(Environment.GetResourceString("EventSource_IllegalOpcodeValue", new object[]
					{
						name,
						value
					}), false);
				}
				string text;
				if (this.opcodeTab.TryGetValue(value, out text) && !name.Equals(text, StringComparison.Ordinal))
				{
					this.ManifestError(Environment.GetResourceString("EventSource_OpcodeCollision", new object[]
					{
						name,
						text,
						value
					}), false);
				}
			}
			this.opcodeTab[value] = name;
		}

		public void AddTask(string name, int value)
		{
			if ((this.flags & EventManifestOptions.Strict) != EventManifestOptions.None)
			{
				if (value <= 0 || value >= 65535)
				{
					this.ManifestError(Environment.GetResourceString("EventSource_IllegalTaskValue", new object[]
					{
						name,
						value
					}), false);
				}
				string text;
				if (this.taskTab != null && this.taskTab.TryGetValue(value, out text) && !name.Equals(text, StringComparison.Ordinal))
				{
					this.ManifestError(Environment.GetResourceString("EventSource_TaskCollision", new object[]
					{
						name,
						text,
						value
					}), false);
				}
			}
			if (this.taskTab == null)
			{
				this.taskTab = new Dictionary<int, string>();
			}
			this.taskTab[value] = name;
		}

		public void AddKeyword(string name, ulong value)
		{
			if ((value & value - 1UL) != 0UL)
			{
				this.ManifestError(Environment.GetResourceString("EventSource_KeywordNeedPowerOfTwo", new object[]
				{
					"0x" + value.ToString("x", CultureInfo.CurrentCulture),
					name
				}), true);
			}
			if ((this.flags & EventManifestOptions.Strict) != EventManifestOptions.None)
			{
				if (value >= 17592186044416UL && !name.StartsWith("Session", StringComparison.Ordinal))
				{
					this.ManifestError(Environment.GetResourceString("EventSource_IllegalKeywordsValue", new object[]
					{
						name,
						"0x" + value.ToString("x", CultureInfo.CurrentCulture)
					}), false);
				}
				string text;
				if (this.keywordTab != null && this.keywordTab.TryGetValue(value, out text) && !name.Equals(text, StringComparison.Ordinal))
				{
					this.ManifestError(Environment.GetResourceString("EventSource_KeywordCollision", new object[]
					{
						name,
						text,
						"0x" + value.ToString("x", CultureInfo.CurrentCulture)
					}), false);
				}
			}
			if (this.keywordTab == null)
			{
				this.keywordTab = new Dictionary<ulong, string>();
			}
			this.keywordTab[value] = name;
		}

		public void AddChannel(string name, int value, EventChannelAttribute channelAttribute)
		{
			EventChannel eventChannel = (EventChannel)value;
			if (value < 16 || value > 255)
			{
				this.ManifestError(Environment.GetResourceString("EventSource_EventChannelOutOfRange", new object[]
				{
					name,
					value
				}), false);
			}
			else if (eventChannel >= EventChannel.Admin && eventChannel <= EventChannel.Debug && channelAttribute != null && this.EventChannelToChannelType(eventChannel) != channelAttribute.EventChannelType)
			{
				this.ManifestError(Environment.GetResourceString("EventSource_ChannelTypeDoesNotMatchEventChannelValue", new object[]
				{
					name,
					((EventChannel)value).ToString()
				}), false);
			}
			ulong channelKeyword = this.GetChannelKeyword(eventChannel);
			if (this.channelTab == null)
			{
				this.channelTab = new Dictionary<int, ManifestBuilder.ChannelInfo>(4);
			}
			this.channelTab[value] = new ManifestBuilder.ChannelInfo
			{
				Name = name,
				Keywords = channelKeyword,
				Attribs = channelAttribute
			};
		}

		private EventChannelType EventChannelToChannelType(EventChannel channel)
		{
			return (EventChannelType)(channel - 16 + 1);
		}

		private EventChannelAttribute GetDefaultChannelAttribute(EventChannel channel)
		{
			EventChannelAttribute eventChannelAttribute = new EventChannelAttribute();
			eventChannelAttribute.EventChannelType = this.EventChannelToChannelType(channel);
			if (eventChannelAttribute.EventChannelType <= EventChannelType.Operational)
			{
				eventChannelAttribute.Enabled = true;
			}
			return eventChannelAttribute;
		}

		public ulong[] GetChannelData()
		{
			if (this.channelTab == null)
			{
				return new ulong[0];
			}
			int num = -1;
			foreach (int num2 in this.channelTab.Keys)
			{
				if (num2 > num)
				{
					num = num2;
				}
			}
			ulong[] array = new ulong[num + 1];
			foreach (KeyValuePair<int, ManifestBuilder.ChannelInfo> keyValuePair in this.channelTab)
			{
				array[keyValuePair.Key] = keyValuePair.Value.Keywords;
			}
			return array;
		}

		public void StartEvent(string eventName, EventAttribute eventAttribute)
		{
			this.eventName = eventName;
			this.numParams = 0;
			this.byteArrArgIndices = null;
			this.events.Append("  <event").Append(" value=\"").Append(eventAttribute.EventId).Append("\"").Append(" version=\"").Append(eventAttribute.Version).Append("\"").Append(" level=\"").Append(ManifestBuilder.GetLevelName(eventAttribute.Level)).Append("\"").Append(" symbol=\"").Append(eventName).Append("\"");
			this.WriteMessageAttrib(this.events, "event", eventName, eventAttribute.Message);
			if (eventAttribute.Keywords != EventKeywords.None)
			{
				this.events.Append(" keywords=\"").Append(this.GetKeywords((ulong)eventAttribute.Keywords, eventName)).Append("\"");
			}
			if (eventAttribute.Opcode != EventOpcode.Info)
			{
				this.events.Append(" opcode=\"").Append(this.GetOpcodeName(eventAttribute.Opcode, eventName)).Append("\"");
			}
			if (eventAttribute.Task != EventTask.None)
			{
				this.events.Append(" task=\"").Append(this.GetTaskName(eventAttribute.Task, eventName)).Append("\"");
			}
			if (eventAttribute.Channel != EventChannel.None)
			{
				this.events.Append(" channel=\"").Append(this.GetChannelName(eventAttribute.Channel, eventName, eventAttribute.Message)).Append("\"");
			}
		}

		public void AddEventParameter(Type type, string name)
		{
			if (this.numParams == 0)
			{
				this.templates.Append("  <template tid=\"").Append(this.eventName).Append("Args\">").AppendLine();
			}
			if (type == typeof(byte[]))
			{
				if (this.byteArrArgIndices == null)
				{
					this.byteArrArgIndices = new List<int>(4);
				}
				this.byteArrArgIndices.Add(this.numParams);
				this.numParams++;
				this.templates.Append("   <data name=\"").Append(name).Append("Size\" inType=\"win:UInt32\"/>").AppendLine();
			}
			this.numParams++;
			this.templates.Append("   <data name=\"").Append(name).Append("\" inType=\"").Append(this.GetTypeName(type)).Append("\"");
			if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
			{
				this.templates.Append(" length=\"").Append(name).Append("Size\"");
			}
			if (type.IsEnum() && Enum.GetUnderlyingType(type) != typeof(ulong) && Enum.GetUnderlyingType(type) != typeof(long))
			{
				this.templates.Append(" map=\"").Append(type.Name).Append("\"");
				if (this.mapsTab == null)
				{
					this.mapsTab = new Dictionary<string, Type>();
				}
				if (!this.mapsTab.ContainsKey(type.Name))
				{
					this.mapsTab.Add(type.Name, type);
				}
			}
			this.templates.Append("/>").AppendLine();
		}

		public void EndEvent()
		{
			if (this.numParams > 0)
			{
				this.templates.Append("  </template>").AppendLine();
				this.events.Append(" template=\"").Append(this.eventName).Append("Args\"");
			}
			this.events.Append("/>").AppendLine();
			if (this.byteArrArgIndices != null)
			{
				this.perEventByteArrayArgIndices[this.eventName] = this.byteArrArgIndices;
			}
			string text;
			if (this.stringTab.TryGetValue("event_" + this.eventName, out text))
			{
				text = this.TranslateToManifestConvention(text, this.eventName);
				this.stringTab["event_" + this.eventName] = text;
			}
			this.eventName = null;
			this.numParams = 0;
			this.byteArrArgIndices = null;
		}

		public ulong GetChannelKeyword(EventChannel channel)
		{
			if (this.channelTab == null)
			{
				this.channelTab = new Dictionary<int, ManifestBuilder.ChannelInfo>(4);
			}
			if (this.channelTab.Count == 8)
			{
				this.ManifestError(Environment.GetResourceString("EventSource_MaxChannelExceeded"), false);
			}
			ManifestBuilder.ChannelInfo channelInfo;
			ulong keywords;
			if (!this.channelTab.TryGetValue((int)channel, out channelInfo))
			{
				keywords = this.nextChannelKeywordBit;
				this.nextChannelKeywordBit >>= 1;
			}
			else
			{
				keywords = channelInfo.Keywords;
			}
			return keywords;
		}

		public byte[] CreateManifest()
		{
			string s = this.CreateManifestString();
			return Encoding.UTF8.GetBytes(s);
		}

		public IList<string> Errors
		{
			get
			{
				return this.errors;
			}
		}

		public void ManifestError(string msg, bool runtimeCritical = false)
		{
			if ((this.flags & EventManifestOptions.Strict) != EventManifestOptions.None)
			{
				this.errors.Add(msg);
				return;
			}
			if (runtimeCritical)
			{
				throw new ArgumentException(msg);
			}
		}

		private string CreateManifestString()
		{
			if (this.channelTab != null)
			{
				this.sb.Append(" <channels>").AppendLine();
				List<KeyValuePair<int, ManifestBuilder.ChannelInfo>> list = new List<KeyValuePair<int, ManifestBuilder.ChannelInfo>>();
				foreach (KeyValuePair<int, ManifestBuilder.ChannelInfo> item in this.channelTab)
				{
					list.Add(item);
				}
				list.Sort((KeyValuePair<int, ManifestBuilder.ChannelInfo> p1, KeyValuePair<int, ManifestBuilder.ChannelInfo> p2) => -Comparer<ulong>.Default.Compare(p1.Value.Keywords, p2.Value.Keywords));
				foreach (KeyValuePair<int, ManifestBuilder.ChannelInfo> keyValuePair in list)
				{
					int key = keyValuePair.Key;
					ManifestBuilder.ChannelInfo value = keyValuePair.Value;
					string text = null;
					string text2 = "channel";
					bool flag = false;
					string text3 = null;
					if (value.Attribs != null)
					{
						EventChannelAttribute attribs = value.Attribs;
						if (Enum.IsDefined(typeof(EventChannelType), attribs.EventChannelType))
						{
							text = attribs.EventChannelType.ToString();
						}
						flag = attribs.Enabled;
					}
					if (text3 == null)
					{
						text3 = this.providerName + "/" + value.Name;
					}
					this.sb.Append("  <").Append(text2);
					this.sb.Append(" chid=\"").Append(value.Name).Append("\"");
					this.sb.Append(" name=\"").Append(text3).Append("\"");
					if (text2 == "channel")
					{
						this.WriteMessageAttrib(this.sb, "channel", value.Name, null);
						this.sb.Append(" value=\"").Append(key).Append("\"");
						if (text != null)
						{
							this.sb.Append(" type=\"").Append(text).Append("\"");
						}
						this.sb.Append(" enabled=\"").Append(flag.ToString().ToLower()).Append("\"");
					}
					this.sb.Append("/>").AppendLine();
				}
				this.sb.Append(" </channels>").AppendLine();
			}
			if (this.taskTab != null)
			{
				this.sb.Append(" <tasks>").AppendLine();
				List<int> list2 = new List<int>(this.taskTab.Keys);
				list2.Sort();
				foreach (int num in list2)
				{
					this.sb.Append("  <task");
					this.WriteNameAndMessageAttribs(this.sb, "task", this.taskTab[num]);
					this.sb.Append(" value=\"").Append(num).Append("\"/>").AppendLine();
				}
				this.sb.Append(" </tasks>").AppendLine();
			}
			if (this.mapsTab != null)
			{
				this.sb.Append(" <maps>").AppendLine();
				foreach (Type type in this.mapsTab.Values)
				{
					bool flag2 = EventSource.GetCustomAttributeHelper(type, typeof(FlagsAttribute), this.flags) != null;
					string value2 = flag2 ? "bitMap" : "valueMap";
					this.sb.Append("  <").Append(value2).Append(" name=\"").Append(type.Name).Append("\">").AppendLine();
					FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
					foreach (FieldInfo fieldInfo in fields)
					{
						object rawConstantValue = fieldInfo.GetRawConstantValue();
						if (rawConstantValue != null)
						{
							long num2;
							if (rawConstantValue is int)
							{
								num2 = (long)((int)rawConstantValue);
							}
							else
							{
								if (!(rawConstantValue is long))
								{
									goto IL_4D1;
								}
								num2 = (long)rawConstantValue;
							}
							if (!flag2 || ((num2 & num2 - 1L) == 0L && num2 != 0L))
							{
								this.sb.Append("   <map value=\"0x").Append(num2.ToString("x", CultureInfo.InvariantCulture)).Append("\"");
								this.WriteMessageAttrib(this.sb, "map", type.Name + "." + fieldInfo.Name, fieldInfo.Name);
								this.sb.Append("/>").AppendLine();
							}
						}
						IL_4D1:;
					}
					this.sb.Append("  </").Append(value2).Append(">").AppendLine();
				}
				this.sb.Append(" </maps>").AppendLine();
			}
			this.sb.Append(" <opcodes>").AppendLine();
			List<int> list3 = new List<int>(this.opcodeTab.Keys);
			list3.Sort();
			foreach (int num3 in list3)
			{
				this.sb.Append("  <opcode");
				this.WriteNameAndMessageAttribs(this.sb, "opcode", this.opcodeTab[num3]);
				this.sb.Append(" value=\"").Append(num3).Append("\"/>").AppendLine();
			}
			this.sb.Append(" </opcodes>").AppendLine();
			if (this.keywordTab != null)
			{
				this.sb.Append(" <keywords>").AppendLine();
				List<ulong> list4 = new List<ulong>(this.keywordTab.Keys);
				list4.Sort();
				foreach (ulong key2 in list4)
				{
					this.sb.Append("  <keyword");
					this.WriteNameAndMessageAttribs(this.sb, "keyword", this.keywordTab[key2]);
					this.sb.Append(" mask=\"0x").Append(key2.ToString("x", CultureInfo.InvariantCulture)).Append("\"/>").AppendLine();
				}
				this.sb.Append(" </keywords>").AppendLine();
			}
			this.sb.Append(" <events>").AppendLine();
			this.sb.Append(this.events);
			this.sb.Append(" </events>").AppendLine();
			this.sb.Append(" <templates>").AppendLine();
			if (this.templates.Length > 0)
			{
				this.sb.Append(this.templates);
			}
			else
			{
				this.sb.Append("    <template tid=\"_empty\"></template>").AppendLine();
			}
			this.sb.Append(" </templates>").AppendLine();
			this.sb.Append("</provider>").AppendLine();
			this.sb.Append("</events>").AppendLine();
			this.sb.Append("</instrumentation>").AppendLine();
			this.sb.Append("<localization>").AppendLine();
			List<CultureInfo> list5;
			if (this.resources != null && (this.flags & EventManifestOptions.AllCultures) != EventManifestOptions.None)
			{
				list5 = ManifestBuilder.GetSupportedCultures(this.resources);
			}
			else
			{
				list5 = new List<CultureInfo>();
				list5.Add(CultureInfo.CurrentUICulture);
			}
			string[] array2 = new string[this.stringTab.Keys.Count];
			this.stringTab.Keys.CopyTo(array2, 0);
			ArraySortHelper<string>.IntrospectiveSort(array2, 0, array2.Length, Comparer<string>.Default);
			foreach (CultureInfo cultureInfo in list5)
			{
				this.sb.Append(" <resources culture=\"").Append(cultureInfo.Name).Append("\">").AppendLine();
				this.sb.Append("  <stringTable>").AppendLine();
				foreach (string text4 in array2)
				{
					string localizedMessage = this.GetLocalizedMessage(text4, cultureInfo, true);
					this.sb.Append("   <string id=\"").Append(text4).Append("\" value=\"").Append(localizedMessage).Append("\"/>").AppendLine();
				}
				this.sb.Append("  </stringTable>").AppendLine();
				this.sb.Append(" </resources>").AppendLine();
			}
			this.sb.Append("</localization>").AppendLine();
			this.sb.AppendLine("</instrumentationManifest>");
			return this.sb.ToString();
		}

		private void WriteNameAndMessageAttribs(StringBuilder stringBuilder, string elementName, string name)
		{
			stringBuilder.Append(" name=\"").Append(name).Append("\"");
			this.WriteMessageAttrib(this.sb, elementName, name, name);
		}

		private void WriteMessageAttrib(StringBuilder stringBuilder, string elementName, string name, string value)
		{
			string text = elementName + "_" + name;
			if (this.resources != null)
			{
				string @string = this.resources.GetString(text, CultureInfo.InvariantCulture);
				if (@string != null)
				{
					value = @string;
				}
			}
			if (value == null)
			{
				return;
			}
			stringBuilder.Append(" message=\"$(string.").Append(text).Append(")\"");
			string text2;
			if (this.stringTab.TryGetValue(text, out text2) && !text2.Equals(value))
			{
				this.ManifestError(Environment.GetResourceString("EventSource_DuplicateStringKey", new object[]
				{
					text
				}), true);
				return;
			}
			this.stringTab[text] = value;
		}

		internal string GetLocalizedMessage(string key, CultureInfo ci, bool etwFormat)
		{
			string text = null;
			if (this.resources != null)
			{
				string @string = this.resources.GetString(key, ci);
				if (@string != null)
				{
					text = @string;
					if (etwFormat && key.StartsWith("event_"))
					{
						string evtName = key.Substring("event_".Length);
						text = this.TranslateToManifestConvention(text, evtName);
					}
				}
			}
			if (etwFormat && text == null)
			{
				this.stringTab.TryGetValue(key, out text);
			}
			return text;
		}

		private static List<CultureInfo> GetSupportedCultures(ResourceManager resources)
		{
			List<CultureInfo> list = new List<CultureInfo>();
			foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			{
				if (resources.GetResourceSet(cultureInfo, true, false) != null)
				{
					list.Add(cultureInfo);
				}
			}
			if (!list.Contains(CultureInfo.CurrentUICulture))
			{
				list.Insert(0, CultureInfo.CurrentUICulture);
			}
			return list;
		}

		private static string GetLevelName(EventLevel level)
		{
			return ((level >= (EventLevel)16) ? "" : "win:") + level.ToString();
		}

		private string GetChannelName(EventChannel channel, string eventName, string eventMessage)
		{
			ManifestBuilder.ChannelInfo channelInfo = null;
			if (this.channelTab == null || !this.channelTab.TryGetValue((int)channel, out channelInfo))
			{
				if (channel < EventChannel.Admin)
				{
					this.ManifestError(Environment.GetResourceString("EventSource_UndefinedChannel", new object[]
					{
						channel,
						eventName
					}), false);
				}
				if (this.channelTab == null)
				{
					this.channelTab = new Dictionary<int, ManifestBuilder.ChannelInfo>(4);
				}
				string text = channel.ToString();
				if (EventChannel.Debug < channel)
				{
					text = "Channel" + text;
				}
				this.AddChannel(text, (int)channel, this.GetDefaultChannelAttribute(channel));
				if (!this.channelTab.TryGetValue((int)channel, out channelInfo))
				{
					this.ManifestError(Environment.GetResourceString("EventSource_UndefinedChannel", new object[]
					{
						channel,
						eventName
					}), false);
				}
			}
			if (this.resources != null && eventMessage == null)
			{
				eventMessage = this.resources.GetString("event_" + eventName, CultureInfo.InvariantCulture);
			}
			if (channelInfo.Attribs.EventChannelType == EventChannelType.Admin && eventMessage == null)
			{
				this.ManifestError(Environment.GetResourceString("EventSource_EventWithAdminChannelMustHaveMessage", new object[]
				{
					eventName,
					channelInfo.Name
				}), false);
			}
			return channelInfo.Name;
		}

		private string GetTaskName(EventTask task, string eventName)
		{
			if (task == EventTask.None)
			{
				return "";
			}
			if (this.taskTab == null)
			{
				this.taskTab = new Dictionary<int, string>();
			}
			string result;
			if (!this.taskTab.TryGetValue((int)task, out result))
			{
				this.taskTab[(int)task] = eventName;
				result = eventName;
			}
			return result;
		}

		private string GetOpcodeName(EventOpcode opcode, string eventName)
		{
			switch (opcode)
			{
			case EventOpcode.Info:
				return "win:Info";
			case EventOpcode.Start:
				return "win:Start";
			case EventOpcode.Stop:
				return "win:Stop";
			case EventOpcode.DataCollectionStart:
				return "win:DC_Start";
			case EventOpcode.DataCollectionStop:
				return "win:DC_Stop";
			case EventOpcode.Extension:
				return "win:Extension";
			case EventOpcode.Reply:
				return "win:Reply";
			case EventOpcode.Resume:
				return "win:Resume";
			case EventOpcode.Suspend:
				return "win:Suspend";
			case EventOpcode.Send:
				return "win:Send";
			default:
				if (opcode != EventOpcode.Receive)
				{
					string result;
					if (this.opcodeTab == null || !this.opcodeTab.TryGetValue((int)opcode, out result))
					{
						this.ManifestError(Environment.GetResourceString("EventSource_UndefinedOpcode", new object[]
						{
							opcode,
							eventName
						}), true);
						result = null;
					}
					return result;
				}
				return "win:Receive";
			}
		}

		private string GetKeywords(ulong keywords, string eventName)
		{
			string text = "";
			for (ulong num = 1UL; num != 0UL; num <<= 1)
			{
				if ((keywords & num) != 0UL)
				{
					string text2 = null;
					if ((this.keywordTab == null || !this.keywordTab.TryGetValue(num, out text2)) && num >= 281474976710656UL)
					{
						text2 = string.Empty;
					}
					if (text2 == null)
					{
						this.ManifestError(Environment.GetResourceString("EventSource_UndefinedKeyword", new object[]
						{
							"0x" + num.ToString("x", CultureInfo.CurrentCulture),
							eventName
						}), true);
						text2 = string.Empty;
					}
					if (text.Length != 0 && text2.Length != 0)
					{
						text += " ";
					}
					text += text2;
				}
			}
			return text;
		}

		private string GetTypeName(Type type)
		{
			if (type.IsEnum())
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				string typeName = this.GetTypeName(fields[0].FieldType);
				return typeName.Replace("win:Int", "win:UInt");
			}
			switch (type.GetTypeCode())
			{
			case TypeCode.Boolean:
				return "win:Boolean";
			case TypeCode.Char:
			case TypeCode.UInt16:
				return "win:UInt16";
			case TypeCode.SByte:
				return "win:Int8";
			case TypeCode.Byte:
				return "win:UInt8";
			case TypeCode.Int16:
				return "win:Int16";
			case TypeCode.Int32:
				return "win:Int32";
			case TypeCode.UInt32:
				return "win:UInt32";
			case TypeCode.Int64:
				return "win:Int64";
			case TypeCode.UInt64:
				return "win:UInt64";
			case TypeCode.Single:
				return "win:Float";
			case TypeCode.Double:
				return "win:Double";
			case TypeCode.DateTime:
				return "win:FILETIME";
			case TypeCode.String:
				return "win:UnicodeString";
			}
			if (type == typeof(Guid))
			{
				return "win:GUID";
			}
			if (type == typeof(IntPtr))
			{
				return "win:Pointer";
			}
			if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
			{
				return "win:Binary";
			}
			this.ManifestError(Environment.GetResourceString("EventSource_UnsupportedEventTypeInManifest", new object[]
			{
				type.Name
			}), true);
			return string.Empty;
		}

		private static void UpdateStringBuilder(ref StringBuilder stringBuilder, string eventMessage, int startIndex, int count)
		{
			if (stringBuilder == null)
			{
				stringBuilder = new StringBuilder();
			}
			stringBuilder.Append(eventMessage, startIndex, count);
		}

		private string TranslateToManifestConvention(string eventMessage, string evtName)
		{
			ManifestBuilder.<>c__DisplayClass28_0 CS$<>8__locals1 = new ManifestBuilder.<>c__DisplayClass28_0();
			CS$<>8__locals1.eventMessage = eventMessage;
			CS$<>8__locals1.stringBuilder = null;
			CS$<>8__locals1.writtenSoFar = 0;
			int i = 0;
			while (i < CS$<>8__locals1.eventMessage.Length)
			{
				int num2;
				if (CS$<>8__locals1.eventMessage[i] == '%')
				{
					ManifestBuilder.UpdateStringBuilder(ref CS$<>8__locals1.stringBuilder, CS$<>8__locals1.eventMessage, CS$<>8__locals1.writtenSoFar, i - CS$<>8__locals1.writtenSoFar);
					CS$<>8__locals1.stringBuilder.Append("%%");
					int j = i;
					i = j + 1;
					CS$<>8__locals1.writtenSoFar = i;
				}
				else if (i < CS$<>8__locals1.eventMessage.Length - 1 && ((CS$<>8__locals1.eventMessage[i] == '{' && CS$<>8__locals1.eventMessage[i + 1] == '{') || (CS$<>8__locals1.eventMessage[i] == '}' && CS$<>8__locals1.eventMessage[i + 1] == '}')))
				{
					ManifestBuilder.UpdateStringBuilder(ref CS$<>8__locals1.stringBuilder, CS$<>8__locals1.eventMessage, CS$<>8__locals1.writtenSoFar, i - CS$<>8__locals1.writtenSoFar);
					CS$<>8__locals1.stringBuilder.Append(CS$<>8__locals1.eventMessage[i]);
					int j = i;
					i = j + 1;
					j = i;
					i = j + 1;
					CS$<>8__locals1.writtenSoFar = i;
				}
				else if (CS$<>8__locals1.eventMessage[i] == '{')
				{
					int i2 = i;
					int j = i;
					i = j + 1;
					int num = 0;
					while (i < CS$<>8__locals1.eventMessage.Length && char.IsDigit(CS$<>8__locals1.eventMessage[i]))
					{
						num = num * 10 + (int)CS$<>8__locals1.eventMessage[i] - 48;
						j = i;
						i = j + 1;
					}
					if (i < CS$<>8__locals1.eventMessage.Length && CS$<>8__locals1.eventMessage[i] == '}')
					{
						j = i;
						i = j + 1;
						ManifestBuilder.UpdateStringBuilder(ref CS$<>8__locals1.stringBuilder, CS$<>8__locals1.eventMessage, CS$<>8__locals1.writtenSoFar, i2 - CS$<>8__locals1.writtenSoFar);
						int value = this.TranslateIndexToManifestConvention(num, evtName);
						CS$<>8__locals1.stringBuilder.Append('%').Append(value);
						if (i < CS$<>8__locals1.eventMessage.Length && CS$<>8__locals1.eventMessage[i] == '!')
						{
							j = i;
							i = j + 1;
							CS$<>8__locals1.stringBuilder.Append("%!");
						}
						CS$<>8__locals1.writtenSoFar = i;
					}
					else
					{
						this.ManifestError(Environment.GetResourceString("EventSource_UnsupportedMessageProperty", new object[]
						{
							evtName,
							CS$<>8__locals1.eventMessage
						}), false);
					}
				}
				else if ((num2 = "&<>'\"\r\n\t".IndexOf(CS$<>8__locals1.eventMessage[i])) >= 0)
				{
					string[] array = new string[]
					{
						"&amp;",
						"&lt;",
						"&gt;",
						"&apos;",
						"&quot;",
						"%r",
						"%n",
						"%t"
					};
					Action<char, string> action = delegate(char ch, string escape)
					{
						ManifestBuilder.UpdateStringBuilder(ref CS$<>8__locals1.stringBuilder, CS$<>8__locals1.eventMessage, CS$<>8__locals1.writtenSoFar, i - CS$<>8__locals1.writtenSoFar);
						int i3 = i;
						i = i3 + 1;
						CS$<>8__locals1.stringBuilder.Append(escape);
						CS$<>8__locals1.writtenSoFar = i;
					};
					action(CS$<>8__locals1.eventMessage[i], array[num2]);
				}
				else
				{
					int j = i;
					i = j + 1;
				}
			}
			if (CS$<>8__locals1.stringBuilder == null)
			{
				return CS$<>8__locals1.eventMessage;
			}
			ManifestBuilder.UpdateStringBuilder(ref CS$<>8__locals1.stringBuilder, CS$<>8__locals1.eventMessage, CS$<>8__locals1.writtenSoFar, i - CS$<>8__locals1.writtenSoFar);
			return CS$<>8__locals1.stringBuilder.ToString();
		}

		private int TranslateIndexToManifestConvention(int idx, string evtName)
		{
			List<int> list;
			if (this.perEventByteArrayArgIndices.TryGetValue(evtName, out list))
			{
				foreach (int num in list)
				{
					if (idx < num)
					{
						break;
					}
					idx++;
				}
			}
			return idx + 1;
		}

		private Dictionary<int, string> opcodeTab;

		private Dictionary<int, string> taskTab;

		private Dictionary<int, ManifestBuilder.ChannelInfo> channelTab;

		private Dictionary<ulong, string> keywordTab;

		private Dictionary<string, Type> mapsTab;

		private Dictionary<string, string> stringTab;

		private ulong nextChannelKeywordBit = 9223372036854775808UL;

		private const int MaxCountChannels = 8;

		private StringBuilder sb;

		private StringBuilder events;

		private StringBuilder templates;

		private string providerName;

		private ResourceManager resources;

		private EventManifestOptions flags;

		private IList<string> errors;

		private Dictionary<string, List<int>> perEventByteArrayArgIndices;

		private string eventName;

		private int numParams;

		private List<int> byteArrArgIndices;

		private class ChannelInfo
		{
			public string Name;

			public ulong Keywords;

			public EventChannelAttribute Attribs;
		}
	}
}
