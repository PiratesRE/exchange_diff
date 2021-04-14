using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	internal static class WcfUtility
	{
		public static Binding CreateIntraServiceBinding(bool enablePortSharing = true)
		{
			return new NetTcpBinding
			{
				Security = 
				{
					Mode = SecurityMode.Transport,
					Transport = 
					{
						ClientCredentialType = TcpClientCredentialType.Windows
					}
				},
				MaxReceivedMessageSize = 524288L,
				MaxBufferPoolSize = 1048576L,
				PortSharingEnabled = enablePortSharing
			};
		}

		public static Binding CreateInterServiceBinding()
		{
			return new WSHttpBinding
			{
				Security = 
				{
					Mode = SecurityMode.Transport,
					Transport = 
					{
						ClientCredentialType = HttpClientCredentialType.Certificate
					}
				},
				MaxReceivedMessageSize = 524288L,
				MaxBufferPoolSize = 1048576L
			};
		}

		public static EndpointAddress GetBackendServerEndpointAddress(string server)
		{
			string uri = string.Format("net.tcp://{0}/complianceservice", server);
			return new EndpointAddress(uri);
		}

		public static EndpointAddress GetInterServiceEndpointAddress(string host)
		{
			string uri = string.Format("https://{0}/complianceservice", host);
			return new EndpointAddress(uri);
		}

		public static string GetThumbprint(string certificateSubject)
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			string thumbprint;
			try
			{
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection source = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateSubject, true);
				X509Certificate2 x509Certificate = (from X509Certificate2 cert in source
				orderby cert.NotAfter descending
				select cert).FirstOrDefault<X509Certificate2>();
				if (x509Certificate == null)
				{
					throw new InvalidOperationException("Unable to load certificate.");
				}
				thumbprint = x509Certificate.Thumbprint;
			}
			finally
			{
				x509Store.Close();
			}
			return thumbprint;
		}

		public static byte[][] GetMessageBlobs(IEnumerable<ComplianceMessage> messages)
		{
			int num = messages.Count<ComplianceMessage>();
			int num2 = 0;
			byte[][] array = new byte[num][];
			foreach (ComplianceMessage inputObject in messages)
			{
				byte[] array2 = ComplianceSerializer.Serialize<ComplianceMessage>(ComplianceMessage.Description, inputObject);
				array[num2] = array2;
				num2++;
			}
			return array;
		}

		public static IEnumerable<ComplianceMessage> GetMessagesFromBlobs(IEnumerable<byte[]> blobs)
		{
			if (blobs != null)
			{
				foreach (byte[] blob in blobs)
				{
					yield return ComplianceSerializer.DeSerialize<ComplianceMessage>(ComplianceMessage.Description, blob);
				}
			}
			yield break;
		}
	}
}
