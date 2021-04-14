using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ModelVersionBreadCrumb
	{
		public ModelVersionBreadCrumb(byte[] bytes)
		{
			ArgumentValidator.ThrowIfNull("bytes", bytes);
			this.versionsReady = new SortedSet<short>(ReverseComparer<short>.Instance);
			this.versionsNotReady = new SortedSet<short>(ReverseComparer<short>.Instance);
			this.Deserialize(bytes);
		}

		public static byte MaxCapacity
		{
			get
			{
				return byte.MaxValue;
			}
		}

		private byte TotalCrumbCount
		{
			get
			{
				return (byte)(this.versionsReady.Count + this.versionsNotReady.Count);
			}
		}

		public byte GetCrumbCount(ModelVersionBreadCrumb.VersionType versionType)
		{
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			return (byte)versionSet.Count;
		}

		public short GetLatest(ModelVersionBreadCrumb.VersionType versionType)
		{
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			if (versionSet.Count == 0)
			{
				throw new InvalidOperationException("Bread crumb doesn't have any versions of the specified type");
			}
			return versionSet.First<short>();
		}

		public bool Contains(short modelVersion, ModelVersionBreadCrumb.VersionType versionType)
		{
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			return versionSet.Contains(modelVersion);
		}

		public bool Add(short modelVersion, ModelVersionBreadCrumb.VersionType versionType)
		{
			if (this.TotalCrumbCount + 1 > ModelVersionBreadCrumb.MaxCapacity)
			{
				throw new InvalidOperationException("Cannot add anymore crumbs. Model Bread Crumb reached max capacity");
			}
			if (modelVersion < 0)
			{
				throw new ArgumentException("Only non-negative versions can be added to the crumb");
			}
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			return versionSet.Add(modelVersion);
		}

		public bool Remove(short modelVersion, ModelVersionBreadCrumb.VersionType versionType)
		{
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			return versionSet.Remove(modelVersion);
		}

		public byte[] Serialize()
		{
			if (this.TotalCrumbCount == 0)
			{
				return Array<byte>.Empty;
			}
			byte[] array = this.InitializeCrumbByteArray((int)this.TotalCrumbCount);
			int num = 2;
			foreach (short value in this.MergeVersionSets())
			{
				BitConverter.GetBytes(value).CopyTo(array, num);
				num += 2;
			}
			return array;
		}

		public IList<short> Prune(int maxNumber, short versionToKeep, ModelVersionBreadCrumb.VersionType versionType)
		{
			if (!this.Contains(versionToKeep, versionType))
			{
				throw new InvalidOperationException("The given input version is not contained in the crumb");
			}
			IList<short> list = new List<short>();
			SortedSet<short> versionSet = this.GetVersionSet(versionType);
			if (versionSet.Count > maxNumber)
			{
				int num = 0;
				bool flag = false;
				foreach (short num2 in versionSet)
				{
					num++;
					bool flag2 = false;
					if (num > maxNumber)
					{
						flag2 = true;
					}
					else if (num == maxNumber && !flag)
					{
						flag2 = true;
					}
					else
					{
						flag = (flag ? flag : (num2 == versionToKeep));
					}
					if (flag2 && num2 != versionToKeep)
					{
						list.Add(num2);
					}
				}
			}
			foreach (short item in list)
			{
				versionSet.Remove(item);
			}
			return list;
		}

		public override string ToString()
		{
			if (this.TotalCrumbCount != 0)
			{
				return string.Join<short>(",", this.MergeVersionSets());
			}
			return "<Empty>";
		}

		public IEnumerable<short> GetVersions(ModelVersionBreadCrumb.VersionType versionType)
		{
			return this.GetVersionSet(versionType).AsEnumerable<short>();
		}

		private SortedSet<short> GetVersionSet(ModelVersionBreadCrumb.VersionType versionType)
		{
			switch (versionType)
			{
			case ModelVersionBreadCrumb.VersionType.Ready:
				return this.versionsReady;
			case ModelVersionBreadCrumb.VersionType.NotReady:
				return this.versionsNotReady;
			default:
				throw new ArgumentException("Invalid version type");
			}
		}

		private IEnumerable<short> MergeVersionSets()
		{
			return this.versionsReady.Union(from version in this.versionsNotReady
			select -version);
		}

		private void Deserialize(byte[] bytes)
		{
			if (bytes.Length > 2)
			{
				byte b = bytes[1];
				int num = (int)(2 + 2 * b);
				if (num == bytes.Length)
				{
					for (int i = 2; i < bytes.Length; i += 2)
					{
						short num2 = BitConverter.ToInt16(bytes, i);
						if (num2 < 0)
						{
							this.Add(-num2, ModelVersionBreadCrumb.VersionType.NotReady);
						}
						else
						{
							this.Add(num2, ModelVersionBreadCrumb.VersionType.Ready);
						}
						if (this.TotalCrumbCount == ModelVersionBreadCrumb.MaxCapacity)
						{
							return;
						}
					}
				}
			}
		}

		private byte[] InitializeCrumbByteArray(int count)
		{
			int num = 2 + 2 * count;
			byte[] array = new byte[num];
			array[0] = 0;
			array[1] = (byte)count;
			return array;
		}

		private const byte SerializationVersion = 0;

		private SortedSet<short> versionsReady;

		private SortedSet<short> versionsNotReady;

		public enum VersionType
		{
			Ready,
			NotReady
		}
	}
}
