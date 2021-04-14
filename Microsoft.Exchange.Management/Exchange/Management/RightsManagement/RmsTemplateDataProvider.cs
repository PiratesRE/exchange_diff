using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.RightsManagement
{
	internal sealed class RmsTemplateDataProvider : IConfigDataProvider
	{
		public RmsTemplateDataProvider(IConfigurationSession adSession) : this(adSession, RmsTemplateType.Distributed, false, null)
		{
		}

		public RmsTemplateDataProvider(IConfigurationSession adSession, RmsTemplateType typeToFetch, bool displayTemplatesIfInternalLicensingDisabled) : this(adSession, typeToFetch, displayTemplatesIfInternalLicensingDisabled, null)
		{
		}

		public RmsTemplateDataProvider(IConfigurationSession adSession, RmsTemplateType typeToFetch, bool displayTemplatesIfInternalLicensingDisabled, RMSTrustedPublishingDomain trustedPublishingDomain)
		{
			if (adSession == null)
			{
				throw new ArgumentNullException("adSession");
			}
			if (adSession.SessionSettings == null)
			{
				throw new ArgumentNullException("adSession.SessionSettings");
			}
			this.adSession = adSession;
			this.orgId = adSession.SessionSettings.CurrentOrganizationId;
			this.typeToFetch = typeToFetch;
			this.displayTemplatesIfInternalLicensingDisabled = displayTemplatesIfInternalLicensingDisabled;
			this.irmConfiguration = IRMConfiguration.Read(this.adSession);
			this.trustedPublishingDomain = trustedPublishingDomain;
		}

		public string Source
		{
			get
			{
				if (this.orgId == OrganizationId.ForestWideOrgId)
				{
					try
					{
						try
						{
							RmsClientManager.ADSession = this.adSession;
							Uri rmsserviceLocation = RmsClientManager.GetRMSServiceLocation(this.adSession.SessionSettings.CurrentOrganizationId, ServiceType.ClientLicensor);
							if (rmsserviceLocation != null)
							{
								return rmsserviceLocation.ToString();
							}
						}
						catch (RightsManagementException)
						{
							return null;
						}
						catch (ExchangeConfigurationException)
						{
							return null;
						}
						goto IL_87;
					}
					finally
					{
						RmsClientManager.ADSession = null;
					}
				}
				if (this.irmConfiguration != null && this.irmConfiguration.ServiceLocation != null)
				{
					return this.irmConfiguration.ServiceLocation.ToString();
				}
				IL_87:
				return null;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			RmsTemplateIdentity rmsTemplateIdentity = identity as RmsTemplateIdentity;
			if (rmsTemplateIdentity == null)
			{
				throw new ArgumentNullException("identity");
			}
			IConfigurable result;
			try
			{
				RmsClientManager.ADSession = this.adSession;
				foreach (RmsTemplate rmsTemplate in this.AcquireRmsTemplates())
				{
					if (rmsTemplate.Id == rmsTemplateIdentity.TemplateId)
					{
						return new RmsTemplatePresentation(rmsTemplate);
					}
				}
				result = null;
			}
			finally
			{
				RmsClientManager.ADSession = null;
			}
			return result;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			IEnumerable<T> enumerable = this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0);
			LinkedList<IConfigurable> linkedList = new LinkedList<IConfigurable>();
			foreach (T t in enumerable)
			{
				IConfigurable value = t;
				linkedList.AddLast(value);
				if (linkedList.Count >= 1000)
				{
					break;
				}
			}
			IConfigurable[] array = new IConfigurable[linkedList.Count];
			linkedList.CopyTo(array, 0);
			return array;
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			RmsTemplateQueryFilter templateQueryFilter = filter as RmsTemplateQueryFilter;
			if (templateQueryFilter == null)
			{
				templateQueryFilter = RmsTemplateQueryFilter.MatchAll;
			}
			try
			{
				RmsClientManager.ADSession = this.adSession;
				foreach (RmsTemplate template in this.AcquireRmsTemplates())
				{
					if (templateQueryFilter.Match(template))
					{
						yield return (T)((object)new RmsTemplatePresentation(template));
					}
				}
			}
			finally
			{
				RmsClientManager.ADSession = null;
			}
			yield break;
		}

		public void Save(IConfigurable instance)
		{
			if (this.orgId == OrganizationId.ForestWideOrgId)
			{
				throw new NotSupportedException();
			}
			RmsTemplatePresentation rmsTemplatePresentation = instance as RmsTemplatePresentation;
			if (rmsTemplatePresentation == null)
			{
				throw new ArgumentException("passed in instance not of type RmsTemplatePresentation", "instance");
			}
			Guid templateGuid = rmsTemplatePresentation.TemplateGuid;
			RMSTrustedPublishingDomain rmstrustedPublishingDomain = this.FindDefaultTPD();
			if (rmstrustedPublishingDomain == null)
			{
				return;
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(rmstrustedPublishingDomain.RMSTemplates))
			{
				string text = null;
				string text2 = null;
				foreach (string text3 in rmstrustedPublishingDomain.RMSTemplates)
				{
					RmsTemplateType rmsTemplateType;
					string text4 = RMUtil.DecompressTemplate(text3, out rmsTemplateType);
					Guid templateGuidFromLicense = DrmClientUtils.GetTemplateGuidFromLicense(text4);
					if (templateGuidFromLicense == templateGuid && rmsTemplateType != rmsTemplatePresentation.Type)
					{
						text = text3;
						text2 = RMUtil.CompressTemplate(text4, rmsTemplatePresentation.Type);
						break;
					}
				}
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
				{
					rmstrustedPublishingDomain.RMSTemplates.Remove(text);
					rmstrustedPublishingDomain.RMSTemplates.Add(text2);
					this.adSession.Save(rmstrustedPublishingDomain);
				}
			}
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotSupportedException();
		}

		private IEnumerable<RmsTemplate> AcquireRmsTemplates()
		{
			if (this.orgId == OrganizationId.ForestWideOrgId)
			{
				return RmsClientManager.AcquireRmsTemplates(this.orgId, true);
			}
			if (this.irmConfiguration == null || (!this.irmConfiguration.InternalLicensingEnabled && !this.displayTemplatesIfInternalLicensingDisabled))
			{
				return DrmEmailConstants.EmptyTemplateArray;
			}
			RMSTrustedPublishingDomain rmstrustedPublishingDomain = this.FindTPD();
			if (rmstrustedPublishingDomain == null)
			{
				return DrmEmailConstants.EmptyTemplateArray;
			}
			List<RmsTemplate> list = null;
			if (!MultiValuedPropertyBase.IsNullOrEmpty(rmstrustedPublishingDomain.RMSTemplates))
			{
				list = new List<RmsTemplate>(rmstrustedPublishingDomain.RMSTemplates.Count + 2);
				using (MultiValuedProperty<string>.Enumerator enumerator = rmstrustedPublishingDomain.RMSTemplates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string encodedTemplate = enumerator.Current;
						RmsTemplateType type = RmsTemplateType.Archived;
						string templateXrml = RMUtil.DecompressTemplate(encodedTemplate, out type);
						if (this.ShouldFetch(type))
						{
							list.Add(RmsTemplate.CreateServerTemplateFromTemplateDefinition(templateXrml, type));
						}
					}
					goto IL_CE;
				}
			}
			list = new List<RmsTemplate>(2);
			IL_CE:
			if (this.typeToFetch != RmsTemplateType.Archived && rmstrustedPublishingDomain.Default)
			{
				list.Add(RmsTemplate.DoNotForward);
				if (this.irmConfiguration.InternetConfidentialEnabled)
				{
					list.Add(RmsTemplate.InternetConfidential);
				}
			}
			return list;
		}

		private bool ShouldFetch(RmsTemplateType type)
		{
			return this.typeToFetch == RmsTemplateType.All || this.typeToFetch == type;
		}

		private RMSTrustedPublishingDomain FindTPD()
		{
			if (this.trustedPublishingDomain == null)
			{
				this.trustedPublishingDomain = this.FindDefaultTPD();
			}
			return this.trustedPublishingDomain;
		}

		private RMSTrustedPublishingDomain FindDefaultTPD()
		{
			if (this.irmConfiguration == null)
			{
				return null;
			}
			ADPagedReader<RMSTrustedPublishingDomain> adpagedReader = this.adSession.FindPaged<RMSTrustedPublishingDomain>(this.irmConfiguration.Id, QueryScope.OneLevel, null, null, 0);
			foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in adpagedReader)
			{
				if (rmstrustedPublishingDomain.Default)
				{
					return rmstrustedPublishingDomain;
				}
			}
			return null;
		}

		private IConfigurationSession adSession;

		private OrganizationId orgId;

		private RmsTemplateType typeToFetch;

		private readonly bool displayTemplatesIfInternalLicensingDisabled;

		private IRMConfiguration irmConfiguration;

		private RMSTrustedPublishingDomain trustedPublishingDomain;
	}
}
