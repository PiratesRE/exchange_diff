using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Pipeline
{
	internal class ErrorHandler : IPipelineErrorHandler
	{
		private ErrorHandler()
		{
		}

		internal static ErrorHandler Instance
		{
			get
			{
				return ErrorHandler.instance;
			}
		}

		public DocumentResolution HandleException(IPipelineComponent component, ComponentException exception)
		{
			if (exception is PoisonComponentException)
			{
				return DocumentResolution.PoisonComponentAndContinue;
			}
			return DocumentResolution.CompleteSuccess;
		}

		private static ErrorHandler instance = new ErrorHandler();
	}
}
