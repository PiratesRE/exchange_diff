using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	[DebuggerStepThrough]
	public class WSServerLicensingServiceClient : ClientBase<IWSServerLicensingService>, IWSServerLicensingService
	{
		public WSServerLicensingServiceClient()
		{
		}

		public WSServerLicensingServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public WSServerLicensingServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public WSServerLicensingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public WSServerLicensingServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<AcquireServerLicenseCompletedEventArgs> AcquireServerLicenseCompleted;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		AcquireServerLicenseResponseMessage IWSServerLicensingService.AcquireServerLicense(AcquireServerLicenseRequestMessage request)
		{
			return base.Channel.AcquireServerLicense(request);
		}

		public BatchLicenseResponses AcquireServerLicense(ref VersionData VersionData, XrmlCertificateChain GroupIdentityCredential, XrmlCertificateChain IssuanceLicense, LicenseeIdentity[] LicenseeIdentities)
		{
			AcquireServerLicenseResponseMessage acquireServerLicenseResponseMessage = ((IWSServerLicensingService)this).AcquireServerLicense(new AcquireServerLicenseRequestMessage
			{
				VersionData = VersionData,
				GroupIdentityCredential = GroupIdentityCredential,
				IssuanceLicense = IssuanceLicense,
				LicenseeIdentities = LicenseeIdentities
			});
			VersionData = acquireServerLicenseResponseMessage.VersionData;
			return acquireServerLicenseResponseMessage.AcquireServerLicenseResponses;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IWSServerLicensingService.BeginAcquireServerLicense(AcquireServerLicenseRequestMessage request, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginAcquireServerLicense(request, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginAcquireServerLicense(VersionData VersionData, XrmlCertificateChain GroupIdentityCredential, XrmlCertificateChain IssuanceLicense, LicenseeIdentity[] LicenseeIdentities, AsyncCallback callback, object asyncState)
		{
			return ((IWSServerLicensingService)this).BeginAcquireServerLicense(new AcquireServerLicenseRequestMessage
			{
				VersionData = VersionData,
				GroupIdentityCredential = GroupIdentityCredential,
				IssuanceLicense = IssuanceLicense,
				LicenseeIdentities = LicenseeIdentities
			}, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		AcquireServerLicenseResponseMessage IWSServerLicensingService.EndAcquireServerLicense(IAsyncResult result)
		{
			return base.Channel.EndAcquireServerLicense(result);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public VersionData EndAcquireServerLicense(IAsyncResult result, out BatchLicenseResponses AcquireServerLicenseResponses)
		{
			AcquireServerLicenseResponseMessage acquireServerLicenseResponseMessage = ((IWSServerLicensingService)this).EndAcquireServerLicense(result);
			AcquireServerLicenseResponses = acquireServerLicenseResponseMessage.AcquireServerLicenseResponses;
			return acquireServerLicenseResponseMessage.VersionData;
		}

		private IAsyncResult OnBeginAcquireServerLicense(object[] inValues, AsyncCallback callback, object asyncState)
		{
			VersionData versionData = (VersionData)inValues[0];
			XrmlCertificateChain groupIdentityCredential = (XrmlCertificateChain)inValues[1];
			XrmlCertificateChain issuanceLicense = (XrmlCertificateChain)inValues[2];
			LicenseeIdentity[] licenseeIdentities = (LicenseeIdentity[])inValues[3];
			return this.BeginAcquireServerLicense(versionData, groupIdentityCredential, issuanceLicense, licenseeIdentities, callback, asyncState);
		}

		private object[] OnEndAcquireServerLicense(IAsyncResult result)
		{
			BatchLicenseResponses defaultValueForInitialization = base.GetDefaultValueForInitialization<BatchLicenseResponses>();
			VersionData versionData = this.EndAcquireServerLicense(result, out defaultValueForInitialization);
			return new object[]
			{
				defaultValueForInitialization,
				versionData
			};
		}

		private void OnAcquireServerLicenseCompleted(object state)
		{
			if (this.AcquireServerLicenseCompleted != null)
			{
				ClientBase<IWSServerLicensingService>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IWSServerLicensingService>.InvokeAsyncCompletedEventArgs)state;
				this.AcquireServerLicenseCompleted(this, new AcquireServerLicenseCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void AcquireServerLicenseAsync(VersionData VersionData, XrmlCertificateChain GroupIdentityCredential, XrmlCertificateChain IssuanceLicense, LicenseeIdentity[] LicenseeIdentities)
		{
			this.AcquireServerLicenseAsync(VersionData, GroupIdentityCredential, IssuanceLicense, LicenseeIdentities, null);
		}

		public void AcquireServerLicenseAsync(VersionData VersionData, XrmlCertificateChain GroupIdentityCredential, XrmlCertificateChain IssuanceLicense, LicenseeIdentity[] LicenseeIdentities, object userState)
		{
			if (this.onBeginAcquireServerLicenseDelegate == null)
			{
				this.onBeginAcquireServerLicenseDelegate = new ClientBase<IWSServerLicensingService>.BeginOperationDelegate(this.OnBeginAcquireServerLicense);
			}
			if (this.onEndAcquireServerLicenseDelegate == null)
			{
				this.onEndAcquireServerLicenseDelegate = new ClientBase<IWSServerLicensingService>.EndOperationDelegate(this.OnEndAcquireServerLicense);
			}
			if (this.onAcquireServerLicenseCompletedDelegate == null)
			{
				this.onAcquireServerLicenseCompletedDelegate = new SendOrPostCallback(this.OnAcquireServerLicenseCompleted);
			}
			base.InvokeAsync(this.onBeginAcquireServerLicenseDelegate, new object[]
			{
				VersionData,
				GroupIdentityCredential,
				IssuanceLicense,
				LicenseeIdentities
			}, this.onEndAcquireServerLicenseDelegate, this.onAcquireServerLicenseCompletedDelegate, userState);
		}

		private ClientBase<IWSServerLicensingService>.BeginOperationDelegate onBeginAcquireServerLicenseDelegate;

		private ClientBase<IWSServerLicensingService>.EndOperationDelegate onEndAcquireServerLicenseDelegate;

		private SendOrPostCallback onAcquireServerLicenseCompletedDelegate;
	}
}
