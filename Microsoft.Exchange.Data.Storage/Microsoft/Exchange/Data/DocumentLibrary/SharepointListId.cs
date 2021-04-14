using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class SharepointListId : SharepointSiteId
	{
		internal SharepointListId(string listName, Uri siteUri, CultureInfo cultureInfo, UriFlags uriFlags) : base(siteUri, uriFlags)
		{
			this.cultureInfo = cultureInfo;
			this.listName = listName;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj) && ((SharepointListId)obj).listName == this.listName;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + this.listName.GetHashCode();
		}

		internal string ListName
		{
			get
			{
				return this.listName;
			}
		}

		internal KeyValuePair<string, XmlNode>? Cache
		{
			get
			{
				return this.cache;
			}
			set
			{
				this.cache = value;
			}
		}

		internal CultureInfo CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
			set
			{
				this.cultureInfo = value;
			}
		}

		protected override StringBuilder ToStringHelper()
		{
			StringBuilder stringBuilder = base.ToStringHelper();
			stringBuilder.AppendFormat("&ListName={0}", Uri.EscapeDataString(this.ListName));
			return stringBuilder;
		}

		private readonly string listName;

		[NonSerialized]
		private KeyValuePair<string, XmlNode>? cache;

		private CultureInfo cultureInfo;
	}
}
