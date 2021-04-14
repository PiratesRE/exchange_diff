using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal abstract class Result : IResultAccessor
	{
		public Result() : this(null)
		{
		}

		public Result(Exception exception)
		{
			this.Source = null;
			this.Parent = null;
			this.Exception = exception;
			this.StartTime = default(ExDateTime);
			this.StopTime = default(ExDateTime);
		}

		public AnalysisMember Source { get; private set; }

		public Result Parent { get; private set; }

		public Exception Exception { get; protected set; }

		public abstract object ValueAsObject { get; }

		public ExDateTime StartTime { get; private set; }

		public ExDateTime StopTime { get; private set; }

		public bool HasException
		{
			get
			{
				return this.Exception != null;
			}
		}

		public IEnumerable<Result> AncestorsAndSelf()
		{
			for (Result current = this; current != null; current = current.Parent)
			{
				yield return current;
			}
			yield break;
		}

		public Result<T> AncestorOfType<T>(AnalysisMember<T> ancestor)
		{
			Result result = (from x in this.AncestorsAndSelf()
			where x.Source == ancestor
			select x).FirstOrDefault<Result>();
			if (ancestor != null)
			{
				return (Result<T>)result;
			}
			throw new AnalysisException(this.Source, Strings.ResultAncestorNotFound(ancestor.Name));
		}

		public Results<T> DescendantsOfType<T>(AnalysisMember<T> analysisMemeber)
		{
			return new Results<T>(analysisMemeber, from x in analysisMemeber.Results
			where x.AncestorsAndSelf().Contains(this)
			select x);
		}

		void IResultAccessor.SetSource(AnalysisMember source)
		{
			this.Source = source;
		}

		void IResultAccessor.SetParent(Result parent)
		{
			this.Parent = parent;
		}

		void IResultAccessor.SetStartTime(ExDateTime startTime)
		{
			this.StartTime = startTime;
		}

		void IResultAccessor.SetStopTime(ExDateTime stopTime)
		{
			this.StopTime = stopTime;
		}
	}
}
