using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Services.Protocols;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal abstract class BaseServiceCallingContext<ServiceBindingType> : IServiceCallingContext<ServiceBindingType> where ServiceBindingType : HttpWebClientProtocol, IServiceBinding, new()
	{
		static BaseServiceCallingContext()
		{
			string arg = "UnknownVersion";
			try
			{
				arg = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
			}
			catch (Exception ex)
			{
				Tracer.TraceError("BaseServiceCallingContext..Ctor: Failed to get assembly file version. {0}", new object[]
				{
					ex
				});
			}
			BaseServiceCallingContext<ServiceBindingType>.userAgent = string.Format("Exchange\\{0}\\EDiscovery", arg);
		}

		public virtual ServiceBindingType CreateServiceBinding(Uri serviceUrl)
		{
			ServiceBindingType serviceBindingType = BaseServiceCallingContext<ServiceBindingType>.CreateDefaultServiceBinding();
			this.SetServiceUrl(serviceBindingType, serviceUrl);
			return serviceBindingType;
		}

		public virtual bool AuthorizeServiceBinding(ServiceBindingType binding)
		{
			return false;
		}

		public virtual void SetServiceApiContext(ServiceBindingType binding, string mailboxEmailAddress)
		{
			binding.HttpContext = BaseServiceCallingContext<ServiceBindingType>.CreateHttpContext(mailboxEmailAddress);
		}

		public virtual void SetServiceUrl(ServiceBindingType binding, Uri targetUrl)
		{
			binding.Url = targetUrl.AbsoluteUri;
			binding.PreAuthenticate = (targetUrl.Scheme == Uri.UriSchemeHttps);
		}

		public virtual void SetServiceUrlAffinity(ServiceBindingType binding, Uri targetUrl)
		{
		}

		protected static ServiceHttpContext CreateHttpContext(string mailboxEmailAddress)
		{
			return new ServiceHttpContext
			{
				ClientRequestId = Guid.NewGuid(),
				AnchorMailbox = mailboxEmailAddress
			};
		}

		private static ServiceBindingType CreateDefaultServiceBinding()
		{
			ServiceBindingType result = Activator.CreateInstance<ServiceBindingType>();
			result.Timeout = 600000;
			result.PreAuthenticate = true;
			result.UserAgent = BaseServiceCallingContext<ServiceBindingType>.userAgent;
			return result;
		}

		private static readonly string userAgent;
	}
}
