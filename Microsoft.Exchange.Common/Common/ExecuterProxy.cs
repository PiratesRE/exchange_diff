using System;

namespace Microsoft.Exchange.Common
{
	internal abstract class ExecuterProxy : IExecuter
	{
		protected ExecuterProxy(IExecuter delegateExecuter)
		{
			this.delegateExecuter = delegateExecuter;
		}

		public virtual void Execute(Action wrappedCall)
		{
			this.delegateExecuter.Execute(wrappedCall);
		}

		private readonly IExecuter delegateExecuter;
	}
}
