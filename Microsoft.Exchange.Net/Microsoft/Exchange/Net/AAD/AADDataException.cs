using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.AAD
{
	internal sealed class AADDataException : AADException
	{
		public AADDataException.AADCode Code { get; private set; }

		public bool IsRetryable { get; private set; }

		public override string Message
		{
			get
			{
				return this.errorMessage.ToString();
			}
		}

		public AADDataException(Exception exception) : base(new LocalizedString(exception.Message), exception)
		{
			this.IsRetryable = AADDataException.IsRetryableError(exception);
			if (exception is DataServiceClientException)
			{
				this.ParseErrorXml(exception.Message);
			}
			else if (exception.InnerException != null && exception.InnerException is DataServiceClientException)
			{
				this.ParseErrorXml(exception.InnerException.Message);
			}
			if (this.errorMessage.Length == 0)
			{
				this.errorMessage.AppendLine(exception.Message);
				if (exception.InnerException != null)
				{
					this.errorMessage.AppendLine(exception.InnerException.Message);
				}
			}
			DataServiceRequestException ex = exception as DataServiceRequestException;
			if (ex != null && ex.Response != null)
			{
				if (ex.Response.IsBatchResponse)
				{
					this.AddHeadersToErrorMessage(ex.Response.BatchHeaders);
					return;
				}
				foreach (OperationResponse operationResponse in ex.Response)
				{
					this.AddHeadersToErrorMessage(operationResponse.Headers);
				}
			}
		}

		private static bool IsRetryableError(Exception exception)
		{
			HttpStatusCode? httpStatusCode = null;
			DataServiceRequestException ex;
			DataServiceClientException ex2;
			DataServiceQueryException ex3;
			if ((ex = (exception as DataServiceRequestException)) != null)
			{
				OperationResponse operationResponse = ex.Response.FirstOrDefault<OperationResponse>();
				httpStatusCode = new HttpStatusCode?((HttpStatusCode)((operationResponse != null) ? operationResponse.StatusCode : ex.Response.BatchStatusCode));
			}
			else if ((ex2 = (exception as DataServiceClientException)) != null)
			{
				httpStatusCode = new HttpStatusCode?((HttpStatusCode)ex2.StatusCode);
			}
			else if ((ex3 = (exception as DataServiceQueryException)) != null)
			{
				httpStatusCode = new HttpStatusCode?((HttpStatusCode)ex3.Response.StatusCode);
			}
			else
			{
				WebException ex4 = exception as WebException;
				if (ex4 != null || (ex4 = (exception.InnerException as WebException)) != null)
				{
					HttpWebResponse httpWebResponse = ex4.Response as HttpWebResponse;
					if (httpWebResponse == null)
					{
						return ex4.Status == WebExceptionStatus.NameResolutionFailure;
					}
					httpStatusCode = new HttpStatusCode?(httpWebResponse.StatusCode);
				}
			}
			return httpStatusCode != null && (httpStatusCode == HttpStatusCode.InternalServerError || httpStatusCode == HttpStatusCode.BadGateway || httpStatusCode == HttpStatusCode.ServiceUnavailable);
		}

		private void ParseErrorXml(string xml)
		{
			try
			{
				using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(new StringReader(xml)))
				{
					if (xmlTextReader.ReadToFollowing("code", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"))
					{
						string key = xmlTextReader.ReadElementContentAsString();
						AADDataException.AADCode code;
						if (AADDataException.CodeMapping.TryGetValue(key, out code))
						{
							this.Code = code;
							this.errorMessage.AppendLine(this.Code.ToString());
						}
					}
					if (xmlTextReader.LocalName == "message" || xmlTextReader.ReadToFollowing("message", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"))
					{
						this.errorMessage.AppendLine(xmlTextReader.ReadElementContentAsString());
					}
				}
			}
			catch (XmlException)
			{
			}
		}

		private void AddHeadersToErrorMessage(IDictionary<string, string> headers)
		{
			if (headers == null)
			{
				return;
			}
			foreach (string text in AADDataException.HeadersToIncludeInErrorMessage)
			{
				string str;
				if (headers.TryGetValue(text, out str))
				{
					this.errorMessage.AppendLine(text + ": " + str);
				}
			}
		}

		private static readonly string[] HeadersToIncludeInErrorMessage = new string[]
		{
			"request-id",
			"Date",
			"ocp-aad-diagnostics-server-name"
		};

		private StringBuilder errorMessage = new StringBuilder();

		private static Dictionary<string, AADDataException.AADCode> CodeMapping = new Dictionary<string, AADDataException.AADCode>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Request_BadRequest",
				AADDataException.AADCode.Request_BadRequest
			},
			{
				"Request_UnsupportedQuery",
				AADDataException.AADCode.Request_UnsupportedQuery
			},
			{
				"Directory_ResultSizeLimitExceeded",
				AADDataException.AADCode.Directory_ResultSizeLimitExceeded
			},
			{
				"Authentication_MissingOrMalformed",
				AADDataException.AADCode.Authentication_MissingOrMalformed
			},
			{
				"Authorization_IdentityNotFound",
				AADDataException.AADCode.Authorization_IdentityNotFound
			},
			{
				"Authorization_IdentityDisabled",
				AADDataException.AADCode.Authorization_IdentityDisabled
			},
			{
				"Authentication_ExpiredToken",
				AADDataException.AADCode.Authentication_ExpiredToken
			},
			{
				"Authorization_RequestDenied",
				AADDataException.AADCode.Authorization_RequestDenied
			},
			{
				"Authentication_Unauthorized",
				AADDataException.AADCode.Authentication_Unauthorized
			},
			{
				"Directory_QuotaExceeded",
				AADDataException.AADCode.Directory_QuotaExceeded
			},
			{
				"Request_ResourceNotFound",
				AADDataException.AADCode.Request_ResourceNotFound
			},
			{
				"Service_InternalServerError",
				AADDataException.AADCode.Service_InternalServerError
			},
			{
				"Request_ThrottledTemporarily",
				AADDataException.AADCode.Request_ThrottledTemporarily
			}
		};

		public enum AADCode
		{
			Unknown,
			Request_BadRequest,
			Request_UnsupportedQuery,
			Directory_ResultSizeLimitExceeded,
			Authentication_MissingOrMalformed,
			Authorization_IdentityNotFound,
			Authorization_IdentityDisabled,
			Authentication_ExpiredToken,
			Authorization_RequestDenied,
			Authentication_Unauthorized,
			Directory_QuotaExceeded,
			Request_ResourceNotFound,
			Service_InternalServerError,
			Request_ThrottledTemporarily
		}
	}
}
