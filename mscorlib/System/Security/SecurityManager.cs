using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Util;
using System.Threading;

namespace System.Security
{
	[ComVisible(true)]
	public static class SecurityManager
	{
		internal static PolicyManager PolicyManager
		{
			get
			{
				return SecurityManager.polmgr;
			}
		}

		[SecuritySafeCritical]
		[Obsolete("IsGranted is obsolete and will be removed in a future release of the .NET Framework.  Please use the PermissionSet property of either AppDomain or Assembly instead.")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsGranted(IPermission perm)
		{
			if (perm == null)
			{
				return true;
			}
			PermissionSet permissionSet = null;
			PermissionSet permissionSet2 = null;
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityManager.GetGrantedPermissions(JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet2), JitHelpers.GetStackCrawlMarkHandle(ref stackCrawlMark));
			return permissionSet.Contains(perm) && (permissionSet2 == null || !permissionSet2.Contains(perm));
		}

		public static PermissionSet GetStandardSandbox(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			Zone hostEvidence = evidence.GetHostEvidence<Zone>();
			if (hostEvidence == null)
			{
				return new PermissionSet(PermissionState.None);
			}
			if (hostEvidence.SecurityZone == SecurityZone.MyComputer)
			{
				return new PermissionSet(PermissionState.Unrestricted);
			}
			if (hostEvidence.SecurityZone == SecurityZone.Intranet)
			{
				PermissionSet localIntranet = BuiltInPermissionSets.LocalIntranet;
				PolicyStatement policyStatement = new NetCodeGroup(new AllMembershipCondition()).Resolve(evidence);
				PolicyStatement policyStatement2 = new FileCodeGroup(new AllMembershipCondition(), FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery).Resolve(evidence);
				if (policyStatement != null)
				{
					localIntranet.InplaceUnion(policyStatement.PermissionSet);
				}
				if (policyStatement2 != null)
				{
					localIntranet.InplaceUnion(policyStatement2.PermissionSet);
				}
				return localIntranet;
			}
			if (hostEvidence.SecurityZone == SecurityZone.Internet || hostEvidence.SecurityZone == SecurityZone.Trusted)
			{
				PermissionSet internet = BuiltInPermissionSets.Internet;
				PolicyStatement policyStatement3 = new NetCodeGroup(new AllMembershipCondition()).Resolve(evidence);
				if (policyStatement3 != null)
				{
					internet.InplaceUnion(policyStatement3.PermissionSet);
				}
				return internet;
			}
			return new PermissionSet(PermissionState.None);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void GetZoneAndOrigin(out ArrayList zone, out ArrayList origin)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			CodeAccessSecurityEngine.GetZoneAndOrigin(ref stackCrawlMark, out zone, out origin);
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
		public static PolicyLevel LoadPolicyLevelFromFile(string path, PolicyLevelType type)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			if (!File.InternalExists(path))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PolicyFileDoesNotExist"));
			}
			string fullPath = Path.GetFullPath(path);
			FileIOPermission fileIOPermission = new FileIOPermission(PermissionState.None);
			fileIOPermission.AddPathList(FileIOPermissionAccess.Read, fullPath);
			fileIOPermission.AddPathList(FileIOPermissionAccess.Write, fullPath);
			fileIOPermission.Demand();
			PolicyLevel result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					result = SecurityManager.LoadPolicyLevelFromStringHelper(streamReader.ReadToEnd(), path, type);
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
		public static PolicyLevel LoadPolicyLevelFromString(string str, PolicyLevelType type)
		{
			return SecurityManager.LoadPolicyLevelFromStringHelper(str, null, type);
		}

