using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RpcHttpConnectionRegistrationCacheEntry : DisposeTrackableBase
	{
		public RpcHttpConnectionRegistrationCacheEntry(Guid associationGroupId, ClientSecurityContext clientSecurityContext, string authIdentifier, string serverTarget, string sessionCookie, string clientIp)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			this.associationGroupId = associationGroupId;
			this.clientSecurityContext = clientSecurityContext;
			this.authIdentifier = authIdentifier;
			this.serverTarget = serverTarget;
			this.sessionCookie = sessionCookie;
			this.clientIp = clientIp;
			this.requestIds = new List<Guid>(4);
		}

		public Guid AssociationGroupId
		{
			get
			{
				base.CheckDisposed();
				return this.associationGroupId;
			}
		}

		public string AuthIdentifier
		{
			get
			{
				base.CheckDisposed();
				return this.authIdentifier;
			}
		}

		public string ClientIp
		{
			get
			{
				base.CheckDisposed();
				return this.clientIp;
			}
		}

		public IList<Guid> RequestIds
		{
			get
			{
				base.CheckDisposed();
				return this.requestIds;
			}
		}

		public string SessionCookie
		{
			get
			{
				base.CheckDisposed();
				return this.sessionCookie;
			}
		}

		public string ServerTarget
		{
			get
			{
				base.CheckDisposed();
				return this.serverTarget;
			}
		}

		public void AddRequest(Guid requestId)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("requestId", requestId);
			this.requestIds.Add(requestId);
		}

		public void AddRequest(Guid requestId, ClientSecurityContext requestClientSecurityContext)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("requestId", requestId);
			ArgumentValidator.ThrowIfNull("requestClientSecurityContext", requestClientSecurityContext);
			if (this.clientSecurityContext.UserSid != requestClientSecurityContext.UserSid)
			{
				string message = string.Format("AssociationGroupId '{0}' already exists with user sid '{1}'.  Cannot replace with user sid '{2}'.", this.associationGroupId, this.clientSecurityContext.UserSid, requestClientSecurityContext.UserSid);
				throw new ConnectionRegistrationException(message);
			}
			this.requestIds.Add(requestId);
			if (!object.ReferenceEquals(requestClientSecurityContext, this.clientSecurityContext))
			{
				ClientSecurityContext clientSecurityContext = this.clientSecurityContext;
				this.clientSecurityContext = requestClientSecurityContext;
				clientSecurityContext.Dispose();
			}
		}

		public void RemoveRequest(Guid requestId)
		{
			base.CheckDisposed();
			this.requestIds.Remove(requestId);
		}

		public ClientSecurityContext GetClientSecurityContext()
		{
			base.CheckDisposed();
			return this.clientSecurityContext.Clone();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RpcHttpConnectionRegistrationCacheEntry>(this);
		}

		private readonly Guid associationGroupId;

		private readonly string authIdentifier;

		private readonly string sessionCookie;

		private readonly string serverTarget;

		private readonly string clientIp;

		private ClientSecurityContext clientSecurityContext;

		private List<Guid> requestIds;
	}
}
