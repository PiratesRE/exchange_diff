using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetTextMessagingAccountCommand : SingleCmdletCommandBase<GetTextMessagingAccountRequest, GetTextMessagingAccountResponse, GetTextMessagingAccount, Microsoft.Exchange.Data.Storage.Management.TextMessagingAccount>
	{
		public GetTextMessagingAccountCommand(CallContext callContext, GetTextMessagingAccountRequest request) : base(callContext, request, "Get-TextMessagingAccount", ScopeLocation.RecipientRead)
		{
			this.GetSmsServicesProviders();
		}

		protected override void PopulateTaskParameters()
		{
			GetTextMessagingAccount task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetTextMessagingAccountResponse response)
		{
			Microsoft.Exchange.Data.Storage.Management.TextMessagingAccount result = this.cmdletRunner.TaskWrapper.Result;
			if (result == null)
			{
				response.Data = null;
				return;
			}
			response.Data = new Microsoft.Exchange.Services.Wcf.Types.TextMessagingAccount
			{
				CountryRegionId = ((result.CountryRegionId == null) ? null : result.CountryRegionId.ToString()),
				E164NotificationPhoneNumber = ((result.NotificationPhoneNumber != null) ? new E164NumberData(result.NotificationPhoneNumber) : null),
				EasEnabled = result.EasEnabled,
				MobileOperatorId = result.MobileOperatorId,
				NotificationPhoneNumber = ((result.NotificationPhoneNumber != null) ? result.NotificationPhoneNumber.ToString() : null),
				NotificationPhoneNumberVerified = result.NotificationPhoneNumberVerified,
				CarrierList = this.carrierList.ToArray(),
				RegionList = this.regionList.ToArray(),
				VoiceMailCarrierList = this.voiceMailCarrierList.ToArray()
			};
		}

		protected override PSLocalTask<GetTextMessagingAccount, Microsoft.Exchange.Data.Storage.Management.TextMessagingAccount> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetTextMessagingAccountTask(base.CallContext.AccessingPrincipal);
		}

		private void GetSmsServicesProviders()
		{
			TextMessagingHostingData hostingData = TextMessagingHostingDataCache.Instance.GetHostingData();
			lock (this)
			{
				this.LoadCarriers(hostingData);
				this.LoadRegions(hostingData);
			}
		}

		private void LoadRegions(TextMessagingHostingData data)
		{
			this.regionList = new List<RegionData>();
			TextMessagingHostingDataRegionsRegion[] region2 = data.Regions.Region;
			int i = 0;
			while (i < region2.Length)
			{
				TextMessagingHostingDataRegionsRegion region = region2[i];
				RegionInfo regionInfo;
				try
				{
					regionInfo = new RegionInfo(region.Iso2);
				}
				catch (ArgumentException)
				{
					goto IL_1A6;
				}
				goto IL_53;
				IL_1A6:
				i++;
				continue;
				IL_53:
				int[] array = (from service in data.Services.Service
				where service.RegionIso2 == region.Iso2
				group service by service.CarrierIdentity into servicesByCarrier
				select servicesByCarrier.Key).ToArray<int>();
				if (array.Length == 0)
				{
					goto IL_1A6;
				}
				List<string> list = new List<string>(array.Length);
				list.AddRange(from carrierId in array
				select carrierId.ToString("00000") into carrierIdStr
				where this.carrierList.Exists((CarrierData x) => x.CarrierId == carrierIdStr)
				select carrierIdStr);
				if (list.Count != 0)
				{
					RegionData item = new RegionData
					{
						RegionId = region.Iso2.ToUpper(),
						RegionName = ((regionInfo.EnglishName == regionInfo.NativeName) ? regionInfo.EnglishName : string.Format("{0} ({1})", regionInfo.NativeName, regionInfo.EnglishName)),
						CountryCode = region.CountryCode,
						CarrierIds = list.ToArray()
					};
					this.regionList.Add(item);
					goto IL_1A6;
				}
				goto IL_1A6;
			}
		}

		private void LoadCarriers(TextMessagingHostingData data)
		{
			this.carrierList = new List<CarrierData>();
			this.voiceMailCarrierList = new List<CarrierData>();
			foreach (TextMessagingHostingDataCarriersCarrier textMessagingHostingDataCarriersCarrier in data.Carriers.Carrier)
			{
				string carrierId = textMessagingHostingDataCarriersCarrier.Identity.ToString("00000");
				CarrierData carrierData = new CarrierData();
				carrierData.CarrierId = carrierId;
				bool flag = false;
				foreach (TextMessagingHostingDataServicesService textMessagingHostingDataServicesService in data.Services.Service)
				{
					if (textMessagingHostingDataServicesService.CarrierIdentity == textMessagingHostingDataCarriersCarrier.Identity)
					{
						if (TextMessagingHostingDataServicesServiceType.SmtpToSmsGateway == textMessagingHostingDataServicesService.Type && textMessagingHostingDataServicesService.SmtpToSmsGateway != null)
						{
							carrierData.HasSmtpGateway = true;
						}
						else if (textMessagingHostingDataServicesService.Type == TextMessagingHostingDataServicesServiceType.VoiceCallForwarding && textMessagingHostingDataServicesService.VoiceCallForwarding != null)
						{
							carrierData.UnifiedMessagingInfo = new UnifiedMessagingInfo
							{
								EnableTemplate = textMessagingHostingDataServicesService.VoiceCallForwarding.Enable,
								DisableTemplate = textMessagingHostingDataServicesService.VoiceCallForwarding.Disable,
								CallForwardingType = textMessagingHostingDataServicesService.VoiceCallForwarding.Type.ToString()
							};
							flag = true;
						}
					}
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>(textMessagingHostingDataCarriersCarrier.LocalizedInfo.Length);
				foreach (TextMessagingHostingDataCarriersCarrierLocalizedInfo textMessagingHostingDataCarriersCarrierLocalizedInfo in textMessagingHostingDataCarriersCarrier.LocalizedInfo)
				{
					dictionary.Add(textMessagingHostingDataCarriersCarrierLocalizedInfo.Culture, textMessagingHostingDataCarriersCarrierLocalizedInfo.DisplayName);
				}
				carrierData.CarrierName = GetTextMessagingAccountCommand.GetLocalizedName(dictionary, base.CallContext.ClientCulture);
				this.carrierList.Add(carrierData);
				if (flag)
				{
					this.voiceMailCarrierList.Add(carrierData);
				}
			}
		}

		public static string GetLocalizedName(Dictionary<string, string> localizedNames, CultureInfo cultureInfo)
		{
			CultureInfo cultureInfo2 = new CultureInfo(cultureInfo.Name);
			string name = cultureInfo2.Name;
			string twoLetterISOLanguageName = cultureInfo2.TwoLetterISOLanguageName;
			string result;
			if (!localizedNames.TryGetValue(name, out result) && !localizedNames.TryGetValue(twoLetterISOLanguageName, out result))
			{
				result = localizedNames.Values.First<string>();
			}
			return result;
		}

		private List<CarrierData> carrierList;

		private List<RegionData> regionList;

		private List<CarrierData> voiceMailCarrierList;
	}
}
