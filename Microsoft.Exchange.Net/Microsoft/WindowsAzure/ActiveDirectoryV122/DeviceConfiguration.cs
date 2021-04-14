using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("objectId")]
	public class DeviceConfiguration : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static DeviceConfiguration CreateDeviceConfiguration(string objectId, Collection<byte[]> publicIssuerCertificates, Collection<byte[]> cloudPublicIssuerCertificates)
		{
			DeviceConfiguration deviceConfiguration = new DeviceConfiguration();
			deviceConfiguration.objectId = objectId;
			if (publicIssuerCertificates == null)
			{
				throw new ArgumentNullException("publicIssuerCertificates");
			}
			deviceConfiguration.publicIssuerCertificates = publicIssuerCertificates;
			if (cloudPublicIssuerCertificates == null)
			{
				throw new ArgumentNullException("cloudPublicIssuerCertificates");
			}
			deviceConfiguration.cloudPublicIssuerCertificates = cloudPublicIssuerCertificates;
			return deviceConfiguration;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<byte[]> publicIssuerCertificates
		{
			get
			{
				return this._publicIssuerCertificates;
			}
			set
			{
				this._publicIssuerCertificates = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<byte[]> cloudPublicIssuerCertificates
		{
			get
			{
				return this._cloudPublicIssuerCertificates;
			}
			set
			{
				this._cloudPublicIssuerCertificates = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? registrationQuota
		{
			get
			{
				return this._registrationQuota;
			}
			set
			{
				this._registrationQuota = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? maximumRegistrationInactivityPeriod
		{
			get
			{
				return this._maximumRegistrationInactivityPeriod;
			}
			set
			{
				this._maximumRegistrationInactivityPeriod = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<byte[]> _publicIssuerCertificates = new Collection<byte[]>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<byte[]> _cloudPublicIssuerCertificates = new Collection<byte[]>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _registrationQuota;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _maximumRegistrationInactivityPeriod;
	}
}
