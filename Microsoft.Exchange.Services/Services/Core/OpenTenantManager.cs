using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class OpenTenantManager : IDisposable
	{
		internal OpenTenantManager()
		{
			try
			{
				this.activeOrganization = this.GetNextActiveOrganization();
			}
			catch (ADTransientException ex)
			{
				OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "Encountered a transient exception when trying to query AD for an open tenant on initialize.  Mesage: {0}", ex.Message);
			}
			catch (OpenTenantQueryException ex2)
			{
				OpenTenantManager.trace.TraceError<string>((long)Thread.CurrentThread.ManagedThreadId, "Encountered an unexpected error when trying to query AD for an open tenant on initialize.  Message: {0}", ex2.Message);
			}
			this.workerThread = new Thread(new ThreadStart(this.EvaluateActiveOrganizationWork))
			{
				IsBackground = true
			};
			this.workerThread.Start();
		}

		internal event EventHandler EvaluationWaitingOnPulse = delegate(object sender, EventArgs args)
		{
		};

		public static OpenTenantManager Instance
		{
			get
			{
				return OpenTenantManager.instance.Value;
			}
		}

		public OrganizationId ActiveOrganizationId
		{
			get
			{
				if (this.ActiveOrganization == null)
				{
					lock (this.disposing)
					{
						if (this.activeOrganization == null)
						{
							this.activeOrganization = this.GetNextActiveOrganization();
						}
					}
				}
				return this.activeOrganization.OrganizationId;
			}
		}

		internal ExchangeConfigurationUnit ActiveOrganization
		{
			get
			{
				lock (this.evaluationPulseObject)
				{
					Monitor.Pulse(this.evaluationPulseObject);
				}
				return this.activeOrganization;
			}
		}

		internal static IntAppSettingsEntry MaximumUserPerTenantThreshold
		{
			get
			{
				return OpenTenantManager.maximumUserPerTenantThreshold;
			}
		}

		void IDisposable.Dispose()
		{
			lock (this.disposing)
			{
				this.disposing.Cancel();
			}
			lock (this.evaluationPulseObject)
			{
				Monitor.Pulse(this.evaluationPulseObject);
			}
			if (!this.workerThread.Join(TimeSpan.FromSeconds(10.0)))
			{
				this.workerThread.Abort();
			}
		}

		private static void SaveFullOrganizationImplementation(ExchangeConfigurationUnit organization)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(organization.OrganizationId.PartitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 287, "SaveFullOrganizationImplementation", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Common\\OpenTenantManager.cs");
			organization = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(organization.Id);
			if (!organization.OpenTenantFull)
			{
				organization.OpenTenantFull = true;
				tenantConfigurationSession.Save(organization);
			}
		}

		private static ExchangeConfigurationUnit ReloadOrganizationImplementation(ExchangeConfigurationUnit organization)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(organization.OrganizationId.PartitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 304, "ReloadOrganizationImplementation", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Common\\OpenTenantManager.cs");
			return tenantConfigurationSession.Read<ExchangeConfigurationUnit>(organization.Id);
		}

		private bool ValidateOrganizationHasVacancy()
		{
			if (this.activeOrganization.OpenTenantFull)
			{
				return false;
			}
			try
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit = OpenTenantManager.ReloadOrganization.Value(this.activeOrganization);
				if (exchangeConfigurationUnit != null)
				{
					this.activeOrganization = exchangeConfigurationUnit;
				}
			}
			catch (ADTransientException ex)
			{
				OpenTenantManager.trace.TraceWarning<string, string>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Encountered a transient exception when reloading the organization, ignoring.  Message: {1}", this.activeOrganization.ToString(), ex.Message);
			}
			return this.ValidateOrganizationHasVacancy(this.activeOrganization);
		}

		private bool ValidateOrganizationHasVacancy(ExchangeConfigurationUnit organization)
		{
			if (organization.OpenTenantFull)
			{
				return false;
			}
			bool flag = true;
			try
			{
				int num = OpenTenantManager.GetMailboxCount.Value(organization.OrganizationId);
				OpenTenantManager.trace.TraceDebug<string, int>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Found {1} mailboxes.", organization.ToString(), num);
				flag = (num < OpenTenantManager.maximumUserPerTenantThreshold.Value);
			}
			catch (ADTransientException ex)
			{
				OpenTenantManager.trace.TraceWarning<string, string>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Encountered a transient exception when querying AD for tenant vacancy, ignoring.  Message: {1}", organization.ToString(), ex.Message);
			}
			if (!flag)
			{
				OpenTenantManager.trace.TraceDebug<string, int>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Exceeded the user threshold of {1}.", organization.ToString(), OpenTenantManager.maximumUserPerTenantThreshold.Value);
				try
				{
					OpenTenantManager.SaveFullOrganization.Value(organization);
				}
				catch (ADTransientException ex2)
				{
					OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "Encountered a transient exception trying to update the vacancy flag on a tenant.  Message: {0}", ex2.Message);
				}
			}
			return flag;
		}

		private void EvaluateActiveOrganizationWork()
		{
			for (;;)
			{
				lock (this.evaluationPulseObject)
				{
					EventHandler evaluationWaitingOnPulse = this.EvaluationWaitingOnPulse;
					if (evaluationWaitingOnPulse != null)
					{
						evaluationWaitingOnPulse(this, EventArgs.Empty);
					}
					Monitor.Wait(this.evaluationPulseObject, OpenTenantManager.activeTenantVacancyEvaluationInterval.Value);
				}
				try
				{
					lock (this.disposing)
					{
						if (this.disposing.IsCancellationRequested)
						{
							break;
						}
						this.EvaluateActiveOrganization();
					}
					continue;
				}
				catch (ADTransientException ex)
				{
					OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "ADTransientException: {0}", ex.Message);
					OpenTenantManager.trace.TraceWarning<TimeSpan>((long)Thread.CurrentThread.ManagedThreadId, "Retry no sooner than {0}.", OpenTenantManager.minimumDelayBetweenFailedEvaluations.Value);
					Thread.Sleep(OpenTenantManager.minimumDelayBetweenFailedEvaluations.Value);
					continue;
				}
				catch (OpenTenantQueryException)
				{
					OpenTenantManager.trace.TraceWarning<TimeSpan>((long)Thread.CurrentThread.ManagedThreadId, "Retry no sooner than {0}.", OpenTenantManager.minimumDelayBetweenFailedEvaluations.Value);
					Thread.Sleep(OpenTenantManager.minimumDelayBetweenFailedEvaluations.Value);
					continue;
				}
				break;
			}
		}

		private void EvaluateActiveOrganization()
		{
			if (this.activeOrganization == null || !this.ValidateOrganizationHasVacancy() || this.activeLifetime.Elapsed > OpenTenantManager.activeTenantLifespan.Value)
			{
				this.activeOrganization = this.GetNextActiveOrganization();
			}
		}

		private ExchangeConfigurationUnit GetNextActiveOrganization()
		{
			OpenTenantManager.trace.TraceDebug((long)Thread.CurrentThread.ManagedThreadId, "Querying for partitions.");
			PartitionId[] array = OpenTenantManager.GetPartitions.Value();
			if (array == null || array.Length == 0)
			{
				OpenTenantManager.trace.TraceError((long)Thread.CurrentThread.ManagedThreadId, "Query for partitions returned no results.");
				throw new OpenTenantQueryException("Query for partitions returned no results.");
			}
			OpenTenantManager.trace.TraceDebug<int>((long)Thread.CurrentThread.ManagedThreadId, "Found {0} partitions.", array.Length);
			ADTransientException ex = null;
			foreach (PartitionId partitionId in from p in array
			orderby OpenTenantManager.random.Next()
			select p)
			{
				OpenTenantManager.trace.TraceDebug<string>((long)Thread.CurrentThread.ManagedThreadId, "Partition [{0}]: Querying for open tenants.", partitionId.ToString());
				ExchangeConfigurationUnit[] array2;
				try
				{
					array2 = OpenTenantManager.GetOpenOrganizations.Value(partitionId);
				}
				catch (ADTransientException ex2)
				{
					OpenTenantManager.trace.TraceWarning<string, string>((long)Thread.CurrentThread.ManagedThreadId, "Partition [{0}]: Encountered a transient exception when querying AD for open tenants.  Message: {1}", partitionId.ToString(), ex2.Message);
					ex = ex2;
					continue;
				}
				if (array2 == null || array2.Length == 0)
				{
					OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "Partition [{0}]: Query for organizations returned no results.", partitionId.ToString());
				}
				else
				{
					OpenTenantManager.trace.TraceDebug<string, int>((long)Thread.CurrentThread.ManagedThreadId, "Partition [{0}]: Found {1} open tenants.", partitionId.ToString(), array2.Length);
					foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in array2.OrderBy((ExchangeConfigurationUnit p) => OpenTenantManager.random.Next()))
					{
						OpenTenantManager.trace.TraceDebug<string>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Querying for vacancy.", exchangeConfigurationUnit.ToString());
						if (this.ValidateOrganizationHasVacancy(exchangeConfigurationUnit))
						{
							OpenTenantManager.trace.TraceDebug<string>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: Has vacancy and will be used.", exchangeConfigurationUnit.ToString());
							this.activeLifetime.Restart();
							return exchangeConfigurationUnit;
						}
						OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "Organization [{0}]: No vacancy.", exchangeConfigurationUnit.ToString());
					}
					OpenTenantManager.trace.TraceWarning<string>((long)Thread.CurrentThread.ManagedThreadId, "Partition [{0}]: No organizations have vacancy.", partitionId.ToString());
				}
			}
			if (ex != null)
			{
				throw ex;
			}
			OpenTenantManager.trace.TraceError((long)Thread.CurrentThread.ManagedThreadId, "Unable to find any organizations with vacancy.");
			throw new OpenTenantQueryException("Unable to find any organizations with vacancy.");
		}

		internal static readonly Hookable<Func<PartitionId[]>> GetPartitions = Hookable<Func<PartitionId[]>>.Create(true, new Func<PartitionId[]>(ADAccountPartitionLocator.GetAllAccountPartitionIds));

		internal static readonly Hookable<Func<PartitionId, ExchangeConfigurationUnit[]>> GetOpenOrganizations = Hookable<Func<PartitionId, ExchangeConfigurationUnit[]>>.Create(true, delegate(PartitionId partitionId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 44, "GetOpenOrganizations", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Common\\OpenTenantManager.cs");
			return tenantConfigurationSession.FindAllOpenConfigurationUnits(true);
		});

		internal static readonly Hookable<Func<OrganizationId, int>> GetMailboxCount = Hookable<Func<OrganizationId, int>>.Create(true, delegate(OrganizationId organizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false);
			IConfigurationSession configSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 61, "GetMailboxCount", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Common\\OpenTenantManager.cs");
			return SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Mail Users(VLV)", false);
		});

		internal static readonly Hookable<Action<ExchangeConfigurationUnit>> SaveFullOrganization = Hookable<Action<ExchangeConfigurationUnit>>.Create(true, new Action<ExchangeConfigurationUnit>(OpenTenantManager.SaveFullOrganizationImplementation));

		internal static readonly Hookable<Func<ExchangeConfigurationUnit, ExchangeConfigurationUnit>> ReloadOrganization = Hookable<Func<ExchangeConfigurationUnit, ExchangeConfigurationUnit>>.Create(true, new Func<ExchangeConfigurationUnit, ExchangeConfigurationUnit>(OpenTenantManager.ReloadOrganizationImplementation));

		private static readonly Microsoft.Exchange.Diagnostics.Trace trace = ExTraceGlobals.OpenTenantManagerTracer;

		private static readonly TimeSpanAppSettingsEntry activeTenantLifespan = new TimeSpanAppSettingsEntry("OpenTenantManager.ActiveTenantLifespan_Hours", TimeSpanUnit.Hours, TimeSpan.FromHours(24.0), OpenTenantManager.trace);

		private static readonly TimeSpanAppSettingsEntry activeTenantVacancyEvaluationInterval = new TimeSpanAppSettingsEntry("OpenTenantManager.ActiveTenantVacancyEvaluationInterval_Minutes", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(5.0), OpenTenantManager.trace);

		private static readonly IntAppSettingsEntry maximumUserPerTenantThreshold = new IntAppSettingsEntry("OpenTenantManager.MaximumUsersPerTenantThreshold", 100000, OpenTenantManager.trace);

		private static readonly TimeSpanAppSettingsEntry minimumDelayBetweenFailedEvaluations = new TimeSpanAppSettingsEntry("OpenTenantManager.MinimumDelayBetweenFailedEvaluations_Seconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(10.0), OpenTenantManager.trace);

		private static readonly Lazy<OpenTenantManager> instance = new Lazy<OpenTenantManager>(() => new OpenTenantManager());

		private static readonly Random random = new Random();

		private readonly Stopwatch activeLifetime = new Stopwatch();

		private readonly object evaluationPulseObject = new object();

		private readonly CancellationTokenSource disposing = new CancellationTokenSource();

		private readonly Thread workerThread;

		private volatile ExchangeConfigurationUnit activeOrganization;
	}
}
