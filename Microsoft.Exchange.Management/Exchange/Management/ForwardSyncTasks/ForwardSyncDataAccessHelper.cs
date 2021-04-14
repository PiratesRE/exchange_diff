using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal class ForwardSyncDataAccessHelper
	{
		public ForwardSyncDataAccessHelper(string serviceInstanceName)
		{
			this.InitializeSession();
			this.serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(serviceInstanceName);
			this.divergenceContainerObjectId = SyncServiceInstance.GetDivergenceContainerId(this.serviceInstanceObjectId);
		}

		public void InitializeServiceInstanceADStructure()
		{
			if (this.configSession.Read<SyncServiceInstance>(this.serviceInstanceObjectId) == null)
			{
				SyncServiceInstance syncServiceInstance = new SyncServiceInstance();
				syncServiceInstance.SetId(this.serviceInstanceObjectId);
				this.configSession.Save(syncServiceInstance);
			}
			if (this.configSession.Read<ADContainer>(this.divergenceContainerObjectId) == null)
			{
				ADContainer adcontainer = new ADContainer();
				adcontainer.SetId(this.divergenceContainerObjectId);
				this.configSession.Save(adcontainer);
			}
		}

		public void PersistNewDivergence(SyncObjectId syncObjectId, DateTime divergenceTime, bool incrementalOnly, bool linkRelated, bool temporary, bool tenantWide, string[] errors)
		{
			this.PersistNewDivergence(syncObjectId, divergenceTime, incrementalOnly, linkRelated, temporary, tenantWide, errors, false, true, null);
		}

		public void PersistNewDivergence(SyncObjectId syncObjectId, DateTime divergenceTime, bool incrementalOnly, bool linkRelated, bool temporary, bool tenantWide, string[] errors, bool validationDivergence, bool retriable, IDictionary divergenceInfoTable)
		{
			FailedMSOSyncObject failedMSOSyncObject = new FailedMSOSyncObject();
			failedMSOSyncObject.LoadDivergenceInfoXml();
			failedMSOSyncObject.SetId(this.GetDivergenceObjectId(syncObjectId));
			failedMSOSyncObject.ObjectId = syncObjectId;
			failedMSOSyncObject.DivergenceTimestamp = new DateTime?(divergenceTime);
			failedMSOSyncObject.IsIncrementalOnly = incrementalOnly;
			failedMSOSyncObject.IsLinkRelated = linkRelated;
			failedMSOSyncObject.IsTemporary = temporary;
			failedMSOSyncObject.DivergenceCount = 1;
			failedMSOSyncObject.IsTenantWideDivergence = tenantWide;
			failedMSOSyncObject.IsValidationDivergence = validationDivergence;
			failedMSOSyncObject.IsRetriable = retriable;
			if (failedMSOSyncObject.IsValidationDivergence)
			{
				failedMSOSyncObject.IsIgnoredInHaltCondition = true;
			}
			else
			{
				failedMSOSyncObject.IsIgnoredInHaltCondition = false;
			}
			failedMSOSyncObject.Errors = new MultiValuedProperty<string>();
			if (errors != null)
			{
				ForwardSyncDataAccessHelper.AddErrors(errors, failedMSOSyncObject);
			}
			if (divergenceInfoTable != null)
			{
				ForwardSyncDataAccessHelper.SetDivergenceInfoValues(divergenceInfoTable, failedMSOSyncObject);
				failedMSOSyncObject.SaveDivergenceInfoXml();
			}
			this.SaveDivergence(failedMSOSyncObject);
		}

		public virtual FailedMSOSyncObject GetExistingDivergence(SyncObjectId syncObjectId)
		{
			ADObjectId divergenceObjectId = this.GetDivergenceObjectId(syncObjectId);
			return this.configSession.Read<FailedMSOSyncObject>(divergenceObjectId);
		}

		public virtual IEnumerable<FailedMSOSyncObject> FindDivergence(QueryFilter filter)
		{
			return from x in this.configSession.FindPaged<FailedMSOSyncObject>(this.divergenceContainerObjectId, QueryScope.OneLevel, filter, null, 0)
			where !string.IsNullOrEmpty(x.ExternalDirectoryObjectId)
			select x;
		}

		public void UpdateExistingDivergence(FailedMSOSyncObject divergence, int occurenceCount, bool incrementalOnly, bool linkRelated, bool temporary, bool tenantWide, string[] errors, int errorListLengthLimit)
		{
			this.UpdateExistingDivergence(divergence, occurenceCount, incrementalOnly, linkRelated, temporary, tenantWide, errors, errorListLengthLimit, false, true, null);
		}

		public void UpdateExistingDivergence(FailedMSOSyncObject divergence, int occurenceCount, bool incrementalOnly, bool linkRelated, bool temporary, bool tenantWide, string[] errors, int errorListLengthLimit, bool validationDivergence, bool retriable, IDictionary divergenceInfoTable)
		{
			divergence.IsIncrementalOnly = (divergence.IsIncrementalOnly && incrementalOnly);
			divergence.IsLinkRelated = (divergence.IsLinkRelated || linkRelated);
			divergence.IsTemporary = temporary;
			divergence.IsTenantWideDivergence = (divergence.IsTenantWideDivergence || tenantWide);
			divergence.IsValidationDivergence = (divergence.IsValidationDivergence && validationDivergence);
			divergence.IsRetriable = retriable;
			if (!validationDivergence && divergence.IsValidationDivergence)
			{
				divergence.DivergenceCount = 1;
			}
			else
			{
				divergence.DivergenceCount += occurenceCount;
			}
			if (divergence.Errors == null)
			{
				divergence.Errors = new MultiValuedProperty<string>();
			}
			if (errors != null)
			{
				ForwardSyncDataAccessHelper.AddErrors(errors, divergence);
			}
			while (divergence.Errors.Count > 0 && divergence.Errors.Count > errorListLengthLimit)
			{
				divergence.Errors.RemoveAt(0);
			}
			if (divergenceInfoTable != null)
			{
				ForwardSyncDataAccessHelper.SetDivergenceInfoValues(divergenceInfoTable, divergence);
				divergence.SaveDivergenceInfoXml();
			}
			this.SaveDivergence(divergence);
		}

		public virtual void DeleteDivergence(FailedMSOSyncObject divergence)
		{
			ForwardSyncDataAccessHelper.CleanUpDivergenceIds(this.configSession, divergence);
			this.configSession.Delete(divergence);
		}

		internal static IConfigurationSession CreateSession(bool isReadOnly = false)
		{
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(isReadOnly, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 241, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\ForwardSyncDataAccessHelper.cs");
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}

		internal static void CleanUpDivergenceIds(IConfigurationSession session, FailedMSOSyncObject divergence)
		{
			divergence.ExternalDirectoryOrganizationId = null;
			divergence.ExternalDirectoryObjectId = null;
			session.Save(divergence);
		}

		internal static ObjectId GetRootId(FailedMSOSyncObjectIdParameter identityParameter)
		{
			if (identityParameter != null && identityParameter.IsServiceInstanceDefinied)
			{
				ADObjectId adobjectId = SyncServiceInstance.GetServiceInstanceObjectId(identityParameter.ServiceInstance.InstanceId);
				return SyncServiceInstance.GetDivergenceContainerId(adobjectId);
			}
			return SyncServiceInstance.GetMsoSyncRootContainer();
		}

		protected virtual void SaveDivergence(FailedMSOSyncObject divergence)
		{
			this.configSession.Save(divergence);
		}

		protected virtual ADObjectId GetDivergenceObjectId(SyncObjectId syncObjectId)
		{
			return this.divergenceContainerObjectId.GetChildId(FailedMSOSyncObject.GetObjectName(syncObjectId));
		}

		private static void SetDivergenceInfoValues(IDictionary divergenceInfoTable, FailedMSOSyncObject divergence)
		{
			Type type = divergence.GetType();
			foreach (object obj in divergenceInfoTable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (dictionaryEntry.Key != null && dictionaryEntry.Value != null)
				{
					PropertyInfo property = type.GetProperty(dictionaryEntry.Key.ToString());
					if (property != null)
					{
						property.SetValue(divergence, (string)dictionaryEntry.Value, null);
					}
				}
			}
		}

		private static void AddErrors(IEnumerable<string> errors, FailedMSOSyncObject divergence)
		{
			foreach (string text in errors)
			{
				string item = text;
				if (text.Length > 100000)
				{
					item = text.Substring(0, 100000);
				}
				if (!divergence.Errors.Contains(item))
				{
					divergence.Errors.Add(item);
				}
				if (text.Contains("WorkflowDelayCreationException") && divergence.IsLinkRelated && divergence.ObjectId.ObjectClass == DirectoryObjectClass.Group)
				{
					divergence.Comment = "PSDivergenceV2_Ignore, GroupMailbox divergence should be ignored by V2";
				}
			}
		}

		private void InitializeSession()
		{
			this.configSession = ForwardSyncDataAccessHelper.CreateSession(false);
		}

		private const int MaxErrorLength = 100000;

		private readonly ADObjectId serviceInstanceObjectId;

		private readonly ADObjectId divergenceContainerObjectId;

		private IConfigurationSession configSession;
	}
}
