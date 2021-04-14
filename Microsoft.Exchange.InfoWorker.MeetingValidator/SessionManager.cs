using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SessionManager : IDisposable
	{
		private SessionManager(SessionManager.SessionData ownerSession)
		{
			this.PrimarySession = ownerSession;
			this.cache = new Dictionary<ADObjectId, SessionManager.SessionData>(100);
			this.cache.Add(ownerSession.Id, ownerSession);
			this.disposeQueue = new Queue<SessionManager.SessionData>(100);
		}

		public SessionManager(MailboxSession ownerSession) : this(new SessionManager.SessionData(ownerSession))
		{
		}

		public SessionManager(ExchangePrincipal principal, string clientInfoString) : this(SessionManager.OpenSession(principal, null, clientInfoString))
		{
			this.disposeQueue.Enqueue(this.PrimarySession);
		}

		private static SessionManager.SessionData OpenSession(ExchangePrincipal principal, ExTimeZone timeZone, string clientInfoString)
		{
			MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(principal, CultureInfo.InvariantCulture, clientInfoString);
			if (timeZone != null)
			{
				mailboxSession.ExTimeZone = timeZone;
			}
			return new SessionManager.SessionData(mailboxSession);
		}

		private SessionManager.SessionData NewSession(ExchangePrincipal principal)
		{
			if (this.disposeQueue.Count >= 100)
			{
				Globals.ValidatorTracer.TraceDebug<int>((long)this.GetHashCode(), "Session manager exceeded the cache limit of {0}", 100);
				SessionManager.SessionData sessionData = this.disposeQueue.Dequeue();
				this.cache.Remove(sessionData.Id);
				sessionData.Dispose();
			}
			SessionManager.SessionData sessionData2 = SessionManager.OpenSession(principal, this.PrimarySession.ExTimeZone, this.PrimarySession.ClientInfoString);
			this.disposeQueue.Enqueue(sessionData2);
			this.cache.Add(principal.ObjectId, sessionData2);
			return sessionData2;
		}

		public SessionManager.SessionData this[ExchangePrincipal principal]
		{
			get
			{
				SessionManager.SessionData result;
				if (this.cache.ContainsKey(principal.ObjectId))
				{
					result = this.cache[principal.ObjectId];
				}
				else
				{
					result = this.NewSession(principal);
				}
				return result;
			}
		}

		public SessionManager.SessionData PrimarySession { get; private set; }

		void IDisposable.Dispose()
		{
			while (this.disposeQueue.Count != 0)
			{
				this.disposeQueue.Dequeue().Dispose();
			}
		}

		private const int CacheLimit = 100;

		private Dictionary<ADObjectId, SessionManager.SessionData> cache;

		private Queue<SessionManager.SessionData> disposeQueue;

		[ClassAccessLevel(AccessLevel.Implementation)]
		internal class SessionData : IDisposable
		{
			public MailboxSession Session { get; private set; }

			public ADObjectId Id
			{
				get
				{
					return this.Session.MailboxOwner.ObjectId;
				}
			}

			public ExTimeZone ExTimeZone
			{
				get
				{
					return this.Session.ExTimeZone;
				}
			}

			public string ClientInfoString
			{
				get
				{
					return this.Session.ClientInfoString;
				}
			}

			public SessionData(MailboxSession session)
			{
				if (session == null)
				{
					throw new ArgumentNullException("session");
				}
				this.Session = session;
			}

			public void Dispose()
			{
				this.Session.Dispose();
			}
		}
	}
}
