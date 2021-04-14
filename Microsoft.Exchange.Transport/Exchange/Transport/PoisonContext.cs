using System;

namespace Microsoft.Exchange.Transport
{
	internal abstract class PoisonContext
	{
		internal PoisonContext(MessageProcessingSource msgSource)
		{
			this.source = msgSource;
		}

		internal MessageProcessingSource Source
		{
			get
			{
				return this.source;
			}
		}

		private MessageProcessingSource source;
	}
}
