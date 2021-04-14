using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class SharepointItemId : SharepointListId
	{
		internal SharepointItemId(string id, string listName, Uri siteUri, CultureInfo cultureInfo, UriFlags uriFlags) : base(listName, siteUri, cultureInfo, uriFlags)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id");
			}
			this.id = id;
		}

		internal string ItemId
		{
			get
			{
				return this.id;
			}
		}

		protected override StringBuilder ToStringHelper()
		{
			StringBuilder stringBuilder = base.ToStringHelper();
			stringBuilder.AppendFormat("&ItemId={0}", this.ItemId);
			return stringBuilder;
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				SharepointItemId sharepointItemId = obj as SharepointItemId;
				return sharepointItemId.id == this.id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + this.id.GetHashCode();
		}

		private readonly string id;
	}
}
