using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32;

namespace System.IO
{
	internal struct PathHelper
	{
		[SecurityCritical]
		internal unsafe PathHelper(char* charArrayPtr, int length)
		{
			this.m_length = 0;
			this.m_sb = null;
			this.m_arrayPtr = charArrayPtr;
			this.m_capacity = length;
			this.m_maxPath = Path.MaxPath;
			this.useStackAlloc = true;
			this.doNotTryExpandShortFileName = false;
		}

		[SecurityCritical]
		internal PathHelper(int capacity, int maxPath)
		{
			this.m_length = 0;
			this.m_arrayPtr = null;
			this.useStackAlloc = false;
			this.m_sb = new StringBuilder(capacity);
			this.m_capacity = capacity;
			this.m_maxPath = maxPath;
			this.doNotTryExpandShortFileName = false;
		}

		internal int Length
		{
			get
			{
				if (this.useStackAlloc)
				{
					return this.m_length;
				}
				return this.m_sb.Length;
			}
			set
			{
				if (this.useStackAlloc)
				{
					this.m_length = value;
					return;
				}
				this.m_sb.Length = value;
			}
		}

		internal int Capacity
		{
			get
			{
				return this.m_capacity;
			}
		}

		internal unsafe char this[int index]
		{
			[SecurityCritical]
			get
			{
				if (this.useStackAlloc)
				{
					return this.m_arrayPtr[index];
				}
				return this.m_sb[index];
			}
			[SecurityCritical]
			set
			{
				if (this.useStackAlloc)
				{
					this.m_arrayPtr[index] = value;
					return;
				}
				this.m_sb[index] = value;
			}
		}

		[SecurityCritical]
		internal unsafe void Append(char value)
		{
			if (this.Length + 1 >= this.m_capacity)
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			if (this.useStackAlloc)
			{
				this.m_arrayPtr[this.Length] = value;
				this.m_length++;
				return;
			}
			this.m_sb.Append(value);
		}

