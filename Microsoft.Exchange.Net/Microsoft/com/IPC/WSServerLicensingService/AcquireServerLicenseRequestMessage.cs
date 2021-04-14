using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[MessageContract(WrapperName = "AcquireServerLicenseRequestMessage", WrapperNamespace = "http://microsoft.com/IPC/WSServerLicensingService", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	[DebuggerStepThrough]
	public class AcquireServerLicenseRequestMessage
	{
		public AcquireServerLicenseRequestMessage()
		{
		}

		public AcquireServerLicenseRequestMessage(VersionData VersionData, XrmlCertificateChain GroupIdentityCredential, XrmlCertificateChain IssuanceLicense, LicenseeIdentity[] LicenseeIdentities)
		{
			this.VersionData = VersionData;
			this.GroupIdentityCredential = GroupIdentityCredential;
			this.IssuanceLicense = IssuanceLicense;
			this.LicenseeIdentities = LicenseeIdentities;
		}

		[MessageHeader(Namespace = "http://microsoft.com/IPC/WSService")]
		public VersionData VersionData;

		[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSServerLicensingService", Order = 0)]
		public XrmlCertificateChain GroupIdentityCredential;

		[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSServerLicensingService", Order = 1)]
		public XrmlCertificateChain IssuanceLicense;

		[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSServerLicensingService", Order = 2)]
		public LicenseeIdentity[] LicenseeIdentities;
	}
}
