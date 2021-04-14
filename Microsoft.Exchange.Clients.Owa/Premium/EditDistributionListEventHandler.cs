using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("EditPDL")]
	internal sealed class EditDistributionListEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(EditDistributionListEventHandler));
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("Save")]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("dn", typeof(string), false)]
		[OwaEventParameter("Itms", typeof(RecipientInfo), true, false)]
		[OwaEventParameter("notes", typeof(string), false, true)]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void Save()
		{
			bool flag = base.IsParameterSet("Id");
			using (DistributionList distributionList = this.GetDistributionList(new PropertyDefinition[0]))
			{
				if (base.IsParameterSet("dn"))
				{
					string text = ((string)base.GetParameter("dn")).Trim();
					if (text.Length > 256)
					{
						text = text.Substring(0, 256);
					}
					if (!string.Equals(distributionList.DisplayName, text))
					{
						distributionList.DisplayName = text;
					}
				}
				if (base.IsParameterSet("notes"))
				{
					string text2 = (string)base.GetParameter("notes");
					if (text2 != null)
					{
						BodyConversionUtilities.SetBody(distributionList, text2, Markup.PlainText, base.UserContext);
					}
				}
				distributionList.Clear();
				RecipientInfo[] array = (RecipientInfo[])base.GetParameter("Itms");
				EditDistributionListEventHandler.AddMembersToDL(distributionList, array, this.CheckDuplicateItems(array));
				Utilities.SaveItem(distributionList, flag);
				distributionList.Load();
				if (!flag)
				{
					this.Writer.Write("<div id=itemId>");
					this.Writer.Write(Utilities.GetIdAsString(distributionList));
					this.Writer.Write("</div>");
				}
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(distributionList.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
				string text3 = EditDistributionListEventHandler.GetTextPropertyValue(distributionList, ContactBaseSchema.FileAs);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = LocalizedStrings.GetNonEncoded(-1576800949);
				}
				this.Writer.Write("<div id=fa>");
				Utilities.HtmlEncode(text3, this.Writer);
				this.Writer.Write("</div>");
				base.MoveItemToDestinationFolderIfInScratchPad(distributionList);
			}
		}

		private static void AddMembersToDL(DistributionList distributionList, RecipientInfo[] recipients, int validCount)
		{
			int num = 0;
			while (num < recipients.Length && num < validCount)
			{
				Participant participant;
				recipients[num].ToParticipant(out participant);
				distributionList.Add(participant);
				num++;
			}
		}

		private static string GetTextPropertyValue(DistributionList distributionList, PropertyDefinition property)
		{
			return ItemUtility.GetProperty<string>(distributionList, property, string.Empty).Trim();
		}

		private DistributionList GetDistributionList(params PropertyDefinition[] prefetchProperties)
		{
			bool flag = base.IsParameterSet("Id");
			DistributionList result;
			if (flag)
			{
				result = base.GetRequestItem<DistributionList>(prefetchProperties);
			}
			else
			{
				OwaStoreObjectId folderId = base.GetParameter("fId") as OwaStoreObjectId;
				result = Utilities.CreateItem<DistributionList>(folderId);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			return result;
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("Itms", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("dupChk", typeof(int), false, true)]
		[OwaEvent("UpdateListView")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("sC", typeof(ColumnId), false, true)]
		[OwaEventParameter("sO", typeof(SortOrder), false, true)]
		public void UpdateListView()
		{
			RecipientInfo[] array = (RecipientInfo[])base.GetParameter("Itms");
			int num = this.CheckDuplicateItems(array);
			if (base.IsParameterSet("dupChk") && array.Length != num)
			{
				int num2 = (int)base.GetParameter("dupChk");
				if (num2 == 1 || num2 == 2)
				{
					this.Writer.Write("<div id=\"dup\"></div>");
					if (num2 == 2)
					{
						return;
					}
				}
			}
			ColumnId sortedColumn = base.IsParameterSet("sC") ? ((ColumnId)base.GetParameter("sC")) : ColumnId.MemberDisplayName;
			SortOrder order = base.IsParameterSet("sO") ? ((SortOrder)base.GetParameter("sO")) : SortOrder.Ascending;
			ListView listView = new DistributionListMemberListView(base.UserContext, array, num, sortedColumn, order);
			this.Writer.Write("<div id=\"data\" sR=\"");
			this.Writer.Write((0 < listView.DataSource.TotalCount) ? (listView.StartRange + 1) : 0);
			this.Writer.Write("\" eR=\"");
			this.Writer.Write((0 < listView.DataSource.TotalCount) ? (listView.EndRange + 1) : 0);
			this.Writer.Write("\" tC=\"");
			this.Writer.Write(listView.DataSource.TotalCount);
			this.Writer.Write("\" sCki=\"");
			this.Writer.Write(listView.Cookie);
			this.Writer.Write("\" iLcid=\"");
			this.Writer.Write(listView.CookieLcid);
			this.Writer.Write("\" sPfdDC=\"");
			this.Writer.Write(Utilities.HtmlEncode(listView.PreferredDC));
			this.Writer.Write("\" uC=\"");
			this.Writer.Write(listView.DataSource.UnreadCount);
			this.Writer.Write("\"></div>");
			listView.Render(this.Writer, ListView.RenderFlags.Contents | ListView.RenderFlags.Headers);
		}

		private int CheckDuplicateItems(RecipientInfo[] items)
		{
			int result = 0;
			IComparer<RecipientInfo> comparer = new EditDistributionListEventHandler.RecipientInfoComparer();
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].AddressOrigin != AddressOrigin.Directory && items[i].AddressOrigin != AddressOrigin.Store && string.IsNullOrEmpty(items[i].RoutingType))
				{
					items[i].AddressOrigin = AddressOrigin.OneOff;
					RecipientAddress recipientAddress = AnrManager.ResolveAnrStringToOneOffEmail(items[i].RoutingAddress);
					if (recipientAddress != null)
					{
						items[i].RoutingType = recipientAddress.RoutingType;
						items[i].RoutingAddress = recipientAddress.RoutingAddress;
					}
					else
					{
						items[i].RoutingType = "SMTP";
					}
				}
			}
			Array.Sort<RecipientInfo>(items, comparer);
			for (int j = 0; j < items.Length; j++)
			{
				if ((items[j].AddressOrigin != AddressOrigin.Store || !items[j].StoreObjectId.Equals(base.IsParameterSet("Id") ? ((OwaStoreObjectId)base.GetParameter("Id")).StoreObjectId : null)) && (j <= 0 || comparer.Compare(items[j - 1], items[j]) != 0))
				{
					items[result++] = items[j];
				}
			}
			return result;
		}

		private const int MaxListNameLength = 256;

		public const string EventNamespace = "EditPDL";

		public const string MethodUpdateListView = "UpdateListView";

		public const string Items = "Itms";

		public const string DisplayNameId = "dn";

		public const string Notes = "notes";

		public const string DuplicateCheck = "dupChk";

		private class RecipientInfoComparer : IComparer<RecipientInfo>
		{
			int IComparer<RecipientInfo>.Compare(RecipientInfo x, RecipientInfo y)
			{
				if (x.AddressOrigin != y.AddressOrigin)
				{
					return x.AddressOrigin - y.AddressOrigin;
				}
				if (x.AddressOrigin == AddressOrigin.Store)
				{
					int num = string.Compare(x.StoreObjectId.ToBase64String(), y.StoreObjectId.ToBase64String());
					if (num != 0)
					{
						return num;
					}
					return x.EmailAddressIndex - y.EmailAddressIndex;
				}
				else
				{
					if (x.AddressOrigin == AddressOrigin.Directory || x.RoutingType == y.RoutingType)
					{
						return string.Compare(x.RoutingAddress, y.RoutingAddress, StringComparison.InvariantCultureIgnoreCase);
					}
					return string.Compare(x.RoutingType, y.RoutingType, StringComparison.InvariantCultureIgnoreCase);
				}
			}
		}

		public enum DuplicateCheckLevel
		{
			RemoveSilently,
			RemoveAndWarn,
			StopAndWarn
		}
	}
}
