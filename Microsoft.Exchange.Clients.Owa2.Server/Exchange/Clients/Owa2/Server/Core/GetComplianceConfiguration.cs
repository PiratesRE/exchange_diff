using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetComplianceConfiguration : ServiceCommand<ComplianceConfiguration>
	{
		public GetComplianceConfiguration(CallContext callContext) : base(callContext)
		{
			OwsLogRegistry.Register("GetComplianceConfiguration", typeof(OwaUserConfigurationLogMetadata), new Type[0]);
		}

		protected override ComplianceConfiguration InternalExecute()
		{
			ComplianceConfiguration complianceConfiguration = new ComplianceConfiguration();
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			OrganizationId organizationId = base.MailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
			complianceConfiguration.RmsTemplates = this.GetRmsTemplates(organizationId, userContext.UserCulture);
			complianceConfiguration.MessageClassifications = this.GetMessageClassifications(organizationId, userContext.UserCulture);
			return complianceConfiguration;
		}

		protected MessageClassificationType[] GetMessageClassifications(OrganizationId organizationId, CultureInfo userCulture)
		{
			IEnumerable<MessageClassificationType> source = from classificationSummary in this.GetRawClassifications(organizationId, userCulture)
			where classificationSummary.PermissionMenuVisible
			select new MessageClassificationType(classificationSummary.ClassificationID.ToString(), classificationSummary.DisplayName, classificationSummary.SenderDescription) into classification
			orderby classification.Name
			select classification;
			return source.ToArray<MessageClassificationType>();
		}

		protected virtual IEnumerable<ClassificationSummary> GetRawClassifications(OrganizationId organizationId, CultureInfo userCulture)
		{
			GetComplianceConfiguration.classificationConfig = (GetComplianceConfiguration.classificationConfig ?? new ClassificationConfig());
			return GetComplianceConfiguration.classificationConfig.GetClassifications(organizationId, userCulture);
		}

		protected RmsTemplateType[] GetRmsTemplates(OrganizationId organizationId, CultureInfo userCulture)
		{
			try
			{
				ExTraceGlobals.IrmTracer.TraceDebug<OrganizationId, CultureInfo>((long)this.GetHashCode(), "Getting rms templates for organization {0} with culture {1}", organizationId, userCulture);
				IEnumerable<RmsTemplate> source = this.AcquireRmsTemplates(organizationId);
				IEnumerable<RmsTemplateType> source2 = from mesrTemplate in source
				select new RmsTemplateType(mesrTemplate.Id.ToString(), mesrTemplate.GetName(userCulture), mesrTemplate.GetDescription(userCulture)) into template
				orderby template.Name
				select template;
				string arg = string.Join(",", from template in source2
				select string.Format(userCulture, "{0}:{1}", new object[]
				{
					template.Id,
					template.Name
				}));
				ExTraceGlobals.IrmTracer.TraceDebug<string>((long)this.GetHashCode(), "Loaded templates= {0}", arg);
				return source2.ToArray<RmsTemplateType>();
			}
			catch (OwaThrottlingException)
			{
				throw;
			}
			catch (Exception ex)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_RmsTemplateLoadFailure, organizationId.ToString(), new object[]
				{
					ex
				});
				OwaServerTraceLogger.AppendToLog(new RmsLoadingLogEvent(organizationId, ex));
			}
			return null;
		}

		protected virtual IEnumerable<RmsTemplate> AcquireRmsTemplates(OrganizationId organizationId)
		{
			this.HandleSimultaneousExpensiveRmsTemplateCalls(organizationId);
			IEnumerable<RmsTemplate> result;
			try
			{
				IEnumerable<RmsTemplate> enumerable;
				if (RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(organizationId))
				{
					enumerable = RmsClientManager.AcquireRmsTemplates(organizationId, false);
				}
				else
				{
					enumerable = DrmEmailConstants.EmptyTemplateArray;
				}
				result = enumerable;
			}
			finally
			{
				if (GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Contains(organizationId))
				{
					lock (GetComplianceConfiguration.organizationsDoingExpensiveRetrivals)
					{
						GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Remove(organizationId);
					}
				}
			}
			return result;
		}

		private void HandleSimultaneousExpensiveRmsTemplateCalls(OrganizationId organizationId)
		{
			lock (GetComplianceConfiguration.organizationsDoingExpensiveRetrivals)
			{
				if (GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Contains(organizationId))
				{
					throw new OwaThrottlingException();
				}
				if (!RmsClientManager.Initialized)
				{
					GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Add(organizationId);
				}
				else if (organizationId == OrganizationId.ForestWideOrgId)
				{
					Cache<Guid, RmsTemplate> templateCacheForFirstOrg = RmsClientManager.TemplateCacheForFirstOrg;
					ICollection<RmsTemplate> collection;
					if (templateCacheForFirstOrg == null || templateCacheForFirstOrg.GetAllValues(out collection) || collection.Count == 0)
					{
						GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Add(organizationId);
					}
				}
				else if (!RmsClientManager.IRMConfig.AreRmsTemplatesInCache(organizationId))
				{
					GetComplianceConfiguration.organizationsDoingExpensiveRetrivals.Add(organizationId);
				}
			}
		}

		private static ClassificationConfig classificationConfig;

		private static HashSet<OrganizationId> organizationsDoingExpensiveRetrivals = new HashSet<OrganizationId>();
	}
}
