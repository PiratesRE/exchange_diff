using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;

namespace Microsoft.Exchange.Search.Query
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class QueryStatistics
	{
		internal QueryStatistics(int traceContext)
		{
			this.traceContext = traceContext;
			this.executionStep = this.StartNewStep(QueryExecutionStepType.InstantSearchRequest);
		}

		public int Version
		{
			get
			{
				return 1;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.executionStep.StartTime;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.executionStep.EndTime;
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return this.executionStep.ElapsedMilliseconds;
			}
		}

		public bool StoreBypassed { get; internal set; }

		public bool LightningEnabled { get; internal set; }

		public IReadOnlyCollection<QueryExecutionStep> Steps
		{
			get
			{
				return this.steps;
			}
		}

		public void Complete()
		{
			this.executionStep.Complete(this.stopwatch);
			ExTraceGlobals.InstantSearchTracer.TraceDebug<QueryExecutionStepType, long>((long)this.traceContext, "Query step completed, {0} {1}ms.", this.executionStep.StepType, this.executionStep.ElapsedMilliseconds);
		}

		public QueryExecutionStep StartNewStep(QueryExecutionStepType stepType)
		{
			ExTraceGlobals.InstantSearchTracer.TraceDebug<QueryExecutionStepType>((long)this.traceContext, "Query step started, {0}.", stepType);
			return new QueryExecutionStep(stepType, this.stopwatch);
		}

		public void CompleteStep(QueryExecutionStep step)
		{
			this.CompleteStep(step, null);
		}

		public void CompleteStep(QueryExecutionStep step, IReadOnlyCollection<KeyValuePair<string, object>> additionalStatistics)
		{
			step.Complete(this.stopwatch, additionalStatistics);
			ExTraceGlobals.InstantSearchTracer.TraceDebug<QueryExecutionStepType, long>((long)this.traceContext, "Query step completed, {0} {1}ms.", step.StepType, step.ElapsedMilliseconds);
			lock (this.steps)
			{
				this.steps.Add(step);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("QueryStatistics -> Version: {0}, StartTime: {1}, EndTime {2}, ElapsedMilliseconds: {3}, LightningEnabled: {4}, StoreBypassed: {5}", new object[]
			{
				this.Version,
				this.StartTime,
				this.EndTime,
				this.ElapsedMilliseconds,
				this.LightningEnabled,
				this.StoreBypassed
			});
			stringBuilder.AppendLine();
			lock (this.steps)
			{
				foreach (QueryExecutionStep queryExecutionStep in this.steps)
				{
					stringBuilder.Append("  ");
					stringBuilder.AppendLine(queryExecutionStep.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		private const int QueryStatisticsVersion = 1;

		private readonly Stopwatch stopwatch = Stopwatch.StartNew();

		private readonly int traceContext;

		private readonly QueryExecutionStep executionStep;

		private List<QueryExecutionStep> steps = new List<QueryExecutionStep>();
	}
}
