using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PropertyBagStream : MemoryStream
	{
		internal PropertyBagStream(MemoryPropertyBag propertyBag, PropertyDefinition propDef, PropertyType propertyType, int sizeEstimate) : base(sizeEstimate)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			Util.ThrowOnNullArgument(propDef, "propDef");
			if (propertyType != PropertyType.Unicode && propertyType != PropertyType.Binary)
			{
				throw new NotSupportedException(string.Format("PropertyBagStream only supports Unicode and Binary streams, actual type: {0}.", propertyType));
			}
			this.propertyBag = propertyBag;
			this.propDef = propDef;
			this.propertyType = propertyType;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.propertyBag != null)
			{
				if (this.propertyType == PropertyType.Unicode)
				{
					this.propertyBag[this.propDef] = Encoding.Unicode.GetString(this.ToArray(), 0, (int)this.Length);
				}
				else
				{
					this.propertyBag[this.propDef] = this.ToArray();
				}
				this.propertyBag = null;
			}
			base.Dispose(disposing);
		}

		private readonly PropertyDefinition propDef;

		private readonly PropertyType propertyType;

		private MemoryPropertyBag propertyBag;
	}
}
