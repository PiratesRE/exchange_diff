using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MobileTransport;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class TextMessageDeliveringPipeline : DisposeTrackableBase
	{
		private ThreadSafeQueue<TextMessageDeliveryContext> DispatchingQueue { get; set; }

		private ThreadSafeQueue<TextMessageDeliveryContext> TranslatingQueue { get; set; }

		private ThreadSafeQueue<TextMessageDeliveryContext> ComposingQueue { get; set; }

		private ThreadSafeQueue<TextMessageDeliveryContext> DeliveringQueue { get; set; }

		private ThreadSafeQueue<TextMessageDeliveryContext> ReportingQueue { get; set; }

		private ThreadSafeQueue<TextMessageDeliveryContext> CleaningQueue { get; set; }

		private QueuePorter<TextMessageDeliveryContext> Worker { get; set; }

		public TextMessageDeliveringPipeline()
		{
			this.DispatchingQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			this.TranslatingQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			this.ComposingQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			this.DeliveringQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			this.ReportingQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			this.CleaningQueue = new ThreadSafeQueue<TextMessageDeliveryContext>();
			QueuePorterWorkingContext<TextMessageDeliveryContext>[] contexts = new QueuePorterWorkingContext<TextMessageDeliveryContext>[]
			{
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.DispatchingQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.DispatcherEventHandler), 1),
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.TranslatingQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.TranslatorEventHandler), 1),
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.ComposingQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.ComposerEventHandler), 1),
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.DeliveringQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.DelivererEventHandler), 1),
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.ReportingQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.ReporterEventHandler), 1),
				new QueuePorterWorkingContext<TextMessageDeliveryContext>(this.CleaningQueue, new QueueDataAvailableEventHandler<TextMessageDeliveryContext>(this.CleanerEventHandler), 1)
			};
			this.Worker = new QueuePorter<TextMessageDeliveryContext>(contexts, false);
			this.Start();
		}

		public void Start()
		{
			base.CheckDisposed();
			this.Worker.Start();
		}

		public void Stop()
		{
			base.CheckDisposed();
			this.Worker.Stop();
		}

		public void Deliver(TextMessageDeliveryContext context)
		{
			base.CheckDisposed();
			this.DispatchingQueue.Enqueue(context);
		}

		private void DispatcherEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			bool flag = false;
			TextMessageDeliverer textMessageDeliverer = new TextMessageDeliverer(e.Item);
			try
			{
				flag = textMessageDeliverer.Stage0Dispatch();
			}
			catch (LocalizedException e2)
			{
				textMessageDeliverer.GenerateDsn(e2);
			}
			finally
			{
				if (flag)
				{
					this.TranslatingQueue.Enqueue(e.Item);
				}
				else
				{
					this.CleaningQueue.Enqueue(e.Item);
				}
			}
		}

		private void TranslatorEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			bool flag = false;
			TextMessageDeliverer textMessageDeliverer = new TextMessageDeliverer(e.Item);
			try
			{
				textMessageDeliverer.Stage1Translate();
				flag = true;
			}
			catch (LocalizedException e2)
			{
				textMessageDeliverer.GenerateDsn(e2);
			}
			finally
			{
				if (flag)
				{
					this.ComposingQueue.Enqueue(e.Item);
				}
				else
				{
					this.CleaningQueue.Enqueue(e.Item);
				}
			}
		}

		private void ComposerEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			bool flag = false;
			TextMessageDeliverer textMessageDeliverer = new TextMessageDeliverer(e.Item);
			try
			{
				textMessageDeliverer.Stage2Compose();
				flag = true;
			}
			catch (LocalizedException e2)
			{
				textMessageDeliverer.GenerateDsn(e2);
			}
			finally
			{
				if (flag)
				{
					this.DeliveringQueue.Enqueue(e.Item);
				}
				else
				{
					this.CleaningQueue.Enqueue(e.Item);
				}
			}
		}

		private void DelivererEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			bool flag = false;
			TextMessageDeliverer textMessageDeliverer = new TextMessageDeliverer(e.Item);
			try
			{
				textMessageDeliverer.Stage3Deliver();
				flag = true;
			}
			catch (LocalizedException e2)
			{
				textMessageDeliverer.GenerateDsn(e2);
			}
			finally
			{
				if (flag)
				{
					this.ReportingQueue.Enqueue(e.Item);
				}
				else
				{
					this.CleaningQueue.Enqueue(e.Item);
				}
			}
		}

		private void ReporterEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			TextMessageDeliverer textMessageDeliverer = new TextMessageDeliverer(e.Item);
			try
			{
				textMessageDeliverer.Stage4Report();
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.XsoTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "ReporterEventHandler raises Exception: {0}", arg);
			}
			finally
			{
				this.CleaningQueue.Enqueue(e.Item);
			}
		}

		private void CleanerEventHandler(QueueDataAvailableEventSource<TextMessageDeliveryContext> src, QueueDataAvailableEventArgs<TextMessageDeliveryContext> e)
		{
			if (e.Item.CleanerEventHandler == null)
			{
				return;
			}
			try
			{
				e.Item.CleanerEventHandler(src, e);
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.XsoTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "CleanerEventHandler raises Exception: {0}", arg);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TextMessageDeliveringPipeline>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.TranslatingQueue != null)
				{
					this.TranslatingQueue.Dispose();
					this.TranslatingQueue = null;
				}
				if (this.ComposingQueue != null)
				{
					this.ComposingQueue.Dispose();
					this.ComposingQueue = null;
				}
				if (this.DeliveringQueue != null)
				{
					this.DeliveringQueue.Dispose();
					this.DeliveringQueue = null;
				}
				if (this.ReportingQueue != null)
				{
					this.ReportingQueue.Dispose();
					this.ReportingQueue = null;
				}
				if (this.CleaningQueue != null)
				{
					this.CleaningQueue.Dispose();
					this.CleaningQueue = null;
				}
			}
		}
	}
}
