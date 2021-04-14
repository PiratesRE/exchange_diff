using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal sealed class SipCultureInfo : SipCultureInfoBase
	{
		internal SipCultureInfo(CultureInfo parent, string segmentID) : base(parent, segmentID)
		{
		}

		internal override string SipSegmentID
		{
			get
			{
				return this.segmentID;
			}
		}
	}
}
