using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateUserConfiguration : UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>
	{
		public UpdateUserConfiguration(CallContext callContext, UpdateUserConfigurationRequest request) : base(callContext, request)
		{
			this.serviceUserConfiguration = base.Request.UserConfiguration;
			ServiceCommandBase.ThrowIfNull(this.serviceUserConfiguration, "serviceUserConfiguration", "UpdateUserConfiguration:PreExecute");
		}

		public UpdateUserConfiguration(CallContext callContext, UpdateUserConfigurationOwaRequest request) : base(callContext, request)
		{
			this.replaceDictionary = false;
			this.serviceUserConfiguration = base.Request.UserConfiguration;
			ServiceCommandBase.ThrowIfNull(this.serviceUserConfiguration, "serviceUserConfiguration", "UpdateUserConfiguration:PreExecute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UpdateUserConfigurationResponse updateUserConfigurationResponse = new UpdateUserConfigurationResponse();
			updateUserConfigurationResponse.ProcessServiceResult(base.Result);
			return updateUserConfigurationResponse;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			this.Update(this.serviceUserConfiguration);
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private void Update(ServiceUserConfiguration serviceUserConfiguration)
		{
			UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>.ValidatePropertiesForUpdate(serviceUserConfiguration);
			UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>.UserConfigurationName userConfigurationName = base.GetUserConfigurationName(serviceUserConfiguration.UserConfigurationName);
			using (UserConfiguration userConfiguration = UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>.Get(userConfigurationName))
			{
				if (this.replaceDictionary)
				{
					UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>.SetProperties(serviceUserConfiguration, userConfiguration);
				}
				else
				{
					UserConfigurationCommandBase<UpdateUserConfigurationRequest, ServiceResultNone>.UpdateProperties(serviceUserConfiguration, userConfiguration);
				}
				userConfiguration.Save();
			}
		}

		private readonly bool replaceDictionary = true;

		private ServiceUserConfiguration serviceUserConfiguration;
	}
}
