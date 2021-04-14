using System;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal sealed class FbaWebSession : WebSession
	{
		public FbaWebSession(Uri loginUrl, NetworkCredential credentials) : base(loginUrl, credentials)
		{
		}

		public override void Initialize()
		{
			base.ServiceAuthority.ToString();
			FbaWebSession.AuthenticationRedirectResponse authenticationRedirectResponse = base.Get<FbaWebSession.AuthenticationRedirectResponse>(base.ServiceAuthority, (HttpWebResponse response) => new FbaWebSession.AuthenticationRedirectResponse(response));
			FbaWebSession.FbaLogonPage fbaLogonPage = base.Get<FbaWebSession.FbaLogonPage>(new Uri(authenticationRedirectResponse.RedirectUrl), (HttpWebResponse response) => FbaWebSession.FbaLogonPage.Create(response, base.ServiceAuthority, base.Credentials));
			base.Post<FbaWebSession.AuthenticationRedirectResponse>(fbaLogonPage.FbaUrl, fbaLogonPage.PostForm, (HttpWebResponse response) => new FbaWebSession.AuthenticationRedirectResponse(response));
		}

		protected override void Authenticate(HttpWebRequest request)
		{
		}

		private abstract class FbaLogonPage : TextResponse
		{
			public static FbaWebSession.FbaLogonPage Create(HttpWebResponse response, Uri appUrl, NetworkCredential credential)
			{
				FbaWebSession.FbaLogonPage fbaLogonPage = string.IsNullOrEmpty(response.Headers["X-OWA-Version"]) ? new FbaWebSession.IsaLogonPage() : new FbaWebSession.OwaLogonPage();
				fbaLogonPage.SetResponse(response);
				if (!fbaLogonPage.Contains("57A118C6-2DA9-419d-BE9A-F92B0F9A418B"))
				{
					throw new AuthenticationException();
				}
				fbaLogonPage.ServiceAuthority = appUrl;
				fbaLogonPage.Credential = credential;
				return fbaLogonPage;
			}

			public virtual HtmlFormBody PostForm
			{
				get
				{
					string text = this.Credential.UserName;
					if (text.Length > 20)
					{
						text = text.Substring(0, 20);
					}
					if (!string.IsNullOrEmpty(this.Credential.Domain))
					{
						text = this.Credential.Domain + "\\" + text;
					}
					return new HtmlFormBody
					{
						{
							"flags",
							"0"
						},
						{
							"username",
							text
						},
						{
							"password",
							this.Credential.Password
						}
					};
				}
			}

			public abstract Uri FbaUrl { get; }

			private protected Uri ServiceAuthority { protected get; private set; }

			private protected NetworkCredential Credential { protected get; private set; }

			private const string FbaLogonPageGuid = "57A118C6-2DA9-419d-BE9A-F92B0F9A418B";

			private const string OwaVersionHeaderName = "X-OWA-Version";

			private const int LogonUserNameMaxLength = 20;
		}

		private class OwaLogonPage : FbaWebSession.FbaLogonPage
		{
			public override HtmlFormBody PostForm
			{
				get
				{
					HtmlFormBody postForm = base.PostForm;
					postForm.Add("destination", base.ServiceAuthority.ToString());
					postForm.Add("trusted", "0");
					return postForm;
				}
			}

			public override Uri FbaUrl
			{
				get
				{
					return new Uri(base.ServiceAuthority, "/owa/auth/owaauth.dll");
				}
			}

			private const string OwaFbaUrlPath = "/owa/auth/owaauth.dll";
		}

		private class IsaLogonPage : FbaWebSession.FbaLogonPage
		{
			public override HtmlFormBody PostForm
			{
				get
				{
					HtmlFormBody postForm = base.PostForm;
					postForm.Add("curl", base.ServiceAuthority.ToString());
					postForm.Add("trusted", "4");
					postForm.Add("formdir", "");
					postForm.Add("SubmitCreds", "Log On");
					return postForm;
				}
			}

			public override Uri FbaUrl
			{
				get
				{
					return new UriBuilder(base.ServiceAuthority)
					{
						Path = "/CookieAuth.dll",
						Query = "Logon"
					}.Uri;
				}
			}

			private const string IsaFbaUrlPath = "/CookieAuth.dll";

			private const string IsaFbaUrlQuery = "Logon";
		}

		private class AuthenticationRedirectResponse : RedirectResponse
		{
			public AuthenticationRedirectResponse(HttpWebResponse response) : base(response)
			{
				if (!base.IsRedirect)
				{
					throw new AuthenticationException();
				}
			}
		}
	}
}
