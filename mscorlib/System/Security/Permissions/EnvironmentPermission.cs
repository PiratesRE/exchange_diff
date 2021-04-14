using System;
using System.Runtime.InteropServices;
using System.Security.Util;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class EnvironmentPermission : CodeAccessPermission, IUnrestrictedPermission, IBuiltInPermission
	{
		public EnvironmentPermission(PermissionState state)
		{
			if (state == PermissionState.Unrestricted)
			{
				this.m_unrestricted = true;
				return;
			}
			if (state == PermissionState.None)
			{
				this.m_unrestricted = false;
				return;
			}
			throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPermissionState"));
		}

		public EnvironmentPermission(EnvironmentPermissionAccess flag, string pathList)
		{
			this.SetPathList(flag, pathList);
		}

		public void SetPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			this.VerifyFlag(flag);
			this.m_unrestricted = false;
			if ((flag & EnvironmentPermissionAccess.Read) != EnvironmentPermissionAccess.NoAccess)
			{
				this.m_read = null;
			}
			if ((flag & EnvironmentPermissionAccess.Write) != EnvironmentPermissionAccess.NoAccess)
			{
				this.m_write = null;
			}
			this.AddPathList(flag, pathList);
		}

		[SecuritySafeCritical]
		public void AddPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			this.VerifyFlag(flag);
			if (this.FlagIsSet(flag, EnvironmentPermissionAccess.Read))
			{
				if (this.m_read == null)
				{
					this.m_read = new EnvironmentStringExpressionSet();
				}
				this.m_read.AddExpressions(pathList);
			}
			if (this.FlagIsSet(flag, EnvironmentPermissionAccess.Write))
			{
				if (this.m_write == null)
				{
					this.m_write = new EnvironmentStringExpressionSet();
				}
				this.m_write.AddExpressions(pathList);
			}
		}

		public string GetPathList(EnvironmentPermissionAccess flag)
		{
			this.VerifyFlag(flag);
			this.ExclusiveFlag(flag);
			if (this.FlagIsSet(flag, EnvironmentPermissionAccess.Read))
			{
				if (this.m_read == null)
				{
					return "";
				}
				return this.m_read.ToString();
			}
			else
			{
				if (!this.FlagIsSet(flag, EnvironmentPermissionAccess.Write))
				{
					return "";
				}
				if (this.m_write == null)
				{
					return "";
				}
				return this.m_write.ToString();
			}
		}

		private void VerifyFlag(EnvironmentPermissionAccess flag)
		{
			if ((flag & ~(EnvironmentPermissionAccess.Read | EnvironmentPermissionAccess.Write)) != EnvironmentPermissionAccess.NoAccess)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)flag
				}));
			}
		}

		private void ExclusiveFlag(EnvironmentPermissionAccess flag)
		{
			if (flag == EnvironmentPermissionAccess.NoAccess)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumNotSingleFlag"));
			}
			if ((flag & flag - 1) != EnvironmentPermissionAccess.NoAccess)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumNotSingleFlag"));
			}
		}

		private bool FlagIsSet(EnvironmentPermissionAccess flag, EnvironmentPermissionAccess question)
		{
			return (flag & question) > EnvironmentPermissionAccess.NoAccess;
		}

		private bool IsEmpty()
		{
			return !this.m_unrestricted && (this.m_read == null || this.m_read.IsEmpty()) && (this.m_write == null || this.m_write.IsEmpty());
		}

		public bool IsUnrestricted()
		{
			return this.m_unrestricted;
		}

		[SecuritySafeCritical]
		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return this.IsEmpty();
			}
			bool result;
			try
			{
				EnvironmentPermission environmentPermission = (EnvironmentPermission)target;
				if (environmentPermission.IsUnrestricted())
				{
					result = true;
				}
				else if (this.IsUnrestricted())
				{
					result = false;
				}
				else
				{
					result = ((this.m_read == null || this.m_read.IsSubsetOf(environmentPermission.m_read)) && (this.m_write == null || this.m_write.IsSubsetOf(environmentPermission.m_write)));
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
				{
					base.GetType().FullName
				}));
			}
			return result;
		}

		[SecuritySafeCritical]
		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			if (!base.VerifyType(target))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
				{
					base.GetType().FullName
				}));
			}
			if (this.IsUnrestricted())
			{
				return target.Copy();
			}
			EnvironmentPermission environmentPermission = (EnvironmentPermission)target;
			if (environmentPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			StringExpressionSet stringExpressionSet = (this.m_read == null) ? null : this.m_read.Intersect(environmentPermission.m_read);
			StringExpressionSet stringExpressionSet2 = (this.m_write == null) ? null : this.m_write.Intersect(environmentPermission.m_write);
			if ((stringExpressionSet == null || stringExpressionSet.IsEmpty()) && (stringExpressionSet2 == null || stringExpressionSet2.IsEmpty()))
			{
				return null;
			}
			return new EnvironmentPermission(PermissionState.None)
			{
				m_unrestricted = false,
				m_read = stringExpressionSet,
				m_write = stringExpressionSet2
			};
		}

		[SecuritySafeCritical]
		public override IPermission Union(IPermission other)
		{
			if (other == null)
			{
				return this.Copy();
			}
			if (!base.VerifyType(other))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
				{
					base.GetType().FullName
				}));
			}
			EnvironmentPermission environmentPermission = (EnvironmentPermission)other;
			if (this.IsUnrestricted() || environmentPermission.IsUnrestricted())
			{
				return new EnvironmentPermission(PermissionState.Unrestricted);
			}
			StringExpressionSet stringExpressionSet = (this.m_read == null) ? environmentPermission.m_read : this.m_read.Union(environmentPermission.m_read);
			StringExpressionSet stringExpressionSet2 = (this.m_write == null) ? environmentPermission.m_write : this.m_write.Union(environmentPermission.m_write);
			if ((stringExpressionSet == null || stringExpressionSet.IsEmpty()) && (stringExpressionSet2 == null || stringExpressionSet2.IsEmpty()))
			{
				return null;
			}
			return new EnvironmentPermission(PermissionState.None)
			{
				m_unrestricted = false,
				m_read = stringExpressionSet,
				m_write = stringExpressionSet2
			};
		}

		public override IPermission Copy()
		{
			EnvironmentPermission environmentPermission = new EnvironmentPermission(PermissionState.None);
			if (this.m_unrestricted)
			{
				environmentPermission.m_unrestricted = true;
			}
			else
			{
				environmentPermission.m_unrestricted = false;
				if (this.m_read != null)
				{
					environmentPermission.m_read = this.m_read.Copy();
				}
				if (this.m_write != null)
				{
					environmentPermission.m_write = this.m_write.Copy();
				}
			}
			return environmentPermission;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = CodeAccessPermission.CreatePermissionElement(this, "System.Security.Permissions.EnvironmentPermission");
			if (!this.IsUnrestricted())
			{
				if (this.m_read != null && !this.m_read.IsEmpty())
				{
					securityElement.AddAttribute("Read", SecurityElement.Escape(this.m_read.ToString()));
				}
				if (this.m_write != null && !this.m_write.IsEmpty())
				{
					securityElement.AddAttribute("Write", SecurityElement.Escape(this.m_write.ToString()));
				}
			}
			else
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			return securityElement;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.ValidateElement(esd, this);
			if (XMLUtil.IsUnrestricted(esd))
			{
				this.m_unrestricted = true;
				return;
			}
			this.m_unrestricted = false;
			this.m_read = null;
			this.m_write = null;
			string text = esd.Attribute("Read");
			if (text != null)
			{
				this.m_read = new EnvironmentStringExpressionSet(text);
			}
			text = esd.Attribute("Write");
			if (text != null)
			{
				this.m_write = new EnvironmentStringExpressionSet(text);
			}
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return EnvironmentPermission.GetTokenIndex();
		}

		internal static int GetTokenIndex()
		{
			return 0;
		}

		private StringExpressionSet m_read;

		private StringExpressionSet m_write;

		private bool m_unrestricted;
	}
}
