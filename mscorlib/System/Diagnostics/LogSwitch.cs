using System;
using System.Security;

namespace System.Diagnostics
{
	[Serializable]
	internal class LogSwitch
	{
		private LogSwitch()
		{
		}

		[SecuritySafeCritical]
		public LogSwitch(string name, string description, LogSwitch parent)
		{
			if (name != null && name.Length == 0)
			{
				throw new ArgumentOutOfRangeException("Name", Environment.GetResourceString("Argument_StringZeroLength"));
			}
			if (name != null && parent != null)
			{
				this.strName = name;
				this.strDescription = description;
				this.iLevel = LoggingLevels.ErrorLevel;
				this.iOldLevel = this.iLevel;
				this.ParentSwitch = parent;
				Log.m_Hashtable.Add(this.strName, this);
				Log.AddLogSwitch(this);
				return;
			}
			throw new ArgumentNullException((name == null) ? "name" : "parent");
		}

		[SecuritySafeCritical]
		internal LogSwitch(string name, string description)
		{
			this.strName = name;
			this.strDescription = description;
			this.iLevel = LoggingLevels.ErrorLevel;
			this.iOldLevel = this.iLevel;
			this.ParentSwitch = null;
			Log.m_Hashtable.Add(this.strName, this);
			Log.AddLogSwitch(this);
		}

		public virtual string Name
		{
			get
			{
				return this.strName;
			}
		}

		public virtual string Description
		{
			get
			{
				return this.strDescription;
			}
		}

		public virtual LogSwitch Parent
		{
			get
			{
				return this.ParentSwitch;
			}
		}

		public virtual LoggingLevels MinimumLevel
		{
			get
			{
				return this.iLevel;
			}
			[SecuritySafeCritical]
			set
			{
				this.iLevel = value;
				this.iOldLevel = value;
				string strParentName = (this.ParentSwitch != null) ? this.ParentSwitch.Name : "";
				if (Debugger.IsAttached)
				{
					Log.ModifyLogSwitch((int)this.iLevel, this.strName, strParentName);
				}
				Log.InvokeLogSwitchLevelHandlers(this, this.iLevel);
			}
		}

		public virtual bool CheckLevel(LoggingLevels level)
		{
			return this.iLevel <= level || (this.ParentSwitch != null && this.ParentSwitch.CheckLevel(level));
		}

		public static LogSwitch GetSwitch(string name)
		{
			return (LogSwitch)Log.m_Hashtable[name];
		}

		internal string strName;

		internal string strDescription;

		private LogSwitch ParentSwitch;

		internal volatile LoggingLevels iLevel;

		internal volatile LoggingLevels iOldLevel;
	}
}
