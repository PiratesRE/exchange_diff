using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetWeatherForecast : ServiceCommand<IAsyncResult>
	{
		public GetWeatherForecast(CallContext context, GetWeatherForecastRequest request, AsyncCallback asyncCallback, object asyncState, IWeatherService weatherService) : base(context)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "GetWeatherForecast::ctor");
			WcfServiceCommandBase.ThrowIfNull(asyncCallback, "asyncCallback", "GetWeatherForecast::ctor");
			WcfServiceCommandBase.ThrowIfNull(weatherService, "weatherService", "GetWeatherForecast::ctor");
			OwsLogRegistry.Register(typeof(GetWeatherForecast).Name, typeof(WeatherMetadata), new Type[0]);
			this.request = request;
			this.asyncResult = new ServiceAsyncResult<Task<GetWeatherForecastResponse>>();
			this.asyncResult.AsyncCallback = asyncCallback;
			this.asyncResult.AsyncState = asyncState;
			this.weatherService = weatherService;
		}

		protected override IAsyncResult InternalExecute()
		{
			GetWeatherForecast.TraceRequest(this.request);
			this.weatherService.VerifyServiceAvailability(base.CallContext);
			Task<GetWeatherForecastResponse> weatherForecastResponseAsync = GetWeatherForecast.GetWeatherForecastResponseAsync(this.request, base.CallContext, this.weatherService);
			weatherForecastResponseAsync.ContinueWith(delegate(Task<GetWeatherForecastResponse> task)
			{
				this.asyncResult.Complete(task);
			});
			this.asyncResult.Data = weatherForecastResponseAsync;
			return this.asyncResult;
		}

		internal static string ConstructLocationString(WeatherLocationId[] weatherLocationIds)
		{
			switch (weatherLocationIds.Length)
			{
			case 0:
				return WeatherCommon.FormatWebFormField("weasearchstr", string.Empty);
			case 1:
			{
				WeatherLocationId weatherLocationId = weatherLocationIds[0];
				return WeatherCommon.FormatWebFormField("weasearchstr", string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
				{
					weatherLocationId.Latitude,
					weatherLocationId.Longitude
				}));
			}
			default:
				return WeatherCommon.FormatWebFormField("wealocations", string.Join("\r", (from locationId in weatherLocationIds
				select string.Format("ei:{0}", locationId.EntityId)).ToList<string>()));
			}
		}

		internal static Uri ConstructUriForLocationId(string locationString, string temperatureUnit, string culture, IWeatherService weatherService)
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
				WeatherCommon.FormatWebFormField("outputview", "standard"),
				locationString,
				WeatherCommon.FormatWebFormField("weadegreetype", temperatureUnit)
			});
			ExTraceGlobals.WeatherTracer.TraceDebug<string>((long)GetWeatherForecast.GetWeatherForecastTraceId, "Constructed URI string is '{0}'", text);
			return new Uri(text);
		}

		private static void TraceRequest(GetWeatherForecastRequest request)
		{
			if (request == null)
			{
				ExTraceGlobals.WeatherTracer.TraceDebug((long)GetWeatherForecast.GetWeatherForecastTraceId, "No request");
				return;
			}
			ExTraceGlobals.WeatherTracer.TraceDebug<TemperatureUnit>((long)GetWeatherForecast.GetWeatherForecastTraceId, "Temperature unit: {0}", request.TemperatureUnit);
			ExTraceGlobals.WeatherTracer.TraceDebug<int>((long)GetWeatherForecast.GetWeatherForecastTraceId, "Number of locations: {0}", request.WeatherLocationIds.Length);
			foreach (WeatherLocationId weatherLocationId in request.WeatherLocationIds)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "{0}: {1}, {2} ({3}, {4}, {5})", new object[]
				{
					weatherLocationId.Name,
					weatherLocationId.Latitude,
					weatherLocationId.Longitude,
					WeatherCommon.FormatLatitude(weatherLocationId.Latitude),
					WeatherCommon.FormatLongitude(weatherLocationId.Longitude),
					WeatherCommon.FormatEntityId(weatherLocationId.EntityId)
				});
				ExTraceGlobals.WeatherTracer.TraceDebug((long)GetWeatherForecast.GetWeatherForecastTraceId, message);
			}
		}

		private static async Task<GetWeatherForecastResponse> GetWeatherForecastResponseAsync(GetWeatherForecastRequest request, CallContext callContext, IWeatherService weatherService)
		{
			RegionInfo region = new RegionInfo(callContext.OwaCulture.Name);
			string temperatureUnit = GetWeatherForecast.GetValidTemperatureUnit(request.TemperatureUnit, region);
			WeatherLocationId[] weatherLocationIds = request.WeatherLocationIds;
			bool canExecuteAsBatch = true;
			if (weatherLocationIds.Any((WeatherLocationId weatherLocationId) => string.IsNullOrEmpty(weatherLocationId.EntityId)))
			{
				ExTraceGlobals.WeatherTracer.TraceInformation(GetWeatherForecast.GetWeatherForecastTraceId, 0L, "Request failed due to exception {0}");
				canExecuteAsBatch = false;
			}
			callContext.ProtocolLog.Set(WeatherMetadata.LocationsCount, weatherLocationIds.Length);
			callContext.ProtocolLog.Set(WeatherMetadata.Culture, callContext.OwaCulture.Name);
			string errorMessage = null;
			Stopwatch stopWatch = Stopwatch.StartNew();
			Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[] weatherLocations = canExecuteAsBatch ? (await Task.Run<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[]>(() => GetWeatherForecast.GetWeather(weatherLocationIds, temperatureUnit, callContext, weatherService, out errorMessage))) : (await GetWeatherForecast.GetWeatherForAllLocations(weatherLocationIds, temperatureUnit, callContext, weatherService));
			stopWatch.Stop();
			callContext.ProtocolLog.Set(WeatherMetadata.SearchLocationsLatency, stopWatch.ElapsedMilliseconds);
			return new GetWeatherForecastResponse
			{
				ErrorMessage = errorMessage,
				PollingWindowInMinutes = 60,
				WeatherLocations = weatherLocations,
				TemperatureUnit = GetWeatherForecast.GetTemperatureUnitWithFallback(request.TemperatureUnit, region)
			};
		}

		private static async Task<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[]> GetWeatherForAllLocations(IEnumerable<WeatherLocationId> weatherLocationIds, string temperatureUnit, CallContext callContext, IWeatherService weatherService)
		{
			List<Task<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation>> tasks = (from id in weatherLocationIds
			select Task.Run<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation>(() => GetWeatherForecast.GetWeather(id, temperatureUnit, callContext, weatherService))).ToList<Task<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation>>();
			return await Task.WhenAll<Microsoft.Exchange.Services.Wcf.Types.WeatherLocation>(tasks);
		}

		private static string GetValidTemperatureUnit(TemperatureUnit temperatureUnit, RegionInfo regionInfo)
		{
			temperatureUnit = GetWeatherForecast.GetTemperatureUnitWithFallback(temperatureUnit, regionInfo);
			if (temperatureUnit != TemperatureUnit.Fahrenheit)
			{
				return "C";
			}
			return "F";
		}

		private static TemperatureUnit GetTemperatureUnitWithFallback(TemperatureUnit temperatureUnit, RegionInfo regionInfo)
		{
			switch (temperatureUnit)
			{
			case TemperatureUnit.Celsius:
			case TemperatureUnit.Fahrenheit:
				return temperatureUnit;
			default:
				if (!regionInfo.IsMetric)
				{
					return TemperatureUnit.Fahrenheit;
				}
				return TemperatureUnit.Celsius;
			}
		}

		private static Microsoft.Exchange.Services.Wcf.Types.WeatherLocation GetWeather(WeatherLocationId weatherLocationId, string temperatureUnit, CallContext callContext, IWeatherService weatherService)
		{
			string text;
			Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[] weather = GetWeatherForecast.GetWeather(new WeatherLocationId[]
			{
				weatherLocationId
			}, temperatureUnit, callContext, weatherService, out text);
			if (weather == null || weather.Length == 0 || weather[0] == null)
			{
				return new Microsoft.Exchange.Services.Wcf.Types.WeatherLocation
				{
					ErrorMessage = text,
					Id = weatherLocationId
				};
			}
			Microsoft.Exchange.Services.Wcf.Types.WeatherLocation weatherLocation = weather[0];
			weatherLocation.ErrorMessage = (weather[0].ErrorMessage ?? text);
			return weatherLocation;
		}

		private static Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[] GetWeather(WeatherLocationId[] weatherLocationIds, string temperatureUnit, CallContext callContext, IWeatherService weatherService, out string errorMessage)
		{
			string weatherLocationQuery = GetWeatherForecast.ConstructLocationString(weatherLocationIds);
			string text = CoreResources.MessageCouldNotGetWeatherDataForLocation;
			errorMessage = ((text == null) ? weatherLocationQuery : string.Format(callContext.OwaCulture, text, new object[]
			{
				weatherLocationQuery
			}));
			Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[] weatherLocations = new Microsoft.Exchange.Services.Wcf.Types.WeatherLocation[weatherLocationIds.Length];
			errorMessage = WeatherCommon.ExecuteActionAndHandleException(callContext, GetWeatherForecast.GetWeatherForecastTraceId, errorMessage, delegate
			{
				string name = callContext.OwaCulture.Name;
				Uri weatherServiceUri = GetWeatherForecast.ConstructUriForLocationId(weatherLocationQuery, temperatureUnit, name, weatherService);
				string text2 = weatherService.Get(weatherServiceUri);
				if (string.IsNullOrWhiteSpace(text2))
				{
					throw new WeatherException("Weather service response string is null or empty");
				}
				if (text2.Length > 300000)
				{
					throw new WeatherException("Weather service response string is too long");
				}
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(WeatherData));
				WeatherData weatherData;
				using (StringReader stringReader = new StringReader(text2))
				{
					weatherData = (WeatherData)safeXmlSerializer.Deserialize(stringReader);
				}
				if (weatherData == null)
				{
					throw new WeatherException("We received an empty response from the weather service");
				}
				if (weatherData.Items == null)
				{
					throw new WeatherException("The response from the weather service does not contain a collection of weather locations");
				}
				if (weatherData.Items.Length != weatherLocationIds.Length)
				{
					throw new WeatherException(string.Format(CultureInfo.InvariantCulture, "The weather service returned unexpected number of weather locations ({0}) when we were expecting exactly {1} location(s)", new object[]
					{
						weatherData.Items.Length,
						weatherLocationIds.Length
					}));
				}
				for (int i = 0; i < weatherData.Items.Length; i++)
				{
					Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4.WeatherLocation weatherLocation = weatherData.Items[i];
					if (!string.IsNullOrEmpty(weatherLocation.ErrorMessage))
					{
						weatherLocations[i] = new Microsoft.Exchange.Services.Wcf.Types.WeatherLocation
						{
							ErrorMessage = weatherLocation.ErrorMessage,
							Id = weatherLocationIds[i]
						};
					}
					else
					{
						WeatherCurrentConditions conditions = GetWeatherForecast.CreateCurrentConditionsFromResponse(weatherLocation);
						WeatherMultidayForecast multidayForecast = GetWeatherForecast.CreateForecastFromResponse(weatherLocation, 3);
						weatherLocations[i] = new Microsoft.Exchange.Services.Wcf.Types.WeatherLocation
						{
							Conditions = conditions,
							MultidayForecast = multidayForecast,
							Id = weatherLocationIds[i]
						};
					}
				}
			});
			return weatherLocations;
		}

		private static WeatherMultidayForecast CreateForecastFromResponse(IWeatherLocation locationResponse, int forecastDays)
		{
			GetWeatherForecast.ThrowIfNull(locationResponse, "location response");
			WeatherForecast[] forecast = locationResponse.Forecast;
			GetWeatherForecast.ThrowIfNull(forecast, "forecast");
			List<WeatherDailyConditions> list = new List<WeatherDailyConditions>();
			int num = Math.Min(forecast.Length, forecastDays);
			for (int i = 0; i < num; i++)
			{
				WeatherForecast weatherForecast = forecast[i];
				GetWeatherForecast.ThrowIfNull(weatherForecast, "daily conditions");
				list.Add(new WeatherDailyConditions
				{
					High = weatherForecast.High,
					Low = weatherForecast.Low,
					PrecipitationChance = weatherForecast.PrecipitationCertainty.ToString(CultureInfo.InvariantCulture),
					SkyCode = weatherForecast.SkyCodeDay,
					SkyText = weatherForecast.SkyTextDay
				});
			}
			return new WeatherMultidayForecast
			{
				Attribution = new WeatherProviderAttribution
				{
					Link = locationResponse.Url,
					Text = locationResponse.Attribution
				},
				DailyForecasts = list.ToArray()
			};
		}

		private static WeatherCurrentConditions CreateCurrentConditionsFromResponse(IWeatherLocation locationResponse)
		{
			GetWeatherForecast.ThrowIfNull(locationResponse, "location response");
			string shortAttribution = locationResponse.ShortAttribution;
			WeatherConditions conditions = locationResponse.Conditions;
			GetWeatherForecast.ThrowIfNull(conditions, "current conditions");
			return new WeatherCurrentConditions
			{
				Attribution = new WeatherProviderAttribution
				{
					Link = locationResponse.Url,
					Text = shortAttribution
				},
				SkyCode = conditions.SkyCode,
				SkyText = conditions.SkyText,
				Temperature = conditions.Temperature
			};
		}

		private static void ThrowIfNull(object theObject, string identifier)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "{0} should not be null", new object[]
			{
				identifier
			});
			if (theObject == null)
			{
				throw new WeatherException(message);
			}
		}

		internal const int DefaultPollingWindowInMinutes = 60;

		private const string OutputViewFieldValue = "standard";

		private const string CelsiusValue = "C";

		private const string FahrenheitValue = "F";

		private const int MaxForecastDays = 3;

		private const string TemperatureUnitFieldName = "weadegreetype";

		private static readonly int GetWeatherForecastTraceId = typeof(GetWeatherForecast).GetHashCode();

		private readonly GetWeatherForecastRequest request;

		private readonly ServiceAsyncResult<Task<GetWeatherForecastResponse>> asyncResult;

		private readonly IWeatherService weatherService;
	}
}
