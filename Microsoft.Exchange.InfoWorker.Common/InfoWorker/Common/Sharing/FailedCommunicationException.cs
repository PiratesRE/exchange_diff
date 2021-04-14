using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class FailedCommunicationException : SharingSynchronizationException
	{
		public FailedCommunicationException() : base(Strings.FailedCommunicationException)
		{
		}

		public FailedCommunicationException(Exception innerException) : base(Strings.FailedCommunicationException, innerException)
		{
		}

		internal FailedCommunicationException(Exception innerException, DelegationTokenRequest delegationTokenRequest) : base(Strings.FailedCommunicationException, innerException)
		{
			this.Data.Add("Delegation Token Request", delegationTokenRequest);
		}

		private const string DelegationTokenRequestAdditionalData = "Delegation Token Request";
	}
}
