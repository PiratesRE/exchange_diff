using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceDiagnosticArgument : DiagnosableArgument
	{
		public bool Verbose
		{
			get
			{
				return base.HasArgument("verbose");
			}
		}

		public bool RefreshData
		{
			get
			{
				return base.HasArgument("refresh");
			}
		}

		public bool ShowForest
		{
			get
			{
				return base.HasArgument("forest");
			}
		}

		public bool ShowSite
		{
			get
			{
				return base.HasArgument("site");
			}
		}

		public Guid Site
		{
			get
			{
				return base.GetArgument<Guid>("site");
			}
		}

		public bool ShowServer
		{
			get
			{
				return base.HasArgument("server");
			}
		}

		public Guid Server
		{
			get
			{
				return base.GetArgument<Guid>("server");
			}
		}

		public bool ShowLoadBalancerResults
		{
			get
			{
				return base.HasArgument("loadbalanceresults");
			}
		}

		public bool StartLoadBalance
		{
			get
			{
				return base.HasArgument("startloadbalance");
			}
		}

		public bool ShowDatabase
		{
			get
			{
				return base.HasArgument("database");
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return base.GetArgument<Guid>("database");
			}
		}

		public bool TraceEnabled
		{
			get
			{
				return base.HasArgument("trace");
			}
		}

		public bool ShowQueues
		{
			get
			{
				return base.HasArgument("showqueues");
			}
		}

		protected override bool FailOnMissingArgument
		{
			get
			{
				return true;
			}
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["database"] = typeof(Guid);
			schema["server"] = typeof(Guid);
			schema["site"] = typeof(Guid);
			schema["forest"] = typeof(bool);
			schema["loadbalanceresults"] = typeof(bool);
			schema["refresh"] = typeof(bool);
			schema["startloadbalance"] = typeof(bool);
			schema["verbose"] = typeof(bool);
			schema["trace"] = typeof(bool);
			schema["showqueues"] = typeof(bool);
		}

		private const string DatabaseArgument = "database";

		private const string ServerArgument = "server";

		private const string SiteArgument = "site";

		private const string ForestArgument = "forest";

		private const string RefreshDataArgument = "refresh";

		private const string LoadBalancerResultsArgument = "loadbalanceresults";

		private const string StartLoadBalanceArgument = "startloadbalance";

		private const string VerboseArgument = "verbose";

		private const string TraceArgument = "trace";

		private const string ShowQueuesArgument = "showqueues";
	}
}
