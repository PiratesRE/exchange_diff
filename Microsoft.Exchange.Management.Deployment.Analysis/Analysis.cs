using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class Analysis
	{
		protected Analysis(Func<AnalysisMember, bool> immediateEvaluationFilter, Func<AnalysisMember, bool> conclusionsFilter, AnalysisThreading threadMode)
		{
			if (immediateEvaluationFilter == null)
			{
				throw new ArgumentNullException("immediateEvaluationFilter");
			}
			if (conclusionsFilter == null)
			{
				throw new ArgumentNullException("conclusionsFilter");
			}
			this.immediateEvaluationFilter = immediateEvaluationFilter;
			this.conclusionsFilter = conclusionsFilter;
			this.currentState = new Optimistic<Analysis.AnalysisState>(new Analysis.AnalysisState(this), new Resolver<Analysis.AnalysisState>(Analysis.AnalysisState.Resolve));
			this.currentProgress = new Optimistic<AnalysisProgress>(new AnalysisProgress(0, 0), new Resolver<AnalysisProgress>(AnalysisProgress.Resolve));
			this.threadMode = threadMode;
			this.rootAnalysisMember = new RootAnalysisMember(this);
			this.completedManualResetEvent = new ManualResetEvent(false);
			this.startAnalysisLock = new object();
		}

		public event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;

		public AnalysisStatus Status
		{
			get
			{
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				if (safeValue.IsCanceled)
				{
					return AnalysisStatus.Canceled;
				}
				if (safeValue.IsCompleted)
				{
					return AnalysisStatus.Completed;
				}
				if (safeValue.IsStarted)
				{
					return AnalysisStatus.Running;
				}
				return AnalysisStatus.Ready;
			}
		}

		public AnalysisProgress Progress
		{
			get
			{
				return this.currentProgress.SafeValue;
			}
		}

		public AnalysisThreading ThreadMode
		{
			get
			{
				return this.threadMode;
			}
		}

		public RootAnalysisMember RootAnalysisMember
		{
			get
			{
				return this.rootAnalysisMember;
			}
		}

		public IEnumerable<AnalysisMember> AnalysisMembers
		{
			get
			{
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				Analysis.AnalysisState analysisState = this.currentState.Update(safeValue, safeValue.DiscoverMembers());
				return analysisState.AnalysisMembers;
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

		public ExDateTime StartTime
		{
			get
			{
				Analysis.AnalysisState unsafeValue = this.currentState.UnsafeValue;
				if (unsafeValue.IsStarted)
				{
					return unsafeValue.StartTime;
				}
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				return safeValue.StartTime;
			}
		}

		public ExDateTime StopTime
		{
			get
			{
				Analysis.AnalysisState unsafeValue = this.currentState.UnsafeValue;
				if (unsafeValue.IsCompleted)
				{
					return unsafeValue.StopTime;
				}
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				return safeValue.StopTime;
			}
		}

		public Exception CancellationException
		{
			get
			{
				Analysis.AnalysisState unsafeValue = this.currentState.UnsafeValue;
				if (unsafeValue.IsCanceled)
				{
					return unsafeValue.CancellationException;
				}
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				return safeValue.CancellationException;
			}
		}

		public void StartAnalysis()
		{
			Analysis.AnalysisState unsafeValue = this.currentState.UnsafeValue;
			if (unsafeValue.IsStarted || unsafeValue.IsCanceled)
			{
				return;
			}
			lock (this.startAnalysisLock)
			{
				Analysis.AnalysisState safeValue = this.currentState.SafeValue;
				if (safeValue.IsStarted || unsafeValue.IsCanceled)
				{
					return;
				}
				Analysis.AnalysisState updatedValue = safeValue.SetAsStarted();
				this.currentState.Update(safeValue, updatedValue);
			}
			AnalysisProgress progress = this.currentProgress.Update(this.currentProgress.UnsafeValue, new AnalysisProgress((from x in this.AnalysisMembers
			where !(x is RootAnalysisMember)
			select x).Count((AnalysisMember x) => x.IsConclusion), 0));
			try
			{
				this.OnAnalysisStart();
			}
			catch (Exception inner)
			{
				this.Cancel(new CriticalException(null, inner));
			}
			EventHandler<ProgressUpdateEventArgs> progressUpdated = this.ProgressUpdated;
			if (progressUpdated != null)
			{
				progressUpdated(this, new ProgressUpdateEventArgs(progress));
			}
			List<AnalysisMember> source = (from x in this.AnalysisMembers
			where x.IsConclusion
			select x).ToList<AnalysisMember>();
			if (source.Any<AnalysisMember>())
			{
				if (this.threadMode == AnalysisThreading.Single)
				{
					using (IEnumerator<AnalysisMember> enumerator = (from x in source
					orderby (int)x.EvaluationMode
					select x).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AnalysisMember analysisMember = enumerator.Current;
							analysisMember.Start();
						}
						return;
					}
				}
				using (IEnumerator<AnalysisMember> enumerator2 = (from x in this.AnalysisMembers
				where x.EvaluationMode == Evaluate.OnAnalysisStart && this.immediateEvaluationFilter(x)
				select x).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						AnalysisMember member = enumerator2.Current;
						Task.Factory.StartNew(delegate()
						{
							member.Start();
						}, TaskCreationOptions.LongRunning);
					}
				}
				Parallel.ForEach<AnalysisMember>((from x in this.AnalysisMembers
				where x.IsConclusion && (x.EvaluationMode != Evaluate.OnAnalysisStart || !this.immediateEvaluationFilter(x))
				select x).ToList<AnalysisMember>(), delegate(AnalysisMember x)
				{
					x.Start();
				});
				return;
			}
			this.CompleteAnalysis();
		}

		public void WaitUntilComplete()
		{
			this.StartAnalysis();
			this.completedManualResetEvent.WaitOne();
			Analysis.AnalysisState safeValue = this.currentState.SafeValue;
			if (safeValue.IsCanceled)
			{
				throw safeValue.CancellationException;
			}
		}

		public void WaitUntilComplete(TimeSpan timeout)
		{
			this.StartAnalysis();
			this.completedManualResetEvent.WaitOne(timeout);
		}

		public void Cancel()
		{
			this.CancelAnalysis(new CanceledException());
		}

		public void Cancel(string reason)
		{
			this.CancelAnalysis(new CanceledException(reason));
		}

		protected void Cancel(CriticalException exception)
		{
			this.CancelAnalysis(exception);
		}

		protected abstract void OnAnalysisStart();

		protected abstract void OnAnalysisStop();

		protected abstract void OnAnalysisMemberStart(AnalysisMember member);

		protected abstract void OnAnalysisMemberStop(AnalysisMember member);

		protected abstract void OnAnalysisMemberEvaluate(AnalysisMember member, Result result);

		private void CancelAnalysis(Exception exception)
		{
			Analysis.AnalysisState unsafeValue = this.currentState.UnsafeValue;
			if (unsafeValue.IsCanceled)
			{
				return;
			}
			this.currentState.Update(unsafeValue, unsafeValue.SetAsCancled(exception));
			this.completedManualResetEvent.Set();
		}

		private void CompleteAnalysis()
		{
			Analysis.AnalysisState safeValue = this.currentState.SafeValue;
			AnalysisProgress safeValue2 = this.currentProgress.SafeValue;
			if (!safeValue.IsStarted || safeValue2.CompletedConclusions != safeValue2.TotalConclusions)
			{
				return;
			}
			Analysis.AnalysisState updatedValue = safeValue.SetAsCompleted();
			this.currentState.Update(safeValue, updatedValue);
			try
			{
				this.OnAnalysisStop();
			}
			catch (Exception inner)
			{
				this.Cancel(new CriticalException(null, inner));
			}
			this.completedManualResetEvent.Set();
		}

		private readonly Optimistic<Analysis.AnalysisState> currentState;

		private readonly Optimistic<AnalysisProgress> currentProgress;

		private readonly Func<AnalysisMember, bool> immediateEvaluationFilter;

		private readonly Func<AnalysisMember, bool> conclusionsFilter;

		private readonly AnalysisThreading threadMode;

		private readonly RootAnalysisMember rootAnalysisMember;

		private readonly ManualResetEvent completedManualResetEvent;

		private readonly object startAnalysisLock;

		public abstract class AnalysisMemberBase
		{
			protected AnalysisMemberBase(Analysis analysis)
			{
				this.analysis = analysis;
			}

			public Analysis Analysis
			{
				get
				{
					return this.analysis;
				}
			}

			public string Name
			{
				get
				{
					Analysis.AnalysisState unsafeValue = this.analysis.currentState.UnsafeValue;
					if (unsafeValue.HasDiscoveredAnalysisMembers)
					{
						return unsafeValue.GetName(this);
					}
					Analysis.AnalysisState safeValue = this.analysis.currentState.SafeValue;
					if (safeValue.HasDiscoveredAnalysisMembers)
					{
						return safeValue.GetName(this);
					}
					Analysis.AnalysisState analysisState = this.analysis.currentState.Update(safeValue, safeValue.DiscoverMembers());
					return analysisState.GetName(this);
				}
			}

			public bool IsAnalysisCanceled
			{
				get
				{
					return this.analysis.currentState.SafeValue.IsCanceled;
				}
			}

			protected Func<AnalysisMember, bool> AnalysisConclusionsFilter
			{
				get
				{
					return this.analysis.conclusionsFilter;
				}
			}

			protected void CancelAnalysis(CriticalException exception)
			{
				this.analysis.Cancel(exception);
			}

			protected virtual void OnStart()
			{
				AnalysisMember analysisMember = this as AnalysisMember;
				if (analysisMember == null || analysisMember is RootAnalysisMember)
				{
					return;
				}
				try
				{
					this.analysis.OnAnalysisMemberStart(analysisMember);
				}
				catch (Exception inner)
				{
					this.analysis.Cancel(new CriticalException(analysisMember, inner));
				}
			}

			protected virtual void OnEvaluate(Result result)
			{
				AnalysisMember analysisMember = this as AnalysisMember;
				if (analysisMember == null || analysisMember is RootAnalysisMember)
				{
					return;
				}
				try
				{
					this.analysis.OnAnalysisMemberEvaluate(analysisMember, result);
				}
				catch (Exception inner)
				{
					this.analysis.Cancel(new CriticalException(analysisMember, inner));
				}
			}

			protected virtual void OnComplete()
			{
				AnalysisMember analysisMember = this as AnalysisMember;
				if (analysisMember == null || analysisMember is RootAnalysisMember)
				{
					return;
				}
				try
				{
					this.analysis.OnAnalysisMemberStop(analysisMember);
				}
				catch (Exception inner)
				{
					this.analysis.Cancel(new CriticalException(analysisMember, inner));
				}
				bool isAnalysisComplete = false;
				if (analysisMember.IsConclusion)
				{
					AnalysisProgress unsafeValue = this.analysis.currentProgress.UnsafeValue;
					AnalysisProgress updatedProgress = new AnalysisProgress(unsafeValue.TotalConclusions, unsafeValue.CompletedConclusions + 1);
					isAnalysisComplete = (updatedProgress.CompletedConclusions == updatedProgress.TotalConclusions);
					this.analysis.currentProgress.Update(unsafeValue, updatedProgress, delegate(AnalysisProgress original, AnalysisProgress current, AnalysisProgress updated)
					{
						isAnalysisComplete = (updatedProgress.CompletedConclusions == updatedProgress.TotalConclusions);
						return new AnalysisProgress(current.TotalConclusions, current.CompletedConclusions + 1);
					});
					EventHandler<ProgressUpdateEventArgs> progressUpdated = this.analysis.ProgressUpdated;
					if (progressUpdated != null)
					{
						try
						{
							progressUpdated(this.analysis, new ProgressUpdateEventArgs(updatedProgress));
						}
						catch (Exception inner2)
						{
							this.analysis.Cancel(new CriticalException(analysisMember, inner2));
						}
					}
				}
				if (isAnalysisComplete)
				{
					this.analysis.CompleteAnalysis();
				}
			}

			private readonly Analysis analysis;
		}

		private sealed class AnalysisState
		{
			public AnalysisState(Analysis analysis)
			{
				this.analysis = analysis;
				this.hasDiscoveredAnalysisMembers = false;
				this.isStarted = false;
				this.isCompleted = false;
				this.isCanceled = false;
				this.analysisMemberNameMap = null;
				this.startTime = default(ExDateTime);
				this.stopTime = default(ExDateTime);
				this.cancellationException = null;
			}

			private AnalysisState(Analysis analysis, bool hasDiscoveredAnalysisMembers, bool isStarted, bool isCompleted, bool isCanceled, Dictionary<Analysis.AnalysisMemberBase, string> analysisMemberNameMap, ExDateTime startTime, ExDateTime stopTime, Exception cancellationException)
			{
				this.analysis = analysis;
				this.hasDiscoveredAnalysisMembers = hasDiscoveredAnalysisMembers;
				this.isStarted = isStarted;
				this.isCompleted = isCompleted;
				this.isCanceled = isCanceled;
				this.analysisMemberNameMap = analysisMemberNameMap;
				this.startTime = startTime;
				this.stopTime = stopTime;
				this.cancellationException = cancellationException;
			}

			public bool HasDiscoveredAnalysisMembers
			{
				get
				{
					return this.hasDiscoveredAnalysisMembers;
				}
			}

			public bool IsStarted
			{
				get
				{
					return this.isStarted;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.isCompleted;
				}
			}

			public bool IsCanceled
			{
				get
				{
					return this.isCanceled;
				}
			}

			public IEnumerable<AnalysisMember> AnalysisMembers
			{
				get
				{
					if (!this.hasDiscoveredAnalysisMembers)
					{
						throw new InvalidOperationException(Strings.CannotGetMembersBeforeDiscovery);
					}
					return this.analysisMemberNameMap.Keys.Cast<AnalysisMember>();
				}
			}

			public ExDateTime StartTime
			{
				get
				{
					if (!this.isStarted)
					{
						throw new InvalidOperationException(Strings.CannotGetStartTimeBeforeStart);
					}
					return this.startTime;
				}
			}

			public ExDateTime StopTime
			{
				get
				{
					if (!this.isCompleted)
					{
						throw new InvalidOperationException(Strings.CannotGetStopTimeBeforeCompletion);
					}
					return this.stopTime;
				}
			}

			public Exception CancellationException
			{
				get
				{
					if (!this.IsCanceled)
					{
						throw new InvalidOperationException(Strings.CannotGetCancellationExceptionWithoutCancellation);
					}
					return this.cancellationException;
				}
			}

			public static Analysis.AnalysisState Resolve(Analysis.AnalysisState originalValue, Analysis.AnalysisState currentValue, Analysis.AnalysisState updatedValue)
			{
				return new Analysis.AnalysisState(updatedValue.analysis, currentValue.hasDiscoveredAnalysisMembers || updatedValue.hasDiscoveredAnalysisMembers, currentValue.isStarted || updatedValue.isStarted, currentValue.isCompleted || updatedValue.isCompleted, currentValue.isCanceled || updatedValue.isCanceled, currentValue.analysisMemberNameMap ?? updatedValue.analysisMemberNameMap, (currentValue.startTime < updatedValue.startTime) ? currentValue.startTime : updatedValue.startTime, (currentValue.stopTime < updatedValue.stopTime) ? currentValue.stopTime : updatedValue.stopTime, currentValue.cancellationException ?? updatedValue.cancellationException);
			}

			public Analysis.AnalysisState SetAsStarted()
			{
				if (this.isStarted)
				{
					return this;
				}
				return this.With(default(Optional<bool>), true, default(Optional<bool>), default(Optional<bool>), default(Optional<Dictionary<Analysis.AnalysisMemberBase, string>>), ExDateTime.Now, default(Optional<ExDateTime>), default(Optional<Exception>));
			}

			public Analysis.AnalysisState SetAsCompleted()
			{
				if (this.isCompleted)
				{
					return this;
				}
				return this.With(default(Optional<bool>), default(Optional<bool>), true, default(Optional<bool>), default(Optional<Dictionary<Analysis.AnalysisMemberBase, string>>), default(Optional<ExDateTime>), ExDateTime.Now, default(Optional<Exception>));
			}

			public Analysis.AnalysisState SetAsCancled(Exception cancellationException)
			{
				if (this.isCanceled)
				{
					return this;
				}
				return this.With(default(Optional<bool>), default(Optional<bool>), default(Optional<bool>), true, default(Optional<Dictionary<Analysis.AnalysisMemberBase, string>>), default(Optional<ExDateTime>), default(Optional<ExDateTime>), cancellationException);
			}

			public string GetName(Analysis.AnalysisMemberBase analysisMember)
			{
				if (!this.hasDiscoveredAnalysisMembers)
				{
					throw new InvalidOperationException(Strings.CannotGetMemberNameBeforeDiscovery);
				}
				return this.analysisMemberNameMap[analysisMember];
			}

			public Analysis.AnalysisState DiscoverMembers()
			{
				Dictionary<Analysis.AnalysisMemberBase, string> value = (from x in this.analysis.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where typeof(AnalysisMember).IsAssignableFrom(x.PropertyType)
				let analysisMember = (Analysis.AnalysisMemberBase)x.GetValue(this.analysis, null)
				where analysisMember != null
				select new
				{
					Member = analysisMember,
					Name = x.Name
				}).ToDictionary(y => y.Member, y => y.Name);
				return this.With(true, default(Optional<bool>), default(Optional<bool>), default(Optional<bool>), value, default(Optional<ExDateTime>), default(Optional<ExDateTime>), default(Optional<Exception>));
			}

			private Analysis.AnalysisState With(Optional<bool> hasDiscoveredAnalysisMembers = default(Optional<bool>), Optional<bool> isStarted = default(Optional<bool>), Optional<bool> isCompleted = default(Optional<bool>), Optional<bool> isCanceled = default(Optional<bool>), Optional<Dictionary<Analysis.AnalysisMemberBase, string>> analysisMemberNameMap = default(Optional<Dictionary<Analysis.AnalysisMemberBase, string>>), Optional<ExDateTime> startTime = default(Optional<ExDateTime>), Optional<ExDateTime> stopTime = default(Optional<ExDateTime>), Optional<Exception> cancellationException = default(Optional<Exception>))
			{
				return new Analysis.AnalysisState(this.analysis, hasDiscoveredAnalysisMembers.DefaultTo(this.hasDiscoveredAnalysisMembers), isStarted.DefaultTo(this.isStarted), isCompleted.DefaultTo(this.isCompleted), isCanceled.DefaultTo(this.isCanceled), analysisMemberNameMap.DefaultTo(this.analysisMemberNameMap), startTime.DefaultTo(this.startTime), stopTime.DefaultTo(this.stopTime), cancellationException.DefaultTo(this.cancellationException));
			}

			private readonly Analysis analysis;

			private readonly bool hasDiscoveredAnalysisMembers;

			private readonly bool isStarted;

			private readonly bool isCompleted;

			private readonly bool isCanceled;

			private readonly Dictionary<Analysis.AnalysisMemberBase, string> analysisMemberNameMap;

			private readonly ExDateTime startTime;

			private readonly ExDateTime stopTime;

			private readonly Exception cancellationException;
		}
	}
}
