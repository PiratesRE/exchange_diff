using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NamedProp : IEquatable<NamedProp>, IComparable<NamedProp>
	{
		public NamedProp(Guid guid, string name)
		{
			if (name == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("name");
			}
			this.kind = NamedPropKind.String;
			this.guid = guid;
			if (!this.guid.Equals(WellKnownNamedPropertyGuid.InternetHeaders))
			{
				this.strName = name;
				return;
			}
			bool flag = false;
			FaultInjectionUtils.FaultInjectionTracer.TraceTest<bool>(3964022077U, ref flag);
			if (!flag && !NamedProp.IsValidInternetHeadersName(name))
			{
				throw MapiExceptionHelper.InvalidParameterException("Internet Header name contains non-ASCII characters");
			}
			this.strName = name.ToLowerInvariant();
		}

		public NamedProp(Guid guid, int id)
		{
			this.kind = NamedPropKind.Id;
			this.guid = guid;
			this.id = id;
		}

		public NamedProp(NamedProp other)
		{
			this.kind = other.kind;
			this.guid = other.guid;
			this.id = other.id;
			this.strName = other.strName;
		}

		public static bool IsValidInternetHeadersName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			foreach (char c in name)
			{
				if (c < '!' || c > '~' || c == ':')
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.kind == NamedPropKind.Id)
			{
				return this.guid.GetHashCode() ^ this.id;
			}
			return this.guid.GetHashCode() ^ this.strName.GetHashCode();
		}

		public override bool Equals(object o)
		{
			NamedProp namedProp = o as NamedProp;
			return namedProp != null && this.Equals(namedProp);
		}

		public bool Equals(NamedProp other)
		{
			if (this.kind != other.kind || this.guid != other.guid)
			{
				return false;
			}
			if (this.kind != NamedPropKind.Id)
			{
				return string.Equals(this.strName, other.strName);
			}
			return this.id == other.id;
		}

		public int CompareTo(NamedProp other)
		{
			if (other == null)
			{
				return -1;
			}
			int num = this.Kind - other.Kind;
			if (num != 0)
			{
				return num;
			}
			num = this.Guid.CompareTo(other.Guid);
			if (num != 0)
			{
				return num;
			}
			if (this.Kind == NamedPropKind.Id)
			{
				return this.Id - other.Id;
			}
			return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
		}

		public NamedPropKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Name
		{
			get
			{
				if (this.Kind != NamedPropKind.String)
				{
					throw MapiExceptionHelper.IncompatiblePropTypeException("Kind of this named prop is Id and not a String");
				}
				return this.strName;
			}
		}

		public int Id
		{
			get
			{
				if (this.Kind != NamedPropKind.Id)
				{
					throw MapiExceptionHelper.IncompatiblePropTypeException("Kind of this named prop is String and not an Id");
				}
				return this.id;
			}
		}

		public virtual bool IsSingleInstanced
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			if (this.Kind == NamedPropKind.Id)
			{
				return "NamedPropGuid=" + this.Guid.ToString() + ", NamedPropId=" + this.Id.ToString();
			}
			return string.Concat(new string[]
			{
				"NamedPropGuid=",
				this.Guid.ToString(),
				", NamedPropName=\"",
				this.Name.ToString(),
				"\""
			});
		}

		[Obsolete("Directly use the C'tor and use WellKnownNamedProperties to find singletons.")]
		public static NamedProp Create(Guid guid, string name)
		{
			return new NamedProp(guid, name);
		}

		[Obsolete("Directly use the C'tor and use WellKnownNamedProperties to find singletons.")]
		public static NamedProp Create(Guid guid, int id)
		{
			return new NamedProp(guid, id);
		}

		internal unsafe static NamedProp MarshalFromNative(SNameId* buffer)
		{
			Guid guid = (Guid)Marshal.PtrToStructure(buffer->lpGuid, typeof(Guid));
			NamedProp namedProp;
			if (buffer->ulKind == 0)
			{
				namedProp = new NamedProp(guid, buffer->union.id);
			}
			else
			{
				if (buffer->ulKind != 1)
				{
					throw new ArgumentException("Invalid named property type");
				}
				namedProp = new NamedProp(guid, Marshal.PtrToStringUni(buffer->union.lpStr));
			}
			NamedProp namedProp2 = WellKnownNamedProperties.Find(namedProp);
			if (namedProp2 == null)
			{
				return namedProp;
			}
			return namedProp2;
		}

		internal int GetBytesToMarshal()
		{
			int num = SNameId.SizeOf + 7 & -8;
			num += (Marshal.SizeOf(typeof(Guid)) + 7 & -8);
			if (this.Kind == NamedPropKind.String)
			{
				num += ((this.Name.Length + 1) * 2 + 7 & -8);
			}
			return num;
		}

		internal unsafe void MarshalToNative(SNameId* pspv, ref byte* pExtra)
		{
			byte* ptr = pExtra;
			pExtra += (IntPtr)(Marshal.SizeOf(typeof(Guid)) + 7 & -8);
			*(Guid*)ptr = this.guid;
			pspv->lpGuid = (IntPtr)((void*)ptr);
			pspv->ulKind = (int)this.Kind;
			if (this.Kind == NamedPropKind.String)
			{
				char* ptr2 = pExtra;
				pspv->union.lpStr = (IntPtr)((void*)ptr2);
				pExtra += (IntPtr)((this.Name.Length + 1) * 2 + 7 & -8);
				foreach (char c in this.Name)
				{
					*(ptr2++) = c;
				}
				*ptr2 = '\0';
				return;
			}
			pspv->union.id = this.Id;
		}

		private NamedPropKind kind;

		private Guid guid;

		private string strName;

		private int id;
	}
}
