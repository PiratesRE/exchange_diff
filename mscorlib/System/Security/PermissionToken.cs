using System;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Security
{
	[Serializable]
	internal sealed class PermissionToken : ISecurityEncodable
	{
		internal static bool IsMscorlibClassName(string className)
		{
			int num = className.IndexOf(',');
			if (num == -1)
			{
				return true;
			}
			num = className.LastIndexOf(']');
			if (num == -1)
			{
				num = 0;
			}
			for (int i = num; i < className.Length; i++)
			{
				if ((className[i] == 'm' || className[i] == 'M') && string.Compare(className, i, "mscorlib", 0, "mscorlib".Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		static PermissionToken()
		{
			PermissionToken.s_theTokenFactory = new PermissionTokenFactory(4);
		}

		internal PermissionToken()
		{
		}

		internal PermissionToken(int index, PermissionTokenType type, string strTypeName)
		{
			this.m_index = index;
			this.m_type = type;
			this.m_strTypeName = strTypeName;
		}

		[SecurityCritical]
		public static PermissionToken GetToken(Type cls)
		{
			if (cls == null)
			{
				return null;
			}
			if (cls.GetInterface("System.Security.Permissions.IBuiltInPermission") != null)
			{
				if (PermissionToken.s_reflectPerm == null)
				{
					PermissionToken.s_reflectPerm = new ReflectionPermission(PermissionState.Unrestricted);
				}
				PermissionToken.s_reflectPerm.Assert();
				MethodInfo method = cls.GetMethod("GetTokenIndex", BindingFlags.Static | BindingFlags.NonPublic);
				RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
				int index = (int)runtimeMethodInfo.UnsafeInvoke(null, BindingFlags.Default, null, null, null);
				return PermissionToken.s_theTokenFactory.BuiltInGetToken(index, null, cls);
			}
			return PermissionToken.s_theTokenFactory.GetToken(cls, null);
		}

		public static PermissionToken GetToken(IPermission perm)
		{
			if (perm == null)
			{
				return null;
			}
			IBuiltInPermission builtInPermission = perm as IBuiltInPermission;
			if (builtInPermission != null)
			{
				return PermissionToken.s_theTokenFactory.BuiltInGetToken(builtInPermission.GetTokenIndex(), perm, null);
			}
			return PermissionToken.s_theTokenFactory.GetToken(perm.GetType(), perm);
		}

		public static PermissionToken GetToken(string typeStr)
		{
			return PermissionToken.GetToken(typeStr, false);
		}

		public static PermissionToken GetToken(string typeStr, bool bCreateMscorlib)
		{
			if (typeStr == null)
			{
				return null;
			}
			if (!PermissionToken.IsMscorlibClassName(typeStr))
			{
				return PermissionToken.s_theTokenFactory.GetToken(typeStr);
			}
			if (!bCreateMscorlib)
			{
				return null;
			}
			return PermissionToken.FindToken(Type.GetType(typeStr));
		}

		[SecuritySafeCritical]
		public static PermissionToken FindToken(Type cls)
		{
			if (cls == null)
			{
				return null;
			}
			if (cls.GetInterface("System.Security.Permissions.IBuiltInPermission") != null)
			{
				if (PermissionToken.s_reflectPerm == null)
				{
					PermissionToken.s_reflectPerm = new ReflectionPermission(PermissionState.Unrestricted);
				}
				PermissionToken.s_reflectPerm.Assert();
				MethodInfo method = cls.GetMethod("GetTokenIndex", BindingFlags.Static | BindingFlags.NonPublic);
				RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
				int index = (int)runtimeMethodInfo.UnsafeInvoke(null, BindingFlags.Default, null, null, null);
				return PermissionToken.s_theTokenFactory.BuiltInGetToken(index, null, cls);
			}
			return PermissionToken.s_theTokenFactory.FindToken(cls);
		}

		public static PermissionToken FindTokenByIndex(int i)
		{
			return PermissionToken.s_theTokenFactory.FindTokenByIndex(i);
		}

		public static bool IsTokenProperlyAssigned(IPermission perm, PermissionToken token)
		{
			PermissionToken token2 = PermissionToken.GetToken(perm);
			return token2.m_index == token.m_index && token.m_type == token2.m_type && (!(perm.GetType().Module.Assembly == Assembly.GetExecutingAssembly()) || token2.m_index < 17);
		}

		public SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("PermissionToken");
			if ((this.m_type & PermissionTokenType.BuiltIn) != (PermissionTokenType)0)
			{
				securityElement.AddAttribute("Index", string.Concat(this.m_index));
			}
			else
			{
				securityElement.AddAttribute("Name", SecurityElement.Escape(this.m_strTypeName));
			}
			securityElement.AddAttribute("Type", this.m_type.ToString("F"));
			return securityElement;
		}

		public void FromXml(SecurityElement elRoot)
		{
			elRoot.Tag.Equals("PermissionToken");
			string text = elRoot.Attribute("Name");
			PermissionToken permissionToken;
			if (text != null)
			{
				permissionToken = PermissionToken.GetToken(text, true);
			}
			else
			{
				permissionToken = PermissionToken.FindTokenByIndex(int.Parse(elRoot.Attribute("Index"), CultureInfo.InvariantCulture));
			}
			this.m_index = permissionToken.m_index;
			this.m_type = (PermissionTokenType)Enum.Parse(typeof(PermissionTokenType), elRoot.Attribute("Type"));
			this.m_strTypeName = permissionToken.m_strTypeName;
		}

		private static readonly PermissionTokenFactory s_theTokenFactory;

		private static volatile ReflectionPermission s_reflectPerm = null;

		private const string c_mscorlibName = "mscorlib";

		internal int m_index;

		internal volatile PermissionTokenType m_type;

		internal string m_strTypeName;

		internal static TokenBasedSet s_tokenSet = new TokenBasedSet();
	}
}
