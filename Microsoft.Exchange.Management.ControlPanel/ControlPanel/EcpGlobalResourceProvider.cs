using System;
using System.Globalization;
using System.Resources;
using System.Web.Compilation;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EcpGlobalResourceProvider : IResourceProvider
	{
		public EcpGlobalResourceProvider(string classKey)
		{
			if (classKey != null)
			{
				if (classKey == "OwaOptionStrings")
				{
					this.resourceManager = EcpGlobalResourceProvider.OwaOptionResourceManager;
					return;
				}
				if (classKey == "OwaOptionClientStrings")
				{
					this.resourceManager = EcpGlobalResourceProvider.OwaOptionClientResourceManager;
					return;
				}
				if (classKey == "ClientStrings")
				{
					this.resourceManager = EcpGlobalResourceProvider.ClientResourceManager;
					return;
				}
				if (!(classKey == "Strings"))
				{
				}
			}
			this.resourceManager = EcpGlobalResourceProvider.ResourceManager;
		}

		public object GetObject(string resourceKey, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			return this.resourceManager.GetString(resourceKey, culture);
		}

		public IResourceReader ResourceReader
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal static ExchangeResourceManager OwaOptionResourceManager
		{
			get
			{
				if (EcpGlobalResourceProvider.owaOptionResourceManager == null)
				{
					EcpGlobalResourceProvider.owaOptionResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.OwaOptionStrings", typeof(OwaOptionStrings).Assembly);
				}
				return EcpGlobalResourceProvider.owaOptionResourceManager;
			}
		}

		internal static ExchangeResourceManager OwaOptionClientResourceManager
		{
			get
			{
				if (EcpGlobalResourceProvider.owaOptionClientResourceManager == null)
				{
					EcpGlobalResourceProvider.owaOptionClientResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.OwaOptionClientStrings", typeof(OwaOptionClientStrings).Assembly);
				}
				return EcpGlobalResourceProvider.owaOptionClientResourceManager;
			}
		}

		internal static ExchangeResourceManager ResourceManager
		{
			get
			{
				if (EcpGlobalResourceProvider.ecpResourceManager == null)
				{
					EcpGlobalResourceProvider.ecpResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.Strings", typeof(Strings).Assembly);
				}
				return EcpGlobalResourceProvider.ecpResourceManager;
			}
		}

		internal static ExchangeResourceManager ClientResourceManager
		{
			get
			{
				if (EcpGlobalResourceProvider.ecpClientResourceManager == null)
				{
					EcpGlobalResourceProvider.ecpClientResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.ClientStrings", typeof(ClientStrings).Assembly);
				}
				return EcpGlobalResourceProvider.ecpClientResourceManager;
			}
		}

		private static ExchangeResourceManager owaOptionResourceManager;

		private static ExchangeResourceManager owaOptionClientResourceManager;

		private static ExchangeResourceManager ecpResourceManager;

		private static ExchangeResourceManager ecpClientResourceManager;

		private ExchangeResourceManager resourceManager;
	}
}
