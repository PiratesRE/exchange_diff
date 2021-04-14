using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class NonRootLocalLongFullPath : LocalLongFullPath
	{
		private NonRootLocalLongFullPath()
		{
		}

		public new static NonRootLocalLongFullPath Parse(string pathName)
		{
			return (NonRootLocalLongFullPath)LongPath.ParseInternal(pathName, new NonRootLocalLongFullPath());
		}

		protected override bool ParseCore(string path, bool nothrow)
		{
			if (base.ParseCore(path, nothrow) && NonRootLocalLongFullPath.IsRootDirectory(base.PathName))
			{
				base.IsValid = false;
				if (!nothrow)
				{
					throw new ArgumentException(DataStrings.ErrorPathCanNotBeRoot, "path");
				}
			}
			return base.IsValid;
		}

		private static bool IsRootDirectory(string path)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(path))
			{
				result = path.Equals(Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}
	}
}
