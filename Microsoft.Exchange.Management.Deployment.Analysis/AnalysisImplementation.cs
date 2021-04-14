using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class AnalysisImplementation<TDataSourceProvider, TMemberBuilder, TConclusionSetBuilder, TConclusionSet, TConclusion, TSettingConclusion, TRuleConclusion> : Analysis where TMemberBuilder : AnalysisMemberBuilder where TConclusionSetBuilder : ConclusionSetBuilderImplementation<TConclusionSet, TConclusion, TSettingConclusion, TRuleConclusion> where TConclusionSet : ConclusionSetImplementation<TConclusion, TSettingConclusion, TRuleConclusion> where TConclusion : ConclusionImplementation<TConclusion, TSettingConclusion, TRuleConclusion> where TSettingConclusion : TConclusion where TRuleConclusion : TConclusion, IRuleConclusion
	{
		protected AnalysisImplementation(TDataSourceProvider dataSourceProvider, TMemberBuilder memberBuilder, TConclusionSetBuilder conclusionSetBuilder, Func<AnalysisMember, bool> immediateEvaluationFilter, Func<AnalysisMember, bool> conclusionsFilter, AnalysisThreading threadMode) : base(immediateEvaluationFilter, conclusionsFilter, threadMode)
		{
			if (dataSourceProvider == null)
			{
				throw new ArgumentNullException("dataSourceProvider");
			}
			if (memberBuilder == null)
			{
				throw new ArgumentNullException("memberBuilder");
			}
			if (conclusionSetBuilder == null)
			{
				throw new ArgumentNullException("conclusionSetBuilder");
			}
			this.dataSourceProvider = dataSourceProvider;
			this.memberBuilder = memberBuilder;
			this.memberBuilder.SetAnalysis(this);
			this.conclusionSetBuilder = conclusionSetBuilder;
			this.conclusionSet = default(TConclusionSet);
			this.conclusionSetLock = new object();
		}

		public TDataSourceProvider DataSources
		{
			get
			{
				return this.dataSourceProvider;
			}
		}

		public TConclusionSet Conclusions
		{
			get
			{
				base.WaitUntilComplete();
				if (base.Status != AnalysisStatus.Completed)
				{
					throw new InvalidOperationException(Strings.AnalysisMustBeCompleteToCreateConclusionSet);
				}
				TConclusionSet result;
				lock (this.conclusionSetLock)
				{
					if (this.conclusionSet != null)
					{
						result = this.conclusionSet;
					}
					else
					{
						TConclusionSetBuilder tconclusionSetBuilder = this.conclusionSetBuilder;
						this.conclusionSet = tconclusionSetBuilder.Build(this);
						result = this.conclusionSet;
					}
				}
				return result;
			}
		}

		protected TMemberBuilder Build
		{
			get
			{
				return this.memberBuilder;
			}
		}

		private readonly TDataSourceProvider dataSourceProvider;

		private readonly TMemberBuilder memberBuilder;

		private readonly TConclusionSetBuilder conclusionSetBuilder;

		private readonly object conclusionSetLock;

		private TConclusionSet conclusionSet;
	}
}
