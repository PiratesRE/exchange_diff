using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization;
using System.Web.Configuration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class IpSafeListing : BaseRow
	{
		public IpSafeListing(PerimeterConfig pc) : base(pc)
		{
			this.perimeterConfig = pc;
		}

		[DataMember]
		public string[] GatewayIPAddresses
		{
			get
			{
				return this.perimeterConfig.GatewayIPAddresses.ToStringArray<IPAddress>();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string[] InternalServerIPAddresses
		{
			get
			{
				return this.perimeterConfig.InternalServerIPAddresses.ToStringArray<IPAddress>();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public bool IPSkiplistingEnabled
		{
			get
			{
				return this.perimeterConfig.IPSkiplistingEnabled;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal SafelistingUIMode SafelistingUIMode
		{
			get
			{
				return this.perimeterConfig.SafelistingUIMode;
			}
		}

		[DataMember]
		internal string FoseLink
		{
			get
			{
				if (!string.IsNullOrEmpty(this.perimeterConfig.PerimeterOrgId))
				{
					return string.Format("{0}{1}", IpSafeListing.FoseUrl, this.perimeterConfig.PerimeterOrgId);
				}
				return HelpUtil.BuildEhcHref(EACHelpId.FoseLinkNotAvailable.ToString());
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private static string GetFoseUrl()
		{
			try
			{
				string text = WebConfigurationManager.AppSettings["FoseSsoUrl"];
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			catch (ConfigurationErrorsException)
			{
			}
			return "https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=urn%3afopsts%3aprod&wctx=rm%3d0%26id%3dFederatedPassiveSignIn1%26ru%3d%252f%253fwa%253dwsignin1.0%2526wtrealm%253dhttps%25253a%25252f%25252fadmin.messaging.microsoft.com%2526wctx%253drm%25253d0%252526id%25253dpassive%252526ru%25253d%2525252fSettings.mvc%2525252fSettings%2525252f";
		}

		private const string FoseSsoProdUrl = "https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=urn%3afopsts%3aprod&wctx=rm%3d0%26id%3dFederatedPassiveSignIn1%26ru%3d%252f%253fwa%253dwsignin1.0%2526wtrealm%253dhttps%25253a%25252f%25252fadmin.messaging.microsoft.com%2526wctx%253drm%25253d0%252526id%25253dpassive%252526ru%25253d%2525252fSettings.mvc%2525252fSettings%2525252f";

		private static readonly string FoseUrl = IpSafeListing.GetFoseUrl();

		private PerimeterConfig perimeterConfig;
	}
}
