using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal class BatchedExecutor : Executor
	{
		public BatchedExecutor(ISearchPolicy policy, Type taskType) : base(policy, taskType)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "BatchedExecutor.ctor Task:", taskType);
			this.BatchSize = policy.ExecutionSettings.DiscoveryMaxAllowedExecutorItems;
			this.BatchKeyFactory = BatchedExecutor.BatchByCount;
		}

		public static Func<object, string> BatchByCount
		{
			get
			{
				return (object o) => string.Empty;
			}
		}

		public uint BatchSize { get; set; }

		public Func<object, string> BatchKeyFactory { get; set; }

		protected override void Enqueue(object item)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "BatchedExecutor.Enqueue Item:", item);
			if (item != null)
			{
				string text = this.BatchKeyFactory(item);
				if (text != null)
				{
					lock (this.updateLock)
					{
						List<object> list;
						if (this.items.ContainsKey(text))
						{
							list = this.items[text];
						}
						else
						{
							list = new List<object>();
							this.items[text] = list;
						}
						list.Add(item);
						if ((long)list.Count >= (long)((ulong)this.BatchSize))
						{
							this.EnqueueBatch(text);
						}
						return;
					}
				}
				Recorder.Trace(2L, TraceType.WarningTrace, new object[]
				{
					"BatchedExecutor.Enqueue Null Key Task:",
					base.TaskType,
					"Item:",
					item
				});
			}
		}

		protected override void SignalComplete()
		{
			lock (this.updateLock)
			{
				foreach (string key in this.items.Keys.ToList<string>())
				{
					this.EnqueueBatch(key);
				}
			}
			base.SignalComplete();
		}

		private void EnqueueBatch(string key)
		{
			if (this.items.ContainsKey(key))
			{
				List<object> item = this.items[key];
				this.items.Remove(key);
				base.Enqueue(item);
			}
		}

		private readonly object updateLock = new object();

		private Dictionary<string, List<object>> items = new Dictionary<string, List<object>>();
	}
}
