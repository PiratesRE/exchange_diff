using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SharepointSiteId : DocumentLibraryObjectId
	{
		internal SharepointSiteId(Uri siteUri, UriFlags uriFlags) : base(uriFlags)
		{
			this.siteUri = siteUri;
		}

		internal SharepointSiteId(string siteUri, UriFlags uriFlags) : this(new Uri(siteUri), uriFlags)
		{
		}

		public new static SharepointSiteId Parse(string arg)
		{
			Uri uri = new Uri(arg);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in uri.Query.Substring(1).Split(new char[]
			{
				'&'
			}))
			{
				string[] array2 = text.Split(new char[]
				{
					'='
				});
				if (array2.Length != 2)
				{
					throw new CorruptDataException(text, Strings.ExCorruptData);
				}
				if (dictionary.ContainsKey(array2[0]))
				{
					throw new CorruptDataException(text, Strings.ExCorruptData);
				}
				dictionary.Add(array2[0], array2[1]);
			}
			int num = 0;
			string s = null;
			if (!dictionary.TryGetValue("UriFlags", out s) || !int.TryParse(s, out num))
			{
				throw new CorruptDataException(arg, Strings.ExCorruptData);
			}
			string text2 = null;
			if (dictionary.TryGetValue("ListName", out text2))
			{
				text2 = Uri.UnescapeDataString(text2);
				string id = null;
				if (dictionary.TryGetValue("ItemId", out id))
				{
					string text3 = null;
					if (dictionary.TryGetValue("ItemHierarchy", out text3))
					{
						if (dictionary.Count != 4 || (num != 33 && num != 17))
						{
							throw new CorruptDataException(arg, Strings.ExCorruptData);
						}
						text3 = Uri.UnescapeDataString(text3);
						return new SharepointDocumentLibraryItemId(id, text2, new Uri(uri.GetLeftPart(UriPartial.Path)), null, (UriFlags)num, text3.Split(new char[]
						{
							'/'
						}));
					}
					else
					{
						if (dictionary.Count != 3)
						{
							throw new CorruptDataException(arg, Strings.ExCorruptData);
						}
						return new SharepointItemId(id, text2, new Uri(uri.GetLeftPart(UriPartial.Path)), null, (UriFlags)num);
					}
				}
				else
				{
					if (dictionary.Count != 2 || (num != 5 && num != 9))
					{
						throw new CorruptDataException(arg, Strings.ExCorruptData);
					}
					return new SharepointListId(text2, new Uri(uri.GetLeftPart(UriPartial.Path)), null, (UriFlags)num);
				}
			}
			else
			{
				if (dictionary.Count != 1 || num != 1)
				{
					throw new CorruptDataException(arg, Strings.ExCorruptData);
				}
				return new SharepointSiteId(uri.GetLeftPart(UriPartial.Path), (UriFlags)num);
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == base.GetType() && ((SharepointSiteId)obj).siteUri == this.siteUri;
		}

		public override int GetHashCode()
		{
			return this.siteUri.GetHashCode();
		}

		public override string ToString()
		{
			return new UriBuilder(this.SiteUri)
			{
				Query = this.ToStringHelper().ToString()
			}.Uri.ToString();
		}

		public Uri SiteUri
		{
			get
			{
				return this.siteUri;
			}
		}

		protected virtual StringBuilder ToStringHelper()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("UriFlags={0}", (int)base.UriFlags);
			return stringBuilder;
		}

		private readonly Uri siteUri;
	}
}