		[SecurityCritical]
		internal unsafe int GetFullPathName()
		{
			if (this.useStackAlloc)
			{
				char* ptr = stackalloc char[checked(unchecked((UIntPtr)(Path.MaxPath + 1)) * 2)];
				int fullPathName = Win32Native.GetFullPathName(this.m_arrayPtr, Path.MaxPath + 1, ptr, IntPtr.Zero);
				if (fullPathName > Path.MaxPath)
				{
					char* ptr2 = stackalloc char[checked(unchecked((UIntPtr)fullPathName) * 2)];
					ptr = ptr2;
					fullPathName = Win32Native.GetFullPathName(this.m_arrayPtr, fullPathName, ptr, IntPtr.Zero);
				}
				if (fullPathName >= Path.MaxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				if (fullPathName == 0 && *this.m_arrayPtr != '\0')
				{
					__Error.WinIOError();
				}
				else if (fullPathName < Path.MaxPath)
				{
					ptr[fullPathName] = '\0';
				}
				this.doNotTryExpandShortFileName = false;
				string.wstrcpy(this.m_arrayPtr, ptr, fullPathName);
				this.Length = fullPathName;
				return fullPathName;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(this.m_capacity + 1);
				int fullPathName2 = Win32Native.GetFullPathName(this.m_sb.ToString(), this.m_capacity + 1, stringBuilder, IntPtr.Zero);
				if (fullPathName2 > this.m_maxPath)
				{
					stringBuilder.Length = fullPathName2;
					fullPathName2 = Win32Native.GetFullPathName(this.m_sb.ToString(), fullPathName2, stringBuilder, IntPtr.Zero);
				}
				if (fullPathName2 >= this.m_maxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				if (fullPathName2 == 0 && this.m_sb[0] != '\0')
				{
					if (this.Length >= this.m_maxPath)
					{
						throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
					}
					__Error.WinIOError();
				}
				this.doNotTryExpandShortFileName = false;
				this.m_sb = stringBuilder;
				return fullPathName2;
			}
		}

		[SecurityCritical]
		internal unsafe bool TryExpandShortFileName()
		{
			if (this.doNotTryExpandShortFileName)
			{
				return false;
			}
			if (this.useStackAlloc)
			{
				this.NullTerminate();
				char* ptr = this.UnsafeGetArrayPtr();
				char* ptr2 = stackalloc char[checked(unchecked((UIntPtr)(Path.MaxPath + 1)) * 2)];
				int longPathName = Win32Native.GetLongPathName(ptr, ptr2, Path.MaxPath);
				if (longPathName >= Path.MaxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				if (longPathName == 0)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 2 || lastWin32Error == 3)
					{
						this.doNotTryExpandShortFileName = true;
					}
					return false;
				}
				string.wstrcpy(ptr, ptr2, longPathName);
				this.Length = longPathName;
				this.NullTerminate();
				return true;
			}
			else
			{
				StringBuilder stringBuilder = this.GetStringBuilder();
				string text = stringBuilder.ToString();
				string text2 = text;
				bool flag = false;
				if (text2.Length > Path.MaxPath)
				{
					text2 = Path.AddLongPathPrefix(text2);
					flag = true;
				}
				stringBuilder.Capacity = this.m_capacity;
				stringBuilder.Length = 0;
				int num = Win32Native.GetLongPathName(text2, stringBuilder, this.m_capacity);
				if (num == 0)
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					if (2 == lastWin32Error2 || 3 == lastWin32Error2)
					{
						this.doNotTryExpandShortFileName = true;
					}
					stringBuilder.Length = 0;
					stringBuilder.Append(text);
					return false;
				}
				if (flag)
				{
					num -= 4;
				}
				if (num >= this.m_maxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				stringBuilder = Path.RemoveLongPathPrefix(stringBuilder);
				this.Length = stringBuilder.Length;
				return true;
			}
		}

		[SecurityCritical]
		internal unsafe void Fixup(int lenSavedName, int lastSlash)
		{
			if (this.useStackAlloc)
			{
				char* ptr = stackalloc char[checked(unchecked((UIntPtr)lenSavedName) * 2)];
				string.wstrcpy(ptr, this.m_arrayPtr + lastSlash + 1, lenSavedName);
				this.Length = lastSlash;
				this.NullTerminate();
				this.doNotTryExpandShortFileName = false;
				bool flag = this.TryExpandShortFileName();
				this.Append(Path.DirectorySeparatorChar);
				if (this.Length + lenSavedName >= Path.MaxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				string.wstrcpy(this.m_arrayPtr + this.Length, ptr, lenSavedName);
				this.Length += lenSavedName;
				return;
			}
			else
			{
				string value = this.m_sb.ToString(lastSlash + 1, lenSavedName);
				this.Length = lastSlash;
				this.doNotTryExpandShortFileName = false;
				bool flag2 = this.TryExpandShortFileName();
				this.Append(Path.DirectorySeparatorChar);
				if (this.Length + lenSavedName >= this.m_maxPath)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				this.m_sb.Append(value);
				return;
			}
		}

		[SecurityCritical]
		internal unsafe bool OrdinalStartsWith(string compareTo, bool ignoreCase)
		{
			if (this.Length < compareTo.Length)
			{
				return false;
			}
			if (this.useStackAlloc)
			{
				this.NullTerminate();
				if (ignoreCase)
				{
					string value = new string(this.m_arrayPtr, 0, compareTo.Length);
					return compareTo.Equals(value, StringComparison.OrdinalIgnoreCase);
				}
				for (int i = 0; i < compareTo.Length; i++)
				{
					if (this.m_arrayPtr[i] != compareTo[i])
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				if (ignoreCase)
				{
					return this.m_sb.ToString().StartsWith(compareTo, StringComparison.OrdinalIgnoreCase);
				}
				return this.m_sb.ToString().StartsWith(compareTo, StringComparison.Ordinal);
			}
		}

		[SecuritySafeCritical]
		public override string ToString()
		{
			if (this.useStackAlloc)
			{
				return new string(this.m_arrayPtr, 0, this.Length);
			}
			return this.m_sb.ToString();
		}

		[SecurityCritical]
		private unsafe char* UnsafeGetArrayPtr()
		{
			return this.m_arrayPtr;
		}

		private StringBuilder GetStringBuilder()
		{
			return this.m_sb;
		}

		[SecurityCritical]
		private unsafe void NullTerminate()
		{
			this.m_arrayPtr[this.m_length] = '\0';
		}

		private int m_capacity;

		private int m_length;

		private int m_maxPath;

		[SecurityCritical]
		private unsafe char* m_arrayPtr;

		private StringBuilder m_sb;

		private bool useStackAlloc;

		private bool doNotTryExpandShortFileName;
	}
}
