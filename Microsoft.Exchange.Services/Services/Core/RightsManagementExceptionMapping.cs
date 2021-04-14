using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class RightsManagementExceptionMapping : ExceptionMappingBase
	{
		public RightsManagementExceptionMapping(Type exceptionType, ExchangeVersion effectiveVersion, ResponseCodeType responseCode) : base(exceptionType, ExceptionMappingBase.Attributes.None)
		{
			this.effectiveVersion = effectiveVersion;
			this.responseCode = responseCode;
		}

		public override LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return exception.LocalizedString;
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			return this.responseCode;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			return this.effectiveVersion;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			return CoreResources.IDs.ErrorRightsManagementException;
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			IDictionary<string, string> dictionary = null;
			Exception ex = null;
			if (exception != null)
			{
				ex = exception.InnerException;
			}
			if (ex != null)
			{
				ex = ex.InnerException;
			}
			if (ex != null)
			{
				COMException ex2 = ex as COMException;
				if (ex2 != null)
				{
					dictionary = new Dictionary<string, string>();
					dictionary.Add("HResult", ex2.HResult.ToString());
				}
			}
			return dictionary;
		}

		private const string HResult = "HResult";

		private readonly ExchangeVersion effectiveVersion;

		private readonly ResponseCodeType responseCode;
	}
}
