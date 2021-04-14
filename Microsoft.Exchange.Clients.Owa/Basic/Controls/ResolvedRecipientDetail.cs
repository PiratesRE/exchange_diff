using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class ResolvedRecipientDetail
	{
		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public string RoutingAddress
		{
			get
			{
				return this.routingAddress;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
		}

		public AddressOrigin AddressOrigin
		{
			get
			{
				return this.addressOrigin;
			}
		}

		public int RecipientFlags
		{
			get
			{
				return this.recipientFlags;
			}
		}

		public StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
		}

		public ADObjectId AdObjectId
		{
			get
			{
				return this.adObjectId;
			}
		}

		public string ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		public EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
		}

		public ResolvedRecipientDetail(RecipientAddress recipientAddress) : this(recipientAddress.SmtpAddress, recipientAddress.RoutingAddress, recipientAddress.DisplayName, recipientAddress.RoutingType, recipientAddress.AddressOrigin, recipientAddress.RecipientFlags, recipientAddress.StoreObjectId, recipientAddress.EmailAddressIndex, recipientAddress.ADObjectId)
		{
		}

		public ResolvedRecipientDetail(RecipientInfoCacheEntry recipientInfoCacheEntry) : this(recipientInfoCacheEntry.SmtpAddress, recipientInfoCacheEntry.RoutingAddress, recipientInfoCacheEntry.DisplayName, recipientInfoCacheEntry.RoutingType, recipientInfoCacheEntry.AddressOrigin, recipientInfoCacheEntry.RecipientFlags, recipientInfoCacheEntry.ItemId, recipientInfoCacheEntry.EmailAddressIndex)
		{
		}

		public ResolvedRecipientDetail(string smtpAddress, string routingAddress, string displayName, string routingType, AddressOrigin addressOrigin, int recipientFlags, StoreObjectId storeObjectId, EmailAddressIndex emailAddressIndex, ADObjectId adObjectId)
		{
			this.smtpAddress = ResolvedRecipientDetail.EnsureNonNull(smtpAddress);
			this.routingAddress = ResolvedRecipientDetail.EnsureNonNull(routingAddress);
			this.displayName = ResolvedRecipientDetail.EnsureNonNull(displayName);
			this.routingType = ResolvedRecipientDetail.EnsureNonNull(routingType);
			this.addressOrigin = addressOrigin;
			this.recipientFlags = recipientFlags;
			this.storeObjectId = storeObjectId;
			this.adObjectId = adObjectId;
			this.emailAddressIndex = EmailAddressIndex.None;
			if (string.IsNullOrEmpty(displayName))
			{
				this.displayName = this.smtpAddress;
			}
			if (this.storeObjectId != null)
			{
				this.itemId = this.storeObjectId.ToBase64String();
				this.emailAddressIndex = emailAddressIndex;
				return;
			}
			if (this.adObjectId != null)
			{
				this.itemId = Convert.ToBase64String(this.adObjectId.ObjectGuid.ToByteArray());
				return;
			}
			this.itemId = string.Empty;
		}

		public ResolvedRecipientDetail(string smtpAddress, string routingAddress, string displayName, string routingType, AddressOrigin addressOrigin, int recipientFlags, string itemId, EmailAddressIndex emailAddressIndex)
		{
			this.smtpAddress = ResolvedRecipientDetail.EnsureNonNull(smtpAddress);
			this.routingAddress = ResolvedRecipientDetail.EnsureNonNull(routingAddress);
			this.displayName = ResolvedRecipientDetail.EnsureNonNull(displayName);
			this.routingType = ResolvedRecipientDetail.EnsureNonNull(routingType);
			this.addressOrigin = addressOrigin;
			this.recipientFlags = recipientFlags;
			this.itemId = ResolvedRecipientDetail.EnsureNonNull(itemId);
			this.emailAddressIndex = emailAddressIndex;
			if (string.IsNullOrEmpty(displayName))
			{
				this.displayName = this.smtpAddress;
			}
		}

		public static ResolvedRecipientDetail[] ParseFromForm(HttpRequest request, string parameterName, bool isRequired)
		{
			string[] array = ResolvedRecipientDetail.SplitConcatStringsFromForm(request, parameterName, isRequired);
			if (array.Length % 8 != 0)
			{
				throw new OwaInvalidRequestException("Invalid account of resolved recipient details. Details received:" + string.Join("\n", array));
			}
			int num = array.Length / 8;
			List<ResolvedRecipientDetail> list = new List<ResolvedRecipientDetail>();
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 8;
				string text = array[num2];
				string text2 = array[num2 + 1];
				string text3 = array[num2 + 2];
				string text4 = array[num2 + 3];
				string s = array[num2 + 4];
				string s2 = array[num2 + 5];
				string text5 = array[num2 + 6];
				string s3 = array[num2 + 7];
				int num3;
				if (!int.TryParse(s, out num3))
				{
					throw new OwaInvalidRequestException("The addressOrigin should be a valid integerDetails received:" + string.Join("\n", array));
				}
				AddressOrigin addressOrigin = (AddressOrigin)num3;
				int num4;
				if (!int.TryParse(s3, out num4))
				{
					throw new OwaInvalidRequestException("The emailAddressIndex should be a valid integerDetails received:" + string.Join("\n", array));
				}
				EmailAddressIndex emailAddressIndex = (EmailAddressIndex)num4;
				int num5;
				if (!int.TryParse(s2, out num5))
				{
					throw new OwaInvalidRequestException("The recipientFlags should be a valid integerDetails received:" + string.Join("\n", array));
				}
				ResolvedRecipientDetail item = new ResolvedRecipientDetail(text, text2, text3, text4, addressOrigin, num5, text5, emailAddressIndex);
				list.Add(item);
			}
			return list.ToArray();
		}

		public void RenderConcatenatedDetails(bool requireJavascriptEncode, TextWriter writer)
		{
			string[] array = new string[8];
			array[0] = this.smtpAddress;
			array[1] = this.routingAddress;
			array[2] = this.displayName;
			array[3] = this.routingType;
			string[] array2 = array;
			int num = 4;
			int num2 = (int)this.addressOrigin;
			array2[num] = num2.ToString();
			array[5] = this.recipientFlags.ToString();
			array[6] = this.itemId;
			string[] array3 = array;
			int num3 = 7;
			int num4 = (int)this.emailAddressIndex;
			array3[num3] = num4.ToString();
			ResolvedRecipientDetail.ConcatAndRenderMultiStringsAsOneValue(requireJavascriptEncode, writer, array);
		}

		public Participant ToParticipant()
		{
			if (!string.IsNullOrEmpty(this.itemId))
			{
				if (this.addressOrigin == AddressOrigin.Directory)
				{
					this.adObjectId = new ADObjectId(Convert.FromBase64String(this.itemId));
				}
				else if (this.addressOrigin == AddressOrigin.Store)
				{
					this.storeObjectId = StoreObjectId.Deserialize(this.itemId);
				}
			}
			Participant result = null;
			Utilities.CreateExchangeParticipant(out result, this.displayName, this.routingAddress, this.routingType, this.addressOrigin, this.storeObjectId, this.emailAddressIndex);
			return result;
		}

		private static string[] SplitConcatStringsFromForm(HttpRequest request, string parameterName, bool isRequired)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (string.IsNullOrEmpty(parameterName))
			{
				throw new ArgumentException("parameterName is null or empty.");
			}
			string formParameter = Utilities.GetFormParameter(request, parameterName, isRequired);
			if (formParameter == null)
			{
				return null;
			}
			string[] array = formParameter.Split(new char[]
			{
				'&'
			});
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = HttpUtility.UrlDecode(array[i]);
			}
			return array2;
		}

		private static void ConcatAndRenderMultiStringsAsOneValue(bool needJavascriptEncode, TextWriter writer, params string[] stringsToRender)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (stringsToRender == null)
			{
				throw new ArgumentNullException("stringsToRender");
			}
			for (int i = 0; i < stringsToRender.Length; i++)
			{
				if (i != 0)
				{
					Utilities.HtmlEncode("&", writer);
				}
				string s = Utilities.UrlEncode(stringsToRender[i]);
				if (needJavascriptEncode)
				{
					Utilities.HtmlEncode(Utilities.JavascriptEncode(s), writer);
				}
				else
				{
					Utilities.HtmlEncode(s, writer);
				}
			}
		}

		private static string EnsureNonNull(string s)
		{
			if (s != null)
			{
				return s;
			}
			return string.Empty;
		}

		private string smtpAddress;

		private string routingAddress;

		private string displayName;

		private string routingType;

		private AddressOrigin addressOrigin;

		private int recipientFlags;

		private StoreObjectId storeObjectId;

		private EmailAddressIndex emailAddressIndex;

		private ADObjectId adObjectId;

		private string itemId;
	}
}
