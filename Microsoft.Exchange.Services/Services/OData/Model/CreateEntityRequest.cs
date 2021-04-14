using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class CreateEntityRequest<TEntity> : ODataRequest<TEntity> where TEntity : Entity
	{
		public CreateEntityRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public TEntity Template { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			this.Template = base.ReadPostBodyAsEntity<TEntity>();
		}

		public override void Validate()
		{
			base.Validate();
			TEntity template = this.Template;
			foreach (PropertyDefinition propertyDefinition in template.Schema.MandatoryCreationProperties)
			{
				TEntity template2 = this.Template;
				if (!template2.PropertyBag.Contains(propertyDefinition))
				{
					throw new MandatoryPropertyMissingException(propertyDefinition.Name);
				}
			}
		}
	}
}
