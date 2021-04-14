using System;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class TnefNameIdWrapper
	{
		public TnefNameIdWrapper(TnefNameId nameId)
		{
			this.nameId = nameId;
			this.ComputeHashCode();
		}

		public TnefNameIdWrapper(Guid guid, string name) : this(new TnefNameId(guid, name))
		{
		}

		public TnefNameIdWrapper(Guid guid, int id) : this(new TnefNameId(guid, id))
		{
		}

		public TnefNameId TnefNameId
		{
			get
			{
				return this.nameId;
			}
		}

		public Guid PropertySetGuid
		{
			get
			{
				return this.nameId.PropertySetGuid;
			}
		}

		public TnefNameIdKind Kind
		{
			get
			{
				return this.nameId.Kind;
			}
		}

		public int Id
		{
			get
			{
				return this.nameId.Id;
			}
		}

		public string Name
		{
			get
			{
				return this.nameId.Name;
			}
		}

		public override bool Equals(object obj)
		{
			TnefNameIdWrapper tnefNameIdWrapper = obj as TnefNameIdWrapper;
			return tnefNameIdWrapper != null && this.PropertySetGuid == tnefNameIdWrapper.PropertySetGuid && this.Id == tnefNameIdWrapper.Id && this.Name == tnefNameIdWrapper.Name;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		private int ComputeHashCode()
		{
			this.hashCode = (this.nameId.PropertySetGuid.GetHashCode() ^ ((this.Kind == TnefNameIdKind.Id) ? this.Id.GetHashCode() : this.Name.GetHashCode()));
			return this.hashCode;
		}

		private int hashCode;

		private TnefNameId nameId;
	}
}
