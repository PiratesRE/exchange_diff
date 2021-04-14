using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Set", "MainStreamForwardSyncCookie")]
	public sealed class SetMainStreamForwardSyncCookie : Task
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public ServiceInstanceId ServiceInstanceId { get; set; }

		[Parameter(Mandatory = true)]
		public int RollbackTimeIntervalMinutes { get; set; }

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || TaskHelper.IsTaskKnownException(exception) || exception is CannotGetDomainInfoException || exception is ServiceInstanceContainerNotFoundException || exception is DataValidationException;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.rollbackTimeSpan = new TimeSpan(0, this.RollbackTimeIntervalMinutes, 0);
			this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 83, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\SetMainStreamForwardSyncCookie.cs");
			this.cookieSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 89, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\SetMainStreamForwardSyncCookie.cs");
			this.cookieSession.UseConfigNC = false;
			this.container = this.cookieSession.GetMsoMainStreamCookieContainer(this.ServiceInstanceId.InstanceId);
			if (this.container.IsMultiObjectCookieEnabled)
			{
				this.cookieObjects[ForwardSyncCookieType.RecipientIncremental] = MsoMultiObjectCookieManager.LoadCookieHeaders(this.cookieSession, this.ServiceInstanceId.InstanceId, ForwardSyncCookieType.RecipientIncremental);
				this.cookieObjects[ForwardSyncCookieType.CompanyIncremental] = MsoMultiObjectCookieManager.LoadCookieHeaders(this.cookieSession, this.ServiceInstanceId.InstanceId, ForwardSyncCookieType.CompanyIncremental);
				this.ValidateAllCookiebjectsAreNotRemoved(this.cookieObjects[ForwardSyncCookieType.RecipientIncremental]);
				this.ValidateAllCookiebjectsAreNotRemoved(this.cookieObjects[ForwardSyncCookieType.CompanyIncremental]);
			}
			else
			{
				this.ValidateAllCookiesAreNotRemoved(this.container.MsoForwardSyncRecipientCookie);
				this.ValidateAllCookiesAreNotRemoved(this.container.MsoForwardSyncNonRecipientCookie);
			}
			this.forwardSyncDataAccessHelper = new ForwardSyncDataAccessHelper(this.ServiceInstanceId.InstanceId);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.container.IsMultiObjectCookieEnabled)
			{
				this.RollbackObjectCookies(ForwardSyncCookieType.RecipientIncremental);
				this.RollbackObjectCookies(ForwardSyncCookieType.CompanyIncremental);
			}
			else
			{
				this.RollbackCookies(this.container.MsoForwardSyncRecipientCookie, false);
				this.RollbackCookies(this.container.MsoForwardSyncNonRecipientCookie, true);
			}
			if (this.cookiesUpdated)
			{
				this.cookieSession.Save(this.container);
				int minorPartnerId = LocalSiteCache.LocalSite.MinorPartnerId;
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.NotEqual, ADSiteSchema.MinorPartnerId, minorPartnerId);
				ADSite[] array = this.configSession.Find<ADSite>(null, QueryScope.SubTree, filter, null, 0);
				if (array.Length > 0)
				{
					ADObjectId[] array2 = new ADObjectId[array.Length];
					int num = 0;
					foreach (ADSite adsite in array)
					{
						array2[num] = new ADObjectId(adsite.DistinguishedName);
						num++;
					}
					this.cookieSession.ReplicateSingleObject(this.container, array2);
				}
			}
			else
			{
				this.WriteWarning(Strings.WarningNoCookiesRemovedForRollback);
			}
			TaskLogger.LogExit();
		}

		private void RollbackObjectCookies(ForwardSyncCookieType type)
		{
			ForwardSyncCookieHeader[] array = this.cookieObjects[type];
			DateTime t = this.dateTimeNow.Subtract(this.rollbackTimeSpan);
			int num = array.Length - 1;
			while (num > 0 && array[num].Timestamp > t)
			{
				this.cookieSession.Delete(array[num--]);
			}
			this.cookiesUpdated = (num < array.Length - 1);
			if (num >= 0)
			{
				this.RollBackDivergences(type == ForwardSyncCookieType.CompanyIncremental, array[num].Timestamp);
			}
		}

		private void RollbackCookies(MultiValuedProperty<byte[]> cookies, bool isCompanyStream)
		{
			DateTime? dateTime = null;
			for (int i = cookies.Count - 1; i >= 0; i--)
			{
				MsoMainStreamCookie msoMainStreamCookie = null;
				Exception ex = null;
				if (MsoMainStreamCookie.TryFromStorageCookie(cookies[i], out msoMainStreamCookie, out ex) && string.Equals(this.ServiceInstanceId.ToString(), msoMainStreamCookie.ServiceInstanceName, StringComparison.OrdinalIgnoreCase))
				{
					if (this.dateTimeNow < new DateTime(msoMainStreamCookie.TimeStamp.Ticks + this.rollbackTimeSpan.Ticks, DateTimeKind.Utc))
					{
						cookies.RemoveAt(i);
						this.cookiesUpdated = true;
					}
					else if (dateTime == null || msoMainStreamCookie.TimeStamp > dateTime.Value)
					{
						dateTime = new DateTime?(msoMainStreamCookie.TimeStamp);
					}
				}
			}
			if (dateTime != null)
			{
				this.RollBackDivergences(isCompanyStream, dateTime.Value);
			}
		}

		private void RollBackDivergences(bool isCompanyStream, DateTime latestCookieTimestamp)
		{
			ComparisonFilter comparisonFilter = new ComparisonFilter(isCompanyStream ? ComparisonOperator.Equal : ComparisonOperator.NotEqual, FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass, DirectoryObjectClass.Company);
			ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.GreaterThan, FailedMSOSyncObjectSchema.DivergenceTimestamp, latestCookieTimestamp);
			ComparisonFilter comparisonFilter3 = new ComparisonFilter(ComparisonOperator.Equal, FailedMSOSyncObjectSchema.IsIncrementalOnly, true);
			IEnumerable<FailedMSOSyncObject> enumerable = this.forwardSyncDataAccessHelper.FindDivergence(new AndFilter(new QueryFilter[]
			{
				comparisonFilter,
				comparisonFilter2,
				comparisonFilter3
			}));
			foreach (FailedMSOSyncObject divergence in enumerable)
			{
				this.forwardSyncDataAccessHelper.DeleteDivergence(divergence);
			}
		}

		private void ValidateAllCookiebjectsAreNotRemoved(IEnumerable<ForwardSyncCookieHeader> cookies)
		{
			ForwardSyncCookieHeader forwardSyncCookieHeader = cookies.FirstOrDefault<ForwardSyncCookieHeader>();
			if (forwardSyncCookieHeader != null && this.dateTimeNow < new DateTime(forwardSyncCookieHeader.Timestamp.Ticks + this.rollbackTimeSpan.Ticks, DateTimeKind.Utc))
			{
				base.WriteError(new InvalidUserInputException(Strings.ErrorCannotRemoveAllCookies(this.rollbackTimeSpan.ToString())), ExchangeErrorCategory.Client, null);
			}
		}

		private void ValidateAllCookiesAreNotRemoved(MultiValuedProperty<byte[]> cookies)
		{
			MsoMainStreamCookie msoMainStreamCookie = null;
			foreach (byte[] storageCookie in cookies)
			{
				MsoMainStreamCookie msoMainStreamCookie2 = null;
				Exception ex = null;
				if (MsoMainStreamCookie.TryFromStorageCookie(storageCookie, out msoMainStreamCookie2, out ex) && string.Equals(this.ServiceInstanceId.ToString(), msoMainStreamCookie2.ServiceInstanceName, StringComparison.OrdinalIgnoreCase) && (msoMainStreamCookie == null || msoMainStreamCookie.TimeStamp > msoMainStreamCookie2.TimeStamp))
				{
					msoMainStreamCookie = msoMainStreamCookie2;
				}
			}
			if (msoMainStreamCookie != null && this.dateTimeNow < new DateTime(msoMainStreamCookie.TimeStamp.Ticks + this.rollbackTimeSpan.Ticks, DateTimeKind.Utc))
			{
				base.WriteError(new InvalidUserInputException(Strings.ErrorCannotRemoveAllCookies(this.rollbackTimeSpan.ToString())), (ErrorCategory)1000, null);
			}
		}

		private readonly Dictionary<ForwardSyncCookieType, ForwardSyncCookieHeader[]> cookieObjects = new Dictionary<ForwardSyncCookieType, ForwardSyncCookieHeader[]>();

		private IConfigurationSession configSession;

		private ITopologyConfigurationSession cookieSession;

		private MsoMainStreamCookieContainer container;

		private bool cookiesUpdated;

		private TimeSpan rollbackTimeSpan;

		private readonly DateTime dateTimeNow = DateTime.UtcNow;

		private ForwardSyncDataAccessHelper forwardSyncDataAccessHelper;
	}
}
