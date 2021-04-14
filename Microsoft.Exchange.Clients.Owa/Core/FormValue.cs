using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class FormValue
	{
		public FormValue(object value, ulong segmentationFlags, bool isCustomForm)
		{
			this.value = value;
			this.segmentationFlags = segmentationFlags;
			this.isCustomForm = isCustomForm;
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public ulong SegmentationFlags
		{
			get
			{
				return this.segmentationFlags;
			}
		}

		public bool IsCustomForm
		{
			get
			{
				return this.isCustomForm;
			}
		}

		private readonly object value;

		private readonly ulong segmentationFlags;

		private readonly bool isCustomForm;
	}
}
