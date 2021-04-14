using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetUcwaUserConfiguration : ServiceCommand<UcwaUserConfiguration>
	{
		public GetUcwaUserConfiguration(CallContext callContext, string sipUri) : base(callContext)
		{
			this.sipUri = sipUri;
			OwsLogRegistry.Register(GetUcwaUserConfiguration.GetUcwaUserConfigurationActionName, typeof(GetUcwaUserConfigurationMetaData), new Type[0]);
		}

		protected override UcwaUserConfiguration InternalExecute()
		{
			return UcwaConfigurationUtilities.GetUcwaUserConfiguration(this.sipUri, base.CallContext);
		}

		private static readonly string GetUcwaUserConfigurationActionName = typeof(GetUcwaUserConfiguration).Name;

		private readonly string sipUri;
	}
}
