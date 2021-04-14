using System;
using System.DirectoryServices;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.HttpProxy;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class VdirConfiguration
	{
		internal VdirConfiguration(ExchangeWebAppVirtualDirectory virtualDirectory)
		{
			this.internalAuthenticationMethod = VdirConfiguration.ConvertAuthenticationMethods(virtualDirectory.InternalAuthenticationMethods);
			this.externalAuthenticationMethod = VdirConfiguration.ConvertAuthenticationMethods(virtualDirectory.ExternalAuthenticationMethods);
		}

		public static VdirConfiguration Instance
		{
			get
			{
				if (VdirConfiguration.instance == null)
				{
					lock (VdirConfiguration.syncRoot)
					{
						if (VdirConfiguration.instance == null)
						{
							VdirConfiguration.instance = VdirConfiguration.BaseCreateInstance();
						}
					}
				}
				return VdirConfiguration.instance;
			}
		}

		internal AuthenticationMethod InternalAuthenticationMethod
		{
			get
			{
				return this.internalAuthenticationMethod;
			}
		}

		internal AuthenticationMethod ExternalAuthenticationMethod
		{
			get
			{
				return this.externalAuthenticationMethod;
			}
		}

		private static AuthenticationMethod ConvertAuthenticationMethods(MultiValuedProperty<AuthenticationMethod> configMethods)
		{
			AuthenticationMethod authenticationMethod = AuthenticationMethod.None;
			using (MultiValuedProperty<AuthenticationMethod>.Enumerator enumerator = configMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case AuthenticationMethod.Basic:
						authenticationMethod |= AuthenticationMethod.Basic;
						break;
					case AuthenticationMethod.Digest:
						authenticationMethod |= AuthenticationMethod.Digest;
						break;
					case AuthenticationMethod.Ntlm:
						authenticationMethod |= AuthenticationMethod.Ntlm;
						break;
					case AuthenticationMethod.Fba:
						authenticationMethod |= AuthenticationMethod.Fba;
						break;
					case AuthenticationMethod.WindowsIntegrated:
						authenticationMethod |= AuthenticationMethod.WindowsIntegrated;
						break;
					case AuthenticationMethod.LiveIdFba:
						authenticationMethod |= AuthenticationMethod.LiveIdFba;
						break;
					case AuthenticationMethod.LiveIdBasic:
						authenticationMethod |= AuthenticationMethod.LiveIdBasic;
						break;
					case AuthenticationMethod.WSSecurity:
						authenticationMethod |= AuthenticationMethod.WSSecurity;
						break;
					case AuthenticationMethod.Certificate:
						authenticationMethod |= AuthenticationMethod.Certificate;
						break;
					case AuthenticationMethod.NegoEx:
						authenticationMethod |= AuthenticationMethod.NegoEx;
						break;
					case AuthenticationMethod.OAuth:
						authenticationMethod |= AuthenticationMethod.OAuth;
						break;
					case AuthenticationMethod.Adfs:
						authenticationMethod |= AuthenticationMethod.Adfs;
						break;
					case AuthenticationMethod.Kerberos:
						authenticationMethod |= AuthenticationMethod.Kerberos;
						break;
					case AuthenticationMethod.Negotiate:
						authenticationMethod |= AuthenticationMethod.Negotiate;
						break;
					case AuthenticationMethod.LiveIdNegotiate:
						authenticationMethod |= AuthenticationMethod.LiveIdNegotiate;
						break;
					}
				}
			}
			return authenticationMethod;
		}

		private static VdirConfiguration BaseCreateInstance()
		{
			ITopologyConfigurationSession session = VdirConfiguration.CreateADSystemConfigurationSessionScopedToFirstOrg();
			ExchangeVirtualDirectory member = HttpProxyGlobals.VdirObject.Member;
			if (member is ADEcpVirtualDirectory)
			{
				return EcpVdirConfiguration.CreateInstance(session, member.Id);
			}
			if (member is ADOwaVirtualDirectory)
			{
				return OwaVdirConfiguration.CreateInstance(session, member.Id);
			}
			throw new ADNoSuchObjectException(new LocalizedString(string.Format("NoVdirConfiguration. AppDomainAppId:{0},VDirDN:{1}", HttpRuntime.AppDomainAppId, (member == null) ? "NULL" : member.DistinguishedName)));
		}

		private static ITopologyConfigurationSession CreateADSystemConfigurationSessionScopedToFirstOrg()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 207, "CreateADSystemConfigurationSessionScopedToFirstOrg", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\fba\\VdirConfiguration.cs");
		}

		private static string GetWebSiteName(string webSiteRootPath)
		{
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(webSiteRootPath))
				{
					using (DirectoryEntry parent = directoryEntry.Parent)
					{
						if (parent != null)
						{
							return ((string)parent.Properties["ServerComment"].Value) ?? string.Empty;
						}
					}
				}
			}
			catch (DirectoryServicesCOMException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			return string.Empty;
		}

		private static volatile VdirConfiguration instance;

		private static object syncRoot = new object();

		private readonly AuthenticationMethod internalAuthenticationMethod;

		private readonly AuthenticationMethod externalAuthenticationMethod;
	}
}
