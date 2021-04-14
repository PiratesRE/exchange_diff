using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class AnalysisMember : Analysis.AnalysisMemberBase
	{
		public AnalysisMember(Analysis analysis, FeatureSet features) : base(analysis)
		{
			this.parent = features.GetFeature<ForEachResultFeature>().ForEachResultFunc;
			this.evaluationMode = features.GetFeature<EvaluationModeFeature>().EvaluationMode;
			this.features = features;
			this.currentState = new Optimistic<AnalysisMember.AnalysisMemberState>(new AnalysisMember.AnalysisMemberState(), new Resolver<AnalysisMember.AnalysisMemberState>(AnalysisMember.AnalysisMemberState.Resolve));
		}

		public AnalysisMember Parent
		{
			get
			{
				return this.parent();
			}
		}

		public Evaluate EvaluationMode
		{
			get
			{
				return this.evaluationMode;
			}
		}

		public bool IsConclusion
		{
			get
			{
				return base.AnalysisConclusionsFilter(this);
			}
		}

		public abstract Type ValueType { get; }

		public abstract IEnumerable<Result> UntypedResults { get; }

		public abstract IEnumerable<Result> CachedResults { get; }

		public ExDateTime StartTime
		{
			get
			{
				AnalysisMember.AnalysisMemberState unsafeValue = this.currentState.UnsafeValue;
				if (unsafeValue.HasStarted)
				{
					return unsafeValue.StartTime;
				}
				AnalysisMember.AnalysisMemberState safeValue = this.currentState.SafeValue;
				return safeValue.StartTime;
			}
		}

		public ExDateTime StopTime
		{
			get
			{
				AnalysisMember.AnalysisMemberState unsafeValue = this.currentState.UnsafeValue;
				if (unsafeValue.HasCompleted)
				{
					return unsafeValue.StopTime;
				}
				AnalysisMember.AnalysisMemberState safeValue = this.currentState.SafeValue;
				return safeValue.StopTime;
			}
		}

		public FeatureSet Features
		{
			get
			{
				return this.features;
			}
		}

		public IEnumerable<AnalysisMember> AncestorsAndSelf()
		{
			for (AnalysisMember current = this; current != null; current = current.Parent)
			{
				yield return current;
			}
			yield break;
		}

		public abstract void Start();

		protected override void OnStart()
		{
			AnalysisMember.AnalysisMemberState unsafeValue = this.currentState.UnsafeValue;
			this.currentState.Update(unsafeValue, unsafeValue.SetAsStarted());
			base.OnStart();
		}

		protected override void OnComplete()
		{
			AnalysisMember.AnalysisMemberState unsafeValue = this.currentState.UnsafeValue;
			this.currentState.Update(unsafeValue, unsafeValue.SetAsCompleted());
			base.OnComplete();
		}

		private readonly Optimistic<AnalysisMember.AnalysisMemberState> currentState;

		private readonly Func<AnalysisMember> parent;

		private readonly Evaluate evaluationMode;

		private readonly FeatureSet features;

		private sealed class AnalysisMemberState
		{
			public AnalysisMemberState()
			{
				this.hasStarted = false;
				this.hasCompleted = false;
				this.startTime = default(ExDateTime);
				this.stopTime = default(ExDateTime);
			}

			private AnalysisMemberState(bool hasStarted, bool hasCompleted, ExDateTime startTime, ExDateTime stopTime)
			{
				this.hasStarted = hasStarted;
				this.hasCompleted = hasCompleted;
				this.startTime = startTime;
				this.stopTime = stopTime;
			}

			public bool HasStarted
			{
				get
				{
					return this.hasStarted;
				}
			}

			public bool HasCompleted
			{
				get
				{
					return this.hasCompleted;
				}
			}

			public ExDateTime StartTime
			{
				get
				{
					if (!this.hasStarted)
					{
						throw new InvalidOperationException(Strings.CannotGetStartTimeBeforeMemberStart);
					}
					return this.startTime;
				}
			}

			public ExDateTime StopTime
			{
				get
				{
					if (!this.hasCompleted)
					{
						throw new InvalidOperationException(Strings.CannotGetStopTimeBeforeMemberCompletion);
					}
					return this.stopTime;
				}
			}

			public static AnalysisMember.AnalysisMemberState Resolve(AnalysisMember.AnalysisMemberState originalValue, AnalysisMember.AnalysisMemberState currentValue, AnalysisMember.AnalysisMemberState updatedValue)
			{
				return new AnalysisMember.AnalysisMemberState(currentValue.hasStarted || updatedValue.hasStarted, currentValue.hasCompleted || updatedValue.hasCompleted, (currentValue.startTime < updatedValue.startTime) ? currentValue.startTime : updatedValue.startTime, (currentValue.stopTime < updatedValue.stopTime) ? currentValue.stopTime : updatedValue.stopTime);
			}

			public AnalysisMember.AnalysisMemberState SetAsStarted()
			{
				if (this.hasStarted)
				{
					return this;
				}
				return this.With(true, default(Optional<bool>), ExDateTime.Now, default(Optional<ExDateTime>));
			}

			public AnalysisMember.AnalysisMemberState SetAsCompleted()
			{
				if (this.hasCompleted)
				{
					return this;
				}
				return this.With(default(Optional<bool>), true, default(Optional<ExDateTime>), ExDateTime.Now);
			}

			private AnalysisMember.AnalysisMemberState With(Optional<bool> hasStarted = default(Optional<bool>), Optional<bool> hasCompleted = default(Optional<bool>), Optional<ExDateTime> startTime = default(Optional<ExDateTime>), Optional<ExDateTime> stopTime = default(Optional<ExDateTime>))
			{
				return new AnalysisMember.AnalysisMemberState(hasStarted.DefaultTo(this.hasStarted), hasCompleted.DefaultTo(this.hasCompleted), startTime.DefaultTo(this.startTime), stopTime.DefaultTo(this.stopTime));
			}

			private readonly bool hasStarted;

			private readonly bool hasCompleted;

			private readonly ExDateTime startTime;

			private readonly ExDateTime stopTime;
		}
	}
}
