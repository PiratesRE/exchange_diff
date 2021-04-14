using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class OverBudgetExceptionMapping : ExceptionMappingBase
	{
		public OverBudgetExceptionMapping() : base(typeof(OverBudgetException), ExceptionMappingBase.Attributes.StopsBatchProcessing)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			OverBudgetException ex = exception as OverBudgetException;
			EwsBudget.LogOverBudgetToIIS(ex);
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.IsMaxConcurrency(exception))
			{
				dictionary.Add("Policy", ex.PolicyPart);
				dictionary.Add("MaxConcurrencyLimit", ex.PolicyValue);
				dictionary.Add("ErrorMessage", ex.Message);
			}
			else if (ex.BackoffTime >= 0)
			{
				dictionary.Add("BackOffMilliseconds", ex.BackoffTime.ToString());
			}
			return dictionary;
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			if (this.UseMaxConcurrencyError(exception))
			{
				return ResponseCodeType.ErrorExceededConnectionCount;
			}
			return ResponseCodeType.ErrorServerBusy;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			if (this.GetResponseCode(exception) != ResponseCodeType.ErrorServerBusy)
			{
				return ExchangeVersion.Exchange2010;
			}
			return ExchangeVersion.Exchange2007;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			if (this.UseMaxConcurrencyError(exception))
			{
				return CoreResources.IDs.ErrorExceededConnectionCount;
			}
			return (CoreResources.IDs)3655513582U;
		}

		private bool UseMaxConcurrencyError(LocalizedException exception)
		{
			return this.IsMaxConcurrency(exception) && ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010);
		}

		private bool IsMaxConcurrency(LocalizedException exception)
		{
			OverBudgetException ex = base.VerifyExceptionType<OverBudgetException>(exception);
			return ex.PolicyPart == "MaxConcurrency" || ex.PolicyPart == "MaxStreamingConcurrency";
		}

		private const string BackOffMilliseconds = "BackOffMilliseconds";

		private const string MaxConcurrencyLimit = "MaxConcurrencyLimit";

		private const string Policy = "Policy";

		private const string ErrorMessage = "ErrorMessage";
	}
}
