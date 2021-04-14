using System;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal class MessageThrottlingComponent : ITransportComponent
	{
		public MessageThrottlingManager MessageThrottlingManager
		{
			get
			{
				this.ThrowIfNotLoaded();
				return this.messageThrottlingManager;
			}
		}

		public void Load()
		{
			this.ThrowIfLoaded();
			this.messageThrottlingManager = new MessageThrottlingManager();
		}

		public void Unload()
		{
			this.ThrowIfNotLoaded();
			this.messageThrottlingManager = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private void ThrowIfLoaded()
		{
			if (this.messageThrottlingManager != null)
			{
				throw new InvalidOperationException("Message Throttling Component is already loaded.");
			}
		}

		private void ThrowIfNotLoaded()
		{
			if (this.messageThrottlingManager == null)
			{
				throw new InvalidOperationException("Message Throttling Component is not loaded.");
			}
		}

		private MessageThrottlingManager messageThrottlingManager;
	}
}
