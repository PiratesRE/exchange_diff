using System;
using System.ServiceModel;
using System.Xml;

namespace Microsoft.Exchange.Management.RightsManagement
{
	internal static class RmsOnlineConstants
	{
		public static TimeSpan SendTimeout
		{
			get
			{
				return TimeSpan.FromMinutes(1.0);
			}
		}

		public static TimeSpan ReceiveTimeout
		{
			get
			{
				return TimeSpan.FromMinutes(1.0);
			}
		}

		public static long MaxReceivedMessageSize
		{
			get
			{
				return 2147483647L;
			}
		}

		public static string BindingName
		{
			get
			{
				return "Microsoft.RightsManagementServices.Online.TenantManagementService";
			}
		}

		public static XmlDictionaryReaderQuotas ReaderQuotas
		{
			get
			{
				return new XmlDictionaryReaderQuotas
				{
					MaxStringContentLength = 70254592
				};
			}
		}

		public static WSHttpSecurity Security
		{
			get
			{
				return new WSHttpSecurity
				{
					Mode = SecurityMode.Transport,
					Transport = 
					{
						ClientCredentialType = HttpClientCredentialType.Certificate
					}
				};
			}
		}

		public static string AuthenticationCertificateSubjectDistinguishedName
		{
			get
			{
				return "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
			}
		}
	}
}
