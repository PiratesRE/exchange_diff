using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMCallDataRecord")]
	public class GetUMCallDataRecord : UMReportsTaskBase<MailboxIdParameter>
	{
		private new MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.userMailbox = this.ValidateMailboxAndSetOrg(this.Mailbox);
		}

		private ADUser ValidateMailboxAndSetOrg(MailboxIdParameter mbParam)
		{
			IRecipientSession session = this.CreateSessionToResolveRecipientObjects(false);
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mbParam, session, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(mbParam.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotUnique(mbParam.ToString())));
			OrganizationId organizationId = aduser.OrganizationId;
			ADUser result = aduser;
			if (organizationId != null)
			{
				base.CurrentOrganizationId = organizationId;
			}
			return result;
		}

		private IRecipientSession CreateSessionToResolveRecipientObjects(bool scopeToExcecutingUser)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, scopeToExcecutingUser);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 123, "CreateSessionToResolveRecipientObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMCallDataRecord.cs");
		}

		protected override void ProcessMailbox()
		{
			try
			{
				using (IUMCallDataRecordStorage umcallDataRecordsAcessor = InterServerMailboxAccessor.GetUMCallDataRecordsAcessor(this.DataObject))
				{
					CDRData[] umcallDataRecordsForUser = umcallDataRecordsAcessor.GetUMCallDataRecordsForUser(this.userMailbox.LegacyExchangeDN);
					if (umcallDataRecordsForUser != null)
					{
						this.WriteAsConfigObjects(umcallDataRecordsForUser);
					}
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerTransient, null);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerTransient, null);
			}
			catch (ContentIndexingNotEnabledException exception3)
			{
				base.WriteError(exception3, ExchangeErrorCategory.ServerTransient, null);
			}
			catch (CDROperationException exception4)
			{
				base.WriteError(exception4, ErrorCategory.ReadError, null);
			}
			catch (EWSUMMailboxAccessException exception5)
			{
				base.WriteError(exception5, ErrorCategory.ReadError, null);
			}
			catch (UnableToFindUMReportDataException)
			{
			}
		}

		private void WriteAsConfigObjects(CDRData[] cdrs)
		{
			foreach (CDRData cdrdata in cdrs)
			{
				UMCallDataRecord umcallDataRecord = new UMCallDataRecord(this.DataObject.Identity);
				umcallDataRecord.Date = cdrdata.CallStartTime;
				umcallDataRecord.Duration = TimeSpan.FromSeconds((double)cdrdata.CallDuration);
				umcallDataRecord.DialPlan = cdrdata.DialPlanName;
				umcallDataRecord.CallType = cdrdata.CallType;
				umcallDataRecord.CallingNumber = cdrdata.CallerPhoneNumber;
				if (!string.IsNullOrEmpty(cdrdata.DialedString))
				{
					umcallDataRecord.CalledNumber = cdrdata.DialedString;
				}
				else
				{
					umcallDataRecord.CalledNumber = cdrdata.CalledPhoneNumber;
				}
				umcallDataRecord.Gateway = cdrdata.IPGatewayName;
				umcallDataRecord.UserMailboxName = this.userMailbox.DisplayName;
				umcallDataRecord.AudioCodec = cdrdata.AudioQualityMetrics.AudioCodec;
				umcallDataRecord.NMOS = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.NMOS);
				umcallDataRecord.NMOSDegradation = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.NMOSDegradation);
				umcallDataRecord.PercentPacketLoss = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.PacketLoss);
				umcallDataRecord.Jitter = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.Jitter);
				umcallDataRecord.RoundTripMilliseconds = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.RoundTrip);
				umcallDataRecord.BurstLossDurationMilliseconds = Utils.GetNullableAudioQualityMetric(cdrdata.AudioQualityMetrics.BurstDuration);
				this.userMailbox.ResetChangeTracking();
				base.WriteObject(umcallDataRecord);
			}
		}

		private ADUser userMailbox;
	}
}
