using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal static class VersionConverter
	{
		public static Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType GetExchangeVersionType(int serverVersion)
		{
			if (ServerCache.Instance.UseE14RtmEwsSchema || serverVersion < Globals.E14SP1Version)
			{
				return Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010;
			}
			if (serverVersion < Globals.E15Version)
			{
				return Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010_SP1;
			}
			return Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2013;
		}

		public static Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType GetRdExchangeVersionType(int serverVersion)
		{
			switch (VersionConverter.GetExchangeVersionType(serverVersion))
			{
			case Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010:
				return Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType.Exchange2010;
			case Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010_SP1:
				return Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType.Exchange2010_SP1;
			case Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2013:
				return Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType.Exchange2012;
			}
			throw new InvalidOperationException("Unexpected version type.");
		}

		public static void Convert(Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportRequestType request, int version)
		{
			Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType exchangeVersionType = VersionConverter.GetExchangeVersionType(version);
			Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType exchangeVersionType2 = exchangeVersionType;
			if (exchangeVersionType2 == Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010)
			{
				TrackingExtendedProperties trackingExtendedProperties = TrackingExtendedProperties.CreateFromTrackingPropertyArray(request.Properties);
				Options options = new Options(VersionConverter.BasicDiagnostics.Equals(request.DiagnosticsLevel, StringComparison.Ordinal), trackingExtendedProperties.ExpandTree, trackingExtendedProperties.SearchAsRecip, request.ServerHint);
				request.DiagnosticsLevel = options.ToString();
				request.ServerHint = null;
				request.Properties = null;
				return;
			}
			if (request.Properties != null && request.Properties.Length == 0)
			{
				request.Properties = null;
			}
		}

		public static void Convert(Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportRequestType request, int version)
		{
			Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType exchangeVersionType = VersionConverter.GetExchangeVersionType(version);
			Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType exchangeVersionType2 = exchangeVersionType;
			if (exchangeVersionType2 == Microsoft.Exchange.SoapWebClient.EWS.ExchangeVersionType.Exchange2010)
			{
				request.Properties = null;
				return;
			}
			if (request.Properties != null && request.Properties.Length == 0)
			{
				request.Properties = null;
			}
		}

		public static string[] GetEventDataToTransmitForRtm(RecipientTrackingEvent recipEvent)
		{
			string[] array = recipEvent.EventData;
			int num = (array != null) ? array.Length : 0;
			if (recipEvent.EventDescription == EventDescription.TransferredToPartnerOrg && num != 1)
			{
				array = new string[]
				{
					recipEvent.RecipientAddress.ToString()
				};
				num = 1;
			}
			int num2 = VersionConverter.eventDataToPropertyMapping.Count + num;
			string[] array2 = new string[num2];
			if (num > 0)
			{
				Array.Copy(array, array2, num);
			}
			int num3 = num;
			foreach (string text in VersionConverter.eventDataToPropertyMapping.Keys)
			{
				string arg = VersionConverter.eventDataToPropertyMapping[text].Getter(recipEvent);
				array2[num3++] = text + '=' + arg;
			}
			return array2;
		}

		public static MtrSchemaVersion GetMtrSchemaVersion(ServerVersion serverVersion)
		{
			int num = serverVersion.ToInt();
			if (num < Globals.E14Version)
			{
				return MtrSchemaVersion.Unknown;
			}
			if (num < Globals.E15Version)
			{
				return MtrSchemaVersion.E14;
			}
			return MtrSchemaVersion.E15RTM;
		}

		public static MtrSchemaVersion GetMtrSchemaVersion(ServerInfo server)
		{
			return VersionConverter.GetMtrSchemaVersion(server.AdminDisplayVersion);
		}

		internal static void ConvertRawEventData(string[] rawEventData, RecipientTrackingEvent eventToConvert)
		{
			VersionConverter.ParseRawEventData(rawEventData, eventToConvert);
		}

		private static Dictionary<string, VersionConverter.RawEventAccessor> CreateEventDataToPropertyMappingDictionary()
		{
			Dictionary<string, VersionConverter.RawEventAccessor> dictionary = new Dictionary<string, VersionConverter.RawEventAccessor>();
			VersionConverter.ParameterSetter setter = delegate(RecipientTrackingEvent trackingEventToSet, string value)
			{
				trackingEventToSet.RootAddress = value;
			};
			VersionConverter.ParameterGetter getter = (RecipientTrackingEvent trackingEventToGetFrom) => trackingEventToGetFrom.RootAddress;
			dictionary.Add("root", new VersionConverter.RawEventAccessor(setter, getter));
			return dictionary;
		}

		private static void ParseRawEventData(string[] data, RecipientTrackingEvent trackingEvent)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			List<string> list = new List<string>(data.Length);
			int i = 0;
			while (i < data.Length)
			{
				int num = data[i].IndexOf('=');
				if (num == -1)
				{
					goto IL_90;
				}
				string key = data[i].Substring(0, num);
				VersionConverter.RawEventAccessor rawEventAccessor;
				if (!VersionConverter.eventDataToPropertyMapping.TryGetValue(key, out rawEventAccessor))
				{
					goto IL_90;
				}
				if (string.IsNullOrEmpty(rawEventAccessor.Getter(trackingEvent)))
				{
					string value = null;
					if (num < data[i].Length - 1)
					{
						value = data[i].Substring(num + 1, data[i].Length - num - 1);
					}
					rawEventAccessor.Setter(trackingEvent, value);
				}
				IL_99:
				i++;
				continue;
				IL_90:
				list.Add(data[i]);
				goto IL_99;
			}
			trackingEvent.EventData = list.ToArray();
		}

		private const char EventDataSeparator = '=';

		public static readonly string BasicDiagnostics = DiagnosticsLevel.Basic.ToString();

		private static Dictionary<string, VersionConverter.RawEventAccessor> eventDataToPropertyMapping = VersionConverter.CreateEventDataToPropertyMappingDictionary();

		private delegate void ParameterSetter(RecipientTrackingEvent trackingEvent, string value);

		private delegate string ParameterGetter(RecipientTrackingEvent trackingEvent);

		private class RawEventAccessor
		{
			public RawEventAccessor(VersionConverter.ParameterSetter setter, VersionConverter.ParameterGetter getter)
			{
				this.Getter = getter;
				this.Setter = setter;
			}

			public readonly VersionConverter.ParameterSetter Setter;

			public readonly VersionConverter.ParameterGetter Getter;
		}
	}
}
