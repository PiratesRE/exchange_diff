using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class DDIVMockedExchangeRunspaceConfiguration : ExchangeRunspaceConfiguration
	{
		internal override bool TryGetExecutingUserId(out ADObjectId executingUserId)
		{
			executingUserId = new ADObjectId();
			return executingUserId != null;
		}

		internal override string ExecutingUserDisplayName
		{
			get
			{
				return string.Empty;
			}
		}

		internal override MultiValuedProperty<CultureInfo> ExecutingUserLanguages
		{
			get
			{
				return new MultiValuedProperty<CultureInfo>();
			}
		}
	}
}
