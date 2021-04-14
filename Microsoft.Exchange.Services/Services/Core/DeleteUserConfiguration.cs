using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DeleteUserConfiguration : UserConfigurationCommandBase<DeleteUserConfigurationRequest, ServiceResultNone>
	{
		public DeleteUserConfiguration(CallContext callContext, DeleteUserConfigurationRequest request) : base(callContext, request)
		{
			this.userConfigurationName = base.Request.UserConfigurationName;
			ServiceCommandBase.ThrowIfNull(this.userConfigurationName, "userConfigurationName", "DeleteUserConfiguration:Execute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			DeleteUserConfigurationResponse deleteUserConfigurationResponse = new DeleteUserConfigurationResponse();
			deleteUserConfigurationResponse.ProcessServiceResult(base.Result);
			return deleteUserConfigurationResponse;
		}

		private static void Delete(UserConfigurationCommandBase<DeleteUserConfigurationRequest, ServiceResultNone>.UserConfigurationName userConfigurationName)
		{
			OperationResult operationResult = userConfigurationName.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(userConfigurationName.FolderId, new string[]
			{
				userConfigurationName.Name
			});
			if (operationResult != OperationResult.Succeeded)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<OperationResult>(0L, "Delete UserConfiguration failed. OperationResult: {0}", operationResult);
				throw new DeleteItemsException((CoreResources.IDs)3912965805U);
			}
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			UserConfigurationCommandBase<DeleteUserConfigurationRequest, ServiceResultNone>.UserConfigurationName userConfigurationName = base.GetUserConfigurationName(this.userConfigurationName);
			UserConfiguration userConfiguration = UserConfigurationCommandBase<DeleteUserConfigurationRequest, ServiceResultNone>.Get(userConfigurationName);
			userConfiguration.Dispose();
			DeleteUserConfiguration.Delete(userConfigurationName);
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private UserConfigurationNameType userConfigurationName;
	}
}
