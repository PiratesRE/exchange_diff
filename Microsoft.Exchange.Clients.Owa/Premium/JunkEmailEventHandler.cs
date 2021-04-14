using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.JunkEMail)]
	[OwaEventNamespace("JunkEmail")]
	internal sealed class JunkEmailEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(JunkEmailEventHandler));
		}

		[OwaEvent("Add")]
		[OwaEventParameter("fFrmOpt", typeof(bool), false, false)]
		[OwaEventParameter("iLT", typeof(int), false, false)]
		[OwaEventParameter("sNE", typeof(string), false, false)]
		public void Add()
		{
			string email = HttpUtility.HtmlDecode((string)base.GetParameter("sNE"));
			JunkEmailListType junkEmailListType = (JunkEmailListType)base.GetParameter("iLT");
			bool isFromOptions = (bool)base.GetParameter("fFrmOpt");
			string empty = string.Empty;
			bool flag = false;
			if (!JunkEmailUtilities.Add(email, junkEmailListType, base.UserContext, isFromOptions, out empty))
			{
				flag = true;
			}
			if (flag)
			{
				this.Writer.Write("<div id=fErr>1</div>");
			}
			this.Writer.Write("<div id=sMsg>{0}</div>", Utilities.HtmlEncode(empty));
		}

		[OwaEventParameter("sId", typeof(string), false, false)]
		[OwaEventParameter("fDmn", typeof(bool), false, false)]
		[OwaEvent("AddSender")]
		[OwaEventParameter("iLT", typeof(int), false, false)]
		public void AddSender()
		{
			JunkEmailListType junkEmailListType = (JunkEmailListType)base.GetParameter("iLT");
			string text = (string)base.GetParameter("sId");
			bool flag = (bool)base.GetParameter("fDmn");
			if (OwaStoreObjectId.CreateFromString(text).IsConversationId)
			{
				return;
			}
			string s = string.Empty;
			bool flag2 = false;
			string senderSmtpAddress = Utilities.GetSenderSmtpAddress(text, base.UserContext);
			if (string.IsNullOrEmpty(senderSmtpAddress))
			{
				flag2 = true;
				s = LocalizedStrings.GetNonEncoded(-562376136);
			}
			else
			{
				int num = senderSmtpAddress.IndexOf('@');
				if (num < 0)
				{
					flag2 = true;
					s = LocalizedStrings.GetNonEncoded(-562376136);
				}
				else
				{
					int length = senderSmtpAddress.Length;
					if (!JunkEmailUtilities.Add(flag ? senderSmtpAddress.Substring(num, length - num) : senderSmtpAddress, junkEmailListType, base.UserContext, false, out s))
					{
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				this.Writer.Write("<div id=fErr>1</div>");
			}
			this.Writer.Write("<div id=sMsg>{0}</div>", Utilities.HtmlEncode(s));
		}

		[OwaEventParameter("sNE", typeof(string), false, false)]
		[OwaEventParameter("iLT", typeof(int), false, false)]
		[OwaEvent("Edit")]
		[OwaEventParameter("sOE", typeof(string), false, false)]
		public void Edit()
		{
			string oldEmail = HttpUtility.HtmlDecode((string)base.GetParameter("sOE"));
			string newEmail = HttpUtility.HtmlDecode((string)base.GetParameter("sNE"));
			JunkEmailListType junkEmailListType = (JunkEmailListType)base.GetParameter("iLT");
			string empty = string.Empty;
			bool flag = false;
			if (!JunkEmailUtilities.Edit(oldEmail, newEmail, junkEmailListType, base.UserContext, true, out empty))
			{
				flag = true;
			}
			if (flag)
			{
				this.Writer.Write("<div id=fErr>1</div>");
			}
			this.Writer.Write("<div id=sMsg>{0}</div>", Utilities.HtmlEncode(empty));
		}

		[OwaEvent("Remove")]
		[OwaEventParameter("iLT", typeof(int), false, false)]
		[OwaEventParameter("sRE", typeof(string), false, false)]
		public void Remove()
		{
			string[] array = ((string)base.GetParameter("sRE")).Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HttpUtility.HtmlDecode(array[i]);
			}
			JunkEmailListType junkEmailListType = (JunkEmailListType)base.GetParameter("iLT");
			JunkEmailUtilities.Remove(array, junkEmailListType, base.UserContext);
		}

		[OwaEventParameter("fEnbl", typeof(bool), false, false)]
		[OwaEvent("SaveOptions")]
		[OwaEventParameter("fTrstCnt", typeof(bool), false, false)]
		[OwaEventParameter("fSfOnly", typeof(bool), false, false)]
		public void SaveOptions()
		{
			bool isEnabled = (bool)base.GetParameter("fEnbl");
			bool isContactsTrusted = (bool)base.GetParameter("fTrstCnt");
			bool safeListsOnly = (bool)base.GetParameter("fSfOnly");
			JunkEmailUtilities.SaveOptions(isEnabled, isContactsTrusted, safeListsOnly, base.UserContext);
		}

		public const string EventNamespace = "JunkEmail";

		public const string MethodAdd = "Add";

		public const string MethodAddSender = "AddSender";

		public const string MethodEdit = "Edit";

		public const string MethodRemove = "Remove";

		public const string MethodSaveOptions = "SaveOptions";

		public const string ListType = "iLT";

		public const string OldEntry = "sOE";

		public const string NewEntry = "sNE";

		public const string RemoveEntry = "sRE";

		public const string IsFromOptions = "fFrmOpt";

		public const string ItemId = "sId";

		public const string IsAddDomain = "fDmn";

		public const string IsEnabled = "fEnbl";

		public const string IsContactsTrusted = "fTrstCnt";

		public const string SafeListsOnly = "fSfOnly";

		private const string DivError = "<div id=fErr>1</div>";

		private const string DivMessage = "<div id=sMsg>{0}</div>";
	}
}
