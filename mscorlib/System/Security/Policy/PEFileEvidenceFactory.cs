using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Policy
{
	internal sealed class PEFileEvidenceFactory : IRuntimeEvidenceFactory
	{
		[SecurityCritical]
		private PEFileEvidenceFactory(SafePEFileHandle peFile)
		{
			this.m_peFile = peFile;
		}

		internal SafePEFileHandle PEFile
		{
			[SecurityCritical]
			get
			{
				return this.m_peFile;
			}
		}

		public IEvidenceFactory Target
		{
			get
			{
				return null;
			}
		}

		[SecurityCritical]
		private static Evidence CreateSecurityIdentity(SafePEFileHandle peFile, Evidence hostProvidedEvidence)
		{
			PEFileEvidenceFactory target = new PEFileEvidenceFactory(peFile);
			Evidence evidence = new Evidence(target);
			if (hostProvidedEvidence != null)
			{
				evidence.MergeWithNoDuplicates(hostProvidedEvidence);
			}
			return evidence;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void FireEvidenceGeneratedEvent(SafePEFileHandle peFile, EvidenceTypeGenerated type);

		[SecuritySafeCritical]
		internal void FireEvidenceGeneratedEvent(EvidenceTypeGenerated type)
		{
			PEFileEvidenceFactory.FireEvidenceGeneratedEvent(this.m_peFile, type);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetAssemblySuppliedEvidence(SafePEFileHandle peFile, ObjectHandleOnStack retSerializedEvidence);

		[SecuritySafeCritical]
		public IEnumerable<EvidenceBase> GetFactorySuppliedEvidence()
		{
			if (this.m_assemblyProvidedEvidence == null)
			{
				byte[] array = null;
				PEFileEvidenceFactory.GetAssemblySuppliedEvidence(this.m_peFile, JitHelpers.GetObjectHandleOnStack<byte[]>(ref array));
				this.m_assemblyProvidedEvidence = new List<EvidenceBase>();
				if (array != null)
				{
					Evidence evidence = new Evidence();
					new SecurityPermission(SecurityPermissionFlag.SerializationFormatter).Assert();
					try
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						using (MemoryStream memoryStream = new MemoryStream(array))
						{
							evidence = (Evidence)binaryFormatter.Deserialize(memoryStream);
						}
					}
					catch
					{
					}
					CodeAccessPermission.RevertAssert();
					if (evidence != null)
					{
						IEnumerator assemblyEnumerator = evidence.GetAssemblyEnumerator();
						while (assemblyEnumerator.MoveNext())
						{
							if (assemblyEnumerator.Current != null)
							{
								EvidenceBase evidenceBase = assemblyEnumerator.Current as EvidenceBase;
								if (evidenceBase == null)
								{
									evidenceBase = new LegacyEvidenceWrapper(assemblyEnumerator.Current);
								}
								this.m_assemblyProvidedEvidence.Add(evidenceBase);
							}
						}
					}
				}
			}
			return this.m_assemblyProvidedEvidence;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetLocationEvidence(SafePEFileHandle peFile, out SecurityZone zone, StringHandleOnStack retUrl);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetPublisherCertificate(SafePEFileHandle peFile, ObjectHandleOnStack retCertificate);

		public EvidenceBase GenerateEvidence(Type evidenceType)
		{
			if (evidenceType == typeof(Site))
			{
				return this.GenerateSiteEvidence();
			}
			if (evidenceType == typeof(Url))
			{
				return this.GenerateUrlEvidence();
			}
			if (evidenceType == typeof(Zone))
			{
				return this.GenerateZoneEvidence();
			}
			if (evidenceType == typeof(Publisher))
			{
				return this.GeneratePublisherEvidence();
			}
			return null;
		}

		[SecuritySafeCritical]
		private void GenerateLocationEvidence()
		{
			if (!this.m_generatedLocationEvidence)
			{
				SecurityZone securityZone = SecurityZone.NoZone;
				string text = null;
				PEFileEvidenceFactory.GetLocationEvidence(this.m_peFile, out securityZone, JitHelpers.GetStringHandleOnStack(ref text));
				if (securityZone != SecurityZone.NoZone)
				{
					this.m_zoneEvidence = new Zone(securityZone);
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.m_urlEvidence = new Url(text, true);
					if (!text.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
					{
						this.m_siteEvidence = Site.CreateFromUrl(text);
					}
				}
				this.m_generatedLocationEvidence = true;
			}
		}

		[SecuritySafeCritical]
		private Publisher GeneratePublisherEvidence()
		{
			byte[] array = null;
			PEFileEvidenceFactory.GetPublisherCertificate(this.m_peFile, JitHelpers.GetObjectHandleOnStack<byte[]>(ref array));
			if (array == null)
			{
				return null;
			}
			return new Publisher(new X509Certificate(array));
		}

		private Site GenerateSiteEvidence()
		{
			if (this.m_siteEvidence == null)
			{
				this.GenerateLocationEvidence();
			}
			return this.m_siteEvidence;
		}

		private Url GenerateUrlEvidence()
		{
			if (this.m_urlEvidence == null)
			{
				this.GenerateLocationEvidence();
			}
			return this.m_urlEvidence;
		}

		private Zone GenerateZoneEvidence()
		{
			if (this.m_zoneEvidence == null)
			{
				this.GenerateLocationEvidence();
			}
			return this.m_zoneEvidence;
		}

		[SecurityCritical]
		private SafePEFileHandle m_peFile;

		private List<EvidenceBase> m_assemblyProvidedEvidence;

		private bool m_generatedLocationEvidence;

		private Site m_siteEvidence;

		private Url m_urlEvidence;

		private Zone m_zoneEvidence;
	}
}
