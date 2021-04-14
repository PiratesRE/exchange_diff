using System;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal abstract class LiveFederationProvision : FederationProvision
	{
		protected LiveFederationProvision(string certificateThumbprint, string applicationIdentifier, Task task)
		{
			this.certificateThumbprint = certificateThumbprint;
			this.applicationIdentifier = applicationIdentifier;
			this.task = task;
		}

		protected void ReserveDomain(string domain, string applicationIdentifier, ManageDelegationClient client, LocalizedString errorProofDomainOwnership, LiveFederationProvision.GetDomainStateDelegate getDomainState)
		{
			LocalizedString localizedString = Strings.SetLiveFedDomainReserveRequest(domain);
			this.WriteVerbose(localizedString);
			this.WriteProgress(localizedString, localizedString, 0);
			try
			{
				client.ReserveDomain(applicationIdentifier, domain, "ExchangeConnector");
			}
			catch (LiveDomainServicesException ex)
			{
				if (ex.DomainError != null && ex.DomainError.Value == DomainError.ProofOfOwnershipNotValid)
				{
					throw new DomainProofOwnershipException(errorProofDomainOwnership, ex);
				}
				throw new UnableToReserveDomainException(domain, applicationIdentifier, ex.Message, ex);
			}
			this.WriteVerbose(Strings.SetLiveFedDomainReserveResponse(domain));
			DomainState domainState = this.WaitWhilePendingState(DomainState.PendingActivation, localizedString, getDomainState);
			this.WriteProgress(localizedString, localizedString, 100);
			if (domainState == DomainState.Unknown)
			{
				this.WriteVerbose(Strings.ErrorCannotGetDomainStatusFromPartnerSTS(domain, applicationIdentifier, string.Empty));
				return;
			}
			if (DomainState.PendingRelease == domainState)
			{
				throw new UnableToReserveDomainException(domain, applicationIdentifier, DomainState.PendingRelease.ToString());
			}
		}

		protected void ReleaseDomain(string domain, string applicationIdentifier, bool force, ManageDelegationClient client, LiveFederationProvision.GetDomainStateDelegate getDomainState)
		{
			LocalizedString localizedString = Strings.RemoveLiveFedDomainReserveRequest(domain);
			this.WriteVerbose(localizedString);
			this.WriteProgress(localizedString, localizedString, 0);
			try
			{
				client.ReleaseDomain(this.ApplicationIdentifier, domain);
			}
			catch (LiveDomainServicesException ex)
			{
				if (!force)
				{
					throw new UnableToReleaseDomainException(domain, applicationIdentifier, ex.Message, ex);
				}
				this.WriteVerbose(Strings.ErrorUnableToReleaseDomain(domain, applicationIdentifier, ex.Message));
			}
			this.WriteVerbose(Strings.RemoveLiveFedDomainReserveResponse(domain));
			this.WaitWhilePendingState(DomainState.PendingRelease, localizedString, getDomainState);
			this.WriteProgress(localizedString, localizedString, 100);
		}

		protected void AddUri(string domain, string applicationIdentifier, ManageDelegationClient client, LocalizedString errorProofDomainOwnership)
		{
			this.WriteVerbose(Strings.SetLiveFedUriReserveRequest(domain));
			try
			{
				client.AddUri(this.ApplicationIdentifier, domain);
			}
			catch (LiveDomainServicesException ex)
			{
				if (ex.DomainError != null && ex.DomainError.Value == DomainError.ProofOfOwnershipNotValid)
				{
					throw new DomainProofOwnershipException(errorProofDomainOwnership, ex);
				}
				throw new UnableToReserveUriException(domain, domain, applicationIdentifier, ex.Message, ex);
			}
			this.WriteVerbose(Strings.SetLiveFedUriReserveResponse(domain));
		}

		protected void RemoveUri(ManageDelegationClient client, string uri, bool force)
		{
			this.WriteVerbose(Strings.RemoveLiveFedUriReserveRequest(uri));
			try
			{
				client.RemoveUri(this.ApplicationIdentifier, uri);
				this.WriteVerbose(Strings.RemoveLiveFedUriReserveResponse(uri));
			}
			catch (LiveDomainServicesException ex)
			{
				if (!force)
				{
					throw new UnableToReleaseUriException(uri, uri, this.ApplicationIdentifier, ex.Message, ex);
				}
				this.WriteVerbose(Strings.ErrorUnableToReleaseUri(uri, uri, this.ApplicationIdentifier, ex.Message));
			}
		}

		protected string ApplicationIdentifier
		{
			get
			{
				if (string.IsNullOrEmpty(this.applicationIdentifier))
				{
					throw new ArgumentNullException("applicationIdentifier");
				}
				return this.applicationIdentifier;
			}
		}

		protected string CertificateThumbprint
		{
			get
			{
				if (string.IsNullOrEmpty(this.certificateThumbprint))
				{
					throw new ArgumentNullException("certificateThumbprint");
				}
				return this.certificateThumbprint;
			}
		}

		protected WriteVerboseDelegate WriteVerbose
		{
			get
			{
				return new WriteVerboseDelegate(this.task.WriteVerbose);
			}
		}

		private DomainState WaitWhilePendingState(DomainState pendingState, LocalizedString pendingMessage, LiveFederationProvision.GetDomainStateDelegate getDomainState)
		{
			int num = 20;
			int num2 = (int)((double)(100 - num) / (LiveFederationProvision.PendingStateWait.TotalSeconds / LiveFederationProvision.PendingStateWaitInterval.TotalSeconds));
			DateTime t = DateTime.UtcNow + LiveFederationProvision.PendingStateWait;
			DomainState domainState = DomainState.Unknown;
			for (;;)
			{
				this.WriteProgress(pendingMessage, pendingMessage, num);
				try
				{
					domainState = getDomainState();
				}
				catch (LiveDomainServicesException)
				{
					domainState = DomainState.Unknown;
				}
				if (domainState != pendingState || DateTime.UtcNow > t)
				{
					break;
				}
				Thread.Sleep(LiveFederationProvision.PendingStateWaitInterval);
				num += num2;
			}
			return domainState;
		}

		private void WriteProgress(LocalizedString activity, LocalizedString statusDescription, int percentCompleted)
		{
			ExProgressRecord exProgressRecord = new ExProgressRecord(0, activity, statusDescription);
			exProgressRecord.PercentComplete = percentCompleted;
			this.task.WriteProgress(exProgressRecord);
		}

		private static readonly TimeSpan PendingStateWait = TimeSpan.FromSeconds(60.0);

		private static readonly TimeSpan PendingStateWaitInterval = TimeSpan.FromSeconds(5.0);

		private readonly string applicationIdentifier;

		private readonly string certificateThumbprint;

		private Task task;

		protected delegate DomainState GetDomainStateDelegate();
	}
}
