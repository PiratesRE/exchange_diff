using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public sealed class AutodiscoverServiceCredentialsElement : ServiceCredentialsElement
	{
		protected override object CreateBehavior()
		{
			object obj = base.CreateBehavior();
			ServiceCredentials serviceCredentials = obj as ServiceCredentials;
			if (serviceCredentials == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. Behavior is not a ServiceCredentials - instead, it is of type {0}", obj.GetType().FullName);
				return obj;
			}
			if (serviceCredentials.IssuedTokenAuthentication == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ServiceCredentials.IssuedTokenAuthentication is null");
				return obj;
			}
			if (serviceCredentials.IssuedTokenAuthentication.KnownCertificates == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ServiceCredentials.IssuedTokenAuthentication.KnownCertificates is null");
				return obj;
			}
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			ApplicationPoolRecycler.EnableOnFederationTrustCertificateChange();
			if (!current.Enabled)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ExternalAuthentication.Enabled is false");
				return obj;
			}
			serviceCredentials.IssuedTokenAuthentication.AllowedAudienceUris.Add(current.TokenValidator.TargetUri.OriginalString);
			foreach (X509Certificate2 x509Certificate in current.Certificates)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<X509Certificate2>((long)this.GetHashCode(), "Adding Exchange certificate to ServiceCredentials: {0}", x509Certificate);
				serviceCredentials.IssuedTokenAuthentication.KnownCertificates.Add(x509Certificate);
			}
			return obj;
		}
	}
}
