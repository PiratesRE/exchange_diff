using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTagAdProvider : IPolicyTagProvider
	{
		public PolicyTag[] PolicyTags
		{
			get
			{
				OrganizationId currentOrganizationId = this.configurationSession.SessionSettings.CurrentOrganizationId;
				IConfigurationSession configurationSession;
				if (SharedConfiguration.IsDehydratedConfiguration(currentOrganizationId))
				{
					configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(currentOrganizationId);
				}
				else
				{
					configurationSession = this.configurationSession;
				}
				RetentionPolicyTag[] array = configurationSession.FindPaged<RetentionPolicyTag>(null, QueryScope.SubTree, null, null, 0).ToArray<RetentionPolicyTag>();
				PolicyTagAdProvider.Tracer.TraceDebug<int>((long)this.GetHashCode(), "PolicyTagADResolver resolving {0} RetentionPolicyTags", array.Count<RetentionPolicyTag>());
				return (from x in array
				select new PolicyTag
				{
					Name = x.Name,
					PolicyGuid = x.RetentionId,
					Description = x.Comment,
					Type = x.Type
				}).ToArray<PolicyTag>();
			}
		}

		public PolicyTagAdProvider(IConfigurationSession configurationSession)
		{
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			this.configurationSession = configurationSession;
		}

		private IConfigurationSession configurationSession;

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;
	}
}
