using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ObjectValidationError : ValidationError
	{
		public ObjectValidationError(LocalizedString description, ObjectId objectId, string dataSourceName) : base(description)
		{
			this.objectId = objectId;
			this.dataSourceName = dataSourceName;
		}

		public ObjectId ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		public string DataSourceName
		{
			get
			{
				return this.dataSourceName;
			}
		}

		public bool Equals(ObjectValidationError other)
		{
			return other != null && string.Equals(this.DataSourceName, other.DataSourceName, StringComparison.OrdinalIgnoreCase) && object.Equals(this.ObjectId, other.ObjectId) && base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ObjectValidationError);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = (base.GetHashCode() ^ (this.DataSourceName ?? string.Empty).ToLowerInvariant().GetHashCode() ^ ((this.ObjectId == null) ? 0 : this.ObjectId.GetHashCode()));
			}
			return this.hashCode;
		}

		private ObjectId objectId;

		private string dataSourceName;

		private int hashCode;
	}
}
