using System;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class DiskSpaceValidator : ValidatorBase
	{
		public DiskSpaceValidator(long requiredSpace, string path) : this(requiredSpace, path, null)
		{
		}

		public DiskSpaceValidator(long requiredSpace, string path, Action<object> callback) : this(requiredSpace, path, true, callback)
		{
		}

		public DiskSpaceValidator(long requiredSpace, string path, bool checkUserTemp, Action<object> callback)
		{
			this.requiredSpace = requiredSpace;
			this.path = path;
			this.checkUserTemp = checkUserTemp;
			base.Callback = callback;
		}

		public override bool Validate()
		{
			base.InvokeCallback(Strings.CheckForAvailableSpace);
			ValidationHelper.ThrowIfNullOrEmpty(this.path, "this.path");
			if (!Directory.Exists(this.path))
			{
				base.InvokeCallback(Strings.NotExist(this.path));
				return false;
			}
			DriveInfo driveInfo = new DriveInfo(this.path);
			if (driveInfo.AvailableFreeSpace < this.requiredSpace)
			{
				return false;
			}
			if (this.checkUserTemp)
			{
				DriveInfo driveInfo2 = new DriveInfo(Path.GetTempPath());
				if (driveInfo2.AvailableFreeSpace < this.requiredSpace)
				{
					return false;
				}
			}
			return true;
		}

		private readonly long requiredSpace;

		private readonly string path;

		private readonly bool checkUserTemp;
	}
}
