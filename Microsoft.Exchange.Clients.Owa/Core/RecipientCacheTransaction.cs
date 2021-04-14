using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class RecipientCacheTransaction : DisposeTrackableBase
	{
		public RecipientCacheTransaction(string configurationName, UserContext userContext)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (userContext.CanActAsOwner)
				{
					this.configuration = UserConfigurationUtilities.GetUserConfiguration(configurationName, userContext);
				}
				disposeGuard.Success();
			}
		}

		public UserConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed)
			{
				if (isDisposing && this.configuration != null)
				{
					this.configuration.Dispose();
				}
				this.disposed = true;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientCacheTransaction>(this);
		}

		private UserConfiguration configuration;

		private bool disposed;
	}
}
