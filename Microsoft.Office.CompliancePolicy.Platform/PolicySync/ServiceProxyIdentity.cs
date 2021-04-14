using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class ServiceProxyIdentity : IEquatable<ServiceProxyIdentity>
	{
		public ServiceProxyIdentity(EndpointAddress endpointAddress, X509Certificate2 certificate, string partnerName)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			this.EndpointAddress = endpointAddress;
			this.Certificate = certificate;
			this.PartnerName = partnerName;
			this.useICredentials = false;
		}

		public ServiceProxyIdentity(EndpointAddress endpointAddress, ICredentials credentials, string partnerName)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			this.EndpointAddress = endpointAddress;
			this.Credentials = credentials;
			this.PartnerName = partnerName;
			this.useICredentials = true;
		}

		public EndpointAddress EndpointAddress { get; private set; }

		public X509Certificate2 Certificate { get; private set; }

		public ICredentials Credentials { get; private set; }

		public string PartnerName { get; private set; }

		public override bool Equals(object other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (!(base.GetType() != other.GetType()) && this.Equals(other as ServiceProxyIdentity)));
		}

		public bool Equals(ServiceProxyIdentity other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (base.GetType() != other.GetType())
			{
				return false;
			}
			if (this.EndpointAddress == null || other.EndpointAddress == null)
			{
				return false;
			}
			if (this.useICredentials != other.useICredentials)
			{
				return false;
			}
			if (this.useICredentials)
			{
				return this.EndpointAddress.Equals(other.EndpointAddress) && this.Credentials.Equals(other.Credentials) && this.PartnerName.Equals(other.PartnerName);
			}
			return this.EndpointAddress.Equals(other.EndpointAddress) && this.Certificate.Equals(other.Certificate) && this.PartnerName.Equals(other.PartnerName);
		}

		public override int GetHashCode()
		{
			if (this.useICredentials)
			{
				return this.EndpointAddress.GetHashCode() ^ this.Credentials.GetHashCode() ^ this.PartnerName.GetHashCode();
			}
			return this.EndpointAddress.GetHashCode() ^ this.Certificate.GetHashCode() ^ this.PartnerName.GetHashCode();
		}

		private readonly bool useICredentials;
	}
}
