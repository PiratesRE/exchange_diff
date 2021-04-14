using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAirSyncResponse
	{
		XmlDocument XmlDocument { get; set; }

		ExDateTime TimeToRespond { get; set; }

		HttpStatusCode HttpStatusCode { get; set; }

		Stream OutputStream { get; }

		string ContentType { get; set; }

		bool IsClientConnected { get; }

		bool IsErrorResponse { get; set; }

		StatusCode AirSyncStatus { get; set; }

		void IssueWbXmlResponse();

		void BuildMultiPartWbXmlResponse(XmlDocument xmlResponse, Stream stream);

		void IssueErrorResponse(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode);

		void SetErrorResponse(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode);

		void Clear();

		List<string> GetHeaderValues(string headerName);

		string GetHeadersAsString();

		void TraceHeaders();

		void AppendHeader(string name, string value, bool allowDuplicateHeader);

		void AppendHeader(string name, string value);

		void AppendToLog(string param);
	}
}
