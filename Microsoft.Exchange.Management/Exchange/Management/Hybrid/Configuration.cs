using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal static class Configuration
	{
		public static string OnPremGetOrgRel
		{
			get
			{
				return Configuration.GetValue<string>("OnPremGetOrgRel", "On Premises to Exchange Online Organization Relationship");
			}
		}

		public static string TenantGetOrgRel
		{
			get
			{
				return Configuration.GetValue<string>("TenantGetOrgRel", "Exchange Online to on premises Organization Relationship");
			}
		}

		public static string InboundConnectorName(string orgConfigGuid)
		{
			if (string.IsNullOrEmpty(orgConfigGuid))
			{
				throw new ArgumentNullException();
			}
			return Configuration.GetValue<string>("InboundConnectorName", string.Format("Inbound from {0}", orgConfigGuid));
		}

		public static string OutboundConnectorName(string orgConfigGuid)
		{
			if (string.IsNullOrEmpty(orgConfigGuid))
			{
				throw new ArgumentNullException();
			}
			return Configuration.GetValue<string>("OutboundConnectorName", string.Format("Outbound to {0}", orgConfigGuid));
		}

		public static bool EnableLogging
		{
			get
			{
				bool flag = true;
				return Configuration.GetValue<int>("EnableLogging", flag ? 1 : 0) != 0;
			}
		}

		public static bool DisableCertificateChecks
		{
			get
			{
				bool flag = false;
				return Configuration.GetValue<int>("DisableCertificateChecks", flag ? 1 : 0) != 0;
			}
		}

		public static string FederatedTrustIdentity
		{
			get
			{
				return Configuration.GetValue<string>("FederatedTrustIdentity", "Microsoft Federation Gateway");
			}
		}

		public static List<Uri> OnPremiseAcceptedTokenIssuerUriList
		{
			get
			{
				string[] defaultValue = new string[]
				{
					"uri:WindowsLiveIDINT",
					"urn:federation:MicrosoftOnlineINT",
					"urn:federation:MicrosoftOnline"
				};
				List<Uri> result;
				try
				{
					result = (from x in Configuration.GetValue<string[]>("OnPremiseAcceptedTokenIssuerUriList", defaultValue)
					select new Uri(x)).ToList<Uri>();
				}
				catch (UriFormatException)
				{
					throw new LocalizedException(HybridStrings.ErrorHybridRegistryInvalidUri("OnPremiseAcceptedTokenIssuerUriList"));
				}
				return result;
			}
		}

		public static List<Uri> TenantAcceptedTokenIssuerUriList
		{
			get
			{
				string[] defaultValue = new string[]
				{
					"urn:federation:MicrosoftOnlineINT",
					"urn:federation:MicrosoftOnline"
				};
				List<Uri> result;
				try
				{
					result = (from x in Configuration.GetValue<string[]>("TenantAcceptedTokenIssuerUriList", defaultValue)
					select new Uri(x)).ToList<Uri>();
				}
				catch (UriFormatException)
				{
					throw new LocalizedException(HybridStrings.ErrorHybridRegistryInvalidUri("TenantAcceptedTokenIssuerUriList"));
				}
				return result;
			}
		}

		public static string PowerShellEndpoint(int instance)
		{
			string[] defaultTable = new string[]
			{
				"ps.outlook.com",
				"ps.partner.outlook.cn",
				"exchangelabs.live-int.com"
			};
			return Configuration.GetValue<string>("TenantPowerShellEndpoint", defaultTable, instance);
		}

		public static SmtpX509Identifier FopeCertificateName()
		{
			return Configuration.FopeCertificateName(0);
		}

		public static SmtpX509Identifier FopeCertificateName(int instance)
		{
			string[] defaultTable = new string[]
			{
				"CN=MSIT Machine Auth CA 2, DC=redmond, DC=corp, DC=microsoft, DC=com",
				"CN=CNNIC SSL, O=CNNIC SSL, C=CN",
				"CN=MS Passport Test Sub CA, DC=redmond, DC=corp, DC=microsoft, DC=com"
			};
			string[] defaultTable2 = new string[]
			{
				"CN=mail.protection.outlook.com, OU=Forefront Online Protection for Exchange, O=Microsoft, L=Redmond, S=WA, C=US",
				"CN=*.mail.protection.partner.outlook.cn, OU=Office365, O=Shanghai Blue Cloud Technology Co. Ltd, L=Shanghai, S=Shanghai, C=CN",
				"CN=*.mail.o365filtering-int.com, OU=Forefront Online Protection for Exchange, O=Microsoft, L=Redmond, S=Washington, C=US"
			};
			string[] defaultTable3 = new string[]
			{
				"AcceptCloudServicesMail",
				"AcceptCloudServicesMail",
				"AcceptCloudServicesMail"
			};
			string value = Configuration.GetValue<string>("FopeCertificateIssuer", defaultTable, instance);
			string value2 = Configuration.GetValue<string>("FopeCertificateSubject", defaultTable2, instance);
			string value3 = Configuration.GetValue<string>("FopeCertificatePermissions", defaultTable3, instance);
			return SmtpX509Identifier.Parse(string.Format("<I>{0}<S>{1}:{2}", value, value2, value3));
		}

		public static SmtpReceiveDomainCapabilities TlsDomainCapabilities(int instance)
		{
			string defaultValue = "AcceptCloudServicesMail";
			string arg = Configuration.FopeCertificateDomain(instance);
			string value = Configuration.GetValue<string>("FopeCertificatePermissions", defaultValue);
			return SmtpReceiveDomainCapabilities.Parse(string.Format("{0}:{1}", arg, value));
		}

		public static string FopeCertificateDomain(int instance)
		{
			string[] defaultTable = new string[]
			{
				"mail.protection.outlook.com",
				"*.mail.protection.partner.outlook.cn",
				"*.mail.o365filtering-int.com"
			};
			return Configuration.GetValue<string>("FopeCertificateDomain", defaultTable, instance);
		}

		public static string SignupDomainSuffix(int instance)
		{
			string[] defaultTable = new string[]
			{
				"onmicrosoft.com",
				"partner.onmschina.cn",
				"msol-test.com"
			};
			return Configuration.GetValue<string>("SignupDomainSuffix", defaultTable, instance);
		}

		public static string TargetOwaPrefix(int instance)
		{
			string[] defaultTable = new string[]
			{
				"http://outlook.com/owa/",
				"http://partner.outlook.cn/owa/",
				"http://outlook.com/owa/"
			};
			return Configuration.GetValue<string>("TargetOwaPrefix", defaultTable, instance);
		}

		public static bool RequiresFederationTrust(int instance)
		{
			int[] defaultTable = new int[]
			{
				1,
				0,
				1
			};
			return Configuration.GetValue<int>("RequiresFederationTrust", defaultTable, instance) != 0;
		}

		public static bool RequiresIntraOrganizationConnector(int instance)
		{
			int[] array = new int[3];
			array[1] = 1;
			int[] defaultTable = array;
			return Configuration.GetValue<int>("RequiresIntraOrganizationConnector", defaultTable, instance) != 0;
		}

		public static bool RestrictIOCToSP1OrGreater(int instance)
		{
			int[] defaultTable = new int[]
			{
				1,
				0,
				1
			};
			return Configuration.GetValue<int>("RestrictIOCToSP1OrGreater", defaultTable, instance) != 0;
		}

		public static string OAuthConfigurationUrl(int instance)
		{
			string[] defaultTable = new string[]
			{
				"http://go.microsoft.com/fwlink/?LinkID=320386&clcid=0x409",
				"http://go.microsoft.com/fwlink/?LinkID=392853&clcid=0x409",
				"http://go.microsoft.com/fwlink/?LinkID=320386&clcid=0x409"
			};
			return Configuration.GetValue<string>("OAuthConfigurationUrl", defaultTable, instance);
		}

		public static T GetValue<T>(string name, T[] defaultTable, int instance)
		{
			if (instance < 0 || instance >= defaultTable.Length)
			{
				instance = 0;
			}
			return Configuration.GetValue<T>(name, defaultTable[instance]);
		}

		private static T GetValue<T>(string name, T defaultValue)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Update-HybridConfiguration"))
				{
					if (registryKey != null)
					{
						return (T)((object)registryKey.GetValue(name, defaultValue));
					}
				}
			}
			catch (IOException)
			{
			}
			return defaultValue;
		}
	}
}
