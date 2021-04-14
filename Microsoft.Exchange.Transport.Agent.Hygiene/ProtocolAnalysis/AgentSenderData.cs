using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport.Agent.Hygiene;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal class AgentSenderData : SenderData
	{
		public AgentSenderData(DateTime tsCreate) : base(tsCreate)
		{
			this.msgsNormValid = 0;
			this.msgsNormInvalid = 0;
			this.helloDomain = string.Empty;
			this.reverseDns = string.Empty;
			this.msgStarted = false;
		}

		public string HelloDomain
		{
			get
			{
				return this.helloDomain;
			}
		}

		public string ReverseDns
		{
			get
			{
				return this.reverseDns;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.reverseDns = value;
				}
			}
		}

		public void OnMailFrom()
		{
			if (this.msgStarted)
			{
				throw new InvalidOperationException("SenderData::OnMailFrom called while msg_started being true");
			}
			this.msgStarted = true;
		}

		public override void OnValidRecipient(string recipient)
		{
			if (this.msgStarted)
			{
				this.msgsNormValid++;
				base.OnValidRecipient(recipient);
				return;
			}
			throw new InvalidOperationException("SenderData::OnValidRecipient called while msg_started being false");
		}

		public override void OnInvalidRecipient(string recipient)
		{
			if (this.msgStarted)
			{
				this.msgsNormInvalid++;
				base.OnInvalidRecipient(recipient);
				return;
			}
			throw new InvalidOperationException("SenderData::OnInvalidRecipient called while msg_started being false");
		}

		public override void OnEndOfData(int scl, long msglen, CallerIdStatus status)
		{
			if (scl < 0 || scl > 10)
			{
				throw new LocalizedException(AgentStrings.InvalidScl(scl));
			}
			if (this.msgStarted)
			{
				base.OnEndOfData(scl, msglen, status);
				this.ValidScl[scl] += this.msgsNormValid;
				this.InvalidScl[scl] += this.msgsNormInvalid;
				this.msgsNormValid = 0;
				this.msgsNormInvalid = 0;
				this.msgStarted = false;
				return;
			}
			throw new InvalidOperationException("SenderData::OnEndOfData called while msg_started being false");
		}

		public void OnEndOfSession(string helloDomain)
		{
			if (this.msgStarted)
			{
				throw new InvalidOperationException("SenderData::OnEndOfSession called while msg_started being true");
			}
			if (!this.msgStarted && !string.IsNullOrEmpty(helloDomain))
			{
				this.helloDomain = helloDomain;
			}
		}

		private int msgsNormValid;

		private int msgsNormInvalid;

		private string helloDomain;

		private string reverseDns;

		private bool msgStarted;
	}
}
