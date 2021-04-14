using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class EventCategoryObject : ExEventCategory, IConfigurable, IComparable
	{
		internal EventCategoryObject(string name, int number, ExEventLog.EventLevel level, EventCategoryIdentity id) : base(name, number, level)
		{
			this.m_identity = id;
			this.isValid = true;
		}

		public EventCategoryObject()
		{
			this.m_identity = null;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", base.Number, base.Name);
		}

		public ObjectId Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		public ValidationError[] Validate()
		{
			return ValidationError.None;
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				this.isValid = value;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public int CompareTo(object value)
		{
			EventCategoryObject eventCategoryObject = value as EventCategoryObject;
			if (eventCategoryObject != null)
			{
				return base.Name.CompareTo(eventCategoryObject.Name);
			}
			throw new ArgumentException("Object is not an EventCategoryObject");
		}

		private EventCategoryIdentity m_identity;

		private bool isValid;
	}
}
