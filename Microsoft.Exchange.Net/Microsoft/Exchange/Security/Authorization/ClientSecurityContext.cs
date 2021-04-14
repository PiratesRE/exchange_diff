using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authorization
{
	public sealed class ClientSecurityContext : DisposeTrackableBase
	{
		public ClientSecurityContext(WindowsIdentity identity) : this(identity, true)
		{
		}

		public ClientSecurityContext(WindowsIdentity identity, bool retainIdentity)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (identity == null)
				{
					throw new ArgumentNullException("identity");
				}
				this.identity = identity;
				this.InitializeContextFromIdentity();
				disposeGuard.Success();
			}
			if (!retainIdentity)
			{
				this.identity = null;
			}
		}

		public ClientSecurityContext(IntPtr authenticatedUserHandle) : this(new AuthzContextHandle(authenticatedUserHandle))
		{
		}

		internal ClientSecurityContext(AuthzContextHandle authZContextHandle)
		{
			this.clientContextHandle = authZContextHandle;
		}

		public ClientSecurityContext(ISecurityAccessToken securityAccessToken) : this(securityAccessToken, AuthzFlags.Default)
		{
		}

		public static ClientSecurityContext DuplicateAuthZContextHandle(IntPtr clientContextHandle)
		{
			if (clientContextHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException(NetException.NoTokenContext);
			}
			AuthzContextHandle authZContextHandle = null;
			NativeMethods.AuthzLuid identifier = default(NativeMethods.AuthzLuid);
			identifier.LowPart = 0U;
			identifier.HighPart = 0;
			if (!NativeMethods.AuthzInitializeContextFromAuthzContext(AuthzFlags.Default, clientContextHandle, IntPtr.Zero, identifier, IntPtr.Zero, out authZContextHandle))
			{
				Exception innerException = new Win32Exception(Marshal.GetLastWin32Error());
				throw new AuthzException(NetException.AuthzInitializeContextFromDuplicateAuthZFailed, innerException);
			}
			return new ClientSecurityContext(authZContextHandle);
		}

		public ClientSecurityContext(ISecurityAccessToken securityAccessToken, AuthzFlags flags)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (securityAccessToken == null)
				{
					throw new ArgumentNullException("securityAccessToken");
				}
				this.securityAccessToken = new ClientSecurityContext.LazyInitSecurityAccessTokenEx(securityAccessToken);
				this.InitializeContextFromSecurityAccessToken(flags);
				disposeGuard.Success();
			}
		}

		public ClientSecurityContext(ISecurityAccessTokenEx securityAccessToken, AuthzFlags flags)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (securityAccessToken == null)
				{
					throw new ArgumentNullException("securityAccessToken");
				}
				this.securityAccessToken = securityAccessToken;
				this.InitializeContextFromSecurityAccessToken(flags);
				disposeGuard.Success();
			}
		}

		internal WindowsIdentity Identity
		{
			get
			{
				base.CheckDisposed();
				return this.identity;
			}
		}

		internal AuthzContextHandle ClientContextHandle
		{
			get
			{
				base.CheckDisposed();
				return this.clientContextHandle;
			}
		}

		public SecurityIdentifier UserSid
		{
			get
			{
				base.CheckDisposed();
				if (this.identity != null)
				{
					return this.identity.User;
				}
				if (this.securityAccessToken != null)
				{
					return this.securityAccessToken.UserSid;
				}
				if (this.clientContextHandle != null)
				{
					if (this.userSid == null)
					{
						this.userSid = this.GetUserSid();
					}
					return this.userSid;
				}
				throw new InvalidOperationException(NetException.NoContext);
			}
		}

		public SecurityIdentifier PrimaryGroupSid
		{
			get
			{
				SecurityIdentifier result = null;
				base.CheckDisposed();
				if (this.identity != null)
				{
					result = (this.identity.Groups.FirstOrDefault<IdentityReference>() as SecurityIdentifier);
				}
				else if (this.securityAccessToken != null)
				{
					SidBinaryAndAttributes[] groupSids = this.securityAccessToken.GroupSids;
					if (groupSids != null && groupSids.Length != 0)
					{
						result = groupSids[0].SecurityIdentifier;
					}
				}
				else
				{
					if (this.clientContextHandle == null)
					{
						throw new InvalidOperationException(NetException.NoContext);
					}
					NativeMethods.SecurityIdentifierAndAttributes[] array = NativeMethods.AuthzGetInformationFromContextTokenGroup(this.clientContextHandle);
					if (array != null && array.Length != 0)
					{
						result = array[0].sid;
					}
				}
				return result;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				base.CheckDisposed();
				if (this.identity != null)
				{
					return this.identity.IsAnonymous;
				}
				return !(this.UserSid != null) || this.UserSid.IsWellKnown(WellKnownSidType.AnonymousSid);
			}
		}

		public bool IsSystem
		{
			get
			{
				base.CheckDisposed();
				if (this.identity != null)
				{
					return this.identity.IsSystem;
				}
				return this.UserSid != null && (this.UserSid.IsWellKnown(WellKnownSidType.NTAuthoritySid) || this.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid));
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				base.CheckDisposed();
				if (this.identity != null)
				{
					return this.identity.IsAuthenticated;
				}
				return !this.IsAnonymous && !this.IsGuest;
			}
		}

		public bool IsGuest
		{
			get
			{
				base.CheckDisposed();
				if (this.isGuest == null)
				{
					if (this.identity != null)
					{
						this.isGuest = new bool?(this.identity.IsGuest);
					}
					else if (this.UserSid == null)
					{
						this.isGuest = new bool?(false);
					}
					else if (this.UserSid.IsWellKnown(WellKnownSidType.BuiltinGuestsSid) || this.UserSid.IsWellKnown(WellKnownSidType.AccountGuestSid))
					{
						this.isGuest = new bool?(true);
					}
					else
					{
						int binaryLength = this.UserSid.BinaryLength;
						if (binaryLength < 4)
						{
							this.isGuest = new bool?(false);
						}
						else
						{
							byte[] array = new byte[binaryLength];
							this.UserSid.GetBinaryForm(array, 0);
							int num = binaryLength - 4;
							int num2 = (int)array[num] << 24 | (int)array[num + 1] << 16 | (int)array[num + 2] << 8 | (int)array[num + 3];
							this.isGuest = new bool?(num2 == 501);
						}
					}
				}
				return this.isGuest.Value;
			}
		}

		public override void SuppressDisposeTracker()
		{
			if (this.clientContextHandle != null)
			{
				this.clientContextHandle.SuppressDisposeTracker();
			}
			base.SuppressDisposeTracker();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.clientContextHandle != null && !this.clientContextHandle.IsInvalid)
			{
				this.clientContextHandle.Dispose();
				this.clientContextHandle = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ClientSecurityContext>(this);
		}

		public override string ToString()
		{
			string text = string.Empty;
			try
			{
				SecurityIdentifier securityIdentifier = this.UserSid;
				text = ((securityIdentifier == null) ? "<Null Sid>" : securityIdentifier.ToString());
			}
			catch (InvalidSidException)
			{
				text = "<Invalid Sid>";
			}
			return string.Format(CultureInfo.InvariantCulture, "User SID: {0}", new object[]
			{
				text
			});
		}

		public void SetSecurityAccessToken(ISecurityAccessToken token)
		{
			base.CheckDisposed();
			SecurityAccessTokenEx securityAccessTokenEx = new SecurityAccessTokenEx();
			this.SetSecurityAccessToken(securityAccessTokenEx);
			token.UserSid = securityAccessTokenEx.UserSid.ToString();
			SidStringAndAttributes[] groupSids;
			if (securityAccessTokenEx.GroupSids == null)
			{
				groupSids = null;
			}
			else
			{
				groupSids = (from @group in securityAccessTokenEx.GroupSids
				select new SidStringAndAttributes(@group.SecurityIdentifier.ToString(), @group.Attributes)).ToArray<SidStringAndAttributes>();
			}
			token.GroupSids = groupSids;
			SidStringAndAttributes[] restrictedGroupSids;
			if (securityAccessTokenEx.RestrictedGroupSids == null)
			{
				restrictedGroupSids = null;
			}
			else
			{
				restrictedGroupSids = (from @group in securityAccessTokenEx.RestrictedGroupSids
				select new SidStringAndAttributes(@group.SecurityIdentifier.ToString(), @group.Attributes)).ToArray<SidStringAndAttributes>();
			}
			token.RestrictedGroupSids = restrictedGroupSids;
		}

		public void SetSecurityAccessToken(SecurityAccessTokenEx token)
		{
			base.CheckDisposed();
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			if (this.clientContextHandle == null || this.clientContextHandle.IsInvalid)
			{
				throw new AuthzException(NetException.AuthzUnableToGetTokenFromNullOrInvalidHandle(this.ToString()));
			}
			SecurityIdentifier localMachineAuthoritySid = new SecurityIdentifier(WellKnownSidType.BuiltinDomainSid, null);
			this.SerializeUserSid(token);
			this.SerializeGroupsToken(AuthzContextInformation.GroupSids, token, localMachineAuthoritySid);
			this.SerializeGroupsToken(AuthzContextInformation.RestrictedSids, token, localMachineAuthoritySid);
		}

		public int GetGrantedAccess(RawSecurityDescriptor rawSecurityDescriptor, AccessMask requestedAccess)
		{
			base.CheckDisposed();
			return this.GetGrantedAccess(rawSecurityDescriptor, null, requestedAccess);
		}

		public int GetGrantedAccess(RawSecurityDescriptor rawSecurityDescriptor, SecurityIdentifier principalSelfSid, AccessMask requestedAccess)
		{
			base.CheckDisposed();
			return this.GetGrantedAccess(SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor), principalSelfSid, requestedAccess);
		}

		public int GetGrantedAccess(SecurityDescriptor securityDescriptor, AccessMask requestedAccess)
		{
			base.CheckDisposed();
			return this.GetGrantedAccess(securityDescriptor, null, requestedAccess);
		}

		public int GetGrantedAccess(SecurityDescriptor securityDescriptor, SecurityIdentifier principalSelfSid, AccessMask requestedAccess)
		{
			base.CheckDisposed();
			if (this.clientContextHandle == null || this.clientContextHandle.IsInvalid)
			{
				throw new AuthzException(NetException.AuthzUnableToDoAccessCheckFromNullOrInvalidHandle);
			}
			SafeHGlobalHandle safeHGlobalHandle = null;
			SafeHGlobalHandle safeHGlobalHandle2 = null;
			int result;
			try
			{
				safeHGlobalHandle = AccessRequest.AllocateNativeStruct(requestedAccess, null, principalSelfSid);
				safeHGlobalHandle2 = AccessReply.AllocateNativeStruct(1);
				if (!NativeMethods.AuthzAccessCheck(0U, this.clientContextHandle, safeHGlobalHandle, IntPtr.Zero, securityDescriptor.BinaryForm, IntPtr.Zero, 0U, safeHGlobalHandle2, IntPtr.Zero))
				{
					Exception innerException = new Win32Exception(Marshal.GetLastWin32Error());
					throw new AuthzException(NetException.AuthzUnableToPerformAccessCheck(this.ToString()), innerException);
				}
				AccessReply accessReply = AccessReply.Create(safeHGlobalHandle2);
				if (accessReply.Errors[0] == 0)
				{
					result = accessReply.GrantedAccessMasks[0];
				}
				else
				{
					result = 0;
				}
			}
			finally
			{
				if (safeHGlobalHandle != null)
				{
					safeHGlobalHandle.Dispose();
				}
				if (safeHGlobalHandle2 != null)
				{
					safeHGlobalHandle2.Dispose();
				}
			}
			return result;
		}

		public IdentityReferenceCollection GetGroups()
		{
			base.CheckDisposed();
			if (this.identity != null)
			{
				return this.identity.Groups;
			}
			if (this.securityAccessToken != null)
			{
				SidBinaryAndAttributes[] groupSids = this.securityAccessToken.GroupSids;
				if (groupSids != null && groupSids.Length != 0)
				{
					IdentityReferenceCollection identityReferenceCollection = new IdentityReferenceCollection(groupSids.Length);
					foreach (SidBinaryAndAttributes sidBinaryAndAttributes in groupSids)
					{
						identityReferenceCollection.Add(sidBinaryAndAttributes.SecurityIdentifier);
					}
					return identityReferenceCollection;
				}
				return new IdentityReferenceCollection();
			}
			else
			{
				if (this.clientContextHandle == null)
				{
					throw new InvalidOperationException(NetException.NoContext);
				}
				NativeMethods.SecurityIdentifierAndAttributes[] array2 = NativeMethods.AuthzGetInformationFromContextTokenGroup(this.clientContextHandle);
				if (array2 != null && array2.Length != 0)
				{
					IdentityReferenceCollection identityReferenceCollection2 = new IdentityReferenceCollection(array2.Length);
					foreach (NativeMethods.SecurityIdentifierAndAttributes securityIdentifierAndAttributes in array2)
					{
						identityReferenceCollection2.Add(securityIdentifierAndAttributes.sid);
					}
					return identityReferenceCollection2;
				}
				return new IdentityReferenceCollection();
			}
		}

		public bool HasExtendedRightOnObject(SecurityDescriptor securityDescriptor, Guid extendedRightGuid)
		{
			base.CheckDisposed();
			return this.HasExtendedRightOnObject(securityDescriptor, extendedRightGuid, AccessMask.ControlAccess, null);
		}

		public bool HasExtendedRightOnObject(SecurityDescriptor securityDescriptor, Guid extendedRightGuid, AccessMask accessMask, SecurityIdentifier principalSelfSid)
		{
			base.CheckDisposed();
			bool[] array = AuthzAuthorization.CheckExtendedRights(this.clientContextHandle, securityDescriptor, new Guid[]
			{
				extendedRightGuid
			}, principalSelfSid, accessMask);
			return array[0];
		}

		public bool AddGroupSids(SidBinaryAndAttributes[] groupSids)
		{
			AuthzContextHandle authzContextHandle = this.clientContextHandle;
			AuthzContextHandle authzContextHandle2 = null;
			NativeMethods.GroupsToken groupsTokenFromGroups = ClientSecurityContext.GetGroupsTokenFromGroups(groupSids);
			bool result;
			try
			{
				bool flag = NativeMethods.AuthzAddSidsToContext(this.clientContextHandle, groupsTokenFromGroups.Groups, groupsTokenFromGroups.GroupCount, null, 0U, out authzContextHandle2);
				if (flag)
				{
					this.clientContextHandle = authzContextHandle2;
					if (authzContextHandle != null && !authzContextHandle.IsClosed)
					{
						authzContextHandle.Dispose();
					}
				}
				result = flag;
			}
			finally
			{
				ClientSecurityContext.ReleaseUnmanagedGroupSidBlocks(ref groupsTokenFromGroups);
			}
			return result;
		}

		private static NativeMethods.GroupsToken GetGroupsTokenFromGroups(SidBinaryAndAttributes[] groups)
		{
			NativeMethods.GroupsToken result = default(NativeMethods.GroupsToken);
			if (groups == null || groups.Length == 0)
			{
				result.GroupCount = 0U;
				result.Groups = null;
			}
			else
			{
				result.GroupCount = (uint)groups.Length;
				result.Groups = new NativeMethods.SidAndAttributes[groups.Length];
				bool flag = false;
				try
				{
					for (int i = 0; i < groups.Length; i++)
					{
						SecurityIdentifier securityIdentifier = groups[i].SecurityIdentifier;
						result.Groups[i] = default(NativeMethods.SidAndAttributes);
						result.Groups[i].Sid = Marshal.AllocHGlobal(securityIdentifier.BinaryLength);
						byte[] array = new byte[securityIdentifier.BinaryLength];
						securityIdentifier.GetBinaryForm(array, 0);
						Marshal.Copy(array, 0, result.Groups[i].Sid, securityIdentifier.BinaryLength);
						uint num = groups[i].Attributes;
						if ((num & 4U) != 0U)
						{
							num = 20U;
						}
						else
						{
							num = 16U;
						}
						result.Groups[i].Attributes = num;
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						ClientSecurityContext.ReleaseUnmanagedGroupSidBlocks(ref result);
					}
				}
			}
			return result;
		}

		private static void ReleaseUnmanagedGroupSidBlocks(ref NativeMethods.GroupsToken groupsToken)
		{
			if (groupsToken.Groups == null)
			{
				return;
			}
			foreach (NativeMethods.SidAndAttributes sidAndAttributes in groupsToken.Groups)
			{
				if (sidAndAttributes.Sid != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(sidAndAttributes.Sid);
				}
			}
			groupsToken.Groups = null;
			groupsToken.GroupCount = 0U;
		}

		private void SerializeUserSid(SecurityAccessTokenEx token)
		{
			token.UserSid = this.GetUserSid();
		}

		private void SerializeGroupsToken(AuthzContextInformation contextInfo, SecurityAccessTokenEx token, SecurityIdentifier localMachineAuthoritySid)
		{
			using (SafeHGlobalHandle informationFromContext = this.GetInformationFromContext(contextInfo))
			{
				int num = 0;
				int num2 = Marshal.ReadInt32(informationFromContext.DangerousGetHandle(), num);
				num += Marshal.SizeOf(typeof(IntPtr));
				List<SidBinaryAndAttributes> list = null;
				if (num2 > 0)
				{
					list = new List<SidBinaryAndAttributes>(num2);
					for (int i = 0; i < num2; i++)
					{
						SidBinaryAndAttributes sidBinaryAndAttributes = SidBinaryAndAttributes.Read(informationFromContext.DangerousGetHandle(), localMachineAuthoritySid, ref num);
						if (sidBinaryAndAttributes != null)
						{
							list.Add(sidBinaryAndAttributes);
						}
					}
				}
				SidBinaryAndAttributes[] array = (list == null || list.Count == 0) ? null : list.ToArray();
				if (contextInfo == AuthzContextInformation.GroupSids)
				{
					token.GroupSids = array;
				}
				else
				{
					token.RestrictedGroupSids = array;
				}
			}
		}

		private SafeHGlobalHandle GetInformationFromContext(AuthzContextInformation contextInfo)
		{
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			uint num = 0U;
			if (NativeMethods.AuthzGetInformationFromContext(this.clientContextHandle, contextInfo, num, ref num, safeHGlobalHandle))
			{
				throw new AuthzException(NetException.AuthzGetInformationFromContextReturnedSuccessForSize);
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				throw new Win32Exception(lastWin32Error);
			}
			safeHGlobalHandle = NativeMethods.AllocHGlobal((int)num);
			if (!NativeMethods.AuthzGetInformationFromContext(this.clientContextHandle, contextInfo, num, ref num, safeHGlobalHandle))
			{
				Exception innerException = new Win32Exception(Marshal.GetLastWin32Error());
				throw new AuthzException(NetException.AuthzGetInformationFromContextFailed(this.ToString()), innerException);
			}
			return safeHGlobalHandle;
		}

		private bool CheckForInvalidGroupSid(AuthzContextHandle tempHandle, SidBinaryAndAttributes group)
		{
			bool flag = true;
			AuthzContextHandle authzContextHandle = null;
			try
			{
				SidBinaryAndAttributes[] groups = new SidBinaryAndAttributes[]
				{
					group
				};
				NativeMethods.GroupsToken groupsToken = default(NativeMethods.GroupsToken);
				groupsToken = ClientSecurityContext.GetGroupsTokenFromGroups(groups);
				flag = NativeMethods.AuthzAddSidsToContext(tempHandle, groupsToken.Groups, groupsToken.GroupCount, null, 0U, out authzContextHandle);
				ClientSecurityContext.ReleaseUnmanagedGroupSidBlocks(ref groupsToken);
				if (!flag)
				{
					ExTraceGlobals.AuthorizationTracer.TraceError<SecurityIdentifier>(0L, "Group Sid {1} is broken.", group.SecurityIdentifier);
				}
			}
			finally
			{
				if (authzContextHandle != null && !authzContextHandle.IsInvalid)
				{
					authzContextHandle.Dispose();
					authzContextHandle = null;
				}
			}
			return flag;
		}

		private void InitializeContextFromIdentity()
		{
			if (this.identity == null)
			{
				throw new AuthzException(NetException.AuthzIdentityNotSet);
			}
			if (AuthzAuthorization.ResourceManagerHandle == null || AuthzAuthorization.ResourceManagerHandle.IsInvalid)
			{
				throw new ResourceManagerHandleInvalidException(NetException.AuthManagerNotInitialized);
			}
			this.clientContextHandle = null;
			NativeMethods.AuthzLuid identifier = default(NativeMethods.AuthzLuid);
			identifier.LowPart = 0U;
			identifier.HighPart = 0;
			if (!NativeMethods.AuthzInitializeContextFromToken(AuthzFlags.Default, this.identity.Token, AuthzAuthorization.ResourceManagerHandle, IntPtr.Zero, identifier, IntPtr.Zero, out this.clientContextHandle))
			{
				Exception innerException = new Win32Exception(Marshal.GetLastWin32Error());
				throw new AuthzException(NetException.AuthzInitializeContextFromTokenFailed(this.ToString()), innerException);
			}
		}

		private void InitializeContextFromSecurityAccessToken(AuthzFlags flags)
		{
			if (this.securityAccessToken == null)
			{
				throw new InvalidOperationException(NetException.NoTokenContext);
			}
			if (AuthzAuthorization.ResourceManagerHandle == null || AuthzAuthorization.ResourceManagerHandle.IsInvalid)
			{
				throw new ResourceManagerHandleInvalidException(NetException.AuthManagerNotInitialized);
			}
			if (flags == AuthzFlags.AuthzSkipTokenGroups && (this.securityAccessToken.GroupSids == null || this.securityAccessToken.GroupSids.Length == 0))
			{
				throw new MissingPrimaryGroupSidException(NetException.MissingPrimaryGroupSid);
			}
			AuthzContextHandle authzContextHandle = null;
			try
			{
				NativeMethods.AuthzLuid identifier = default(NativeMethods.AuthzLuid);
				SecurityIdentifier securityIdentifier = this.securityAccessToken.UserSid;
				int num = (securityIdentifier == null) ? 0 : securityIdentifier.BinaryLength;
				byte[] binaryForm = new byte[num];
				if (securityIdentifier != null)
				{
					securityIdentifier.GetBinaryForm(binaryForm, 0);
				}
				if (!NativeMethods.AuthzInitializeContextFromSid(flags, binaryForm, AuthzAuthorization.ResourceManagerHandle, IntPtr.Zero, identifier, IntPtr.Zero, out authzContextHandle))
				{
					Exception innerException = new Win32Exception(Marshal.GetLastWin32Error());
					throw new AuthzException(NetException.AuthzInitializeContextFromSidFailed(this.ToString()), innerException);
				}
				if (flags == AuthzFlags.AuthzSkipTokenGroups)
				{
					AuthzContextHandle authzContextHandle2 = null;
					NativeMethods.GroupsToken groupsToken = default(NativeMethods.GroupsToken);
					NativeMethods.GroupsToken groupsTokenFromGroups = ClientSecurityContext.GetGroupsTokenFromGroups(this.securityAccessToken.GroupSids);
					try
					{
						groupsToken = ClientSecurityContext.GetGroupsTokenFromGroups(this.securityAccessToken.RestrictedGroupSids);
						if (this.clientContextHandle != null && !this.clientContextHandle.IsInvalid)
						{
							this.clientContextHandle.Dispose();
							this.clientContextHandle = null;
						}
						if (!NativeMethods.AuthzAddSidsToContext(authzContextHandle, groupsTokenFromGroups.Groups, groupsTokenFromGroups.GroupCount, groupsToken.Groups, groupsToken.GroupCount, out authzContextHandle2))
						{
							Exception innerException2 = new Win32Exception(Marshal.GetLastWin32Error());
							if (ExTraceGlobals.AuthorizationTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.CertificateValidationTracer.TraceError(0L, "AuthzAddSidsToContext failed.");
								int num2 = 0;
								while ((long)num2 < (long)((ulong)groupsTokenFromGroups.GroupCount))
								{
									if (!this.CheckForInvalidGroupSid(authzContextHandle, this.securityAccessToken.GroupSids[num2]))
									{
										ExTraceGlobals.AuthorizationTracer.TraceError<int>(0L, "{0}: Group Sid in token is invalid.", num2);
										break;
									}
									num2++;
								}
								int num3 = 0;
								while ((long)num3 < (long)((ulong)groupsToken.GroupCount))
								{
									if (!this.CheckForInvalidGroupSid(authzContextHandle, this.securityAccessToken.RestrictedGroupSids[num3]))
									{
										ExTraceGlobals.AuthorizationTracer.TraceError<int>(0L, "{0}: Restricted Group Sid in token is invalid.", num3);
										break;
									}
									num3++;
								}
							}
							throw new AuthzException(NetException.AuthzAddSidsToContextFailed(this.ToString()), innerException2);
						}
					}
					finally
					{
						ClientSecurityContext.ReleaseUnmanagedGroupSidBlocks(ref groupsTokenFromGroups);
						ClientSecurityContext.ReleaseUnmanagedGroupSidBlocks(ref groupsToken);
					}
					this.clientContextHandle = authzContextHandle2;
				}
				else
				{
					this.clientContextHandle = authzContextHandle;
				}
			}
			finally
			{
				if (authzContextHandle != null && (flags == AuthzFlags.AuthzSkipTokenGroups || this.clientContextHandle == null))
				{
					authzContextHandle.Dispose();
					authzContextHandle = null;
				}
			}
		}

		public ClientSecurityContext Clone()
		{
			base.CheckDisposed();
			if (this.ClientContextHandle != null && !this.ClientContextHandle.IsInvalid)
			{
				return ClientSecurityContext.DuplicateAuthZContextHandle(this.clientContextHandle.DangerousGetHandle());
			}
			throw new AuthzException(NetException.AuthzUnableToDoAccessCheckFromNullOrInvalidHandle);
		}

		private SecurityIdentifier GetUserSid()
		{
			SecurityIdentifier result;
			using (SafeHGlobalHandle informationFromContext = this.GetInformationFromContext(AuthzContextInformation.UserSid))
			{
				IntPtr binaryForm = Marshal.ReadIntPtr(informationFromContext.DangerousGetHandle(), 0);
				SecurityIdentifier securityIdentifier = new SecurityIdentifier(binaryForm);
				result = securityIdentifier;
			}
			return result;
		}

		internal const string EveryoneIdentity = "S-1-1-0";

		private WindowsIdentity identity;

		private AuthzContextHandle clientContextHandle;

		private ISecurityAccessTokenEx securityAccessToken;

		private SecurityIdentifier userSid;

		private bool? isGuest;

		internal static readonly SidStringAndAttributes[] DisabledEveryoneOnlySidStringAndAttributesArray = new SidStringAndAttributes[]
		{
			new SidStringAndAttributes("S-1-1-0", 0U)
		};

		internal static readonly ClientSecurityContext FreeBusyPermissionDefaultClientSecurityContext = new ClientSecurityContext(new SecurityAccessToken
		{
			UserSid = "S-1-1-0",
			GroupSids = ClientSecurityContext.DisabledEveryoneOnlySidStringAndAttributesArray
		}, AuthzFlags.AuthzSkipTokenGroups);

		private class LazyInitSecurityAccessTokenEx : ISecurityAccessTokenEx
		{
			public LazyInitSecurityAccessTokenEx(ISecurityAccessToken securityAccessToken)
			{
				this.securityAccessToken = securityAccessToken;
			}

			SecurityIdentifier ISecurityAccessTokenEx.UserSid
			{
				get
				{
					if (this.userSid == null)
					{
						this.userSid = ClientSecurityContext.LazyInitSecurityAccessTokenEx.SidFromString(this.securityAccessToken.UserSid);
					}
					return this.userSid;
				}
			}

			SidBinaryAndAttributes[] ISecurityAccessTokenEx.GroupSids
			{
				get
				{
					if (this.groupSids == null)
					{
						this.groupSids = ClientSecurityContext.LazyInitSecurityAccessTokenEx.TranslateGroup(this.securityAccessToken.GroupSids);
					}
					return this.groupSids;
				}
			}

			SidBinaryAndAttributes[] ISecurityAccessTokenEx.RestrictedGroupSids
			{
				get
				{
					if (this.restrictedGroupSids == null)
					{
						this.restrictedGroupSids = ClientSecurityContext.LazyInitSecurityAccessTokenEx.TranslateGroup(this.securityAccessToken.RestrictedGroupSids);
					}
					return this.restrictedGroupSids;
				}
			}

			private static SidBinaryAndAttributes[] TranslateGroup(SidStringAndAttributes[] input)
			{
				if (input == null)
				{
					return null;
				}
				SidBinaryAndAttributes[] array = new SidBinaryAndAttributes[input.Length];
				for (int num = 0; num != input.Length; num++)
				{
					array[num] = new SidBinaryAndAttributes(ClientSecurityContext.LazyInitSecurityAccessTokenEx.SidFromString(input[num].SecurityIdentifier), input[num].Attributes);
				}
				return array;
			}

			private static SecurityIdentifier SidFromString(string sidString)
			{
				if (string.IsNullOrEmpty(sidString))
				{
					throw new InvalidSidException(sidString);
				}
				SecurityIdentifier result;
				try
				{
					result = new SecurityIdentifier(sidString);
				}
				catch (ArgumentException innerException)
				{
					throw new InvalidSidException(sidString, innerException);
				}
				return result;
			}

			private readonly ISecurityAccessToken securityAccessToken;

			private SecurityIdentifier userSid;

			private SidBinaryAndAttributes[] groupSids;

			private SidBinaryAndAttributes[] restrictedGroupSids;
		}
	}
}
