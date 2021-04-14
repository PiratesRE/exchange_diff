using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class WindowsLiveIdApplicationCertificate : ISerializable
	{
		public WindowsLiveIdApplicationCertificate(string name, bool isCurrent, X509Certificate2 x509Certificate)
		{
			this.Name = name;
			this.IsCurrent = isCurrent;
			this.Certificate = x509Certificate;
		}

		private WindowsLiveIdApplicationCertificate(SerializationInfo info, StreamingContext context)
		{
			byte[] rawData = (byte[])info.GetValue("CertificateData", typeof(byte[]));
			this.Certificate = new X509Certificate2(rawData);
			this.IsCurrent = info.GetBoolean("IsCurrent");
			this.Name = info.GetString("Name");
		}

		public string Name { get; set; }

		public bool IsCurrent { get; set; }

		public X509Certificate2 Certificate { get; set; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", this.Name);
			info.AddValue("IsCurrent", this.IsCurrent);
			info.AddValue("CertificateData", this.Certificate.Export(X509ContentType.SerializedCert));
		}

		internal static X509Certificate2 CertificateFromBase64(string base64Certificate)
		{
			X509Certificate2 result = null;
			if (!string.IsNullOrEmpty(base64Certificate))
			{
				byte[] array = Convert.FromBase64String(base64Certificate);
				if (array != null)
				{
					result = new X509Certificate2(array);
				}
			}
			return result;
		}

		private const string SerializedNameValueName = "Name";

		private const string SerializedIsCurrentValueName = "IsCurrent";

		private const string SerializedCertificateDataValueName = "CertificateData";
	}
}
