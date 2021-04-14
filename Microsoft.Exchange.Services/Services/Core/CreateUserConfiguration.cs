using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateUserConfiguration : UserConfigurationCommandBase<CreateUserConfigurationRequest, ServiceResultNone>
	{
		public CreateUserConfiguration(CallContext callContext, CreateUserConfigurationRequest request) : base(callContext, request)
		{
			this.serviceUserConfiguration = request.UserConfiguration;
			ServiceCommandBase.ThrowIfNull(this.serviceUserConfiguration, "serviceUserConfiguration", "CreateUserConfiguration::ctor");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CreateUserConfigurationResponse createUserConfigurationResponse = new CreateUserConfigurationResponse();
			createUserConfigurationResponse.ProcessServiceResult(base.Result);
			return createUserConfigurationResponse;
		}

		private static UserConfiguration CreateInstance(UserConfigurationCommandBase<CreateUserConfigurationRequest, ServiceResultNone>.UserConfigurationName userConfigurationName)
		{
			UserConfiguration result;
			try
			{
				result = userConfigurationName.MailboxSession.UserConfigurationManager.CreateFolderConfiguration(userConfigurationName.Name, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, userConfigurationName.FolderId);
			}
			catch (ObjectExistedException ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<ObjectExistedException, string, StoreId>(0L, "ObjectExistedException during UserConfiguration creation: {0} Name {1} FolderId: {2}", ex, userConfigurationName.Name, userConfigurationName.FolderId);
				throw new ObjectSaveException(CoreResources.IDs.ErrorItemSaveUserConfigurationExists, ex);
			}
			return result;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			UserConfigurationCommandBase<CreateUserConfigurationRequest, ServiceResultNone>.ValidatePropertiesForUpdate(this.serviceUserConfiguration);
			UserConfigurationCommandBase<CreateUserConfigurationRequest, ServiceResultNone>.UserConfigurationName userConfigurationName = base.GetUserConfigurationName(this.serviceUserConfiguration.UserConfigurationName);
			using (UserConfiguration userConfiguration = CreateUserConfiguration.CreateInstance(userConfigurationName))
			{
				UserConfigurationCommandBase<CreateUserConfigurationRequest, ServiceResultNone>.SetProperties(this.serviceUserConfiguration, userConfiguration);
				userConfiguration.Save();
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private ServiceUserConfiguration serviceUserConfiguration;
	}
}
