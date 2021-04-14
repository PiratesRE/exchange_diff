using System;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExtractionData
	{
		public ExtractionData()
		{
			this.bodyFragment = null;
			this.childDisclaimer = null;
			this.childUniqueBody = null;
			this.originalTagInfo = null;
		}

		public ExtractionData(BodyFragmentInfo childFragment, BodyTagInfo parentBodyTag)
		{
			childFragment.ExtractNestedBodyParts(parentBodyTag, out this.bodyFragment, out this.childUniqueBody, out this.childDisclaimer);
			this.originalTagInfo = parentBodyTag;
		}

		public bool IsFormatReliable
		{
			get
			{
				return this.BodyFragment != null && this.BodyFragment.BodyTag == this.OriginalTagInfo;
			}
		}

		public BodyFragmentInfo BodyFragment
		{
			get
			{
				return this.bodyFragment;
			}
		}

		public BodyTagInfo OriginalTagInfo
		{
			get
			{
				return this.originalTagInfo;
			}
		}

		public FragmentInfo ChildUniqueBody
		{
			get
			{
				return this.childUniqueBody;
			}
		}

		public FragmentInfo ChildDisclaimer
		{
			get
			{
				return this.childDisclaimer;
			}
		}

		private readonly BodyFragmentInfo bodyFragment;

		private readonly BodyTagInfo originalTagInfo;

		private readonly FragmentInfo childUniqueBody;

		private readonly FragmentInfo childDisclaimer;
	}
}
