using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class AnalysisMember<T> : AnalysisMember
	{
		protected AnalysisMember(Analysis analysis, FeatureSet features) : base(analysis, features)
		{
			this.producer = null;
			this.results = new ResultsCache<T>();
			this.producerLock = new object();
		}

		public override Type ValueType
		{
			get
			{
				return typeof(T);
			}
		}

		public Results<T> Results
		{
			get
			{
				return new Results<T>(this, this.CreateConsumerEnumerable());
			}
		}

		public override IEnumerable<Result> UntypedResults
		{
			get
			{
				return this.Results;
			}
		}

		public override IEnumerable<Result> CachedResults
		{
			get
			{
				if (this.results.IsComplete)
				{
					return this.results;
				}
				IEnumerable<Result> result;
				lock (this.producerLock)
				{
					result = this.results;
				}
				return result;
			}
		}

		public Result<T> Result
		{
			get
			{
				if (this.Results.Skip(1).Any<Result<T>>())
				{
					throw new MultipleResultsException(this);
				}
				Result<T> result = this.Results.FirstOrDefault<Result<T>>();
				if (result == null)
				{
					throw new EmptyResultsException(this);
				}
				return result;
			}
		}

		public T Value
		{
			get
			{
				return this.Result.Value;
			}
		}

		public T ValueOrDefault
		{
			get
			{
				T result;
				try
				{
					result = this.Result.ValueOrDefault;
				}
				catch
				{
					result = default(T);
				}
				return result;
			}
		}

		public Results<T> RelativeResults(Result relativeTo)
		{
			HashSet<AnalysisMember> @object = new HashSet<AnalysisMember>(base.AncestorsAndSelf());
			AnalysisMember commonAncestor = relativeTo.Source.AncestorsAndSelf().First(new Func<AnalysisMember, bool>(@object.Contains));
			Result result = relativeTo.AncestorsAndSelf().First((Result x) => x.Source == commonAncestor);
			return result.DescendantsOfType<T>(this);
		}

		public override void Start()
		{
			IEnumerator enumerator = this.Results.GetEnumerator();
			while (enumerator.MoveNext())
			{
			}
		}

		private IEnumerator<Result<T>> CreateProducerEnumerator()
		{
			AnalysisMember parent = base.Features.GetFeature<ForEachResultFeature>().ForEachResultFunc();
			if (parent == null)
			{
				this.OnStart();
				Result<T> rootResult = base.Analysis.RootAnalysisMember.UntypedResults.Single<Result>() as Result<T>;
				this.results = this.results.Add(rootResult);
				this.OnEvaluate(rootResult);
				yield return rootResult;
				this.OnComplete();
			}
			else
			{
				Func<Result, IEnumerable<Result>> resultFunc = base.Features.GetFeature<ResultsFeature>().ResultFunc;
				Func<FeatureSet, Result, bool> func;
				if (!base.Features.HasFeature<FilterFeature>())
				{
					func = ((FeatureSet fs, Result r) => true);
				}
				else
				{
					func = base.Features.GetFeature<FilterFeature>().FilterFunc;
				}
				Func<FeatureSet, Result, bool> filterFunc = func;
				this.OnStart();
				foreach (Result parentResult in parent.UntypedResults)
				{
					if (!parentResult.HasException)
					{
						ExDateTime startTime = ExDateTime.Now;
						Stopwatch stopWatch = Stopwatch.StartNew();
						foreach (Result result in resultFunc(parentResult))
						{
							Result<T> producerResult = (Result<T>)result;
							if (base.IsAnalysisCanceled)
							{
								yield break;
							}
							stopWatch.Stop();
							ExDateTime stopTime = startTime + stopWatch.Elapsed;
							if (producerResult.HasException)
							{
								if (producerResult.Exception is AnalysisException)
								{
									AnalysisException ex = (AnalysisException)producerResult.Exception;
									ex.AnalysisMemberSource = this;
								}
								if (producerResult.Exception is CriticalException)
								{
									base.CancelAnalysis((CriticalException)producerResult.Exception);
								}
							}
							Result<T> filterResult = producerResult;
							try
							{
								if (!filterFunc(base.Features, producerResult))
								{
									filterResult = new Result<T>(new FilteredException(this, producerResult));
								}
							}
							catch (Exception inner)
							{
								base.CancelAnalysis(new CriticalException(this, inner));
							}
							Result<T> consumerResult;
							if (filterResult is RuleResult)
							{
								RuleResult toCopy = filterResult as RuleResult;
								RuleResult ruleResult = new RuleResult(toCopy, this, parentResult, startTime, stopTime);
								consumerResult = (ruleResult as Result<T>);
							}
							else
							{
								consumerResult = new Result<T>(filterResult, this, parentResult, startTime, stopTime);
							}
							this.results = this.results.Add(consumerResult);
							this.OnEvaluate(consumerResult);
							yield return consumerResult;
							startTime = stopTime;
							stopWatch.Restart();
						}
					}
				}
				this.results = this.results.AsCompleted();
				this.OnComplete();
			}
			yield break;
		}

		private IEnumerable<Result<T>> CreateConsumerEnumerable()
		{
			int nextResultIndex = 0;
			ResultsCache<T> unsafeResults = this.results;
			foreach (Result<T> result in unsafeResults.Skip(nextResultIndex))
			{
				yield return result;
			}
			nextResultIndex = unsafeResults.Count;
			if (!unsafeResults.IsComplete)
			{
				ResultsCache<T> currentResults;
				do
				{
					lock (this.producerLock)
					{
						if (nextResultIndex == this.results.Count)
						{
							if (this.producer == null)
							{
								this.producer = this.CreateProducerEnumerator();
							}
							try
							{
								if (!this.producer.MoveNext())
								{
									yield break;
								}
							}
							catch (Exception inner)
							{
								base.CancelAnalysis(new CriticalException(this, inner));
							}
						}
						currentResults = this.results;
					}
					foreach (Result<T> result2 in currentResults.Skip(nextResultIndex))
					{
						yield return result2;
					}
					nextResultIndex = currentResults.Count;
				}
				while (!currentResults.IsComplete);
			}
			yield break;
		}

		private readonly object producerLock;

		private IEnumerator<Result<T>> producer;

		private ResultsCache<T> results;
	}
}
