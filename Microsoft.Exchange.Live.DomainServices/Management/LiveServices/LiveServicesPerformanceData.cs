using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Management.LiveServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class LiveServicesPerformanceData
	{
		public static PerformanceDataProvider SPFConnection
		{
			get
			{
				if (LiveServicesPerformanceData.spfConnection == null)
				{
					LiveServicesPerformanceData.spfConnection = new PerformanceDataProvider("SPF Connection");
				}
				return LiveServicesPerformanceData.spfConnection;
			}
		}

		public static PerformanceDataProvider SPFCall
		{
			get
			{
				if (LiveServicesPerformanceData.spfCall == null)
				{
					LiveServicesPerformanceData.spfCall = new PerformanceDataProvider("SPF Call");
				}
				return LiveServicesPerformanceData.spfCall;
			}
		}

		public static PerformanceDataProvider CredentialServicesCall
		{
			get
			{
				if (LiveServicesPerformanceData.credentialServicesCall == null)
				{
					LiveServicesPerformanceData.credentialServicesCall = new PerformanceDataProvider("CredentialServices Call");
				}
				return LiveServicesPerformanceData.credentialServicesCall;
			}
		}

		public static PerformanceDataProvider ProfileServicesCall
		{
			get
			{
				if (LiveServicesPerformanceData.profileServicesCall == null)
				{
					LiveServicesPerformanceData.profileServicesCall = new PerformanceDataProvider("ProfileServices Call");
				}
				return LiveServicesPerformanceData.profileServicesCall;
			}
		}

		public static PerformanceDataProvider NamespaceServicesCall
		{
			get
			{
				if (LiveServicesPerformanceData.namespaceServicesCall == null)
				{
					LiveServicesPerformanceData.namespaceServicesCall = new PerformanceDataProvider("NamespaceServices Call");
				}
				return LiveServicesPerformanceData.namespaceServicesCall;
			}
		}

		public static IDisposable StartSPFConnectionRequest()
		{
			return LiveServicesPerformanceData.SPFConnection.StartRequestTimer();
		}

		public static IDisposable StartSPFCallRequest()
		{
			return LiveServicesPerformanceData.SPFCall.StartRequestTimer();
		}

		public static IDisposable StartCredentialServicesCallRequest()
		{
			return LiveServicesPerformanceData.CredentialServicesCall.StartRequestTimer();
		}

		public static IDisposable StartNamespaceServicesCallRequest()
		{
			return LiveServicesPerformanceData.NamespaceServicesCall.StartRequestTimer();
		}

		public static IDisposable StartProfileServiceCallRequest()
		{
			return LiveServicesPerformanceData.ProfileServicesCall.StartRequestTimer();
		}

		[ThreadStatic]
		private static PerformanceDataProvider spfConnection;

		[ThreadStatic]
		private static PerformanceDataProvider spfCall;

		[ThreadStatic]
		private static PerformanceDataProvider credentialServicesCall;

		[ThreadStatic]
		private static PerformanceDataProvider profileServicesCall;

		[ThreadStatic]
		private static PerformanceDataProvider namespaceServicesCall;
	}
}
