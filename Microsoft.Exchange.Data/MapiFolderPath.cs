using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class MapiFolderPath : IEnumerable<string>, IEnumerable, IEquatable<MapiFolderPath>
	{
		public IEnumerator<string> GetEnumerator()
		{
			return new MapiFolderPath.LevelHierarchyEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(MapiFolderPath other)
		{
			if (object.ReferenceEquals(null, other) || (this.isNonIpmPath ^ other.isNonIpmPath) || this.levelHierarchy.Length != other.levelHierarchy.Length)
			{
				return false;
			}
			if (this.levelHierarchy == other.levelHierarchy)
			{
				return true;
			}
			int num = 0;
			while (this.levelHierarchy.Length > num)
			{
				if (!this.levelHierarchy[num].Equals(other.levelHierarchy[num], StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MapiFolderPath);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = new int?(this.isNonIpmPath.GetHashCode());
				foreach (string text in this)
				{
					this.hashCode = (this.hashCode << 13 | (int)((uint)this.hashCode.Value >> 19));
					this.hashCode ^= text.ToUpperInvariant().GetHashCode();
				}
			}
			return this.hashCode.Value;
		}

		public override string ToString()
		{
			if (this.literalRepresentation == null)
			{
				char value = '\\';
				foreach (string text in this)
				{
					if (-1 != text.IndexOf('\\'))
					{
						value = '￾';
						break;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (this.isNonIpmPath)
				{
					stringBuilder.Append(value);
					stringBuilder.Append("NON_IPM_SUBTREE");
				}
				foreach (string value2 in this)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(value2);
				}
				this.literalRepresentation = ((stringBuilder.Length == 0) ? MapiFolderPath.IpmSubtreeRoot.ToString() : stringBuilder.ToString());
			}
			return this.literalRepresentation;
		}

		public static MapiFolderPath Parse(string input)
		{
			return new MapiFolderPath(input);
		}

		public static bool operator ==(MapiFolderPath operand1, MapiFolderPath operand2)
		{
			return object.Equals(operand1, operand2);
		}

		public static bool operator !=(MapiFolderPath operand1, MapiFolderPath operand2)
		{
			return !object.Equals(operand1, operand2);
		}

		public bool IsIpmPath
		{
			get
			{
				return !this.isNonIpmPath;
			}
		}

		public bool IsNonIpmPath
		{
			get
			{
				return this.isNonIpmPath;
			}
		}

		public bool IsSubtreeRoot
		{
			get
			{
				return 0 == this.Depth;
			}
		}

		public int Depth
		{
			get
			{
				return this.levelHierarchy.Length;
			}
		}

		public string this[int level]
		{
			get
			{
				if (0 > level || this.Depth <= level)
				{
					throw new ArgumentException("level");
				}
				return this.levelHierarchy[level];
			}
		}

		public string Name
		{
			get
			{
				if (this.IsSubtreeRoot)
				{
					return string.Empty;
				}
				return this[this.Depth - 1];
			}
		}

		public MapiFolderPath Parent
		{
			get
			{
				if (this.IsSubtreeRoot)
				{
					return null;
				}
				if (null == this.parent)
				{
					this.parent = new MapiFolderPath(this, 1, false);
				}
				return this.parent;
			}
		}

		public static MapiFolderPath GenerateFolderPath(MapiFolderPath parentFolerPath, string subFolderName)
		{
			return MapiFolderPath.GenerateFolderPath(parentFolerPath, subFolderName, true);
		}

		public static MapiFolderPath GenerateFolderPath(MapiFolderPath parentFolerPath, string subFolderName, bool autoDetectRoot)
		{
			MapiFolderPath mapiFolderPath = parentFolerPath ?? MapiFolderPath.IpmSubtreeRoot;
			if (!string.IsNullOrEmpty(subFolderName))
			{
				return new MapiFolderPath(mapiFolderPath, subFolderName, autoDetectRoot);
			}
			return mapiFolderPath;
		}

		public static MapiFolderPath GenerateFolderPath(string parentFolderName, MapiFolderPath subFolderPath)
		{
			return MapiFolderPath.GenerateFolderPath(parentFolderName, subFolderPath, true);
		}

		public static MapiFolderPath GenerateFolderPath(string parentFolderName, MapiFolderPath subFolderPath, bool autoDetectRoot)
		{
			MapiFolderPath mapiFolderPath = subFolderPath ?? MapiFolderPath.IpmSubtreeRoot;
			if (!string.IsNullOrEmpty(parentFolderName))
			{
				return new MapiFolderPath(parentFolderName, mapiFolderPath, autoDetectRoot);
			}
			return mapiFolderPath;
		}

		public MapiFolderPath(string folderPath) : this(folderPath, string.IsNullOrEmpty(folderPath) ? '￾' : folderPath[0], null)
		{
		}

		public MapiFolderPath(string folderPath, bool nonIpmSubtree) : this(folderPath, string.IsNullOrEmpty(folderPath) ? '￾' : folderPath[0], new bool?(nonIpmSubtree))
		{
		}

		public MapiFolderPath(string folderPath, char folderSeperator) : this(folderPath, folderSeperator, null)
		{
		}

		public MapiFolderPath(string folderPath, char folderSeperator, bool nonIpmSubtree) : this(folderPath, folderSeperator, new bool?(nonIpmSubtree))
		{
		}

		private MapiFolderPath(string folderPath, char folderSeperator, bool? nonIpmSubtree)
		{
			if ('\\' != folderSeperator && '￾' != folderSeperator)
			{
				throw new FormatException(DataStrings.ExceptionFormatInvalid(folderPath));
			}
			if (string.IsNullOrEmpty(folderPath))
			{
				throw new FormatException(DataStrings.ExceptionFormatInvalid(folderPath));
			}
			if (folderSeperator != folderPath[0])
			{
				throw new FormatException(DataStrings.ExceptionFormatInvalid(folderPath));
			}
			List<string> list = new List<string>();
			int num = 0;
			int num2 = 0;
			while (folderPath.Length > num2)
			{
				if (folderSeperator == folderPath[num2])
				{
					list.Add(folderPath.Substring(num, num2 - num));
					num = 1 + num2;
				}
				if (folderPath.Length - 1 == num2)
				{
					list.Add((num <= num2) ? folderPath.Substring(num, folderPath.Length - num) : string.Empty);
				}
				num2++;
			}
			string[] array = list.ToArray();
			int num3 = 1;
			int num4 = array.Length - 1;
			if (folderSeperator == folderPath[folderPath.Length - 1])
			{
				num4--;
			}
			if (nonIpmSubtree == null && 0 < num4 && "NON_IPM_SUBTREE".Equals(array[num3], StringComparison.OrdinalIgnoreCase))
			{
				num4--;
				num3++;
			}
			int num5 = num3;
			while (num3 + num4 > num5)
			{
				if (string.IsNullOrEmpty(array[num5]))
				{
					throw new FormatException(DataStrings.ExceptionFormatInvalid(folderPath));
				}
				num5++;
			}
			string[] destinationArray = new string[num4];
			Array.Copy(array, num3, destinationArray, 0, num4);
			if (nonIpmSubtree != null && nonIpmSubtree == false && '\\' == folderSeperator)
			{
				this.literalRepresentation = folderPath;
			}
			this.levelHierarchy = destinationArray;
			this.isNonIpmPath = (nonIpmSubtree ?? (2 == num3));
		}

		private MapiFolderPath(MapiFolderPath parentFolderPath, string subFolderName, bool autoDetectRoot)
		{
			if (null == parentFolderPath)
			{
				throw new ArgumentNullException("parentFolderPath");
			}
			if (string.IsNullOrEmpty(subFolderName))
			{
				throw new ArgumentNullException("subFolderName");
			}
			if (autoDetectRoot && parentFolderPath.IsSubtreeRoot)
			{
				if (string.Equals("IPM_SUBTREE", subFolderName))
				{
					this.isNonIpmPath = false;
					this.levelHierarchy = parentFolderPath.levelHierarchy;
					return;
				}
				if (string.Equals("NON_IPM_SUBTREE", subFolderName))
				{
					this.isNonIpmPath = true;
					this.levelHierarchy = parentFolderPath.levelHierarchy;
					return;
				}
			}
			this.isNonIpmPath = parentFolderPath.isNonIpmPath;
			this.levelHierarchy = new string[1 + parentFolderPath.levelHierarchy.Length];
			Array.Copy(parentFolderPath.levelHierarchy, this.levelHierarchy, parentFolderPath.levelHierarchy.Length);
			this.levelHierarchy[this.levelHierarchy.Length - 1] = subFolderName;
		}

		private MapiFolderPath(string parentFolderName, MapiFolderPath subFolderPath, bool autoDetectRoot)
		{
			if (string.IsNullOrEmpty(parentFolderName))
			{
				throw new ArgumentNullException("parentFolderName");
			}
			if (null == subFolderPath)
			{
				throw new ArgumentNullException("subFolderPath");
			}
			this.isNonIpmPath = subFolderPath.isNonIpmPath;
			string text = parentFolderName;
			if (autoDetectRoot)
			{
				if ("IPM_SUBTREE".Equals(parentFolderName, StringComparison.OrdinalIgnoreCase))
				{
					this.isNonIpmPath = false;
					if (!subFolderPath.isNonIpmPath)
					{
						this.levelHierarchy = subFolderPath.levelHierarchy;
						return;
					}
					text = "NON_IPM_SUBTREE";
				}
				else if ("NON_IPM_SUBTREE".Equals(parentFolderName, StringComparison.OrdinalIgnoreCase))
				{
					this.isNonIpmPath = true;
					if (!subFolderPath.isNonIpmPath)
					{
						this.levelHierarchy = subFolderPath.levelHierarchy;
						return;
					}
					text = "NON_IPM_SUBTREE";
				}
			}
			this.levelHierarchy = new string[1 + subFolderPath.levelHierarchy.Length];
			Array.Copy(subFolderPath.levelHierarchy, 0, this.levelHierarchy, 1, subFolderPath.levelHierarchy.Length);
			this.levelHierarchy[0] = text;
		}

		private MapiFolderPath(MapiFolderPath source, int ascendedLevel, bool stopAscendingIfRoot)
		{
			if (0 > ascendedLevel)
			{
				throw new ArgumentException("ascendedLevel");
			}
			if (source.levelHierarchy.Length < ascendedLevel)
			{
				if (!stopAscendingIfRoot)
				{
					throw new ArgumentOutOfRangeException("ascendedLevel");
				}
				ascendedLevel = source.levelHierarchy.Length;
			}
			if (source.IsSubtreeRoot)
			{
				this.isNonIpmPath = source.isNonIpmPath;
				this.levelHierarchy = source.levelHierarchy;
				return;
			}
			this.isNonIpmPath = source.isNonIpmPath;
			this.levelHierarchy = new string[source.levelHierarchy.Length - ascendedLevel];
			Array.Copy(source.levelHierarchy, this.levelHierarchy, this.levelHierarchy.Length);
		}

		public const char DefaultFolderSeparator = '\\';

		public const char MapiDotNetFolderSeparator = '￾';

		public const string IpmSubtreeName = "IPM_SUBTREE";

		public const string NonIpmSubtreeName = "NON_IPM_SUBTREE";

		public static readonly MapiFolderPath IpmSubtreeRoot = new MapiFolderPath("\\", false);

		public static readonly MapiFolderPath NonIpmSubtreeRoot = new MapiFolderPath("\\", true);

		private readonly string[] levelHierarchy;

		private readonly bool isNonIpmPath;

		private string literalRepresentation;

		private int? hashCode;

		private MapiFolderPath parent;

		[Serializable]
		private struct LevelHierarchyEnumerator : IEnumerator<string>, IDisposable, IEnumerator
		{
			public string Current
			{
				get
				{
					if (0 > this.currentIndex || this.count <= this.currentIndex)
					{
						throw new InvalidOperationException(DataStrings.ExceptionCurrentIndexOutOfRange(this.currentIndex.ToString(), 0.ToString(), this.count.ToString()));
					}
					return this.levelHierarchy[this.currentIndex];
				}
			}

			public void Dispose()
			{
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				return this.count > ++this.currentIndex;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}

			public LevelHierarchyEnumerator(MapiFolderPath folderPath)
			{
				this.currentIndex = -1;
				this.levelHierarchy = folderPath.levelHierarchy;
				this.count = folderPath.levelHierarchy.Length;
			}

			private int currentIndex;

			private readonly int count;

			private readonly string[] levelHierarchy;
		}
	}
}
