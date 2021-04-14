using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GrammarIdentifier : IEquatable<GrammarIdentifier>
	{
		internal OrganizationId OrgId { get; private set; }

		internal string GrammarName { get; private set; }

		internal CultureInfo Culture { get; private set; }

		internal string TenantTopLevelGrammarDirPath
		{
			get
			{
				if (this.tenantTopLevelGrammarDirPath == null)
				{
					this.tenantTopLevelGrammarDirPath = this.GetTenantTopLevelGrammarDirPath();
				}
				return this.tenantTopLevelGrammarDirPath;
			}
		}

		public GrammarIdentifier(OrganizationId tenantId, CultureInfo culture, string grammarFileName)
		{
			ValidateArgument.NotNull(culture, "Culture");
			ValidateArgument.NotNullOrEmpty(grammarFileName, "GrammarFileName");
			ValidateArgument.NotNull(tenantId, "OrganizationId");
			this.OrgId = tenantId;
			this.Culture = culture;
			this.GrammarName = grammarFileName;
		}

		private string GetTenantTopLevelGrammarDirPath()
		{
			string text = Utils.GrammarPathFromCulture(this.Culture);
			text = Path.Combine(text, "Cache");
			if (this.OrgId.OrganizationalUnit == null)
			{
				text = Path.Combine(text, "Enterprise");
			}
			else
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.OrgId);
				string path = iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId().ToString();
				text = Path.Combine(text, path);
			}
			return text;
		}

		public static Guid GetSystemMailboxGuid(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarIdentifier.GetSystemMailboxGuid, orgId='{0}'", new object[]
			{
				orgId
			});
			Guid guid = Guid.Empty;
			ADUser localOrganizationMailboxByCapability = OrganizationMailbox.GetLocalOrganizationMailboxByCapability(orgId, OrganizationCapability.UMGrammarReady, true);
			if (localOrganizationMailboxByCapability == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarIdentifier.GetSystemMailboxGuid, orgId='{0}', No UMGrammarReady mailbox", new object[]
				{
					orgId
				});
				localOrganizationMailboxByCapability = OrganizationMailbox.GetLocalOrganizationMailboxByCapability(orgId, OrganizationCapability.UMGrammar, true);
			}
			if (localOrganizationMailboxByCapability != null)
			{
				guid = localOrganizationMailboxByCapability.ExchangeGuid;
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarIdentifier.GetSystemMailboxGuid, orgId='{0}', mbxGuid='{1}'", new object[]
				{
					orgId,
					guid
				});
				UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMGrammarUsage.ToString());
			}
			else
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_GrammarMailboxNotFound, null, new object[]
				{
					orgId
				});
				UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMGrammarUsage.ToString());
			}
			return guid;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as GrammarIdentifier);
		}

		public bool Equals(GrammarIdentifier other)
		{
			return other != null && (this.OrgId.Equals(other.OrgId) && this.Culture.Equals(other.Culture)) && this.GrammarName.Equals(other.GrammarName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.OrgId.GetHashCode() ^ this.Culture.GetHashCode() ^ this.GrammarName.GetHashCode();
		}

		public override string ToString()
		{
			return Path.Combine(this.TenantTopLevelGrammarDirPath, this.GrammarName);
		}

		private const string Enterprise = "Enterprise";

		private string tenantTopLevelGrammarDirPath;
	}
}
