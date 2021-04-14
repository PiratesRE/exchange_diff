using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class VirtualColumnDefinition
	{
		public VirtualColumnDefinition(VirtualColumnId id, Type type, int size, string name, string description)
		{
			this.id = id;
			this.type = type;
			this.size = size;
			this.name = name;
			this.description = description;
		}

		public VirtualColumnId Id
		{
			get
			{
				return this.id;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public int Size
		{
			get
			{
				return this.size;
			}
		}

		[Queryable]
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		[Queryable]
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		private readonly VirtualColumnId id;

		private readonly Type type;

		private readonly int size;

		private readonly string name;

		private readonly string description;
	}
}
