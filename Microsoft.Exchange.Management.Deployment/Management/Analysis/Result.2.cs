using System;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class Result<T> : Result
	{
		public Result(T value)
		{
			this.value = value;
		}

		public Result(Exception exception) : base(exception)
		{
			this.value = default(T);
		}

		public Result(Func<T> valueFunction)
		{
			try
			{
				this.value = valueFunction();
			}
			catch (Exception exception)
			{
				this.value = default(T);
				base.Exception = exception;
			}
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
