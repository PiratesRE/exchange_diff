using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PAAMenuItem
	{
		internal bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		internal int MenuKey
		{
			get
			{
				return this.menuKey;
			}
			set
			{
				this.menuKey = value;
			}
		}

		internal string MenuType
		{
			get
			{
				return this.menuType;
			}
			set
			{
				this.menuType = value;
			}
		}

		internal string Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		internal object TargetName
		{
			get
			{
				return this.targetName;
			}
			set
			{
				this.targetName = value;
			}
		}

		internal PhoneNumber TargetPhone
		{
			get
			{
				return this.targetPhone;
			}
			set
			{
				this.targetPhone = value;
			}
		}

		private bool enabled;

		private int menuKey;

		private string menuType;

		private string context;

		private object targetName;

		private PhoneNumber targetPhone;
	}
}
