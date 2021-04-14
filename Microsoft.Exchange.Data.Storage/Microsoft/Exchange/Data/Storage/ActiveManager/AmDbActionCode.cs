using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmDbActionCode
	{
		public static int ToInt(AmDbActionInitiator initiator, AmDbActionReason reason, AmDbActionCategory category)
		{
			return (int)(category | (AmDbActionCategory)((int)reason << 8) | (AmDbActionCategory)((int)initiator << 18));
		}

		public static void ToEnumFields(int actionCode, out AmDbActionInitiator initiator, out AmDbActionReason reason, out AmDbActionCategory category)
		{
			category = (AmDbActionCategory)(actionCode & 255);
			if (category > (AmDbActionCategory)255)
			{
				throw new AmInvalidActionCodeException(actionCode, category.GetType().Name, category.ToString());
			}
			reason = (AmDbActionReason)(actionCode >> 8 & 1023);
			if (reason > (AmDbActionReason)1023)
			{
				throw new AmInvalidActionCodeException(actionCode, reason.GetType().Name, reason.ToString());
			}
			initiator = (AmDbActionInitiator)(actionCode >> 18 & 255);
			if (initiator > (AmDbActionInitiator)255)
			{
				throw new AmInvalidActionCodeException(actionCode, initiator.GetType().Name, initiator.ToString());
			}
		}

		public AmDbActionCode(AmDbActionInitiator initiator, AmDbActionReason reason, AmDbActionCategory category)
		{
			this.m_initiator = initiator;
			this.m_reason = reason;
			this.m_category = category;
			this.m_intValue = AmDbActionCode.ToInt(initiator, reason, category);
			this.UpdateStringRepresentation();
		}

		public AmDbActionCode(int actionCode)
		{
			AmDbActionCode.ToEnumFields(actionCode, out this.m_initiator, out this.m_reason, out this.m_category);
			this.UpdateStringRepresentation();
		}

		public AmDbActionCode() : this(AmDbActionInitiator.None, AmDbActionReason.None, AmDbActionCategory.None)
		{
		}

		public AmDbActionInitiator Initiator
		{
			get
			{
				return this.m_initiator;
			}
		}

		public AmDbActionReason Reason
		{
			get
			{
				return this.m_reason;
			}
		}

		public AmDbActionCategory Category
		{
			get
			{
				return this.m_category;
			}
		}

		public int IntValue
		{
			get
			{
				return this.m_intValue;
			}
		}

		public bool IsMoveOperation
		{
			get
			{
				return this.Category == AmDbActionCategory.Move;
			}
		}

		public bool IsMountOrRemountOperation
		{
			get
			{
				return this.Category == AmDbActionCategory.Mount || this.Category == AmDbActionCategory.Remount;
			}
		}

		public bool IsDismountOperation
		{
			get
			{
				return this.Category == AmDbActionCategory.Dismount || this.Category == AmDbActionCategory.ForceDismount;
			}
		}

		public bool IsAdminOperation
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Admin;
			}
		}

		public bool IsAutomaticOperation
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic;
			}
		}

		public bool IsAdminMountOperation
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Admin && this.Category == AmDbActionCategory.Mount;
			}
		}

		public bool IsAdminMoveOperation
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Admin && this.Category == AmDbActionCategory.Move;
			}
		}

		public bool IsAdminDismountOperation
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Admin && this.Category == AmDbActionCategory.Dismount;
			}
		}

		public bool IsAutomaticMove
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic && this.Category == AmDbActionCategory.Move;
			}
		}

		public bool IsAutomaticShutdownSwitchover
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic && this.Reason == AmDbActionReason.SystemShutdown && this.Category == AmDbActionCategory.Move;
			}
		}

		public bool IsStartupAutoMount
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic && (this.Reason == AmDbActionReason.Startup || this.Reason == AmDbActionReason.StoreStarted) && this.Category == AmDbActionCategory.Mount;
			}
		}

		public bool IsAutomaticFailureItem
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic && this.Reason == AmDbActionReason.FailureItem;
			}
		}

		public bool IsAutomaticManagedAvailabilityFailover
		{
			get
			{
				return this.Initiator == AmDbActionInitiator.Automatic && this.Reason == AmDbActionReason.ManagedAvailability;
			}
		}

		public static explicit operator int(AmDbActionCode actionCode)
		{
			return actionCode.IntValue;
		}

		public override string ToString()
		{
			return this.m_actionCodeStr;
		}

		private void UpdateStringRepresentation()
		{
			this.m_actionCodeStr = string.Format("[Initiator:{0} Reason:{1} Category:{2}]", this.m_initiator, this.m_reason, this.m_category);
		}

		private const int CategoryMaxValue = 255;

		private const int ReasonMaxValue = 1023;

		private const int InitiatorMaxValue = 255;

		private string m_actionCodeStr;

		private AmDbActionInitiator m_initiator;

		private AmDbActionReason m_reason;

		private AmDbActionCategory m_category;

		private int m_intValue;
	}
}
