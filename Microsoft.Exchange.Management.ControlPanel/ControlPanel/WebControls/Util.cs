using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public static class Util
	{
		static Util()
		{
			try
			{
				Util.isMicrosoftHostedOnly = Datacenter.IsMicrosoftHostedOnly(true);
			}
			catch (CannotDetermineExchangeModeException)
			{
				Util.isMicrosoftHostedOnly = false;
			}
			try
			{
				Util.isPartnerHostedOnly = Datacenter.IsPartnerHostedOnly(true);
			}
			catch (CannotDetermineExchangeModeException)
			{
				Util.isPartnerHostedOnly = false;
			}
			Util.isDataCenter = Datacenter.IsMultiTenancyEnabled();
		}

		internal static string OwaApplicationVersion
		{
			get
			{
				return AssemblyUtil.OwaAppVersion;
			}
		}

		internal static string ApplicationVersion
		{
			get
			{
				if (Util.applicationVersion == null)
				{
					Util.applicationVersion = typeof(Util).GetApplicationVersion();
				}
				return Util.applicationVersion;
			}
		}

		internal static bool IsDataCenter
		{
			get
			{
				return Util.isDataCenter;
			}
		}

		internal static bool IsMicrosoftHostedOnly
		{
			get
			{
				return Util.isMicrosoftHostedOnly;
			}
		}

		internal static bool IsPartnerHostedOnly
		{
			get
			{
				return Util.isPartnerHostedOnly;
			}
		}

		public static string CultureName
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture.Name;
			}
		}

		internal static string GetPageTitleFormat(SiteMapNode node)
		{
			if (!Util.isMicrosoftHostedOnly)
			{
				return node["entFormat"];
			}
			return node["liveFormat"];
		}

		internal static string ConvertBoolToYesNo(bool value)
		{
			if (!value)
			{
				return "no";
			}
			return "yes";
		}

		[Conditional("DEBUG")]
		internal static void EnsureScriptManager(Control c)
		{
			Page page = (c as Page) ?? c.Page;
			if (ScriptManager.GetCurrent(page) == null)
			{
				throw new InvalidOperationException(string.Format("The control with ID '{0}' requires a ScriptManager on the page. The ScriptManager must appear before any controls that need it.", c.ID));
			}
		}

		internal static void EndResponse(HttpResponse response, HttpStatusCode statusCode)
		{
			response.Clear();
			response.StatusCode = (int)statusCode;
			response.End();
		}

		internal static EncodingLabel CreateHiddenForSRLabel(string text, string forCtrlId)
		{
			return new EncodingLabel
			{
				ID = forCtrlId + "_label",
				Text = text,
				CssClass = "HiddenForScreenReader",
				AssociatedControlID = forCtrlId
			};
		}

		internal static void MakeControlRbacDisabled(Control c)
		{
			if (c is EcpCollectionEditor)
			{
				((EcpCollectionEditor)c).ReadOnly = true;
			}
			else if (c is InlineEditor)
			{
				((InlineEditor)c).ReadOnly = true;
			}
			else if (c is TextBox)
			{
				((TextBox)c).ReadOnly = true;
				TextBox textBox = (TextBox)c;
				textBox.CssClass += " ReadOnly";
			}
			else if (c is WebControl)
			{
				((WebControl)c).Enabled = false;
			}
			else
			{
				if (!(c is HtmlControl))
				{
					throw new ArgumentException("Cannot make the control readonly");
				}
				((HtmlControl)c).Disabled = true;
			}
			IAttributeAccessor attributeAccessor = c as IAttributeAccessor;
			if (attributeAccessor != null)
			{
				Util.MarkRBACDisabled(attributeAccessor);
			}
			if (c is RadioButton)
			{
				string groupName = ((RadioButton)c).GroupName;
				foreach (RadioButton radioButton in Util.FindControls<RadioButton>(c.NamingContainer))
				{
					if (string.Equals(radioButton.GroupName, groupName, StringComparison.OrdinalIgnoreCase) && radioButton != c)
					{
						radioButton.Enabled = false;
						Util.MarkRBACDisabled(radioButton);
					}
				}
			}
		}

		internal static IEnumerable<T> FindControls<T>(Control parent) where T : Control
		{
			return Util.FindControls(parent, (Control x) => x is T).Cast<T>();
		}

		internal static IEnumerable<Control> FindControls(Control parent, Predicate<Control> predicate)
		{
			Stack<Control> stack = new Stack<Control>();
			stack.Push(parent);
			while (stack.Count > 0)
			{
				Control c = stack.Pop();
				if (c.HasControls())
				{
					foreach (object obj in c.Controls)
					{
						Control item = (Control)obj;
						stack.Push(item);
					}
				}
				if (predicate(c))
				{
					yield return c;
				}
			}
			yield break;
		}

		internal static void MarkRBACDisabled(IAttributeAccessor attributeAccessor)
		{
			attributeAccessor.SetAttribute("rbacDisabled", "true");
		}

		internal static bool IsChrome()
		{
			return HttpContext.Current.Request.Browser.IsBrowser("Chrome");
		}

		internal static bool IsSafari()
		{
			return HttpContext.Current.Request.Browser.IsBrowser("Safari");
		}

		internal static bool IsIE()
		{
			return (HttpContext.Current.Request.UserAgent != null && HttpContext.Current.Request.UserAgent.IndexOf("Trident", StringComparison.OrdinalIgnoreCase) > -1) || HttpContext.Current.Request.Browser.IsBrowser("IE");
		}

		internal static bool IsFirefox()
		{
			return HttpContext.Current.Request.Browser.IsBrowser("Firefox");
		}

		public static void RenderCultureName(Control ctrl)
		{
			ctrl.Page.Response.Output.Write(Util.CultureName);
		}

		internal static string GetLCID()
		{
			return Thread.CurrentThread.CurrentUICulture.LCID.ToString("X");
		}

		internal static string GetLCIDInDecimal()
		{
			return Thread.CurrentThread.CurrentUICulture.LCID.ToString("d");
		}

		public static void RenderLCIDInDecimal(Control ctrl)
		{
			ctrl.Page.Response.Output.Write(Util.GetLCIDInDecimal());
		}

		public static void RenderLCID(Control ctrl)
		{
			ctrl.Page.Response.Output.Write(Util.GetLCID());
		}

		public static void RenderLocStringAndLCID(Control ctrl, string stringId)
		{
			string @string = EcpGlobalResourceProvider.ResourceManager.GetString(stringId, Thread.CurrentThread.CurrentUICulture);
			ctrl.Page.Response.Output.Write(@string, Util.GetLCID());
		}

		public static void RenderLocStringAndLCIDForOwaOption(Control ctrl, string stringId)
		{
			string @string = EcpGlobalResourceProvider.OwaOptionResourceManager.GetString(stringId, Thread.CurrentThread.CurrentUICulture);
			ctrl.Page.Response.Output.Write(@string, Util.GetLCID());
		}

		internal static void NotifyOWAUserSettingsChanged(UserSettings userSettings)
		{
			HttpCookieCollection cookies = HttpContext.Current.Response.Cookies;
			string name = "EcpUpdatedUserSettings";
			int num = (int)userSettings;
			cookies.Add(new HttpCookie(name, num.ToString())
			{
				HttpOnly = false
			});
		}

		internal static int GetMaxLengthFromDefinition(ProviderPropertyDefinition propDefinition)
		{
			int num = int.MaxValue;
			foreach (PropertyDefinitionConstraint propertyDefinitionConstraint in propDefinition.AllConstraints)
			{
				StringLengthConstraint stringLengthConstraint = propertyDefinitionConstraint as StringLengthConstraint;
				if (stringLengthConstraint != null && stringLengthConstraint.MaxLength < num)
				{
					num = stringLengthConstraint.MaxLength;
				}
			}
			return num;
		}

		public static string GetSpriteImageSrc(Control c)
		{
			return ThemeResource.Private_GetThemeResource(c, "clear1x1.gif");
		}

		public static void RequireUpdateProgressPopUp(Control control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			control.Init += delegate(object param0, EventArgs param1)
			{
				control.Page.InitComplete += delegate(object param0, EventArgs param1)
				{
					UpdateProgressPopUp.GetCurrent(control.Page);
				};
			};
		}

		public static void RenderXDomainChecker(HttpResponse response)
		{
			if (!Util.IsDataCenter && response.Headers["X-Frame-Options"] == "SameOrigin")
			{
				response.Headers.Remove("X-Frame-Options");
				response.Write("<script type=\"text/javascript\"><!-- if (window.self != window.top) { try { var parentLocation = window.parent.location.toString(); } catch(e) { window.location = \"/ecp/error.aspx?cause=DenyCrossDomainHost\"; } } --></script>");
			}
		}

		internal const string EcpUpdatedUserSettingsKey = "EcpUpdatedUserSettings";

		internal const string DoNothingScript = "javascript:return false;";

		private const string RbacDisabled = "rbacDisabled";

		private static readonly bool isDataCenter;

		private static readonly bool isMicrosoftHostedOnly;

		private static readonly bool isPartnerHostedOnly;

		private static string applicationVersion;
	}
}
