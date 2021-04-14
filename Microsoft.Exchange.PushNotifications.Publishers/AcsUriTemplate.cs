using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AcsUriTemplate
	{
		public AcsUriTemplate(string uriTemplate, string scopeTemplate)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("uriTemplate", uriTemplate);
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Format", uriTemplate, (string x) => x.Contains("{0}"));
			if (uriTemplate.Contains("{1}"))
			{
				this.isPartitionTemplate = true;
			}
			if (this.isPartitionTemplate)
			{
				ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Uri", uriTemplate, (string x) => Uri.IsWellFormedUriString(string.Format(x, "ns", "pn"), UriKind.Absolute));
				ArgumentValidator.ThrowIfNullOrWhiteSpace("scopeTemplate", scopeTemplate);
				ArgumentValidator.ThrowIfInvalidValue<string>("scopeTemplate-Format", scopeTemplate, (string x) => x.Contains("{0}") && x.Contains("{1}"));
				ArgumentValidator.ThrowIfInvalidValue<string>("scopeTemplate-Uri", scopeTemplate, (string x) => Uri.IsWellFormedUriString(string.Format(x, "ns", "pn"), UriKind.Absolute));
			}
			else
			{
				ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Uri", uriTemplate, (string x) => Uri.IsWellFormedUriString(string.Format(x, "ns"), UriKind.Absolute));
				ArgumentValidator.ThrowIfNullOrWhiteSpace("scopeTemplate", scopeTemplate);
				ArgumentValidator.ThrowIfInvalidValue<string>("scopeTemplate-Format", scopeTemplate, (string x) => x.Contains("{0}"));
				ArgumentValidator.ThrowIfInvalidValue<string>("scopeTemplate-Uri", scopeTemplate, (string x) => Uri.IsWellFormedUriString(string.Format(x, "ns"), UriKind.Absolute));
			}
			this.UriTemplate = uriTemplate;
			this.ScopeTemplate = scopeTemplate;
		}

		public string UriTemplate { get; private set; }

		public string ScopeTemplate { get; private set; }

		public Uri CreateAcsTokenRequestUri(AzureHubCreationNotification notification)
		{
			if (this.isPartitionTemplate)
			{
				return new Uri(new Uri(string.Format(this.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), notification.Partition)), "WRAPv0.9");
			}
			return new Uri(new Uri(string.Format(this.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId))), "WRAPv0.9");
		}

		public string CreateScopeUriString(AzureHubCreationNotification notification)
		{
			if (this.isPartitionTemplate)
			{
				return string.Format(this.ScopeTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), notification.Partition);
			}
			return string.Format(this.ScopeTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId));
		}

		public const string WrapResourceName = "WRAPv0.9";

		private readonly bool isPartitionTemplate;
	}
}
