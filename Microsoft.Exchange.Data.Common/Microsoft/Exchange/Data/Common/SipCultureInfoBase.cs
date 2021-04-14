using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Common
{
	internal class SipCultureInfoBase : CultureInfo
	{
		internal SipCultureInfoBase(CultureInfo parent, string segmentID) : base(parent.Name)
		{
			this.parent = parent;
			this.segmentID = segmentID;
			this.name = parent.Name;
			this.sipName = string.Format(CultureInfo.InvariantCulture, "{0}-x-{1}", new object[]
			{
				parent.IsNeutralCulture ? parent.Name : parent.Parent.Name,
				segmentID
			});
			this.description = string.Format(CultureInfo.InvariantCulture, "Role-Based Culture ({0})", new object[]
			{
				this.name
			});
		}

		public override string Name
		{
			get
			{
				if (!this.useSipName)
				{
					return this.name;
				}
				return this.sipName;
			}
		}

		public override CultureInfo Parent
		{
			get
			{
				return this.parent;
			}
		}

		public override string EnglishName
		{
			get
			{
				return this.description;
			}
		}

		internal virtual bool UseSipName
		{
			get
			{
				return this.useSipName;
			}
			set
			{
				this.useSipName = value;
			}
		}

		internal virtual string SipName
		{
			get
			{
				return this.sipName;
			}
		}

		internal virtual string SipSegmentID
		{
			get
			{
				return this.segmentID;
			}
		}

		protected string description;

		protected string name;

		protected string sipName;

		protected bool useSipName;

		protected string segmentID;

		protected CultureInfo parent;
	}
}
