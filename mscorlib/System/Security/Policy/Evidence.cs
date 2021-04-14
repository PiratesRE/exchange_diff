using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Threading;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Evidence : ICollection, IEnumerable
	{
		public Evidence()
		{
			this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
			this.m_evidenceLock = new ReaderWriterLock();
		}

		public Evidence(Evidence evidence)
		{
			this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
			if (evidence != null)
			{
				using (new Evidence.EvidenceLockHolder(evidence, Evidence.EvidenceLockHolder.LockType.Reader))
				{
					foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in evidence.m_evidence)
					{
						EvidenceTypeDescriptor evidenceTypeDescriptor = keyValuePair.Value;
						if (evidenceTypeDescriptor != null)
						{
							evidenceTypeDescriptor = evidenceTypeDescriptor.Clone();
						}
						this.m_evidence[keyValuePair.Key] = evidenceTypeDescriptor;
					}
					this.m_target = evidence.m_target;
					this.m_locked = evidence.m_locked;
					this.m_deserializedTargetEvidence = evidence.m_deserializedTargetEvidence;
					if (evidence.Target != null)
					{
						this.m_cloneOrigin = new WeakReference(evidence);
					}
				}
			}
			this.m_evidenceLock = new ReaderWriterLock();
		}

		[Obsolete("This constructor is obsolete. Please use the constructor which takes arrays of EvidenceBase instead.")]
		public Evidence(object[] hostEvidence, object[] assemblyEvidence)
		{
			this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
			if (hostEvidence != null)
			{
				foreach (object id in hostEvidence)
				{
					this.AddHost(id);
				}
			}
			if (assemblyEvidence != null)
			{
				foreach (object id2 in assemblyEvidence)
				{
					this.AddAssembly(id2);
				}
			}
			this.m_evidenceLock = new ReaderWriterLock();
		}

		public Evidence(EvidenceBase[] hostEvidence, EvidenceBase[] assemblyEvidence)
		{
			this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
			if (hostEvidence != null)
			{
				foreach (EvidenceBase evidence in hostEvidence)
				{
					this.AddHostEvidence(evidence, Evidence.GetEvidenceIndexType(evidence), Evidence.DuplicateEvidenceAction.Throw);
				}
			}
			if (assemblyEvidence != null)
			{
				foreach (EvidenceBase evidence2 in assemblyEvidence)
				{
					this.AddAssemblyEvidence(evidence2, Evidence.GetEvidenceIndexType(evidence2), Evidence.DuplicateEvidenceAction.Throw);
				}
			}
			this.m_evidenceLock = new ReaderWriterLock();
		}

		[SecuritySafeCritical]
		internal Evidence(IRuntimeEvidenceFactory target)
		{
			this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
			this.m_target = target;
			foreach (Type key in Evidence.RuntimeEvidenceTypes)
			{
				this.m_evidence[key] = null;
			}
			this.QueryHostForPossibleEvidenceTypes();
			this.m_evidenceLock = new ReaderWriterLock();
		}

		internal static Type[] RuntimeEvidenceTypes
		{
			get
			{
				if (Evidence.s_runtimeEvidenceTypes == null)
				{
					Type[] array = new Type[]
					{
						typeof(ActivationArguments),
						typeof(ApplicationDirectory),
						typeof(ApplicationTrust),
						typeof(GacInstalled),
						typeof(Hash),
						typeof(Publisher),
						typeof(Site),
						typeof(StrongName),
						typeof(Url),
						typeof(Zone)
					};
					if (AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
					{
						int num = array.Length;
						Array.Resize<Type>(ref array, num + 1);
						array[num] = typeof(PermissionRequestEvidence);
					}
					Evidence.s_runtimeEvidenceTypes = array;
				}
				return Evidence.s_runtimeEvidenceTypes;
			}
		}

		private bool IsReaderLockHeld
		{
			get
			{
				return this.m_evidenceLock == null || this.m_evidenceLock.IsReaderLockHeld;
			}
		}

		private bool IsWriterLockHeld
		{
			get
			{
				return this.m_evidenceLock == null || this.m_evidenceLock.IsWriterLockHeld;
			}
		}

		private void AcquireReaderLock()
		{
			if (this.m_evidenceLock != null)
			{
				this.m_evidenceLock.AcquireReaderLock(5000);
			}
		}

		private void AcquireWriterlock()
		{
			if (this.m_evidenceLock != null)
			{
				this.m_evidenceLock.AcquireWriterLock(5000);
			}
		}

		private void DowngradeFromWriterLock(ref LockCookie lockCookie)
		{
			if (this.m_evidenceLock != null)
			{
				this.m_evidenceLock.DowngradeFromWriterLock(ref lockCookie);
			}
		}

		private LockCookie UpgradeToWriterLock()
		{
			if (this.m_evidenceLock == null)
			{
				return default(LockCookie);
			}
			return this.m_evidenceLock.UpgradeToWriterLock(5000);
		}

		private void ReleaseReaderLock()
		{
			if (this.m_evidenceLock != null)
			{
				this.m_evidenceLock.ReleaseReaderLock();
			}
		}

		private void ReleaseWriterLock()
		{
			if (this.m_evidenceLock != null)
			{
				this.m_evidenceLock.ReleaseWriterLock();
			}
		}

		[Obsolete("This method is obsolete. Please use AddHostEvidence instead.")]
		[SecuritySafeCritical]
		public void AddHost(object id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (!id.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Policy_EvidenceMustBeSerializable"), "id");
			}
			if (this.m_locked)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			EvidenceBase evidence = Evidence.WrapLegacyEvidence(id);
			Type evidenceIndexType = Evidence.GetEvidenceIndexType(evidence);
			this.AddHostEvidence(evidence, evidenceIndexType, Evidence.DuplicateEvidenceAction.Merge);
		}

		[Obsolete("This method is obsolete. Please use AddAssemblyEvidence instead.")]
		public void AddAssembly(object id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (!id.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Policy_EvidenceMustBeSerializable"), "id");
			}
			EvidenceBase evidence = Evidence.WrapLegacyEvidence(id);
			Type evidenceIndexType = Evidence.GetEvidenceIndexType(evidence);
			this.AddAssemblyEvidence(evidence, evidenceIndexType, Evidence.DuplicateEvidenceAction.Merge);
		}

		[ComVisible(false)]
		public void AddAssemblyEvidence<T>(T evidence) where T : EvidenceBase
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			Type evidenceType = typeof(T);
			if (typeof(T) == typeof(EvidenceBase) || evidence is ILegacyEvidenceAdapter)
			{
				evidenceType = Evidence.GetEvidenceIndexType(evidence);
			}
			this.AddAssemblyEvidence(evidence, evidenceType, Evidence.DuplicateEvidenceAction.Throw);
		}

		private void AddAssemblyEvidence(EvidenceBase evidence, Type evidenceType, Evidence.DuplicateEvidenceAction duplicateAction)
		{
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				this.AddAssemblyEvidenceNoLock(evidence, evidenceType, duplicateAction);
			}
		}

		private void AddAssemblyEvidenceNoLock(EvidenceBase evidence, Type evidenceType, Evidence.DuplicateEvidenceAction duplicateAction)
		{
			this.DeserializeTargetEvidence();
			EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType, true);
			this.m_version += 1U;
			if (evidenceTypeDescriptor.AssemblyEvidence == null)
			{
				evidenceTypeDescriptor.AssemblyEvidence = evidence;
				return;
			}
			evidenceTypeDescriptor.AssemblyEvidence = Evidence.HandleDuplicateEvidence(evidenceTypeDescriptor.AssemblyEvidence, evidence, duplicateAction);
		}

		[ComVisible(false)]
		public void AddHostEvidence<T>(T evidence) where T : EvidenceBase
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			Type evidenceType = typeof(T);
			if (typeof(T) == typeof(EvidenceBase) || evidence is ILegacyEvidenceAdapter)
			{
				evidenceType = Evidence.GetEvidenceIndexType(evidence);
			}
			this.AddHostEvidence(evidence, evidenceType, Evidence.DuplicateEvidenceAction.Throw);
		}

		[SecuritySafeCritical]
		private void AddHostEvidence(EvidenceBase evidence, Type evidenceType, Evidence.DuplicateEvidenceAction duplicateAction)
		{
			if (this.Locked)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				this.AddHostEvidenceNoLock(evidence, evidenceType, duplicateAction);
			}
		}

		private void AddHostEvidenceNoLock(EvidenceBase evidence, Type evidenceType, Evidence.DuplicateEvidenceAction duplicateAction)
		{
			EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType, true);
			this.m_version += 1U;
			if (evidenceTypeDescriptor.HostEvidence == null)
			{
				evidenceTypeDescriptor.HostEvidence = evidence;
				return;
			}
			evidenceTypeDescriptor.HostEvidence = Evidence.HandleDuplicateEvidence(evidenceTypeDescriptor.HostEvidence, evidence, duplicateAction);
		}

		[SecurityCritical]
		private void QueryHostForPossibleEvidenceTypes()
		{
			if (AppDomain.CurrentDomain.DomainManager != null)
			{
				HostSecurityManager hostSecurityManager = AppDomain.CurrentDomain.DomainManager.HostSecurityManager;
				if (hostSecurityManager != null)
				{
					Type[] array = null;
					AppDomain appDomain = this.m_target.Target as AppDomain;
					Assembly assembly = this.m_target.Target as Assembly;
					if (assembly != null && (hostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
					{
						array = hostSecurityManager.GetHostSuppliedAssemblyEvidenceTypes(assembly);
					}
					else if (appDomain != null && (hostSecurityManager.Flags & HostSecurityManagerOptions.HostAppDomainEvidence) == HostSecurityManagerOptions.HostAppDomainEvidence)
					{
						array = hostSecurityManager.GetHostSuppliedAppDomainEvidenceTypes();
					}
					if (array != null)
					{
						foreach (Type evidenceType in array)
						{
							EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType, true);
							evidenceTypeDescriptor.HostCanGenerate = true;
						}
					}
				}
			}
		}

		internal bool IsUnmodified
		{
			get
			{
				return this.m_version == 0U;
			}
		}

		public bool Locked
		{
			get
			{
				return this.m_locked;
			}
			[SecuritySafeCritical]
			set
			{
				if (!value)
				{
					new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
					this.m_locked = false;
					return;
				}
				this.m_locked = true;
			}
		}

		internal IRuntimeEvidenceFactory Target
		{
			get
			{
				return this.m_target;
			}
			[SecurityCritical]
			set
			{
				using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
				{
					this.m_target = value;
					this.QueryHostForPossibleEvidenceTypes();
				}
			}
		}

		private static Type GetEvidenceIndexType(EvidenceBase evidence)
		{
			ILegacyEvidenceAdapter legacyEvidenceAdapter = evidence as ILegacyEvidenceAdapter;
			if (legacyEvidenceAdapter != null)
			{
				return legacyEvidenceAdapter.EvidenceType;
			}
			return evidence.GetType();
		}

		internal EvidenceTypeDescriptor GetEvidenceTypeDescriptor(Type evidenceType)
		{
			return this.GetEvidenceTypeDescriptor(evidenceType, false);
		}

		private EvidenceTypeDescriptor GetEvidenceTypeDescriptor(Type evidenceType, bool addIfNotExist)
		{
			EvidenceTypeDescriptor evidenceTypeDescriptor = null;
			if (!this.m_evidence.TryGetValue(evidenceType, out evidenceTypeDescriptor) && !addIfNotExist)
			{
				return null;
			}
			if (evidenceTypeDescriptor == null)
			{
				evidenceTypeDescriptor = new EvidenceTypeDescriptor();
				bool flag = false;
				LockCookie lockCookie = default(LockCookie);
				try
				{
					if (!this.IsWriterLockHeld)
					{
						lockCookie = this.UpgradeToWriterLock();
						flag = true;
					}
					this.m_evidence[evidenceType] = evidenceTypeDescriptor;
				}
				finally
				{
					if (flag)
					{
						this.DowngradeFromWriterLock(ref lockCookie);
					}
				}
			}
			return evidenceTypeDescriptor;
		}

		private static EvidenceBase HandleDuplicateEvidence(EvidenceBase original, EvidenceBase duplicate, Evidence.DuplicateEvidenceAction action)
		{
			switch (action)
			{
			case Evidence.DuplicateEvidenceAction.Throw:
				throw new InvalidOperationException(Environment.GetResourceString("Policy_DuplicateEvidence", new object[]
				{
					duplicate.GetType().FullName
				}));
			case Evidence.DuplicateEvidenceAction.Merge:
			{
				LegacyEvidenceList legacyEvidenceList = original as LegacyEvidenceList;
				if (legacyEvidenceList == null)
				{
					legacyEvidenceList = new LegacyEvidenceList();
					legacyEvidenceList.Add(original);
				}
				legacyEvidenceList.Add(duplicate);
				return legacyEvidenceList;
			}
			case Evidence.DuplicateEvidenceAction.SelectNewObject:
				return duplicate;
			default:
				return null;
			}
		}

		private static EvidenceBase WrapLegacyEvidence(object evidence)
		{
			EvidenceBase evidenceBase = evidence as EvidenceBase;
			if (evidenceBase == null)
			{
				evidenceBase = new LegacyEvidenceWrapper(evidence);
			}
			return evidenceBase;
		}

		private static object UnwrapEvidence(EvidenceBase evidence)
		{
			ILegacyEvidenceAdapter legacyEvidenceAdapter = evidence as ILegacyEvidenceAdapter;
			if (legacyEvidenceAdapter != null)
			{
				return legacyEvidenceAdapter.EvidenceObject;
			}
			return evidence;
		}

		[SecuritySafeCritical]
		public void Merge(Evidence evidence)
		{
			if (evidence == null)
			{
				return;
			}
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				bool flag = false;
				IEnumerator hostEnumerator = evidence.GetHostEnumerator();
				while (hostEnumerator.MoveNext())
				{
					if (this.Locked && !flag)
					{
						new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
						flag = true;
					}
					Type type = hostEnumerator.Current.GetType();
					if (this.m_evidence.ContainsKey(type))
					{
						this.GetHostEvidenceNoLock(type);
					}
					EvidenceBase evidence2 = Evidence.WrapLegacyEvidence(hostEnumerator.Current);
					this.AddHostEvidenceNoLock(evidence2, Evidence.GetEvidenceIndexType(evidence2), Evidence.DuplicateEvidenceAction.Merge);
				}
				IEnumerator assemblyEnumerator = evidence.GetAssemblyEnumerator();
				while (assemblyEnumerator.MoveNext())
				{
					object evidence3 = assemblyEnumerator.Current;
					EvidenceBase evidence4 = Evidence.WrapLegacyEvidence(evidence3);
					this.AddAssemblyEvidenceNoLock(evidence4, Evidence.GetEvidenceIndexType(evidence4), Evidence.DuplicateEvidenceAction.Merge);
				}
			}
		}

		internal void MergeWithNoDuplicates(Evidence evidence)
		{
			if (evidence == null)
			{
				return;
			}
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				IEnumerator hostEnumerator = evidence.GetHostEnumerator();
				while (hostEnumerator.MoveNext())
				{
					object evidence2 = hostEnumerator.Current;
					EvidenceBase evidence3 = Evidence.WrapLegacyEvidence(evidence2);
					this.AddHostEvidenceNoLock(evidence3, Evidence.GetEvidenceIndexType(evidence3), Evidence.DuplicateEvidenceAction.SelectNewObject);
				}
				IEnumerator assemblyEnumerator = evidence.GetAssemblyEnumerator();
				while (assemblyEnumerator.MoveNext())
				{
					object evidence4 = assemblyEnumerator.Current;
					EvidenceBase evidence5 = Evidence.WrapLegacyEvidence(evidence4);
					this.AddAssemblyEvidenceNoLock(evidence5, Evidence.GetEvidenceIndexType(evidence5), Evidence.DuplicateEvidenceAction.SelectNewObject);
				}
			}
		}

		[ComVisible(false)]
		[OnSerializing]
		[SecurityCritical]
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		private void OnSerializing(StreamingContext context)
		{
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				foreach (Type type in new List<Type>(this.m_evidence.Keys))
				{
					this.GetHostEvidenceNoLock(type);
				}
				this.DeserializeTargetEvidence();
			}
			ArrayList arrayList = new ArrayList();
			IEnumerator hostEnumerator = this.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				object value = hostEnumerator.Current;
				arrayList.Add(value);
			}
			this.m_hostList = arrayList;
			ArrayList arrayList2 = new ArrayList();
			IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
			while (assemblyEnumerator.MoveNext())
			{
				object value2 = assemblyEnumerator.Current;
				arrayList2.Add(value2);
			}
			this.m_assemblyList = arrayList2;
		}

		[ComVisible(false)]
		[OnDeserialized]
		[SecurityCritical]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.m_evidence == null)
			{
				this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
				if (this.m_hostList != null)
				{
					foreach (object obj in this.m_hostList)
					{
						if (obj != null)
						{
							this.AddHost(obj);
						}
					}
					this.m_hostList = null;
				}
				if (this.m_assemblyList != null)
				{
					foreach (object obj2 in this.m_assemblyList)
					{
						if (obj2 != null)
						{
							this.AddAssembly(obj2);
						}
					}
					this.m_assemblyList = null;
				}
			}
			this.m_evidenceLock = new ReaderWriterLock();
		}

		private void DeserializeTargetEvidence()
		{
			if (this.m_target != null && !this.m_deserializedTargetEvidence)
			{
				bool flag = false;
				LockCookie lockCookie = default(LockCookie);
				try
				{
					if (!this.IsWriterLockHeld)
					{
						lockCookie = this.UpgradeToWriterLock();
						flag = true;
					}
					this.m_deserializedTargetEvidence = true;
					foreach (EvidenceBase evidence in this.m_target.GetFactorySuppliedEvidence())
					{
						this.AddAssemblyEvidenceNoLock(evidence, Evidence.GetEvidenceIndexType(evidence), Evidence.DuplicateEvidenceAction.Throw);
					}
				}
				finally
				{
					if (flag)
					{
						this.DowngradeFromWriterLock(ref lockCookie);
					}
				}
			}
		}

		[SecurityCritical]
		internal byte[] RawSerialize()
		{
			byte[] result;
			try
			{
				using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
				{
					Dictionary<Type, EvidenceBase> dictionary = new Dictionary<Type, EvidenceBase>();
					foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in this.m_evidence)
					{
						if (keyValuePair.Value != null && keyValuePair.Value.HostEvidence != null)
						{
							dictionary[keyValuePair.Key] = keyValuePair.Value.HostEvidence;
						}
					}
					using (MemoryStream memoryStream = new MemoryStream())
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						binaryFormatter.Serialize(memoryStream, dictionary);
						result = memoryStream.ToArray();
					}
				}
			}
			catch (SecurityException)
			{
				result = null;
			}
			return result;
		}

		[Obsolete("Evidence should not be treated as an ICollection. Please use the GetHostEnumerator and GetAssemblyEnumerator methods rather than using CopyTo.")]
		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index > array.Length - this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = index;
			IEnumerator hostEnumerator = this.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				object value = hostEnumerator.Current;
				array.SetValue(value, num);
				num++;
			}
			IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
			while (assemblyEnumerator.MoveNext())
			{
				object value2 = assemblyEnumerator.Current;
				array.SetValue(value2, num);
				num++;
			}
		}

		public IEnumerator GetHostEnumerator()
		{
			IEnumerator result;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				result = new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Host);
			}
			return result;
		}

		public IEnumerator GetAssemblyEnumerator()
		{
			IEnumerator result;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				this.DeserializeTargetEvidence();
				result = new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Assembly);
			}
			return result;
		}

		internal Evidence.RawEvidenceEnumerator GetRawAssemblyEvidenceEnumerator()
		{
			this.DeserializeTargetEvidence();
			return new Evidence.RawEvidenceEnumerator(this, new List<Type>(this.m_evidence.Keys), false);
		}

		internal Evidence.RawEvidenceEnumerator GetRawHostEvidenceEnumerator()
		{
			return new Evidence.RawEvidenceEnumerator(this, new List<Type>(this.m_evidence.Keys), true);
		}

		[Obsolete("GetEnumerator is obsolete. Please use GetAssemblyEnumerator and GetHostEnumerator instead.")]
		public IEnumerator GetEnumerator()
		{
			IEnumerator result;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				result = new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Host | Evidence.EvidenceEnumerator.Category.Assembly);
			}
			return result;
		}

		[ComVisible(false)]
		public T GetAssemblyEvidence<T>() where T : EvidenceBase
		{
			return Evidence.UnwrapEvidence(this.GetAssemblyEvidence(typeof(T))) as T;
		}

		internal EvidenceBase GetAssemblyEvidence(Type type)
		{
			EvidenceBase assemblyEvidenceNoLock;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				assemblyEvidenceNoLock = this.GetAssemblyEvidenceNoLock(type);
			}
			return assemblyEvidenceNoLock;
		}

		private EvidenceBase GetAssemblyEvidenceNoLock(Type type)
		{
			this.DeserializeTargetEvidence();
			EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(type);
			if (evidenceTypeDescriptor != null)
			{
				return evidenceTypeDescriptor.AssemblyEvidence;
			}
			return null;
		}

		[ComVisible(false)]
		public T GetHostEvidence<T>() where T : EvidenceBase
		{
			return Evidence.UnwrapEvidence(this.GetHostEvidence(typeof(T))) as T;
		}

		internal T GetDelayEvaluatedHostEvidence<T>() where T : EvidenceBase, IDelayEvaluatedEvidence
		{
			return Evidence.UnwrapEvidence(this.GetHostEvidence(typeof(T), false)) as T;
		}

		internal EvidenceBase GetHostEvidence(Type type)
		{
			return this.GetHostEvidence(type, true);
		}

		[SecuritySafeCritical]
		private EvidenceBase GetHostEvidence(Type type, bool markDelayEvaluatedEvidenceUsed)
		{
			EvidenceBase result;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				EvidenceBase hostEvidenceNoLock = this.GetHostEvidenceNoLock(type);
				if (markDelayEvaluatedEvidenceUsed)
				{
					IDelayEvaluatedEvidence delayEvaluatedEvidence = hostEvidenceNoLock as IDelayEvaluatedEvidence;
					if (delayEvaluatedEvidence != null)
					{
						delayEvaluatedEvidence.MarkUsed();
					}
				}
				result = hostEvidenceNoLock;
			}
			return result;
		}

		[SecurityCritical]
		private EvidenceBase GetHostEvidenceNoLock(Type type)
		{
			EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(type);
			if (evidenceTypeDescriptor == null)
			{
				return null;
			}
			if (evidenceTypeDescriptor.HostEvidence != null)
			{
				return evidenceTypeDescriptor.HostEvidence;
			}
			if (this.m_target != null && !evidenceTypeDescriptor.Generated)
			{
				using (new Evidence.EvidenceUpgradeLockHolder(this))
				{
					evidenceTypeDescriptor.Generated = true;
					EvidenceBase evidenceBase = this.GenerateHostEvidence(type, evidenceTypeDescriptor.HostCanGenerate);
					if (evidenceBase != null)
					{
						evidenceTypeDescriptor.HostEvidence = evidenceBase;
						Evidence evidence = (this.m_cloneOrigin != null) ? (this.m_cloneOrigin.Target as Evidence) : null;
						if (evidence != null)
						{
							using (new Evidence.EvidenceLockHolder(evidence, Evidence.EvidenceLockHolder.LockType.Writer))
							{
								EvidenceTypeDescriptor evidenceTypeDescriptor2 = evidence.GetEvidenceTypeDescriptor(type);
								if (evidenceTypeDescriptor2 != null && evidenceTypeDescriptor2.HostEvidence == null)
								{
									evidenceTypeDescriptor2.HostEvidence = evidenceBase.Clone();
								}
							}
						}
					}
					return evidenceBase;
				}
			}
			return null;
		}

		[SecurityCritical]
		private EvidenceBase GenerateHostEvidence(Type type, bool hostCanGenerate)
		{
			if (hostCanGenerate)
			{
				AppDomain appDomain = this.m_target.Target as AppDomain;
				Assembly assembly = this.m_target.Target as Assembly;
				EvidenceBase evidenceBase = null;
				if (appDomain != null)
				{
					evidenceBase = AppDomain.CurrentDomain.HostSecurityManager.GenerateAppDomainEvidence(type);
				}
				else if (assembly != null)
				{
					evidenceBase = AppDomain.CurrentDomain.HostSecurityManager.GenerateAssemblyEvidence(type, assembly);
				}
				if (evidenceBase != null)
				{
					if (!type.IsAssignableFrom(evidenceBase.GetType()))
					{
						string fullName = AppDomain.CurrentDomain.HostSecurityManager.GetType().FullName;
						string fullName2 = evidenceBase.GetType().FullName;
						string fullName3 = type.FullName;
						throw new InvalidOperationException(Environment.GetResourceString("Policy_IncorrectHostEvidence", new object[]
						{
							fullName,
							fullName2,
							fullName3
						}));
					}
					return evidenceBase;
				}
			}
			return this.m_target.GenerateEvidence(type);
		}

		[Obsolete("Evidence should not be treated as an ICollection. Please use GetHostEnumerator and GetAssemblyEnumerator to iterate over the evidence to collect a count.")]
		public int Count
		{
			get
			{
				int num = 0;
				IEnumerator hostEnumerator = this.GetHostEnumerator();
				while (hostEnumerator.MoveNext())
				{
					num++;
				}
				IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
				while (assemblyEnumerator.MoveNext())
				{
					num++;
				}
				return num;
			}
		}

		[ComVisible(false)]
		internal int RawCount
		{
			get
			{
				int num = 0;
				using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
				{
					foreach (Type evidenceType in new List<Type>(this.m_evidence.Keys))
					{
						EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType);
						if (evidenceTypeDescriptor != null)
						{
							if (evidenceTypeDescriptor.AssemblyEvidence != null)
							{
								num++;
							}
							if (evidenceTypeDescriptor.HostEvidence != null)
							{
								num++;
							}
						}
					}
				}
				return num;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[ComVisible(false)]
		public Evidence Clone()
		{
			return new Evidence(this);
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		public void Clear()
		{
			if (this.Locked)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				this.m_version += 1U;
				this.m_evidence.Clear();
			}
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		public void RemoveType(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
			{
				EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(t);
				if (evidenceTypeDescriptor != null)
				{
					this.m_version += 1U;
					if (this.Locked && (evidenceTypeDescriptor.HostEvidence != null || evidenceTypeDescriptor.HostCanGenerate))
					{
						new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
					}
					this.m_evidence.Remove(t);
				}
			}
		}

		internal void MarkAllEvidenceAsUsed()
		{
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in this.m_evidence)
				{
					if (keyValuePair.Value != null)
					{
						IDelayEvaluatedEvidence delayEvaluatedEvidence = keyValuePair.Value.HostEvidence as IDelayEvaluatedEvidence;
						if (delayEvaluatedEvidence != null)
						{
							delayEvaluatedEvidence.MarkUsed();
						}
						IDelayEvaluatedEvidence delayEvaluatedEvidence2 = keyValuePair.Value.AssemblyEvidence as IDelayEvaluatedEvidence;
						if (delayEvaluatedEvidence2 != null)
						{
							delayEvaluatedEvidence2.MarkUsed();
						}
					}
				}
			}
		}

		private bool WasStrongNameEvidenceUsed()
		{
			bool result;
			using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
			{
				EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(typeof(StrongName));
				if (evidenceTypeDescriptor != null)
				{
					IDelayEvaluatedEvidence delayEvaluatedEvidence = evidenceTypeDescriptor.HostEvidence as IDelayEvaluatedEvidence;
					result = (delayEvaluatedEvidence != null && delayEvaluatedEvidence.WasUsed);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		[OptionalField(VersionAdded = 4)]
		private Dictionary<Type, EvidenceTypeDescriptor> m_evidence;

		[OptionalField(VersionAdded = 4)]
		private bool m_deserializedTargetEvidence;

		private volatile ArrayList m_hostList;

		private volatile ArrayList m_assemblyList;

		[NonSerialized]
		private ReaderWriterLock m_evidenceLock;

		[NonSerialized]
		private uint m_version;

		[NonSerialized]
		private IRuntimeEvidenceFactory m_target;

		private bool m_locked;

		[NonSerialized]
		private WeakReference m_cloneOrigin;

		private static volatile Type[] s_runtimeEvidenceTypes;

		private const int LockTimeout = 5000;

		private enum DuplicateEvidenceAction
		{
			Throw,
			Merge,
			SelectNewObject
		}

		private class EvidenceLockHolder : IDisposable
		{
			public EvidenceLockHolder(Evidence target, Evidence.EvidenceLockHolder.LockType lockType)
			{
				this.m_target = target;
				this.m_lockType = lockType;
				if (this.m_lockType == Evidence.EvidenceLockHolder.LockType.Reader)
				{
					this.m_target.AcquireReaderLock();
					return;
				}
				this.m_target.AcquireWriterlock();
			}

			public void Dispose()
			{
				if (this.m_lockType == Evidence.EvidenceLockHolder.LockType.Reader && this.m_target.IsReaderLockHeld)
				{
					this.m_target.ReleaseReaderLock();
					return;
				}
				if (this.m_lockType == Evidence.EvidenceLockHolder.LockType.Writer && this.m_target.IsWriterLockHeld)
				{
					this.m_target.ReleaseWriterLock();
				}
			}

			private Evidence m_target;

			private Evidence.EvidenceLockHolder.LockType m_lockType;

			public enum LockType
			{
				Reader,
				Writer
			}
		}

		private class EvidenceUpgradeLockHolder : IDisposable
		{
			public EvidenceUpgradeLockHolder(Evidence target)
			{
				this.m_target = target;
				this.m_cookie = this.m_target.UpgradeToWriterLock();
			}

			public void Dispose()
			{
				if (this.m_target.IsWriterLockHeld)
				{
					this.m_target.DowngradeFromWriterLock(ref this.m_cookie);
				}
			}

			private Evidence m_target;

			private LockCookie m_cookie;
		}

		internal sealed class RawEvidenceEnumerator : IEnumerator<EvidenceBase>, IDisposable, IEnumerator
		{
			public RawEvidenceEnumerator(Evidence evidence, IEnumerable<Type> evidenceTypes, bool hostEnumerator)
			{
				this.m_evidence = evidence;
				this.m_hostEnumerator = hostEnumerator;
				this.m_evidenceTypes = Evidence.RawEvidenceEnumerator.GenerateEvidenceTypes(evidence, evidenceTypes, hostEnumerator);
				this.m_evidenceVersion = evidence.m_version;
				this.Reset();
			}

			public EvidenceBase Current
			{
				get
				{
					if (this.m_evidence.m_version != this.m_evidenceVersion)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumFailedVersion"));
					}
					return this.m_currentEvidence;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.m_evidence.m_version != this.m_evidenceVersion)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumFailedVersion"));
					}
					return this.m_currentEvidence;
				}
			}

			private static List<Type> ExpensiveEvidence
			{
				get
				{
					if (Evidence.RawEvidenceEnumerator.s_expensiveEvidence == null)
					{
						Evidence.RawEvidenceEnumerator.s_expensiveEvidence = new List<Type>
						{
							typeof(Hash),
							typeof(Publisher)
						};
					}
					return Evidence.RawEvidenceEnumerator.s_expensiveEvidence;
				}
			}

			public void Dispose()
			{
			}

			private static Type[] GenerateEvidenceTypes(Evidence evidence, IEnumerable<Type> evidenceTypes, bool hostEvidence)
			{
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				List<Type> list3 = new List<Type>(Evidence.RawEvidenceEnumerator.ExpensiveEvidence.Count);
				foreach (Type type in evidenceTypes)
				{
					EvidenceTypeDescriptor evidenceTypeDescriptor = evidence.GetEvidenceTypeDescriptor(type);
					bool flag = (hostEvidence && evidenceTypeDescriptor.HostEvidence != null) || (!hostEvidence && evidenceTypeDescriptor.AssemblyEvidence != null);
					if (flag)
					{
						list.Add(type);
					}
					else if (Evidence.RawEvidenceEnumerator.ExpensiveEvidence.Contains(type))
					{
						list3.Add(type);
					}
					else
					{
						list2.Add(type);
					}
				}
				Type[] array = new Type[list.Count + list2.Count + list3.Count];
				list.CopyTo(array, 0);
				list2.CopyTo(array, list.Count);
				list3.CopyTo(array, list.Count + list2.Count);
				return array;
			}

			[SecuritySafeCritical]
			public bool MoveNext()
			{
				using (new Evidence.EvidenceLockHolder(this.m_evidence, Evidence.EvidenceLockHolder.LockType.Reader))
				{
					if (this.m_evidence.m_version != this.m_evidenceVersion)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumFailedVersion"));
					}
					this.m_currentEvidence = null;
					do
					{
						this.m_typeIndex++;
						if (this.m_typeIndex < this.m_evidenceTypes.Length)
						{
							if (this.m_hostEnumerator)
							{
								this.m_currentEvidence = this.m_evidence.GetHostEvidenceNoLock(this.m_evidenceTypes[this.m_typeIndex]);
							}
							else
							{
								this.m_currentEvidence = this.m_evidence.GetAssemblyEvidenceNoLock(this.m_evidenceTypes[this.m_typeIndex]);
							}
						}
					}
					while (this.m_typeIndex < this.m_evidenceTypes.Length && this.m_currentEvidence == null);
				}
				return this.m_currentEvidence != null;
			}

			public void Reset()
			{
				if (this.m_evidence.m_version != this.m_evidenceVersion)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumFailedVersion"));
				}
				this.m_typeIndex = -1;
				this.m_currentEvidence = null;
			}

			private Evidence m_evidence;

			private bool m_hostEnumerator;

			private uint m_evidenceVersion;

			private Type[] m_evidenceTypes;

			private int m_typeIndex;

			private EvidenceBase m_currentEvidence;

			private static volatile List<Type> s_expensiveEvidence;
		}

		private sealed class EvidenceEnumerator : IEnumerator
		{
			internal EvidenceEnumerator(Evidence evidence, Evidence.EvidenceEnumerator.Category category)
			{
				this.m_evidence = evidence;
				this.m_category = category;
				this.ResetNoLock();
			}

			public bool MoveNext()
			{
				IEnumerator currentEnumerator = this.CurrentEnumerator;
				if (currentEnumerator == null)
				{
					this.m_currentEvidence = null;
					return false;
				}
				if (currentEnumerator.MoveNext())
				{
					LegacyEvidenceWrapper legacyEvidenceWrapper = currentEnumerator.Current as LegacyEvidenceWrapper;
					LegacyEvidenceList legacyEvidenceList = currentEnumerator.Current as LegacyEvidenceList;
					if (legacyEvidenceWrapper != null)
					{
						this.m_currentEvidence = legacyEvidenceWrapper.EvidenceObject;
					}
					else if (legacyEvidenceList != null)
					{
						IEnumerator enumerator = legacyEvidenceList.GetEnumerator();
						this.m_enumerators.Push(enumerator);
						this.MoveNext();
					}
					else
					{
						this.m_currentEvidence = currentEnumerator.Current;
					}
					return true;
				}
				this.m_enumerators.Pop();
				return this.MoveNext();
			}

			public object Current
			{
				get
				{
					return this.m_currentEvidence;
				}
			}

			private IEnumerator CurrentEnumerator
			{
				get
				{
					if (this.m_enumerators.Count <= 0)
					{
						return null;
					}
					return this.m_enumerators.Peek() as IEnumerator;
				}
			}

			public void Reset()
			{
				using (new Evidence.EvidenceLockHolder(this.m_evidence, Evidence.EvidenceLockHolder.LockType.Reader))
				{
					this.ResetNoLock();
				}
			}

			private void ResetNoLock()
			{
				this.m_currentEvidence = null;
				this.m_enumerators = new Stack();
				if ((this.m_category & Evidence.EvidenceEnumerator.Category.Host) == Evidence.EvidenceEnumerator.Category.Host)
				{
					this.m_enumerators.Push(this.m_evidence.GetRawHostEvidenceEnumerator());
				}
				if ((this.m_category & Evidence.EvidenceEnumerator.Category.Assembly) == Evidence.EvidenceEnumerator.Category.Assembly)
				{
					this.m_enumerators.Push(this.m_evidence.GetRawAssemblyEvidenceEnumerator());
				}
			}

			private Evidence m_evidence;

			private Evidence.EvidenceEnumerator.Category m_category;

			private Stack m_enumerators;

			private object m_currentEvidence;

			[Flags]
			internal enum Category
			{
				Host = 1,
				Assembly = 2
			}
		}
	}
}
