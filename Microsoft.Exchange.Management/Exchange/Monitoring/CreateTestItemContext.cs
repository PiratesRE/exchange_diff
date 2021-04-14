using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Monitoring
{
	internal class CreateTestItemContext
	{
		public CreateTestItemContext(ExchangePrincipal exchangePrincipal, int sleepTime)
		{
			this.exchangePrincipal = exchangePrincipal;
			this.sleepTime = sleepTime;
			this.createTestItemEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
		}

		public EventWaitHandle CreateTestItemEvent
		{
			get
			{
				return this.createTestItemEvent;
			}
		}

		public ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.exchangePrincipal;
			}
		}

		public int SleepTime
		{
			get
			{
				return this.sleepTime;
			}
		}

		public VersionedId TestItemId
		{
			get
			{
				return this.testItemId;
			}
			set
			{
				this.testItemId = value;
			}
		}

		public LocalizedException LocalizedException
		{
			get
			{
				return this.localizedException;
			}
			set
			{
				this.localizedException = value;
			}
		}

		private ExchangePrincipal exchangePrincipal;

		private readonly int sleepTime;

		private VersionedId testItemId;

		private LocalizedException localizedException;

		private EventWaitHandle createTestItemEvent;
	}
}
