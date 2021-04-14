using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebUIProbe
{
	public class WebUIProbe : ProbeWorkItem
	{
		public WebUIProbe()
		{
			this.debugData = new StringBuilder();
			this.executionContext = new StringBuilder();
			this.captures = new List<ResponseCapture>();
			this.requestedUrls = new List<string>();
		}

		internal WebUIProbeWorkDefinition WorkDefinition
		{
			get
			{
				return this.workDefinition;
			}
			set
			{
				this.workDefinition = value;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				if (this.workDefinition == null)
				{
					this.workDefinition = new WebUIProbeWorkDefinition(base.Definition.ExtensionAttributes);
				}
				RemoteCertificateValidationCallback callback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
				CertificateValidationManager.RegisterCallback("WebUIProbe", callback);
				IHttpRequest httpRequest = this.GetHttpRequest();
				httpRequest.ProcessCookies = this.workDefinition.ProcessCookies;
				foreach (EndPoint endPoint in this.workDefinition.EndPoints)
				{
					PostData postData = this.ReplacePostDataVariablesWithCaptures(endPoint.PostData);
					string text = this.ReplaceVariablesWithCaptures(endPoint.Uri);
					int timeout = 0;
					if (endPoint.PageLoadTimeout != TimeSpan.MaxValue)
					{
						timeout = Convert.ToInt32(endPoint.PageLoadTimeout.TotalMilliseconds) + 5000;
					}
					this.requestedUrls.Add(string.Format("{0}:{1}", endPoint.Method.ToString(), text));
					if (dateTime == DateTime.MinValue)
					{
						dateTime = DateTime.UtcNow;
					}
					ServerResponse serverResponse;
					switch (endPoint.Method)
					{
					case RequestMethod.Get:
						goto IL_16B;
					case RequestMethod.Post:
						serverResponse = httpRequest.SendPostRequest(text, endPoint.AllowAutoRedirect, endPoint.GetHiddenInputValues, ref postData, endPoint.ContentType, endPoint.FormName, timeout);
						break;
					case RequestMethod.MSLiveLogin:
						serverResponse = this.MSLiveLogin(ref httpRequest, text, ref postData, timeout);
						break;
					default:
						goto IL_16B;
					}
					IL_1A5:
					this.GetCaptureValues(endPoint.Captures, serverResponse);
					this.StoreDebugData(text, endPoint.Uri, endPoint.Method, endPoint.ContentType, postData, endPoint.PostData, serverResponse);
					if (serverResponse.StatusCode != HttpStatusCode.OK)
					{
						if (serverResponse.HeaderText == null || !serverResponse.HeaderText.Contains("Microsoft.Exchange.Data.Directory.SystemConfiguration.OverBudgetException"))
						{
							throw new Exception(string.Format("The response from {0} was not an OK status code. The returned status code was {1}.", text, serverResponse.StatusCode));
						}
						WTFDiagnostics.TraceWarning<StringBuilder>(ExTraceGlobals.HTTPTracer, new TracingContext(), "{0}", this.debugData, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebUI\\Probes\\WebUIProbe.cs", 191);
					}
					if (serverResponse.ResponseTime > endPoint.PageLoadTimeout)
					{
						throw new Exception(string.Format("The response from {0} was not within the expected amount of time ({1}). The actual time was {2}.", text, endPoint.PageLoadTimeout.ToString(), serverResponse.ResponseTime.ToString()));
					}
					foreach (ExpectedResult expectedResult in endPoint.ExpectedResults)
					{
						if (!this.ValidateResult(expectedResult, serverResponse))
						{
							throw new Exception(string.Format("The response from {0} did not contain the expected value of \"{1}\" in the {2}.", text, expectedResult.Value, expectedResult.Type.ToString()));
						}
					}
					continue;
					IL_16B:
					serverResponse = httpRequest.SendGetRequest(text, endPoint.SslValidation, "WebUIProbe", endPoint.AllowAutoRedirect, timeout, endPoint.AuthenticationType, endPoint.AuthenticationUser, endPoint.AuthenticationPassword, endPoint.Properties);
					goto IL_1A5;
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<Exception, StringBuilder>(ExTraceGlobals.HTTPTracer, new TracingContext(), "{0}\r\n{1}", ex, this.debugData, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebUI\\Probes\\WebUIProbe.cs", 231);
				base.Result.FailureContext = ex.ToString();
				throw;
			}
			finally
			{
				base.Result.SampleValue = ((dateTime == DateTime.MinValue) ? 0.0 : Convert.ToDouble((DateTime.UtcNow - dateTime).TotalMilliseconds));
				base.Result.ExecutionContext = this.executionContext.ToString();
				base.Result.StateAttribute2 = string.Join(Environment.NewLine, this.requestedUrls.ToArray().Reverse<string>());
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HTTPTracer, new TracingContext(), "All end points have been successfully evaluated and validated.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebUI\\Probes\\WebUIProbe.cs", 245);
		}

		protected virtual IHttpRequest GetHttpRequest()
		{
			return new HttpRequest();
		}

		private static string GetNextPageUrl(string response)
		{
			string text = HtmlUtility.GetJavaScriptVariableValue("var srf_uPost", response);
			if (string.IsNullOrWhiteSpace(text))
			{
				MatchCollection matchCollection = Regex.Matches(response, "(http|ftp|https):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?");
				foreach (object obj in matchCollection)
				{
					Match match = (Match)obj;
					string value = match.Value;
					if (!string.IsNullOrWhiteSpace(value) && value.IndexOf("ppsecure/post.srf?") > 0)
					{
						text = match.Value;
						break;
					}
				}
			}
			return text;
		}

		private bool ValidateResult(ExpectedResult expectedResult, ServerResponse serverResponse)
		{
			string text;
			switch (expectedResult.Type)
			{
			case ExpectedResultType.Title:
				text = HtmlUtility.GetTagInnerHtml("title", serverResponse.Text, null);
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Trim();
					goto IL_53;
				}
				goto IL_53;
			case ExpectedResultType.Url:
				text = serverResponse.Uri.ToString();
				goto IL_53;
			}
			text = serverResponse.Text;
			IL_53:
			string text2 = this.ReplaceVariablesWithCaptures(expectedResult.Value);
			if (expectedResult.IsRegEx)
			{
				Regex regex = new Regex(text2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				return regex.IsMatch(text);
			}
			return string.Equals(text2, text, StringComparison.InvariantCultureIgnoreCase);
		}

		private ServerResponse MSLiveLogin(ref IHttpRequest request, string uri, ref PostData postData, int timeout)
		{
			ServerResponse serverResponse = request.SendGetRequest(uri, true, string.Empty, true, timeout, null, null, null, null);
			this.StoreDebugData(uri, null, RequestMethod.Get, string.Empty, postData, null, serverResponse);
			Dictionary<string, string> hiddenFormInputs = HtmlUtility.GetHiddenFormInputs(serverResponse.Text);
			if (!hiddenFormInputs.ContainsKey("PPFT"))
			{
				throw new Exception(string.Format("Unable to log into MS Live. {0} field could not be found in the server response.", "PPFT"));
			}
			postData.Add("PPFT", hiddenFormInputs["PPFT"]);
			string text = WebUIProbe.GetNextPageUrl(serverResponse.Text);
			this.VerifyMSLiveLoginNextStep(text, serverResponse.Text);
			serverResponse = request.SendPostRequest(text, true, false, ref postData, "application/x-www-form-urlencoded", null, timeout);
			this.StoreDebugData(uri, null, RequestMethod.Post, "application/x-www-form-urlencoded", postData, null, serverResponse);
			hiddenFormInputs = HtmlUtility.GetHiddenFormInputs(serverResponse.Text);
			text = HtmlUtility.GetNamedRegExCapture("<form[^>]+?name=\"[A-Za-z0-9]*\"[^>]+?action=\"(?<Value>[^\"]*)\"", serverResponse.Text, "Value", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			this.VerifyMSLiveLoginNextStep(text, serverResponse.Text);
			postData = new PostData(hiddenFormInputs);
			if (postData.ContainsKey("wresult"))
			{
				serverResponse = request.SendPostRequest(text, true, false, ref postData, "application/x-www-form-urlencoded", null, timeout);
				this.StoreDebugData(uri, null, RequestMethod.Post, "application/x-www-form-urlencoded", postData, null, serverResponse);
				text = HtmlUtility.GetNamedRegExCapture("<form[^>]+?name=\"[A-Za-z0-9]*\"[^>]+?action=\"(?<Value>[^\"]*)\"", serverResponse.Text, "Value", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				this.VerifyMSLiveLoginNextStep(text, serverResponse.Text);
				hiddenFormInputs = HtmlUtility.GetHiddenFormInputs(serverResponse.Text);
				postData = new PostData(hiddenFormInputs);
				text = HtmlUtility.GetNamedRegExCapture("<form[^>]+?name=\"[A-Za-z0-9]*\"[^>]+?action=\"(?<Value>[^\"]*)\"", serverResponse.Text, "Value", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				this.VerifyMSLiveLoginNextStep(text, serverResponse.Text);
			}
			serverResponse = request.SendPostRequest(text, true, false, ref postData, "application/x-www-form-urlencoded", null, timeout);
			if (Path.GetFileName(serverResponse.Uri.AbsolutePath).Equals("languageselection.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				this.StoreDebugData(text, null, RequestMethod.Post, "application/x-www-form-urlencoded", postData, null, serverResponse);
				text = HtmlUtility.GetNamedRegExCapture("<form[^>]+?action=(?<Value>[^ ]*)", serverResponse.Text, "Value", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				if (string.IsNullOrEmpty(text))
				{
					throw new Exception("Unable to find the page to post to on the language/time zone selection page.");
				}
				text = string.Format("{0}://{1}/{2}/{3}", new object[]
				{
					serverResponse.Uri.Scheme,
					serverResponse.Uri.Host,
					Path.GetDirectoryName(serverResponse.Uri.AbsolutePath).Trim(new char[]
					{
						Path.DirectorySeparatorChar
					}),
					text
				});
				hiddenFormInputs = HtmlUtility.GetHiddenFormInputs(serverResponse.Text);
				postData = new PostData(hiddenFormInputs);
				postData.Add("lcid", "1033");
				postData.Add("tzid", "Pacific Standard Time");
				this.requestedUrls.Add(string.Format("Post:{0}", text));
				serverResponse = request.SendPostRequest(text, true, false, ref postData, "application/x-www-form-urlencoded", null, timeout);
			}
			return serverResponse;
		}

		private void VerifyMSLiveLoginNextStep(string url, string responseText)
		{
			this.requestedUrls.Add(string.Format("Post:{0}", url));
			base.Result.StateAttribute1 = responseText;
			if (string.IsNullOrEmpty(url))
			{
				string text = "Unable to log into MS Live. The form containing the next URL could not be found in the server response.";
				string javaScriptVariableValue = HtmlUtility.GetJavaScriptVariableValue("var srf_sErr", responseText);
				if (!string.IsNullOrEmpty(javaScriptVariableValue))
				{
					text = text + " The following error information was found on the page: " + javaScriptVariableValue;
				}
				throw new Exception(text);
			}
		}

		private void GetCaptureValues(ICollection<ResponseCapture> captures, ServerResponse response)
		{
			if (captures != null)
			{
				foreach (ResponseCapture responseCapture in captures)
				{
					string namedRegExCapture = HtmlUtility.GetNamedRegExCapture(responseCapture.Pattern, (responseCapture.Type == CaptureType.ResponseText) ? response.Text : response.Uri.ToString(), responseCapture.Name, RegexOptions.IgnoreCase | RegexOptions.Singleline);
					if (!string.IsNullOrEmpty(namedRegExCapture))
					{
						responseCapture.Value = namedRegExCapture;
					}
					this.captures.Add(responseCapture);
				}
			}
		}

		private PostData ReplacePostDataVariablesWithCaptures(PostData postData)
		{
			if (postData == null)
			{
				return null;
			}
			PostData postData2 = new PostData();
			foreach (KeyValuePair<string, string> keyValuePair in postData)
			{
				string value = this.ReplaceVariablesWithCaptures(keyValuePair.Value);
				postData2.Add(keyValuePair.Key, value);
			}
			return postData2;
		}

		private string ReplaceVariablesWithCaptures(string text)
		{
			foreach (ResponseCapture responseCapture in this.captures)
			{
				text = text.Replace("{" + responseCapture.Name + "}", responseCapture.Value);
			}
			return text;
		}

		private void StoreDebugData(string uri, string originalUri, RequestMethod requestMethod, string requestContentType, PostData postData, PostData originalPostData, ServerResponse response)
		{
			bool formEncoded = requestContentType != null && requestContentType.Equals("application/x-www-form-urlencoded", StringComparison.InvariantCultureIgnoreCase);
			string[] source = response.HeaderText.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.RemoveEmptyEntries);
			IEnumerable<string> values = from header in source
			where header.StartsWith("PPServer", StringComparison.InvariantCultureIgnoreCase) || header.StartsWith("X-", StringComparison.InvariantCultureIgnoreCase)
			select header;
			base.Result.StateAttribute3 = string.Join(Environment.NewLine, values) + base.Result.StateAttribute3;
			int num = Math.Min(5120, response.Text.Length);
			int num2 = 1;
			int i = 0;
			while (i < num)
			{
				if (num2 == 2)
				{
					num2 = 11;
				}
				int num3 = Math.Min(1024, num - i);
				PropertyInfo property = base.Result.GetType().GetProperty("StateAttribute" + num2, typeof(string));
				if (property != null)
				{
					property.SetValue(base.Result, response.Text.Substring(i, num3), null);
				}
				i += num3;
				num2++;
			}
			this.debugData.AppendFormat("{0}: {1}", requestMethod, uri);
			this.executionContext.AppendFormat("{0}: {1} ", requestMethod, uri);
			if (!string.IsNullOrEmpty(originalUri) && !uri.Equals(originalUri, StringComparison.InvariantCultureIgnoreCase))
			{
				this.debugData.AppendFormat(" originally: {0}", originalUri);
			}
			this.debugData.AppendLine();
			if (postData != null)
			{
				string value = postData.ToString(formEncoded);
				this.debugData.AppendFormat("Post data (as {0}):\r\n{1}", requestContentType, postData.ToString(formEncoded));
				this.executionContext.AppendFormat("Post: {0} ", postData.ToString());
				if (originalPostData != null)
				{
					string text = originalPostData.ToString(formEncoded);
					if (!text.Equals(value, StringComparison.InvariantCultureIgnoreCase))
					{
						this.debugData.AppendFormat(" originally: {0}", text);
					}
				}
				this.debugData.AppendLine();
			}
			this.debugData.AppendLine("Server response: ");
			this.debugData.AppendLine(response.ToString());
			this.debugData.AppendLine("Captures: ");
			foreach (ResponseCapture responseCapture in this.captures)
			{
				this.debugData.AppendFormat("{0}: {1}, ", responseCapture.Name, responseCapture.Value);
				if (!string.IsNullOrEmpty(responseCapture.PersistInStateAttributeName))
				{
					PropertyInfo property2 = base.Result.GetType().GetProperty(responseCapture.PersistInStateAttributeName, typeof(string));
					if (property2 != null)
					{
						property2.SetValue(base.Result, responseCapture.Value, null);
					}
					else
					{
						this.debugData.AppendFormat("Cannot persist capture '{0}'. StateAttribute property '{1}' does not exist or its data type is not a string.", responseCapture.Name, responseCapture.PersistInStateAttributeName);
					}
				}
			}
		}

		private const string ContentTypeEncodedForm = "application/x-www-form-urlencoded";

		private const string ComponentId = "WebUIProbe";

		private const string OverBudgetExceptionEcpError = "Microsoft.Exchange.Data.Directory.SystemConfiguration.OverBudgetException";

		private WebUIProbeWorkDefinition workDefinition;

		private List<ResponseCapture> captures;

		private StringBuilder debugData;

		private StringBuilder executionContext;

		private List<string> requestedUrls;
	}
}
