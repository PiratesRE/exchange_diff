using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class DelegateUserValidationException : StoragePermanentException
	{
		public DelegateUserValidationException(LocalizedString message, DelegateValidationProblem problem) : base(message)
		{
			EnumValidator.ThrowIfInvalid<DelegateValidationProblem>(problem, "problem");
			this.problem = problem;
		}

		protected DelegateUserValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.problem = (DelegateValidationProblem)info.GetValue("problem", typeof(DelegateValidationProblem));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("problem", this.problem);
		}

		public DelegateValidationProblem Problem
		{
			get
			{
				return this.problem;
			}
		}

		private const string ProblemLabel = "problem";

		private DelegateValidationProblem problem;
	}
}
