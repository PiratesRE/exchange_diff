using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InternalServerErrorException : ServicePermanentException
	{
		public InternalServerErrorException(Exception innerException) : base(CoreResources.IDs.ErrorInternalServerError, innerException)
		{
			if (Global.WriteStackTraceOnISE)
			{
				InternalServerErrorException ex = innerException as InternalServerErrorException;
				if (ex != null)
				{
					base.ConstantValues.Add("ExceptionClass", ex.ConstantValues["ExceptionClass"]);
					base.ConstantValues.Add("ExceptionMessage", ex.ConstantValues["ExceptionMessage"]);
					base.ConstantValues.Add("StackTrace", ex.ConstantValues["StackTrace"]);
					return;
				}
				base.ConstantValues.Add("ExceptionClass", innerException.GetType().FullName);
				base.ConstantValues.Add("ExceptionMessage", innerException.Message);
				base.ConstantValues.Add("StackTrace", innerException.StackTrace);
			}
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private const string ExceptionClassKey = "ExceptionClass";

		private const string ExceptionMessageKey = "ExceptionMessage";

		private const string StackTraceKey = "StackTrace";
	}
}
