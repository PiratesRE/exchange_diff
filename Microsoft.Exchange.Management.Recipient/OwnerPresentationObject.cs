using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public class OwnerPresentationObject : IConfigurable
	{
		public OwnerPresentationObject(ObjectId identity, string ownerValue)
		{
			this.id = identity;
			this.owner = ownerValue;
		}

		public ObjectId Identity
		{
			get
			{
				ObjectId objectId = this.id;
				if (objectId is ADObjectId && SuppressingPiiContext.NeedPiiSuppression)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, objectId);
				}
				return objectId;
			}
		}

		public string Owner
		{
			get
			{
				string text = this.owner;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					text = SuppressingPiiData.Redact(text);
				}
				return text;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
		}

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		private ObjectId id;

		private readonly string owner;
	}
}
