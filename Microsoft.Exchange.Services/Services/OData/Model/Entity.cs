using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class Entity
	{
		internal PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal virtual EntitySchema Schema
		{
			get
			{
				return EntitySchema.SchemaInstance;
			}
		}

		internal object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
				return this.PropertyBag[propertyDefinition];
			}
			set
			{
				ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
				this.PropertyBag[propertyDefinition] = value;
			}
		}

		public string Id
		{
			get
			{
				return (string)this[EntitySchema.Id];
			}
			set
			{
				this[EntitySchema.Id] = value;
			}
		}

		internal virtual Uri GetWebUri(ODataContext odataContext)
		{
			ArgumentValidator.ThrowIfNull("odataContext", odataContext);
			if (this.Id == null)
			{
				return null;
			}
			string uriString = string.Format("{0}Users('{1}')/{2}('{3}')", new object[]
			{
				odataContext.HttpContext.GetServiceRootUri(),
				odataContext.TargetMailbox.PrimarySmtpAddress,
				this.UserRootNavigationName,
				this.Id
			});
			return new Uri(uriString);
		}

		protected virtual string UserRootNavigationName
		{
			get
			{
				return string.Format("{0}s", this.Schema.EdmEntityType.Name);
			}
		}

		internal static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Entity).Namespace, typeof(Entity).Name, null, true, false);

		private PropertyBag propertyBag = new PropertyBag();
	}
}
