using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpSessions
	{
		public void StartShuttingDown()
		{
			Interlocked.Increment(ref this.shuttingDown);
		}

		public void ShutdownAllSessionsAndBlockUntilComplete(bool tryHarder)
		{
			this.StartShuttingDown();
			this.ShutdownAllConnections();
			this.BlockUntilAllSessionsRemoved(tryHarder);
		}

		public List<ISmtpInSession> TakeSnapshot()
		{
			List<ISmtpInSession> result;
			lock (this.sessions)
			{
				List<ISmtpInSession> list = new List<ISmtpInSession>(this.sessions.Count);
				list.AddRange(from pair in this.sessions
				select pair.Value);
				result = list;
			}
			return result;
		}

		public bool TryAdd(long id, ISmtpInSession session)
		{
			if (this.IsShuttingDown)
			{
				return false;
			}
			bool result;
			lock (this.sessions)
			{
				result = TransportHelpers.AttemptAddToDictionary<long, ISmtpInSession>(this.sessions, id, session, null);
			}
			return result;
		}

		public void Remove(long id)
		{
			lock (this.sessions)
			{
				this.sessions.Remove(id);
			}
		}

		private bool IsShuttingDown
		{
			get
			{
				return 0 != Interlocked.CompareExchange(ref this.shuttingDown, 1, 1);
			}
		}

		private void ShutdownAllConnections()
		{
			List<ISmtpInSession> list = this.TakeSnapshot();
			foreach (ISmtpInSession smtpInSession in list)
			{
				smtpInSession.ShutdownConnection();
			}
		}

		private void BlockUntilAllSessionsRemoved(bool tryHarder)
		{
			int num = 0;
			while (this.sessions.Any<KeyValuePair<long, ISmtpInSession>>())
			{
				if (!tryHarder && num >= 15)
				{
					return;
				}
				if (num >= 60)
				{
					throw new InvalidOperationException(string.Format("There are still {0} outstanding sessions after sleeping for {1} seconds", this.sessions.Count, num));
				}
				num++;
				Thread.Sleep(1000);
			}
		}

		private readonly Dictionary<long, ISmtpInSession> sessions = new Dictionary<long, ISmtpInSession>();

		private int shuttingDown;
	}
}
