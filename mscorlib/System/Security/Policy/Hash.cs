using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Util;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Hash : EvidenceBase, ISerializable
	{
		[SecurityCritical]
		internal Hash(SerializationInfo info, StreamingContext context)
		{
			Dictionary<Type, byte[]> dictionary = info.GetValueNoThrow("Hashes", typeof(Dictionary<Type, byte[]>)) as Dictionary<Type, byte[]>;
			if (dictionary != null)
			{
				this.m_hashes = dictionary;
				return;
			}
			this.m_hashes = new Dictionary<Type, byte[]>();
			byte[] array = info.GetValueNoThrow("Md5", typeof(byte[])) as byte[];
			if (array != null)
			{
				this.m_hashes[typeof(MD5)] = array;
			}
			byte[] array2 = info.GetValueNoThrow("Sha1", typeof(byte[])) as byte[];
			if (array2 != null)
			{
				this.m_hashes[typeof(SHA1)] = array2;
			}
			byte[] array3 = info.GetValueNoThrow("RawData", typeof(byte[])) as byte[];
			if (array3 != null)
			{
				this.GenerateDefaultHashes(array3);
			}
		}

		public Hash(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly.IsDynamic)
			{
				throw new ArgumentException(Environment.GetResourceString("Security_CannotGenerateHash"), "assembly");
			}
			this.m_hashes = new Dictionary<Type, byte[]>();
			this.m_assembly = (assembly as RuntimeAssembly);
			if (this.m_assembly == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"), "assembly");
			}
		}

		private Hash(Hash hash)
		{
			this.m_assembly = hash.m_assembly;
			this.m_rawData = hash.m_rawData;
			this.m_hashes = new Dictionary<Type, byte[]>(hash.m_hashes);
		}

		private Hash(Type hashType, byte[] hashValue)
		{
			this.m_hashes = new Dictionary<Type, byte[]>();
			byte[] array = new byte[hashValue.Length];
			Array.Copy(hashValue, array, array.Length);
			this.m_hashes[hashType] = hashValue;
		}

		public static Hash CreateSHA1(byte[] sha1)
		{
			if (sha1 == null)
			{
				throw new ArgumentNullException("sha1");
			}
			return new Hash(typeof(SHA1), sha1);
		}

		public static Hash CreateSHA256(byte[] sha256)
		{
			if (sha256 == null)
			{
				throw new ArgumentNullException("sha256");
			}
			return new Hash(typeof(SHA256), sha256);
		}

		public static Hash CreateMD5(byte[] md5)
		{
			if (md5 == null)
			{
				throw new ArgumentNullException("md5");
			}
			return new Hash(typeof(MD5), md5);
		}

		public override EvidenceBase Clone()
		{
			return new Hash(this);
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.GenerateDefaultHashes();
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GenerateDefaultHashes();
			byte[] value;
			if (this.m_hashes.TryGetValue(typeof(MD5), out value))
			{
				info.AddValue("Md5", value);
			}
			byte[] value2;
			if (this.m_hashes.TryGetValue(typeof(SHA1), out value2))
			{
				info.AddValue("Sha1", value2);
			}
			info.AddValue("RawData", null);
			info.AddValue("PEFile", IntPtr.Zero);
			info.AddValue("Hashes", this.m_hashes);
		}

		public byte[] SHA1
		{
			get
			{
				byte[] array = null;
				if (!this.m_hashes.TryGetValue(typeof(SHA1), out array))
				{
					array = this.GenerateHash(Hash.GetDefaultHashImplementationOrFallback(typeof(SHA1), typeof(SHA1)));
				}
				byte[] array2 = new byte[array.Length];
				Array.Copy(array, array2, array2.Length);
				return array2;
			}
		}

		public byte[] SHA256
		{
			get
			{
				byte[] array = null;
				if (!this.m_hashes.TryGetValue(typeof(SHA256), out array))
				{
					array = this.GenerateHash(Hash.GetDefaultHashImplementationOrFallback(typeof(SHA256), typeof(SHA256)));
				}
				byte[] array2 = new byte[array.Length];
				Array.Copy(array, array2, array2.Length);
				return array2;
			}
		}

		public byte[] MD5
		{
			get
			{
				byte[] array = null;
				if (!this.m_hashes.TryGetValue(typeof(MD5), out array))
				{
					array = this.GenerateHash(Hash.GetDefaultHashImplementationOrFallback(typeof(MD5), typeof(MD5)));
				}
				byte[] array2 = new byte[array.Length];
				Array.Copy(array, array2, array2.Length);
				return array2;
			}
		}

		public byte[] GenerateHash(HashAlgorithm hashAlg)
		{
			if (hashAlg == null)
			{
				throw new ArgumentNullException("hashAlg");
			}
			byte[] array = this.GenerateHash(hashAlg.GetType());
			byte[] array2 = new byte[array.Length];
			Array.Copy(array, array2, array2.Length);
			return array2;
		}

		private byte[] GenerateHash(Type hashType)
		{
			Type hashIndexType = Hash.GetHashIndexType(hashType);
			byte[] array = null;
			if (!this.m_hashes.TryGetValue(hashIndexType, out array))
			{
				if (this.m_assembly == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Security_CannotGenerateHash"));
				}
				array = Hash.GenerateHash(hashType, this.GetRawData());
				this.m_hashes[hashIndexType] = array;
			}
			return array;
		}

		private static byte[] GenerateHash(Type hashType, byte[] assemblyBytes)
		{
			byte[] result;
			using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashType.FullName))
			{
				result = hashAlgorithm.ComputeHash(assemblyBytes);
			}
			return result;
		}

		private void GenerateDefaultHashes()
		{
			if (this.m_assembly != null)
			{
				this.GenerateDefaultHashes(this.GetRawData());
			}
		}

		private void GenerateDefaultHashes(byte[] assemblyBytes)
		{
			Type[] array = new Type[]
			{
				Hash.GetHashIndexType(typeof(SHA1)),
				Hash.GetHashIndexType(typeof(SHA256)),
				Hash.GetHashIndexType(typeof(MD5))
			};
			foreach (Type type in array)
			{
				Type defaultHashImplementation = Hash.GetDefaultHashImplementation(type);
				if (defaultHashImplementation != null && !this.m_hashes.ContainsKey(type))
				{
					this.m_hashes[type] = Hash.GenerateHash(defaultHashImplementation, assemblyBytes);
				}
			}
		}

		private static Type GetDefaultHashImplementationOrFallback(Type hashAlgorithm, Type fallbackImplementation)
		{
			Type defaultHashImplementation = Hash.GetDefaultHashImplementation(hashAlgorithm);
			if (!(defaultHashImplementation != null))
			{
				return fallbackImplementation;
			}
			return defaultHashImplementation;
		}

		private static Type GetDefaultHashImplementation(Type hashAlgorithm)
		{
			if (hashAlgorithm.IsAssignableFrom(typeof(MD5)))
			{
				if (!CryptoConfig.AllowOnlyFipsAlgorithms)
				{
					return typeof(MD5CryptoServiceProvider);
				}
				return null;
			}
			else
			{
				if (hashAlgorithm.IsAssignableFrom(typeof(SHA256)))
				{
					return Type.GetType("System.Security.Cryptography.SHA256CryptoServiceProvider, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
				}
				return hashAlgorithm;
			}
		}

		private static Type GetHashIndexType(Type hashType)
		{
			Type type = hashType;
			while (type != null && type.BaseType != typeof(HashAlgorithm))
			{
				type = type.BaseType;
			}
			if (type == null)
			{
				type = typeof(HashAlgorithm);
			}
			return type;
		}

		private byte[] GetRawData()
		{
			byte[] array = null;
			if (this.m_assembly != null)
			{
				if (this.m_rawData != null)
				{
					array = (this.m_rawData.Target as byte[]);
				}
				if (array == null)
				{
					array = this.m_assembly.GetRawBytes();
					this.m_rawData = new WeakReference(array);
				}
			}
			return array;
		}

		private SecurityElement ToXml()
		{
			this.GenerateDefaultHashes();
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Hash");
			securityElement.AddAttribute("version", "2");
			foreach (KeyValuePair<Type, byte[]> keyValuePair in this.m_hashes)
			{
				SecurityElement securityElement2 = new SecurityElement("hash");
				securityElement2.AddAttribute("algorithm", keyValuePair.Key.Name);
				securityElement2.AddAttribute("value", Hex.EncodeHexString(keyValuePair.Value));
				securityElement.AddChild(securityElement2);
			}
			return securityElement;
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		private RuntimeAssembly m_assembly;

		private Dictionary<Type, byte[]> m_hashes;

		private WeakReference m_rawData;
	}
}