		private static PolicyLevel LoadPolicyLevelFromStringHelper(string str, string path, PolicyLevelType type)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			PolicyLevel policyLevel = new PolicyLevel(type, path);
			Parser parser = new Parser(str);
			SecurityElement topElement = parser.GetTopElement();
			if (topElement == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_BadXml"), "configuration"));
			}
			SecurityElement securityElement = topElement.SearchForChildByTag("mscorlib");
			if (securityElement == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_BadXml"), "mscorlib"));
			}
			SecurityElement securityElement2 = securityElement.SearchForChildByTag("security");
			if (securityElement2 == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_BadXml"), "security"));
			}
			SecurityElement securityElement3 = securityElement2.SearchForChildByTag("policy");
			if (securityElement3 == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_BadXml"), "policy"));
			}
			SecurityElement securityElement4 = securityElement3.SearchForChildByTag("PolicyLevel");
			if (securityElement4 != null)
			{
				policyLevel.FromXml(securityElement4);
				return policyLevel;
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Policy_BadXml"), "PolicyLevel"));
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
		public static void SavePolicyLevel(PolicyLevel level)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			PolicyManager.EncodeLevel(level);
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static PermissionSet ResolvePolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			return SecurityManager.ResolvePolicy(evidence, reqdPset, optPset, denyPset, out denied, true);
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static PermissionSet ResolvePolicy(Evidence evidence)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			if (evidence == null)
			{
				evidence = new Evidence();
			}
			return SecurityManager.polmgr.Resolve(evidence);
		}

		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static PermissionSet ResolvePolicy(Evidence[] evidences)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			if (evidences == null || evidences.Length == 0)
			{
				evidences = new Evidence[1];
			}
			PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidences[0]);
			if (permissionSet == null)
			{
				return null;
			}
			for (int i = 1; i < evidences.Length; i++)
			{
				permissionSet = permissionSet.Intersect(SecurityManager.ResolvePolicy(evidences[i]));
				if (permissionSet == null || permissionSet.IsEmpty())
				{
					return permissionSet;
				}
			}
			return permissionSet;
		}

		[SecurityCritical]
		public static bool CurrentThreadRequiresSecurityContextCapture()
		{
			return !CodeAccessSecurityEngine.QuickCheckForAllDemands();
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static PermissionSet ResolveSystemPolicy(Evidence evidence)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			if (PolicyManager.IsGacAssembly(evidence))
			{
				return new PermissionSet(PermissionState.Unrestricted);
			}
			return SecurityManager.polmgr.CodeGroupResolve(evidence, true);
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static IEnumerator ResolvePolicyGroups(Evidence evidence)
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			return SecurityManager.polmgr.ResolveCodeGroups(evidence);
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public static IEnumerator PolicyHierarchy()
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			return SecurityManager.polmgr.PolicyHierarchy();
		}

		[SecuritySafeCritical]
		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
		public static void SavePolicy()
		{
			if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyExplicit"));
			}
			SecurityManager.polmgr.Save();
		}

		[SecurityCritical]
		private static PermissionSet ResolveCasPolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied, out int securitySpecialFlags, bool checkExecutionPermission)
		{
			CodeAccessPermission.Assert(true);
			PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidence, reqdPset, optPset, denyPset, out denied, checkExecutionPermission);
			securitySpecialFlags = SecurityManager.GetSpecialFlags(permissionSet, denied);
			return permissionSet;
		}

		[SecurityCritical]
		private static PermissionSet ResolvePolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied, bool checkExecutionPermission)
		{
			if (SecurityManager.executionSecurityPermission == null)
			{
				SecurityManager.executionSecurityPermission = new SecurityPermission(SecurityPermissionFlag.Execution);
			}
			Exception exception = null;
			PermissionSet permissionSet;
			if (reqdPset == null)
			{
				permissionSet = optPset;
			}
			else
			{
				permissionSet = ((optPset == null) ? null : reqdPset.Union(optPset));
			}
			if (permissionSet != null && !permissionSet.IsUnrestricted())
			{
				permissionSet.AddPermission(SecurityManager.executionSecurityPermission);
			}
			if (evidence == null)
			{
				evidence = new Evidence();
			}
			PermissionSet permissionSet2 = SecurityManager.polmgr.Resolve(evidence);
			if (permissionSet != null)
			{
				permissionSet2.InplaceIntersect(permissionSet);
			}
			if (checkExecutionPermission && (!permissionSet2.Contains(SecurityManager.executionSecurityPermission) || (denyPset != null && denyPset.Contains(SecurityManager.executionSecurityPermission))))
			{
				throw new PolicyException(Environment.GetResourceString("Policy_NoExecutionPermission"), -2146233320, exception);
			}
			if (reqdPset != null && !reqdPset.IsSubsetOf(permissionSet2))
			{
				throw new PolicyException(Environment.GetResourceString("Policy_NoRequiredPermission"), -2146233321, exception);
			}
			if (denyPset != null)
			{
				denied = denyPset.Copy();
				permissionSet2.MergeDeniedSet(denied);
				if (denied.IsEmpty())
				{
					denied = null;
				}
			}
			else
			{
				denied = null;
			}
			permissionSet2.IgnoreTypeLoadFailures = true;
			return permissionSet2;
		}

		[Obsolete("Because execution permission checks can no longer be turned off, the CheckExecutionRights property no longer has any effect.")]
		public static bool CheckExecutionRights
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[Obsolete("Because security can no longer be turned off, the SecurityEnabled property no longer has any effect.")]
		public static bool SecurityEnabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal static int GetSpecialFlags(PermissionSet grantSet, PermissionSet deniedSet)
		{
			if (grantSet != null && grantSet.IsUnrestricted() && (deniedSet == null || deniedSet.IsEmpty()))
			{
				return -1;
			}
			SecurityPermissionFlag securityPermissionFlag = SecurityPermissionFlag.NoFlags;
			ReflectionPermissionFlag reflectionPermissionFlag = ReflectionPermissionFlag.NoFlags;
			CodeAccessPermission[] array = new CodeAccessPermission[6];
			if (grantSet != null)
			{
				if (grantSet.IsUnrestricted())
				{
					securityPermissionFlag = SecurityPermissionFlag.AllFlags;
					reflectionPermissionFlag = (ReflectionPermissionFlag.TypeInformation | ReflectionPermissionFlag.MemberAccess | ReflectionPermissionFlag.ReflectionEmit | ReflectionPermissionFlag.RestrictedMemberAccess);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = SecurityManager.s_UnrestrictedSpecialPermissionMap[i];
					}
				}
				else
				{
					SecurityPermission securityPermission = grantSet.GetPermission(6) as SecurityPermission;
					if (securityPermission != null)
					{
						securityPermissionFlag = securityPermission.Flags;
					}
					ReflectionPermission reflectionPermission = grantSet.GetPermission(4) as ReflectionPermission;
					if (reflectionPermission != null)
					{
						reflectionPermissionFlag = reflectionPermission.Flags;
					}
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = (grantSet.GetPermission(SecurityManager.s_BuiltInPermissionIndexMap[j][0]) as CodeAccessPermission);
					}
				}
			}
			if (deniedSet != null)
			{
				if (deniedSet.IsUnrestricted())
				{
					securityPermissionFlag = SecurityPermissionFlag.NoFlags;
					reflectionPermissionFlag = ReflectionPermissionFlag.NoFlags;
					for (int k = 0; k < SecurityManager.s_BuiltInPermissionIndexMap.Length; k++)
					{
						array[k] = null;
					}
				}
				else
				{
					SecurityPermission securityPermission = deniedSet.GetPermission(6) as SecurityPermission;
					if (securityPermission != null)
					{
						securityPermissionFlag &= ~securityPermission.Flags;
					}
					ReflectionPermission reflectionPermission = deniedSet.GetPermission(4) as ReflectionPermission;
					if (reflectionPermission != null)
					{
						reflectionPermissionFlag &= ~reflectionPermission.Flags;
					}
					for (int l = 0; l < SecurityManager.s_BuiltInPermissionIndexMap.Length; l++)
					{
						CodeAccessPermission codeAccessPermission = deniedSet.GetPermission(SecurityManager.s_BuiltInPermissionIndexMap[l][0]) as CodeAccessPermission;
						if (codeAccessPermission != null && !codeAccessPermission.IsSubsetOf(null))
						{
							array[l] = null;
						}
					}
				}
			}
			int num = SecurityManager.MapToSpecialFlags(securityPermissionFlag, reflectionPermissionFlag);
			if (num != -1)
			{
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m] != null && ((IUnrestrictedPermission)array[m]).IsUnrestricted())
					{
						num |= 1 << SecurityManager.s_BuiltInPermissionIndexMap[m][1];
					}
				}
			}
			return num;
		}

		private static int MapToSpecialFlags(SecurityPermissionFlag securityPermissionFlags, ReflectionPermissionFlag reflectionPermissionFlags)
		{
			int num = 0;
			if ((securityPermissionFlags & SecurityPermissionFlag.UnmanagedCode) == SecurityPermissionFlag.UnmanagedCode)
			{
				num |= 1;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.SkipVerification) == SecurityPermissionFlag.SkipVerification)
			{
				num |= 2;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.Assertion) == SecurityPermissionFlag.Assertion)
			{
				num |= 8;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.SerializationFormatter) == SecurityPermissionFlag.SerializationFormatter)
			{
				num |= 32;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.BindingRedirects) == SecurityPermissionFlag.BindingRedirects)
			{
				num |= 256;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.ControlEvidence) == SecurityPermissionFlag.ControlEvidence)
			{
				num |= 65536;
			}
			if ((securityPermissionFlags & SecurityPermissionFlag.ControlPrincipal) == SecurityPermissionFlag.ControlPrincipal)
			{
				num |= 131072;
			}
			if ((reflectionPermissionFlags & ReflectionPermissionFlag.RestrictedMemberAccess) == ReflectionPermissionFlag.RestrictedMemberAccess)
			{
				num |= 64;
			}
			if ((reflectionPermissionFlags & ReflectionPermissionFlag.MemberAccess) == ReflectionPermissionFlag.MemberAccess)
			{
				num |= 16;
			}
			return num;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool IsSameType(string strLeft, string strRight);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool _SetThreadSecurity(bool bThreadSecurity);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void GetGrantedPermissions(ObjectHandleOnStack retGranted, ObjectHandleOnStack retDenied, StackCrawlMarkHandle stackMark);

		private static volatile SecurityPermission executionSecurityPermission = null;

		private static PolicyManager polmgr = new PolicyManager();

		private static int[][] s_BuiltInPermissionIndexMap = new int[][]
		{
			new int[]
			{
				0,
				10
			},
			new int[]
			{
				1,
				11
			},
			new int[]
			{
				2,
				12
			},
			new int[]
			{
				4,
				13
			},
			new int[]
			{
				6,
				14
			},
			new int[]
			{
				7,
				9
			}
		};

		private static CodeAccessPermission[] s_UnrestrictedSpecialPermissionMap = new CodeAccessPermission[]
		{
			new EnvironmentPermission(PermissionState.Unrestricted),
			new FileDialogPermission(PermissionState.Unrestricted),
			new FileIOPermission(PermissionState.Unrestricted),
			new ReflectionPermission(PermissionState.Unrestricted),
			new SecurityPermission(PermissionState.Unrestricted),
			new UIPermission(PermissionState.Unrestricted)
		};
	}
}
