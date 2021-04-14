using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Navigation")]
	internal sealed class NavigationEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(NavigationEventHandler));
		}

		[OwaEvent("GetSecondaryNavigation")]
		[OwaEventParameter("m", typeof(NavigationModule))]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void GetSecondaryNavigation()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "NavigationEventHandler.GetSecondaryNavigation");
			switch ((NavigationModule)base.GetParameter("m"))
			{
			case NavigationModule.Mail:
				NavigationHost.RenderMailSecondaryNavigation(this.Writer, base.UserContext);
				return;
			case NavigationModule.Calendar:
				if (!base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					throw new OwaSegmentationException("The calendar is disabled");
				}
				using (CalendarFolder folder = Utilities.GetFolder<CalendarFolder>(base.UserContext, base.UserContext.CalendarFolderOwaId, new PropertyDefinition[]
				{
					ViewStateProperties.CalendarViewType,
					ViewStateProperties.DailyViewDays
				}))
				{
					DailyView.RenderSecondaryNavigation(this.Writer, folder, base.UserContext);
					return;
				}
				break;
			case NavigationModule.Contacts:
				break;
			case NavigationModule.Tasks:
				if (!base.UserContext.IsFeatureEnabled(Feature.Tasks))
				{
					throw new OwaSegmentationException("Tasks are disabled");
				}
				TaskView.RenderSecondaryNavigation(this.Writer, base.UserContext);
				return;
			case NavigationModule.Options:
				return;
			case NavigationModule.AddressBook:
				DirectoryView.RenderSecondaryNavigation(this.Writer, base.UserContext);
				return;
			case NavigationModule.Documents:
				DocumentLibraryUtilities.RenderSecondaryNavigation(this.Writer, base.UserContext);
				return;
			case NavigationModule.PublicFolders:
				NavigationHost.RenderPublicFolderSecondaryNavigation(this.Writer, base.UserContext);
				return;
			default:
				return;
			}
			if (!base.UserContext.IsFeatureEnabled(Feature.Contacts))
			{
				throw new OwaSegmentationException("The Contacts feature is disabled");
			}
			ContactView.RenderSecondaryNavigation(this.Writer, base.UserContext, false);
		}

		[OwaEvent("GetPFFilter")]
		[OwaEventParameter("t", typeof(string))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void GetPublicFolderSecondaryNavigationFilter()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "NavigationEventHandler.GetSecondaryNavigationFilter");
			string containerClass = (string)base.GetParameter("t");
			if (ObjectClass.IsCalendarFolder(containerClass))
			{
				OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
				using (CalendarFolder folder = Utilities.GetFolder<CalendarFolder>(base.UserContext, folderId, new PropertyDefinition[]
				{
					ViewStateProperties.CalendarViewType,
					ViewStateProperties.DailyViewDays
				}))
				{
					this.Writer.Write("<div id=divPFCalFlt style=\"display:none\">");
					RenderingUtilities.RenderSecondaryNavigationDatePicker(folder, this.Writer, "divErrPfDp", "divPfDp", base.UserContext);
					new MonthPicker(base.UserContext, "divPfMp").Render(this.Writer);
					this.Writer.Write("</div>");
					return;
				}
			}
			if (ObjectClass.IsContactsFolder(containerClass))
			{
				ContactView.RenderSecondaryNavigationFilter(this.Writer, "divPFCntFlt");
				return;
			}
			if (ObjectClass.IsTaskFolder(containerClass))
			{
				TaskView.RenderSecondaryNavigationFilter(this.Writer, "divPFTskFlt");
			}
		}

		[OwaEventParameter("q", typeof(bool))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("PersistQuickLinksBar")]
		public void PersistQuickLinksBar()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "NavigationEventHandler.PersistQuickLinksBar");
			bool isQuickLinksBarVisible = (bool)base.GetParameter("q");
			base.UserContext.UserOptions.IsQuickLinksBarVisible = isQuickLinksBarVisible;
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("PersistWidth")]
		[OwaEventParameter("w", typeof(int))]
		public void PersistWidth()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "NavigationEventHandler.PersistWidth");
			int navigationBarWidth = (int)base.GetParameter("w");
			base.UserContext.UserOptions.NavigationBarWidth = navigationBarWidth;
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventParameter("pTr", typeof(bool))]
		[OwaEvent("GetFolderPickerTree")]
		public void GetFolderPickerTrees()
		{
			bool requirePublicFolderTree = (bool)base.GetParameter("pTr");
			FolderPickerTree folderPickerTree = FolderPickerTree.CreateFolderPickerTree(base.UserContext, requirePublicFolderTree);
			string text = "divFPErr";
			this.Writer.Write("<div id=\"divFPTrR\">");
			Infobar infobar = new Infobar(text, "infobar");
			infobar.Render(this.Writer);
			NavigationHost.RenderTreeRegionDivStart(this.Writer, null);
			NavigationHost.RenderTreeDivStart(this.Writer, "fptree");
			folderPickerTree.ErrDiv = text;
			folderPickerTree.Render(this.Writer);
			NavigationHost.RenderTreeDivEnd(this.Writer);
			NavigationHost.RenderTreeRegionDivEnd(this.Writer);
			this.Writer.Write("</div>");
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("ReloadCalendarNavigationTree")]
		public void ReloadCalendarNavigationTree()
		{
			NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, null, new NavigationNodeGroupSection[]
			{
				NavigationNodeGroupSection.Calendar
			});
		}

		public const string EventNamespace = "Navigation";

		public const string MethodGetSecondaryNavigation = "GetSecondaryNavigation";

		public const string MethodGetPublicFolderSecondaryNavigationFilter = "GetPFFilter";

		public const string MethodPersistQuickLinksBar = "PersistQuickLinksBar";

		public const string MethodPersistWidth = "PersistWidth";

		public const string MethodGetFolderPickerTrees = "GetFolderPickerTree";

		public const string MethodReloadCalendarNavigationTree = "ReloadCalendarNavigationTree";

		public const string FolderId = "fId";

		public const string FolderName = "fN";

		public const string Module = "m";

		public const string QuickLinksVisible = "q";

		public const string Width = "w";

		public const string FolderContainerClass = "t";

		public const string RequirePublicFolderTree = "pTr";

		public const string IsAddressBook = "fAB";

		public const string Recipient = "RCP";

		public const string Recipients = "Recips";
	}
}
