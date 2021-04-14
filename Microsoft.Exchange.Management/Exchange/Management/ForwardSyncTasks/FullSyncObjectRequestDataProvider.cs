using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FullSyncObjectRequestDataProvider : IConfigDataProvider
	{
		public FullSyncObjectRequestDataProvider(bool readOnly, string serviceInstanceName)
		{
			this.lastRidMasterRefreshTime = DateTime.MinValue;
			this.serviceInstanceName = serviceInstanceName;
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, readOnly, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 55, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\FullSyncObjectRequestDataProvider.cs");
			this.configurationSession.UseConfigNC = false;
			this.RefreshRidMasterInformation();
		}

		private void RefreshRidMasterInformation()
		{
			if (DateTime.UtcNow - this.lastRidMasterRefreshTime > FullSyncObjectRequestDataProvider.RidMasterRefreshInterval)
			{
				RidMasterInfo ridMasterInfo = SyncDaemonArbitrationConfigHelper.GetRidMasterInfo(this.configurationSession);
				this.configurationSession.DomainController = ridMasterInfo.RidMasterServer;
				this.lastRidMasterRefreshTime = DateTime.UtcNow;
			}
		}

		public string Source
		{
			get
			{
				return this.configurationSession.Source;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (this.serviceInstanceName == null)
			{
				throw new InvalidOperationException("Read can be performed only for specific service instance.");
			}
			SyncObjectId syncObjectId = identity as SyncObjectId;
			if (syncObjectId == null)
			{
				throw new NotSupportedException(identity.GetType().FullName);
			}
			this.RefreshRidMasterInformation();
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.configurationSession.GetMsoMainStreamCookieContainer(this.serviceInstanceName);
			MultiValuedProperty<FullSyncObjectRequest> msoForwardSyncObjectFullSyncRequests = msoMainStreamCookieContainer.MsoForwardSyncObjectFullSyncRequests;
			foreach (FullSyncObjectRequest fullSyncObjectRequest in msoForwardSyncObjectFullSyncRequests)
			{
				if (syncObjectId.Equals(fullSyncObjectRequest.Identity))
				{
					fullSyncObjectRequest.ResetChangeTracking(true);
					return fullSyncObjectRequest;
				}
			}
			return null;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).Cast<IConfigurable>().ToArray<IConfigurable>();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			Func<SyncServiceInstance, string> func = null;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (filter != null && comparisonFilter == null)
			{
				throw new NotSupportedException(filter.GetType().FullName);
			}
			if (comparisonFilter != null && comparisonFilter.ComparisonOperator != ComparisonOperator.Equal)
			{
				throw new NotSupportedException(comparisonFilter.ComparisonOperator.ToString());
			}
			this.RefreshRidMasterInformation();
			List<string> list = new List<string>();
			if (this.serviceInstanceName != null)
			{
				list.Add(this.serviceInstanceName);
			}
			else
			{
				List<string> list2 = list;
				IEnumerable<SyncServiceInstance> allServiceInstances = this.GetAllServiceInstances();
				if (func == null)
				{
					func = ((SyncServiceInstance i) => i.Name);
				}
				list2.AddRange(allServiceInstances.Select(func));
			}
			List<T> list3 = new List<T>();
			foreach (string text in list)
			{
				list3.AddRange(this.FindInServiceInstance<T>(comparisonFilter, text));
			}
			return list3;
		}

		private IEnumerable<SyncServiceInstance> GetAllServiceInstances()
		{
			return this.configurationSession.FindPaged<SyncServiceInstance>(SyncServiceInstance.GetMsoSyncRootContainer(), QueryScope.OneLevel, null, null, 0);
		}

		private IEnumerable<T> FindInServiceInstance<T>(ComparisonFilter comparisonFilter, string serviceInstanceName)
		{
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.configurationSession.GetMsoMainStreamCookieContainer(serviceInstanceName);
			MultiValuedProperty<FullSyncObjectRequest> msoForwardSyncObjectFullSyncRequests = msoMainStreamCookieContainer.MsoForwardSyncObjectFullSyncRequests;
			if (comparisonFilter == null)
			{
				return (IEnumerable<T>)msoForwardSyncObjectFullSyncRequests;
			}
			return (IEnumerable<T>)(from request in msoForwardSyncObjectFullSyncRequests
			where StringComparer.OrdinalIgnoreCase.Equals(request[comparisonFilter.Property], comparisonFilter.PropertyValue)
			select request);
		}

		public void Save(IConfigurable instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (this.configurationSession.ReadOnly)
			{
				throw new InvalidOperationException("read only");
			}
			if (this.serviceInstanceName == null)
			{
				throw new InvalidOperationException("Save can be performed only for specific service instance.");
			}
			FullSyncObjectRequest request = instance as FullSyncObjectRequest;
			if (request == null)
			{
				throw new NotSupportedException(instance.GetType().FullName);
			}
			this.RefreshRidMasterInformation();
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.configurationSession.GetMsoMainStreamCookieContainer(this.serviceInstanceName);
			MultiValuedProperty<FullSyncObjectRequest> msoForwardSyncObjectFullSyncRequests = msoMainStreamCookieContainer.MsoForwardSyncObjectFullSyncRequests;
			FullSyncObjectRequest fullSyncObjectRequest = msoForwardSyncObjectFullSyncRequests.Find((FullSyncObjectRequest r) => request.Identity.Equals(r.Identity) && request.ServiceInstanceId == r.ServiceInstanceId);
			if (fullSyncObjectRequest != null)
			{
				if (request.ObjectState == ObjectState.New)
				{
					throw new ADObjectAlreadyExistsException(DirectoryStrings.ExceptionADOperationFailedAlreadyExist(this.configurationSession.DomainController, request.ToString()));
				}
				if (request.ObjectState == ObjectState.Changed)
				{
					msoForwardSyncObjectFullSyncRequests.Remove(fullSyncObjectRequest);
				}
			}
			else
			{
				IEnumerable<FullSyncObjectRequest> source = from r in msoForwardSyncObjectFullSyncRequests
				where request.ServiceInstanceId == r.ServiceInstanceId
				select r;
				int maxObjectFullSyncRequestsPerServiceInstance = ProvisioningTasksConfigImpl.MaxObjectFullSyncRequestsPerServiceInstance;
				if (source.Count<FullSyncObjectRequest>() >= maxObjectFullSyncRequestsPerServiceInstance)
				{
					throw new DataSourceOperationException(Strings.OperationExceedsPerServiceInstanceFullSyncObjectRequestLimit(maxObjectFullSyncRequestsPerServiceInstance, request.ServiceInstanceId));
				}
			}
			msoForwardSyncObjectFullSyncRequests.Add(request);
			this.configurationSession.Save(msoMainStreamCookieContainer);
		}

		public void Delete(IConfigurable instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (this.configurationSession.ReadOnly)
			{
				throw new InvalidOperationException("read only");
			}
			if (this.serviceInstanceName == null)
			{
				throw new InvalidOperationException("Delete can be performed only for specific service instance.");
			}
			FullSyncObjectRequest fullSyncObjectRequest = instance as FullSyncObjectRequest;
			if (fullSyncObjectRequest == null)
			{
				throw new NotSupportedException(instance.GetType().FullName);
			}
			this.RefreshRidMasterInformation();
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.configurationSession.GetMsoMainStreamCookieContainer(this.serviceInstanceName);
			MultiValuedProperty<FullSyncObjectRequest> msoForwardSyncObjectFullSyncRequests = msoMainStreamCookieContainer.MsoForwardSyncObjectFullSyncRequests;
			if (msoForwardSyncObjectFullSyncRequests.Contains(fullSyncObjectRequest))
			{
				msoForwardSyncObjectFullSyncRequests.Remove(fullSyncObjectRequest);
				this.configurationSession.Save(msoMainStreamCookieContainer);
				return;
			}
			throw new ADNoSuchObjectException(DirectoryStrings.ExceptionADOperationFailedNoSuchObject(this.configurationSession.DomainController, fullSyncObjectRequest.ToString()));
		}

		private static readonly TimeSpan RidMasterRefreshInterval = TimeSpan.FromMinutes(1.0);

		private readonly ITopologyConfigurationSession configurationSession;

		private DateTime lastRidMasterRefreshTime;

		private readonly string serviceInstanceName;
	}
}
