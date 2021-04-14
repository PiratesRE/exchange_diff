using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Common
{
	internal class TimeZoneDefinitionAdaptor : TimeZoneDefinition
	{
		private void ConvertPeriods()
		{
			this.periods = new TimeZoneDefinition.PeriodType[this.timeZoneDefinitionProxy.Periods.Length];
			int num = 0;
			foreach (Microsoft.Exchange.SoapWebClient.EWS.PeriodType periodType in this.timeZoneDefinitionProxy.Periods)
			{
				this.periods[num++] = new TimeZoneDefinition.PeriodType(periodType.Bias, periodType.Name, periodType.Id);
			}
		}

		private void ConvertTransitionsGroups()
		{
			this.transitionsGroups = new TimeZoneDefinition.TransitionsGroup[this.timeZoneDefinitionProxy.TransitionsGroups.Length];
			int num = 0;
			foreach (ArrayOfTransitionsType allTransitions in this.timeZoneDefinitionProxy.TransitionsGroups)
			{
				TimeZoneDefinition.TransitionsGroup container = new TimeZoneDefinition.TransitionsGroup(true);
				this.transitionsGroups[num++] = this.ConvertTransitions(container, allTransitions);
			}
		}

		private void ConvertTransitions()
		{
			TimeZoneDefinition.TransitionsGroup container = new TimeZoneDefinition.TransitionsGroup(false);
			this.transitions = this.ConvertTransitions(container, this.timeZoneDefinitionProxy.Transitions);
		}

		public TimeZoneDefinitionAdaptor(TimeZoneDefinitionType timeZoneDefinitionProxy) : base(timeZoneDefinitionProxy.Name, timeZoneDefinitionProxy.Id)
		{
			this.timeZoneDefinitionProxy = timeZoneDefinitionProxy;
			this.ConvertPeriods();
			this.ConvertTransitions();
			this.ConvertTransitionsGroups();
		}

		private TimeZoneDefinition.TransitionsGroup ConvertTransitions(TimeZoneDefinition.TransitionsGroup container, ArrayOfTransitionsType allTransitions)
		{
			container.Id = allTransitions.Id;
			container.Transitions = new TimeZoneDefinition.Transition[allTransitions.Items.Length];
			int num = 0;
			foreach (TransitionType transitionType in allTransitions.Items)
			{
				container.Transitions[num++] = this.ConvertTransition(transitionType);
			}
			return container;
		}

		private TimeZoneDefinition.TransitionTargetKindType ConvertTransitionKind(Microsoft.Exchange.SoapWebClient.EWS.TransitionTargetKindType transitionKindProxy)
		{
			if (transitionKindProxy == Microsoft.Exchange.SoapWebClient.EWS.TransitionTargetKindType.Group)
			{
				return TimeZoneDefinition.TransitionTargetKindType.Group;
			}
			return TimeZoneDefinition.TransitionTargetKindType.Period;
		}

		private TimeZoneDefinition.Transition ConvertTransition(TransitionType transitionType)
		{
			TimeZoneDefinition.TransitionTargetType to = new TimeZoneDefinition.TransitionTargetType(this.ConvertTransitionKind(transitionType.To.Kind), transitionType.To.Value);
			AbsoluteDateTransitionType absoluteDateTransitionType = transitionType as AbsoluteDateTransitionType;
			if (absoluteDateTransitionType != null)
			{
				return new TimeZoneDefinition.AbsoluteDateTransition(to, absoluteDateTransitionType.DateTime);
			}
			RecurringDayTransitionType recurringDayTransitionType = transitionType as RecurringDayTransitionType;
			if (recurringDayTransitionType != null)
			{
				return new TimeZoneDefinition.RecurringDayTransition(to, recurringDayTransitionType.TimeOffset, recurringDayTransitionType.Month, recurringDayTransitionType.DayOfWeek, recurringDayTransitionType.Occurrence);
			}
			RecurringDateTransitionType recurringDateTransitionType = transitionType as RecurringDateTransitionType;
			if (recurringDateTransitionType != null)
			{
				return new TimeZoneDefinition.RecurringDateTransition(to, recurringDateTransitionType.TimeOffset, recurringDateTransitionType.Month, recurringDateTransitionType.Day);
			}
			return new TimeZoneDefinition.Transition(to);
		}

		private TimeZoneDefinitionType timeZoneDefinitionProxy;
	}
}
