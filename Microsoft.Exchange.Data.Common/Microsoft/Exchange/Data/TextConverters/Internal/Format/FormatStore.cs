using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal class FormatStore
	{
		public FormatStore()
		{
			this.Nodes = new FormatStore.NodeStore(this);
			this.Styles = new FormatStore.StyleStore(this, FormatStoreData.GlobalStyles);
			this.Strings = new FormatStore.StringValueStore(FormatStoreData.GlobalStringValues);
			this.MultiValues = new FormatStore.MultiValueStore(this, FormatStoreData.GlobalMultiValues);
			this.Text = new FormatStore.TextStore();
			this.Initialize();
		}

		public FormatNode RootNode
		{
			get
			{
				return new FormatNode(this, 1);
			}
		}

		public uint CurrentTextPosition
		{
			get
			{
				return this.Text.CurrentPosition;
			}
		}

		public void Initialize()
		{
			this.Nodes.Initialize();
			this.Styles.Initialize(FormatStoreData.GlobalStyles);
			this.Strings.Initialize(FormatStoreData.GlobalStringValues);
			this.MultiValues.Initialize(FormatStoreData.GlobalMultiValues);
			this.Text.Initialize();
		}

		public void ReleaseValue(PropertyValue value)
		{
			if (value.IsString)
			{
				this.GetStringValue(value).Release();
				return;
			}
			if (value.IsMultiValue)
			{
				this.GetMultiValue(value).Release();
			}
		}

		public void AddRefValue(PropertyValue value)
		{
			if (value.IsString)
			{
				this.GetStringValue(value).AddRef();
				return;
			}
			if (value.IsMultiValue)
			{
				this.GetMultiValue(value).AddRef();
			}
		}

		public FormatNode GetNode(int nodeHandle)
		{
			return new FormatNode(this, nodeHandle);
		}

		public FormatNode AllocateNode(FormatContainerType type)
		{
			return new FormatNode(this.Nodes, this.Nodes.Allocate(type, this.CurrentTextPosition));
		}

		public FormatNode AllocateNode(FormatContainerType type, uint beginTextPosition)
		{
			return new FormatNode(this.Nodes, this.Nodes.Allocate(type, beginTextPosition));
		}

		public void FreeNode(FormatNode node)
		{
			this.Nodes.Free(node.Handle);
		}

		public FormatStyle AllocateStyle(bool isStatic)
		{
			return new FormatStyle(this, this.Styles.Allocate(isStatic));
		}

		public FormatStyle GetStyle(int styleHandle)
		{
			return new FormatStyle(this, styleHandle);
		}

		public void FreeStyle(FormatStyle style)
		{
			this.Styles.Free(style.Handle);
		}

		public StringValue AllocateStringValue(bool isStatic)
		{
			return new StringValue(this, this.Strings.Allocate(isStatic));
		}

		public StringValue AllocateStringValue(bool isStatic, string value)
		{
			StringValue result = this.AllocateStringValue(isStatic);
			result.SetString(value);
			return result;
		}

		public StringValue GetStringValue(PropertyValue propertyValue)
		{
			return new StringValue(this, propertyValue.StringHandle);
		}

		public void FreeStringValue(StringValue str)
		{
			this.Strings.Free(str.Handle);
		}

		public MultiValue AllocateMultiValue(bool isStatic)
		{
			return new MultiValue(this, this.MultiValues.Allocate(isStatic));
		}

		public MultiValue GetMultiValue(PropertyValue propertyValue)
		{
			return new MultiValue(this, propertyValue.MultiValueHandle);
		}

		public void FreeMultiValue(MultiValue multi)
		{
			this.MultiValues.Free(multi.Handle);
		}

		public void InitializeCodepageDetector()
		{
			this.Text.InitializeCodepageDetector();
		}

		public int GetBestWindowsCodePage()
		{
			return this.Text.GetBestWindowsCodePage();
		}

		public int GetBestWindowsCodePage(int preferredCodePage)
		{
			return this.Text.GetBestWindowsCodePage(preferredCodePage);
		}

		public void SetTextBoundary()
		{
			this.Text.DoNotMergeNextRun();
		}

		public void AddBlockBoundary()
		{
			if (this.Text.LastRunType != TextRunType.BlockBoundary)
			{
				this.Text.AddSimpleRun(TextRunType.BlockBoundary, 1);
			}
		}

		public void AddMarkupText(char[] textBuffer, int offset, int count)
		{
			this.Text.AddText(TextRunType.Markup, textBuffer, offset, count);
		}

		public void AddText(char[] textBuffer, int offset, int count)
		{
			this.Text.AddText(TextRunType.NonSpace, textBuffer, offset, count);
		}

		public void AddInlineObject()
		{
			this.Text.AddSimpleRun(TextRunType.FirstShort, 1);
			this.Text.DoNotMergeNextRun();
		}

		public void AddSpace(int count)
		{
			this.Text.AddSimpleRun(TextRunType.Space, count);
		}

		public void AddLineBreak(int count)
		{
			this.Text.AddSimpleRun(TextRunType.NewLine, count);
		}

		public void AddNbsp(int count)
		{
			this.Text.AddSimpleRun(TextRunType.NbSp, count);
		}

		public void AddTabulation(int count)
		{
			this.Text.AddSimpleRun(TextRunType.Tabulation, count);
		}

		internal TextRun GetTextRun(uint position)
		{
			if (position < this.CurrentTextPosition)
			{
				return this.GetTextRunReally(position);
			}
			return TextRun.Invalid;
		}

		internal TextRun GetTextRunReally(uint position)
		{
			return new TextRun(this.Text, position);
		}

		internal FormatStore.NodeStore Nodes;

		internal FormatStore.StyleStore Styles;

		internal FormatStore.StringValueStore Strings;

		internal FormatStore.MultiValueStore MultiValues;

		internal FormatStore.TextStore Text;

		[Flags]
		internal enum NodeFlags : byte
		{
			OnRightEdge = 1,
			OnLeftEdge = 2,
			CanFlush = 4,
			OutOfOrder = 8,
			Visited = 16
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct NodeEntry
		{
			internal int NextFree
			{
				get
				{
					return this.NextSibling;
				}
				set
				{
					this.NextSibling = value;
				}
			}

			public void Clean()
			{
				this = default(FormatStore.NodeEntry);
			}

			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.Type.ToString(),
					" (",
					this.Parent.ToString("X"),
					", ",
					this.LastChild.ToString("X"),
					", ",
					this.NextSibling.ToString("X"),
					") ",
					this.BeginTextPosition.ToString("X"),
					" - ",
					this.EndTextPosition.ToString("X")
				});
			}

			internal FormatContainerType Type;

			internal FormatStore.NodeFlags NodeFlags;

			internal TextMapping TextMapping;

			internal int Parent;

			internal int LastChild;

			internal int NextSibling;

			internal uint BeginTextPosition;

			internal uint EndTextPosition;

			internal int InheritanceMaskIndex;

			internal FlagProperties FlagProperties;

			internal PropertyBitMask PropertyMask;

			internal Property[] Properties;
		}

		internal struct StyleEntry
		{
			public StyleEntry(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propertyList)
			{
				this.RefCount = int.MaxValue;
				this.FlagProperties = flagProperties;
				this.PropertyMask = propertyMask;
				this.PropertyList = propertyList;
			}

			internal int NextFree
			{
				get
				{
					return this.FlagProperties.IntegerBag;
				}
				set
				{
					this.FlagProperties.IntegerBag = value;
				}
			}

			public void Clean()
			{
				this = default(FormatStore.StyleEntry);
			}

			internal int RefCount;

			internal FlagProperties FlagProperties;

			internal PropertyBitMask PropertyMask;

			internal Property[] PropertyList;
		}

		internal struct MultiValueEntry
		{
			public MultiValueEntry(PropertyValue[] values)
			{
				this.RefCount = int.MaxValue;
				this.Values = values;
				this.NextFree = 0;
			}

			public void Clean()
			{
				this = default(FormatStore.MultiValueEntry);
			}

			internal int RefCount;

			internal int NextFree;

			internal PropertyValue[] Values;
		}

		internal struct StringValueEntry
		{
			public StringValueEntry(string str)
			{
				this.RefCount = int.MaxValue;
				this.Str = str;
				this.NextFree = 0;
			}

			public void Clean()
			{
				this = default(FormatStore.StringValueEntry);
			}

			internal int RefCount;

			internal int NextFree;

			internal string Str;
		}

		internal struct InheritaceMask
		{
			public InheritaceMask(FlagProperties flagProperties, PropertyBitMask propertyMask)
			{
				this.FlagProperties = flagProperties;
				this.PropertyMask = propertyMask;
			}

			internal FlagProperties FlagProperties;

			internal PropertyBitMask PropertyMask;
		}

		internal class NodeStore
		{
			public NodeStore(FormatStore store)
			{
				this.store = store;
				this.planes = new FormatStore.NodeEntry[32][];
				this.planes[0] = new FormatStore.NodeEntry[16];
				this.freeListHead = 0;
				this.top = 0;
			}

			public FormatStore.NodeEntry[] Plane(int handle)
			{
				return this.planes[handle / 1024];
			}

			public int Index(int handle)
			{
				return handle % 1024;
			}

			public void Initialize()
			{
				this.freeListHead = -1;
				this.top = 1;
				this.planes[0][1].Type = FormatContainerType.Root;
				this.planes[0][1].NodeFlags = (FormatStore.NodeFlags.OnRightEdge | FormatStore.NodeFlags.CanFlush);
				this.planes[0][1].TextMapping = TextMapping.Unicode;
				this.planes[0][1].Parent = 0;
				this.planes[0][1].NextSibling = 1;
				this.planes[0][1].LastChild = 0;
				this.planes[0][1].BeginTextPosition = 0U;
				this.planes[0][1].EndTextPosition = uint.MaxValue;
				this.planes[0][1].FlagProperties = default(FlagProperties);
				this.planes[0][1].PropertyMask = default(PropertyBitMask);
				this.planes[0][1].Properties = null;
				this.top++;
			}

			public int Allocate(FormatContainerType type, uint currentTextPosition)
			{
				int num = this.freeListHead;
				int num2;
				FormatStore.NodeEntry[] array;
				if (num != -1)
				{
					num2 = num % 1024;
					array = this.planes[num / 1024];
					this.freeListHead = array[num2].NextFree;
				}
				else
				{
					num = this.top++;
					num2 = num % 1024;
					int num3 = num / 1024;
					if (num2 == 0)
					{
						if (num3 == 1024)
						{
							throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
						}
						if (num3 == this.planes.Length)
						{
							int num4 = Math.Min(this.planes.Length * 2, 1024);
							FormatStore.NodeEntry[][] destinationArray = new FormatStore.NodeEntry[num4][];
							Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
							this.planes = destinationArray;
						}
						if (this.planes[num3] == null)
						{
							this.planes[num3] = new FormatStore.NodeEntry[1024];
						}
					}
					else if (num3 == 0 && num2 == this.planes[num3].Length)
					{
						int num5 = Math.Min(this.planes[0].Length * 2, 1024);
						FormatStore.NodeEntry[] array2 = new FormatStore.NodeEntry[num5];
						Array.Copy(this.planes[0], 0, array2, 0, this.planes[0].Length);
						this.planes[0] = array2;
					}
					array = this.planes[num3];
				}
				array[num2].Type = type;
				array[num2].NodeFlags = (FormatStore.NodeFlags)0;
				array[num2].TextMapping = TextMapping.Unicode;
				array[num2].Parent = 0;
				array[num2].LastChild = 0;
				array[num2].NextSibling = num;
				array[num2].BeginTextPosition = currentTextPosition;
				array[num2].EndTextPosition = uint.MaxValue;
				array[num2].FlagProperties.ClearAll();
				array[num2].PropertyMask.ClearAll();
				array[num2].Properties = null;
				return num;
			}

			public void Free(int handle)
			{
				int num = handle % 1024;
				FormatStore.NodeEntry[] array = this.planes[handle / 1024];
				if (array[num].Properties != null)
				{
					for (int i = 0; i < array[num].Properties.Length; i++)
					{
						if (array[num].Properties[i].Value.IsRefCountedHandle)
						{
							this.store.ReleaseValue(array[num].Properties[i].Value);
						}
					}
				}
				array[num].NextFree = this.freeListHead;
				this.freeListHead = handle;
			}

			public long DumpStat(TextWriter dumpWriter)
			{
				int num = (this.top < 1024) ? 1 : ((this.top % 1024 == 0) ? (this.top / 1024) : (this.top / 1024 + 1));
				long num2 = (long)((num == 1) ? this.planes[0].Length : (num * 1024));
				long num3 = (long)(12 + this.planes.Length * 4 + 12 * num) + num2 * (long)Marshal.SizeOf(typeof(FormatStore.NodeEntry));
				long num4 = 0L;
				long num5 = 0L;
				long num6 = 0L;
				long num7 = 0L;
				for (int i = 0; i < this.top; i++)
				{
					int num8 = i % 1024;
					FormatStore.NodeEntry[] array = this.planes[i / 1024];
					if (array[num8].Type != FormatContainerType.Null)
					{
						num4 += 1L;
						if (array[num8].Properties != null)
						{
							num3 += (long)(12 + array[num8].Properties.Length * Marshal.SizeOf(typeof(Property)));
							num5 += 1L;
							num6 += (long)array[num8].Properties.Length;
							if ((long)array[num8].Properties.Length > num7)
							{
								num7 = (long)array[num8].Properties.Length;
							}
						}
					}
				}
				long num9 = (num5 == 0L) ? 0L : ((num6 + num5 - 1L) / num5);
				if (dumpWriter != null)
				{
					dumpWriter.WriteLine("Nodes alloc: {0}", num2);
					dumpWriter.WriteLine("Nodes used: {0}", num4);
					dumpWriter.WriteLine("Nodes proplists: {0}", num5);
					if (num5 != 0L)
					{
						dumpWriter.WriteLine("Nodes props: {0}", num6);
						dumpWriter.WriteLine("Nodes average proplist: {0}", num9);
						dumpWriter.WriteLine("Nodes max proplist: {0}", num7);
					}
					dumpWriter.WriteLine("Nodes bytes: {0}", num3);
				}
				return num3;
			}

			internal const int MaxElementsPerPlane = 1024;

			internal const int MaxPlanes = 1024;

			internal const int InitialPlanes = 32;

			internal const int InitialElements = 16;

			private FormatStore store;

			private FormatStore.NodeEntry[][] planes;

			private int freeListHead;

			private int top;
		}

		internal class StyleStore
		{
			public StyleStore(FormatStore store, FormatStore.StyleEntry[] globalStyles)
			{
				this.store = store;
				this.planes = new FormatStore.StyleEntry[16][];
				this.planes[0] = new FormatStore.StyleEntry[Math.Max(32, globalStyles.Length + 1)];
				this.freeListHead = 0;
				this.top = 0;
				if (globalStyles != null && globalStyles.Length != 0)
				{
					Array.Copy(globalStyles, 0, this.planes[0], 0, globalStyles.Length);
				}
			}

			public FormatStore.StyleEntry[] Plane(int handle)
			{
				return this.planes[handle / 2048];
			}

			public int Index(int handle)
			{
				return handle % 2048;
			}

			public void Initialize(FormatStore.StyleEntry[] globalStyles)
			{
				this.freeListHead = -1;
				if (globalStyles != null && globalStyles.Length != 0)
				{
					this.top = globalStyles.Length;
					return;
				}
				this.top = 1;
			}

			public int Allocate(bool isStatic)
			{
				int num = this.freeListHead;
				int num2;
				FormatStore.StyleEntry[] array;
				if (num != -1)
				{
					num2 = num % 2048;
					array = this.planes[num / 2048];
					this.freeListHead = array[num2].NextFree;
				}
				else
				{
					num = this.top++;
					num2 = num % 2048;
					int num3 = num / 2048;
					if (num2 == 0)
					{
						if (num3 == 512)
						{
							throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
						}
						if (num3 == this.planes.Length)
						{
							int num4 = Math.Min(this.planes.Length * 2, 512);
							FormatStore.StyleEntry[][] destinationArray = new FormatStore.StyleEntry[num4][];
							Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
							this.planes = destinationArray;
						}
						if (this.planes[num3] == null)
						{
							this.planes[num3] = new FormatStore.StyleEntry[2048];
						}
					}
					else if (num3 == 0 && num2 == this.planes[num3].Length)
					{
						int num5 = Math.Min(this.planes[0].Length * 2, 2048);
						FormatStore.StyleEntry[] array2 = new FormatStore.StyleEntry[num5];
						Array.Copy(this.planes[0], 0, array2, 0, this.planes[0].Length);
						this.planes[0] = array2;
					}
					array = this.planes[num3];
				}
				array[num2].PropertyList = null;
				array[num2].RefCount = (isStatic ? int.MaxValue : 1);
				array[num2].FlagProperties.ClearAll();
				array[num2].PropertyMask.ClearAll();
				return num;
			}

			public void Free(int handle)
			{
				int num = handle % 2048;
				FormatStore.StyleEntry[] array = this.planes[handle / 2048];
				if (array[num].PropertyList != null)
				{
					for (int i = 0; i < array[num].PropertyList.Length; i++)
					{
						if (array[num].PropertyList[i].Value.IsRefCountedHandle)
						{
							this.store.ReleaseValue(array[num].PropertyList[i].Value);
						}
					}
				}
				array[num].NextFree = this.freeListHead;
				this.freeListHead = handle;
			}

			public long DumpStat(TextWriter dumpWriter)
			{
				int num = (this.top < 2048) ? 1 : ((this.top % 2048 == 0) ? (this.top / 2048) : (this.top / 2048 + 1));
				long num2 = (long)((num == 1) ? this.planes[0].Length : (num * 2048));
				long num3 = (long)(12 + this.planes.Length * 4 + 12 * num) + num2 * (long)Marshal.SizeOf(typeof(FormatStore.StyleEntry));
				long num4 = 0L;
				long num5 = 0L;
				long num6 = 0L;
				long num7 = 0L;
				for (int i = 0; i < this.top; i++)
				{
					int num8 = i % 2048;
					FormatStore.StyleEntry[] array = this.planes[i / 2048];
					if (array[num8].RefCount != 0)
					{
						num4 += 1L;
						if (array[num8].PropertyList != null)
						{
							num3 += (long)(12 + array[num8].PropertyList.Length * Marshal.SizeOf(typeof(Property)));
							num5 += 1L;
							num6 += (long)array[num8].PropertyList.Length;
							if ((long)array[num8].PropertyList.Length > num7)
							{
								num7 = (long)array[num8].PropertyList.Length;
							}
						}
					}
				}
				long num9 = (num5 == 0L) ? 0L : ((num6 + num5 - 1L) / num5);
				if (dumpWriter != null)
				{
					dumpWriter.WriteLine("Styles alloc: {0}", num2);
					dumpWriter.WriteLine("Styles used: {0}", num4);
					dumpWriter.WriteLine("Styles non-null prop lists: {0}", num5);
					if (num5 != 0L)
					{
						dumpWriter.WriteLine("Styles total prop lists length: {0}", num6);
						dumpWriter.WriteLine("Styles average prop list length: {0}", num9);
						dumpWriter.WriteLine("Styles max prop list length: {0}", num7);
					}
					dumpWriter.WriteLine("Styles bytes: {0}", num3);
				}
				return num3;
			}

			internal const int MaxElementsPerPlane = 2048;

			internal const int MaxPlanes = 512;

			internal const int InitialPlanes = 16;

			internal const int InitialElements = 32;

			private FormatStore store;

			private FormatStore.StyleEntry[][] planes;

			private int freeListHead;

			private int top;
		}

		internal class StringValueStore
		{
			public StringValueStore(FormatStore.StringValueEntry[] globalStrings)
			{
				this.planes = new FormatStore.StringValueEntry[16][];
				this.planes[0] = new FormatStore.StringValueEntry[Math.Max(16, globalStrings.Length + 1)];
				this.freeListHead = 0;
				this.top = 0;
				if (globalStrings != null && globalStrings.Length != 0)
				{
					Array.Copy(globalStrings, 0, this.planes[0], 0, globalStrings.Length);
				}
			}

			public FormatStore.StringValueEntry[] Plane(int handle)
			{
				return this.planes[handle / 4096];
			}

			public int Index(int handle)
			{
				return handle % 4096;
			}

			public void Initialize(FormatStore.StringValueEntry[] globalStrings)
			{
				this.freeListHead = -1;
				if (globalStrings != null && globalStrings.Length != 0)
				{
					this.top = globalStrings.Length;
					return;
				}
				this.top = 1;
			}

			public int Allocate(bool isStatic)
			{
				int num = this.freeListHead;
				int num2;
				FormatStore.StringValueEntry[] array;
				if (num != -1)
				{
					num2 = num % 4096;
					array = this.planes[num / 4096];
					this.freeListHead = array[num2].NextFree;
				}
				else
				{
					num = this.top++;
					num2 = num % 4096;
					int num3 = num / 4096;
					if (num2 == 0)
					{
						if (num3 == 256)
						{
							throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
						}
						if (num3 == this.planes.Length)
						{
							int num4 = Math.Min(this.planes.Length * 2, 256);
							FormatStore.StringValueEntry[][] destinationArray = new FormatStore.StringValueEntry[num4][];
							Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
							this.planes = destinationArray;
						}
						if (this.planes[num3] == null)
						{
							this.planes[num3] = new FormatStore.StringValueEntry[4096];
						}
					}
					else if (num3 == 0 && num2 == this.planes[num3].Length)
					{
						int num5 = Math.Min(this.planes[0].Length * 2, 4096);
						FormatStore.StringValueEntry[] array2 = new FormatStore.StringValueEntry[num5];
						Array.Copy(this.planes[0], 0, array2, 0, this.planes[0].Length);
						this.planes[0] = array2;
					}
					array = this.planes[num3];
				}
				array[num2].Str = null;
				array[num2].RefCount = (isStatic ? int.MaxValue : 1);
				array[num2].NextFree = -1;
				return num;
			}

			public void Free(int handle)
			{
				int num = handle % 4096;
				FormatStore.StringValueEntry[] array = this.planes[handle / 4096];
				array[num].NextFree = this.freeListHead;
				this.freeListHead = handle;
			}

			public long DumpStat(TextWriter dumpWriter)
			{
				int num = (this.top < 4096) ? 1 : ((this.top % 4096 == 0) ? (this.top / 4096) : (this.top / 4096 + 1));
				long num2 = (long)((num == 1) ? this.planes[0].Length : (num * 4096));
				long num3 = (long)(12 + this.planes.Length * 4 + 12 * num) + num2 * (long)Marshal.SizeOf(typeof(FormatStore.StringValueEntry));
				long num4 = 0L;
				long num5 = 0L;
				long num6 = 0L;
				long num7 = 0L;
				for (int i = 0; i < this.top; i++)
				{
					int num8 = i % 4096;
					FormatStore.StringValueEntry[] array = this.planes[i / 4096];
					if (array[num8].RefCount != 0)
					{
						num4 += 1L;
						if (array[num8].Str != null)
						{
							num3 += (long)(12 + array[num8].Str.Length * 2);
							num5 += 1L;
							num6 += (long)array[num8].Str.Length;
							if ((long)array[num8].Str.Length > num7)
							{
								num7 = (long)array[num8].Str.Length;
							}
						}
					}
				}
				long num9 = (num5 == 0L) ? 0L : ((num6 + num5 - 1L) / num5);
				if (dumpWriter != null)
				{
					dumpWriter.WriteLine("StringValues alloc: {0}", num2);
					dumpWriter.WriteLine("StringValues used: {0}", num4);
					dumpWriter.WriteLine("StringValues non-null strings: {0}", num5);
					if (num5 != 0L)
					{
						dumpWriter.WriteLine("StringValues total string length: {0}", num6);
						dumpWriter.WriteLine("StringValues average string length: {0}", num9);
						dumpWriter.WriteLine("StringValues max string length: {0}", num7);
					}
					dumpWriter.WriteLine("StringValues bytes: {0}", num3);
				}
				return num3;
			}

			internal const int MaxElementsPerPlane = 4096;

			internal const int MaxPlanes = 256;

			internal const int InitialPlanes = 16;

			internal const int InitialElements = 16;

			private FormatStore.StringValueEntry[][] planes;

			private int freeListHead;

			private int top;
		}

		internal class MultiValueStore
		{
			public MultiValueStore(FormatStore store, FormatStore.MultiValueEntry[] globaMultiValues)
			{
				this.store = store;
				this.planes = new FormatStore.MultiValueEntry[16][];
				this.planes[0] = new FormatStore.MultiValueEntry[Math.Max(16, globaMultiValues.Length + 1)];
				this.freeListHead = 0;
				this.top = 0;
				if (globaMultiValues != null && globaMultiValues.Length != 0)
				{
					Array.Copy(globaMultiValues, 0, this.planes[0], 0, globaMultiValues.Length);
				}
			}

			public FormatStore Store
			{
				get
				{
					return this.store;
				}
			}

			public FormatStore.MultiValueEntry[] Plane(int handle)
			{
				return this.planes[handle / 4096];
			}

			public int Index(int handle)
			{
				return handle % 4096;
			}

			public void Initialize(FormatStore.MultiValueEntry[] globaMultiValues)
			{
				this.freeListHead = -1;
				if (globaMultiValues != null && globaMultiValues.Length != 0)
				{
					this.top = globaMultiValues.Length;
					return;
				}
				this.top = 1;
			}

			public int Allocate(bool isStatic)
			{
				int num = this.freeListHead;
				int num2;
				FormatStore.MultiValueEntry[] array;
				if (num != -1)
				{
					num2 = num % 4096;
					array = this.planes[num / 4096];
					this.freeListHead = array[num2].NextFree;
				}
				else
				{
					num = this.top++;
					num2 = num % 4096;
					int num3 = num / 4096;
					if (num2 == 0)
					{
						if (num3 == 256)
						{
							throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
						}
						if (num3 == this.planes.Length)
						{
							int num4 = Math.Min(this.planes.Length * 2, 256);
							FormatStore.MultiValueEntry[][] destinationArray = new FormatStore.MultiValueEntry[num4][];
							Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
							this.planes = destinationArray;
						}
						if (this.planes[num3] == null)
						{
							this.planes[num3] = new FormatStore.MultiValueEntry[4096];
						}
					}
					else if (num3 == 0 && num2 == this.planes[num3].Length)
					{
						int num5 = Math.Min(this.planes[0].Length * 2, 4096);
						FormatStore.MultiValueEntry[] array2 = new FormatStore.MultiValueEntry[num5];
						Array.Copy(this.planes[0], 0, array2, 0, this.planes[0].Length);
						this.planes[0] = array2;
					}
					array = this.planes[num3];
				}
				array[num2].Values = null;
				array[num2].RefCount = (isStatic ? int.MaxValue : 1);
				array[num2].NextFree = -1;
				return num;
			}

			public void Free(int handle)
			{
				int num = handle % 4096;
				FormatStore.MultiValueEntry[] array = this.planes[handle / 4096];
				if (array[num].Values != null)
				{
					for (int i = 0; i < array[num].Values.Length; i++)
					{
						if (array[num].Values[i].IsRefCountedHandle)
						{
							this.store.ReleaseValue(array[num].Values[i]);
						}
					}
				}
				array[num].NextFree = this.freeListHead;
				this.freeListHead = handle;
			}

			public long DumpStat(TextWriter dumpWriter)
			{
				int num = (this.top < 4096) ? 1 : ((this.top % 4096 == 0) ? (this.top / 4096) : (this.top / 4096 + 1));
				long num2 = (long)((num == 1) ? this.planes[0].Length : (num * 4096));
				long num3 = (long)(12 + this.planes.Length * 4 + 12 * num) + num2 * (long)Marshal.SizeOf(typeof(FormatStore.MultiValueEntry));
				long num4 = 0L;
				long num5 = 0L;
				long num6 = 0L;
				long num7 = 0L;
				for (int i = 0; i < this.top; i++)
				{
					int num8 = i % 4096;
					FormatStore.MultiValueEntry[] array = this.planes[i / 4096];
					if (array[num8].RefCount != 0)
					{
						num4 += 1L;
						if (array[num8].Values != null)
						{
							num3 += (long)(12 + array[num8].Values.Length * Marshal.SizeOf(typeof(PropertyValue)));
							num5 += 1L;
							num6 += (long)array[num8].Values.Length;
							if ((long)array[num8].Values.Length > num7)
							{
								num7 = (long)array[num8].Values.Length;
							}
						}
					}
				}
				long num9 = (num5 == 0L) ? 0L : ((num6 + num5 - 1L) / num5);
				if (dumpWriter != null)
				{
					dumpWriter.WriteLine("MultiValues alloc: {0}", num2);
					dumpWriter.WriteLine("MultiValues used: {0}", num4);
					dumpWriter.WriteLine("MultiValues non-null value lists: {0}", num5);
					if (num5 != 0L)
					{
						dumpWriter.WriteLine("MultiValues total value lists length: {0}", num6);
						dumpWriter.WriteLine("MultiValues average value list length: {0}", num9);
						dumpWriter.WriteLine("MultiValues max value list length: {0}", num7);
					}
					dumpWriter.WriteLine("MultiValues bytes: {0}", num3);
				}
				return num3;
			}

			internal const int MaxElementsPerPlane = 4096;

			internal const int MaxPlanes = 256;

			internal const int InitialPlanes = 16;

			internal const int InitialElements = 16;

			private FormatStore store;

			private FormatStore.MultiValueEntry[][] planes;

			private int freeListHead;

			private int top;
		}

		internal class TextStore
		{
			public TextStore()
			{
				this.planes = new char[16][];
				this.planes[0] = new char[1024];
			}

			public uint CurrentPosition
			{
				get
				{
					return this.position;
				}
			}

			public TextRunType LastRunType
			{
				get
				{
					return this.lastRunType;
				}
			}

			public char[] Plane(uint position)
			{
				return this.planes[(int)((UIntPtr)(position >> 15))];
			}

			public int Index(uint position)
			{
				return (int)(position & 32767U);
			}

			public char Pick(uint position)
			{
				return this.planes[(int)((UIntPtr)(position >> 15))][(int)((UIntPtr)(position & 32767U))];
			}

			public void Initialize()
			{
				this.position = 0U;
				this.lastRunType = TextRunType.Invalid;
				this.lastRunPosition = 0U;
				if (this.detector != null)
				{
					this.detector.Reset();
				}
			}

			public void InitializeCodepageDetector()
			{
				if (this.detector == null)
				{
					this.detector = new OutboundCodePageDetector();
				}
			}

			public int GetBestWindowsCodePage()
			{
				return this.detector.GetBestWindowsCodePage();
			}

			public int GetBestWindowsCodePage(int preferredCodePage)
			{
				return this.detector.GetBestWindowsCodePage(preferredCodePage);
			}

			public void AddText(TextRunType runType, char[] textBuffer, int offset, int count)
			{
				if (this.detector != null && runType != TextRunType.Markup)
				{
					this.detector.AddText(textBuffer, offset, count);
				}
				int num = (int)(this.position & 32767U);
				int num2 = (int)(this.position >> 15);
				if (this.lastRunType == runType && num != 0)
				{
					char[] array = this.planes[num2];
					int num3 = (int)(this.lastRunPosition & 32767U);
					int num4 = Math.Min(Math.Min(count, 4095 - FormatStore.TextStore.LengthFromRunHeader(array[num3])), 32768 - num);
					if (num4 != 0)
					{
						if (num2 == 0 && num + num4 > array.Length)
						{
							int effectiveLengthForFirstPlane = this.GetEffectiveLengthForFirstPlane(Math.Max(this.planes[0].Length * 2, num + num4));
							char[] array2 = new char[effectiveLengthForFirstPlane];
							Buffer.BlockCopy(this.planes[0], 0, array2, 0, (int)(this.position * 2U));
							array = (this.planes[0] = array2);
						}
						array[num3] = this.MakeTextRunHeader(runType, num4 + FormatStore.TextStore.LengthFromRunHeader(array[num3]));
						Buffer.BlockCopy(textBuffer, offset * 2, array, num * 2, num4 * 2);
						offset += num4;
						count -= num4;
						this.position += (uint)num4;
					}
				}
				while (count != 0)
				{
					num = (int)(this.position & 32767U);
					num2 = (int)(this.position >> 15);
					if (32768 - num < 21)
					{
						this.planes[num2][num] = this.MakeTextRunHeader(TextRunType.Invalid, 32768 - num - 1);
						this.position += (uint)(32768 - num);
					}
					else
					{
						int num5 = Math.Min(Math.Min(count, 4095), 32768 - num - 1);
						if (num2 == 0 && num + num5 + 1 > this.planes[0].Length)
						{
							int effectiveLengthForFirstPlane2 = this.GetEffectiveLengthForFirstPlane(Math.Max(this.planes[0].Length * 2, num + num5 + 1));
							char[] array3 = new char[effectiveLengthForFirstPlane2];
							Buffer.BlockCopy(this.planes[0], 0, array3, 0, (int)(this.position * 2U));
							this.planes[0] = array3;
						}
						else if (num == 0)
						{
							if (num2 == this.planes.Length)
							{
								if (num2 == 640)
								{
									throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
								}
								int num6 = Math.Min(this.planes.Length * 2, 640);
								char[][] destinationArray = new char[num6][];
								Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
								this.planes = destinationArray;
							}
							if (this.planes[num2] == null)
							{
								this.planes[num2] = new char[32768];
							}
						}
						this.lastRunType = runType;
						this.lastRunPosition = this.position;
						this.planes[num2][num] = this.MakeTextRunHeader(runType, num5);
						Buffer.BlockCopy(textBuffer, offset * 2, this.planes[num2], (num + 1) * 2, num5 * 2);
						offset += num5;
						count -= num5;
						this.position += (uint)(num5 + 1);
					}
				}
			}

			public void AddSimpleRun(TextRunType runType, int count)
			{
				if (this.lastRunType == runType)
				{
					char[] array = this.planes[(int)((UIntPtr)(this.lastRunPosition >> 15))];
					int num = (int)(this.lastRunPosition & 32767U);
					int num2 = Math.Min(count, 4095 - FormatStore.TextStore.LengthFromRunHeader(array[num]));
					if (num2 != 0)
					{
						array[num] = this.MakeTextRunHeader(runType, num2 + FormatStore.TextStore.LengthFromRunHeader(array[num]));
						count -= num2;
					}
				}
				if (count != 0)
				{
					int num3 = (int)(this.position & 32767U);
					int num4 = (int)(this.position >> 15);
					if (num3 == 0)
					{
						if (num4 == this.planes.Length)
						{
							if (num4 == 640)
							{
								throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
							}
							int num5 = Math.Min(this.planes.Length * 2, 640);
							char[][] destinationArray = new char[num5][];
							Array.Copy(this.planes, 0, destinationArray, 0, this.planes.Length);
							this.planes = destinationArray;
						}
						if (this.planes[num4] == null)
						{
							this.planes[num4] = new char[32768];
						}
					}
					else if (num4 == 0 && (ulong)(this.position + 1U) > (ulong)((long)this.planes[0].Length))
					{
						int effectiveLengthForFirstPlane = this.GetEffectiveLengthForFirstPlane(this.planes[0].Length * 2);
						char[] array2 = new char[effectiveLengthForFirstPlane];
						Buffer.BlockCopy(this.planes[0], 0, array2, 0, (int)(this.position * 2U));
						this.planes[0] = array2;
					}
					this.lastRunType = runType;
					this.lastRunPosition = this.position;
					this.planes[num4][num3] = this.MakeTextRunHeader(runType, count);
					this.position += 1U;
				}
			}

			public void ConvertToInvalid(uint startPosition)
			{
				char[] array = this.Plane(startPosition);
				int num = this.Index(startPosition);
				int num2 = (array[num] >= '\u3000') ? 1 : (FormatStore.TextStore.LengthFromRunHeader(array[num]) + 1);
				array[num] = this.MakeTextRunHeader(TextRunType.Invalid, num2 - 1);
			}

			public void ConvertToInvalid(uint startPosition, int countToConvert)
			{
				char[] array = this.Plane(startPosition);
				int num = this.Index(startPosition);
				int num2 = FormatStore.TextStore.LengthFromRunHeader(array[num]);
				int num3 = num2 - countToConvert;
				int num4 = num2 + 1 - (num3 + 1);
				array[num] = this.MakeTextRunHeader(TextRunType.Invalid, num4 - 1);
				array[num + num4] = this.MakeTextRunHeader(TextRunType.NonSpace, num3);
			}

			public void ConvertShortRun(uint startPosition, TextRunType type, int newEffectiveLength)
			{
				char[] array = this.Plane(startPosition);
				int num = this.Index(startPosition);
				array[num] = this.MakeTextRunHeader(type, newEffectiveLength);
			}

			public void DoNotMergeNextRun()
			{
				if (this.lastRunType != TextRunType.BlockBoundary)
				{
					this.lastRunType = TextRunType.Invalid;
				}
			}

			public long DumpStat(TextWriter dumpWriter)
			{
				int num = (int)(this.position + 32768U - 1U >> 15);
				if (num == 0)
				{
					num = 1;
				}
				long num2 = (long)((num == 1) ? this.planes[0].Length : (num * 32768));
				long num3 = (long)((ulong)this.position);
				long num4 = (long)(12 + this.planes.Length * 4 + num * 12) + num2 * 2L;
				if (dumpWriter != null)
				{
					dumpWriter.WriteLine("Text alloc: {0}", num2);
					dumpWriter.WriteLine("Text used: {0}", num3);
					dumpWriter.WriteLine("Text bytes: {0}", num4);
				}
				return num4;
			}

			internal static TextRunType TypeFromRunHeader(char runHeader)
			{
				return (TextRunType)(runHeader & '');
			}

			internal static int LengthFromRunHeader(char runHeader)
			{
				return (int)(runHeader & '࿿');
			}

			internal char MakeTextRunHeader(TextRunType runType, int length)
			{
				return (char)((int)runType | length);
			}

			private int GetEffectiveLengthForFirstPlane(int requestedLength)
			{
				int num = Math.Min(requestedLength, 32768);
				if (num == 32768)
				{
					return num;
				}
				if (32768 - num <= 21)
				{
					num = 32768;
				}
				return num;
			}

			internal const int LogMaxCharactersPerPlane = 15;

			internal const int MaxCharactersPerPlane = 32768;

			internal const int MaxPlanes = 640;

			internal const int InitialPlanes = 16;

			internal const int InitialCharacters = 1024;

			internal const int MaxRunEffectivelength = 4095;

			internal const int PaddingBufferLength = 21;

			private OutboundCodePageDetector detector;

			private char[][] planes;

			private uint position;

			private TextRunType lastRunType;

			private uint lastRunPosition;
		}
	}
}
