using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Policy
{
	internal sealed class AssemblyEvidenceFactory : IRuntimeEvidenceFactory
	{
		private AssemblyEvidenceFactory(RuntimeAssembly targetAssembly, PEFileEvidenceFactory peFileFactory)
		{
			this.m_targetAssembly = targetAssembly;
			this.m_peFileFactory = peFileFactory;
		}

		internal SafePEFileHandle PEFile
		{
			[SecurityCritical]
			get
			{
				return this.m_peFileFactory.PEFile;
			}
		}

		public IEvidenceFactory Target
		{
			get
			{
				return this.m_targetAssembly;
			}
		}

		public EvidenceBase GenerateEvidence(Type evidenceType)
		{
			EvidenceBase evidenceBase = this.m_peFileFactory.GenerateEvidence(evidenceType);
			if (evidenceBase != null)
			{
				return evidenceBase;
			}
			if (evidenceType == typeof(GacInstalled))
			{
				return this.GenerateGacEvidence();
			}
			if (evidenceType == typeof(Hash))
			{
				return this.GenerateHashEvidence();
			}
			if (evidenceType == typeof(PermissionRequestEvidence))
			{
				return this.GeneratePermissionRequestEvidence();
			}
			if (evidenceType == typeof(StrongName))
			{
				return this.GenerateStrongNameEvidence();
			}
			return null;
		}

		private GacInstalled GenerateGacEvidence()
		{
			if (!this.m_targetAssembly.GlobalAssemblyCache)
			{
				return null;
			}
			this.m_peFileFactory.FireEvidenceGeneratedEvent(EvidenceTypeGenerated.Gac);
			return new GacInstalled();
		}

		private Hash GenerateHashEvidence()
		{
			if (this.m_targetAssembly.IsDynamic)
			{
				return null;
			}
			this.m_peFileFactory.FireEvidenceGeneratedEvent(EvidenceTypeGenerated.Hash);
			return new Hash(this.m_targetAssembly);
		}

		[SecuritySafeCritical]
		private PermissionRequestEvidence GeneratePermissionRequestEvidence()
		{
			PermissionSet permissionSet = null;
			PermissionSet permissionSet2 = null;
			PermissionSet permissionSet3 = null;
			AssemblyEvidenceFactory.GetAssemblyPermissionRequests(this.m_targetAssembly.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet2), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet3));
			if (permissionSet != null || permissionSet2 != null || permissionSet3 != null)
			{
				return new PermissionRequestEvidence(permissionSet, permissionSet2, permissionSet3);
			}
			return null;
		}

		[SecuritySafeCritical]
		private StrongName GenerateStrongNameEvidence()
		{
			byte[] array = null;
			string name = null;
			ushort major = 0;
			ushort minor = 0;
			ushort build = 0;
			ushort revision = 0;
			AssemblyEvidenceFactory.GetStrongNameInformation(this.m_targetAssembly.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<byte[]>(ref array), JitHelpers.GetStringHandleOnStack(ref name), out major, out minor, out build, out revision);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return new StrongName(new StrongNamePublicKeyBlob(array), name, new Version((int)major, (int)minor, (int)build, (int)revision), this.m_targetAssembly);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetAssemblyPermissionRequests(RuntimeAssembly assembly, ObjectHandleOnStack retMinimumPermissions, ObjectHandleOnStack retOptionalPermissions, ObjectHandleOnStack retRefusedPermissions);

		public IEnumerable<EvidenceBase> GetFactorySuppliedEvidence()
		{
			return this.m_peFileFactory.GetFactorySuppliedEvidence();
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetStrongNameInformation(RuntimeAssembly assembly, ObjectHandleOnStack retPublicKeyBlob, StringHandleOnStack retSimpleName, out ushort majorVersion, out ushort minorVersion, out ushort build, out ushort revision);

		[SecurityCritical]
		private static Evidence UpgradeSecurityIdentity(Evidence peFileEvidence, RuntimeAssembly targetAssembly)
		{
			peFileEvidence.Target = new AssemblyEvidenceFactory(targetAssembly, peFileEvidence.Target as PEFileEvidenceFactory);
			HostSecurityManager hostSecurityManager = AppDomain.CurrentDomain.HostSecurityManager;
			if ((hostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
			{
				peFileEvidence = hostSecurityManager.ProvideAssemblyEvidence(targetAssembly, peFileEvidence);
				if (peFileEvidence == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Policy_NullHostEvidence", new object[]
					{
						hostSecurityManager.GetType().FullName,
						targetAssembly.FullName
					}));
				}
			}
			return peFileEvidence;
		}

		private PEFileEvidenceFactory m_peFileFactory;

		private RuntimeAssembly m_targetAssembly;
	}
}
