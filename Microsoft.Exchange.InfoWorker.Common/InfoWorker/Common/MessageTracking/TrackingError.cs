using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class TrackingError
	{
		internal TrackingError(ErrorCode errorCode, string target, string data, string exception) : this(Names<Microsoft.Exchange.InfoWorker.Common.MessageTracking.ErrorCode>.Map[(int)errorCode], ServerCache.Instance.GetLocalServer().Fqdn, ServerCache.Instance.GetLocalServer().Domain, target, data, exception)
		{
		}

		private TrackingError(string errorCodeString, string server, string domain, string target, string data, string exception)
		{
			this.properties = new Dictionary<string, string>(5);
			base..ctor();
			if (string.IsNullOrEmpty(errorCodeString))
			{
				throw new ArgumentNullException("errorCodeString");
			}
			if (string.IsNullOrEmpty(server))
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(domain))
			{
				throw new ArgumentNullException("domain");
			}
			this.properties["ErrorCode"] = errorCodeString;
			this.properties["Server"] = server;
			this.properties["Domain"] = domain;
			this.properties["Target"] = target;
			this.properties["Data"] = data;
			this.properties["Exception"] = exception;
		}

		private TrackingError(ArrayOfTrackingPropertiesType propertyBag)
		{
			this.properties = new Dictionary<string, string>(5);
			base..ctor();
			foreach (TrackingPropertyType trackingPropertyType in propertyBag.Items)
			{
				this.properties[trackingPropertyType.Name] = trackingPropertyType.Value;
			}
		}

		internal string ErrorCode
		{
			get
			{
				return this.properties["ErrorCode"];
			}
		}

		internal string Target
		{
			get
			{
				string result;
				if (this.properties.TryGetValue("Target", out result))
				{
					return result;
				}
				return string.Empty;
			}
		}

		internal string Domain
		{
			get
			{
				return this.properties["Domain"];
			}
		}

		internal string Server
		{
			get
			{
				return this.properties["Server"];
			}
		}

		internal string Data
		{
			get
			{
				string result;
				if (this.properties.TryGetValue("Data", out result))
				{
					return result;
				}
				return string.Empty;
			}
		}

		internal Dictionary<string, string> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Error Details: ", this.properties.Count * 20);
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in this.properties)
			{
				stringBuilder.AppendFormat("{0}={1}", keyValuePair.Key, keyValuePair.Value);
				if (++num < this.properties.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		internal static TrackingError CreateFromWSMessage(ArrayOfTrackingPropertiesType propertyBag)
		{
			string value = string.Empty;
			string value2 = string.Empty;
			string value3 = string.Empty;
			foreach (TrackingPropertyType trackingPropertyType in propertyBag.Items)
			{
				if (string.Equals(trackingPropertyType.Name, "ErrorCode", StringComparison.OrdinalIgnoreCase))
				{
					value = trackingPropertyType.Value;
				}
				else if (string.Equals(trackingPropertyType.Name, "Server", StringComparison.OrdinalIgnoreCase))
				{
					value2 = trackingPropertyType.Value;
				}
				else if (string.Equals(trackingPropertyType.Name, "Domain", StringComparison.OrdinalIgnoreCase))
				{
					value3 = trackingPropertyType.Value;
				}
			}
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value2) || string.IsNullOrEmpty(value3))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Error is not well formed. One or more of errorCode, server or domain is missing", new object[0]);
				return null;
			}
			TrackingError trackingError = new TrackingError(propertyBag);
			TraceWrapper.SearchLibraryTracer.TraceDebug<TrackingError>(0, "Decoded error: {0}", trackingError);
			return trackingError;
		}

		internal string ToErrorMessage(bool isMultiMessageSearch, out ErrorCodeInformationAttribute errorCodeInfo, out ErrorCode errorCode)
		{
			errorCodeInfo = null;
			string result;
			if (!EnumValidator<Microsoft.Exchange.InfoWorker.Common.MessageTracking.ErrorCode>.TryParse(this.ErrorCode, EnumParseOptions.Default, out errorCode))
			{
				result = (isMultiMessageSearch ? Strings.TrackingTransientErrorMultiMessageSearch : Strings.TrackingTransientError);
			}
			else
			{
				if (!EnumAttributeInfo<Microsoft.Exchange.InfoWorker.Common.MessageTracking.ErrorCode, ErrorCodeInformationAttribute>.TryGetValue((int)errorCode, out errorCodeInfo))
				{
					throw new InvalidOperationException(string.Format("{0} not annotated with ErrorCodeInformationAttribute", errorCode));
				}
				result = errorCodeInfo.ErrorFormatter(this, isMultiMessageSearch);
			}
			return result;
		}

		private const string ErrorCodeProperty = "ErrorCode";

		private const string ServerProperty = "Server";

		private const string DomainProperty = "Domain";

		private const string DataProperty = "Data";

		private const string ExceptionProperty = "Exception";

		private const string TargetProperty = "Target";

		private Dictionary<string, string> properties;
	}
}
