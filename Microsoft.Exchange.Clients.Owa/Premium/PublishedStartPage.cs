using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PublishedStartPage : NavigationHost, IRegistryOnlyForm
	{
		protected override void OnInit(EventArgs e)
		{
			this.navigationModule = this.SelectNavagationModule();
			this.lastModuleApplicationElement = "PublishedFolder";
			this.lastModuleName = this.navigationModule.ToString();
			this.lastModuleContentClass = "IPF.Appointment";
			this.lastModuleContainerId = "PublishedFolder";
			base.InitializeView(this.viewPlaceHolder);
		}

		protected override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		protected void RenderSecondaryNavigation()
		{
			Infobar infobar = new Infobar("divErrDP", "infobar");
			ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
			DatePicker.Features features = DatePicker.Features.MultiDaySelection | DatePicker.Features.WeekSelector;
			DatePicker datePicker = new DatePicker("dp", date, (int)features);
			MonthPicker monthPicker = new MonthPicker(base.SessionContext, "divMp");
			if (base.SessionContext.ShowWeekNumbers)
			{
				features |= DatePicker.Features.WeekNumbers;
			}
			base.SanitizingResponse.Write("<div id=\"divCalPicker\">");
			infobar.Render(base.SanitizingResponse);
			datePicker.Render(base.SanitizingResponse);
			monthPicker.Render(base.SanitizingResponse);
			base.SanitizingResponse.Write("</div>");
		}

		protected void RenderBreadcrumbs()
		{
			Breadcrumbs breadcrumbs = new Breadcrumbs(base.SessionContext, base.NavigationModule);
			breadcrumbs.Render(base.Response.Output);
		}

		protected void RenderPublishRange()
		{
			IPublishedView publishedView = this.GetPublishedView();
			if (publishedView != null)
			{
				base.SanitizingResponse.Write(publishedView.PublishTimeRange);
			}
		}

		protected string GetEscapedPath()
		{
			return ((AnonymousSessionContext)base.SessionContext).EscapedPath;
		}

		protected override NavigationModule SelectNavagationModule()
		{
			return NavigationModule.Calendar;
		}

		private IPublishedView GetPublishedView()
		{
			IPublishedView publishedView = null;
			foreach (OwaSubPage owaSubPage in base.ChildSubPages)
			{
				publishedView = (owaSubPage as IPublishedView);
				if (publishedView != null)
				{
					break;
				}
			}
			return publishedView;
		}

		private const string PublishedFolder = "PublishedFolder";

		private string[] externalScriptFiles = new string[]
		{
			"publishedstartpage.js"
		};
	}
}
