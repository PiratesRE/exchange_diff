using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserContext
	{
		public UserContext(string userName, string userPrincipalName, string userSecurityIdentifier, string userAuthIdentifier, string organization)
		{
			this.userName = userName;
			this.userPrincipalName = userPrincipalName;
			this.userSecurityIdentifier = userSecurityIdentifier;
			this.userAuthIdentifier = userAuthIdentifier;
			this.organization = organization;
			this.creationDateTime = ExDateTime.UtcNow;
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		public string UserSecurityIdentifier
		{
			get
			{
				return this.userSecurityIdentifier;
			}
		}

		public string UserAuthIdentifier
		{
			get
			{
				return this.userAuthIdentifier;
			}
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		public bool IsActive
		{
			get
			{
				bool result;
				lock (this.userContextLock)
				{
					TimeSpan t = TimeSpan.FromMilliseconds((double)(Environment.TickCount - this.lastActivity));
					result = (this.sessionContexts.Count > 0 || this.activityCount > 0 || t < Constants.UserContextIdleTimeout);
				}
				return result;
			}
		}

		public ExDateTime CreationDateTime
		{
			get
			{
				return this.creationDateTime;
			}
		}

		public SessionContextActivity CreateSessionContextActivity(string mailboxIdentifier, SessionContextIdentifier sessionContextIdentifier, TimeSpan idleTimeout)
		{
			SessionContext sessionContext = new SessionContext(this, mailboxIdentifier, sessionContextIdentifier, idleTimeout, null);
			SessionContextActivity result;
			lock (this.userContextLock)
			{
				if (this.sessionContexts.ContainsKey(sessionContext.Id))
				{
					throw new InvalidOperationException("Context identifier already exists");
				}
				this.sessionContexts[sessionContext.Id] = sessionContext;
				SessionContextActivity sessionContextActivity;
				if (!SessionContextActivity.TryCreate(sessionContext, out sessionContextActivity))
				{
					throw ProtocolException.FromResponseCode((LID)57600, "Unable to create session context activity object.", ResponseCode.ContextNotFound, null);
				}
				result = sessionContextActivity;
			}
			return result;
		}

		public bool TryGetSessionContextActivity(long id, TimeSpan idleTimeout, out SessionContextActivity sessionContextActivity, out Exception failureException)
		{
			sessionContextActivity = null;
			failureException = null;
			bool result;
			lock (this.userContextLock)
			{
				SessionContext sessionContext;
				if (!this.sessionContexts.TryGetValue(id, out sessionContext))
				{
					failureException = ProtocolException.FromResponseCode((LID)51744, "Unable to find session context based on cookie.", ResponseCode.ContextNotFound, null);
					result = false;
				}
				else if (!SessionContextActivity.TryCreate(sessionContext, out sessionContextActivity))
				{
					failureException = ProtocolException.FromResponseCode((LID)45600, "Unable to create session context activity object.", ResponseCode.ContextNotFound, null);
					result = false;
				}
				else
				{
					sessionContext.IdleTimeout = idleTimeout;
					result = true;
				}
			}
			return result;
		}

		public bool TryGetSessionContextInfo(out SessionContextInfo[] sessionContextInfoArray)
		{
			sessionContextInfoArray = null;
			bool result;
			lock (this.userContextLock)
			{
				if (this.sessionContexts.Count > 0)
				{
					sessionContextInfoArray = (from x in this.sessionContexts
					select x.Value.GetSessionContextInfo()).ToArray<SessionContextInfo>();
				}
				result = (sessionContextInfoArray != null);
			}
			return result;
		}

		public void AddReference()
		{
			lock (this.userContextLock)
			{
				this.lastActivity = Environment.TickCount;
				this.activityCount++;
			}
		}

		public void ReleaseReference()
		{
			lock (this.userContextLock)
			{
				this.lastActivity = Environment.TickCount;
				this.activityCount--;
			}
		}

		public void GatherExpiredSessionContexts(List<SessionContext> expiredSessionContextList, out ExDateTime nextExpiration)
		{
			nextExpiration = ExDateTime.MaxValue;
			lock (this.userContextLock)
			{
				if (this.sessionContexts.Count > 0)
				{
					List<long> list = new List<long>();
					foreach (KeyValuePair<long, SessionContext> keyValuePair in this.sessionContexts)
					{
						if (keyValuePair.Value != null)
						{
							if (keyValuePair.Value.IsRundown)
							{
								list.Add(keyValuePair.Key);
								expiredSessionContextList.Add(keyValuePair.Value);
							}
							else
							{
								ExDateTime expires = keyValuePair.Value.Expires;
								if (expires < nextExpiration)
								{
									nextExpiration = expires;
								}
							}
						}
						else
						{
							list.Add(keyValuePair.Key);
						}
					}
					foreach (long key in list)
					{
						this.sessionContexts.Remove(key);
					}
				}
			}
		}

		private readonly object userContextLock = new object();

		private readonly Dictionary<long, SessionContext> sessionContexts = new Dictionary<long, SessionContext>();

		private readonly string userName;

		private readonly string userPrincipalName;

		private readonly string userSecurityIdentifier;

		private readonly string userAuthIdentifier;

		private readonly string organization;

		private readonly ExDateTime creationDateTime;

		private int activityCount;

		private int lastActivity = Environment.TickCount;
	}
}
