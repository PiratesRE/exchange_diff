using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class CallerIdRuleEvaluator : IRuleEvaluator, ICallerIdRuleEvaluator
	{
		public CallerIdRuleEvaluator(List<CallerIdBase> conditions)
		{
			this.conditions = conditions;
		}

		public PersonalContactInfo[] MatchedPersonalContacts
		{
			get
			{
				return this.matchedContacts;
			}
		}

		public ADContactInfo MatchedADContact
		{
			get
			{
				return this.matchedADContact;
			}
		}

		public List<string> MatchedPersonaEmails
		{
			get
			{
				return this.matchedPersonaEmails;
			}
		}

		public PhoneNumber GetCallerId()
		{
			return this.callerid;
		}

		public UMSubscriber GetUMSubscriber()
		{
			return this.subscriber;
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			if (this.conditions.Count == 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "CallerIdRuleEvaluator:Evaluate() no callerid conditions defined. Returning true", new object[0]);
				return true;
			}
			this.callerid = dataLoader.GetCallerId();
			this.subscriber = dataLoader.GetUMSubscriber();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			this.conditions.Sort((CallerIdBase left, CallerIdBase right) => left.EvaluationCost - right.EvaluationCost);
			for (int i = 0; i < this.conditions.Count; i++)
			{
				CallerIdBase callerIdBase = this.conditions[i];
				if (!flag && (callerIdBase is ADContactCallerId || callerIdBase is PersonaContactCallerId))
				{
					flag = true;
					this.matchedADContact = dataLoader.GetMatchingADContact(this.callerid);
				}
				if (!flag2 && (callerIdBase is ContactItemCallerId || callerIdBase is ContactFolderCallerId || callerIdBase is PersonaContactCallerId))
				{
					flag2 = true;
					this.matchedContacts = dataLoader.GetMatchingPersonalContacts(this.callerid);
				}
				if (!flag3 && callerIdBase is PersonaContactCallerId)
				{
					flag3 = true;
					this.matchedPersonaEmails = dataLoader.GetMatchingPersonaEmails();
				}
				if (callerIdBase.Evaluate(this))
				{
					return true;
				}
			}
			return false;
		}

		private PersonalContactInfo[] matchedContacts;

		private List<CallerIdBase> conditions;

		private ADContactInfo matchedADContact;

		private List<string> matchedPersonaEmails;

		private PhoneNumber callerid;

		private UMSubscriber subscriber;
	}
}
