using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaVdirConfiguration : VdirConfiguration
	{
		private OwaVdirConfiguration(ADOwaVirtualDirectory owaVirtualDirectory) : base(owaVirtualDirectory)
		{
			this.logonFormat = owaVirtualDirectory.LogonFormat;
			this.publicPrivateSelectionEnabled = (owaVirtualDirectory.LogonPagePublicPrivateSelectionEnabled != null && owaVirtualDirectory.LogonPagePublicPrivateSelectionEnabled.Value);
			this.lightSelectionEnabled = (owaVirtualDirectory.LogonPageLightSelectionEnabled != null && owaVirtualDirectory.LogonPageLightSelectionEnabled.Value);
			this.logonAndErrorLanguage = owaVirtualDirectory.LogonAndErrorLanguage;
			this.redirectToOptimalOWAServer = (owaVirtualDirectory.RedirectToOptimalOWAServer ?? true);
		}

		public new static OwaVdirConfiguration Instance
		{
			get
			{
				return VdirConfiguration.Instance as OwaVdirConfiguration;
			}
		}

		public LogonFormats LogonFormat
		{
			get
			{
				return this.logonFormat;
			}
		}

		public bool PublicPrivateSelectionEnabled
		{
			get
			{
				return this.publicPrivateSelectionEnabled;
			}
		}

		public bool LightSelectionEnabled
		{
			get
			{
				return this.lightSelectionEnabled;
			}
		}

		public int LogonAndErrorLanguage
		{
			get
			{
				return this.logonAndErrorLanguage;
			}
		}

		public bool RedirectToOptimalOWAServer
		{
			get
			{
				return this.redirectToOptimalOWAServer;
			}
		}

		internal static OwaVdirConfiguration CreateInstance(ITopologyConfigurationSession session, ADObjectId virtualDirectoryDN)
		{
			ADOwaVirtualDirectory adowaVirtualDirectory = null;
			ADOwaVirtualDirectory[] array = session.Find<ADOwaVirtualDirectory>(virtualDirectoryDN, QueryScope.Base, null, null, 1);
			if (array != null && array.Length == 1)
			{
				adowaVirtualDirectory = array[0];
			}
			if (adowaVirtualDirectory == null)
			{
				throw new ADNoSuchObjectException(LocalizedString.Empty);
			}
			return new OwaVdirConfiguration(adowaVirtualDirectory);
		}

		private readonly bool publicPrivateSelectionEnabled;

		private readonly bool lightSelectionEnabled;

		private readonly bool redirectToOptimalOWAServer;

		private readonly int logonAndErrorLanguage;

		private LogonFormats logonFormat;
	}
}
