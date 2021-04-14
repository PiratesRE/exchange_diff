using System;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Ceres.SearchCore.Admin;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class IndexSeeder : FastManagementClient, IIndexSeederSource, IIndexSeederTarget, IDisposable
	{
		internal IndexSeeder(string catalog)
		{
			Util.ThrowOnNullOrEmptyArgument(catalog, "catalog");
			this.catalog = catalog;
			base.DiagnosticsSession.ComponentName = "IndexSeeder";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.IndexManagementTracer;
			base.ConnectManagementAgents();
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 63;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IndexSeeder>(this);
		}

		public string SeedToEndPoint(string seedingEndPoint, string reason)
		{
			Util.ThrowOnNullOrEmptyArgument(seedingEndPoint, "seedingEndPoint");
			if (string.IsNullOrEmpty(reason))
			{
				reason = "Seeding reason not specified";
			}
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Seed-EndPoint({0}) - EndPoint={1}, Reason={2}", new object[]
			{
				this.catalog,
				seedingEndPoint,
				reason
			});
			FailureCode failureCode = new FailureCode(0, reason, 0);
			return this.PerformFastOperation<string>(() => this.seedingService.SeedToEndPointWithReason(seedingEndPoint, failureCode), "SeedToEndPoint");
		}

		public int GetProgress(string identifier)
		{
			Util.ThrowOnNullOrEmptyArgument(identifier, "identifier");
			int num = this.PerformFastOperation<int>(() => this.seedingService.GetProgress(identifier), "GetProgress");
			if (num < 0)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Get-Progress({0}) - Return Value={1}", new object[]
				{
					this.catalog,
					num
				});
			}
			else if (num == 100)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Get-Progress({0}) - Seeding Completed", new object[]
				{
					this.catalog
				});
			}
			return num;
		}

		public void Cancel(string identifier, string reason)
		{
			Util.ThrowOnNullOrEmptyArgument(identifier, "identifier");
			if (string.IsNullOrEmpty(reason))
			{
				reason = "Cancel reason not specified";
			}
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Abort-Seeding({0}) - Reason={1}", new object[]
			{
				this.catalog,
				reason
			});
			FailureCode failureCode = new FailureCode(0, reason, 0);
			base.PerformFastOperation(delegate()
			{
				this.seedingService.AbortWithReason(identifier, failureCode);
			}, "Abort");
		}

		public string GetSeedingEndPoint()
		{
			return this.PerformFastOperation<string>(() => this.seedingService.GetSeedingEndPoint(), "GetSeedingEndPoint");
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.seedingService = client.GetManagementAgent<ISeedingManagementAgent>("SeedingAgent-" + this.catalog + "/Single");
		}

		private readonly string catalog;

		private volatile ISeedingManagementAgent seedingService;
	}
}
