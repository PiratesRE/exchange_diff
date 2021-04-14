using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	internal abstract class RestartResponderChecker
	{
		internal RestartResponderChecker(ResponderDefinition definition)
		{
			this.definition = definition;
		}

		internal bool IsRestartAllowed
		{
			get
			{
				return !this.Enabled || !this.CheckChangedSetting() || this.IsWithinThreshold();
			}
		}

		internal virtual string SkipReasonOrException
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual string KeyOfEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual string KeyOfSetting
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual bool EnabledByDefault
		{
			get
			{
				return true;
			}
		}

		internal virtual string DefaultSetting
		{
			get
			{
				return null;
			}
		}

		protected virtual bool OnSettingChange(string newSetting)
		{
			throw new NotImplementedException();
		}

		protected virtual bool IsWithinThreshold()
		{
			throw new NotImplementedException();
		}

		protected virtual bool Enabled
		{
			get
			{
				return new AttributeHelper(this.definition).GetBool(this.KeyOfEnabled, false, this.EnabledByDefault);
			}
		}

		protected virtual bool CheckChangedSetting()
		{
			string @string = new AttributeHelper(this.definition).GetString(this.KeyOfSetting, false, this.DefaultSetting);
			if (string.Compare(this.setting, @string, true) != 0)
			{
				this.setting = @string;
				return this.OnSettingChange(@string);
			}
			return true;
		}

		private readonly ResponderDefinition definition;

		private string setting;
	}
}
