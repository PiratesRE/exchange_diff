using System;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public struct TnefNameId
	{
		public TnefNameId(Guid propertySetGuid, int id)
		{
			this.propertySetGuid = propertySetGuid;
			this.id = id;
			this.name = null;
			this.kind = TnefNameIdKind.Id;
		}

		public TnefNameId(Guid propertySetGuid, string name)
		{
			this.propertySetGuid = propertySetGuid;
			this.id = 0;
			this.name = name;
			this.kind = TnefNameIdKind.Name;
		}

		public Guid PropertySetGuid
		{
			get
			{
				return this.propertySetGuid;
			}
		}

		public TnefNameIdKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		internal void Set(Guid propertySetGuid, int id)
		{
			this.propertySetGuid = propertySetGuid;
			this.id = id;
			this.name = null;
			this.kind = TnefNameIdKind.Id;
		}

		internal void Set(Guid propertySetGuid, string name)
		{
			this.propertySetGuid = propertySetGuid;
			this.id = 0;
			this.name = name;
			this.kind = TnefNameIdKind.Name;
		}

		private Guid propertySetGuid;

		private int id;

		private string name;

		private TnefNameIdKind kind;
	}
}
