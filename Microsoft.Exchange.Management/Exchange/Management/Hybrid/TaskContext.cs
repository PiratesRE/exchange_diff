using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class TaskContext : ITaskContext
	{
		public TaskContext(IUserInterface ui, ILogger logger, HybridConfiguration hybridConfigurationObject, IOnPremisesSession onPremisesSession, ITenantSession tenantSession)
		{
			this.UI = ui;
			this.Logger = logger;
			this.HybridConfigurationObject = hybridConfigurationObject;
			this.OnPremisesSession = onPremisesSession;
			this.TenantSession = tenantSession;
			this.Parameters = new ContextParameters();
			this.Errors = new List<LocalizedString>();
			this.Warnings = new List<LocalizedString>();
		}

		public HybridConfiguration HybridConfigurationObject { get; private set; }

		public IContextParameters Parameters { get; private set; }

		public ILogger Logger { get; private set; }

		public IUserInterface UI { get; private set; }

		public IOnPremisesSession OnPremisesSession { get; private set; }

		public ITenantSession TenantSession { get; private set; }

		public IList<LocalizedString> Errors { get; private set; }

		public IList<LocalizedString> Warnings { get; private set; }
	}
}
