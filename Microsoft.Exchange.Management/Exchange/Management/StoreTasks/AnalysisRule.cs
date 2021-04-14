using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	internal class AnalysisRule
	{
		protected AnalysisRule()
		{
		}

		public string Name { get; protected set; }

		public AnalysisRule.RuleAlertLevel AlertLevel { get; protected set; }

		public string Message { get; protected set; }

		public string[] RuleProperties
		{
			get
			{
				return (from f in this.RequiredProperties
				select f.Name).ToArray<string>();
			}
		}

		internal IEnumerable<PropertyDefinition> RequiredProperties { get; set; }

		public void Analyze(LinkedList<CalendarLogAnalysis> list)
		{
			for (LinkedListNode<CalendarLogAnalysis> linkedListNode = list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				this.AnalyzeLog(linkedListNode);
			}
		}

		protected virtual void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}:{2}", this.AlertLevel, this.Name, this.Message);
		}

		public static IEnumerable<AnalysisRule> GetAnalysisRules()
		{
			return new List<AnalysisRule>
			{
				new MessageClassCheck(),
				new MissingOrganizerEmailAddressCheck(),
				new MissingSenderEmailAddressCheck(),
				new StartTimeCheck(),
				new EndTimeCheck(),
				new CheckClientIntent(),
				new KnowniPhoneIssues(),
				new SentRepresentingChangeCheck()
			};
		}

		internal AnalysisRule Clone()
		{
			return new AnalysisRule
			{
				AlertLevel = this.AlertLevel,
				Message = this.Message,
				Name = this.Name,
				RequiredProperties = this.RequiredProperties
			};
		}

		public enum RuleAlertLevel
		{
			Info,
			Warning,
			Error
		}
	}
}
