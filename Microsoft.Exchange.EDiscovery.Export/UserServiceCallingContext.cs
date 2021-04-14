using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Services.Protocols;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class UserServiceCallingContext<ServiceBindingType> : BaseServiceCallingContext<ServiceBindingType> where ServiceBindingType : HttpWebClientProtocol, IServiceBinding, new()
	{
		public UserServiceCallingContext(ICredentialHandler credentialHandler, IDictionary<string, ICredentials> cachedCredentials)
		{
			if (credentialHandler == null)
			{
				throw new ArgumentNullException("credentialHandler");
			}
			this.credentialHandler = credentialHandler;
			this.cachedCredentials = (cachedCredentials ?? new Dictionary<string, ICredentials>());
		}

		public override bool AuthorizeServiceBinding(ServiceBindingType binding)
		{
			Uri uri = new Uri(binding.Url);
			if (uri.Scheme != Uri.UriSchemeHttps)
			{
				return false;
			}
			string key = uri.Host.ToLowerInvariant();
			if (binding.UseDefaultCredentials)
			{
				Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: binding was using default credentials.", new object[0]);
				if (this.cachedCredentials.ContainsKey(key))
				{
					Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: cache contains the credential and returns it.", new object[0]);
					ICredentials credentials = this.cachedCredentials[key];
					binding.UseDefaultCredentials = false;
					binding.Credentials = credentials;
					return true;
				}
				Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: cache doesn't contains the credential.", new object[0]);
			}
			else
			{
				Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: binding was using explicit credentials.", new object[0]);
				ICredentials credentials2;
				if (this.cachedCredentials.TryGetValue(key, out credentials2))
				{
					Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: cache contains the credential and returns it.", new object[0]);
					if (!credentials2.Equals(binding.Credentials))
					{
						Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: credential in cache is the different from the binding is using", new object[0]);
						binding.UseDefaultCredentials = false;
						binding.Credentials = credentials2;
						return true;
					}
					Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: credential in cache is the same as the binding is using", new object[0]);
					this.credentialHandler.InvalidateCredential(uri);
				}
				Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: cache doesn't contains the credential.", new object[0]);
			}
			if (this.credentialHandler.RequestCredential(uri))
			{
				Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: credential requested and returned.", new object[0]);
				ICredentials credential = this.credentialHandler.Credential;
				this.cachedCredentials[key] = credential;
				binding.UseDefaultCredentials = false;
				binding.Credentials = credential;
				return true;
			}
			Tracer.TraceInformation("UserServiceCallingContext.AuthorizeServiceBinding: credential request failed.", new object[0]);
			return false;
		}

		public override void SetServiceUrl(ServiceBindingType binding, Uri targetUrl)
		{
			base.SetServiceUrl(binding, targetUrl);
			string key = targetUrl.Host.ToLowerInvariant();
			if (this.cachedCredentials.ContainsKey(key))
			{
				binding.UseDefaultCredentials = false;
				binding.Credentials = this.cachedCredentials[key];
				return;
			}
			binding.Credentials = null;
			binding.UseDefaultCredentials = true;
		}

		public override void SetServiceUrlAffinity(ServiceBindingType binding, Uri targetUrl)
		{
			base.SetServiceUrlAffinity(binding, targetUrl);
			if (binding.Credentials != null && !binding.UseDefaultCredentials)
			{
				string key = targetUrl.Host.ToLowerInvariant();
				this.cachedCredentials[key] = binding.Credentials;
			}
		}

		private readonly ICredentialHandler credentialHandler;

		private IDictionary<string, ICredentials> cachedCredentials;
	}
}
