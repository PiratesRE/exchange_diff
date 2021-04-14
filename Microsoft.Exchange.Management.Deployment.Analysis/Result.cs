using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class Result
	{
		protected Result(AnalysisMember source, Result parent, Exception exception, ExDateTime startTime, ExDateTime stopTime)
		{
			this.source = source;
			this.parent = parent;
			this.exception = exception;
			this.startTime = startTime;
			this.stopTime = stopTime;
		}

		public AnalysisMember Source
		{
			get
			{
				return this.source;
			}
		}

		public Result Parent
		{
			get
			{
				return this.parent;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public ExDateTime StopTime
		{
			get
			{
				return this.stopTime;
			}
		}

		public abstract object ValueAsObject { get; }

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
			Result result = this.AncestorsAndSelf().FirstOrDefault((Result x) => x.Source == ancestor);
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

		private readonly AnalysisMember source;

		private readonly Result parent;

		private readonly Exception exception;

		private readonly ExDateTime startTime;

		private readonly ExDateTime stopTime;
	}
}
