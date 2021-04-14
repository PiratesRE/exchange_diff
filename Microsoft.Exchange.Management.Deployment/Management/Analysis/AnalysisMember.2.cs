using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal abstract class AnalysisMember<T> : AnalysisMember
	{
		public AnalysisMember(Func<AnalysisMember> parent, ConcurrencyType runAs, Analysis analysis, IEnumerable<Feature> features, Func<Result, IEnumerable<Result<T>>> setFunction) : base(parent, runAs, analysis, features)
		{
			this.setFunction = setFunction;
			this.values = new List<Result<T>>();
			this.producerLock = new object();
			this.rwls = new ReaderWriterLockSlim();
			this.parentResultEnumerator = new Lazy<IEnumerator<Result>>(() => base.Parent.GetResults().GetEnumerator(), true);
			this.producerEnumerator = Enumerable.Empty<Result<T>>().GetEnumerator();
			this.task = null;
			this.taskLock = new object();
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
				return new Results<T>(this, new AnalysisMember<T>.ConsumerEnumerable(this));
			}
		}

		public Results<T> RelativeResults(Result relativeTo)
		{
			HashSet<AnalysisMember> ancestors = new HashSet<AnalysisMember>(base.AncestorsAndSelf());
			AnalysisMember commonAncestor = (from x in relativeTo.Source.AncestorsAndSelf()
			where ancestors.Contains(x)
			select x).First<AnalysisMember>();
			Result result = (from x in relativeTo.AncestorsAndSelf()
			where x.Source == commonAncestor
			select x).First<Result>();
			return result.DescendantsOfType<T>(this);
		}

		public override void Start()
		{
			if (base.RunAs == ConcurrencyType.Synchronous)
			{
				while (this.ProduceNextResult())
				{
				}
				return;
			}
			if (this.task != null)
			{
				return;
			}
			lock (this.taskLock)
			{
				if (this.task == null)
				{
					this.task = new Task(delegate()
					{
						while (this.ProduceNextResult())
						{
						}
					}, TaskCreationOptions.LongRunning);
					this.task.Start();
				}
			}
		}

		public override IEnumerable<Result> GetResults()
		{
			return this.Results;
		}

		private bool ProduceNextResult()
		{
			bool result;
			lock (this.producerLock)
			{
				if (base.StartTime == default(ExDateTime))
				{
					base.StartTime = ExDateTime.Now;
					this.OnStart();
				}
				ExDateTime now = ExDateTime.Now;
				if (this.producerEnumerator.MoveNext())
				{
					this.ProcessResult(now);
					result = true;
				}
				else
				{
					while (this.parentResultEnumerator.Value.MoveNext())
					{
						if (!this.parentResultEnumerator.Value.Current.HasException)
						{
							this.producerEnumerator = this.setFunction(this.parentResultEnumerator.Value.Current).GetEnumerator();
							if (this.producerEnumerator.MoveNext())
							{
								this.ProcessResult(now);
								return true;
							}
						}
					}
					if (base.StopTime == default(ExDateTime))
					{
						base.StopTime = ExDateTime.Now;
						this.OnStop();
						if (this is Rule)
						{
							((IAnalysisAccessor)base.Analysis).UpdateProgress(this as Rule);
						}
					}
					result = false;
				}
			}
			return result;
		}

		private void ProcessResult(ExDateTime evaluationStartTime)
		{
			this.rwls.EnterWriteLock();
			Result<T> result = null;
			try
			{
				result = this.producerEnumerator.Current;
			}
			catch (Exception exception)
			{
				result = new Result<T>(exception);
			}
			finally
			{
				if (result == null)
				{
					result = new Result<T>(new AnalysisException(this, Strings.CannotReturnNullForResult));
				}
				if (result.HasException && result.Exception is FailureException)
				{
					((FailureException)result.Exception).AnalysisMemberSource = this;
				}
				((IResultAccessor)result).SetParent(this.parentResultEnumerator.Value.Current);
				((IResultAccessor)result).SetSource(this);
				((IResultAccessor)result).SetStartTime(evaluationStartTime);
				((IResultAccessor)result).SetStopTime(ExDateTime.Now);
				this.values.Add(result);
				this.rwls.ExitWriteLock();
			}
			this.OnEvaluate(result);
		}

		private void OnStart()
		{
			((IAnalysisAccessor)base.Analysis).CallOnAnalysisMemberStart(this);
		}

		private void OnStop()
		{
			((IAnalysisAccessor)base.Analysis).CallOnAnalysisMemberStop(this);
		}

		private void OnEvaluate(Result result)
		{
			((IAnalysisAccessor)base.Analysis).CallOnAnalysisMemberEvaluate(this, result);
		}

		private readonly Func<Result, IEnumerable<Result<T>>> setFunction;

		private readonly List<Result<T>> values;

		private readonly object producerLock;

		private readonly ReaderWriterLockSlim rwls;

		private readonly Lazy<IEnumerator<Result>> parentResultEnumerator;

		private readonly object taskLock;

		private IEnumerator<Result<T>> producerEnumerator;

		private Task task;

		private class ConsumerEnumerable : IEnumerable<Result<T>>, IEnumerable
		{
			public ConsumerEnumerable(AnalysisMember<T> owner)
			{
				this.owner = owner;
			}

			public IEnumerator<Result<T>> GetEnumerator()
			{
				return new AnalysisMember<T>.ConsumerEnumerator(this.owner);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private AnalysisMember<T> owner;
		}

		private class ConsumerEnumerator : IEnumerator<Result<T>>, IDisposable, IEnumerator
		{
			public ConsumerEnumerator(AnalysisMember<T> owner)
			{
				this.owner = owner;
				this.currentIndex = -1;
				this.hasValue = false;
			}

			public Result<T> Current
			{
				get
				{
					if (!this.hasValue)
					{
						throw new InvalidOperationException("Current property called before MoveNext().");
					}
					return this.currentValue;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				this.currentIndex++;
				this.owner.rwls.EnterReadLock();
				try
				{
					if (this.currentIndex < this.owner.values.Count)
					{
						this.currentValue = this.owner.values[this.currentIndex];
						this.hasValue = true;
						return true;
					}
				}
				finally
				{
					this.owner.rwls.ExitReadLock();
				}
				return this.GetNextValueFromSource();
			}

			public void Reset()
			{
				this.currentIndex = -1;
				this.hasValue = false;
			}

			public void Dispose()
			{
			}

			private bool GetNextValueFromSource()
			{
				this.owner.ProduceNextResult();
				this.owner.rwls.EnterReadLock();
				try
				{
					if (this.currentIndex < this.owner.values.Count)
					{
						this.currentValue = this.owner.values[this.currentIndex];
						this.hasValue = true;
						return true;
					}
				}
				finally
				{
					this.owner.rwls.ExitReadLock();
				}
				this.hasValue = false;
				return false;
			}

			private AnalysisMember<T> owner;

			private int currentIndex;

			private Result<T> currentValue;

			private bool hasValue;
		}
	}
}
