using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class SourceDataProviderManager
	{
		public SourceDataProviderManager(IServiceClientFactory serviceClientFactory, CancellationToken abortTokenForTasks)
		{
			this.serviceClientFactory = serviceClientFactory;
			this.abortTokenForTasks = abortTokenForTasks;
		}

		public IProgressController ProgressController { get; set; }

		private bool IsStopRequested
		{
			get
			{
				return this.abortTokenForTasks.IsCancellationRequested;
			}
		}

		public void AutoDiscoverSourceServiceEndpoints(SourceInformationCollection allSourceInformation, Uri configurationServiceEndpoint, ICredentialHandler credentialHandler)
		{
			this.serviceClientFactory.CredentialHandler = credentialHandler;
			if (configurationServiceEndpoint != null && configurationServiceEndpoint.IsAbsoluteUri && configurationServiceEndpoint.Scheme == Uri.UriSchemeHttps)
			{
				if (this.IsStopRequested)
				{
					Tracer.TraceInformation("SourceDataProviderManager.CreateServiceClients: Stop requested when trying to auto discover with configuration service server", new object[0]);
					return;
				}
				Uri autoDiscoverUrl = new Uri(configurationServiceEndpoint.GetLeftPart(UriPartial.Authority) + "/autodiscover/autodiscover.svc");
				this.AutoDiscoverServiceEndpoints((from sourceInformation in allSourceInformation.Values
				where this.IsServiceEndpointNeededForSource(sourceInformation)
				select sourceInformation).ToList<SourceInformation>(), autoDiscoverUrl, 0);
				if (this.IsStopRequested)
				{
					Tracer.TraceInformation("SourceDataProviderManager.CreateServiceClients: Stop requested when trying to auto discover with configuration service endpoint", new object[0]);
					return;
				}
				using (IServiceClient<ISourceDataProvider> serviceClient = this.serviceClientFactory.CreateSourceDataProvider(configurationServiceEndpoint, this.abortTokenForTasks))
				{
					if (serviceClient.Connect())
					{
						SourceDataProviderManager.VerifyAndSetServiceEndpoint((from sourceInformation in allSourceInformation.Values
						where this.IsServiceEndpointNeededForSource(sourceInformation)
						select sourceInformation).ToList<SourceInformation>(), serviceClient);
					}
				}
			}
			IEnumerable<IGrouping<string, SourceInformation>> enumerable = from sourceInformation in allSourceInformation.Values
			where this.IsServiceEndpointNeededForSource(sourceInformation)
			group sourceInformation by SourceDataProviderManager.GetDomainFromSmtpEmailAddress(sourceInformation.Configuration.Id.StartsWith("\\") ? sourceInformation.Configuration.Name : sourceInformation.Configuration.Id);
			foreach (IGrouping<string, SourceInformation> grouping in enumerable)
			{
				if (this.IsStopRequested)
				{
					Tracer.TraceInformation("SourceDataProviderManager.CreateServiceClients: Stop requested when trying to auto discover with email domains of source mailboxes.", new object[0]);
					return;
				}
				this.AutoDiscoverServiceEndpointsWithEmailDomain(grouping.ToList<SourceInformation>(), grouping.Key.ToLowerInvariant());
			}
			SourceInformation sourceInformation2 = allSourceInformation.Values.FirstOrDefault((SourceInformation sourceInformation) => this.IsServiceEndpointNeededForSource(sourceInformation));
			if (sourceInformation2 != null && !this.IsStopRequested)
			{
				throw new ExportException(ExportErrorType.FailedToAutoDiscoverExchangeWebServiceUrl, sourceInformation2.Configuration.Id);
			}
		}

		public void CreateSourceServiceClients(SourceInformationCollection allSourceInformation)
		{
			Dictionary<Uri, IServiceClient<ISourceDataProvider>> dictionary = new Dictionary<Uri, IServiceClient<ISourceDataProvider>>(allSourceInformation.Count);
			foreach (SourceInformation sourceInformation in allSourceInformation.Values)
			{
				if (this.IsStopRequested)
				{
					Tracer.TraceInformation("SourceDataProviderManager.CreateServiceClients: Stop requested when creating service clients.", new object[0]);
					return;
				}
				if (this.IsServiceClientNeededForSource(sourceInformation))
				{
					IServiceClient<ISourceDataProvider> serviceClient;
					if (!dictionary.TryGetValue(sourceInformation.Configuration.ServiceEndpoint, out serviceClient))
					{
						serviceClient = this.serviceClientFactory.CreateSourceDataProvider(sourceInformation.Configuration.ServiceEndpoint, this.abortTokenForTasks);
						if (!serviceClient.Connect())
						{
							throw new ExportException(ExportErrorType.Unauthorized, sourceInformation.Configuration.ServiceEndpoint.ToString());
						}
						dictionary.Add(sourceInformation.Configuration.ServiceEndpoint, serviceClient);
					}
					sourceInformation.ServiceClient = serviceClient.FunctionalInterface;
				}
			}
			Tracer.TraceInformation("SourceDataProviderManager.CreateSourceServiceClients: number of service clients created: {0}", new object[]
			{
				dictionary.Count
			});
		}

		private static void VerifyAndSetServiceEndpoint(IEnumerable<SourceInformation> sourcesToVerify, IServiceClient<ISourceDataProvider> serviceClient)
		{
			foreach (SourceInformation sourceInformation in sourcesToVerify)
			{
				try
				{
					ISourceDataProvider functionalInterface = serviceClient.FunctionalInterface;
					string rootFolder = functionalInterface.GetRootFolder(sourceInformation.Configuration.Id.StartsWith("\\") ? sourceInformation.Configuration.Name : sourceInformation.Configuration.Id, false);
					if (!string.IsNullOrEmpty(rootFolder))
					{
						sourceInformation.Configuration.ServiceEndpoint = serviceClient.ServiceEndpoint;
					}
				}
				catch (ExportException)
				{
				}
			}
		}

		private static string GetDomainFromSmtpEmailAddress(string email)
		{
			return email.Split(new char[]
			{
				'@'
			})[1].ToLowerInvariant();
		}

		private void AutoDiscoverServiceEndpointsWithEmailDomain(IList<SourceInformation> sources, string emailDomain)
		{
			IList<SourceInformation> list = sources;
			string[] array = new string[]
			{
				string.Format("https://autodiscover.{0}/autodiscover/autodiscover.svc", emailDomain),
				string.Format("http://autodiscover.{0}/autodiscover/autodiscover.svc", emailDomain),
				string.Format("https://{0}/autodiscover/autodiscover.svc", emailDomain),
				string.Format("http://{0}/autodiscover/autodiscover.svc", emailDomain)
			};
			int num = 0;
			while (list.Count > 0 && num < array.Length)
			{
				string uriString = array[num++];
				this.AutoDiscoverServiceEndpoints(list, new Uri(uriString), 0);
				list = (from sourceInformation in sources
				where this.IsServiceEndpointNeededForSource(sourceInformation)
				select sourceInformation).ToList<SourceInformation>();
			}
		}

		private void AutoDiscoverServiceEndpoints(IList<SourceInformation> sources, Uri autoDiscoverUrl, int retriedTimes)
		{
			if (retriedTimes >= 3 && sources != null && sources.Count > 0)
			{
				Tracer.TraceError("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Failed to auto discover service endpoint for mailbox {0} after retries for transient errors.", new object[]
				{
					sources[0].Configuration.Id
				});
				return;
			}
			if (autoDiscoverUrl != null)
			{
				List<SourceInformation> list = new List<SourceInformation>(ConstantProvider.AutoDiscoverBatchSize);
				for (int i = 0; i < sources.Count; i++)
				{
					if (this.IsStopRequested)
					{
						Tracer.TraceInformation("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Stop requested when auto discovering from '{0}'.", new object[]
						{
							autoDiscoverUrl
						});
						return;
					}
					list.Add(sources[i]);
					if (list.Count >= ConstantProvider.AutoDiscoverBatchSize || i >= sources.Count - 1)
					{
						IServiceClient<IAutoDiscoverClient> serviceClient = this.serviceClientFactory.CreateAutoDiscoverClient(autoDiscoverUrl, this.abortTokenForTasks);
						List<AutoDiscoverResult> userEwsEndpoints;
						try
						{
							userEwsEndpoints = serviceClient.FunctionalInterface.GetUserEwsEndpoints(list.Select(delegate(SourceInformation sourceInformation)
							{
								if (!sourceInformation.Configuration.Id.StartsWith("\\"))
								{
									return sourceInformation.Configuration.Id;
								}
								return sourceInformation.Configuration.Name;
							}));
						}
						catch (ExportException ex)
						{
							Tracer.TraceError("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Exeption happened when calling GetUserEwsEndpoints. Exception: {0}", new object[]
							{
								ex
							});
							list.Clear();
							break;
						}
						Dictionary<Uri, List<SourceInformation>> dictionary = new Dictionary<Uri, List<SourceInformation>>();
						Dictionary<string, List<SourceInformation>> dictionary2 = new Dictionary<string, List<SourceInformation>>();
						List<SourceInformation> list2 = new List<SourceInformation>();
						for (int j = 0; j < userEwsEndpoints.Count; j++)
						{
							AutoDiscoverResult autoDiscoverResult = userEwsEndpoints[j];
							Uri uri = null;
							string text = null;
							switch (autoDiscoverResult.ResultCode)
							{
							case AutoDiscoverResultCode.Success:
								list[j].Configuration.ServiceEndpoint = new Uri(autoDiscoverResult.ResultValue);
								break;
							case AutoDiscoverResultCode.TransientError:
								list2.Add(list[j]);
								break;
							case AutoDiscoverResultCode.EmailAddressRedirected:
								text = SourceDataProviderManager.GetDomainFromSmtpEmailAddress(autoDiscoverResult.ResultValue);
								break;
							case AutoDiscoverResultCode.UrlRedirected:
								uri = new Uri(autoDiscoverResult.ResultValue);
								break;
							}
							if (uri != null)
							{
								List<SourceInformation> list3;
								if (!dictionary.TryGetValue(uri, out list3))
								{
									list3 = new List<SourceInformation>();
									dictionary.Add(uri, list3);
								}
								list3.Add(list[j]);
							}
							if (!string.IsNullOrEmpty(text))
							{
								List<SourceInformation> list4;
								if (!dictionary2.TryGetValue(text, out list4))
								{
									list4 = new List<SourceInformation>();
									dictionary2.Add(text, list4);
								}
								list4.Add(list[j]);
							}
						}
						foreach (Uri uri2 in dictionary.Keys)
						{
							Tracer.TraceInformation("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Further autodiscover via {0}", new object[]
							{
								uri2
							});
							this.AutoDiscoverServiceEndpoints(dictionary[uri2], uri2, 0);
						}
						foreach (string text2 in dictionary2.Keys)
						{
							Tracer.TraceInformation("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Further autodiscover with email domain {0}", new object[]
							{
								text2
							});
							this.AutoDiscoverServiceEndpointsWithEmailDomain(dictionary2[text2], text2);
						}
						if (list2.Count > 0)
						{
							Tracer.TraceInformation("SourceDataProviderManager.AutoDiscoverServiceEndpoints: Retry autodiscover via {0}, retring times: {1}", new object[]
							{
								autoDiscoverUrl,
								retriedTimes + 1
							});
							this.AutoDiscoverServiceEndpoints(list2, autoDiscoverUrl, retriedTimes + 1);
						}
						list.Clear();
					}
				}
			}
		}

		private bool IsServiceEndpointNeededForSource(SourceInformation source)
		{
			return source.ServiceClient == null && source.Configuration.ServiceEndpoint == null;
		}

		private bool IsServiceClientNeededForSource(SourceInformation source)
		{
			return source.ServiceClient == null;
		}

		private readonly IServiceClientFactory serviceClientFactory;

		private readonly CancellationToken abortTokenForTasks;
	}
}
