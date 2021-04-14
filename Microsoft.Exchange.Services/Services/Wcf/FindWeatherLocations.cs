using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class FindWeatherLocations : ServiceCommand<IAsyncResult>
	{
		public FindWeatherLocations(CallContext context, FindWeatherLocationsRequest request, AsyncCallback asyncCallback, object asyncState, IWeatherService weatherService) : base(context)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "FindWeatherLocations::ctor");
			WcfServiceCommandBase.ThrowIfNull(asyncCallback, "asyncCallback", "FindWeatherLocations::ctor");
			WcfServiceCommandBase.ThrowIfNull(weatherService, "weatherService", "FindWeatherLocations::ctor");
			OwsLogRegistry.Register(typeof(FindWeatherLocations).Name, typeof(WeatherMetadata), new Type[0]);
			this.request = request;
			this.asyncResult = new ServiceAsyncResult<Task<FindWeatherLocationsResponse>>();
			this.asyncResult.AsyncCallback = asyncCallback;
			this.asyncResult.AsyncState = asyncState;
			this.weatherService = weatherService;
		}

		protected override IAsyncResult InternalExecute()
		{
			FindWeatherLocations.TraceRequest(this.request);
			this.weatherService.VerifyServiceAvailability(base.CallContext);
			Task<FindWeatherLocationsResponse> task2 = new Task<FindWeatherLocationsResponse>(() => FindWeatherLocations.FindLocations(this.request.SearchString, base.CallContext, this.weatherService));
			task2.Start();
			task2.ContinueWith(delegate(Task<FindWeatherLocationsResponse> task)
			{
				this.asyncResult.Complete(task);
			});
			this.asyncResult.Data = task2;
			return this.asyncResult;
		}

		internal static Uri ConstructUriForLocationId(string escapedSearchString, string culture, IWeatherService weatherService)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}?{1}={2}", new object[]
			{
				weatherService.BaseUrl,
				"src",
				weatherService.PartnerId
			});
			text = string.Join("&", new string[]
			{
				text,
				WeatherCommon.FormatWebFormField("culture", culture),
				WeatherCommon.FormatWebFormField("outputview", "searchLocations"),
				WeatherCommon.FormatWebFormField("weasearchstr", escapedSearchString)
			});
			ExTraceGlobals.WeatherTracer.TraceDebug<string>((long)FindWeatherLocations.FindWeatherLocationsTraceId, "Constructed URI string is '{0}'", text);
			return new Uri(text);
		}

		private static void TraceRequest(FindWeatherLocationsRequest request)
		{
			if (request == null)
			{
				ExTraceGlobals.WeatherTracer.TraceDebug((long)FindWeatherLocations.FindWeatherLocationsTraceId, "No request");
				return;
			}
			ExTraceGlobals.WeatherTracer.TraceDebug<string>((long)FindWeatherLocations.FindWeatherLocationsTraceId, "Search string: {0}", request.SearchString);
		}

		private static FindWeatherLocationsResponse FindLocations(string searchString, CallContext callContext, IWeatherService weatherService)
		{
			string text = CoreResources.MessageCouldNotFindWeatherLocationsForSearchString;
			string text2 = (text == null) ? searchString : string.Format(callContext.OwaCulture, text, new object[]
			{
				searchString
			});
			List<WeatherLocationId> locationIds = new List<WeatherLocationId>();
			string innerErrorMessage = null;
			text2 = WeatherCommon.ExecuteActionAndHandleException(callContext, FindWeatherLocations.FindWeatherLocationsTraceId, text2, delegate
			{
				string text3 = Uri.EscapeDataString(searchString ?? string.Empty);
				string name = callContext.OwaCulture.Name;
				callContext.ProtocolLog.Set(WeatherMetadata.QueryString, text3);
				callContext.ProtocolLog.Set(WeatherMetadata.Culture, name);
				Uri weatherServiceUri = FindWeatherLocations.ConstructUriForLocationId(text3, name, weatherService);
				Stopwatch stopwatch = Stopwatch.StartNew();
				string text4 = weatherService.Get(weatherServiceUri);
				stopwatch.Stop();
				callContext.ProtocolLog.Set(WeatherMetadata.ForecastLatency, stopwatch.ElapsedMilliseconds);
				if (string.IsNullOrWhiteSpace(text4))
				{
					throw new WeatherException("Weather service response string is null or empty");
				}
				if (text4.Length > 70000)
				{
					throw new WeatherException("Weather service response string is too long");
				}
				SafeXmlSerializer safeXmlSerializer;
				using (StringReader stringReader = new StringReader(text4))
				{
					using (XmlReader xmlReader = XmlReader.Create(stringReader))
					{
						if (xmlReader.MoveToContent() == XmlNodeType.Element && string.Equals(xmlReader.Name, "weatherdata"))
						{
							safeXmlSerializer = new SafeXmlSerializer(typeof(WeatherData));
							WeatherData weatherData;
							using (StringReader stringReader2 = new StringReader(text4))
							{
								weatherData = (WeatherData)safeXmlSerializer.Deserialize(stringReader2);
							}
							if (weatherData.Items != null && weatherData.Items.Length == 1 && !string.IsNullOrWhiteSpace(weatherData.Items[0].ErrorMessage))
							{
								innerErrorMessage = weatherData.Items[0].ErrorMessage;
								return;
							}
						}
					}
				}
				safeXmlSerializer = new SafeXmlSerializer(typeof(WeatherLocationData));
				WeatherLocationData weatherLocationData;
				using (StringReader stringReader3 = new StringReader(text4))
				{
					weatherLocationData = (WeatherLocationData)safeXmlSerializer.Deserialize(stringReader3);
				}
				if (weatherLocationData == null)
				{
					innerErrorMessage = "We received an empty response from the weather service";
				}
				else
				{
					if (weatherLocationData.Items == null)
					{
						innerErrorMessage = "The response from the weather service does not contain a collection of weather locations";
						return;
					}
					locationIds.AddRange(from item in weatherLocationData.Items
					select new WeatherLocationId
					{
						Latitude = item.Latitude,
						Longitude = item.Longitude,
						Name = FindWeatherLocations.GetLocationNameFrom(item),
						EntityId = item.EntityId.ToString(CultureInfo.InvariantCulture)
					});
					return;
				}
			});
			return new FindWeatherLocationsResponse
			{
				WeatherLocationIds = locationIds.ToArray(),
				ErrorMessage = (string.IsNullOrWhiteSpace(text2) ? innerErrorMessage : text2)
			};
		}

		private static string GetLocationNameFrom(IWeatherLocation location)
		{
			if (string.IsNullOrEmpty(location.Name))
			{
				return location.FullName;
			}
			if (string.IsNullOrEmpty(location.FullName))
			{
				return location.Name;
			}
			if (location.Name.Length >= location.FullName.Length)
			{
				return location.FullName;
			}
			return location.Name;
		}

		private const string OutputViewFieldValue = "searchLocations";

		private static readonly int FindWeatherLocationsTraceId = typeof(FindWeatherLocations).GetHashCode();

		private readonly FindWeatherLocationsRequest request;

		private readonly ServiceAsyncResult<Task<FindWeatherLocationsResponse>> asyncResult;

		private readonly IWeatherService weatherService;
	}
}
