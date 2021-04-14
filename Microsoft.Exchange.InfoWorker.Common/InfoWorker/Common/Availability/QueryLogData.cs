using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class QueryLogData
	{
		public QueryLogData()
		{
			this.succeededQueries = new Dictionary<string, List<FreeBusyQuery>>();
			this.failedQueries = new Dictionary<string, List<FreeBusyQuery>>();
		}

		public void Add(FreeBusyQuery query, bool isFailed)
		{
			string text = query.Target;
			text = ((text == null) ? "NULL" : text);
			Dictionary<string, List<FreeBusyQuery>> dictionary = isFailed ? this.failedQueries : this.succeededQueries;
			List<FreeBusyQuery> list;
			if (dictionary.TryGetValue(text, out list))
			{
				list.Add(query);
				return;
			}
			list = new List<FreeBusyQuery>();
			list.Add(query);
			dictionary[text] = list;
		}

		public string GetSucceededQueryLogData()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (KeyValuePair<string, List<FreeBusyQuery>> keyValuePair in this.succeededQueries)
			{
				string key = keyValuePair.Key;
				List<FreeBusyQuery> value = keyValuePair.Value;
				stringBuilder.AppendFormat("{0}-{1}|", "Target", key);
				foreach (KeyValuePair<string, string> keyValuePair2 in value[0].LogData)
				{
					stringBuilder.AppendFormat("{0}-{1}|", keyValuePair2.Key, keyValuePair2.Value);
				}
				string text = (value[0].Type != null) ? value[0].Type.Value.ToString() : "NULL";
				stringBuilder.AppendFormat("{0}-{1}|{2}-{3}|", new object[]
				{
					"QT",
					text,
					"Cnt",
					value.Count
				});
				foreach (FreeBusyQuery query in value)
				{
					string intraForestLatency = QueryLogData.GetIntraForestLatency(query);
					if (!string.IsNullOrEmpty(intraForestLatency))
					{
						stringBuilder.AppendFormat("[{0}]", intraForestLatency);
					}
				}
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		public List<string> GetFailedQueryLogData()
		{
			List<string> list = new List<string>(this.failedQueries.Count);
			foreach (KeyValuePair<string, List<FreeBusyQuery>> keyValuePair in this.failedQueries)
			{
				string key = keyValuePair.Key;
				List<FreeBusyQuery> value = keyValuePair.Value;
				StringBuilder stringBuilder = new StringBuilder(1024);
				stringBuilder.AppendFormat("{0}-{1}|", "Target", key);
				foreach (KeyValuePair<string, string> keyValuePair2 in value[0].LogData)
				{
					stringBuilder.AppendFormat("{0}-{1}|", keyValuePair2.Key, keyValuePair2.Value);
				}
				if (value[0].Type != null)
				{
					stringBuilder.AppendFormat("{0}-{1}|{2}-{3}|", new object[]
					{
						"QT",
						value[0].Type.Value,
						"Cnt",
						value.Count
					});
				}
				else
				{
					stringBuilder.AppendFormat("{0}-{1}|", "Cnt", value.Count);
				}
				HashSet<Type> hashSet = new HashSet<Type>();
				foreach (FreeBusyQuery freeBusyQuery in value)
				{
					Exception exceptionInfo = freeBusyQuery.Result.ExceptionInfo;
					Exception ex = exceptionInfo;
					WebException ex2 = exceptionInfo as WebException;
					while (ex.InnerException != null)
					{
						ex = ex.InnerException;
						if (ex2 == null)
						{
							ex2 = (ex as WebException);
						}
					}
					if (!hashSet.Contains(ex.GetType()))
					{
						stringBuilder.Append("[");
						AvailabilityException ex3 = exceptionInfo as AvailabilityException;
						if (ex3 != null)
						{
							stringBuilder.AppendFormat("{0}-{1}|", "EXPS", ex3.ServerName);
							if (!string.IsNullOrEmpty(ex3.LocationIdentifier))
							{
								stringBuilder.AppendFormat("{0}-{1}|", "LID", ex3.LocationIdentifier);
							}
						}
						if (freeBusyQuery.Email != null)
						{
							stringBuilder.AppendFormat("{0}-{1}|", "AT", freeBusyQuery.Email.Address);
						}
						stringBuilder.AppendFormat("{0}-{1}|", "EXPM", exceptionInfo.GetType().Name + ":" + exceptionInfo.Message);
						if (exceptionInfo.GetType() != ex.GetType())
						{
							stringBuilder.AppendFormat("{0}-{1}|", "IEXPM", ex.GetType().Name + ":" + ex.Message);
						}
						hashSet.Add(ex.GetType());
						if (ex2 != null)
						{
							HttpWebResponse httpWebResponse = ex2.Response as HttpWebResponse;
							if (httpWebResponse != null)
							{
								string text = httpWebResponse.Headers.Get(MSDiagnosticsHeader.HeaderName);
								if (text != null)
								{
									stringBuilder.AppendFormat("{0}-{1}|", "DI", text);
								}
								string text2 = httpWebResponse.Headers.Get("request-id");
								if (text2 != null)
								{
									string text3 = httpWebResponse.Headers.Get(WellKnownHeader.XFEServer);
									string text4 = httpWebResponse.Headers.Get(WellKnownHeader.XCalculatedBETarget);
									stringBuilder.AppendFormat("{0}-{1}|{2}-{3}|{4}-{5}", new object[]
									{
										"RID",
										text2,
										"TFE",
										text3,
										"TBE",
										text4
									});
								}
							}
						}
						stringBuilder.Append(QueryLogData.GetIntraForestLatency(freeBusyQuery));
						stringBuilder.Append(']');
					}
				}
				list.Add(stringBuilder.ToString());
			}
			return list;
		}

		private static string GetIntraForestLatency(BaseQuery query)
		{
			string text = string.Empty;
			if (query.Type == RequestType.Local || query.Type == RequestType.IntraSite || query.Type == RequestType.CrossSite)
			{
				if (query.ExchangePrincipalLatency > 0L)
				{
					text = string.Format("{0}-{1}|", "EPL", query.ExchangePrincipalLatency);
				}
				if (query.ServiceDiscoveryLatency > 0L)
				{
					text = string.Format("{0}{1}-{2}|", text, "SDL", query.ServiceDiscoveryLatency);
				}
			}
			return text;
		}

		public const string EmptyTarget = "NULL";

		public const string AutoDInfo = "AutoDInfo";

		private const string Target = "Target";

		private const string Count = "Cnt";

		private const string Attendee = "AT";

		private const string QueryType = "QT";

		private const string ExchangePrincipalLatency = "EPL";

		private const string ServiceDiscoveryLatency = "SDL";

		private const string ExceptionMessage = "EXPM";

		private const string InnerExceptionMessage = "IEXPM";

		private const string ExceptionServer = "EXPS";

		private const string LocationIdentifierKey = "LID";

		private const string RequestId = "RID";

		private const string TargetBE = "TBE";

		private const string TargetFE = "TFE";

		private const string DiagnosticInfo = "DI";

		private const string LogFormat = "{0}-{1}|";

		private const int DefaultLogCapacity = 1024;

		private Dictionary<string, List<FreeBusyQuery>> succeededQueries;

		private Dictionary<string, List<FreeBusyQuery>> failedQueries;
	}
}
