using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.EventLog;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ScpSearch
	{
		private ScpSearch(ADServiceConnectionPoint[] results)
		{
			this.results = results;
		}

		private ScpSearch(Exception exception)
		{
			this.Exception = exception;
		}

		public Exception Exception { get; private set; }

		public static ScpSearch FindLocal()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 114, "FindLocal", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\ScpSearch.cs");
			ScpSearch scpSearch = ScpSearch.Find(tenantOrTopologyConfigurationSession, ScpSearch.localQueryFilter);
			if (scpSearch != null && scpSearch.Exception != null)
			{
				Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_SCPErrorSearchingLocalADForSCP, null, new object[]
				{
					Globals.ProcessId,
					scpSearch.Exception
				});
			}
			return scpSearch;
		}

		public Uri FindRemote(string domainName, NetworkCredential networkCredential)
		{
			if (this.results == null || this.results.Length == 0)
			{
				return null;
			}
			ADServiceConnectionPoint[] orderedByPriority = ScpSearch.GetOrderedByPriority(this.results, (ADServiceConnectionPoint scp) => ScpSearch.CalculatePriorityForLocal(scp, domainName));
			List<ScpSearch.ServiceBindingInformation> serviceBindingInformation = ScpSearch.GetServiceBindingInformation(orderedByPriority);
			if (serviceBindingInformation == null)
			{
				Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_SCPMisconfiguredRemoteServiceBindings, null, new object[]
				{
					Globals.ProcessId,
					domainName
				});
				return null;
			}
			using (List<ScpSearch.ServiceBindingInformation>.Enumerator enumerator = serviceBindingInformation.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ScpSearch.<>c__DisplayClass4 CS$<>8__locals2 = new ScpSearch.<>c__DisplayClass4();
					CS$<>8__locals2.serviceBindingInformation = enumerator.Current;
					ScpSearch.ConfigurationTracer.TraceDebug<string>(0L, "Creating AD remote session to {0}", CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host);
					IConfigurationSession remoteSession = null;
					Exception ex = ScpSearch.PerformRetryableAdOperation("remote AD session", delegate
					{
						remoteSession = ADSystemConfigurationSession.CreateRemoteForestSession(CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host, networkCredential);
					});
					if (ex == null)
					{
						ScpSearch.ConfigurationTracer.TraceDebug<string>(0L, "Successful creation of AD remote session to {0}", CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host);
						ScpSearch scpSearch = ScpSearch.Find(remoteSession, ScpSearch.remoteQueryFilter);
						if (scpSearch.Exception == null)
						{
							ADServiceConnectionPoint[] orderedByPriority2 = ScpSearch.GetOrderedByPriority(scpSearch.results, new Converter<ADServiceConnectionPoint, int>(ScpSearch.CalculatePriorityForRemote));
							List<ScpSearch.ServiceBindingInformation> serviceBindingInformation2 = ScpSearch.GetServiceBindingInformation(orderedByPriority2);
							if (serviceBindingInformation2 == null || serviceBindingInformation2.Count == 0)
							{
								Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_SCPMisconfiguredRemoteServiceBindings, null, new object[]
								{
									Globals.ProcessId,
									CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host
								});
								return null;
							}
							return serviceBindingInformation2[0].ServiceBinding;
						}
						else
						{
							Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_SCPErrorSearchingForRemoteSCP, null, new object[]
							{
								Globals.ProcessId,
								CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host,
								ex
							});
						}
					}
					else
					{
						Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_SCPCannotConnectToRemoteDirectory, null, new object[]
						{
							Globals.ProcessId,
							CS$<>8__locals2.serviceBindingInformation.ServiceBinding.Host,
							CS$<>8__locals2.serviceBindingInformation.SCP.DistinguishedName,
							ex
						});
					}
				}
			}
			return null;
		}

		private static List<ScpSearch.ServiceBindingInformation> GetServiceBindingInformation(ADServiceConnectionPoint[] scpArray)
		{
			if (scpArray == null || scpArray.Length == 0)
			{
				ScpSearch.ConfigurationTracer.TraceError(0L, "No SCP objects returned from the AD query.");
				return null;
			}
			List<ScpSearch.ServiceBindingInformation> list = new List<ScpSearch.ServiceBindingInformation>(scpArray.Length);
			foreach (ADServiceConnectionPoint adserviceConnectionPoint in scpArray)
			{
				using (MultiValuedProperty<string>.Enumerator enumerator = adserviceConnectionPoint.ServiceBindingInformation.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string serviceBindingInformationItem = enumerator.Current;
						if (Uri.IsWellFormedUriString(serviceBindingInformationItem, UriKind.Absolute))
						{
							if (!list.Exists((ScpSearch.ServiceBindingInformation listItem) => StringComparer.InvariantCultureIgnoreCase.Equals(listItem, serviceBindingInformationItem)))
							{
								list.Add(new ScpSearch.ServiceBindingInformation
								{
									ServiceBinding = new Uri(serviceBindingInformationItem),
									SCP = adserviceConnectionPoint
								});
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				ScpSearch.ConfigurationTracer.TraceError(0L, "SCP objects returned from the AD query have no valid service binding information.");
				return null;
			}
			return list;
		}

		private static ScpSearch Find(IConfigurationSession session, QueryFilter filter)
		{
			ScpSearch.ConfigurationTracer.TraceDebug<string, QueryFilter>(0L, "Searching for ADServiceConnectionPoint objects in the AD at {0} using filter {1}", session.DomainController ?? "<any local DC>", filter);
			ADServiceConnectionPoint[] results = null;
			Exception ex = null;
			try
			{
				ex = ScpSearch.PerformRetryableAdOperation("query for SCP", delegate
				{
					results = session.Find<ADServiceConnectionPoint>(null, QueryScope.SubTree, filter, null, 400);
				});
			}
			catch (DataValidationException ex2)
			{
				ex = ex2;
			}
			catch (ADOperationException ex3)
			{
				ex = ex3;
			}
			catch (UriFormatException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ScpSearch.ConfigurationTracer.TraceError<Exception>(0L, "Failed to find ADServiceConnectionPoint objects in the AD due exception: {0}.", ex);
				return new ScpSearch(ex);
			}
			if (ScpSearch.ConfigurationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ScpSearch.ConfigurationTracer.TraceDebug<int>(0L, "Search for ADServiceConnectionPoint objects in the AD returned {0} objects.", (results == null) ? 0 : results.Length);
				foreach (ADServiceConnectionPoint adserviceConnectionPoint in results)
				{
					ScpSearch.ConfigurationTracer.TraceDebug<ADObjectId, string, string>(0L, "Found ADServiceConnectionPoint object {0}, Keywords={1}, ServiceBindingInformation={2}", adserviceConnectionPoint.Id, string.Join(";", adserviceConnectionPoint.Keywords.ToArray()), string.Join(";", adserviceConnectionPoint.ServiceBindingInformation.ToArray()));
				}
			}
			return new ScpSearch(results);
		}

		private static ADServiceConnectionPoint[] GetOrderedByPriority(ADServiceConnectionPoint[] scps, Converter<ADServiceConnectionPoint, int> converter)
		{
			ADServiceConnectionPoint[] array = new ADServiceConnectionPoint[scps.Length];
			Array.Copy(scps, array, scps.Length);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = converter(array[i]);
			}
			Array.Sort<int, ADServiceConnectionPoint>(array2, array);
			return array;
		}

		private static int CalculatePriorityForLocal(ADServiceConnectionPoint scp, string domainName)
		{
			if (scp.Keywords != null && scp.Keywords.Count > 0)
			{
				string y = "domain=" + domainName;
				bool flag = false;
				foreach (string text in scp.Keywords)
				{
					if (!string.IsNullOrEmpty(text))
					{
						if (StringComparer.InvariantCultureIgnoreCase.Equals(text, y))
						{
							return 0;
						}
						if (text.StartsWith("domain=", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					return 1;
				}
				return 2;
			}
			return 2;
		}

		private static int CalculatePriorityForRemote(ADServiceConnectionPoint scp)
		{
			if (scp.Keywords != null && scp.Keywords.Count > 0)
			{
				string y = "site=" + ScpSearch.LocalSiteName;
				bool flag = false;
				foreach (string text in scp.Keywords)
				{
					if (!string.IsNullOrEmpty(text))
					{
						if (!string.IsNullOrEmpty(ScpSearch.LocalSiteName) && StringComparer.InvariantCultureIgnoreCase.Equals(text, y))
						{
							return 0;
						}
						if (text.StartsWith("site=", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					return 1;
				}
				return 2;
			}
			return 2;
		}

		private static string LocalSiteName
		{
			get
			{
				if (ScpSearch.localSiteName == null)
				{
					try
					{
						ScpSearch.localSiteName = NativeHelpers.GetSiteName(false);
						if (ScpSearch.localSiteName == null)
						{
							ScpSearch.localSiteName = string.Empty;
						}
					}
					catch (CannotGetSiteInfoException arg)
					{
						ScpSearch.localSiteName = string.Empty;
						ScpSearch.ConfigurationTracer.TraceDebug<CannotGetSiteInfoException>(0L, "Failed to get LocalSiteName. Exception: {0}", arg);
						Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_CannotGetLocalSiteName, null, new object[]
						{
							Globals.ProcessId
						});
					}
					ScpSearch.ConfigurationTracer.TraceDebug<string>(0L, "Using LocalSiteName={0}", ScpSearch.localSiteName);
				}
				return ScpSearch.localSiteName;
			}
		}

		private static Exception PerformRetryableAdOperation(string operationDescription, ScpSearch.RetryableAdOperation operation)
		{
			DateTime t = DateTime.UtcNow + ScpSearch.AdOperationRetryTimeout;
			Exception result = null;
			do
			{
				try
				{
					operation();
					return null;
				}
				catch (ADTransientException ex)
				{
					ScpSearch.ConfigurationTracer.TraceError<string, ADTransientException>(0L, "Failed AD operation {0} with exception {1}, retrying.", operationDescription, ex);
					result = ex;
				}
				Thread.Sleep(ScpSearch.AdOperationRetryWait);
			}
			while (DateTime.UtcNow < t);
			return result;
		}

		private const int MaximumScpPointerCount = 400;

		private const string DomainPrefix = "domain=";

		private const string SitePrefix = "site=";

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;

		private static readonly TimeSpan AdOperationRetryTimeout = TimeSpan.FromSeconds(20.0);

		private static readonly TimeSpan AdOperationRetryWait = TimeSpan.FromSeconds(1.0);

		private static readonly QueryFilter localQueryFilter = ExchangeScpObjects.AutodiscoverDomainPointerKeyword.Filter;

		private static readonly QueryFilter remoteQueryFilter = ExchangeScpObjects.AutodiscoverUrlKeyword.Filter;

		private static string localSiteName;

		private ADServiceConnectionPoint[] results;

		private delegate void RetryableAdOperation();

		private class ServiceBindingInformation
		{
			public Uri ServiceBinding;

			public ADServiceConnectionPoint SCP;
		}
	}
}
