using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Security
{
	public class Education : Page
	{
		protected override void OnLoad(EventArgs e)
		{
			string[] userLanguages = HttpContext.Current.Request.UserLanguages;
			if (userLanguages != null)
			{
				int num = Math.Min(5, userLanguages.Length);
				for (int i = 0; i < num; i++)
				{
					string text = Utilities.ValidateLanguageTag(userLanguages[i]);
					if (text != null)
					{
						CultureInfo supportedBrowserLanguage = Utilities.GetSupportedBrowserLanguage(text);
						if (supportedBrowserLanguage != null)
						{
							Thread.CurrentThread.CurrentCulture = supportedBrowserLanguage;
							Thread.CurrentThread.CurrentUICulture = supportedBrowserLanguage;
						}
					}
				}
			}
			this.OnInit(e);
		}

		protected bool IsEducationUrlAvailable
		{
			get
			{
				return !string.IsNullOrEmpty(this.EducationUrl);
			}
		}

		protected string EducationUrl
		{
			get
			{
				return HttpContext.Current.Request.Params[Utilities.EducationUrlParameter];
			}
		}

		protected string DestinationUrlParameter
		{
			get
			{
				return HttpContext.Current.Request.Params[Utilities.DestinationUrlParameter];
			}
		}

		protected string Destination
		{
			get
			{
				string text = this.DestinationUrlParameter;
				string text2 = HttpContext.Current.Request.Params[Utilities.LiveIdUrlParameter];
				if (!string.IsNullOrEmpty(text2))
				{
					string userDomain = Utilities.GetUserDomain(text2);
					if (!string.IsNullOrEmpty(userDomain))
					{
						if (text.IndexOf('?') > 0)
						{
							text = string.Format("{0}&{1}={2}&{3}={4}", new object[]
							{
								text,
								"realm",
								userDomain,
								Utilities.UserNameParameter,
								text2
							});
						}
						else
						{
							text = string.Format("{0}?{1}={2}&{3}={4}", new object[]
							{
								text,
								"realm",
								userDomain,
								Utilities.UserNameParameter,
								text2
							});
						}
					}
				}
				return text;
			}
		}

		protected string UserId
		{
			get
			{
				string text = HttpContext.Current.Request.Params[Utilities.LiveIdUrlParameter];
				if (text == null)
				{
					return string.Empty;
				}
				return text;
			}
		}

		protected string UserDomain
		{
			get
			{
				string text = HttpContext.Current.Request.Params[Utilities.LiveIdUrlParameter];
				if (text == null)
				{
					return string.Empty;
				}
				return Utilities.GetUserDomain(text);
			}
		}

		protected bool ShowWarning
		{
			get
			{
				string text = HttpContext.Current.Request.Params[Utilities.LiveIdUrlParameter];
				if (text == null)
				{
					return false;
				}
				string userDomain = Utilities.GetUserDomain(text);
				return string.IsNullOrEmpty(userDomain) || !SmtpAddress.IsValidDomain(userDomain);
			}
		}

		protected void RenderImage(string imageName)
		{
			string s = Utilities.ImagesPath + imageName;
			base.Response.Write(s);
		}

		protected string EducationMessage
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)4265046865U));
			}
		}

		protected string LiveIdLabel
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)2723658401U));
			}
		}

		protected string GetLiveIdMessage
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)2886870364U));
			}
		}

		protected string InvalidLiveIdWarning
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString(Strings.IDs.InvalidLiveIdWarning));
			}
		}

		protected string OutlookWebAccess
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)3228633421U));
			}
		}

		protected string ConnectedToExchange
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)3976540663U));
			}
		}

		protected string LogonCopyright
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)2308016970U));
			}
		}

		protected string AddToFavorites
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)3266846781U));
			}
		}

		protected string GoThereNowButtonText
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)3147877247U));
			}
		}

		protected string NextButtonText
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)4082986936U));
			}
		}

		protected string WhyMessage
		{
			get
			{
				return Utilities.HtmlEncode(Strings.GetLocalizedString((Strings.IDs)3937131501U));
			}
		}

		protected string HelpLink
		{
			get
			{
				return "http://outlookliveanswers.com/forums/p/6581/20456.aspx#20456";
			}
		}

		protected string logonTopLeftImg = "lgntopl.gif";

		protected string logonTopRightImg = "lgntopr.gif";

		protected string logonBottomLeftImg = "lgnbotl.gif";

		protected string logonBottomRightImg = "lgnbotr.gif";

		protected string exchangeLogoImg = "lgnexlogo.gif";
	}
}
