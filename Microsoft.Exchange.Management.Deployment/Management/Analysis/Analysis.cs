using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal abstract class Analysis : IAnalysisAccessor, IXmlSerializable
	{
		public Analysis(IDataProviderFactory providers) : this(providers, (AnalysisMember x) => true, (AnalysisMember x) => true)
		{
		}

		public Analysis(IDataProviderFactory providers, Func<AnalysisMember, bool> startFilter, Func<AnalysisMember, bool> conclusionsFilter)
		{
			this.Providers = providers;
			this.StartFilter = startFilter;
			this.ConclusionsFilter = conclusionsFilter;
			this.isAnalysisStarted = false;
			this.analysisMemberNames = new Lazy<Dictionary<AnalysisMember, string>>(new Func<Dictionary<AnalysisMember, string>>(this.PopulateAnalysisMemberNames), true);
			this.totalConclusionRules = new Lazy<int>(() => this.Rules.Where(this.ConclusionsFilter).Count<AnalysisMember>(), true);
			this.RootAnalysisMember = new RootAnalysisMember(this);
			this.progressUpdateLock = new object();
		}

		public event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;

		public IDataProviderFactory Providers { get; private set; }

		public RootAnalysisMember RootAnalysisMember { get; private set; }

		public int CompletedRules
		{
			get
			{
				int result;
				lock (this.progressUpdateLock)
				{
					result = this.completedConclusionRules;
				}
				return result;
			}
		}

		public int TotalConclusionRules
		{
			get
			{
				return this.totalConclusionRules.Value;
			}
		}

		public IEnumerable<Result> Conclusions
		{
			get
			{
				this.StartAnalysis();
				return this.AnalysisMembers.Where(this.ConclusionsFilter).SelectMany((AnalysisMember x) => x.GetResults());
			}
		}

		public IEnumerable<Result> Errors
		{
			get
			{
				return from x in this.Conclusions
				where !x.HasException && x.Source is Rule && ((RuleResult)x).Value
				where (from y in x.Source.Features.OfType<RuleTypeFeature>()
				where y.RuleType == RuleType.Error
				select y).Any<RuleTypeFeature>()
				select x;
			}
		}

		public IEnumerable<Result> Warnings
		{
			get
			{
				return from x in this.Conclusions
				where !x.HasException && x.Source is Rule && ((RuleResult)x).Value
				where (from y in x.Source.Features.OfType<RuleTypeFeature>()
				where y.RuleType == RuleType.Warning
				select y).Any<RuleTypeFeature>()
				select x;
			}
		}

		public IEnumerable<Result> Info
		{
			get
			{
				return from x in this.Conclusions
				where !x.HasException && x.Source is Rule && ((RuleResult)x).Value
				where (from y in x.Source.Features.OfType<RuleTypeFeature>()
				where y.RuleType == RuleType.Info
				select y).Any<RuleTypeFeature>()
				select x;
			}
		}

		public IEnumerable<Result> Exceptions
		{
			get
			{
				return from x in this.Conclusions
				where x.HasException
				select x;
			}
		}

		public IEnumerable<AnalysisMember> AnalysisMembers
		{
			get
			{
				return this.analysisMemberNames.Value.Keys;
			}
		}

		public IEnumerable<AnalysisMember> Settings
		{
			get
			{
				return from x in this.AnalysisMembers
				where x.GetType().IsGenericType && x.GetType().GetGenericTypeDefinition() == typeof(Setting<>)
				select x;
			}
		}

		public IEnumerable<Rule> Rules
		{
			get
			{
				return (from x in this.AnalysisMembers
				where x is Rule
				select x).Cast<Rule>();
			}
		}

		public Func<AnalysisMember, bool> StartFilter { get; private set; }

		public Func<AnalysisMember, bool> ConclusionsFilter { get; private set; }

		public ExDateTime StartTime
		{
			get
			{
				long ticks = Thread.VolatileRead(ref this.startTimeTicks);
				return new ExDateTime(ExTimeZone.UtcTimeZone, ticks);
			}
			private set
			{
				long utcTicks = value.UtcTicks;
				Interlocked.Exchange(ref this.startTimeTicks, utcTicks);
			}
		}

		public ExDateTime StopTime
		{
			get
			{
				long ticks = Thread.VolatileRead(ref this.stopTimeTicks);
				return new ExDateTime(ExTimeZone.UtcTimeZone, ticks);
			}
			private set
			{
				long utcTicks = value.UtcTicks;
				Interlocked.Exchange(ref this.stopTimeTicks, utcTicks);
			}
		}

		public void StartAnalysis()
		{
			if (this.isAnalysisStarted)
			{
				return;
			}
			this.isAnalysisStarted = true;
			this.StartTime = ExDateTime.Now;
			this.OnAnalysisStart();
			foreach (AnalysisMember analysisMember in from y in this.AnalysisMembers
			where y.RunAs != ConcurrencyType.Synchronous && this.StartFilter(y)
			select y)
			{
				analysisMember.Start();
			}
			Parallel.ForEach<AnalysisMember>(this.AnalysisMembers.Where(this.ConclusionsFilter).ToArray<AnalysisMember>(), delegate(AnalysisMember x)
			{
				x.Start();
			});
		}

		public string GetAnalysisMemberName(AnalysisMember analysisMemeber)
		{
			return this.analysisMemberNames.Value[analysisMemeber];
		}

		void IAnalysisAccessor.UpdateProgress(Rule completedRule)
		{
			if (completedRule == null)
			{
				throw new ArgumentNullException("completedRule");
			}
			if (completedRule.Analysis != this)
			{
				throw new AnalysisException(completedRule, Strings.UpdateProgressForWrongAnalysis);
			}
			if (!this.ConclusionsFilter(completedRule))
			{
				return;
			}
			lock (this.progressUpdateLock)
			{
				int completedRules = ++this.completedConclusionRules;
				int value = this.totalConclusionRules.Value;
				ProgressUpdateEventArgs progressUpdateEventArgs = new ProgressUpdateEventArgs(completedRules, value);
				EventHandler<ProgressUpdateEventArgs> progressUpdated = this.ProgressUpdated;
				if (progressUpdated != null)
				{
					progressUpdated(this, progressUpdateEventArgs);
				}
				if (progressUpdateEventArgs.CompletedPercentage == 100)
				{
					this.StopTime = ExDateTime.Now;
					this.OnAnalysisStop();
				}
			}
		}

		void IAnalysisAccessor.CallOnAnalysisMemberStart(AnalysisMember member)
		{
			if (member is RootAnalysisMember)
			{
				return;
			}
			this.OnAnalysisMemberStart(member);
		}

		void IAnalysisAccessor.CallOnAnalysisMemberStop(AnalysisMember member)
		{
			if (member is RootAnalysisMember)
			{
				return;
			}
			this.OnAnalysisMemberStop(member);
		}

		void IAnalysisAccessor.CallOnAnalysisMemberEvaluate(AnalysisMember member, Result result)
		{
			if (member is RootAnalysisMember)
			{
				return;
			}
			this.OnAnalysisMemberEvaluate(member, result);
		}

		protected virtual void OnAnalysisStart()
		{
		}

		protected virtual void OnAnalysisStop()
		{
		}

		protected virtual void OnAnalysisMemberStart(AnalysisMember member)
		{
		}

		protected virtual void OnAnalysisMemberStop(AnalysisMember member)
		{
		}

		protected virtual void OnAnalysisMemberEvaluate(AnalysisMember member, Result result)
		{
		}

		protected virtual void WriteConfiguration(XmlWriter writer)
		{
		}

		protected virtual void ReadConfiguration(XmlReader reader)
		{
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException("Read from XML is not supported.");
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("analysis");
			writer.WriteStartElement("configuration");
			this.WriteConfiguration(writer);
			writer.WriteEndElement();
			foreach (AnalysisMember analysisMember in this.AnalysisMembers.Where(this.ConclusionsFilter))
			{
				analysisMember.WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		private Dictionary<AnalysisMember, string> PopulateAnalysisMemberNames()
		{
			Dictionary<AnalysisMember, string> dictionary = new Dictionary<AnalysisMember, string>();
			IEnumerable<KeyValuePair<AnalysisMember, string>> enumerable = from x in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where typeof(AnalysisMember).IsAssignableFrom(x.PropertyType)
			let analysisMember = (AnalysisMember)x.GetValue(this, null)
			where analysisMember != null
			select new KeyValuePair<AnalysisMember, string>(analysisMember, x.Name);
			foreach (KeyValuePair<AnalysisMember, string> keyValuePair in enumerable)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}

		private Lazy<Dictionary<AnalysisMember, string>> analysisMemberNames;

		private int completedConclusionRules;

		private Lazy<int> totalConclusionRules;

		private bool isAnalysisStarted;

		private long startTimeTicks;

		private long stopTimeTicks;

		private object progressUpdateLock;
	}
}
