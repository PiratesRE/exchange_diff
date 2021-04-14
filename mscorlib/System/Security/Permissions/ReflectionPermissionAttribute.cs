﻿using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class ReflectionPermissionAttribute : CodeAccessSecurityAttribute
	{
		public ReflectionPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		public ReflectionPermissionFlag Flags
		{
			get
			{
				return this.m_flag;
			}
			set
			{
				this.m_flag = value;
			}
		}

		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		public bool TypeInformation
		{
			get
			{
				return (this.m_flag & ReflectionPermissionFlag.TypeInformation) > ReflectionPermissionFlag.NoFlags;
			}
			set
			{
				this.m_flag = (value ? (this.m_flag | ReflectionPermissionFlag.TypeInformation) : (this.m_flag & ~ReflectionPermissionFlag.TypeInformation));
			}
		}

		public bool MemberAccess
		{
			get
			{
				return (this.m_flag & ReflectionPermissionFlag.MemberAccess) > ReflectionPermissionFlag.NoFlags;
			}
			set
			{
				this.m_flag = (value ? (this.m_flag | ReflectionPermissionFlag.MemberAccess) : (this.m_flag & ~ReflectionPermissionFlag.MemberAccess));
			}
		}

		[Obsolete("This permission is no longer used by the CLR.")]
		public bool ReflectionEmit
		{
			get
			{
				return (this.m_flag & ReflectionPermissionFlag.ReflectionEmit) > ReflectionPermissionFlag.NoFlags;
			}
			set
			{
				this.m_flag = (value ? (this.m_flag | ReflectionPermissionFlag.ReflectionEmit) : (this.m_flag & ~ReflectionPermissionFlag.ReflectionEmit));
			}
		}

		public bool RestrictedMemberAccess
		{
			get
			{
				return (this.m_flag & ReflectionPermissionFlag.RestrictedMemberAccess) > ReflectionPermissionFlag.NoFlags;
			}
			set
			{
				this.m_flag = (value ? (this.m_flag | ReflectionPermissionFlag.RestrictedMemberAccess) : (this.m_flag & ~ReflectionPermissionFlag.RestrictedMemberAccess));
			}
		}

		public override IPermission CreatePermission()
		{
			if (this.m_unrestricted)
			{
				return new ReflectionPermission(PermissionState.Unrestricted);
			}
			return new ReflectionPermission(this.m_flag);
		}

		private ReflectionPermissionFlag m_flag;
	}
}
