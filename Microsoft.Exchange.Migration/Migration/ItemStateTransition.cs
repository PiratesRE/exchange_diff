using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	internal class ItemStateTransition
	{
		public ItemStateTransition(MigrationUserStatus status, SubscriptionStatusChangedResponse response, LocalizedException error) : this(status, response, error, true)
		{
		}

		public ItemStateTransition(MigrationUserStatus status, SubscriptionStatusChangedResponse response, LocalizedException error, bool supportsIncrementalSync)
		{
			this.response = response;
			this.status = status;
			this.error = error;
			this.SupportsIncrementalSync = supportsIncrementalSync;
		}

		public MigrationUserStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public SubscriptionStatusChangedResponse Response
		{
			get
			{
				return this.response;
			}
		}

		public LocalizedException Error
		{
			get
			{
				return this.error;
			}
		}

		public bool SupportsIncrementalSync { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ItemTransition: Status: {0}, Response: {1}, Message: {2}", new object[]
			{
				this.Status,
				this.Response,
				this.Error
			});
		}

		private readonly SubscriptionStatusChangedResponse response;

		private readonly MigrationUserStatus status;

		private readonly LocalizedException error;
	}
}
