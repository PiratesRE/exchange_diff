using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AuthRedirect : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AuthRedirect.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AuthRedirect.MostDerivedClass;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public AuthScheme AuthScheme
		{
			get
			{
				return (AuthScheme)this[AuthRedirectSchema.AuthScheme];
			}
			set
			{
				this[AuthRedirectSchema.AuthScheme] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string TargetUrl
		{
			get
			{
				return (string)this[AuthRedirectSchema.TargetUrl];
			}
			set
			{
				this[AuthRedirectSchema.TargetUrl] = value;
			}
		}

		internal static readonly string AuthRedirectKeywords = "C0B7AC3F-FE64-4b4b-A907-9226F8027CCE";

		internal static readonly ComparisonFilter AuthRedirectKeywordsFilter = new ComparisonFilter(ComparisonOperator.Equal, AuthRedirectSchema.Keywords, AuthRedirect.AuthRedirectKeywords);

		private static readonly AuthRedirectSchema SchemaObject = ObjectSchema.GetInstance<AuthRedirectSchema>();

		private static readonly string MostDerivedClass = "serviceConnectionPoint";
	}
}
