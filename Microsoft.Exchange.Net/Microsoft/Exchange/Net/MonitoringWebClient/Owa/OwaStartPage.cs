using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaStartPage : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public OwaStartPage StartPage { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaStartPage;
			}
		}

		public OwaStartPage(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			List<string> hostNames = this.session.GetHostNames(RequestTarget.LiveIdBusiness);
			foreach (string domain in hostNames)
			{
				Cookie cookie = new Cookie("MSPBack", "0", "/", domain);
				this.session.CookieContainer.Add(cookie);
			}
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), RedirectionOptions.FollowUntilNo302OrSpecificRedirection, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.OwaResponseReceived), tempResult);
			}, new Dictionary<string, object>
			{
				{
					"LastExpectedRedirection",
					new string[]
					{
						"errorfe.aspx"
					}
				}
			});
		}

		private void OwaResponseReceived(IAsyncResult result)
		{
			object obj = this.session.EndGetFollowingRedirections<object>(result, delegate(HttpWebResponseWrapper response)
			{
				List<string> hostNames = this.session.GetHostNames(RequestTarget.LiveIdBusiness);
				if (response.Request.RequestUri.ToString().IndexOf("login.srf", StringComparison.OrdinalIgnoreCase) >= 0 && hostNames.ContainsMatchingSuffix(response.Request.RequestUri.Host))
				{
					return response;
				}
				OwaLanguageSelectionPage result2;
				if (OwaLanguageSelectionPage.TryParse(response, out result2))
				{
					return result2;
				}
				return OwaStartPage.Parse(response);
			});
			if (!(obj is HttpWebResponseWrapper))
			{
				this.ParseOwaResponseReceived(obj);
				return;
			}
			HttpWebResponseWrapper httpWebResponseWrapper = obj as HttpWebResponseWrapper;
			string text = ParsingUtility.ParseFormAction(httpWebResponseWrapper);
			string text2 = null;
			Dictionary<string, string> dictionary = ParsingUtility.ParseHiddenFields(httpWebResponseWrapper);
			if (dictionary.Count > 0)
			{
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					if (text2 == null)
					{
						text2 = string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value);
					}
					else
					{
						text2 = string.Format("&{0}={1}", keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			if (text == null || text2 == null)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingOwaStartPage(string.Format("{0},{1}", "Target URL", "POST data")), httpWebResponseWrapper.Request, httpWebResponseWrapper, "Single Namespace Auth");
			}
			this.session.BeginPostFollowingRedirections(this.Id, text, RequestBody.Format("{0}", new object[]
			{
				text2
			}), "application/x-www-form-urlencoded", null, RedirectionOptions.FollowUntilNo302OrSpecificRedirection, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.OwaFinalResponseReceived), tempResult);
			}, new Dictionary<string, object>
			{
				{
					"LastExpectedRedirection",
					new string[]
					{
						"errorfe.aspx"
					}
				}
			});
		}

		private void OwaFinalResponseReceived(IAsyncResult result)
		{
			object resultingPage = this.session.EndPostFollowingRedirections<object>(result, delegate(HttpWebResponseWrapper response)
			{
				OwaLanguageSelectionPage result2;
				if (OwaLanguageSelectionPage.TryParse(response, out result2))
				{
					return result2;
				}
				return OwaStartPage.Parse(response);
			});
			this.ParseOwaResponseReceived(resultingPage);
		}

		private void ParseOwaResponseReceived(object resultingPage)
		{
			if (resultingPage is OwaStartPage)
			{
				this.StartPage = (resultingPage as OwaStartPage);
				if (this.StartPage.FinalUri != null && this.StartPage.FinalUri != this.Uri)
				{
					this.Uri = this.StartPage.FinalUri;
				}
				this.StoreCanary();
				base.ExecutionCompletedSuccessfully();
				return;
			}
			OwaLanguageSelectionPage owaLanguageSelectionPage = resultingPage as OwaLanguageSelectionPage;
			if (owaLanguageSelectionPage.FinalUri != null && owaLanguageSelectionPage.FinalUri != this.Uri)
			{
				this.Uri = new Uri(owaLanguageSelectionPage.FinalUri, this.Uri.PathAndQuery);
			}
			RequestBody body = RequestBody.Format("lcid={0}&tzid={1}", new object[]
			{
				RequestBody.RequestBodyItemWrapper.Create("1033", true),
				RequestBody.RequestBodyItemWrapper.Create("Pacific Standard Time", true)
			});
			this.session.BeginPost(this.Id, new Uri(this.Uri, owaLanguageSelectionPage.PostTarget).ToString(), body, "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LanguageSelectionPostResponseReceived), resultTemp);
			}, null);
		}

		private void LanguageSelectionPostResponseReceived(IAsyncResult result)
		{
			OwaStartPage startPage = null;
			bool flag = this.session.EndPost<bool>(result, delegate(HttpWebResponseWrapper response)
			{
				if (response.StatusCode == HttpStatusCode.Found)
				{
					return false;
				}
				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					throw new PassiveDatabaseException(MonitoringWebClientStrings.PassiveDatabase, response.Request, response, "404 on language selection");
				}
				startPage = OwaStartPage.Parse(response);
				return true;
			});
			if (flag)
			{
				this.StartPage = startPage;
				this.StoreCanary();
				base.ExecutionCompletedSuccessfully();
				return;
			}
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.OwaResponseReceivedAfterLanguagePost), tempResult);
			}, null);
		}

		private void OwaResponseReceivedAfterLanguagePost(IAsyncResult result)
		{
			this.StartPage = this.session.EndGetFollowingRedirections<OwaStartPage>(result, (HttpWebResponseWrapper response) => OwaStartPage.Parse(response));
			this.StoreCanary();
			base.ExecutionCompletedSuccessfully();
		}

		private void StoreCanary()
		{
			Cookie cookie = this.session.CookieContainer.GetCookies(this.Uri)["X-OWA-CANARY"];
			if (cookie != null)
			{
				this.session.PersistentHeaders.Add("x-owa-canary", cookie.Value);
			}
		}

		private const TestId ID = TestId.OwaStartPage;
	}
}
