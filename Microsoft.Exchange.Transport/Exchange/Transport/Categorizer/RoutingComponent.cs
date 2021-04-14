using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingComponent : IRoutingComponent, ITransportComponent, IDiagnosable
	{
		public RoutingComponent()
		{
			this.mailRouter = new MailRouter();
		}

		public IMailRouter MailRouter
		{
			get
			{
				return this.mailRouter;
			}
		}

		public void Load()
		{
			if (this.dependencies == null)
			{
				throw new InvalidOperationException("Load-time dependencies must be set before loading Routing Component");
			}
			RoutingPerformanceCounters perfCounters = new RoutingPerformanceCounters(this.dependencies.ProcessTransportRole);
			RoutingContextCore contextCore = new RoutingContextCore(this.dependencies.ProcessTransportRole, this.settings, this.dependencies, this.edgeDependencies, perfCounters);
			this.mailRouter.Load(contextCore);
		}

		public void Unload()
		{
			this.mailRouter.Unload();
		}

		public void SetLoadTimeDependencies(TransportAppConfig appConfig, ITransportConfiguration transportConfig)
		{
			this.dependencies = new RoutingDependencies(appConfig, transportConfig);
			this.edgeDependencies = new EdgeRoutingDependencies(transportConfig);
			if (appConfig != null)
			{
				this.settings = appConfig.Routing;
			}
		}

		public void SetRunTimeDependencies(ShadowRedundancyComponent shadowRedundancy, UnhealthyTargetFilterComponent unhealthyTargetFilter, CategorizerComponent categorizer)
		{
			if (this.dependencies == null)
			{
				throw new InvalidOperationException("Load-time dependencies must be set before run-time dependencies");
			}
			this.dependencies.ShadowRedundancy = shadowRedundancy;
			this.dependencies.UnhealthyTargetFilter = unhealthyTargetFilter;
			this.dependencies.Categorizer = categorizer;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public string GetDiagnosticComponentName()
		{
			return "Routing";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			bool verbose = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			foreach (XElement content in this.mailRouter.GetDiagnosticInfo(verbose, parameters))
			{
				xelement.Add(content);
			}
			if (flag)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, help, config, dagSelector, tenantDagQuota, tenant:{tenantID e.g.1afa2e80-0251-4521-8086-039fb2f9d8d6}."));
			}
			return xelement;
		}

		private TransportAppConfig.RoutingConfig settings;

		private RoutingDependencies dependencies;

		private EdgeRoutingDependencies edgeDependencies;

		private MailRouter mailRouter;
	}
}
