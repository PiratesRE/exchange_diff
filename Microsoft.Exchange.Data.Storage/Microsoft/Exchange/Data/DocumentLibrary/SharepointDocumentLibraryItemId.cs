using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class SharepointDocumentLibraryItemId : SharepointItemId
	{
		internal SharepointDocumentLibraryItemId(string id, string listName, Uri siteUri, CultureInfo cultureInfo, UriFlags uriFlags, ICollection<string> itemHierarchy) : base(id, listName, siteUri, cultureInfo, uriFlags)
		{
			if (itemHierarchy == null)
			{
				throw new ArgumentNullException("itemHierarchy");
			}
			if (itemHierarchy.Count < 1)
			{
				throw new ArgumentException("itemHierarchy");
			}
			this.itemHierarchy = new List<string>(itemHierarchy.Count);
			foreach (string text in itemHierarchy)
			{
				if (text.Length > 0 && (text[text.Length - 1] == '/' || text[text.Length - 1] == '\\'))
				{
					this.itemHierarchy.Add(text.Substring(0, text.Length - 1));
				}
				else
				{
					this.itemHierarchy.Add(text);
				}
			}
		}

		internal string ParentDirectoryStructure
		{
			get
			{
				if (this.itemHierarchy.Count == 1)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.itemHierarchy.Count - 1; i++)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('/');
					}
					stringBuilder.Append(this.itemHierarchy[i]);
				}
				return stringBuilder.ToString();
			}
		}

		internal ReadOnlyCollection<string> ItemHierarchy
		{
			get
			{
				return new ReadOnlyCollection<string>(this.itemHierarchy);
			}
		}

		protected override StringBuilder ToStringHelper()
		{
			StringBuilder stringBuilder = base.ToStringHelper();
			stringBuilder.Append("&ItemHierarchy=");
			for (int i = 0; i < this.itemHierarchy.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(Uri.EscapeDataString("/"));
				}
				stringBuilder.Append(Uri.EscapeDataString(this.itemHierarchy[i]));
			}
			return stringBuilder;
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = (SharepointDocumentLibraryItemId)obj;
				return sharepointDocumentLibraryItemId.ItemId == base.ItemId;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + this.itemHierarchy.GetHashCode();
		}

		private readonly List<string> itemHierarchy;
	}
}
