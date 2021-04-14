using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetClientExtension : SingleStepServiceCommand<GetClientExtensionRequest, GetClientExtensionResponse>
	{
		public GetClientExtension(CallContext callContext, GetClientExtensionRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(GetExtensionsMetadata), new Type[0]);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetClientExtensionResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetClientExtensionResponse> Execute()
		{
			List<ClientExtension> clientExtensions = new List<ClientExtension>();
			GetClientExtensionResponse getClientExtensionResponse = null;
			bool isAdminRequest = null == base.Request.UserParameters;
			if (base.Request.RequestedExtensionIds != null && base.Request.RequestedExtensionIds.Length > 0)
			{
				this.requestedExtensionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (string extensionId in base.Request.RequestedExtensionIds)
				{
					this.requestedExtensionIds.Add(ExtensionDataHelper.FormatExtensionId(extensionId));
				}
			}
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				getClientExtensionResponse = new GetClientExtensionResponse();
				string text;
				List<ExtensionData> orgExtensionDataList = GetExtensibilityContext.GetOrgExtensionDataList(this.CallContext, !isAdminRequest, this.Request.IsDebug, out text);
				if (orgExtensionDataList != null)
				{
					foreach (ExtensionData extensionData in orgExtensionDataList)
					{
						if (this.ShouldReturnExtension(extensionData))
						{
							ClientExtension clientExtension = new ClientExtension();
							clientExtension.IsAvailable = extensionData.Enabled;
							clientExtension.IsMandatory = extensionData.IsMandatory;
							clientExtension.IsEnabledByDefault = extensionData.IsEnabledByDefault;
							if (extensionData.Type != null)
							{
								clientExtension.Type = extensionData.Type.Value;
							}
							if (extensionData.Scope != null)
							{
								clientExtension.Scope = extensionData.Scope.Value;
							}
							clientExtension.MarketplaceAssetId = extensionData.MarketplaceAssetID;
							clientExtension.MarketplaceContentMarket = extensionData.MarketplaceContentMarket;
							clientExtension.AppStatus = extensionData.AppStatus;
							clientExtension.Etoken = extensionData.Etoken;
							if (isAdminRequest)
							{
								clientExtension.ProvidedTo = extensionData.ProvidedTo;
								clientExtension.SpecificUsers = extensionData.SpecificUsers;
							}
							clientExtension.Manifest = GetAppManifests.GetEncodedManifestString(extensionData);
							clientExtensions.Add(clientExtension);
						}
					}
				}
				getClientExtensionResponse.ClientExtensions = clientExtensions.ToArray();
				if (this.Request.IsDebug && !string.IsNullOrWhiteSpace(text))
				{
					try
					{
						byte[] bytes = Encoding.UTF8.GetBytes(text);
						getClientExtensionResponse.RawMasterTableXml = Convert.ToBase64String(bytes);
					}
					catch (ArgumentException innerException)
					{
						throw new OwaExtensionOperationException(innerException);
					}
				}
			});
			if (serviceError != null)
			{
				return new ServiceResult<GetClientExtensionResponse>(serviceError);
			}
			return new ServiceResult<GetClientExtensionResponse>(getClientExtensionResponse);
		}

		private bool ShouldReturnExtension(ExtensionData extensionData)
		{
			string text = ExtensionDataHelper.FormatExtensionId(extensionData.ExtensionId);
			if (this.requestedExtensionIds != null && !this.requestedExtensionIds.Contains(text))
			{
				return false;
			}
			if (base.Request.UserParameters == null)
			{
				return true;
			}
			if (string.IsNullOrEmpty(base.Request.UserParameters.UserId))
			{
				return false;
			}
			if (!extensionData.IsAvailable)
			{
				return false;
			}
			bool flag = extensionData.ProvidedTo == ClientExtensionProvidedTo.Everyone;
			if (extensionData.ProvidedTo == ClientExtensionProvidedTo.SpecificUsers && extensionData.SpecificUsers != null)
			{
				foreach (string a in extensionData.SpecificUsers)
				{
					if (string.Equals(a, base.Request.UserParameters.UserId, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			if (!base.Request.UserParameters.EnabledOnly)
			{
				return true;
			}
			if (extensionData.IsMandatory)
			{
				return true;
			}
			if (extensionData.IsEnabledByDefault)
			{
				if (!base.Request.UserParameters.IsDisabledByUser(text))
				{
					return true;
				}
			}
			else if (base.Request.UserParameters.IsEnabledByUser(text))
			{
				return true;
			}
			return false;
		}

		private HashSet<string> requestedExtensionIds;
	}
}
