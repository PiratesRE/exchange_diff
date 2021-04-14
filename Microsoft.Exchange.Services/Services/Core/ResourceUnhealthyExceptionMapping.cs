using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ResourceUnhealthyExceptionMapping : StaticExceptionMapping
	{
		public ResourceUnhealthyExceptionMapping() : base(typeof(ResourceUnhealthyException), ExchangeVersion.Exchange2007, ResponseCodeType.ErrorServerBusy, (CoreResources.IDs)3655513582U)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			if (Global.WriteThrottlingDiagnostics)
			{
				ResourceUnhealthyException ex = base.VerifyExceptionType<ResourceUnhealthyException>(exception);
				return new Dictionary<string, string>
				{
					{
						"Resource",
						ex.ResourceKey.ToString()
					}
				};
			}
			return null;
		}

		private const string Resource = "Resource";
	}
}
