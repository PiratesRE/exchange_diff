using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class NativeObjectSecurity : CommonObjectSecurity
	{
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType) : base(isContainer)
		{
			this._resourceType = resourceType;
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : this(isContainer, resourceType)
		{
			this._exceptionContext = exceptionContext;
			this._exceptionFromErrorCode = exceptionFromErrorCode;
		}

		[SecurityCritical]
		internal NativeObjectSecurity(ResourceType resourceType, CommonSecurityDescriptor securityDescriptor) : this(resourceType, securityDescriptor, null)
		{
		}

		[SecurityCritical]
		internal NativeObjectSecurity(ResourceType resourceType, CommonSecurityDescriptor securityDescriptor, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode) : base(securityDescriptor)
		{
			this._resourceType = resourceType;
			this._exceptionFromErrorCode = exceptionFromErrorCode;
		}

		[SecuritySafeCritical]
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : this(resourceType, NativeObjectSecurity.CreateInternal(resourceType, isContainer, name, null, includeSections, true, exceptionFromErrorCode, exceptionContext), exceptionFromErrorCode)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections) : this(isContainer, resourceType, name, includeSections, null, null)
		{
		}

		[SecuritySafeCritical]
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : this(resourceType, NativeObjectSecurity.CreateInternal(resourceType, isContainer, null, handle, includeSections, false, exceptionFromErrorCode, exceptionContext), exceptionFromErrorCode)
		{
		}

		[SecuritySafeCritical]
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections) : this(isContainer, resourceType, handle, includeSections, null, null)
		{
		}

		[SecurityCritical]
		private static CommonSecurityDescriptor CreateInternal(ResourceType resourceType, bool isContainer, string name, SafeHandle handle, AccessControlSections includeSections, bool createByName, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext)
		{
			if (createByName && name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!createByName && handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			RawSecurityDescriptor rawSecurityDescriptor;
			int securityInfo = Win32.GetSecurityInfo(resourceType, name, handle, includeSections, out rawSecurityDescriptor);
			if (securityInfo != 0)
			{
				Exception ex = null;
				if (exceptionFromErrorCode != null)
				{
					ex = exceptionFromErrorCode(securityInfo, name, handle, exceptionContext);
				}
				if (ex == null)
				{
					if (securityInfo == 5)
					{
						ex = new UnauthorizedAccessException();
					}
					else if (securityInfo == 1307)
					{
						ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_InvalidOwner"));
					}
					else if (securityInfo == 1308)
					{
						ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_InvalidGroup"));
					}
					else if (securityInfo == 87)
					{
						ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_UnexpectedError", new object[]
						{
							securityInfo
						}));
					}
					else if (securityInfo == 123)
					{
						ex = new ArgumentException(Environment.GetResourceString("Argument_InvalidName"), "name");
					}
					else if (securityInfo == 2)
					{
						ex = ((name == null) ? new FileNotFoundException() : new FileNotFoundException(name));
					}
					else if (securityInfo == 1350)
					{
						ex = new NotSupportedException(Environment.GetResourceString("AccessControl_NoAssociatedSecurity"));
					}
					else
					{
						ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_UnexpectedError", new object[]
						{
							securityInfo
						}));
					}
				}
				throw ex;
			}
			return new CommonSecurityDescriptor(isContainer, false, rawSecurityDescriptor, true);
		}

		[SecurityCritical]
		private void Persist(string name, SafeHandle handle, AccessControlSections includeSections, object exceptionContext)
		{
			base.WriteLock();
			try
			{
				SecurityInfos securityInfos = (SecurityInfos)0;
				SecurityIdentifier owner = null;
				SecurityIdentifier group = null;
				SystemAcl sacl = null;
				DiscretionaryAcl dacl = null;
				if ((includeSections & AccessControlSections.Owner) != AccessControlSections.None && this._securityDescriptor.Owner != null)
				{
					securityInfos |= SecurityInfos.Owner;
					owner = this._securityDescriptor.Owner;
				}
				if ((includeSections & AccessControlSections.Group) != AccessControlSections.None && this._securityDescriptor.Group != null)
				{
					securityInfos |= SecurityInfos.Group;
					group = this._securityDescriptor.Group;
				}
				if ((includeSections & AccessControlSections.Audit) != AccessControlSections.None)
				{
					securityInfos |= SecurityInfos.SystemAcl;
					if (this._securityDescriptor.IsSystemAclPresent && this._securityDescriptor.SystemAcl != null && this._securityDescriptor.SystemAcl.Count > 0)
					{
						sacl = this._securityDescriptor.SystemAcl;
					}
					else
					{
						sacl = null;
					}
					if ((this._securityDescriptor.ControlFlags & ControlFlags.SystemAclProtected) != ControlFlags.None)
					{
						securityInfos |= (SecurityInfos)this.ProtectedSystemAcl;
					}
					else
					{
						securityInfos |= (SecurityInfos)this.UnprotectedSystemAcl;
					}
				}
				if ((includeSections & AccessControlSections.Access) != AccessControlSections.None && this._securityDescriptor.IsDiscretionaryAclPresent)
				{
					securityInfos |= SecurityInfos.DiscretionaryAcl;
					if (this._securityDescriptor.DiscretionaryAcl.EveryOneFullAccessForNullDacl)
					{
						dacl = null;
					}
					else
					{
						dacl = this._securityDescriptor.DiscretionaryAcl;
					}
					if ((this._securityDescriptor.ControlFlags & ControlFlags.DiscretionaryAclProtected) != ControlFlags.None)
					{
						securityInfos |= (SecurityInfos)this.ProtectedDiscretionaryAcl;
					}
					else
					{
						securityInfos |= (SecurityInfos)this.UnprotectedDiscretionaryAcl;
					}
				}
				if (securityInfos != (SecurityInfos)0)
				{
					int num = Win32.SetSecurityInfo(this._resourceType, name, handle, securityInfos, owner, group, sacl, dacl);
					if (num != 0)
					{
						Exception ex = null;
						if (this._exceptionFromErrorCode != null)
						{
							ex = this._exceptionFromErrorCode(num, name, handle, exceptionContext);
						}
						if (ex == null)
						{
							if (num == 5)
							{
								ex = new UnauthorizedAccessException();
							}
							else if (num == 1307)
							{
								ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_InvalidOwner"));
							}
							else if (num == 1308)
							{
								ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_InvalidGroup"));
							}
							else if (num == 123)
							{
								ex = new ArgumentException(Environment.GetResourceString("Argument_InvalidName"), "name");
							}
							else if (num == 6)
							{
								ex = new NotSupportedException(Environment.GetResourceString("AccessControl_InvalidHandle"));
							}
							else if (num == 2)
							{
								ex = new FileNotFoundException();
							}
							else if (num == 1350)
							{
								ex = new NotSupportedException(Environment.GetResourceString("AccessControl_NoAssociatedSecurity"));
							}
							else
							{
								ex = new InvalidOperationException(Environment.GetResourceString("AccessControl_UnexpectedError", new object[]
								{
									num
								}));
							}
						}
						throw ex;
					}
					base.OwnerModified = false;
					base.GroupModified = false;
					base.AccessRulesModified = false;
					base.AuditRulesModified = false;
				}
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		protected sealed override void Persist(string name, AccessControlSections includeSections)
		{
			this.Persist(name, includeSections, this._exceptionContext);
		}

		[SecuritySafeCritical]
		protected void Persist(string name, AccessControlSections includeSections, object exceptionContext)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.Persist(name, null, includeSections, exceptionContext);
		}

		[SecuritySafeCritical]
		protected sealed override void Persist(SafeHandle handle, AccessControlSections includeSections)
		{
			this.Persist(handle, includeSections, this._exceptionContext);
		}

		[SecuritySafeCritical]
		protected void Persist(SafeHandle handle, AccessControlSections includeSections, object exceptionContext)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			this.Persist(null, handle, includeSections, exceptionContext);
		}

		private readonly ResourceType _resourceType;

		private NativeObjectSecurity.ExceptionFromErrorCode _exceptionFromErrorCode;

		private object _exceptionContext;

		private readonly uint ProtectedDiscretionaryAcl = 2147483648U;

		private readonly uint ProtectedSystemAcl = 1073741824U;

		private readonly uint UnprotectedDiscretionaryAcl = 536870912U;

		private readonly uint UnprotectedSystemAcl = 268435456U;

		[SecuritySafeCritical]
		protected internal delegate Exception ExceptionFromErrorCode(int errorCode, string name, SafeHandle handle, object context);
	}
}
