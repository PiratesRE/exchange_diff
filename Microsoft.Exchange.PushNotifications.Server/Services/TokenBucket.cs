using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class TokenBucket : ITokenBucket
	{
		public TokenBucket(uint maxBurst, uint rechargeRate, uint rechargeInterval) : this(maxBurst, rechargeRate, rechargeInterval, maxBurst, null)
		{
		}

		public TokenBucket(uint maxBurst, uint rechargeRate, uint rechargeInterval, uint initialBalance, ExDateTime? nextRecharge = null)
		{
			ArgumentValidator.ThrowIfOutOfRange<uint>("maxBurst", maxBurst, 1U, uint.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<uint>("rechargeRate", rechargeRate, 1U, uint.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<uint>("rechargeInterval", rechargeInterval, 1U, uint.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<uint>("initialBalance", initialBalance, 0U, maxBurst);
			this.MaxBurst = maxBurst;
			this.RechargeRate = rechargeRate;
			this.RechargeInterval = rechargeInterval;
			this.currentBalance = initialBalance;
			this.nextRecharge = ((nextRecharge != null) ? nextRecharge.Value : ((ExDateTime)TimeProvider.UtcNow).AddMilliseconds(this.RechargeInterval));
		}

		public uint MaxBurst { get; private set; }

		public uint RechargeRate { get; private set; }

		public uint RechargeInterval { get; private set; }

		public uint CurrentBalance
		{
			get
			{
				this.RechargeBalance();
				return this.currentBalance;
			}
			private set
			{
				this.currentBalance = value;
			}
		}

		public ExDateTime NextRecharge
		{
			get
			{
				this.RechargeBalance();
				return this.nextRecharge;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.CurrentBalance == this.MaxBurst;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.CurrentBalance <= 0U;
			}
		}

		public bool TryTakeToken()
		{
			if (this.CurrentBalance > 0U)
			{
				this.CurrentBalance -= 1U;
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("{{currentBalance:{0}; maxBurst:{1}; rechargeRate:{2}; rechargeInterval:{3}; nextRecharge:{4}}}", new object[]
			{
				this.CurrentBalance,
				this.MaxBurst,
				this.RechargeRate,
				this.RechargeInterval,
				this.NextRecharge
			});
		}

		private void RechargeBalance()
		{
			ExDateTime t = (ExDateTime)TimeProvider.UtcNow;
			if (t < this.nextRecharge)
			{
				return;
			}
			double num = this.RechargeInterval;
			num += t.Subtract(this.nextRecharge).TotalMilliseconds;
			double num2 = Math.Floor(num / this.RechargeInterval);
			if (num2 <= (this.MaxBurst - this.currentBalance) / this.RechargeRate)
			{
				this.currentBalance += (uint)num2 * this.RechargeRate;
			}
			else
			{
				this.currentBalance = this.MaxBurst;
			}
			double num3 = num - num2 * this.RechargeInterval;
			this.nextRecharge = t.AddMilliseconds(this.RechargeInterval - num3);
		}

		private uint currentBalance;

		private ExDateTime nextRecharge;
	}
}
