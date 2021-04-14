using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	public class ServicePrincipalAuthenticationPolicy
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ServicePrincipalAuthenticationPolicy CreateServicePrincipalAuthenticationPolicy(Collection<string> allowedPolicies)
		{
			ServicePrincipalAuthenticationPolicy servicePrincipalAuthenticationPolicy = new ServicePrincipalAuthenticationPolicy();
			if (allowedPolicies == null)
			{
				throw new ArgumentNullException("allowedPolicies");
			}
			servicePrincipalAuthenticationPolicy.allowedPolicies = allowedPolicies;
			return servicePrincipalAuthenticationPolicy;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string defaultPolicy
		{
			get
			{
				return this._defaultPolicy;
			}
			set
			{
				this._defaultPolicy = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> allowedPolicies
		{
			get
			{
				return this._allowedPolicies;
			}
			set
			{
				this._allowedPolicies = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _defaultPolicy;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _allowedPolicies = new Collection<string>();
	}
}
