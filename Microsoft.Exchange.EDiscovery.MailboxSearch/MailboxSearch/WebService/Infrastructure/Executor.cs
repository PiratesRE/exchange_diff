using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal class Executor : IExecutor
	{
		public Executor(ISearchPolicy policy, Type taskType)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.ctor Task:", taskType);
			this.defaultTimeout = policy.ExecutionSettings.SearchTimeout;
			this.Policy = policy;
			this.TaskType = taskType;
			this.ExecutesInParallel = this.Policy.ExecutionSettings.DiscoveryExecutesInParallel;
			this.useRealThreads = this.useRealThreads;
		}

		public uint Concurrency
		{
			get
			{
				return this.threads;
			}
			set
			{
				this.threads = value;
			}
		}

		public object TaskContext { get; set; }

		public ISearchPolicy Policy { get; private set; }

		public ExecutorContext Context { get; private set; }

		protected bool ExecutesInParallel { get; set; }

		private protected Executor ChainedExecutor { protected get; private set; }

		private protected Executor ParentExecutor { protected get; private set; }

		private protected Type TaskType { protected get; private set; }

		protected bool IsSynchronous
		{
			get
			{
				return this.threads == this.Policy.ExecutionSettings.DiscoverySynchronousConcurrency;
			}
		}

		protected bool IsEnqueable
		{
			get
			{
				return !this.queue.IsAddingCompleted && !this.Context.CancellationTokenSource.IsCancellationRequested;
			}
		}

		protected bool IsCancelled
		{
			get
			{
				return this.Context.CancellationTokenSource.IsCancellationRequested;
			}
		}

		public Executor Chain(Executor executor)
		{
			this.EnsureContext();
			this.ChainedExecutor = executor;
			this.ChainedExecutor.Context = this.Context;
			this.ChainedExecutor.ParentExecutor = this;
			return executor;
		}

		public virtual object Process(object item)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.Process Item:", item);
			this.EnsureContext();
			object output;
			using (this.Context)
			{
				if (this.Context.Input != null || !this.IsEnqueable || this.Context.Output != null)
				{
					Recorder.Trace(2L, TraceType.ErrorTrace, new object[]
					{
						"Executor.Process Invalid State  Input:",
						this.Context.Input,
						"Enqueuable:",
						this.IsEnqueable,
						"Output:",
						this.Context.Output
					});
					throw new InvalidOperationException();
				}
				this.Context.Input = item;
				this.Enqueue(item);
				this.SignalComplete();
				bool flag = this.Context.WaitHandle.WaitOne(this.Policy.ExecutionSettings.SearchTimeout);
				if (this.Context.FatalException != null)
				{
					Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.Process Failed Error:", this.Context.FatalException);
					throw new SearchException(this.Context.FatalException);
				}
				if (!flag)
				{
					Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.Process TimedOut");
					Exception ex = new SearchException(KnownError.ErrorSearchTimedOut);
					this.Cancel(ex);
					throw ex;
				}
				Recorder.Trace(2L, TraceType.InfoTrace, "Executor.Process Completed Output:", this.Context.Output);
				output = this.Context.Output;
			}
			return output;
		}

		public virtual void EnqueueNext(object item)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.Chain Item:", item);
			if (this.ChainedExecutor != null)
			{
				this.ChainedExecutor.Enqueue(item);
				return;
			}
			this.Context.Output = item;
		}

		public virtual void Cancel(Exception ex)
		{
			Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.Cancel Called Error:", ex);
			if (!this.IsCancelled)
			{
				Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.Cancel First Cancel Error:", ex);
				this.Context.FatalException = ex;
				this.Context.CancellationTokenSource.Cancel(false);
				this.AttemptComplete();
			}
			Recorder.Record record = this.Policy.Recorder.Start(this.TaskType.Name, TraceType.FatalTrace, true);
			SearchException ex2 = ex as SearchException;
			if (ex2 != null)
			{
				record.Attributes["CancelError"] = ex2.Error.ToString();
				record.Attributes["CancelSource"] = ex2.Source;
			}
			record.Attributes["EX"] = ex.ToString();
			this.Policy.Recorder.End(record);
		}

		public virtual void Fail(Exception ex)
		{
			Recorder.Trace(2L, TraceType.WarningTrace, "Executor.Fail Error:", ex);
			this.Context.Failures.Add(ex);
			Recorder.Record record = this.Policy.Recorder.Start(this.TaskType.Name, TraceType.ErrorTrace, true);
			SearchException ex2 = ex as SearchException;
			if (ex2 != null)
			{
				record.Attributes["FailError"] = ex2.Error.ToString();
				record.Attributes["FailSource"] = ex2.Source;
			}
			record.Attributes["EX"] = ex.ToString();
			this.Policy.Recorder.End(record);
		}

		public override string ToString()
		{
			return string.Format("Executor: {0}", this.TaskType);
		}

		protected virtual void EnsureContext()
		{
			if (this.Context == null)
			{
				this.Context = new ExecutorContext();
			}
		}

		protected virtual void EnsureRecorder()
		{
			if (this.currentRecord == null)
			{
				this.currentRecord = this.Policy.Recorder.Start(this.TaskType.Name, TraceType.InfoTrace, true);
			}
		}

		protected virtual void Enqueue(object item)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, new object[]
			{
				"Executor.Enqueue Item:",
				item,
				"Enqueable:",
				this.IsEnqueable,
				"Sync:",
				this.IsSynchronous
			});
			this.EnsureRecorder();
			if (this.IsEnqueable)
			{
				if (this.IsSynchronous)
				{
					this.RunThread(item);
					return;
				}
				this.queue.Add(item);
				this.EnsureThreads();
			}
		}

		protected virtual void EnsureThreads()
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.EnsureThreads");
			if ((!this.ExecutesInParallel && !this.queue.IsAddingCompleted) || this.IsSynchronous)
			{
				return;
			}
			uint num = Math.Min((uint)this.queue.Count, (uint)((ulong)this.threads - (ulong)((long)this.current)));
			uint num2 = 0U;
			while ((long)this.current < (long)((ulong)this.threads) && num2 < num)
			{
				int num3 = Interlocked.Increment(ref this.current);
				if ((long)num3 < (long)((ulong)this.threads))
				{
					this.CreateThread();
				}
				else
				{
					Interlocked.Decrement(ref this.current);
				}
				num2 += 1U;
			}
		}

		protected virtual void CreateThread()
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.CreateThreads Real:", this.useRealThreads);
			if (this.useRealThreads)
			{
				new Thread(new ParameterizedThreadStart(this.RunThread))
				{
					Name = string.Format("DiscoveryExecutionThread: {0}", this.TaskType)
				}.Start();
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunThread));
		}

		protected virtual void RunThread(object state)
		{
			try
			{
				Recorder.Trace(2L, TraceType.InfoTrace, new object[]
				{
					"Executor.RunThread State:",
					state,
					"Synchronous:",
					this.IsSynchronous,
					"Current:",
					this.current
				});
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						if (this.IsSynchronous)
						{
							Interlocked.Increment(ref this.current);
							this.RunTask(state);
						}
						else
						{
							foreach (object item in this.queue.GetConsumingEnumerable())
							{
								this.RunTask(item);
							}
						}
					}
					catch (OperationCanceledException)
					{
						Recorder.Trace(2L, TraceType.WarningTrace, "Executor.RunThread OperationCancelled");
					}
					catch (SearchException ex2)
					{
						Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.RunThread Search Error:", ex2);
						this.Cancel(ex2);
					}
				});
			}
			catch (GrayException ex)
			{
				Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.RunThread Gray Error:", ex);
				this.Cancel(ex);
			}
			finally
			{
				Interlocked.Decrement(ref this.current);
				Recorder.Trace(2L, TraceType.InfoTrace, new object[]
				{
					"Executor.RunThread Completed Current:",
					this.current,
					"Cancelled:",
					this.IsCancelled
				});
				if (!this.IsCancelled)
				{
					this.AttemptComplete();
				}
			}
		}

		protected virtual void RunTask(object item)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, new object[]
			{
				"Executor.RunTask Item:",
				item,
				"IsCancelled:",
				this.IsCancelled
			});
			if (!this.IsCancelled)
			{
				ITask task = null;
				try
				{
					long timestamp = this.Policy.Recorder.Timestamp;
					task = (Activator.CreateInstance(this.TaskType) as ITask);
					task.State = new SearchTaskContext
					{
						TaskContext = this.TaskContext,
						Executor = this,
						Item = item
					};
					task.Execute(this.defaultQueueDelay, this.defaultTimeout);
					task.Complete(this.defaultQueueDelay, this.defaultTimeout);
					long num = this.Policy.Recorder.Timestamp - timestamp;
					Interlocked.Increment(ref this.itemCount);
					Interlocked.Add(ref this.totalDuration, num);
					IList list = item as IList;
					int num2 = 1;
					if (list != null)
					{
						num2 = list.Count;
					}
					this.batchDurations.Add(new Tuple<long, long, long>(timestamp, num, (long)num2));
				}
				catch (SearchException ex)
				{
					if (task != null)
					{
						task.Cancel();
					}
					this.Cancel(ex);
					Recorder.Trace(2L, TraceType.ErrorTrace, "Executor.RunTask Failed Error:", ex);
				}
			}
		}

		protected virtual void SignalComplete()
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.SignalComplete");
			this.queue.CompleteAdding();
			if (!this.IsSynchronous)
			{
				this.EnsureThreads();
			}
			this.AttemptComplete();
		}

		protected virtual void AttemptComplete()
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Executor.AttemptComplete");
			if ((this.queue.IsCompleted && this.current < 0) || this.IsCancelled)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "Executor.AttemptComplete Started");
				if (this.currentRecord != null)
				{
					this.currentRecord.Attributes["COUNT"] = this.itemCount;
					this.currentRecord.Attributes["WORKDURATION"] = this.totalDuration;
					this.Policy.Recorder.End(this.currentRecord);
					if (this.batchDurations.Count > 0)
					{
						string description = string.Format("{0}Batches", this.currentRecord.Description);
						Recorder.Record record = this.Policy.Recorder.Start(description, TraceType.InfoTrace, false);
						int num = 0;
						foreach (Tuple<long, long, long> tuple in this.batchDurations)
						{
							record.Attributes[string.Format("BATCH{0}START", num)] = tuple.Item1;
							record.Attributes[string.Format("BATCH{0}DURATION", num)] = tuple.Item2;
							record.Attributes[string.Format("BATCH{0}COUNT", num)] = tuple.Item3;
							num++;
						}
						this.Policy.Recorder.End(record);
					}
				}
				if (this.ChainedExecutor != null)
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "Executor.AttemptComplete SignalNext");
					this.ChainedExecutor.SignalComplete();
				}
				else
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "Executor.AttemptComplete SignalRoot");
					if (this.Context != null && !this.Context.IsDisposed && this.Context.WaitHandle != null && !this.Context.WaitHandle.SafeWaitHandle.IsClosed)
					{
						this.Context.WaitHandle.Set();
					}
				}
				Recorder.Trace(2L, TraceType.InfoTrace, "Executor.AttemptComplete Completed");
			}
		}

		protected BlockingCollection<object> queue = new BlockingCollection<object>();

		private readonly TimeSpan defaultQueueDelay = new TimeSpan(0L);

		private readonly TimeSpan defaultTimeout;

		private readonly bool useRealThreads;

		private readonly ConcurrentBag<Tuple<long, long, long>> batchDurations = new ConcurrentBag<Tuple<long, long, long>>();

		private Recorder.Record currentRecord;

		private uint threads = 1U;

		private int current = -1;

		private int itemCount;

		private long totalDuration;
	}
}
