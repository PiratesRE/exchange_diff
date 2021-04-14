using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class AddressFinderModule : IHttpModule
	{
		public AddressFinderModule() : this(AddressFinderFactory.GetInstance(), HttpProxySettings.AddressFinderEnabled.Value, null)
		{
		}

		internal AddressFinderModule(IAddressFinderFactory addressFinderFactory, bool isEnabled, IAddressFinderDiagnostics diagnostics)
		{
			if (addressFinderFactory == null)
			{
				throw new ArgumentNullException("addressFinderFactory");
			}
			this.addressFinderFactory = addressFinderFactory;
			this.isEnabled = isEnabled;
			this.diagnostics = diagnostics;
		}

		public void Init(HttpApplication application)
		{
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		public void Dispose()
		{
		}

		internal void OnPostAuthorizeRequest(HttpContextBase context)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (!this.isEnabled)
				{
					return;
				}
				IAddressFinderDiagnostics addressFinderDiagnostics = this.GetDiagnostics(context);
				IAddressFinder addressFinder = this.addressFinderFactory.CreateAddressFinder(HttpProxyGlobals.ProtocolType, context.Request.Url.AbsolutePath);
				if (addressFinder != null)
				{
					AddressFinderSource source = new AddressFinderSource(context.Items, context.Request.Headers, context.Request.QueryString, context.Request.Url, context.Request.ApplicationPath, context.Request.FilePath, context.Request.Cookies);
					IRoutingKey[] value = addressFinder.Find(source, addressFinderDiagnostics);
					context.Items["RoutingKeys"] = value;
				}
				else
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string, Uri, ProtocolType>((long)this.GetHashCode(), "[AddressFinderModule::OnPostAuthorizeRequest]: addressFinder is null: Method {0}; Url {1}; Protocol {2};", context.Request.HttpMethod, context.Request.Url, HttpProxyGlobals.ProtocolType);
					addressFinderDiagnostics.AddErrorInfo("addressFinder is null");
				}
				addressFinderDiagnostics.LogRoutingKeys();
			}, new Diagnostics.LastChanceExceptionHandler(this.LastChanceExceptionHandler));
		}

		private IAddressFinderDiagnostics GetDiagnostics(HttpContextBase context)
		{
			IAddressFinderDiagnostics result;
			if (this.diagnostics == null)
			{
				result = new AddressFinderDiagnostics(context);
			}
			else
			{
				result = this.diagnostics;
			}
			return result;
		}

		private void LastChanceExceptionHandler(Exception ex)
		{
			IAddressFinderDiagnostics addressFinderDiagnostics = this.GetDiagnostics(new HttpContextWrapper(HttpContext.Current));
			if (addressFinderDiagnostics != null)
			{
				addressFinderDiagnostics.LogUnhandledException(ex);
			}
		}

		private readonly bool isEnabled;

		private readonly IAddressFinderDiagnostics diagnostics;

		private readonly IAddressFinderFactory addressFinderFactory;
	}
}
