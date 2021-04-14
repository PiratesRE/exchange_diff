using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class GetRegionalConfiguration : ServiceCommand<GetRegionalConfigurationResponse>
	{
		public GetRegionalConfiguration(CallContext callContext, GetRegionalConfigurationRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "GetRegionalConfigurationRequest", "GetRegionalConfiguration::GetRegionalConfiguration");
			request.Validate();
		}

		protected override GetRegionalConfigurationResponse InternalExecute()
		{
			GetRegionalConfigurationResponse getRegionalConfigurationResponse = new GetRegionalConfigurationResponse();
			List<CultureInfo> supportedCultureInfos = ClientCultures.SupportedCultureInfos;
			getRegionalConfigurationResponse.SupportedCultures = this.ToCultureInfoData(supportedCultureInfos);
			return getRegionalConfigurationResponse;
		}

		private CultureInfoData[] ToCultureInfoData(List<CultureInfo> preferredCultureInfo)
		{
			CultureInfoData[] array = new CultureInfoData[preferredCultureInfo.Count];
			for (int i = 0; i < preferredCultureInfo.Count; i++)
			{
				array[i] = new CultureInfoData
				{
					Name = preferredCultureInfo[i].Name,
					NativeName = preferredCultureInfo[i].NativeName,
					LCID = preferredCultureInfo[i].LCID
				};
			}
			return array;
		}
	}
}
