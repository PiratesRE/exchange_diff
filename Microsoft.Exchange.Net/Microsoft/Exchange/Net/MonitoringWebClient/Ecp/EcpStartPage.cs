using System;
using System.Net;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpStartPage : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public Uri CdnStaticFileUri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.EcpStartPage;
			}
		}

		public override object Result
		{
			get
			{
				return this.Uri;
			}
		}

		public EcpStartPage(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.EcpResponseReceived), tempResult);
			}, null);
		}

		private void EcpResponseReceived(IAsyncResult result)
		{
			object obj = this.session.EndGetFollowingRedirections<object>(result, delegate(HttpWebResponseWrapper response)
			{
				OwaLanguageSelectionPage result2;
				if (OwaLanguageSelectionPage.TryParse(response, out result2))
				{
					return result2;
				}
				LiveIdCompactTokenPage result3;
				if (LiveIdCompactTokenPage.TryParse(response, out result3))
				{
					return result3;
				}
				return EcpStartPage.Parse(response);
			});
			if (obj is EcpStartPage)
			{
				EcpStartPage ecpStartPage = obj as EcpStartPage;
				if (ecpStartPage.FinalUri != null && ecpStartPage.FinalUri != this.Uri)
				{
					this.Uri = ecpStartPage.FinalUri;
				}
				this.CdnStaticFileUri = ecpStartPage.StaticFileUri;
				base.ExecutionCompletedSuccessfully();
				return;
			}
			if (obj is LiveIdCompactTokenPage)
			{
				LiveIdCompactTokenPage liveIdCompactTokenPage = obj as LiveIdCompactTokenPage;
				this.session.BeginPost(this.Id, liveIdCompactTokenPage.PostUrl, RequestBody.Format(liveIdCompactTokenPage.HiddenFieldsString, new object[0]), "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.CompactTicketPostResponseRecived), resultTemp);
				}, null);
				return;
			}
			OwaLanguageSelectionPage owaLanguageSelectionPage = obj as OwaLanguageSelectionPage;
			if (owaLanguageSelectionPage.FinalUri != null && owaLanguageSelectionPage.FinalUri != this.Uri)
			{
				this.Uri = owaLanguageSelectionPage.FinalUri;
			}
			RequestBody body = RequestBody.Format("lcid={0}&tzid={1}&destination={2}", new object[]
			{
				RequestBody.RequestBodyItemWrapper.Create("1033", true),
				RequestBody.RequestBodyItemWrapper.Create("Pacific Standard Time", true),
				RequestBody.RequestBodyItemWrapper.Create(owaLanguageSelectionPage.Destination, true)
			});
			this.session.BeginPost(this.Id, new Uri(this.Uri, owaLanguageSelectionPage.PostTarget).ToString(), body, "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LanguageSelectionPostResponseReceived), resultTemp);
			}, null);
		}

		private void LanguageSelectionPostResponseReceived(IAsyncResult result)
		{
			var <>f__AnonymousType = this.session.EndPost(result, (HttpWebResponseWrapper response) => new
			{
				StatusCode = response.StatusCode,
				Location = response.Headers["Location"],
				Response = response
			});
			if (<>f__AnonymousType.StatusCode == HttpStatusCode.Found)
			{
				this.session.BeginGet(this.Id, new Uri(this.Uri, <>f__AnonymousType.Location).ToString(), delegate(IAsyncResult resultTemp)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.LanguageSelectionPostAndRedirectedResponseReceived), resultTemp);
				}, null);
				return;
			}
			throw new ScenarioException(MonitoringWebClientStrings.NoRedirectToEcpAfterLanguageSelection, null, RequestTarget.Ecp, FailureReason.UnexpectedHttpResponseCode, FailingComponent.Ecp, "MissingStartPageAfterLanguageSelection");
		}

		private void LanguageSelectionPostAndRedirectedResponseReceived(IAsyncResult result)
		{
			EcpStartPage ecpStartPage = this.session.EndGet<EcpStartPage>(result, (HttpWebResponseWrapper response) => EcpStartPage.Parse(response));
			if (ecpStartPage.FinalUri != null && ecpStartPage.FinalUri != this.Uri)
			{
				this.Uri = ecpStartPage.FinalUri;
			}
			this.CdnStaticFileUri = ecpStartPage.StaticFileUri;
			base.ExecutionCompletedSuccessfully();
		}

		private void CompactTicketPostResponseRecived(IAsyncResult result)
		{
			Uri uri = this.session.EndPost<Uri>(result, delegate(HttpWebResponseWrapper response)
			{
				if (response.StatusCode != HttpStatusCode.Found)
				{
					return response.Request.RequestUri;
				}
				return new Uri(response.Headers["Location"]);
			});
			if (uri != null && uri != this.Uri)
			{
				this.Uri = uri;
			}
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.EcpResponseReceived), tempResult);
			}, null);
		}

		private const TestId ID = TestId.EcpStartPage;
	}
}
