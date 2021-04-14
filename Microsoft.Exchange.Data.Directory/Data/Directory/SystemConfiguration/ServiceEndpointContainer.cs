using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	internal class ServiceEndpointContainer : Container
	{
		public ServiceEndpoint GetEndpoint(string commonName)
		{
			if (string.IsNullOrEmpty(commonName))
			{
				throw new ArgumentNullException("commonName");
			}
			ADServiceConnectionPoint scp = this.GetSCP(commonName);
			if (scp == null)
			{
				throw new ServiceEndpointNotFoundException(commonName);
			}
			Uri uri = null;
			if (scp.ServiceBindingInformation.Count > 0)
			{
				uri = new Uri(scp.ServiceBindingInformation[0]);
			}
			string uriTemplate = null;
			string certificateSubject = null;
			string token = null;
			foreach (string text in scp.Keywords)
			{
				if (text.StartsWith(ServiceEndpointContainer.UriTemplateKey, StringComparison.OrdinalIgnoreCase))
				{
					uriTemplate = text.Substring(ServiceEndpointContainer.UriTemplateKey.Length);
				}
				else if (text.StartsWith(ServiceEndpointContainer.CertSubjectKey, StringComparison.OrdinalIgnoreCase))
				{
					certificateSubject = text.Substring(ServiceEndpointContainer.CertSubjectKey.Length);
				}
				else if (text.StartsWith(ServiceEndpointContainer.TokenKey, StringComparison.OrdinalIgnoreCase))
				{
					token = text.Substring(ServiceEndpointContainer.TokenKey.Length);
				}
			}
			return new ServiceEndpoint(uri, uriTemplate, certificateSubject, token);
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ServiceEndpointContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ServiceEndpointContainer.mostDerivedClass;
			}
		}

		private ADServiceConnectionPoint GetSCP(string commonName)
		{
			return base.Session.Read<ADServiceConnectionPoint>(base.Id.GetChildId(commonName));
		}

		private static ServiceEndpointContainerSchema schema = ObjectSchema.GetInstance<ServiceEndpointContainerSchema>();

		private static string mostDerivedClass = "msExchContainer";

		public static readonly string DefaultName = "ServiceEndpoints";

		public static readonly string UriTemplateKey = "urltemplate:";

		public static readonly string CertSubjectKey = "subject:";

		public static readonly string TokenKey = "token:";
	}
}
