using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class StrongName : EvidenceBase, IIdentityPermissionFactory, IDelayEvaluatedEvidence
	{
		internal StrongName()
		{
		}

		public StrongName(StrongNamePublicKeyBlob blob, string name, Version version) : this(blob, name, version, null)
		{
		}

		internal StrongName(StrongNamePublicKeyBlob blob, string name, Version version, Assembly assembly)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyStrongName"));
			}
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			RuntimeAssembly runtimeAssembly = assembly as RuntimeAssembly;
			if (assembly != null && runtimeAssembly == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"), "assembly");
			}
			this.m_publicKeyBlob = blob;
			this.m_name = name;
			this.m_version = version;
			this.m_assembly = runtimeAssembly;
		}

		public StrongNamePublicKeyBlob PublicKey
		{
			get
			{
				return this.m_publicKeyBlob;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public Version Version
		{
			get
			{
				return this.m_version;
			}
		}

		bool IDelayEvaluatedEvidence.IsVerified
		{
			[SecurityCritical]
			get
			{
				return !(this.m_assembly != null) || this.m_assembly.IsStrongNameVerified;
			}
		}

		bool IDelayEvaluatedEvidence.WasUsed
		{
			get
			{
				return this.m_wasUsed;
			}
		}

		void IDelayEvaluatedEvidence.MarkUsed()
		{
			this.m_wasUsed = true;
		}

		internal static bool CompareNames(string asmName, string mcName)
		{
			if (mcName.Length > 0 && mcName[mcName.Length - 1] == '*' && mcName.Length - 1 <= asmName.Length)
			{
				return string.Compare(mcName, 0, asmName, 0, mcName.Length - 1, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return string.Compare(mcName, asmName, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new StrongNameIdentityPermission(this.m_publicKeyBlob, this.m_name, this.m_version);
		}

		public override EvidenceBase Clone()
		{
			return new StrongName(this.m_publicKeyBlob, this.m_name, this.m_version);
		}

		public object Copy()
		{
			return this.Clone();
		}

		internal SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("StrongName");
			securityElement.AddAttribute("version", "1");
			if (this.m_publicKeyBlob != null)
			{
				securityElement.AddAttribute("Key", Hex.EncodeHexString(this.m_publicKeyBlob.PublicKey));
			}
			if (this.m_name != null)
			{
				securityElement.AddAttribute("Name", this.m_name);
			}
			if (this.m_version != null)
			{
				securityElement.AddAttribute("Version", this.m_version.ToString());
			}
			return securityElement;
		}

		internal void FromXml(SecurityElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (string.Compare(element.Tag, "StrongName", StringComparison.Ordinal) != 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidXML"));
			}
			this.m_publicKeyBlob = null;
			this.m_version = null;
			string text = element.Attribute("Key");
			if (text != null)
			{
				this.m_publicKeyBlob = new StrongNamePublicKeyBlob(Hex.DecodeHexString(text));
			}
			this.m_name = element.Attribute("Name");
			string text2 = element.Attribute("Version");
			if (text2 != null)
			{
				this.m_version = new Version(text2);
			}
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		public override bool Equals(object o)
		{
			StrongName strongName = o as StrongName;
			return strongName != null && object.Equals(this.m_publicKeyBlob, strongName.m_publicKeyBlob) && object.Equals(this.m_name, strongName.m_name) && object.Equals(this.m_version, strongName.m_version);
		}

		public override int GetHashCode()
		{
			if (this.m_publicKeyBlob != null)
			{
				return this.m_publicKeyBlob.GetHashCode();
			}
			if (this.m_name != null || this.m_version != null)
			{
				return ((this.m_name == null) ? 0 : this.m_name.GetHashCode()) + ((this.m_version == null) ? 0 : this.m_version.GetHashCode());
			}
			return typeof(StrongName).GetHashCode();
		}

		internal object Normalize()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_publicKeyBlob.PublicKey);
			binaryWriter.Write(this.m_version.Major);
			binaryWriter.Write(this.m_name);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private StrongNamePublicKeyBlob m_publicKeyBlob;

		private string m_name;

		private Version m_version;

		[NonSerialized]
		private RuntimeAssembly m_assembly;

		[NonSerialized]
		private bool m_wasUsed;
	}
}
