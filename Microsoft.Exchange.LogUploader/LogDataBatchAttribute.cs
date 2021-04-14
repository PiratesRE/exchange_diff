using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogDataBatchAttribute : Attribute
	{
		public bool IsRawBatch
		{
			get
			{
				return this.isRawBatch;
			}
			set
			{
				this.isRawBatch = value;
			}
		}

		private bool isRawBatch;
	}
}
