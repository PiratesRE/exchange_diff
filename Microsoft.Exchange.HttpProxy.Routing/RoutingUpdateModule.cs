using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	internal class RoutingUpdateModule : IHttpModule
	{
		public RoutingUpdateModule() : this(null, null)
		{
		}

		public RoutingUpdateModule(IRoutingLookupFactory lookupFactory = null, IRoutingDiagnostics diagnostics = null)
		{
			this.lookupFactory = (lookupFactory ?? new RoutingEntryLookupFactory(RoutingUpdateModule.activeCopiesCacheProvider, new ActiveDirectoryUserProvider(RoutingUpdateModule.RUMUseADCache.Value)));
			this.diagnostics = (diagnostics ?? new RoutingUpdateDiagnostics());
		}

		public void Init(HttpApplication application)
		{
			application.BeginRequest += delegate(object sender, EventArgs args)
			{
				this.OnBeginRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		public void Dispose()
		{
		}

		public void OnBeginRequest(HttpContextBase context)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				RequestDetailsLogger requestDetailsLogger = null;
				RoutingUpdateDiagnostics routingUpdateDiagnostics = this.diagnostics as RoutingUpdateDiagnostics;
				if (routingUpdateDiagnostics != null)
				{
					HttpContext context2 = context.ApplicationInstance.Context;
					requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
					RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(context2, requestDetailsLogger);
					requestDetailsLogger.Set(RoutingUpdateModuleMetadata.Protocol, RequestDetailsLogger.ProtocolType.Value);
					routingUpdateDiagnostics.Clear();
				}
				NameValueCollection headers = context.Request.Headers;
				List<string> list = new List<string>();
				list.AddIfNotNull(headers.Get("X-RoutingEntry"));
				if (RoutingUpdateModule.RUMLegacyRoutingEntryEnabled.Value)
				{
					list.AddIfNotNull(headers.Get("X-LegacyRoutingEntry"));
				}
				if (list.Count > 0)
				{
					try
					{
						foreach (string text in list)
						{
							string[] source = text.Split(new char[]
							{
								','
							});
							IEnumerable<IRoutingEntry> routingEntries = from entry in source
							where RoutingEntryHeaderSerializer.IsValidHeaderString(entry)
							select RoutingEntryHeaderSerializer.Deserialize(entry);
							foreach (string value in this.GetRoutingUpdates(routingEntries))
							{
								context.Response.Headers.Add("X-RoutingEntryUpdate", value);
							}
						}
					}
					catch (Exception ex)
					{
						requestDetailsLogger.AppendGenericError("Exception", ex.ToString());
						throw;
					}
				}
				if (routingUpdateDiagnostics != null && !requestDetailsLogger.IsDisposed)
				{
					if (routingUpdateDiagnostics.GetTotalLatency() > 0L)
					{
						routingUpdateDiagnostics.LogLatencies(requestDetailsLogger);
						requestDetailsLogger.Commit();
					}
					else
					{
						requestDetailsLogger.SkipLogging = true;
					}
					requestDetailsLogger.Dispose();
				}
			});
		}

		internal IEnumerable<string> GetRoutingUpdates(IEnumerable<IRoutingEntry> routingEntries)
		{
			if (routingEntries == null)
			{
				throw new ArgumentNullException("routingEntries");
			}
			List<string> list = new List<string>();
			foreach (IRoutingEntry routingEntry in routingEntries)
			{
				IRoutingLookup lookupForType = this.lookupFactory.GetLookupForType(routingEntry.Key.RoutingItemType);
				if (lookupForType != null)
				{
					IRoutingEntry routingEntry2 = lookupForType.GetRoutingEntry(routingEntry.Key, this.diagnostics);
					if (routingEntry2 != null && this.ShouldSendRoutingEntryUpdate(routingEntry, routingEntry2))
					{
						list.Add(RoutingEntryHeaderSerializer.Serialize(routingEntry2));
					}
				}
			}
			return list;
		}

		private bool ShouldSendRoutingEntryUpdate(IRoutingEntry originalRoutingEntry, IRoutingEntry updatedRoutingEntry)
		{
			if (!originalRoutingEntry.Destination.Equals(updatedRoutingEntry.Destination))
			{
				DatabaseGuidRoutingDestination databaseGuidRoutingDestination = originalRoutingEntry.Destination as DatabaseGuidRoutingDestination;
				DatabaseGuidRoutingDestination databaseGuidRoutingDestination2 = updatedRoutingEntry.Destination as DatabaseGuidRoutingDestination;
				return !(databaseGuidRoutingDestination2 != null) || !(databaseGuidRoutingDestination != null) || !(databaseGuidRoutingDestination.DatabaseGuid == databaseGuidRoutingDestination2.DatabaseGuid) || !(databaseGuidRoutingDestination.DomainName == databaseGuidRoutingDestination2.DomainName) || !string.IsNullOrEmpty(databaseGuidRoutingDestination.ResourceForest) || !HttpProxyGlobals.LocalMachineForest.Member.Equals(databaseGuidRoutingDestination2.ResourceForest, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static readonly BoolAppSettingsEntry RUMLegacyRoutingEntryEnabled = new BoolAppSettingsEntry("RoutingUpdateModule.LegacyRoutingEntryEnabled", VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.RUMLegacyRoutingEntry.Enabled, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry RUMUseADCache = new BoolAppSettingsEntry("RoutingUpdateModule.RUMUseADCache", VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.RUMUseADCache.Enabled, ExTraceGlobals.VerboseTracer);

		private static ActiveCopiesCacheProvider activeCopiesCacheProvider = new ActiveCopiesCacheProvider();

		private readonly IRoutingLookupFactory lookupFactory;

		private IRoutingDiagnostics diagnostics;
	}
}
