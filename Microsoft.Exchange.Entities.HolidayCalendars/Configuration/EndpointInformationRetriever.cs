using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Entities.HolidayCalendars;
using Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class EndpointInformationRetriever : IEndpointInformationRetriever
	{
		public EndpointInformationRetriever(Uri baseUrl, int timeout, IHolidayCalendarsService service = null)
		{
			if (baseUrl == null)
			{
				throw new ArgumentNullException("baseUrl");
			}
			this.baseUrl = baseUrl;
			this.timeout = timeout;
			this.service = (service ?? new HolidayCalendarsService());
		}

		public EndpointInformation FetchEndpointInformation()
		{
			IReadOnlyDictionary<int, int> versionInfo = null;
			IReadOnlyList<CultureInfo> cultures = null;
			try
			{
				Action[] array = new Action[2];
				array[0] = delegate()
				{
					versionInfo = this.GetCalendarVersionMapping();
				};
				array[1] = delegate()
				{
					cultures = this.GetListOfAllCultures();
				};
				Parallel.Invoke(array);
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException;
			}
			return new EndpointInformation(this.baseUrl, versionInfo, cultures, this.GetVersionFromUriPath());
		}

		private Dictionary<int, int> GetCalendarVersionMapping()
		{
			HttpWebRequest request = this.CreateRequest("calendarversions.txt");
			Dictionary<int, int> versions = new Dictionary<int, int>(120);
			try
			{
				this.service.GetResource(request, delegate(Stream response)
				{
					using (StreamReader streamReader = new StreamReader(response, Encoding.UTF8))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							if (!string.IsNullOrWhiteSpace(text))
							{
								int[] array = (from s in text.Trim().Split(new char[]
								{
									','
								})
								select int.Parse(s, CultureInfo.InvariantCulture)).ToArray<int>();
								versions.Add(array[0], array[1]);
							}
						}
					}
				});
			}
			catch (WebException ex)
			{
				throw new EndpointConfigurationException(EndPointConfigurationError.UnableToFetchCalendarVersions, ex, "Status: {0}, Endpoint: {1}", new object[]
				{
					ex.Status.ToString(),
					this.baseUrl.AbsoluteUri
				});
			}
			return versions;
		}

		private HttpWebRequest CreateRequest(string resourceFileName)
		{
			HttpWebRequest result;
			try
			{
				Uri requestUri = new Uri(this.baseUrl, resourceFileName);
				HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
				if (httpWebRequest == null)
				{
					throw new EndpointConfigurationException(EndPointConfigurationError.UrlDidNotResolveToHttpRequest, "Url: {0}", new object[]
					{
						this.baseUrl.AbsoluteUri
					});
				}
				httpWebRequest.Timeout = this.timeout;
				result = httpWebRequest;
			}
			catch (NotSupportedException innerException)
			{
				throw new EndpointConfigurationException(EndPointConfigurationError.UrlSchemeNotSupported, innerException, "Url: {0}", new object[]
				{
					this.baseUrl.AbsoluteUri
				});
			}
			return result;
		}

		private List<CultureInfo> GetListOfAllCultures()
		{
			HttpWebRequest request = this.CreateRequest("cultures.txt");
			List<CultureInfo> cultures = new List<CultureInfo>(120);
			try
			{
				this.service.GetResource(request, delegate(Stream response)
				{
					using (StreamReader streamReader = new StreamReader(response, Encoding.UTF8))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							if (!string.IsNullOrWhiteSpace(text))
							{
								try
								{
									CultureInfo cultureInfo = CultureInfo.GetCultureInfo(text.Trim());
									cultures.Add(cultureInfo);
								}
								catch (CultureNotFoundException)
								{
									ExTraceGlobals.EndpointConfigurationRetrieverTracer.TraceDebug<string>((long)this.GetHashCode(), "Unable to add culture {0} to list.", text);
								}
							}
						}
					}
				});
			}
			catch (WebException ex)
			{
				throw new EndpointConfigurationException(EndPointConfigurationError.UnableToFetchListOfCultures, ex, "Status: {0}, Endpoint: {1}", new object[]
				{
					ex.Status.ToString(),
					this.baseUrl.AbsoluteUri
				});
			}
			return cultures;
		}

		private Version GetVersionFromUriPath()
		{
			string text = this.baseUrl.Segments.Last<string>().TrimEnd(new char[]
			{
				'/'
			});
			Version result;
			if (!Version.TryParse(text, out result))
			{
				throw new EndpointConfigurationException(EndPointConfigurationError.VersionNumberError, "Cannot parse version number. {0}", new object[]
				{
					text
				});
			}
			return result;
		}

		public const string CultureFileRelativePath = "cultures.txt";

		public const string VersionsFileRelativePath = "calendarversions.txt";

		private readonly Uri baseUrl;

		private readonly int timeout;

		private readonly IHolidayCalendarsService service;
	}
}
