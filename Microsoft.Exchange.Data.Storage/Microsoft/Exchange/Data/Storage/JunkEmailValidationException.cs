using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class JunkEmailValidationException : StoragePermanentException
	{
		public JunkEmailValidationException(string value, JunkEmailCollection.ValidationProblem problem) : base(ServerStrings.JunkEmailValidationError(value))
		{
			EnumValidator.ThrowIfInvalid<JunkEmailCollection.ValidationProblem>(problem, "problem");
			this.problem = problem;
		}

		public JunkEmailCollection.ValidationProblem Problem
		{
			get
			{
				return this.problem;
			}
		}

		private JunkEmailCollection.ValidationProblem problem;
	}
}
