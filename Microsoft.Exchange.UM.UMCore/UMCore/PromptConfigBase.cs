using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PromptConfigBase : ICloneable
	{
		internal PromptConfigBase(string name, string type, string conditionString, ActivityManagerConfig managerConfig)
		{
			this.promptName = string.Intern(name.Trim());
			this.promptType = string.Intern(type.Trim());
			this.conditionString = conditionString;
			this.ProsodyRate = "+0%";
			this.language = string.Empty;
			if (conditionString.Length > 0)
			{
				this.conditionTree = ConditionParser.Instance.Parse(conditionString, managerConfig);
			}
		}

		internal static ResourceManager PromptResourceManager
		{
			get
			{
				if (PromptConfigBase.promptResources == null)
				{
					PromptConfigBase.promptResources = new ResourceManager("Microsoft.Exchange.UM.Prompts.Prompts.Strings", Assembly.Load("Microsoft.Exchange.UM.Prompts"));
				}
				return PromptConfigBase.promptResources;
			}
		}

		internal ExpressionParser.Expression Condition
		{
			get
			{
				return this.conditionTree;
			}
		}

		internal string Suffix
		{
			get
			{
				return this.suffix;
			}
		}

		internal string Language
		{
			get
			{
				return this.language;
			}
		}

		protected internal string PromptName
		{
			get
			{
				return this.promptName;
			}
			protected set
			{
				this.promptName = value;
			}
		}

		protected internal string PromptType
		{
			get
			{
				return this.promptType;
			}
			protected set
			{
				this.promptType = value;
			}
		}

		protected internal string ProsodyRate
		{
			get
			{
				return this.prosodyRate;
			}
			protected set
			{
				this.prosodyRate = value;
			}
		}

		protected string ConditionString
		{
			get
			{
				return this.conditionString;
			}
			set
			{
				this.conditionString = value;
			}
		}

		protected ExpressionParser.Expression ConditionTree
		{
			get
			{
				return this.conditionTree;
			}
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		internal static PromptConfigBase Create(XmlNode node, string parentScope, ActivityManagerConfig managerConfig)
		{
			string value = node.Attributes["name"].Value;
			string value2 = node.Attributes["type"].Value;
			string text = ActivityConfig.BuildCondition(parentScope, node);
			string value3 = node.Attributes["suffix"].Value;
			string text2 = (node.Attributes["prosodyRate"] == null) ? string.Empty : string.Intern(node.Attributes["prosodyRate"].Value);
			string text3 = (node.Attributes["language"] == null) ? string.Empty : string.Intern(node.Attributes["language"].Value);
			string text4 = (node.Attributes["limitKey"] == null) ? string.Empty : string.Intern(node.Attributes["limitKey"].Value);
			return PromptConfigBase.Create(value, value2, text, value3, text2, text3, text4, parentScope, managerConfig, PromptConfigBase.ParseParameterPrompts(node, text, managerConfig));
		}

		internal static PromptConfigBase Create(string configName, string configType, string conditionString, params PromptConfigBase[] parameters)
		{
			List<PromptConfigBase> parameters2 = new List<PromptConfigBase>(parameters);
			return PromptConfigBase.Create(configName, configType, conditionString, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, parameters2);
		}

		internal static PromptConfigBase Create(string configName, string configType, string conditionString)
		{
			return PromptConfigBase.Create(configName, configType, conditionString, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null);
		}

		internal static PromptConfigBase Create(string configName, string configType, string conditionString, string suffix, string prosodyRate, string language, string limitKey, string parentScope, ActivityManagerConfig managerConfig, List<PromptConfigBase> parameters)
		{
			if (!string.IsNullOrEmpty(limitKey))
			{
				ConstantValidator.Instance.ValidatePromptLimit(limitKey);
			}
			if (configType != null)
			{
				if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x60006b1-1 == null)
				{
					<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x60006b1-1 = new Dictionary<string, int>(34)
					{
						{
							"statement",
							0
						},
						{
							"multiStatement",
							1
						},
						{
							"wave",
							2
						},
						{
							"text",
							3
						},
						{
							"date",
							4
						},
						{
							"time",
							5
						},
						{
							"digit",
							6
						},
						{
							"tempwave",
							7
						},
						{
							"cardinal",
							8
						},
						{
							"varwave",
							9
						},
						{
							"email",
							10
						},
						{
							"simpleTime",
							11
						},
						{
							"telephone",
							12
						},
						{
							"name",
							13
						},
						{
							"address",
							14
						},
						{
							"timeRange",
							15
						},
						{
							"silence",
							16
						},
						{
							"timeZone",
							17
						},
						{
							"bookmark",
							18
						},
						{
							"emailAddress",
							19
						},
						{
							"language",
							20
						},
						{
							"languageList",
							21
						},
						{
							"textList",
							22
						},
						{
							"businessHours",
							23
						},
						{
							"dayOfWeekTime",
							24
						},
						{
							"aaCustomMenu",
							25
						},
						{
							"aaWelcomeGreeting",
							26
						},
						{
							"aaBusinessLocation",
							27
						},
						{
							"mbxVoicemailGreeting",
							28
						},
						{
							"mbxAwayGreeting",
							29
						},
						{
							"scheduleGroupList",
							30
						},
						{
							"scheduleIntervalList",
							31
						},
						{
							"searchItemDetail",
							32
						},
						{
							"callerNameOrNumber",
							33
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x60006b1-1.TryGetValue(configType, out num))
				{
					PromptConfigBase promptConfigBase;
					switch (num)
					{
					case 0:
						promptConfigBase = new SingleStatementPromptConfig(parameters, configName, conditionString, managerConfig);
						break;
					case 1:
						promptConfigBase = new MultiStatementPromptConfig(parameters, configName, conditionString, managerConfig);
						break;
					case 2:
						promptConfigBase = new PromptConfig<FilePrompt>(configName, configName, conditionString, managerConfig);
						break;
					case 3:
						promptConfigBase = new VariablePromptConfig<TextPrompt, string>(configName, configName, conditionString, managerConfig);
						break;
					case 4:
						promptConfigBase = new VariablePromptConfig<DatePrompt, ExDateTime>(configName, configName, conditionString, managerConfig);
						break;
					case 5:
						promptConfigBase = new VariablePromptConfig<TimePrompt, ExDateTime>(configName, configName, conditionString, managerConfig);
						break;
					case 6:
						promptConfigBase = new VariablePromptConfig<DigitPrompt, string>(configName, configName, conditionString, managerConfig);
						break;
					case 7:
						promptConfigBase = new VariablePromptConfig<TempFilePrompt, ITempWavFile>(configName, configName, conditionString, managerConfig);
						break;
					case 8:
						promptConfigBase = new VariablePromptConfig<CardinalPrompt, int>(configName, configName, conditionString, managerConfig);
						break;
					case 9:
						promptConfigBase = new VariablePromptConfig<VarFilePrompt, string>(configName, configName, conditionString, managerConfig);
						break;
					case 10:
						promptConfigBase = new VariablePromptConfig<EmailPrompt, EmailNormalizedText>(configName, configName, conditionString, managerConfig);
						break;
					case 11:
						promptConfigBase = new VariablePromptConfig<SimpleTimePrompt, ExDateTime>(configName, configName, conditionString, managerConfig);
						break;
					case 12:
						promptConfigBase = new VariablePromptConfig<TelephoneNumberPrompt, PhoneNumber>(configName, configName, conditionString, managerConfig);
						break;
					case 13:
						promptConfigBase = new VariablePromptConfig<SpokenNamePrompt, object>(configName, configName, conditionString, managerConfig);
						break;
					case 14:
						promptConfigBase = new VariablePromptConfig<AddressPrompt, string>(configName, configName, conditionString, managerConfig);
						break;
					case 15:
						promptConfigBase = new VariablePromptConfig<TimeRangePrompt, TimeRange>(configName, configName, conditionString, managerConfig);
						break;
					case 16:
						promptConfigBase = new PromptConfig<SilencePrompt>(configName, configName, conditionString, managerConfig);
						break;
					case 17:
						promptConfigBase = new VariablePromptConfig<TimeZonePrompt, ExTimeZone>(configName, configName, conditionString, managerConfig);
						break;
					case 18:
						promptConfigBase = new PromptConfig<PromptBookmark>(configName, configName, conditionString, managerConfig);
						break;
					case 19:
						promptConfigBase = new VariablePromptConfig<EmailAddressPrompt, string>(configName, configName, conditionString, managerConfig);
						break;
					case 20:
						promptConfigBase = new VariablePromptConfig<LanguagePrompt, CultureInfo>(configName, configName, conditionString, managerConfig);
						break;
					case 21:
						promptConfigBase = new VariablePromptConfig<LanguageListPrompt, List<CultureInfo>>(configName, configName, conditionString, managerConfig);
						break;
					case 22:
						promptConfigBase = new VariablePromptConfig<TextListPrompt, List<string>>(configName, configName, conditionString, managerConfig);
						break;
					case 23:
						promptConfigBase = new VariablePromptConfig<BusinessHoursPrompt, UMAutoAttendant>(configName, configName, conditionString, managerConfig);
						break;
					case 24:
						promptConfigBase = new VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext>(configName, configName, conditionString, managerConfig);
						break;
					case 25:
						promptConfigBase = new VariablePromptConfig<AACustomMenuPrompt, AutoAttendantContext>(configName, configName, conditionString, managerConfig);
						break;
					case 26:
						promptConfigBase = new VariablePromptConfig<AAWelcomeGreetingPrompt, AutoAttendantContext>(configName, configName, conditionString, managerConfig);
						break;
					case 27:
						promptConfigBase = new VariablePromptConfig<AABusinessLocationPrompt, AutoAttendantLocationContext>(configName, configName, conditionString, managerConfig);
						break;
					case 28:
						promptConfigBase = new VariablePromptConfig<MailboxVoicemailGreetingPrompt, object>(configName, configName, conditionString, managerConfig);
						break;
					case 29:
						promptConfigBase = new VariablePromptConfig<MailboxAwayGreetingPrompt, object>(configName, configName, conditionString, managerConfig);
						break;
					case 30:
						promptConfigBase = new VariablePromptConfig<ScheduleGroupListPrompt, List<ScheduleGroup>>(configName, configName, conditionString, managerConfig);
						break;
					case 31:
						promptConfigBase = new VariablePromptConfig<ScheduleIntervalListPrompt, List<TimeRange>>(configName, configName, conditionString, managerConfig);
						break;
					case 32:
						promptConfigBase = new VariablePromptConfig<SearchItemDetailPrompt, ContactSearchItem>(configName, configName, conditionString, managerConfig);
						break;
					case 33:
						promptConfigBase = new VariablePromptConfig<CallerNameOrNumberPrompt, NameOrNumberOfCaller>(configName, configName, conditionString, managerConfig);
						break;
					default:
						goto IL_499;
					}
					promptConfigBase.suffix = suffix;
					promptConfigBase.ProsodyRate = (string.IsNullOrEmpty(prosodyRate) ? "+0%" : prosodyRate);
					promptConfigBase.language = language;
					promptConfigBase.limitKey = limitKey;
					promptConfigBase.Validate();
					return promptConfigBase;
				}
			}
			IL_499:
			throw new FsmConfigurationException(Strings.InvalidPromptType(configType));
		}

		internal static ArrayList BuildConditionalPrompts(ArrayList promptConfigArray, ActivityManager manager, CultureInfo culture, IPromptCounter promptCounts)
		{
			ArrayList arrayList = new ArrayList();
			ExpressionParser.Expression expression = null;
			bool flag = true;
			foreach (object obj in promptConfigArray)
			{
				PromptConfigBase promptConfigBase = (PromptConfigBase)obj;
				if (promptConfigBase.Condition != expression)
				{
					expression = promptConfigBase.Condition;
					object obj2 = (expression == null) ? true : expression.Eval(manager);
					flag = (obj2 != null && obj2 is bool && (bool)obj2);
				}
				if (flag && !PromptConfigBase.IsOverPromptLimit(promptCounts, promptConfigBase.limitKey))
				{
					promptConfigBase.AddPrompts(arrayList, manager, culture);
				}
			}
			return arrayList;
		}

		internal abstract void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture);

		internal virtual void Validate()
		{
			ArrayList arrayList = new ArrayList();
			foreach (CultureInfo culture in UmCultures.GetSupportedPromptCultures())
			{
				this.AddPrompts(arrayList, null, culture);
			}
			arrayList.Clear();
		}

		internal string GetConditionString()
		{
			return this.conditionString ?? string.Empty;
		}

		internal void SetConditionString(string conditionString, ActivityManagerConfig managerConfig)
		{
			this.conditionString = conditionString;
			if (this.conditionString.Length > 0)
			{
				this.conditionTree = ConditionParser.Instance.Parse(conditionString, managerConfig);
			}
		}

		internal virtual string GetSuffixVariable(ActivityManager m)
		{
			throw new NotImplementedException();
		}

		private static bool IsOverPromptLimit(IPromptCounter promptCounter, string limitKey)
		{
			if (string.IsNullOrEmpty(limitKey))
			{
				return false;
			}
			int promptCount = promptCounter.GetPromptCount(limitKey);
			if (promptCount >= 5)
			{
				return true;
			}
			promptCounter.SetPromptCount(limitKey, promptCount + 1);
			return false;
		}

		private static List<PromptConfigBase> ParseParameterPrompts(XmlNode root, string conditionString, ActivityManagerConfig managerConfig)
		{
			List<PromptConfigBase> list = new List<PromptConfigBase>();
			for (int i = 0; i < root.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = root.ChildNodes[i];
				string a = string.Intern(xmlNode.Name);
				if (string.Equals(a, "Prompt", StringComparison.OrdinalIgnoreCase))
				{
					PromptConfigBase promptConfigBase = PromptConfigBase.Create(xmlNode, conditionString, managerConfig);
					if (!promptConfigBase.ConditionString.Equals(conditionString, StringComparison.OrdinalIgnoreCase))
					{
						throw new FsmConfigurationException(Strings.PromptParameterCondition(root.OuterXml));
					}
					list.Add(promptConfigBase);
				}
			}
			return list;
		}

		private static ResourceManager promptResources;

		private string promptName;

		private string promptType;

		private string prosodyRate;

		private string language;

		private string conditionString;

		private ExpressionParser.Expression conditionTree;

		private string suffix;

		private string limitKey;
	}
}
