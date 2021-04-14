using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	public abstract class IsolatedStorage : MarshalByRefObject
	{
		internal static bool IsRoaming(IsolatedStorageScope scope)
		{
			return (scope & IsolatedStorageScope.Roaming) > IsolatedStorageScope.None;
		}

		internal bool IsRoaming()
		{
			return (this.m_Scope & IsolatedStorageScope.Roaming) > IsolatedStorageScope.None;
		}

		internal static bool IsDomain(IsolatedStorageScope scope)
		{
			return (scope & IsolatedStorageScope.Domain) > IsolatedStorageScope.None;
		}

		internal bool IsDomain()
		{
			return (this.m_Scope & IsolatedStorageScope.Domain) > IsolatedStorageScope.None;
		}

		internal static bool IsMachine(IsolatedStorageScope scope)
		{
			return (scope & IsolatedStorageScope.Machine) > IsolatedStorageScope.None;
		}

		internal bool IsAssembly()
		{
			return (this.m_Scope & IsolatedStorageScope.Assembly) > IsolatedStorageScope.None;
		}

		internal static bool IsApp(IsolatedStorageScope scope)
		{
			return (scope & IsolatedStorageScope.Application) > IsolatedStorageScope.None;
		}

		internal bool IsApp()
		{
			return (this.m_Scope & IsolatedStorageScope.Application) > IsolatedStorageScope.None;
		}

		private string GetNameFromID(string typeID, string instanceID)
		{
			return typeID + this.SeparatorInternal.ToString() + instanceID;
		}

		private static string GetPredefinedTypeName(object o)
		{
			if (o is Publisher)
			{
				return "Publisher";
			}
			if (o is StrongName)
			{
				return "StrongName";
			}
			if (o is Url)
			{
				return "Url";
			}
			if (o is Site)
			{
				return "Site";
			}
			if (o is Zone)
			{
				return "Zone";
			}
			return null;
		}

		internal static string GetHash(Stream s)
		{
			string result;
			using (SHA1 sha = new SHA1CryptoServiceProvider())
			{
				byte[] buff = sha.ComputeHash(s);
				result = Path.ToBase32StringSuitableForDirName(buff);
			}
			return result;
		}

		private static bool IsValidName(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsLetter(s[i]) && !char.IsDigit(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static SecurityPermission GetControlEvidencePermission()
		{
			if (IsolatedStorage.s_PermControlEvidence == null)
			{
				IsolatedStorage.s_PermControlEvidence = new SecurityPermission(SecurityPermissionFlag.ControlEvidence);
			}
			return IsolatedStorage.s_PermControlEvidence;
		}

		private static PermissionSet GetUnrestricted()
		{
			if (IsolatedStorage.s_PermUnrestricted == null)
			{
				IsolatedStorage.s_PermUnrestricted = new PermissionSet(PermissionState.Unrestricted);
			}
			return IsolatedStorage.s_PermUnrestricted;
		}

		protected virtual char SeparatorExternal
		{
			get
			{
				return '\\';
			}
		}

		protected virtual char SeparatorInternal
		{
			get
			{
				return '.';
			}
		}

		[CLSCompliant(false)]
		[Obsolete("IsolatedStorage.MaximumSize has been deprecated because it is not CLS Compliant.  To get the maximum size use IsolatedStorage.Quota")]
		public virtual ulong MaximumSize
		{
			get
			{
				if (this.m_ValidQuota)
				{
					return this.m_Quota;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_QuotaIsUndefined", new object[]
				{
					"MaximumSize"
				}));
			}
		}

		[CLSCompliant(false)]
		[Obsolete("IsolatedStorage.CurrentSize has been deprecated because it is not CLS Compliant.  To get the current size use IsolatedStorage.UsedSize")]
		public virtual ulong CurrentSize
		{
			get
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined", new object[]
				{
					"CurrentSize"
				}));
			}
		}

		[ComVisible(false)]
		public virtual long UsedSize
		{
			get
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined", new object[]
				{
					"UsedSize"
				}));
			}
		}

		[ComVisible(false)]
		public virtual long Quota
		{
			get
			{
				if (this.m_ValidQuota)
				{
					return (long)this.m_Quota;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_QuotaIsUndefined", new object[]
				{
					"Quota"
				}));
			}
			internal set
			{
				this.m_Quota = (ulong)value;
				this.m_ValidQuota = true;
			}
		}

		[ComVisible(false)]
		public virtual long AvailableFreeSpace
		{
			get
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_QuotaIsUndefined", new object[]
				{
					"AvailableFreeSpace"
				}));
			}
		}

		public object DomainIdentity
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
			get
			{
				if (this.IsDomain())
				{
					return this.m_DomainIdentity;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_DomainUndefined"));
			}
		}

		[ComVisible(false)]
		public object ApplicationIdentity
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
			get
			{
				if (this.IsApp())
				{
					return this.m_AppIdentity;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_ApplicationUndefined"));
			}
		}

		public object AssemblyIdentity
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
			get
			{
				if (this.IsAssembly())
				{
					return this.m_AssemIdentity;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_AssemblyUndefined"));
			}
		}

		[ComVisible(false)]
		public virtual bool IncreaseQuotaTo(long newQuotaSize)
		{
			return false;
		}

		[SecurityCritical]
		internal MemoryStream GetIdentityStream(IsolatedStorageScope scope)
		{
			IsolatedStorage.GetUnrestricted().Assert();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			object obj;
			if (IsolatedStorage.IsApp(scope))
			{
				obj = this.m_AppIdentity;
			}
			else if (IsolatedStorage.IsDomain(scope))
			{
				obj = this.m_DomainIdentity;
			}
			else
			{
				obj = this.m_AssemIdentity;
			}
			if (obj != null)
			{
				binaryFormatter.Serialize(memoryStream, obj);
			}
			memoryStream.Position = 0L;
			return memoryStream;
		}

		public IsolatedStorageScope Scope
		{
			get
			{
				return this.m_Scope;
			}
		}

		internal string DomainName
		{
			get
			{
				if (this.IsDomain())
				{
					return this.m_DomainName;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_DomainUndefined"));
			}
		}

		internal string AssemName
		{
			get
			{
				if (this.IsAssembly())
				{
					return this.m_AssemName;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_AssemblyUndefined"));
			}
		}

		internal string AppName
		{
			get
			{
				if (this.IsApp())
				{
					return this.m_AppName;
				}
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_ApplicationUndefined"));
			}
		}

		[SecuritySafeCritical]
		protected void InitStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
		{
			PermissionSet permissionSet = null;
			PermissionSet psDenied = null;
			RuntimeAssembly caller = IsolatedStorage.GetCaller();
			IsolatedStorage.GetControlEvidencePermission().Assert();
			if (IsolatedStorage.IsDomain(scope))
			{
				AppDomain domain = Thread.GetDomain();
				if (!IsolatedStorage.IsRoaming(scope))
				{
					permissionSet = domain.PermissionSet;
					if (permissionSet == null)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainGrantSet"));
					}
				}
				this._InitStore(scope, domain.Evidence, domainEvidenceType, caller.Evidence, assemblyEvidenceType, null, null);
			}
			else
			{
				if (!IsolatedStorage.IsRoaming(scope))
				{
					caller.GetGrantSet(out permissionSet, out psDenied);
					if (permissionSet == null)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyGrantSet"));
					}
				}
				this._InitStore(scope, null, null, caller.Evidence, assemblyEvidenceType, null, null);
			}
			this.SetQuota(permissionSet, psDenied);
		}

		[SecuritySafeCritical]
		protected void InitStore(IsolatedStorageScope scope, Type appEvidenceType)
		{
			PermissionSet permissionSet = null;
			PermissionSet psDenied = null;
			Assembly caller = IsolatedStorage.GetCaller();
			IsolatedStorage.GetControlEvidencePermission().Assert();
			if (IsolatedStorage.IsApp(scope))
			{
				AppDomain domain = Thread.GetDomain();
				if (!IsolatedStorage.IsRoaming(scope))
				{
					permissionSet = domain.PermissionSet;
					if (permissionSet == null)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainGrantSet"));
					}
				}
				ActivationContext activationContext = AppDomain.CurrentDomain.ActivationContext;
				if (activationContext == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
				}
				ApplicationSecurityInfo applicationSecurityInfo = new ApplicationSecurityInfo(activationContext);
				this._InitStore(scope, null, null, null, null, applicationSecurityInfo.ApplicationEvidence, appEvidenceType);
			}
			this.SetQuota(permissionSet, psDenied);
		}

		[SecuritySafeCritical]
		internal void InitStore(IsolatedStorageScope scope, object domain, object assem, object app)
		{
			PermissionSet permissionSet = null;
			PermissionSet psDenied = null;
			Evidence evidence = null;
			Evidence evidence2 = null;
			Evidence evidence3 = null;
			if (IsolatedStorage.IsApp(scope))
			{
				EvidenceBase evidenceBase = app as EvidenceBase;
				if (evidenceBase == null)
				{
					evidenceBase = new LegacyEvidenceWrapper(app);
				}
				evidence3 = new Evidence();
				evidence3.AddHostEvidence<EvidenceBase>(evidenceBase);
			}
			else
			{
				EvidenceBase evidenceBase2 = assem as EvidenceBase;
				if (evidenceBase2 == null)
				{
					evidenceBase2 = new LegacyEvidenceWrapper(assem);
				}
				evidence2 = new Evidence();
				evidence2.AddHostEvidence<EvidenceBase>(evidenceBase2);
				if (IsolatedStorage.IsDomain(scope))
				{
					EvidenceBase evidenceBase3 = domain as EvidenceBase;
					if (evidenceBase3 == null)
					{
						evidenceBase3 = new LegacyEvidenceWrapper(domain);
					}
					evidence = new Evidence();
					evidence.AddHostEvidence<EvidenceBase>(evidenceBase3);
				}
			}
			this._InitStore(scope, evidence, null, evidence2, null, evidence3, null);
			if (!IsolatedStorage.IsRoaming(scope))
			{
				RuntimeAssembly caller = IsolatedStorage.GetCaller();
				IsolatedStorage.GetControlEvidencePermission().Assert();
				caller.GetGrantSet(out permissionSet, out psDenied);
				if (permissionSet == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyGrantSet"));
				}
			}
			this.SetQuota(permissionSet, psDenied);
		}

		[SecurityCritical]
		internal void InitStore(IsolatedStorageScope scope, Evidence domainEv, Type domainEvidenceType, Evidence assemEv, Type assemEvidenceType, Evidence appEv, Type appEvidenceType)
		{
			PermissionSet psAllowed = null;
			if (!IsolatedStorage.IsRoaming(scope))
			{
				if (IsolatedStorage.IsApp(scope))
				{
					psAllowed = SecurityManager.GetStandardSandbox(appEv);
				}
				else if (IsolatedStorage.IsDomain(scope))
				{
					psAllowed = SecurityManager.GetStandardSandbox(domainEv);
				}
				else
				{
					psAllowed = SecurityManager.GetStandardSandbox(assemEv);
				}
			}
			this._InitStore(scope, domainEv, domainEvidenceType, assemEv, assemEvidenceType, appEv, appEvidenceType);
			this.SetQuota(psAllowed, null);
		}

		[SecuritySafeCritical]
		internal bool InitStore(IsolatedStorageScope scope, Stream domain, Stream assem, Stream app, string domainName, string assemName, string appName)
		{
			try
			{
				IsolatedStorage.GetUnrestricted().Assert();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				if (IsolatedStorage.IsApp(scope))
				{
					this.m_AppIdentity = binaryFormatter.Deserialize(app);
					this.m_AppName = appName;
				}
				else
				{
					this.m_AssemIdentity = binaryFormatter.Deserialize(assem);
					this.m_AssemName = assemName;
					if (IsolatedStorage.IsDomain(scope))
					{
						this.m_DomainIdentity = binaryFormatter.Deserialize(domain);
						this.m_DomainName = domainName;
					}
				}
			}
			catch
			{
				return false;
			}
			this.m_Scope = scope;
			return true;
		}

		[SecurityCritical]
		private void _InitStore(IsolatedStorageScope scope, Evidence domainEv, Type domainEvidenceType, Evidence assemEv, Type assemblyEvidenceType, Evidence appEv, Type appEvidenceType)
		{
			IsolatedStorage.VerifyScope(scope);
			if (IsolatedStorage.IsApp(scope))
			{
				if (appEv == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
				}
			}
			else
			{
				if (assemEv == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyMissingIdentity"));
				}
				if (IsolatedStorage.IsDomain(scope) && domainEv == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainMissingIdentity"));
				}
			}
			IsolatedStorage.DemandPermission(scope);
			string typeID = null;
			string instanceID = null;
			if (IsolatedStorage.IsApp(scope))
			{
				this.m_AppIdentity = IsolatedStorage.GetAccountingInfo(appEv, appEvidenceType, IsolatedStorageScope.Application, out typeID, out instanceID);
				this.m_AppName = this.GetNameFromID(typeID, instanceID);
			}
			else
			{
				this.m_AssemIdentity = IsolatedStorage.GetAccountingInfo(assemEv, assemblyEvidenceType, IsolatedStorageScope.Assembly, out typeID, out instanceID);
				this.m_AssemName = this.GetNameFromID(typeID, instanceID);
				if (IsolatedStorage.IsDomain(scope))
				{
					this.m_DomainIdentity = IsolatedStorage.GetAccountingInfo(domainEv, domainEvidenceType, IsolatedStorageScope.Domain, out typeID, out instanceID);
					this.m_DomainName = this.GetNameFromID(typeID, instanceID);
				}
			}
			this.m_Scope = scope;
		}

		[SecurityCritical]
		private static object GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, out string typeName, out string instanceName)
		{
			object obj = null;
			object obj2 = IsolatedStorage._GetAccountingInfo(evidence, evidenceType, fAssmDomApp, out obj);
			typeName = IsolatedStorage.GetPredefinedTypeName(obj2);
			if (typeName == null)
			{
				IsolatedStorage.GetUnrestricted().Assert();
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, obj2.GetType());
				memoryStream.Position = 0L;
				typeName = IsolatedStorage.GetHash(memoryStream);
				CodeAccessPermission.RevertAssert();
			}
			instanceName = null;
			if (obj != null)
			{
				if (obj is Stream)
				{
					instanceName = IsolatedStorage.GetHash((Stream)obj);
				}
				else if (obj is string)
				{
					if (IsolatedStorage.IsValidName((string)obj))
					{
						instanceName = (string)obj;
					}
					else
					{
						MemoryStream memoryStream = new MemoryStream();
						BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
						binaryWriter.Write((string)obj);
						memoryStream.Position = 0L;
						instanceName = IsolatedStorage.GetHash(memoryStream);
					}
				}
			}
			else
			{
				obj = obj2;
			}
			if (instanceName == null)
			{
				IsolatedStorage.GetUnrestricted().Assert();
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Position = 0L;
				instanceName = IsolatedStorage.GetHash(memoryStream);
				CodeAccessPermission.RevertAssert();
			}
			return obj2;
		}

		private static object _GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, out object oNormalized)
		{
			object hostEvidence;
			if (evidenceType == null)
			{
				hostEvidence = evidence.GetHostEvidence<Publisher>();
				if (hostEvidence == null)
				{
					hostEvidence = evidence.GetHostEvidence<StrongName>();
				}
				if (hostEvidence == null)
				{
					hostEvidence = evidence.GetHostEvidence<Url>();
				}
				if (hostEvidence == null)
				{
					hostEvidence = evidence.GetHostEvidence<Site>();
				}
				if (hostEvidence == null)
				{
					hostEvidence = evidence.GetHostEvidence<Zone>();
				}
				if (hostEvidence == null)
				{
					if (fAssmDomApp == IsolatedStorageScope.Domain)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainNoEvidence"));
					}
					if (fAssmDomApp == IsolatedStorageScope.Application)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationNoEvidence"));
					}
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyNoEvidence"));
				}
			}
			else
			{
				hostEvidence = evidence.GetHostEvidence(evidenceType);
				if (hostEvidence == null)
				{
					if (fAssmDomApp == IsolatedStorageScope.Domain)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DomainNoEvidence"));
					}
					if (fAssmDomApp == IsolatedStorageScope.Application)
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationNoEvidence"));
					}
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_AssemblyNoEvidence"));
				}
			}
			if (hostEvidence is INormalizeForIsolatedStorage)
			{
				oNormalized = ((INormalizeForIsolatedStorage)hostEvidence).Normalize();
			}
			else if (hostEvidence is Publisher)
			{
				oNormalized = ((Publisher)hostEvidence).Normalize();
			}
			else if (hostEvidence is StrongName)
			{
				oNormalized = ((StrongName)hostEvidence).Normalize();
			}
			else if (hostEvidence is Url)
			{
				oNormalized = ((Url)hostEvidence).Normalize();
			}
			else if (hostEvidence is Site)
			{
				oNormalized = ((Site)hostEvidence).Normalize();
			}
			else if (hostEvidence is Zone)
			{
				oNormalized = ((Zone)hostEvidence).Normalize();
			}
			else
			{
				oNormalized = null;
			}
			return hostEvidence;
		}

		[SecurityCritical]
		private static void DemandPermission(IsolatedStorageScope scope)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = null;
			if (scope <= (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming))
			{
				if (scope <= (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly))
				{
					if (scope != (IsolatedStorageScope.User | IsolatedStorageScope.Assembly))
					{
						if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly))
						{
							if (IsolatedStorage.s_PermDomain == null)
							{
								IsolatedStorage.s_PermDomain = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByUser, 0L, false);
							}
							isolatedStorageFilePermission = IsolatedStorage.s_PermDomain;
						}
					}
					else
					{
						if (IsolatedStorage.s_PermAssem == null)
						{
							IsolatedStorage.s_PermAssem = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByUser, 0L, false);
						}
						isolatedStorageFilePermission = IsolatedStorage.s_PermAssem;
					}
				}
				else if (scope != (IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming))
				{
					if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming))
					{
						if (IsolatedStorage.s_PermDomainRoaming == null)
						{
							IsolatedStorage.s_PermDomainRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByRoamingUser, 0L, false);
						}
						isolatedStorageFilePermission = IsolatedStorage.s_PermDomainRoaming;
					}
				}
				else
				{
					if (IsolatedStorage.s_PermAssemRoaming == null)
					{
						IsolatedStorage.s_PermAssemRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByRoamingUser, 0L, false);
					}
					isolatedStorageFilePermission = IsolatedStorage.s_PermAssemRoaming;
				}
			}
			else if (scope <= (IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine))
			{
				if (scope != (IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine))
				{
					if (scope == (IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine))
					{
						if (IsolatedStorage.s_PermMachineDomain == null)
						{
							IsolatedStorage.s_PermMachineDomain = new IsolatedStorageFilePermission(IsolatedStorageContainment.DomainIsolationByMachine, 0L, false);
						}
						isolatedStorageFilePermission = IsolatedStorage.s_PermMachineDomain;
					}
				}
				else
				{
					if (IsolatedStorage.s_PermMachineAssem == null)
					{
						IsolatedStorage.s_PermMachineAssem = new IsolatedStorageFilePermission(IsolatedStorageContainment.AssemblyIsolationByMachine, 0L, false);
					}
					isolatedStorageFilePermission = IsolatedStorage.s_PermMachineAssem;
				}
			}
			else if (scope != (IsolatedStorageScope.User | IsolatedStorageScope.Application))
			{
				if (scope != (IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application))
				{
					if (scope == (IsolatedStorageScope.Machine | IsolatedStorageScope.Application))
					{
						if (IsolatedStorage.s_PermAppMachine == null)
						{
							IsolatedStorage.s_PermAppMachine = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByMachine, 0L, false);
						}
						isolatedStorageFilePermission = IsolatedStorage.s_PermAppMachine;
					}
				}
				else
				{
					if (IsolatedStorage.s_PermAppUserRoaming == null)
					{
						IsolatedStorage.s_PermAppUserRoaming = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByRoamingUser, 0L, false);
					}
					isolatedStorageFilePermission = IsolatedStorage.s_PermAppUserRoaming;
				}
			}
			else
			{
				if (IsolatedStorage.s_PermAppUser == null)
				{
					IsolatedStorage.s_PermAppUser = new IsolatedStorageFilePermission(IsolatedStorageContainment.ApplicationIsolationByUser, 0L, false);
				}
				isolatedStorageFilePermission = IsolatedStorage.s_PermAppUser;
			}
			isolatedStorageFilePermission.Demand();
		}

		internal static void VerifyScope(IsolatedStorageScope scope)
		{
			if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly) || scope == (IsolatedStorageScope.User | IsolatedStorageScope.Assembly) || scope == (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming) || scope == (IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming) || scope == (IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine) || scope == (IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine) || scope == (IsolatedStorageScope.User | IsolatedStorageScope.Application) || scope == (IsolatedStorageScope.Machine | IsolatedStorageScope.Application) || scope == (IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application))
			{
				return;
			}
			throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_Scope_Invalid"));
		}

		[SecurityCritical]
		internal virtual void SetQuota(PermissionSet psAllowed, PermissionSet psDenied)
		{
			IsolatedStoragePermission permission = this.GetPermission(psAllowed);
			this.m_Quota = 0UL;
			if (permission != null)
			{
				if (permission.IsUnrestricted())
				{
					this.m_Quota = 9223372036854775807UL;
				}
				else
				{
					this.m_Quota = (ulong)permission.UserQuota;
				}
			}
			if (psDenied != null)
			{
				IsolatedStoragePermission permission2 = this.GetPermission(psDenied);
				if (permission2 != null)
				{
					if (permission2.IsUnrestricted())
					{
						this.m_Quota = 0UL;
					}
					else
					{
						ulong userQuota = (ulong)permission2.UserQuota;
						if (userQuota > this.m_Quota)
						{
							this.m_Quota = 0UL;
						}
						else
						{
							this.m_Quota -= userQuota;
						}
					}
				}
			}
			this.m_ValidQuota = true;
		}

		public abstract void Remove();

		protected abstract IsolatedStoragePermission GetPermission(PermissionSet ps);

		[SecuritySafeCritical]
		internal static RuntimeAssembly GetCaller()
		{
			RuntimeAssembly result = null;
			IsolatedStorage.GetCaller(JitHelpers.GetObjectHandleOnStack<RuntimeAssembly>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetCaller(ObjectHandleOnStack retAssembly);

		internal const IsolatedStorageScope c_Assembly = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;

		internal const IsolatedStorageScope c_Domain = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;

		internal const IsolatedStorageScope c_AssemblyRoaming = IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming;

		internal const IsolatedStorageScope c_DomainRoaming = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming;

		internal const IsolatedStorageScope c_MachineAssembly = IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;

		internal const IsolatedStorageScope c_MachineDomain = IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;

		internal const IsolatedStorageScope c_AppUser = IsolatedStorageScope.User | IsolatedStorageScope.Application;

		internal const IsolatedStorageScope c_AppMachine = IsolatedStorageScope.Machine | IsolatedStorageScope.Application;

		internal const IsolatedStorageScope c_AppUserRoaming = IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application;

		private const string s_Publisher = "Publisher";

		private const string s_StrongName = "StrongName";

		private const string s_Site = "Site";

		private const string s_Url = "Url";

		private const string s_Zone = "Zone";

		private ulong m_Quota;

		private bool m_ValidQuota;

		private object m_DomainIdentity;

		private object m_AssemIdentity;

		private object m_AppIdentity;

		private string m_DomainName;

		private string m_AssemName;

		private string m_AppName;

		private IsolatedStorageScope m_Scope;

		private static volatile IsolatedStorageFilePermission s_PermDomain;

		private static volatile IsolatedStorageFilePermission s_PermMachineDomain;

		private static volatile IsolatedStorageFilePermission s_PermDomainRoaming;

		private static volatile IsolatedStorageFilePermission s_PermAssem;

		private static volatile IsolatedStorageFilePermission s_PermMachineAssem;

		private static volatile IsolatedStorageFilePermission s_PermAssemRoaming;

		private static volatile IsolatedStorageFilePermission s_PermAppUser;

		private static volatile IsolatedStorageFilePermission s_PermAppMachine;

		private static volatile IsolatedStorageFilePermission s_PermAppUserRoaming;

		private static volatile SecurityPermission s_PermControlEvidence;

		private static volatile PermissionSet s_PermUnrestricted;
	}
}
