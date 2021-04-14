using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionsInProgress
	{
		public SubmissionsInProgress(int capacity)
		{
			this.map = new SynchronizedDictionary<Thread, MailItemSubmitter>(capacity);
		}

		public MailItemSubmitter this[Thread thread]
		{
			get
			{
				return this.map[thread];
			}
			set
			{
				this.map[thread] = value;
			}
		}

		public void Remove(Thread thread)
		{
			this.map.Remove(thread);
		}

		public bool DetectHang(TimeSpan limit, out Thread thread, out MailItemSubmitter mailItemSubmitter)
		{
			bool hang = false;
			Thread hangThread = null;
			MailItemSubmitter hangMailItemSubmitter = null;
			DateTime utcNow = DateTime.UtcNow;
			this.map.ForEach((MailItemSubmitter perEntryMailItemSubmitter) => !hang, delegate(Thread perEntryThread, MailItemSubmitter perEntryMailItemSubmitter)
			{
				if (default(DateTime) != perEntryMailItemSubmitter.StartTime && limit < utcNow - perEntryMailItemSubmitter.StartTime)
				{
					hang = true;
					hangThread = perEntryThread;
					hangMailItemSubmitter = perEntryMailItemSubmitter;
				}
			});
			thread = hangThread;
			mailItemSubmitter = hangMailItemSubmitter;
			return hang;
		}

		public XElement GetDiagnosticInfo()
		{
			XElement root = new XElement("CurrentSubmissions");
			this.map.ForEach(null, delegate(Thread thread, MailItemSubmitter mailItemSubmitter)
			{
				SubmissionInfo submissionInfo = mailItemSubmitter.SubmissionInfo;
				string mailboxHopLatency = submissionInfo.MailboxHopLatency;
				Guid mdbGuid = submissionInfo.MdbGuid;
				IPAddress networkAddress = submissionInfo.NetworkAddress;
				DateTime originalCreateTime = submissionInfo.OriginalCreateTime;
				string content = null;
				string content2 = null;
				long num = 0L;
				byte[] array = null;
				MapiSubmissionInfo mapiSubmissionInfo = (MapiSubmissionInfo)mailItemSubmitter.SubmissionInfo;
				num = mapiSubmissionInfo.EventCounter;
				array = mapiSubmissionInfo.EntryId;
				content = mailItemSubmitter.Result.MessageId;
				content2 = mailItemSubmitter.Result.Sender;
				thread.Suspend();
				StackTrace content3;
				try
				{
					content3 = new StackTrace(thread, true);
				}
				finally
				{
					thread.Resume();
				}
				XElement xelement = new XElement("Submission");
				xelement.Add(new object[]
				{
					new XElement("ThreadID", thread.ManagedThreadId),
					new XElement("ConnectionID", mailItemSubmitter.SubmissionConnectionId),
					new XElement("Duration", (default(DateTime) == mailItemSubmitter.StartTime) ? TimeSpan.Zero : (DateTime.UtcNow - mailItemSubmitter.StartTime)),
					new XElement("MailboxServer", submissionInfo.MailboxFqdn),
					new XElement("MailboxServerIP", submissionInfo.NetworkAddress),
					new XElement("MdbName", submissionInfo.DatabaseName),
					new XElement("MdbGuid", submissionInfo.MdbGuid),
					new XElement("OriginalCreationTime", submissionInfo.OriginalCreateTime),
					new XElement("MessageID", content),
					new XElement("Sender", content2),
					new XElement("EventCounter", num),
					new XElement("EntryID", (array == null) ? null : BitConverter.ToString(array)),
					new XElement("Stage", mailItemSubmitter.Stage),
					new XElement("ErrorCode", mailItemSubmitter.ErrorCode),
					new XElement("MessageSize", mailItemSubmitter.MessageSize),
					new XElement("RecipientCount", mailItemSubmitter.RecipientCount),
					new XElement("RpcLatency", mailItemSubmitter.RpcLatency),
					new XElement("StackTrace", content3),
					LatencyFormatter.GetDiagnosticInfo(mailItemSubmitter.LatencyTracker)
				});
				root.Add(xelement);
			});
			return root;
		}

		private SynchronizedDictionary<Thread, MailItemSubmitter> map;
	}
}
