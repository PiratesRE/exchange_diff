using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMailboxCollectionBuilder : IGroupMailboxCollectionBuilder
	{
		public GroupMailboxCollectionBuilder(IRecipientSession adSession, IGroupsLogger logger)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.adSession = adSession;
			this.logger = logger;
		}

		private IRecipientSession AdSessionToPreferredDc
		{
			get
			{
				ADServerInfo adserverInfo;
				if (this.tenantPreferredAdSession == null && GroupMailboxAccessLayerHelper.GetDomainControllerAffinityForOrganization(this.adSession.SessionSettings.CurrentOrganizationId, out adserverInfo))
				{
					this.tenantPreferredAdSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(adserverInfo.Fqdn, true, this.adSession.ConsistencyMode, this.adSession.SessionSettings, 90, "AdSessionToPreferredDc", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Groups\\GroupMailboxCollectionBuilder.cs");
				}
				return this.tenantPreferredAdSession;
			}
		}

		public List<GroupMailbox> BuildGroupMailboxes(string[] externalIds)
		{
			List<GroupMailbox> groups = new List<GroupMailbox>(externalIds.Length);
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					if (GroupMailboxCollectionBuilder.Tracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						GroupMailboxCollectionBuilder.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupMailboxCollectionBuilder.BuildGroupMailboxes - Resolving Group ADUser object for ExternalIds={0}", string.Join(",", externalIds));
					}
					ITenantRecipientSession tenantAdSession = this.adSession as ITenantRecipientSession;
					if (tenantAdSession == null)
					{
						return;
					}
					Result<ADRawEntry>[] array = this.ExecuteAdQueryAndHandleAdExceptions<Result<ADRawEntry>[]>(() => tenantAdSession.FindByExternalDirectoryObjectIds(externalIds, GroupMailboxCollectionBuilder.GroupPropertiesToRead));
					for (int i = 0; i < array.Length; i++)
					{
						Result<ADRawEntry> result = array[i];
						if (result.Error != null || result.Data == null)
						{
							GroupMailbox item;
							if (this.TryResolveGroupFromTenantPreferredDc(externalIds[i], out item))
							{
								GroupMailboxCollectionBuilder.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupMailboxCollectionBuilder.BuildGroupMailboxes - Found AAD Group in EXODS using tenant preferred DC. ExternalDirectoryObjectId={0}", externalIds[i]);
								groups.Add(item);
							}
							else
							{
								this.logger.LogTrace("GroupMailboxCollectionBuilder.BuildGroupMailboxes - Unable to find AAD Group in EXODS using tenant preferred DC. ExternalDirectoryObjectId={0}. Error={1}", new object[]
								{
									externalIds[i],
									result.Error
								});
							}
						}
						else
						{
							GroupMailboxCollectionBuilder.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupMailboxCollectionBuilder.BuildGroupMailboxes - Found AAD Group in EXODS using batch AD query. ExternalDirectoryObjectId={0}", externalIds[i]);
							groups.Add(this.BuildGroupMailbox(result.Data));
						}
					}
				}, (Exception e) => GrayException.IsSystemGrayException(e));
			}
			catch (GrayException exception)
			{
				this.logger.LogException(exception, "GroupMailboxCollectionBuilder.BuildGroupMailboxes - Error reading groups from AD.", new object[0]);
			}
			catch (LocalizedException exception2)
			{
				this.logger.LogException(exception2, "GroupMailboxCollectionBuilder.BuildGroupMailboxes - Error reading groups from AD.", new object[0]);
			}
			return groups;
		}

		private T ExecuteAdQueryAndHandleAdExceptions<T>(Func<T> query)
		{
			T result = default(T);
			Exception ex = null;
			try
			{
				result = query();
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				this.logger.LogException(ex, "Caught exception while querying AD.", new object[0]);
			}
			return result;
		}

		private bool TryResolveGroupFromTenantPreferredDc(string externalId, out GroupMailbox groupMailbox)
		{
			groupMailbox = null;
			IRecipientSession preferredDcSession = this.AdSessionToPreferredDc;
			if (preferredDcSession == null)
			{
				return false;
			}
			if (this.adSession.LastUsedDc == preferredDcSession.DomainController)
			{
				return false;
			}
			ADUser group = this.ExecuteAdQueryAndHandleAdExceptions<ADUser>(() => preferredDcSession.FindADUserByExternalDirectoryObjectId(externalId));
			if (group == null)
			{
				return false;
			}
			this.ExecuteAdQueryAndHandleAdExceptions<bool>(delegate
			{
				this.EnsureGroupIsCached(group);
				return true;
			});
			groupMailbox = this.BuildGroupMailbox(group);
			return true;
		}

		private void EnsureGroupIsCached(ADUser group)
		{
			ProxyAddress proxyAddress = new SmtpProxyAddress(group.PrimarySmtpAddress.ToString(), true);
			ADUser aduser = this.AdSessionToPreferredDc.FindByProxyAddress(proxyAddress) as ADUser;
			OWAMiniRecipient owaminiRecipient = this.AdSessionToPreferredDc.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
			this.logger.LogTrace("Queried AD for group. ExternalId={0}, ProxyAddress={1}, DomainController={2}, FoundADUser={3}, FoundOwaMiniRecipient={4}", new object[]
			{
				group.ExternalDirectoryObjectId,
				proxyAddress,
				group.OriginatingServer,
				aduser != null,
				owaminiRecipient != null
			});
		}

		private GroupMailbox BuildGroupMailbox(ADRawEntry adEntry)
		{
			MailboxAssociation association = new MailboxAssociation
			{
				IsMember = true,
				JoinDate = ExDateTime.UtcNow
			};
			GroupMailboxLocator locator = new GroupMailboxLocator(this.adSession, adEntry[ADRecipientSchema.ExternalDirectoryObjectId] as string, adEntry[ADRecipientSchema.LegacyExchangeDN] as string);
			GroupMailboxBuilder groupMailboxBuilder = new GroupMailboxBuilder(locator);
			return groupMailboxBuilder.BuildFromAssociation(association).BuildFromDirectory(adEntry).Mailbox;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly PropertyDefinition[] AdditionalGroupPropertiesToRead = new PropertyDefinition[]
		{
			ADRecipientSchema.ExternalDirectoryObjectId,
			ADRecipientSchema.LegacyExchangeDN
		};

		private static readonly PropertyDefinition[] GroupPropertiesToRead = GroupMailboxBuilder.AllADProperties.Union(GroupMailboxCollectionBuilder.AdditionalGroupPropertiesToRead).ToArray<PropertyDefinition>();

		private readonly IGroupsLogger logger;

		private readonly IRecipientSession adSession;

		private IRecipientSession tenantPreferredAdSession;
	}
}
