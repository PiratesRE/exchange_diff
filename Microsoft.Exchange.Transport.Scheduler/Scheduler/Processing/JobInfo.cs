using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class JobInfo
	{
		public JobInfo(DateTime startTime, IEnumerable<IMessageScope> scopes)
		{
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			this.id = Guid.NewGuid();
			this.startTime = startTime;
			this.scopes = scopes;
		}

		public Task ExecutingTask { get; set; }

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public IEnumerable<IMessageScope> Scopes
		{
			get
			{
				return this.scopes;
			}
		}

		private readonly Guid id;

		private readonly DateTime startTime;

		private readonly IEnumerable<IMessageScope> scopes;
	}
}
