using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class AnalysisMemberBuilder : FeatureSet.Builder, IAnalysisDependantSetter
	{
		private protected Analysis Analysis { protected get; private set; }

		void IAnalysisDependantSetter.SetAnalysis(Analysis analysis)
		{
			this.Analysis = analysis;
		}

		public Setting<TResult> CopyOfSetting<TResult>(Func<Setting<TResult>> setting)
		{
			return this.CopyOfSetting<TResult, object>(setting, () => this.Analysis.RootAnalysisMember);
		}

		public Setting<TResult> CopyOfSetting<TResult, TParent>(Func<Setting<TResult>> setting, Func<AnalysisMember<TParent>> forEachResult)
		{
			return new Setting<TResult>(this.Analysis, base.BuildFeatureSet(() => setting().Features, Enumerable.Empty<Feature>(), new Feature[]
			{
				new ForEachResultFeature(forEachResult)
			}));
		}

		public Rule CopyOfRule(Func<Rule> rule)
		{
			return this.CopyOfRule<object>(rule, () => this.Analysis.RootAnalysisMember);
		}

		public Rule CopyOfRule<TParent>(Func<Rule> rule, Func<AnalysisMember<TParent>> forEachResult)
		{
			return new Rule(this.Analysis, base.BuildFeatureSet(() => rule().Features, Enumerable.Empty<Feature>(), new Feature[]
			{
				new ForEachResultFeature(forEachResult)
			}));
		}

		protected Setting<TResult> Setting<TResult>(Func<TResult> setValue, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(() => this.Analysis.RootAnalysisMember, delegate(Result x)
			{
				Result<TResult> result;
				try
				{
					result = new Result<TResult>(setValue());
				}
				catch (Exception exception)
				{
					result = new Result<TResult>(exception);
				}
				return new Result[]
				{
					result
				};
			}, evaluate, features);
		}

		protected Setting<TResult> Setting<TResult>(Func<IEnumerable<TResult>> setValues, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(() => this.Analysis.RootAnalysisMember, delegate(Result x)
			{
				IEnumerable<Result> result;
				try
				{
					result = from y in setValues()
					select new Result<TResult>(y);
				}
				catch (Exception exception)
				{
					result = new Result[]
					{
						new Result<TResult>(exception)
					};
				}
				return result;
			}, evaluate, features);
		}

		protected Setting<TResult> Setting<TResult>(Func<IEnumerable<Result<TResult>>> setResults, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(() => this.Analysis.RootAnalysisMember, delegate(Result x)
			{
				IEnumerable<Result> enumerable;
				try
				{
					enumerable = setResults();
					if (enumerable.Any((Result y) => y == null))
					{
						throw new AnalysisException(Strings.NullResult);
					}
				}
				catch (Exception exception)
				{
					enumerable = new Result[]
					{
						new Result<TResult>(exception)
					};
				}
				return enumerable;
			}, evaluate, features);
		}

		protected Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, TResult> setValue, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(forEachResult, delegate(Result x)
			{
				Result<TResult> result;
				try
				{
					result = new Result<TResult>(setValue((Result<TParent>)x));
				}
				catch (Exception exception)
				{
					result = new Result<TResult>(exception);
				}
				return new Result[]
				{
					result
				};
			}, evaluate, features);
		}

		protected Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, IEnumerable<TResult>> setValues, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(forEachResult, delegate(Result x)
			{
				IEnumerable<Result> enumerable;
				try
				{
					enumerable = from y in setValues((Result<TParent>)x)
					select new Result<TResult>(y);
					if (enumerable.Any((Result y) => y == null))
					{
						throw new AnalysisException(Strings.NullResult);
					}
				}
				catch (Exception exception)
				{
					enumerable = new Result[]
					{
						new Result<TResult>(exception)
					};
				}
				return enumerable;
			}, evaluate, features);
		}

		protected Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, IEnumerable<Result<TResult>>> setResults, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildSetting<TResult>(forEachResult, delegate(Result x)
			{
				IEnumerable<Result> enumerable;
				try
				{
					enumerable = setResults((Result<TParent>)x);
					if (enumerable.Any((Result y) => y == null))
					{
						throw new AnalysisException(Strings.NullResult);
					}
				}
				catch (Exception exception)
				{
					enumerable = new Result[]
					{
						new Result<TResult>(exception)
					};
				}
				return enumerable;
			}, evaluate, features);
		}

		protected Rule Rule(Func<bool> condition, Evaluate evaluate, Severity severity, params Feature[] features)
		{
			return this.BuildRule(() => this.Analysis.RootAnalysisMember, delegate(Result x)
			{
				RuleResult ruleResult;
				try
				{
					ruleResult = new RuleResult(condition());
				}
				catch (Exception exception)
				{
					ruleResult = new RuleResult(exception);
				}
				return new Result[]
				{
					ruleResult
				};
			}, evaluate, severity, features);
		}

		protected Rule Rule(Func<Tuple<bool, Severity>> condition, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildRule(() => this.Analysis.RootAnalysisMember, delegate(Result x)
			{
				RuleResult ruleResult;
				try
				{
					Tuple<bool, Severity> tuple = condition();
					ruleResult = new RuleResult(tuple.Item1)
					{
						Severity = new Severity?(tuple.Item2)
					};
				}
				catch (Exception exception)
				{
					ruleResult = new RuleResult(exception);
				}
				return new Result[]
				{
					ruleResult
				};
			}, evaluate, Severity.Info, features);
		}

		protected Rule Rule<TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, bool> condition, Evaluate evaluate, Severity severity, params Feature[] features)
		{
			return this.BuildRule(forEachResult, delegate(Result x)
			{
				RuleResult ruleResult;
				try
				{
					ruleResult = new RuleResult(condition((Result<TParent>)x));
				}
				catch (Exception exception)
				{
					ruleResult = new RuleResult(exception);
				}
				return new Result[]
				{
					ruleResult
				};
			}, evaluate, severity, features);
		}

		protected Rule Rule<TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, Tuple<bool, Severity>> condition, Evaluate evaluate, params Feature[] features)
		{
			return this.BuildRule(forEachResult, delegate(Result x)
			{
				RuleResult ruleResult;
				try
				{
					Tuple<bool, Severity> tuple = condition((Result<TParent>)x);
					ruleResult = new RuleResult(tuple.Item1)
					{
						Severity = new Severity?(tuple.Item2)
					};
				}
				catch (Exception exception)
				{
					ruleResult = new RuleResult(exception);
				}
				return new Result[]
				{
					ruleResult
				};
			}, evaluate, Severity.Info, features);
		}

		private Setting<TResult> BuildSetting<TResult>(Func<AnalysisMember> forEachResultFunc, Func<Result, IEnumerable<Result>> resultsFunc, Evaluate evaluate, IEnumerable<Feature> features)
		{
			if (forEachResultFunc == null)
			{
				throw new ArgumentNullException("forEachResultFunc");
			}
			if (resultsFunc == null)
			{
				throw new ArgumentNullException("resultsFunc");
			}
			if (features == null)
			{
				throw new ArgumentNullException("features");
			}
			return new Setting<TResult>(this.Analysis, base.BuildFeatureSet(features, new Feature[]
			{
				new EvaluationModeFeature(evaluate),
				new ForEachResultFeature(forEachResultFunc),
				new ResultsFeature(resultsFunc)
			}));
		}

		private Rule BuildRule(Func<AnalysisMember> forEachResultFunc, Func<Result, IEnumerable<Result>> resultsFunc, Evaluate evaluate, Severity severity, IEnumerable<Feature> features)
		{
			if (forEachResultFunc == null)
			{
				throw new ArgumentNullException("forEachResultFunc");
			}
			if (resultsFunc == null)
			{
				throw new ArgumentNullException("resultsFunc");
			}
			if (features == null)
			{
				throw new ArgumentNullException("features");
			}
			return new Rule(this.Analysis, base.BuildFeatureSet(features, new Feature[]
			{
				new EvaluationModeFeature(evaluate),
				new SeverityFeature(severity),
				new ForEachResultFeature(forEachResultFunc),
				new ResultsFeature(resultsFunc)
			}));
		}
	}
}
