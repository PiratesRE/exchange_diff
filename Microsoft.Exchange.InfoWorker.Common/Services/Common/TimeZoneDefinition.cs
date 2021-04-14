using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.Services.Common
{
	internal class TimeZoneDefinition
	{
		public TimeZoneDefinition(XmlElement xmlParent)
		{
			if (this.ParseAttributes(xmlParent) && this.ParsePeriods(xmlParent) && this.ParseTransitionsGroups(xmlParent))
			{
				this.ParseTransitions(xmlParent);
			}
		}

		public TimeZoneDefinition(ExTimeZone exchTimeZone)
		{
			this.timeZone = exchTimeZone;
		}

		public TimeZoneDefinition(string id)
		{
			this.timeZone = null;
			if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(id, out this.timeZone))
			{
				this.id = id;
			}
		}

		internal TimeZoneDefinition(string name, string id)
		{
			this.name = name;
			this.id = id;
		}

		public ExTimeZone ExTimeZone
		{
			get
			{
				if (this.TryAsCustomTimeZone())
				{
					this.ValidateAndProcessXmlData();
					this.ConvertToExTimeZone();
				}
				return this.timeZone;
			}
		}

		private void ValidateAndProcessXmlData()
		{
			for (int i = 0; i < this.transitionsGroups.Length; i++)
			{
				for (int j = i + 1; j < this.transitionsGroups.Length; j++)
				{
					if (!string.IsNullOrEmpty(this.transitionsGroups[i].Id) && this.transitionsGroups[i].Id == this.transitionsGroups[j].Id)
					{
						throw new TimeZoneException((Strings.IDs)3276944824U, new string[]
						{
							"TransitionsGroup.Id",
							"TransitionsGroup.index.0",
							"TransitionsGroup.index.1"
						}, new string[]
						{
							this.transitionsGroups[i].Id,
							i.ToString(),
							j.ToString()
						});
					}
				}
			}
			for (int k = 0; k < this.periods.Length; k++)
			{
				for (int l = k + 1; l < this.periods.Length; l++)
				{
					if (this.periods[k].Id == this.periods[l].Id)
					{
						throw new TimeZoneException(Strings.IDs.MessageInvalidTimeZoneDuplicatePeriods, new string[]
						{
							"Periods.Period.Id",
							"Periods.Period.index.0",
							"Periods.Period.index.1"
						}, new string[]
						{
							this.periods[k].Id,
							k.ToString(),
							l.ToString()
						});
					}
				}
			}
			this.periodsDictionary = new Dictionary<string, TimeZoneDefinition.PeriodType>(this.periods.Length);
			foreach (TimeZoneDefinition.PeriodType periodType in this.periods)
			{
				this.periodsDictionary.Add(periodType.Id, periodType);
			}
		}

		private bool TryAsCustomTimeZone()
		{
			bool flag = this.periods != null && this.periods.Length != 0;
			bool flag2 = this.transitions != null && this.transitions.Transitions != null && this.transitions.Transitions.Length != 0;
			bool flag3 = this.transitionsGroups != null && this.transitionsGroups.Length != 0;
			this.timeZone = null;
			if (!flag && !flag2 && !flag3)
			{
				if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(this.id, out this.timeZone))
				{
					return false;
				}
				throw new TimeZoneException((Strings.IDs)2530022313U, new string[]
				{
					"Id"
				}, new string[]
				{
					this.id
				});
			}
			else
			{
				if (flag && flag2 && flag3)
				{
					return true;
				}
				throw new TimeZoneException((Strings.IDs)2852570616U, new string[]
				{
					"Id"
				}, new string[]
				{
					this.id
				});
			}
		}

		private void ConvertToExTimeZone()
		{
			ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation("tzone://Microsoft/Custom", this.name);
			exTimeZoneInformation.AlternativeId = this.id;
			if (this.transitions.Transitions[0].GetType() != typeof(TimeZoneDefinition.Transition))
			{
				throw new TimeZoneException((Strings.IDs)3332140560U, this.transitions, this.transitions.Transitions[0]);
			}
			for (int i = 0; i < this.transitions.Transitions.Length; i++)
			{
				DateTime? endTransition;
				if (i != this.transitions.Transitions.Length - 1)
				{
					TimeZoneDefinition.AbsoluteDateTransition absoluteDateTransition = this.transitions.Transitions[i + 1] as TimeZoneDefinition.AbsoluteDateTransition;
					if (absoluteDateTransition == null)
					{
						throw new TimeZoneException((Strings.IDs)3644766027U, this.transitions, this.transitions.Transitions[i + 1]);
					}
					endTransition = new DateTime?(absoluteDateTransition.DateTime);
				}
				else
				{
					endTransition = null;
				}
				ExTimeZoneRuleGroup exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(endTransition);
				this.AddRulesToRuleGroup(exTimeZoneRuleGroup, this.transitions.Transitions[i], this.transitions);
				if (exTimeZoneRuleGroup.Rules.Count != 0)
				{
					exTimeZoneInformation.AddGroup(exTimeZoneRuleGroup);
				}
			}
			try
			{
				this.timeZone = new ExTimeZone(exTimeZoneInformation);
			}
			catch (ArgumentException exception)
			{
				throw new TimeZoneException(exception);
			}
			catch (NotImplementedException exception2)
			{
				throw new TimeZoneException(exception2);
			}
			catch (InvalidOperationException exception3)
			{
				throw new TimeZoneException(exception3);
			}
		}

		private void AddRulesToRuleGroup(ExTimeZoneRuleGroup timeZoneRuleGroup, TimeZoneDefinition.Transition transition, TimeZoneDefinition.TransitionsGroup transitions)
		{
			if (transition.To.Kind != TimeZoneDefinition.TransitionTargetKindType.Group)
			{
				throw new TimeZoneException((Strings.IDs)2265146620U, transitions, transition);
			}
			foreach (TimeZoneDefinition.TransitionsGroup transitionsGroup in this.transitionsGroups)
			{
				if (transitionsGroup.Id == transition.To.Value)
				{
					int num = transitionsGroup.Transitions.Length;
					for (int j = 0; j < num; j++)
					{
						int num2 = (j + 1) % num;
						TimeZoneDefinition.Transition transition2 = transitionsGroup.Transitions[j];
						TimeZoneDefinition.Transition transitionFromPeriod = transitionsGroup.Transitions[num2];
						if (transition2.To.Kind != TimeZoneDefinition.TransitionTargetKindType.Period)
						{
							throw new TimeZoneException(Strings.IDs.MessageInvalidTimeZoneReferenceToPeriod, transitionsGroup, transition2);
						}
						this.AddRuleToRuleGroup(timeZoneRuleGroup, transition2, transitionFromPeriod, transitionsGroup);
					}
					return;
				}
			}
			throw new TimeZoneException(Strings.IDs.MessageInvalidTimeZoneMissedGroup, transitions, transition);
		}

		private void AddRuleToRuleGroup(ExTimeZoneRuleGroup timeZoneRuleGroup, TimeZoneDefinition.Transition transitionToPeriod, TimeZoneDefinition.Transition transitionFromPeriod, TimeZoneDefinition.TransitionsGroup transitionToGroup)
		{
			TimeZoneDefinition.RecurringDayTransition recurringDayTransition = transitionFromPeriod as TimeZoneDefinition.RecurringDayTransition;
			TimeZoneDefinition.RecurringDateTransition recurringDateTransition = transitionFromPeriod as TimeZoneDefinition.RecurringDateTransition;
			if (!this.periodsDictionary.ContainsKey(transitionToPeriod.To.Value))
			{
				throw new TimeZoneException((Strings.IDs)3865092385U, transitionToGroup, transitionToPeriod);
			}
			TimeZoneDefinition.PeriodType periodType = this.periodsDictionary[transitionToPeriod.To.Value];
			TimeSpan bias = XmlConvert.ToTimeSpan(periodType.Bias);
			bias = bias.Negate();
			ExYearlyRecurringTime observanceEnd;
			if (recurringDateTransition != null)
			{
				TimeSpan timeSpan = TimeZoneDefinition.ConvertOffsetToTimeSpan(recurringDateTransition.TimeOffset, transitionToPeriod, transitionToGroup);
				try
				{
					observanceEnd = new ExYearlyRecurringDate(recurringDateTransition.Month, recurringDateTransition.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
					goto IL_16C;
				}
				catch (ArgumentOutOfRangeException ex)
				{
					throw new TimeZoneException((Strings.IDs)3961981453U, ex, transitionToGroup, transitionToPeriod, "ParameterName", ex.ParamName);
				}
			}
			if (recurringDayTransition != null)
			{
				TimeSpan timeSpan2 = TimeZoneDefinition.ConvertOffsetToTimeSpan(recurringDayTransition.TimeOffset, transitionToPeriod, transitionToGroup);
				DayOfWeek? dayOfWeek = this.ConvertToDayOfWeek(recurringDayTransition.DayOfWeek);
				if (dayOfWeek == null)
				{
					throw new TimeZoneException(Strings.IDs.MessageInvalidTimeZoneDayOfWeekValue, transitionToGroup, transitionToPeriod);
				}
				try
				{
					observanceEnd = new ExYearlyRecurringDay(recurringDayTransition.Occurrence, dayOfWeek.Value, recurringDayTransition.Month, timeSpan2.Hours, timeSpan2.Minutes, timeSpan2.Seconds, timeSpan2.Milliseconds);
					goto IL_16C;
				}
				catch (ArgumentOutOfRangeException ex2)
				{
					throw new TimeZoneException((Strings.IDs)3961981453U, ex2, transitionToGroup, transitionToPeriod, "ParameterName", ex2.ParamName);
				}
			}
			observanceEnd = null;
			IL_16C:
			ExTimeZoneRule ruleInfo = new ExTimeZoneRule(periodType.Id, periodType.Name, bias, observanceEnd);
			timeZoneRuleGroup.AddRule(ruleInfo);
		}

		private DayOfWeek? ConvertToDayOfWeek(string dayOfWeek)
		{
			switch (dayOfWeek)
			{
			case "Monday":
				return new DayOfWeek?(DayOfWeek.Monday);
			case "Tuesday":
				return new DayOfWeek?(DayOfWeek.Tuesday);
			case "Wednesday":
				return new DayOfWeek?(DayOfWeek.Wednesday);
			case "Thursday":
				return new DayOfWeek?(DayOfWeek.Thursday);
			case "Friday":
				return new DayOfWeek?(DayOfWeek.Friday);
			case "Saturday":
				return new DayOfWeek?(DayOfWeek.Saturday);
			case "Sunday":
				return new DayOfWeek?(DayOfWeek.Sunday);
			}
			return null;
		}

		private static TimeSpan ConvertOffsetToTimeSpan(string timeOffset, TimeZoneDefinition.Transition transitionToPeriod, TimeZoneDefinition.TransitionsGroup transitionToGroup)
		{
			TimeSpan result;
			try
			{
				result = XmlConvert.ToTimeSpan(timeOffset);
			}
			catch (FormatException innerException)
			{
				throw new TimeZoneException(Strings.IDs.MessageInvalidTimeZoneInvalidOffsetFormat, innerException, transitionToGroup, transitionToPeriod);
			}
			return result;
		}

		private bool ParseAttributes(XmlElement xmlParent)
		{
			this.name = xmlParent.GetAttribute("Name");
			this.id = xmlParent.GetAttribute("Id");
			return !string.IsNullOrEmpty(this.id);
		}

		private bool ParsePeriods(XmlElement xmlParent)
		{
			XmlElement xmlElement = xmlParent["Periods", xmlParent.NamespaceURI];
			if (xmlElement == null || xmlElement.ChildNodes.Count == 0)
			{
				return false;
			}
			int num = 0;
			this.periods = new TimeZoneDefinition.PeriodType[xmlElement.ChildNodes.Count];
			foreach (object obj in xmlElement.ChildNodes)
			{
				XmlElement xmlParent2 = (XmlElement)obj;
				string andVerifyNotNullString = TimeZoneDefinition.GetAndVerifyNotNullString(xmlParent2, "Bias", Strings.IDs.MessageInvalidTimeZonePeriodNullBias, true);
				string andVerifyNotNullString2 = TimeZoneDefinition.GetAndVerifyNotNullString(xmlParent2, "Name", (Strings.IDs)2912574056U, true);
				string andVerifyNotNullString3 = TimeZoneDefinition.GetAndVerifyNotNullString(xmlParent2, "Id", (Strings.IDs)3558532322U, true);
				this.periods[num++] = new TimeZoneDefinition.PeriodType(andVerifyNotNullString, andVerifyNotNullString2, andVerifyNotNullString3);
			}
			return true;
		}

		private bool ParseTransitionsGroups(XmlElement xmlParent)
		{
			XmlElement xmlElement = xmlParent["TransitionsGroups", xmlParent.NamespaceURI];
			if (xmlElement == null || xmlElement.ChildNodes.Count == 0)
			{
				return false;
			}
			int num = 0;
			this.transitionsGroups = new TimeZoneDefinition.TransitionsGroup[xmlElement.ChildNodes.Count];
			foreach (object obj in xmlElement.ChildNodes)
			{
				XmlElement xmlTransitionGroup = (XmlElement)obj;
				this.transitionsGroups[num] = this.ParseTransitionsGroup(xmlTransitionGroup, true);
				num++;
			}
			return true;
		}

		private TimeZoneDefinition.TransitionsGroup ParseTransitionsGroup(XmlElement xmlTransitionGroup, bool checkTransitionsGroup)
		{
			TimeZoneDefinition.TransitionsGroup transitionsGroup = new TimeZoneDefinition.TransitionsGroup(checkTransitionsGroup);
			transitionsGroup.Id = TimeZoneDefinition.GetAndVerifyNotNullString(xmlTransitionGroup, "Id", (Strings.IDs)4098403379U, checkTransitionsGroup);
			transitionsGroup.Transitions = this.ParseTransitionArray(xmlTransitionGroup);
			if (checkTransitionsGroup && transitionsGroup.Transitions.Length > 2)
			{
				throw new TimeZoneException((Strings.IDs)3442221872U, transitionsGroup, transitionsGroup.Transitions[2]);
			}
			return transitionsGroup;
		}

		private TimeZoneDefinition.Transition[] ParseTransitionArray(XmlElement xmlTransitions)
		{
			int count = xmlTransitions.ChildNodes.Count;
			TimeZoneDefinition.Transition[] array = new TimeZoneDefinition.Transition[count];
			int num = 0;
			foreach (object obj in xmlTransitions.ChildNodes)
			{
				XmlElement xmlTransition = (XmlElement)obj;
				array[num] = this.ParseTransitionElement(xmlTransition);
				num++;
			}
			return array;
		}

		private TimeZoneDefinition.Transition ParseTransitionElement(XmlElement xmlTransition)
		{
			string localName = xmlTransition.LocalName;
			string a;
			if ((a = localName) != null)
			{
				if (a == "AbsoluteDateTransition")
				{
					return this.ParseAbsoluteDateTransition(xmlTransition);
				}
				if (a == "RecurringDateTransition")
				{
					return this.ParseRecurringDateTransition(xmlTransition);
				}
				if (a == "RecurringDayTransition")
				{
					return this.ParseRecurringDayTransition(xmlTransition);
				}
			}
			return this.ParseTransition(xmlTransition);
		}

		private TimeZoneDefinition.Transition ParseTransition(XmlElement xmlParent)
		{
			return new TimeZoneDefinition.Transition(new TimeZoneDefinition.TransitionTargetType(xmlParent));
		}

		private TimeZoneDefinition.AbsoluteDateTransition ParseAbsoluteDateTransition(XmlElement xmlParent)
		{
			string s = this.ParseStringElement(xmlParent, "DateTime");
			DateTime dateTime = XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
			return new TimeZoneDefinition.AbsoluteDateTransition(new TimeZoneDefinition.TransitionTargetType(xmlParent), dateTime);
		}

		private TimeZoneDefinition.RecurringDateTransition ParseRecurringDateTransition(XmlElement xmlParent)
		{
			return new TimeZoneDefinition.RecurringDateTransition(new TimeZoneDefinition.TransitionTargetType(xmlParent), this.ParseStringElement(xmlParent, "TimeOffset"), this.ParseIntElement(xmlParent, "Month"), this.ParseIntElement(xmlParent, "Day"));
		}

		private TimeZoneDefinition.RecurringDayTransition ParseRecurringDayTransition(XmlElement xmlParent)
		{
			return new TimeZoneDefinition.RecurringDayTransition(new TimeZoneDefinition.TransitionTargetType(xmlParent), this.ParseStringElement(xmlParent, "TimeOffset"), this.ParseIntElement(xmlParent, "Month"), this.ParseStringElement(xmlParent, "DayOfWeek"), this.ParseIntElement(xmlParent, "Occurrence"));
		}

		private string ParseStringElement(XmlElement xmlParent, string xmlElementName)
		{
			XmlElement xmlElement = xmlParent[xmlElementName, xmlParent.NamespaceURI];
			return xmlElement.InnerText;
		}

		private int ParseIntElement(XmlElement xmlParent, string xmlElementName)
		{
			XmlElement xmlElement = xmlParent[xmlElementName, xmlParent.NamespaceURI];
			return int.Parse(xmlElement.InnerText);
		}

		private bool ParseTransitions(XmlElement xmlParent)
		{
			XmlElement xmlElement = xmlParent["Transitions", xmlParent.NamespaceURI];
			if (xmlElement == null || xmlElement.ChildNodes.Count == 0)
			{
				return false;
			}
			this.transitions = this.ParseTransitionsGroup(xmlElement, false);
			return true;
		}

		private static string GetAndVerifyNotNullString(XmlElement xmlParent, string xmlAttributeName, Enum messageId, bool checkNull)
		{
			string attribute = xmlParent.GetAttribute(xmlAttributeName);
			if (string.IsNullOrEmpty(attribute) && checkNull)
			{
				throw new TimeZoneException(messageId);
			}
			return attribute;
		}

		public void Render(XmlElement xmlParent, string prefix, string typeNameSpace, string xmlElementName, bool returnFullTimeZoneData, CultureInfo cultureInfo)
		{
			if (this.timeZone != null)
			{
				this.typeNameSpace = typeNameSpace;
				this.typePrefix = prefix;
				XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(xmlParent, prefix, xmlElementName, this.typeNameSpace);
				this.name = this.timeZone.LocalizableDisplayName.ToString(cultureInfo);
				this.id = (this.timeZone.AlternativeId ?? string.Empty);
				TimeZoneDefinition.XmlHelper.CreateAttribute(xmlElement, "Name", this.name, false);
				TimeZoneDefinition.XmlHelper.CreateAttribute(xmlElement, "Id", this.id, false);
				if (returnFullTimeZoneData)
				{
					this.RenderTimeZoneContentElements(xmlElement);
				}
			}
		}

		private void RenderTimeZoneContentElements(XmlElement xmlParent)
		{
			XmlElement xmlPeriods = TimeZoneDefinition.XmlHelper.CreateElement(xmlParent, this.typePrefix, "Periods", this.typeNameSpace);
			XmlElement xmlParent2 = TimeZoneDefinition.XmlHelper.CreateElement(xmlParent, this.typePrefix, "TransitionsGroups", this.typeNameSpace);
			XmlElement xmlTransitions = TimeZoneDefinition.XmlHelper.CreateElement(xmlParent, this.typePrefix, "Transitions", this.typeNameSpace);
			int num = 0;
			this.RenderTransitionToGroup(xmlTransitions, num);
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup in this.timeZone.TimeZoneInformation.Groups)
			{
				XmlElement xmlTransitionsGroup = this.RenderTransitionsGroup(xmlParent2, num);
				for (int i = 0; i < exTimeZoneRuleGroup.Rules.Count; i++)
				{
					int index = (i + 1) % exTimeZoneRuleGroup.Rules.Count;
					this.RenderPeriodAndReference(xmlPeriods, xmlTransitionsGroup, exTimeZoneRuleGroup.Rules[i], exTimeZoneRuleGroup.Rules[index], num);
				}
				num++;
				if (exTimeZoneRuleGroup.EndTransition != null)
				{
					XmlElement parentElement = this.RenderTransitionToGroup(xmlTransitions, num);
					string textValue = XmlConvert.ToString(exTimeZoneRuleGroup.EndTransition.Value, XmlDateTimeSerializationMode.Unspecified);
					TimeZoneDefinition.XmlHelper.CreateTextElement(parentElement, this.typePrefix, "DateTime", textValue, this.typeNameSpace);
				}
			}
		}

		private XmlElement RenderTransitionsGroup(XmlElement xmlParent, int ruleGroupIdx)
		{
			XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(xmlParent, this.typePrefix, "TransitionsGroup", this.typeNameSpace);
			TimeZoneDefinition.XmlHelper.CreateAttribute(xmlElement, "Id", ruleGroupIdx.ToString());
			return xmlElement;
		}

		private void RenderPeriodAndReference(XmlElement xmlPeriods, XmlElement xmlTransitionsGroup, ExTimeZoneRule rule, ExTimeZoneRule nextRule, int ruleGroupIdx)
		{
			XmlElement parentElement = TimeZoneDefinition.XmlHelper.CreateElement(xmlPeriods, this.typePrefix, "Period", this.typeNameSpace);
			TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, "Bias", TimeZoneDefinition.RenderXsDuration(rule.Bias.Negate()));
			TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, "Name", rule.DisplayName);
			TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, "Id", rule.Id);
			this.RenderTransitionToPeriod(xmlTransitionsGroup, rule, nextRule, ruleGroupIdx);
		}

		private XmlElement RenderTransitionToGroup(XmlElement xmlTransitions, int ruleGroupIdx)
		{
			XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(xmlTransitions, this.typePrefix, (ruleGroupIdx == 0) ? "Transition" : "AbsoluteDateTransition", this.typeNameSpace);
			XmlElement parentElement = TimeZoneDefinition.XmlHelper.CreateTextElement(xmlElement, this.typePrefix, "To", ruleGroupIdx.ToString(), this.typeNameSpace);
			TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, "Kind", "Group");
			return xmlElement;
		}

		private void RenderTransitionToPeriod(XmlElement xmlTransitionsGroup, ExTimeZoneRule rule, ExTimeZoneRule nextRule, int ruleGroupIds)
		{
			ExYearlyRecurringTime observanceEnd = rule.ObservanceEnd;
			ExYearlyRecurringDate exYearlyRecurringDate = rule.ObservanceEnd as ExYearlyRecurringDate;
			ExYearlyRecurringDay exYearlyRecurringDay = rule.ObservanceEnd as ExYearlyRecurringDay;
			string localName;
			if (exYearlyRecurringDay != null)
			{
				localName = "RecurringDayTransition";
			}
			else if (exYearlyRecurringDate != null)
			{
				localName = "RecurringDateTransition";
			}
			else
			{
				if (observanceEnd != null)
				{
					return;
				}
				localName = "Transition";
			}
			XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(xmlTransitionsGroup, this.typePrefix, localName, this.typeNameSpace);
			XmlElement parentElement = TimeZoneDefinition.XmlHelper.CreateTextElement(xmlElement, this.typePrefix, "To", nextRule.Id, this.typeNameSpace);
			TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, "Kind", "Period");
			if (observanceEnd != null)
			{
				this.RenderExYearlyRecurringTime(xmlElement, observanceEnd);
				if (exYearlyRecurringDay != null)
				{
					TimeZoneDefinition.XmlHelper.CreateTextElement(xmlElement, this.typePrefix, "DayOfWeek", exYearlyRecurringDay.DayOfWeek.ToString(), this.typeNameSpace);
					TimeZoneDefinition.XmlHelper.CreateTextElement(xmlElement, this.typePrefix, "Occurrence", exYearlyRecurringDay.Occurrence.ToString(), this.typeNameSpace);
					return;
				}
				if (exYearlyRecurringDate != null)
				{
					TimeZoneDefinition.XmlHelper.CreateTextElement(xmlElement, this.typePrefix, "Day", exYearlyRecurringDate.Day.ToString(), this.typeNameSpace);
				}
			}
		}

		private void RenderExYearlyRecurringTime(XmlElement xmlTransition, ExYearlyRecurringTime recurringTime)
		{
			if (recurringTime != null)
			{
				TimeSpan timeSpan;
				if (recurringTime.Hour < 0)
				{
					timeSpan = new TimeSpan(0, -recurringTime.Hour, recurringTime.Minute, recurringTime.Second, recurringTime.Milliseconds).Negate();
				}
				else
				{
					timeSpan = new TimeSpan(0, recurringTime.Hour, recurringTime.Minute, recurringTime.Second, recurringTime.Milliseconds);
				}
				TimeZoneDefinition.XmlHelper.CreateTextElement(xmlTransition, this.typePrefix, "TimeOffset", TimeZoneDefinition.RenderXsDuration(timeSpan), this.typeNameSpace);
				TimeZoneDefinition.XmlHelper.CreateTextElement(xmlTransition, this.typePrefix, "Month", recurringTime.Month.ToString(), this.typeNameSpace);
			}
		}

		private static string RenderXsDuration(TimeSpan timeSpan)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (timeSpan.Ticks < 0L)
			{
				timeSpan = timeSpan.Negate();
				stringBuilder.Append("-");
			}
			stringBuilder.Append("PT");
			if (timeSpan.Hours != 0 || timeSpan.Ticks == 0L)
			{
				stringBuilder.Append(timeSpan.Hours.ToString());
				stringBuilder.Append("H");
			}
			if (timeSpan.Minutes != 0)
			{
				stringBuilder.Append(timeSpan.Minutes.ToString());
				stringBuilder.Append("M");
			}
			if (timeSpan.Seconds != 0 || timeSpan.Milliseconds != 0)
			{
				stringBuilder.Append(timeSpan.Seconds.ToString());
				if (timeSpan.Milliseconds != 0)
				{
					stringBuilder.Append("." + timeSpan.Milliseconds.ToString("000"));
				}
				stringBuilder.Append("S");
			}
			return stringBuilder.ToString();
		}

		private const string XmlAttributeBias = "Bias";

		private const string XmlAttributeId = "Id";

		private const string XmlAttributeKind = "Kind";

		private const string XmlAttributeName = "Name";

		private const string XmlElementAbsoluteDateTransition = "AbsoluteDateTransition";

		private const string XmlElementDateTime = "DateTime";

		private const string XmlElementDay = "Day";

		private const string XmlElementDayOfWeek = "DayOfWeek";

		private const string XmlElementGroup = "Group";

		private const string XmlElementMonth = "Month";

		private const string XmlElementTimeOffset = "TimeOffset";

		private const string XmlElementTo = "To";

		private const string XmlElementTransition = "Transition";

		private const string XmlElementTransitions = "Transitions";

		private const string XmlElementTransitionsGroups = "TransitionsGroups";

		private const string XmlElementTransitionsGroup = "TransitionsGroup";

		private const string XmlElementOccurrence = "Occurrence";

		private const string XmlElemendPeriod = "Period";

		private const string XmlElementPeriods = "Periods";

		private const string XmlElementRecurringDateTransition = "RecurringDateTransition";

		private const string XmlElementRecurringDayTransition = "RecurringDayTransition";

		private const string customTimeZoneId = "tzone://Microsoft/Custom";

		private string id;

		private string name;

		protected TimeZoneDefinition.PeriodType[] periods;

		protected TimeZoneDefinition.TransitionsGroup[] transitionsGroups;

		protected TimeZoneDefinition.TransitionsGroup transitions;

		private Dictionary<string, TimeZoneDefinition.PeriodType> periodsDictionary;

		private ExTimeZone timeZone;

		private string typeNameSpace;

		private string typePrefix;

		internal enum TransitionTargetKindType
		{
			Period,
			Group
		}

		internal class TransitionsGroup
		{
			public TransitionsGroup(bool transitionsGroup, string id, TimeZoneDefinition.Transition[] transitions) : this(transitionsGroup)
			{
				this.id = id;
				this.transitions = transitions;
			}

			internal TransitionsGroup(bool transitionsGroup)
			{
				this.name = (transitionsGroup ? "TransitionsGroup" : "Transitions");
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public TimeZoneDefinition.Transition[] Transitions
			{
				get
				{
					return this.transitions;
				}
				set
				{
					this.transitions = value;
				}
			}

			public string Id
			{
				get
				{
					return this.id;
				}
				set
				{
					this.id = value;
				}
			}

			private readonly string name;

			private TimeZoneDefinition.Transition[] transitions;

			private string id;
		}

		internal class Transition
		{
			public Transition(TimeZoneDefinition.TransitionTargetType to)
			{
				this.to = to;
			}

			public TimeZoneDefinition.TransitionTargetType To
			{
				get
				{
					return this.to;
				}
				set
				{
					this.to = value;
				}
			}

			private TimeZoneDefinition.TransitionTargetType to;
		}

		internal class TransitionTargetType
		{
			public TransitionTargetType(XmlElement xmlParent)
			{
				XmlElement xmlElement = xmlParent["To", xmlParent.NamespaceURI];
				string attribute = xmlElement.GetAttribute("Kind");
				this.Value = xmlElement.InnerText;
				string a;
				if ((a = attribute) != null)
				{
					if (a == "Group")
					{
						this.Kind = TimeZoneDefinition.TransitionTargetKindType.Group;
						return;
					}
					if (!(a == "Period"))
					{
						return;
					}
					this.Kind = TimeZoneDefinition.TransitionTargetKindType.Period;
				}
			}

			public TransitionTargetType(TimeZoneDefinition.TransitionTargetKindType kind, string target)
			{
				this.kind = kind;
				this.value = target;
			}

			public TimeZoneDefinition.TransitionTargetKindType Kind
			{
				get
				{
					return this.kind;
				}
				set
				{
					this.kind = value;
				}
			}

			public string Value
			{
				get
				{
					return this.value;
				}
				set
				{
					this.value = value;
				}
			}

			private TimeZoneDefinition.TransitionTargetKindType kind;

			private string value;
		}

		internal class AbsoluteDateTransition : TimeZoneDefinition.Transition
		{
			public AbsoluteDateTransition(TimeZoneDefinition.TransitionTargetType to, DateTime dateTime) : base(to)
			{
				this.dateTime = dateTime;
			}

			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
				set
				{
					this.dateTime = value;
				}
			}

			private DateTime dateTime;
		}

		internal abstract class RecurringTimeTransitionType : TimeZoneDefinition.Transition
		{
			public RecurringTimeTransitionType(TimeZoneDefinition.TransitionTargetType to, string timeOffset, int month) : base(to)
			{
				this.timeOffset = timeOffset;
				this.month = month;
			}

			public string TimeOffset
			{
				get
				{
					return this.timeOffset;
				}
				set
				{
					this.timeOffset = value;
				}
			}

			public int Month
			{
				get
				{
					return this.month;
				}
				set
				{
					this.month = value;
				}
			}

			private string timeOffset;

			private int month;
		}

		internal class RecurringDayTransition : TimeZoneDefinition.RecurringTimeTransitionType
		{
			public RecurringDayTransition(TimeZoneDefinition.TransitionTargetType to, string timeOffset, int month, string dayOfWeek, int occurrence) : base(to, timeOffset, month)
			{
				this.dayOfWeek = dayOfWeek;
				this.occurrence = occurrence;
			}

			public string DayOfWeek
			{
				get
				{
					return this.dayOfWeek;
				}
				set
				{
					this.dayOfWeek = value;
				}
			}

			public int Occurrence
			{
				get
				{
					return this.occurrence;
				}
				set
				{
					this.occurrence = value;
				}
			}

			private string dayOfWeek;

			private int occurrence;
		}

		internal class RecurringDateTransition : TimeZoneDefinition.RecurringTimeTransitionType
		{
			public RecurringDateTransition(TimeZoneDefinition.TransitionTargetType to, string timeOffset, int month, int day) : base(to, timeOffset, month)
			{
				this.day = day;
			}

			public int Day
			{
				get
				{
					return this.day;
				}
				set
				{
					this.day = value;
				}
			}

			private int day;
		}

		internal class PeriodType
		{
			public PeriodType(string bias, string name, string id)
			{
				this.bias = bias;
				this.name = name;
				this.id = id;
			}

			public string Bias
			{
				get
				{
					return this.bias;
				}
				set
				{
					this.bias = value;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public string Id
			{
				get
				{
					return this.id;
				}
				set
				{
					this.id = value;
				}
			}

			private string bias;

			private string name;

			private string id;
		}

		private class Diagnostics
		{
			[Conditional("DEBUG")]
			internal static void Assert(bool condition)
			{
			}

			[Conditional("DEBUG")]
			internal static void Assert(bool condition, string format, params object[] parameters)
			{
			}
		}

		private class XmlHelper
		{
			public static XmlElement CreateElement(XmlElement parentElement, string prefix, string localName, string namespaceUri)
			{
				XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(parentElement.OwnerDocument, prefix, localName, namespaceUri);
				parentElement.AppendChild(xmlElement);
				return xmlElement;
			}

			public static XmlElement CreateElement(XmlDocument xmlDocument, string prefix, string localName, string namespaceUri)
			{
				return xmlDocument.CreateElement(prefix, localName, namespaceUri);
			}

			public static XmlElement CreateTextElement(XmlElement parentElement, string prefix, string localName, string textValue, string namespaceUri)
			{
				XmlElement xmlElement = TimeZoneDefinition.XmlHelper.CreateElement(parentElement, prefix, localName, namespaceUri);
				TimeZoneDefinition.XmlHelper.AppendText(xmlElement, textValue);
				return xmlElement;
			}

			private static void AppendText(XmlElement parentElement, string textValue)
			{
				if (!string.IsNullOrEmpty(textValue))
				{
					XmlText newChild = parentElement.OwnerDocument.CreateTextNode(textValue);
					parentElement.AppendChild(newChild);
				}
			}

			public static XmlAttribute CreateAttribute(XmlElement parentElement, string attributeName, string attributeValue)
			{
				return TimeZoneDefinition.XmlHelper.CreateAttribute(parentElement, attributeName, attributeValue, true);
			}

			public static XmlAttribute CreateAttribute(XmlElement parentElement, string attributeName, string attributeValue, bool checkAttributeValueIsEmpty)
			{
				XmlAttribute xmlAttribute = parentElement.OwnerDocument.CreateAttribute(attributeName);
				xmlAttribute.Value = attributeValue;
				parentElement.Attributes.Append(xmlAttribute);
				return xmlAttribute;
			}
		}
	}
}
