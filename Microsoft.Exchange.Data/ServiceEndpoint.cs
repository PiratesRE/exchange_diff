using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ServiceEndpoint
	{
		public ServiceEndpoint(Uri uri) : this(uri, string.Empty, string.Empty, string.Empty)
		{
		}

		public ServiceEndpoint(Uri uri, string certificateSubject) : this(uri, string.Empty, certificateSubject, string.Empty)
		{
		}

		public ServiceEndpoint(Uri uri, string uriTemplate, string certificateSubject, string token)
		{
			this.uri = uri;
			this.uriTemplate = uriTemplate;
			this.certificateSubject = certificateSubject;
			this.token = token;
		}

		public ServiceEndpoint ApplyTemplate(params object[] uriTemplateArgs)
		{
			if (uriTemplateArgs == null)
			{
				throw new ArgumentNullException("uriTemplateArgs");
			}
			if (this.uri != null)
			{
				throw new FormatException(string.Format("URI {0} is already formatted", this.uri.AbsoluteUri));
			}
			if (string.IsNullOrEmpty(this.uriTemplate))
			{
				throw new FormatException("URI template is empty");
			}
			Uri uri = new Uri(string.Format(this.uriTemplate, uriTemplateArgs));
			return new ServiceEndpoint(uri, string.Empty, this.certificateSubject, this.token);
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public string UriTemplate
		{
			get
			{
				return this.uriTemplate;
			}
		}

		public string CertificateSubject
		{
			get
			{
				return this.certificateSubject;
			}
		}

		public string Token
		{
			get
			{
				return this.token;
			}
		}

		private Uri uri;

		private string uriTemplate;

		private string certificateSubject;

		private string token;
	}
}
