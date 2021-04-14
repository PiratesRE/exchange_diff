using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class Result<T> : Result
	{
		public Result(T value) : base(null, null, null, default(ExDateTime), default(ExDateTime))
		{
			this.value = value;
		}

		public Result(Exception exception) : base(null, null, exception, default(ExDateTime), default(ExDateTime))
		{
			this.value = default(T);
		}

		public Result(Result<T> toCopy, AnalysisMember source, Result parent, ExDateTime startTime, ExDateTime stopTime) : base(source, parent, toCopy.Exception, startTime, stopTime)
		{
			this.value = toCopy.ValueOrDefault;
		}

		public static Result<T> Default
		{
			get
			{
				return new Result<T>(default(T));
			}
		}

		public static Result<T> Failure
		{
			get
			{
				return new Result<T>(new FailureException());
			}
		}

		public T Value
		{
			get
			{
				if (base.HasException)
				{
					throw new AccessedFailedResultException(base.Source, base.Exception);
				}
				return this.value;
			}
		}

		public override object ValueAsObject
		{
			get
			{
				return this.Value;
			}
		}

		public T ValueOrDefault
		{
			get
			{
				if (base.HasException)
				{
					return default(T);
				}
				return this.value;
			}
		}

		public static Result<T> FailureBecause(string reason)
		{
			return new Result<T>(new FailureException(reason));
		}

		private readonly T value;
	}
}
