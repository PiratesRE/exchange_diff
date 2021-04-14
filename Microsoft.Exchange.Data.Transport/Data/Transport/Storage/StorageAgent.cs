using System;

namespace Microsoft.Exchange.Data.Transport.Storage
{
	internal abstract class StorageAgent : Agent
	{
		protected event LoadedMessageEventHandler OnLoadedMessage
		{
			add
			{
				base.AddHandler("OnLoadedMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnLoadedMessage");
			}
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				return;
			}
			if (eventTopic != null)
			{
				if (!(eventTopic == "OnLoadedMessage"))
				{
					return;
				}
				((LoadedMessageEventHandler)@delegate)((StorageEventSource)source, (StorageEventArgs)e);
			}
		}
	}
}
