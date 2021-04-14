using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[DebuggerStepThrough]
	[MessageContract(WrapperName = "AcquireServerLicenseResponseMessage", WrapperNamespace = "http://microsoft.com/IPC/WSServerLicensingService", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	public class AcquireServerLicenseResponseMessage
	{
		public AcquireServerLicenseResponseMessage()
		{
		}

		public AcquireServerLicenseResponseMessage(VersionData VersionData, BatchLicenseResponses AcquireServerLicenseResponses)
		{
			this.VersionData = VersionData;
			this.AcquireServerLicenseResponses = AcquireServerLicenseResponses;
		}

		[MessageHeader(Namespace = "http://microsoft.com/IPC/WSService")]
		public VersionData VersionData;

		[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSServerLicensingService", Order = 0)]
		public BatchLicenseResponses AcquireServerLicenseResponses;
	}
}
