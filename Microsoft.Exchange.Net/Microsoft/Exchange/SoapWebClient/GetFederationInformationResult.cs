using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.SoapWebClient
{
	[Serializable]
	public sealed class GetFederationInformationResult : ISerializable
	{
		public AutodiscoverResult Type { get; internal set; }

		public LocalizedException Exception { get; internal set; }

		public GetFederationInformationResult Alternate { get; internal set; }

		public Uri Url { get; internal set; }

		public string ApplicationUri { get; internal set; }

		public string[] TokenIssuerUris { get; internal set; }

		public StringList Domains { get; internal set; }

		public Uri RedirectUrl { get; internal set; }

		public StringList SslCertificateHostnames { get; internal set; }

		public string Details { get; internal set; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo serializationInfo, StreamingContext context)
		{
			serializationInfo.AddValue("Type", this.Type);
			serializationInfo.AddValue("Exception", this.Exception);
			serializationInfo.AddValue("Alternate", this.Alternate);
			serializationInfo.AddValue("Url", this.Url);
			serializationInfo.AddValue("ApplicationUri", this.ApplicationUri);
			serializationInfo.AddValue("TokenIssuerUris", this.TokenIssuerUris);
			serializationInfo.AddValue("Domains", this.Domains);
			serializationInfo.AddValue("RedirectUrl", this.RedirectUrl);
			serializationInfo.AddValue("SslCertificateHostnames", this.SslCertificateHostnames);
			serializationInfo.AddValue("Details", this.Details);
		}

		public GetFederationInformationResult(SerializationInfo serializationInfo, StreamingContext context)
		{
			this.Type = GetFederationInformationResult.GetSerializedValue<AutodiscoverResult>(serializationInfo, "Type");
			this.Exception = GetFederationInformationResult.GetSerializedValue<LocalizedException>(serializationInfo, "Exception");
			this.Alternate = GetFederationInformationResult.GetSerializedValue<GetFederationInformationResult>(serializationInfo, "Alternate");
			this.Url = GetFederationInformationResult.GetSerializedValue<Uri>(serializationInfo, "Url");
			this.ApplicationUri = GetFederationInformationResult.GetSerializedValue<string>(serializationInfo, "ApplicationUri");
			this.TokenIssuerUris = GetFederationInformationResult.GetSerializedValue<string[]>(serializationInfo, "TokenIssuerUris");
			this.Domains = GetFederationInformationResult.GetSerializedValue<StringList>(serializationInfo, "Domains");
			this.RedirectUrl = GetFederationInformationResult.GetSerializedValue<Uri>(serializationInfo, "RedirectUrl");
			this.SslCertificateHostnames = GetFederationInformationResult.GetSerializedValue<StringList>(serializationInfo, "SslCertificateHostnames");
			this.Details = GetFederationInformationResult.GetSerializedValue<string>(serializationInfo, "Details");
		}

		private static T GetSerializedValue<T>(SerializationInfo serializationInfo, string name)
		{
			object value = serializationInfo.GetValue(name, typeof(T));
			if (value == null)
			{
				return default(T);
			}
			return (T)((object)value);
		}

		internal GetFederationInformationResult()
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(400);
			stringBuilder.Append("Type=" + this.Type.ToString() + ";");
			if (this.Url != null)
			{
				stringBuilder.Append("Url=" + this.Url + ";");
			}
			if (this.ApplicationUri != null)
			{
				stringBuilder.Append("ApplicationUri=" + this.ApplicationUri + ";");
			}
			if (this.TokenIssuerUris != null)
			{
				stringBuilder.Append("TokenIssuerUris=" + string.Join(",", this.TokenIssuerUris) + ";");
			}
			if (this.Domains != null)
			{
				stringBuilder.Append("Domains=" + this.Domains.ToString() + ";");
			}
			if (this.Exception != null)
			{
				stringBuilder.Append("Exception=" + this.Exception.Message + ";");
			}
			if (this.Alternate != null)
			{
				stringBuilder.Append("Alternate=(" + this.Alternate.ToString() + ");");
			}
			if (this.Details != null)
			{
				stringBuilder.Append("Details=(" + this.Details + ");");
			}
			return stringBuilder.ToString();
		}
	}
}
