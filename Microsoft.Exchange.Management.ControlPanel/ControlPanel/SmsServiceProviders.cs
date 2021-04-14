using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class SmsServiceProviders
	{
		private SmsServiceProviders()
		{
			TextMessagingHostingDataCache.Instance.Changed += this.Refresh;
			this.Refresh(null);
		}

		public static SmsServiceProviders Instance
		{
			get
			{
				return SmsServiceProviders.instance;
			}
		}

		public IEnumerable<RegionData> RegionList
		{
			get
			{
				return this.regionList;
			}
		}

		public IDictionary<string, CarrierData> CarrierDictionary
		{
			get
			{
				return this.carrierList;
			}
		}

		public IDictionary<string, CarrierData> VoiceMailCarrierDictionary
		{
			get
			{
				return this.voiceMailCarrierList;
			}
		}

		public static string GetLocalizedName(Dictionary<string, string> localizedNames)
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string name = currentCulture.Name;
			string twoLetterISOLanguageName = currentCulture.TwoLetterISOLanguageName;
			string result;
			if (!localizedNames.TryGetValue(name, out result) && !localizedNames.TryGetValue(twoLetterISOLanguageName, out result))
			{
				result = localizedNames.Values.First<string>();
			}
			return result;
		}

		private void Refresh(TextMessagingHostingData hostingData)
		{
			TextMessagingHostingData textMessagingHostingData = hostingData ?? TextMessagingHostingDataCache.Instance.GetHostingData();
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				this.regionList = new List<RegionData>();
				this.carrierList = new Dictionary<string, CarrierData>();
				this.voiceMailCarrierList = new Dictionary<string, CarrierData>();
				foreach (TextMessagingHostingDataCarriersCarrier textMessagingHostingDataCarriersCarrier in textMessagingHostingData.Carriers.Carrier)
				{
					string text = textMessagingHostingDataCarriersCarrier.Identity.ToString("00000");
					CarrierData carrierData = new CarrierData();
					carrierData.ID = text;
					bool flag2 = false;
					foreach (TextMessagingHostingDataServicesService textMessagingHostingDataServicesService in textMessagingHostingData.Services.Service)
					{
						if (textMessagingHostingDataServicesService.CarrierIdentity == textMessagingHostingDataCarriersCarrier.Identity)
						{
							if (TextMessagingHostingDataServicesServiceType.SmtpToSmsGateway == textMessagingHostingDataServicesService.Type && textMessagingHostingDataServicesService.SmtpToSmsGateway != null)
							{
								carrierData.HasSmtpGateway = true;
							}
							else if (textMessagingHostingDataServicesService.Type == TextMessagingHostingDataServicesServiceType.VoiceCallForwarding && textMessagingHostingDataServicesService.VoiceCallForwarding != null)
							{
								carrierData.UnifiedMessagingInfo = new UnifiedMessagingInfo(textMessagingHostingDataServicesService.VoiceCallForwarding.Enable, textMessagingHostingDataServicesService.VoiceCallForwarding.Disable, textMessagingHostingDataServicesService.VoiceCallForwarding.Type.ToString());
								flag2 = true;
							}
						}
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>(textMessagingHostingDataCarriersCarrier.LocalizedInfo.Length);
					for (int k = 0; k < textMessagingHostingDataCarriersCarrier.LocalizedInfo.Length; k++)
					{
						dictionary.Add(textMessagingHostingDataCarriersCarrier.LocalizedInfo[k].Culture, textMessagingHostingDataCarriersCarrier.LocalizedInfo[k].DisplayName);
					}
					carrierData.LocalizedNames = dictionary;
					this.carrierList.Add(text, carrierData);
					if (flag2)
					{
						this.voiceMailCarrierList.Add(text, carrierData);
					}
				}
				Dictionary<string, string> localizedCarrierNames = new Dictionary<string, string>(this.carrierList.Count);
				foreach (KeyValuePair<string, CarrierData> keyValuePair in this.carrierList)
				{
					localizedCarrierNames.Add(keyValuePair.Key, SmsServiceProviders.GetLocalizedName(keyValuePair.Value.LocalizedNames));
				}
				TextMessagingHostingDataRegionsRegion[] region2 = textMessagingHostingData.Regions.Region;
				int l = 0;
				while (l < region2.Length)
				{
					TextMessagingHostingDataRegionsRegion region = region2[l];
					RegionInfo regionInfo;
					try
					{
						regionInfo = new RegionInfo(region.Iso2);
					}
					catch (ArgumentException)
					{
						goto IL_3AD;
					}
					goto IL_26A;
					IL_3AD:
					l++;
					continue;
					IL_26A:
					int[] array = (from service in textMessagingHostingData.Services.Service
					where service.RegionIso2 == region.Iso2
					group service by service.CarrierIdentity into servicesByCarrier
					select servicesByCarrier.Key).ToArray<int>();
					if (array.Length == 0)
					{
						goto IL_3AD;
					}
					List<string> list = new List<string>(array.Length);
					foreach (int num in array)
					{
						string text2 = num.ToString("00000");
						if (this.carrierList.ContainsKey(text2))
						{
							list.Add(text2);
						}
					}
					if (list.Count != 0)
					{
						list.Sort((string x, string y) => string.Compare(localizedCarrierNames[x], localizedCarrierNames[y], StringComparison.CurrentCultureIgnoreCase));
						RegionData regionData = new RegionData();
						regionData.ID = region.Iso2.ToUpper();
						regionData.RegionInfo = regionInfo;
						regionData.CountryCode = region.CountryCode;
						regionData.CarrierIDs = list.ToArray();
						this.regionList.Add(regionData);
						goto IL_3AD;
					}
					goto IL_3AD;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
				}
			}
		}

		private static SmsServiceProviders instance = new SmsServiceProviders();

		private List<RegionData> regionList;

		private Dictionary<string, CarrierData> carrierList;

		private Dictionary<string, CarrierData> voiceMailCarrierList;
	}
}
