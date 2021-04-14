using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateGroupMailbox : SingleStepServiceCommand<UpdateGroupMailboxRequest, ServiceResultNone>
	{
		public UpdateGroupMailbox(CallContext callContext, UpdateGroupMailboxRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(UpdateGroupMailboxMetadata), new Type[0]);
		}

		private IRecipientSession ReadWriteADSession
		{
			get
			{
				if (this.adSession == null)
				{
					using (new StopwatchRequestDetailsLogger(base.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.ADSessionCreateTime))
					{
						ADSessionSettings sessionSettings = Directory.SessionSettingsFromAddress(base.Request.GroupSmtpAddress);
						this.adSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.Request.DomainControllerOrNull, false, ConsistencyMode.IgnoreInvalid, sessionSettings, 84, "ReadWriteADSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\UpdateGroupMailbox.cs");
					}
				}
				return this.adSession;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UpdateGroupMailboxResponse updateGroupMailboxResponse = new UpdateGroupMailboxResponse();
			updateGroupMailboxResponse.ProcessServiceResult(base.Result);
			return updateGroupMailboxResponse;
		}

		internal override bool InternalPreExecute()
		{
			base.CallContext.ProtocolLog.Set(ActivityStandardMetadata.TenantId, base.Request.GroupPrimarySmtpAddress.Domain);
			if (!S2SRightsWrapper.AllowsTokenSerializationBy(base.CallContext.EffectiveCaller.ClientSecurityContext))
			{
				UpdateGroupMailbox.Tracer.TraceError((long)this.GetHashCode(), "Unauthorized request: EffectiveCaller is not trusted for token serialization.");
				throw new ServiceAccessDeniedException();
			}
			UpdateGroupMailbox.Tracer.TraceDebug((long)this.GetHashCode(), "Successfully authorized request: EffectiveCaller is trusted for token serialization and is therefore an exchange server.");
			return base.InternalPreExecute();
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			ADUser executingUser = this.FindExecutingUser();
			ADUser aduser = this.FindGroupAdUser();
			using (MailboxSession mailboxSession = this.CreateMailboxSession(aduser))
			{
				this.ConfigureGroupMailbox(mailboxSession, aduser, executingUser, base.Request.ForceConfigurationAction);
				this.PopulateGroupInADCache();
				this.SetMembership(mailboxSession, aduser, executingUser);
				this.SetPermissionsVersion(mailboxSession, base.Request.PermissionsVersion);
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private MailboxSession CreateMailboxSession(ADUser groupAdUser)
		{
			MailboxSession result;
			using (new StopwatchRequestDetailsLogger(base.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.MailboxLogonTime))
			{
				ExchangePrincipal groupPrincipal = ExchangePrincipal.FromADUser(groupAdUser, null);
				result = Microsoft.Exchange.Data.GroupMailbox.ConfigureGroupMailbox.CreateMailboxSessionForConfiguration(groupPrincipal, groupAdUser.OriginatingServer);
			}
			return result;
		}

		private ADUser FindGroupAdUser()
		{
			ADUser result;
			using (new StopwatchRequestDetailsLogger(base.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.GroupAdLookupTime))
			{
				ADUser aduser = this.ReadWriteADSession.FindByProxyAddress(ProxyAddress.Parse(base.Request.GroupSmtpAddress)) as ADUser;
				if (aduser == null)
				{
					UpdateGroupMailbox.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Unable to find group by proxy address. GroupSmtpAddress={0}, DomainController={1}", base.Request.GroupSmtpAddress, this.ReadWriteADSession.DomainController);
					throw new MailboxNotFoundException(CoreResources.GetGroupMailboxFailed(base.Request.GroupSmtpAddress, "NotFound"));
				}
				UpdateGroupMailbox.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Found group by proxy address. GroupSmtpAddress={0}, DomainController={1}, ExternalDirectoryObjectId={2}", base.Request.GroupSmtpAddress, this.ReadWriteADSession.DomainController, aduser.ExternalDirectoryObjectId);
				base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.ExchangeGuid, aduser.ExchangeGuid);
				base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.ExternalDirectoryObjectId, aduser.ExternalDirectoryObjectId);
				result = aduser;
			}
			return result;
		}

		private void PopulateGroupInADCache()
		{
			IRecipientSession tenantOrRootRecipientReadOnlySession = DirectorySessionFactory.Default.GetTenantOrRootRecipientReadOnlySession(this.ReadWriteADSession, base.Request.DomainControllerOrNull, 217, "PopulateGroupInADCache", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\UpdateGroupMailbox.cs");
			ProxyAddress proxyAddress = ProxyAddress.Parse(base.Request.GroupSmtpAddress);
			ADUser aduser = tenantOrRootRecipientReadOnlySession.FindByProxyAddress(proxyAddress) as ADUser;
			base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.IsPopulateADUserInCacheSuccessful, aduser != null);
			OWAMiniRecipient owaminiRecipient = tenantOrRootRecipientReadOnlySession.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
			base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.IsPopulateMiniRecipientInCacheSuccessful, owaminiRecipient != null);
		}

		private ADUser FindExecutingUser()
		{
			ADUser aduser = null;
			if (!string.IsNullOrWhiteSpace(base.Request.ExecutingUserSmtpAddress))
			{
				using (new StopwatchRequestDetailsLogger(base.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.ExecutingUserLookupTime))
				{
					aduser = (this.ReadWriteADSession.FindByProxyAddress(ProxyAddress.Parse(base.Request.ExecutingUserSmtpAddress)) as ADUser);
					if (aduser == null)
					{
						UpdateGroupMailbox.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Unable to find executing user by proxy address. SmtpAddress={0}, DomainController={1}", base.Request.GroupSmtpAddress, this.ReadWriteADSession.DomainController);
						throw new MailboxNotFoundException(CoreResources.ExecutingUserNotFound(base.Request.ExecutingUserSmtpAddress));
					}
					UpdateGroupMailbox.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Found ExecutingUser by proxy address. SmtpAddress={0}, DomainController={1}, ExternalDirectoryObjectId={2}", base.Request.ExecutingUserSmtpAddress, this.ReadWriteADSession.DomainController, aduser.ExternalDirectoryObjectId);
				}
			}
			return aduser;
		}

		private void ConfigureGroupMailbox(MailboxSession session, ADUser group, ADUser executingUser, GroupMailboxConfigurationAction forceConfigurationActionMask)
		{
			base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.ForceConfigurationActionValue, forceConfigurationActionMask);
			ConfigureGroupMailbox configureGroupMailbox = new ConfigureGroupMailbox(this.ReadWriteADSession, group, executingUser, session);
			GroupMailboxConfigurationReport report = configureGroupMailbox.Execute(forceConfigurationActionMask);
			this.LogConfigurationReport(report);
		}

		private void LogConfigurationReport(GroupMailboxConfigurationReport report)
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			foreach (KeyValuePair<GroupMailboxConfigurationAction, LatencyStatistics> keyValuePair in report.ConfigurationActionLatencyStatistics)
			{
				LatencyStatistics value = keyValuePair.Value;
				long num = (long)value.ElapsedTime.TotalMilliseconds;
				GroupMailboxConfigurationAction key = keyValuePair.Key;
				if (key <= GroupMailboxConfigurationAction.SetAllFolderPermissions)
				{
					switch (key)
					{
					case GroupMailboxConfigurationAction.SetRegionalSettings:
						base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.SetRegionalSettingsTime, num);
						goto IL_19D;
					case GroupMailboxConfigurationAction.CreateDefaultFolders:
						base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.CreateDefaultFoldersTime, num);
						base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.CreateDefaultFoldersCount, report.FoldersCreatedCount);
						goto IL_19D;
					case GroupMailboxConfigurationAction.SetRegionalSettings | GroupMailboxConfigurationAction.CreateDefaultFolders:
						goto IL_19D;
					case GroupMailboxConfigurationAction.SetInitialFolderPermissions:
						break;
					default:
						if (key != GroupMailboxConfigurationAction.SetAllFolderPermissions)
						{
							goto IL_19D;
						}
						break;
					}
					base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.SetFolderPermissionsTime, num);
					base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.SetFolderPermissionsCount, report.FoldersPrivilegedCount);
				}
				else if (key != GroupMailboxConfigurationAction.ConfigureCalendar)
				{
					if (key != GroupMailboxConfigurationAction.SendWelcomeMessage)
					{
						if (key == GroupMailboxConfigurationAction.GenerateGroupPhoto)
						{
							base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.GroupPhotoUploadTime, num);
						}
					}
					else
					{
						base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.SendWelcomeMessageTime, num);
					}
				}
				else
				{
					base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.ConfigureCalendarTime, num);
				}
				IL_19D:
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append("={");
				AggregatedOperationStatistics? adlatency = value.ADLatency;
				if (adlatency != null)
				{
					stringBuilder.Append("ADC=");
					stringBuilder.Append(adlatency.Value.Count);
					stringBuilder.Append(" AD=");
					stringBuilder.Append((long)adlatency.Value.TotalMilliseconds);
				}
				AggregatedOperationStatistics? rpcLatency = value.RpcLatency;
				if (rpcLatency != null)
				{
					stringBuilder.Append(" RpcC=");
					stringBuilder.Append(rpcLatency.Value.Count);
					stringBuilder.Append(" Rpc=");
					stringBuilder.Append((long)rpcLatency.Value.TotalMilliseconds);
				}
				stringBuilder.Append("} ");
			}
			base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.AdditionalConfigurationDetails, stringBuilder);
			if (report.Warnings.Count > 0)
			{
				base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.ConfigurationWarnings, string.Join(", ", from s in report.Warnings
				select s.ToString()));
			}
			base.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.IsConfigurationExecuted, report.IsConfigurationExecuted);
		}

		private void SetMembership(MailboxSession session, ADUser group, ADUser executingUser)
		{
			if (base.Request.IsAddedMembersSpecified || base.Request.IsRemovedMembersSpecified)
			{
				GroupMailboxAccessLayer.Execute("UpdateGroupMailbox.SetMembership", this.ReadWriteADSession, session, delegate(GroupMailboxAccessLayer accessLayer)
				{
					GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(this.adSession, group);
					UserMailboxLocator[] joiningUsers = Array<UserMailboxLocator>.Empty;
					UserMailboxLocator[] departingUsers = Array<UserMailboxLocator>.Empty;
					UpdateGroupMailbox.Tracer.TraceDebug<ADObjectId, GroupMailboxLocator>((long)this.GetHashCode(), "Adding/removing members in group mailbox {0}, locator={1}", group.Id, groupMailboxLocator);
					using (new StopwatchRequestDetailsLogger(this.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.ResolveMembersTime))
					{
						if (this.Request.IsAddedMembersSpecified)
						{
							this.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.AddedMembersCount, this.Request.AddedMembers.Length);
							joiningUsers = this.BuildUserMailboxLocators(this.Request.AddedMembers).ToArray<UserMailboxLocator>();
						}
						if (this.Request.IsRemovedMembersSpecified)
						{
							this.CallContext.ProtocolLog.Set(UpdateGroupMailboxMetadata.RemovedMembersCount, this.Request.RemovedMembers.Length);
							departingUsers = this.BuildUserMailboxLocators(this.Request.RemovedMembers).ToArray<UserMailboxLocator>();
						}
					}
					using (new StopwatchRequestDetailsLogger(this.CallContext.ProtocolLog, UpdateGroupMailboxMetadata.SetMembershipTime))
					{
						accessLayer.SetMembershipState(executingUser, joiningUsers, departingUsers, groupMailboxLocator);
					}
				});
			}
		}

		private IEnumerable<UserMailboxLocator> BuildUserMailboxLocators(string[] ids)
		{
			if (base.Request.MemberIdentifierType != null && base.Request.MemberIdentifierType.Value == GroupMemberIdentifierType.LegacyExchangeDN)
			{
				return this.BuildUserMailboxLocatorsFromIds<string>(ids, new Func<string[], PropertyDefinition[], Result<ADRawEntry>[]>(this.ReadWriteADSession.FindByLegacyExchangeDNs));
			}
			if (base.Request.MemberIdentifierType != null && base.Request.MemberIdentifierType.Value == GroupMemberIdentifierType.SmtpAddress)
			{
				List<ProxyAddress> list = new List<ProxyAddress>(ids.Length);
				foreach (string text in ids)
				{
					ProxyAddress item;
					if (!ProxyAddress.TryParse(text, out item))
					{
						base.CallContext.ProtocolLog.AppendGenericError("InvalidSmtp", string.IsNullOrEmpty(text) ? "NULL" : text);
						throw new InvalidSmtpAddressException();
					}
					list.Add(item);
				}
				return this.BuildUserMailboxLocatorsFromIds<ProxyAddress>(list.ToArray(), new Func<ProxyAddress[], PropertyDefinition[], Result<ADRawEntry>[]>(this.ReadWriteADSession.FindByProxyAddresses));
			}
			return this.BuildUserMailboxLocatorsFromIds<string>(ids, delegate(string[] externalDirectoryObjectIds, PropertyDefinition[] properties)
			{
				ITenantRecipientSession tenantRecipientSession = this.ReadWriteADSession as ITenantRecipientSession;
				return tenantRecipientSession.FindByExternalDirectoryObjectIds(externalDirectoryObjectIds, properties);
			});
		}

		private IEnumerable<UserMailboxLocator> BuildUserMailboxLocatorsFromIds<TIdentifier>(TIdentifier[] ids, Func<TIdentifier[], PropertyDefinition[], Result<ADRawEntry>[]> adLookup)
		{
			var func = null;
			var func2 = null;
			var func3 = null;
			Result<ADRawEntry>[] array = adLookup(ids, UpdateGroupMailbox.GroupMemberProperties);
			if (array.Any((Result<ADRawEntry> result) => result.Error != null))
			{
				IEnumerable<Result<ADRawEntry>> second = array;
				if (func == null)
				{
					func = ((TIdentifier id, Result<ADRawEntry> adResult) => new
					{
						Id = id,
						ADResult = adResult
					});
				}
				var source = ids.Zip(second, func);
				if (func2 == null)
				{
					func2 = (pair => pair.ADResult.Error != null);
				}
				var source2 = source.Where(func2);
				if (func3 == null)
				{
					func3 = (pair => pair.Id);
				}
				TIdentifier[] values = source2.Select(func3).ToArray<TIdentifier>();
				base.CallContext.ProtocolLog.AppendGenericError("UsersNotFound", string.Join<TIdentifier>("|", values));
				throw new CannotFindUserException(Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorCannotFindUser, CoreResources.ErrorUserADObjectNotFound);
			}
			return from result in array
			select new UserMailboxLocator(this.ReadWriteADSession, (string)result.Data[ADRecipientSchema.ExternalDirectoryObjectId], (string)result.Data[ADRecipientSchema.LegacyExchangeDN]);
		}

		private void SetPermissionsVersion(MailboxSession mailboxSession, int? permissionsVersion)
		{
			if (permissionsVersion != null)
			{
				mailboxSession.Mailbox[MailboxSchema.GroupMailboxPermissionsVersion] = permissionsVersion.Value;
				mailboxSession.Mailbox.Save();
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.WebServicesTracer;

		private static readonly PropertyDefinition[] GroupMemberProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.ExternalDirectoryObjectId
		};

		private IRecipientSession adSession;
	}
}
