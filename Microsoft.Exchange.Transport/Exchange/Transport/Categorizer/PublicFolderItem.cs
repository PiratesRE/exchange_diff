using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class PublicFolderItem : ForwardableItem
	{
		public PublicFolderItem(MailRecipient recipient) : base(recipient)
		{
		}

		public override void Allow(Expansion expansion)
		{
			OrganizationId mailItemScopeOrganizationId = base.Recipient.MailItemScopeOrganizationId;
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(mailItemScopeOrganizationId);
			HeuristicsFlags heuristicsFlags = value.HeuristicsFlags;
			bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled;
			bool flag = mailItemScopeOrganizationId == OrganizationId.ForestWideOrgId && this.Database != null;
			if ((flag && heuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMigrationComplete)) || (enabled && heuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMailboxesMigrationComplete)))
			{
				foreach (Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain in expansion.Configuration.AcceptedDomains)
				{
					AcceptedDomainEntry acceptedDomainEntry = (AcceptedDomainEntry)acceptedDomain;
					if (acceptedDomainEntry.Name.Equals("PublicFolderDestination_78c0b207_5ad2_4fee_8cb9_f373175b3f99", StringComparison.OrdinalIgnoreCase))
					{
						string text = base.Recipient.Email.LocalPart + "@" + acceptedDomainEntry.NameSpecification;
						ExTraceGlobals.ResolverTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Redirecting public folder recipient {0} to {1} since PublicFolderMigrationComplete bit is set.", base.Recipient.Email.ToString(), text);
						if (SmtpAddress.IsValidSmtpAddress(text) && expansion.Resolver.RewriteEmail(base.Recipient, ProxyAddress.Parse(text), MessageTrackingSource.PUBLICFOLDER))
						{
							base.Recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.IsOneOffRecipient", true);
							base.Allow(expansion);
						}
						else
						{
							base.FailRecipient(AckReason.MigratedPublicFolderInvalidTargetAddress);
						}
						return;
					}
				}
			}
			ADObjectId adobjectId;
			bool flag2 = PublicFolderItem.TryGetContentMailbox(value, base.Recipient, out adobjectId);
			if (flag2 || flag)
			{
				bool flag3 = flag2;
				if (flag2 && flag)
				{
					flag3 = heuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMigrationComplete);
				}
				if (flag3)
				{
					Result<TransportMiniRecipient>? result = null;
					if (adobjectId != null)
					{
						result = new Result<TransportMiniRecipient>?(expansion.MailItem.ADRecipientCache.FindAndCacheRecipient(adobjectId));
					}
					if (result == null || result.Value.Error is ObjectValidationError)
					{
						ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "Content mailbox recipient is invalid");
						base.FailRecipient(AckReason.ContentMailboxInvalid);
						return;
					}
					TransportMiniRecipient data = result.Value.Data;
					if (data == null)
					{
						ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "Content mailbox recipient not found");
						base.FailRecipient(AckReason.ContentMailboxRecipientNotFound);
						return;
					}
					if (enabled && heuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMailboxesLockedForNewConnections))
					{
						ExTraceGlobals.ResolverTracer.TraceWarning((long)this.GetHashCode(), "Public folders are locked for migration, bifurcating and deferring delivery");
						List<MailRecipient> list = new List<MailRecipient>(1);
						list.Add(base.Recipient);
						expansion.Resolver.BifurcateAndDeferRecipients(list, expansion.MailItem, expansion.TaskContext, AckReason.PublicFolderMailboxesInTransit);
						throw new UnresolvedRecipientBifurcatedTransientException();
					}
					base.Recipient.ExtendedProperties.SetValue<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Database", data.Database);
					base.Recipient.ExtendedProperties.SetValue<Guid>("Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid", data.ExchangeGuid);
					base.Recipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.DirectoryData.ServerName", data.ServerName);
					MessageTrackingLog.TrackRedirect(MessageTrackingSource.PUBLICFOLDER, expansion.MailItem, new MsgTrackRedirectInfo(base.Recipient.Email, new RoutingAddress(data.PrimarySmtpAddress.ToString()), null));
				}
				base.Allow(expansion);
				return;
			}
			ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "Public folder mailboxes are unavailable due to ongoing migration");
			base.FailRecipient(AckReason.PublicFolderMailboxesInTransit);
		}

		internal static bool IsRemoteRecipient(MailRecipient recipient)
		{
			string proxyAddressString;
			ProxyAddress proxyAddress;
			if (!ContactItem.TryGetTargetAddress(recipient, out proxyAddressString) || !ProxyAddress.TryParse(proxyAddressString, out proxyAddress) || proxyAddress.Prefix != ProxyAddressPrefix.Smtp || !SmtpAddress.IsValidSmtpAddress(proxyAddress.AddressString))
			{
				return false;
			}
			OrganizationId mailItemScopeOrganizationId = recipient.MailItemScopeOrganizationId;
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(mailItemScopeOrganizationId);
			ADObjectId adobjectId;
			bool flag = PublicFolderItem.TryGetContentMailbox(value, recipient, out adobjectId);
			bool flag2 = mailItemScopeOrganizationId == OrganizationId.ForestWideOrgId && recipient.ExtendedProperties.GetValue<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Database", null) != null;
			return (!flag && !flag2) || value.PublicFoldersDeploymentType == PublicFoldersDeployment.Remote;
		}

		private static bool TryGetContentMailbox(TenantPublicFolderConfiguration publicFolderConfiguration, MailRecipient recipient, out ADObjectId contentMailbox)
		{
			PublicFolderInformation hierarchyMailboxInformation = publicFolderConfiguration.GetHierarchyMailboxInformation();
			contentMailbox = recipient.ExtendedProperties.GetValue<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ContentMailbox", null);
			return contentMailbox != null && !contentMailbox.IsDeleted && hierarchyMailboxInformation.Type == PublicFolderInformation.HierarchyType.MailboxGuid && hierarchyMailboxInformation.HierarchyMailboxGuid != Guid.Empty;
		}

		private const string PostMigrationPublicFolderDestinationDomainName = "PublicFolderDestination_78c0b207_5ad2_4fee_8cb9_f373175b3f99";
	}
}
