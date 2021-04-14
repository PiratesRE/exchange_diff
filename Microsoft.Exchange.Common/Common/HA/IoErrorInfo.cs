using System;

namespace Microsoft.Exchange.Common.HA
{
	internal class IoErrorInfo
	{
		internal IoErrorInfo()
		{
		}

		internal IoErrorInfo(IoErrorCategory category, string fileName, long offset, long size)
		{
			this.Category = category;
			this.FileName = fileName;
			this.Offset = offset;
			this.Size = size;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.GetType() != obj.GetType())
			{
				return false;
			}
			IoErrorInfo ioErrorInfo = obj as IoErrorInfo;
			return this.Category.Equals(ioErrorInfo.Category) && string.Equals(this.FileName, ioErrorInfo.FileName, StringComparison.OrdinalIgnoreCase) && this.Offset.Equals(ioErrorInfo.Offset) && this.Size.Equals(ioErrorInfo.Size);
		}

		public override string ToString()
		{
			return string.Format("(Category={0}, FileName={1}, Offset={2}, Size={3})", new object[]
			{
				this.Category,
				this.FileName,
				this.Offset,
				this.Size
			});
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal IoErrorCategory Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
			}
		}

		internal string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		internal long Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		internal long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		private IoErrorCategory category;

		private string fileName;

		private long offset;

		private long size;
	}
}
