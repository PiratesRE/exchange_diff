using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.AirSync
{
	internal class ClientSecurityContextWrapper : IDisposable
	{
		private ClientSecurityContextWrapper(ClientSecurityContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.clientSecurityContext = context;
			this.AddRef();
		}

		public static ClientSecurityContextWrapper FromWindowsIdentity(WindowsIdentity identity)
		{
			return new ClientSecurityContextWrapper(new ClientSecurityContext(identity));
		}

		public static ClientSecurityContextWrapper FromIdentity(IIdentity identity)
		{
			return new ClientSecurityContextWrapper(ClientSecurityContextWrapper.ClientSecurityContextFromIdentity(identity));
		}

		public static ClientSecurityContextWrapper FromClientSecurityContext(ClientSecurityContext context)
		{
			return new ClientSecurityContextWrapper(context);
		}

		public int AddRef()
		{
			int result;
			lock (this.lockObject)
			{
				this.ThrowIfDisposed();
				result = ++this.refCount;
			}
			return result;
		}

		public AirSyncUserSecurityContext SerializedSecurityContext
		{
			get
			{
				if (this.serializedSecurityContext == null)
				{
					lock (this.lockObject)
					{
						this.ThrowIfDisposed();
						if (this.serializedSecurityContext == null)
						{
							this.serializedSecurityContext = new AirSyncUserSecurityContext();
							this.clientSecurityContext.SetSecurityAccessToken(this.serializedSecurityContext);
						}
					}
				}
				return this.serializedSecurityContext;
			}
		}

		public SecurityIdentifier UserSid
		{
			get
			{
				return this.clientSecurityContext.UserSid;
			}
		}

		public byte[] UserSidBytes
		{
			get
			{
				if (this.userSidBytes == null)
				{
					lock (this.lockObject)
					{
						if (this.userSidBytes == null)
						{
							this.userSidBytes = new byte[this.UserSid.BinaryLength];
							this.UserSid.GetBinaryForm(this.userSidBytes, 0);
						}
					}
				}
				return this.userSidBytes;
			}
		}

		public void Dispose()
		{
			this.InternalDispose(true);
		}

		public ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		private static ClientSecurityContext ClientSecurityContextFromIdentity(IIdentity identity)
		{
			if (GlobalSettings.UseOAuthMasterSidForSecurityContext)
			{
				OAuthIdentity oauthIdentity = identity as OAuthIdentity;
				if (oauthIdentity != null && oauthIdentity.ActAsUser != null && oauthIdentity.ActAsUser.MasterAccountSid != null)
				{
					return new GenericSidIdentity(oauthIdentity.ActAsUser.MasterAccountSid.ToString(), oauthIdentity.AuthenticationType, oauthIdentity.ActAsUser.MasterAccountSid).CreateClientSecurityContext();
				}
			}
			return identity.CreateClientSecurityContext(true);
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ClientSecurityContextWrapper was already disposed.");
			}
		}

		private void InternalDispose(bool fromDispose)
		{
			if (fromDispose && !this.disposed)
			{
				lock (this.lockObject)
				{
					if (!this.disposed)
					{
						this.refCount--;
						if (this.refCount <= 0 && this.clientSecurityContext != null)
						{
							this.clientSecurityContext.Dispose();
							this.clientSecurityContext = null;
							this.disposed = true;
							GC.SuppressFinalize(this);
						}
					}
				}
			}
		}

		private ClientSecurityContext clientSecurityContext;

		private AirSyncUserSecurityContext serializedSecurityContext;

		private object lockObject = new object();

		private volatile int refCount;

		private bool disposed;

		private byte[] userSidBytes;
	}
}
