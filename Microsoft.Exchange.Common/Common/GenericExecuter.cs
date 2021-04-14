using System;

namespace Microsoft.Exchange.Common
{
	internal class GenericExecuter : IExecuter
	{
		public GenericExecuter(IExceptionTranslator exceptionTranslator)
		{
			this.exceptionTranslator = exceptionTranslator;
		}

		public virtual void Execute(Action wrappedCall)
		{
			try
			{
				wrappedCall();
			}
			catch (Exception exception)
			{
				Exception ex;
				if (this.exceptionTranslator.TryTranslate(exception, out ex))
				{
					throw ex;
				}
				throw;
			}
		}

		private readonly IExceptionTranslator exceptionTranslator;
	}
}
