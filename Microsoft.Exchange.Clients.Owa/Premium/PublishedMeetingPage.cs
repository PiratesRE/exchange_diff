using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PublishedMeetingPage : OwaSubPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string parameter = base.GetParameter("id", true);
			try
			{
				StoreObjectId itemId = Utilities.CreateStoreObjectId(parameter);
				PublishingUrl publishingUrl = ((AnonymousSessionContext)base.SessionContext).PublishingUrl;
				using (PublishedCalendar publishedCalendar = (PublishedCalendar)PublishedFolder.Create(publishingUrl))
				{
					this.detailLevel = publishedCalendar.DetailLevel;
					if (this.detailLevel == DetailLevelEnumType.AvailabilityOnly)
					{
						Utilities.EndResponse(OwaContext.Current.HttpContext, HttpStatusCode.Forbidden);
					}
					this.item = publishedCalendar.GetItemData(itemId);
				}
			}
			catch (FolderNotPublishedException)
			{
				Utilities.EndResponse(OwaContext.Current.HttpContext, HttpStatusCode.NotFound);
			}
			catch (OwaInvalidIdFormatException innerException)
			{
				throw new OwaInvalidRequestException("Invalid id param", innerException);
			}
			catch (PublishedFolderAccessDeniedException innerException2)
			{
				throw new OwaInvalidRequestException("Cannot access this published folder", innerException2);
			}
			catch (ObjectNotFoundException innerException3)
			{
				throw new OwaInvalidRequestException("Cannot open this item", innerException3);
			}
		}

		protected void RenderSubject()
		{
			base.SanitizingResponse.Write(this.item.Subject);
		}

		protected void RenderLocation()
		{
			base.SanitizingResponse.Write(this.item.Location);
		}

		protected void RenderWhen()
		{
			base.SanitizingResponse.Write(this.item.When);
		}

		protected void RenderBody()
		{
			if (this.DetailLevel == DetailLevelEnumType.FullDetails)
			{
				base.SanitizingResponse.Write(this.item.BodyText);
			}
		}

		protected DetailLevelEnumType DetailLevel
		{
			get
			{
				return this.detailLevel;
			}
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return new SanitizedHtmlString(this.item.Subject);
			}
		}

		public override string PageType
		{
			get
			{
				return "ReadPublishedMeetingPage";
			}
		}

		private PublishedCalendarItemData item;

		private DetailLevelEnumType detailLevel;

		private string[] externalScriptFiles = new string[]
		{
			"freadpublishedmeeting.js"
		};
	}
}
