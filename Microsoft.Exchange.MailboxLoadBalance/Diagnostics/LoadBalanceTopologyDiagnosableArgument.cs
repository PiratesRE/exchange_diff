using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceTopologyDiagnosableArgument : LoadBalanceDiagnosableArgumentBase
	{
		public bool ShowServer
		{
			get
			{
				return base.HasArgument("server");
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return base.GetArgument<Guid>("server");
			}
		}

		public bool ShowDatabase
		{
			get
			{
				return base.HasArgument("database");
			}
		}

		public bool ShowForest
		{
			get
			{
				return base.HasArgument("forest");
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return base.GetArgument<Guid>("database");
			}
		}

		public bool ShowDag
		{
			get
			{
				return base.HasArgument("dag");
			}
		}

		public Guid DagGuid
		{
			get
			{
				return base.GetArgument<Guid>("dag");
			}
		}

		public bool ShowForestHeatMap
		{
			get
			{
				return base.HasArgument("forestHeatMap");
			}
		}

		public bool ShowLocalServerHeatMap
		{
			get
			{
				return base.HasArgument("localServerHeatMap");
			}
		}

		protected override void ExtendSchema(Dictionary<string, Type> schema)
		{
			schema["database"] = typeof(Guid);
			schema["server"] = typeof(Guid);
			schema["dag"] = typeof(Guid);
			schema["forest"] = typeof(bool);
			schema["forestHeatMap"] = typeof(bool);
			schema["localServerHeatMap"] = typeof(bool);
		}

		private const string DatabaseArgument = "database";

		private const string ServerArgument = "server";

		private const string DagArgument = "dag";

		private const string ForestArgument = "forest";

		private const string ShowForestHeatMapArgument = "forestHeatMap";

		private const string ShowLocalServerHeatMapArgument = "localServerHeatMap";
	}
}
