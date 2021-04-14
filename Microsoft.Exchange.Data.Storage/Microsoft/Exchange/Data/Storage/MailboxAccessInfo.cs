using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi.Security;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAccessInfo
	{
		public MailboxAccessInfo(ClientSecurityContext authenticatedUserContext)
		{
			Util.ThrowOnNullArgument(authenticatedUserContext, "authenticatedUserContext");
			this.Initialize(authenticatedUserContext, null, null, null, null, null);
		}

		public MailboxAccessInfo(AuthzContextHandle authenticatedUserHandle)
		{
			Util.ThrowOnNullArgument(authenticatedUserHandle, "authenticatedUserHandle");
			this.Initialize(null, authenticatedUserHandle, null, null, null, null);
		}

		public MailboxAccessInfo(string accessingUserDn, ClientSecurityContext authenticatedUserContext)
		{
			Util.ThrowOnNullOrEmptyArgument(accessingUserDn, "accessingUserDn");
			Util.ThrowOnNullArgument(authenticatedUserContext, "authenticatedUserContext");
			this.Initialize(authenticatedUserContext, null, null, accessingUserDn, null, null);
		}

		public MailboxAccessInfo(string accessingUserDn, AuthzContextHandle authenticatedUserHandle)
		{
			Util.ThrowOnNullOrEmptyArgument(accessingUserDn, "accessingUserDn");
			Util.ThrowOnNullArgument(authenticatedUserHandle, "authenticatedUserHandle");
			this.Initialize(null, authenticatedUserHandle, null, accessingUserDn, null, null);
		}

		public MailboxAccessInfo(string accessingUserDn, ClientIdentityInfo clientIdentityInfo)
		{
			Util.ThrowOnNullOrEmptyArgument(accessingUserDn, "accessingUserDn");
			Util.ThrowOnNullArgument(clientIdentityInfo, "clientIdentityInfo");
			this.Initialize(null, null, clientIdentityInfo, accessingUserDn, null, null);
		}

		private void Initialize(ClientSecurityContext context, AuthzContextHandle handle, ClientIdentityInfo clientIdentityInfo, string userDn, IADOrgPerson adEntry, GenericIdentity auxiliaryIdentity)
		{
			this.context = context;
			this.authzContextHandle = handle;
			this.clientIdentityInfo = clientIdentityInfo;
			this.userDn = userDn;
			this.adEntry = adEntry;
			this.auxiliaryIdentity = auxiliaryIdentity;
		}

		public ClientSecurityContext AuthenticatedUserContext
		{
			get
			{
				return this.context;
			}
		}

		public AuthzContextHandle AuthenticatedUserHandle
		{
			get
			{
				return this.authzContextHandle;
			}
		}

		public ClientIdentityInfo ClientIdentityInfo
		{
			get
			{
				return this.clientIdentityInfo;
			}
		}

		public string AccessingUserDn
		{
			get
			{
				return this.userDn;
			}
		}

		public IADOrgPerson AccessingUserAdEntry
		{
			get
			{
				return this.adEntry;
			}
		}

		public GenericIdentity AuxiliaryIdentity
		{
			get
			{
				return this.auxiliaryIdentity;
			}
		}

		public MailboxAccessInfo(WindowsPrincipal authenticatedUserPrincipal)
		{
			Util.ThrowOnNullArgument(authenticatedUserPrincipal, "authenticatedUserPrincipal");
			this.Initialize(authenticatedUserPrincipal, null, null, null, null, null, null);
		}

		public MailboxAccessInfo(WindowsPrincipal authenticatedUserPrincipal, GenericIdentity auxiliaryIdentity)
		{
			Util.ThrowOnNullArgument(authenticatedUserPrincipal, "authenticatedUserPrincipal");
			this.Initialize(authenticatedUserPrincipal, null, null, null, null, null, auxiliaryIdentity);
		}

		public MailboxAccessInfo(ClientSecurityContext authenticatedUserContext, GenericIdentity auxiliaryIdentity)
		{
			Util.ThrowOnNullArgument(authenticatedUserContext, "authenticatedUserContext");
			this.Initialize(null, authenticatedUserContext, null, null, null, null, auxiliaryIdentity);
		}

		public MailboxAccessInfo(string accessingUserDn, WindowsPrincipal authenticatedUserPrincipal)
		{
			Util.ThrowOnNullOrEmptyArgument(accessingUserDn, "accessingUserDn");
			Util.ThrowOnNullArgument(authenticatedUserPrincipal, "authenticatedUserPrincipal");
			this.Initialize(authenticatedUserPrincipal, null, null, null, accessingUserDn, null, null);
		}

		public MailboxAccessInfo(string accessingUserDn, ClientSecurityContext authenticatedUserContext, GenericIdentity auxiliaryIdentity)
		{
			Util.ThrowOnNullOrEmptyArgument(accessingUserDn, "accessingUserDn");
			Util.ThrowOnNullArgument(authenticatedUserContext, "authenticatedUserContext");
			this.Initialize(null, authenticatedUserContext, null, null, accessingUserDn, null, auxiliaryIdentity);
		}

		public MailboxAccessInfo(IADOrgPerson accessingUserAdEntry, WindowsPrincipal authenticatedUserPrincipal)
		{
			Util.ThrowOnNullArgument(accessingUserAdEntry, "accessingUserAdEntry");
			Util.ThrowOnNullArgument(authenticatedUserPrincipal, "authenticatedUserPrincipal");
			this.Initialize(authenticatedUserPrincipal, null, null, null, null, accessingUserAdEntry, null);
		}

		public MailboxAccessInfo(IADOrgPerson accessingUserAdEntry, ClientSecurityContext authenticatedUserContext)
		{
			Util.ThrowOnNullArgument(accessingUserAdEntry, "accessingUserAdEntry");
			Util.ThrowOnNullArgument(authenticatedUserContext, "authenticatedUserContext");
			this.Initialize(null, authenticatedUserContext, null, null, null, accessingUserAdEntry, null);
		}

		public MailboxAccessInfo(IADOrgPerson accessingUserAdEntry, AuthzContextHandle authenticatedUserHandle)
		{
			Util.ThrowOnNullArgument(accessingUserAdEntry, "accessingUserAdEntry");
			Util.ThrowOnNullArgument(authenticatedUserHandle, "authenticatedUserHandle");
			this.Initialize(null, null, authenticatedUserHandle, null, null, accessingUserAdEntry, null);
		}

		public MailboxAccessInfo(IADOrgPerson accessingUserAdEntry, ClientIdentityInfo clientIdentityInfo)
		{
			Util.ThrowOnNullArgument(accessingUserAdEntry, "accessingUserAdEntry");
			Util.ThrowOnNullArgument(clientIdentityInfo, "clientIdentityInfo");
			this.Initialize(null, null, null, clientIdentityInfo, null, accessingUserAdEntry, null);
		}

		private void Initialize(WindowsPrincipal principal, ClientSecurityContext context, AuthzContextHandle handle, ClientIdentityInfo clientIdentityInfo, string userDn, IADOrgPerson adEntry, GenericIdentity auxiliaryIdentity)
		{
			this.principal = principal;
			this.Initialize(context, handle, clientIdentityInfo, userDn, adEntry, auxiliaryIdentity);
		}

		public WindowsPrincipal AuthenticatedUserPrincipal
		{
			get
			{
				return this.principal;
			}
		}

		private ClientSecurityContext context;

		private AuthzContextHandle authzContextHandle;

		private ClientIdentityInfo clientIdentityInfo;

		private string userDn;

		private IADOrgPerson adEntry;

		private GenericIdentity auxiliaryIdentity;

		private WindowsPrincipal principal;
	}
}
