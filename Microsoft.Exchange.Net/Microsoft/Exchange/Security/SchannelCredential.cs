using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Security
{
	internal struct SchannelCredential
	{
		public SchannelCredential(int version, X509Certificate certificate, SchannelCredential.Flags flags, SchannelProtocols protocols)
		{
			this.rootStore = (this.phMappers = (this.palgSupportedAlgs = (this.certContextArray = IntPtr.Zero)));
			this.cCreds = (this.cMappers = (this.cSupportedAlgs = 0));
			this.dwMinimumCipherStrength = (this.dwMaximumCipherStrength = 0);
			this.dwSessionLifespan = (this.reserved = 0);
			this.version = version;
			this.dwFlags = flags;
			this.grbitEnabledProtocols = protocols;
			if (certificate != null)
			{
				this.certContextArray = certificate.Handle;
				this.cCreds = 1;
			}
		}

		public const int CurrentVersion = 4;

		public int version;

		public int cCreds;

		public IntPtr certContextArray;

		private readonly IntPtr rootStore;

		public int cMappers;

		private readonly IntPtr phMappers;

		public int cSupportedAlgs;

		private readonly IntPtr palgSupportedAlgs;

		public SchannelProtocols grbitEnabledProtocols;

		public int dwMinimumCipherStrength;

		public int dwMaximumCipherStrength;

		public int dwSessionLifespan;

		public SchannelCredential.Flags dwFlags;

		public int reserved;

		[Flags]
		public enum Flags
		{
			Zero = 0,
			NoSystemMapper = 2,
			NoNameCheck = 4,
			ValidateManual = 8,
			NoDefaultCred = 16,
			ValidateAuto = 32,
			UseDefaultCreds = 64,
			DisableReonnects = 128,
			RevocationCheckEndCert = 256,
			RevocationCheckChain = 512,
			RevocationCheckChainExcludeRoot = 1024,
			IgnoreNoRevocationCheck = 2048,
			IgnoreRevocationOffline = 4096,
			CacheOnlyUrlRetrievalOnCreate = 131072,
			SendRootCert = 262144,
			SendAuxRecord = 2097152,
			UseStrongCrypto = 4194304
		}
	}
}
