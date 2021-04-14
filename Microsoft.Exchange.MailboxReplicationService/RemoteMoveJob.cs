using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.FreeBusy;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteMoveJob : MoveBaseJob
	{
		protected override void ConfigureProviders(bool continueAfterConfiguringProviders)
		{
			base.ConfigureProviders(continueAfterConfiguringProviders);
			this.principalTranslator = new PrincipalTranslator(base.GetRootMailboxContext().SourceMailboxWrapper.PrincipalMapper, base.GetRootMailboxContext().DestMailboxWrapper.PrincipalMapper);
			base.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				mbxCtx.ConfigTranslators(this.principalTranslator, null);
			});
		}

		protected override void CleanupSourceMailbox(MailboxMover mbxCtx, bool moveIsSuccessful)
		{
			if (base.CachedRequestJob.Direction == RequestDirection.Pull)
			{
				mbxCtx.SourceMailbox.UpdateRemoteHostName(null);
			}
		}

		protected override void CleanupDestinationMailbox(MailboxCopierBase mbxCtx, bool moveIsSuccessful)
		{
			if (base.CachedRequestJob.Direction == RequestDirection.Push)
			{
				mbxCtx.DestMailbox.UpdateRemoteHostName(null);
			}
		}

		protected override void UpdateCachedRequestJob(RequestJobBase request)
		{
			base.UpdateCachedRequestJob(request);
			if ((request.Flags & RequestFlags.RemoteLegacy) != RequestFlags.None)
			{
				if (request.Direction == RequestDirection.Pull)
				{
					base.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
					{
						mbxCtx.SourceMailbox.ConfigADConnection(request.SourceDCName, request.SourceDCName, request.SourceCredential);
					});
					return;
				}
				base.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
				{
					mbxCtx.DestMailbox.ConfigADConnection(request.TargetDCName, request.TargetDCName, request.TargetCredential);
				});
			}
		}

		protected override void MigrateSecurityDescriptors()
		{
			MrsTracer.Service.Function("RemoteMoveJob.MigrateSecurityDescriptors", new object[0]);
			Dictionary<Guid, RawSecurityDescriptor> mailboxSDs = new Dictionary<Guid, RawSecurityDescriptor>();
			base.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				RawSecurityDescriptor mailboxSecurityDescriptor = mbxCtx.SourceMailbox.GetMailboxSecurityDescriptor();
				MrsTracer.Service.Debug("Loaded MailboxSD: {0}", new object[]
				{
					CommonUtils.GetSDDLString(mailboxSecurityDescriptor)
				});
				if (this.CleanupMailboxSD)
				{
					this.RemoveExchangeServersDenyACE(mailboxSecurityDescriptor, mbxCtx);
				}
				mailboxSDs[mbxCtx.SourceMailboxGuid] = mailboxSecurityDescriptor;
			});
			RawSecurityDescriptor rawSecurityDescriptor = base.GetRootMailboxContext().SourceMailbox.GetUserSecurityDescriptor();
			MrsTracer.Service.Debug("Loaded source UserSD: {0}", new object[]
			{
				CommonUtils.GetSDDLString(rawSecurityDescriptor)
			});
			foreach (RawSecurityDescriptor sd in mailboxSDs.Values)
			{
				this.principalTranslator.EnumerateSecurityDescriptor(sd);
			}
			this.principalTranslator.EnumerateSecurityDescriptor(rawSecurityDescriptor);
			base.ForeachMailboxContext(delegate(MailboxMover mbxCtx)
			{
				RawSecurityDescriptor mailboxSecurityDescriptor = this.principalTranslator.TranslateSecurityDescriptor(mailboxSDs[mbxCtx.SourceMailboxGuid], TranslateSecurityDescriptorFlags.None);
				mbxCtx.DestMailbox.SetMailboxSecurityDescriptor(mailboxSecurityDescriptor);
			});
			RawSecurityDescriptor userSecurityDescriptor = base.GetRootMailboxContext().DestMailbox.GetUserSecurityDescriptor();
			MrsTracer.Service.Debug("Loaded target UserSD: {0}", new object[]
			{
				CommonUtils.GetSDDLString(userSecurityDescriptor)
			});
			if (rawSecurityDescriptor == null || rawSecurityDescriptor.DiscretionaryAcl == null)
			{
				base.Report.Append(MrsStrings.ReportSourceSDCannotBeRead);
				return;
			}
			if (userSecurityDescriptor == null || userSecurityDescriptor.DiscretionaryAcl == null)
			{
				base.Report.Append(MrsStrings.ReportDestinationSDCannotBeRead);
				return;
			}
			rawSecurityDescriptor = this.principalTranslator.TranslateSecurityDescriptor(rawSecurityDescriptor, TranslateSecurityDescriptorFlags.ExcludeUnmappedACEs);
			CommonSecurityDescriptor commonSecurityDescriptor = new CommonSecurityDescriptor(true, true, userSecurityDescriptor);
			bool flag = false;
			foreach (GenericAce genericAce in rawSecurityDescriptor.DiscretionaryAcl)
			{
				if (!genericAce.IsInherited)
				{
					switch (genericAce.AceType)
					{
					case AceType.AccessAllowedObject:
					case AceType.AccessDeniedObject:
					{
						ObjectAce objectAce = genericAce as ObjectAce;
						if (objectAce.AccessMask == 256 && (objectAce.ObjectAceFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None && (!(objectAce.ObjectAceType != WellKnownGuid.SendAsExtendedRightGuid) || !(objectAce.ObjectAceType != WellKnownGuid.ReceiveAsExtendedRightGuid)))
						{
							commonSecurityDescriptor.DiscretionaryAcl.AddAccess((objectAce.AceType == AceType.AccessAllowedObject) ? AccessControlType.Allow : AccessControlType.Deny, objectAce.SecurityIdentifier, objectAce.AccessMask, objectAce.InheritanceFlags, objectAce.PropagationFlags, objectAce.ObjectAceFlags, objectAce.ObjectAceType, objectAce.InheritedObjectAceType);
							flag = true;
						}
						break;
					}
					}
				}
			}
			if (flag)
			{
				byte[] binaryForm = new byte[commonSecurityDescriptor.BinaryLength];
				commonSecurityDescriptor.GetBinaryForm(binaryForm, 0);
				RawSecurityDescriptor userSecurityDescriptor2 = new RawSecurityDescriptor(binaryForm, 0);
				try
				{
					base.GetRootMailboxContext().DestMailbox.SetUserSecurityDescriptor(userSecurityDescriptor2);
				}
				catch (LocalizedException ex)
				{
					if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
					{
						WellKnownException.Transient
					}))
					{
						throw;
					}
					LocalizedString localizedString = MrsStrings.ReportFailedToUpdateUserSD2(CommonUtils.GetFailureType(ex));
					base.Report.Append(localizedString, ex, ReportEntryFlags.Target);
					base.Warnings.Add(localizedString);
				}
			}
		}

		protected override void UpdateMovedMailbox()
		{
			ReportEntry[] entries = null;
			ADUser aduser = base.GetRootMailboxContext().SourceMailbox.GetADUser();
			ConfigurableObjectXML configObject = ConfigurableObjectXML.Create(aduser);
			base.Report.Append(MrsStrings.ReportSourceMailboxBeforeFinalization2(aduser.ToString(), aduser.OriginatingServer), configObject, ReportEntryFlags.Source | ReportEntryFlags.Before);
			ADUser aduser2 = base.GetRootMailboxContext().DestMailbox.GetADUser();
			configObject = ConfigurableObjectXML.Create(aduser2);
			base.Report.Append(MrsStrings.ReportTargetMailUserBeforeFinalization2(aduser2.ToString(), aduser2.OriginatingServer), configObject, ReportEntryFlags.Target | ReportEntryFlags.Before);
			bool isFromDatacenter = aduser.IsFromDatacenter;
			bool isFromDatacenter2 = aduser2.IsFromDatacenter;
			if (base.CachedRequestJob.PrimaryIsMoving)
			{
				CommonUtils.ValidateTargetDeliveryDomain(aduser2.EmailAddresses, base.CachedRequestJob.TargetDeliveryDomain);
				MailboxCopierBase rootMailboxContext = base.GetRootMailboxContext();
				if (!isFromDatacenter && isFromDatacenter2)
				{
					rootMailboxContext.SyncState.ExternalLegacyExchangeDN = FreeBusyFolder.GetExternalLegacyDN(aduser);
				}
				else if (isFromDatacenter && !isFromDatacenter2)
				{
					string mdbLegDN = base.GetRootMailboxContext().DestMailbox.GetMailboxInformation().MdbLegDN;
					rootMailboxContext.SyncState.InternalLegacyExchangeDN = FreeBusyFolder.GetInternalLegacyDN(aduser2, mdbLegDN);
				}
				List<PropertyUpdateXML> list = new List<PropertyUpdateXML>();
				if (rootMailboxContext.SyncState.ExternalLegacyExchangeDN != null)
				{
					this.AddX500ProxyAddressIfNeeded(list, aduser2, rootMailboxContext.SyncState.ExternalLegacyExchangeDN, aduser.Identity.ToString());
				}
				if (rootMailboxContext.SyncState.InternalLegacyExchangeDN != null)
				{
					PropertyUpdateXML.Add(list, ADRecipientSchema.LegacyExchangeDN, rootMailboxContext.SyncState.InternalLegacyExchangeDN, PropertyUpdateOperation.Replace);
					this.AddX500ProxyAddressIfNeeded(list, aduser2, aduser2.LegacyExchangeDN, aduser2.Identity.ToString());
				}
				aduser.LinkedMasterAccount = XMLSerializableBase.Serialize(list.ToArray(), false);
			}
			MrsTracer.Service.Debug("Updating destination mailbox...", new object[0]);
			UpdateMovedMailboxOperation op;
			Guid newDatabaseGuid;
			Guid newArchiveDatabaseGuid;
			ArchiveStatusFlags archiveStatus;
			string archiveDomain;
			if (base.CachedRequestJob.PrimaryOnly)
			{
				op = UpdateMovedMailboxOperation.MorphToMailbox;
				newDatabaseGuid = base.CachedRequestJob.TargetMDBGuid;
				newArchiveDatabaseGuid = ((aduser2.ArchiveDatabase != null) ? aduser2.ArchiveDatabase.ObjectGuid : Guid.Empty);
				archiveStatus = ArchiveStatusFlags.None;
				archiveDomain = ((aduser2.ArchiveDatabase == null) ? base.CachedRequestJob.ArchiveDomain : null);
			}
			else if (base.CachedRequestJob.ArchiveOnly)
			{
				op = UpdateMovedMailboxOperation.UpdateArchiveOnly;
				newDatabaseGuid = Guid.Empty;
				newArchiveDatabaseGuid = base.CachedRequestJob.TargetArchiveMDBGuid;
				archiveStatus = ((aduser2.Database == null) ? ArchiveStatusFlags.Active : ArchiveStatusFlags.None);
				archiveDomain = null;
			}
			else
			{
				op = UpdateMovedMailboxOperation.MorphToMailbox;
				newDatabaseGuid = base.CachedRequestJob.TargetMDBGuid;
				newArchiveDatabaseGuid = base.CachedRequestJob.TargetArchiveMDBGuid;
				archiveDomain = null;
				archiveStatus = ArchiveStatusFlags.None;
			}
			UpdateMovedMailboxFlags updateMovedMailboxFlags = UpdateMovedMailboxFlags.None;
			if (base.CachedRequestJob.SkipMailboxReleaseCheck)
			{
				updateMovedMailboxFlags |= UpdateMovedMailboxFlags.SkipMailboxReleaseCheck;
			}
			if (base.CachedRequestJob.SkipProvisioningCheck)
			{
				updateMovedMailboxFlags |= UpdateMovedMailboxFlags.SkipProvisioningCheck;
			}
			try
			{
				base.GetRootMailboxContext().DestMailbox.UpdateMovedMailbox(op, aduser, base.CachedRequestJob.DestDomainControllerToUpdate, out entries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags, null, null);
			}
			finally
			{
				base.AppendReportEntries(entries);
			}
		}

		protected override void UpdateSourceMailbox()
		{
			ReportEntry[] entries = null;
			MailboxCopierBase rootCtx = base.GetRootMailboxContext();
			ADUser srcUser = rootCtx.SourceMailbox.GetADUser();
			ADUser destUser = null;
			ConfigurableObjectXML configObj;
			CommonUtils.CatchKnownExceptions(delegate
			{
				destUser = rootCtx.DestMailbox.GetADUser();
				configObj = ConfigurableObjectXML.Create(destUser);
				this.Report.Append(MrsStrings.ReportTargetMailboxAfterFinalization2(destUser.ToString(), destUser.OriginatingServer), configObj, ReportEntryFlags.Target | ReportEntryFlags.After);
			}, delegate(Exception failure)
			{
				this.Report.Append(MrsStrings.ReportUnableToLoadDestinationUser(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
				FailureLog.Write(this.RequestJobGuid, failure, false, RequestState.Cleanup, SyncStage.CleanupUnableToLoadTargetMailbox, null, null);
				destUser = (ADUser)srcUser.Clone();
			});
			if (base.CachedRequestJob.PrimaryIsMoving)
			{
				SmtpAddress? smtpAddress = null;
				foreach (ProxyAddress proxyAddress in destUser.EmailAddresses)
				{
					SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
					if (smtpProxyAddress != null)
					{
						SmtpAddress value = new SmtpAddress(smtpProxyAddress.SmtpAddress);
						if (StringComparer.OrdinalIgnoreCase.Equals(value.Domain, base.CachedRequestJob.TargetDeliveryDomain))
						{
							smtpAddress = new SmtpAddress?(value);
							break;
						}
					}
				}
				if (smtpAddress == null)
				{
					LocalizedString localizedString = MrsStrings.ReportUnableToComputeTargetAddress(base.CachedRequestJob.TargetDeliveryDomain, destUser.PrimarySmtpAddress.ToString());
					base.Report.Append(localizedString);
					base.Warnings.Add(localizedString);
					FailureLog.Write(base.RequestJobGuid, new MailboxReplicationTransientException(localizedString), false, RequestState.Cleanup, SyncStage.CleanupUnableToComputeTargetAddress, null, null);
					smtpAddress = new SmtpAddress?(destUser.PrimarySmtpAddress);
				}
				SmtpProxyAddress smtpProxyAddress2 = new SmtpProxyAddress(smtpAddress.Value.ToString(), true);
				destUser.ExternalEmailAddress = smtpProxyAddress2;
				List<PropertyUpdateXML> list = new List<PropertyUpdateXML>();
				PropertyUpdateXML.Add(list, ADRecipientSchema.ExternalEmailAddress, smtpProxyAddress2, PropertyUpdateOperation.Replace);
				if (rootCtx.SyncState.ExternalLegacyExchangeDN != null)
				{
					PropertyUpdateXML.Add(list, ADRecipientSchema.LegacyExchangeDN, rootCtx.SyncState.ExternalLegacyExchangeDN, PropertyUpdateOperation.Replace);
					this.AddX500ProxyAddressIfNeeded(list, srcUser, srcUser.LegacyExchangeDN, srcUser.Identity.ToString());
				}
				if (rootCtx.SyncState.InternalLegacyExchangeDN != null)
				{
					this.AddX500ProxyAddressIfNeeded(list, srcUser, rootCtx.SyncState.InternalLegacyExchangeDN, destUser.Identity.ToString());
				}
				destUser.LinkedMasterAccount = XMLSerializableBase.Serialize(list.ToArray(), false);
			}
			try
			{
				Guid? newMailboxContainerGuid = null;
				CrossTenantObjectId newUnifiedMailboxId = null;
				MrsTracer.Service.Debug("Updating source mailbox...", new object[0]);
				UpdateMovedMailboxOperation op;
				Guid newArchiveDatabaseGuid;
				ArchiveStatusFlags archiveStatus;
				string archiveDomain;
				if (base.CachedRequestJob.PrimaryOnly)
				{
					op = UpdateMovedMailboxOperation.MorphToMailUser;
					newArchiveDatabaseGuid = ((srcUser.ArchiveDatabase != null) ? srcUser.ArchiveDatabase.ObjectGuid : Guid.Empty);
					archiveStatus = ((srcUser.ArchiveDatabase != null) ? ArchiveStatusFlags.Active : ArchiveStatusFlags.None);
					archiveDomain = null;
				}
				else if (base.CachedRequestJob.ArchiveOnly)
				{
					op = UpdateMovedMailboxOperation.UpdateArchiveOnly;
					newArchiveDatabaseGuid = Guid.Empty;
					archiveStatus = ArchiveStatusFlags.None;
					archiveDomain = ((srcUser.Database != null) ? base.CachedRequestJob.ArchiveDomain : null);
					newMailboxContainerGuid = srcUser.MailboxContainerGuid;
					newUnifiedMailboxId = srcUser.UnifiedMailbox;
				}
				else
				{
					op = UpdateMovedMailboxOperation.MorphToMailUser;
					newArchiveDatabaseGuid = Guid.Empty;
					archiveDomain = null;
					archiveStatus = ArchiveStatusFlags.None;
				}
				UpdateMovedMailboxFlags updateMovedMailboxFlags = UpdateMovedMailboxFlags.None;
				if (base.CachedRequestJob.SkipMailboxReleaseCheck)
				{
					updateMovedMailboxFlags |= UpdateMovedMailboxFlags.SkipMailboxReleaseCheck;
				}
				rootCtx.SourceMailbox.UpdateMovedMailbox(op, destUser, base.CachedRequestJob.SourceDomainControllerToUpdate ?? srcUser.OriginatingServer, out entries, Guid.Empty, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags, newMailboxContainerGuid, newUnifiedMailboxId);
			}
			finally
			{
				base.AppendReportEntries(entries);
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				srcUser = rootCtx.SourceMailbox.GetADUser();
				configObj = ConfigurableObjectXML.Create(srcUser);
				this.Report.Append(MrsStrings.ReportSourceMailUserAfterFinalization2(srcUser.ToString(), srcUser.OriginatingServer), configObj, ReportEntryFlags.Source | ReportEntryFlags.After);
			}, null);
		}

		private void AddX500ProxyAddressIfNeeded(List<PropertyUpdateXML> updates, ADUser user, string legacyDN, string userCorrespondingToLegDn)
		{
			Exception ex = null;
			try
			{
				CustomProxyAddress customProxyAddress = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.X500, legacyDN, false);
				if (!user.EmailAddresses.Contains(customProxyAddress))
				{
					PropertyUpdateXML.Add(updates, ADRecipientSchema.EmailAddresses, customProxyAddress.ToString(), PropertyUpdateOperation.AddValues);
				}
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (LocalizedException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				LocalizedString localizedString = MrsStrings.RecipientInvalidLegDN(userCorrespondingToLegDn, legacyDN ?? "<empty>");
				base.Report.Append(localizedString, ex, ReportEntryFlags.None);
				base.Warnings.Add(localizedString);
			}
		}

		private PrincipalTranslator principalTranslator;
	}
}
