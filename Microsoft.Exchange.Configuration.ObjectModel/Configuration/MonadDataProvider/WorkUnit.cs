using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	public sealed class WorkUnit : WorkUnitBase, INotifyPropertyChanged
	{
		public WorkUnit()
		{
			this.description = "";
			this.text = "";
			this.canShowElapsedTime = true;
			this.canShowExecutedCommand = true;
			this.stopWatch = new Stopwatch();
		}

		public WorkUnit(string text, Icon icon) : this()
		{
			this.Text = text;
			this.Icon = icon;
		}

		public WorkUnit(string text, Icon icon, object target) : this(text, icon)
		{
			this.Target = target;
		}

		public bool CanShowElapsedTime
		{
			get
			{
				return this.canShowElapsedTime;
			}
			set
			{
				if (this.CanShowElapsedTime != value)
				{
					this.canShowElapsedTime = value;
					this.RaisePropertyChanged("CanShowElapsedTime");
				}
			}
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		public string ElapsedTimeText
		{
			get
			{
				TimeSpan elapsedTime = this.ElapsedTime;
				return Strings.ElapsedTimeDescription(elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
			}
		}

		[DefaultValue("")]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				value = (value ?? "");
				if (this.Description != value)
				{
					this.description = value;
					this.RaisePropertyChanged("Description");
				}
			}
		}

		[DefaultValue("")]
		public string StatusDescription
		{
			get
			{
				return this.statusDescription;
			}
			set
			{
				value = (value ?? "");
				if (this.StatusDescription != value)
				{
					this.statusDescription = value;
					this.RaisePropertyChanged("StatusDescription");
				}
			}
		}

		public bool CanShowExecutedCommand
		{
			get
			{
				return this.canShowExecutedCommand;
			}
			set
			{
				if (this.CanShowExecutedCommand != value)
				{
					this.canShowExecutedCommand = value;
					this.RaisePropertyChanged("CanShowExecutedCommand");
				}
			}
		}

		[DefaultValue("")]
		public string ExecutedCommandText
		{
			get
			{
				return this.executedCommandText;
			}
			set
			{
				if (this.ExecutedCommandText != value)
				{
					this.executedCommandText = value;
					this.RaisePropertyChanged("ExecutedCommandText");
				}
			}
		}

		public string ExecutedCommandTextForWorkUnit
		{
			get
			{
				if (!string.IsNullOrEmpty(this.executedCommandText) && this.Target != null)
				{
					return string.Format("{0} | {1}", MonadCommand.FormatParameterValue(this.Target), this.executedCommandText);
				}
				return this.executedCommandText;
			}
		}

		public string ErrorsDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < base.Errors.Count; i++)
				{
					ErrorRecord errorRecord = base.Errors[i];
					stringBuilder.AppendLine(Strings.WorkUnitError);
					stringBuilder.AppendLine(errorRecord.ToString());
					ErrorDetails errorDetails = errorRecord.ErrorDetails;
					if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.RecommendedAction))
					{
						stringBuilder.AppendLine(string.Format("{0} {1}", Strings.HelpUrlHeaderText, errorDetails.RecommendedAction));
					}
					if (i < base.Errors.Count - 1)
					{
						stringBuilder.AppendLine();
					}
				}
				return stringBuilder.ToString();
			}
		}

		public string WarningsDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < base.Warnings.Count; i++)
				{
					stringBuilder.AppendLine(Strings.WorkUnitWarning);
					using (WarningReportEventArgs warningReportEventArgs = new WarningReportEventArgs(default(Guid), base.Warnings[i], 0, new MonadCommand()))
					{
						stringBuilder.AppendLine(warningReportEventArgs.WarningMessage);
						if (!string.IsNullOrEmpty(warningReportEventArgs.HelpUrl))
						{
							stringBuilder.AppendLine(string.Format("{0} {1}", Strings.HelpUrlHeaderText, warningReportEventArgs.HelpUrl));
						}
					}
					if (i < base.Warnings.Count - 1)
					{
						stringBuilder.AppendLine();
					}
				}
				return stringBuilder.ToString();
			}
		}

		[DefaultValue(null)]
		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.Icon != value)
				{
					this.icon = value;
					this.RaisePropertyChanged("Icon");
				}
			}
		}

		[DefaultValue(0)]
		public override int PercentComplete
		{
			get
			{
				return this.percentComplete;
			}
			set
			{
				value = Math.Max(0, Math.Min(value, 100));
				if (this.PercentComplete != value)
				{
					this.percentComplete = value;
					this.RaisePropertyChanged("PercentComplete");
				}
			}
		}

		[DefaultValue(0)]
		public override WorkUnitBaseStatus CurrentStatus
		{
			get
			{
				return base.CurrentStatus;
			}
			set
			{
				base.CurrentStatus = value;
				this.Status = this.ConvertWorkUnitBaseStatus(base.CurrentStatus);
			}
		}

		[DefaultValue(WorkUnitStatus.NotStarted)]
		public WorkUnitStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				if (this.status != value)
				{
					if (!Enum.IsDefined(typeof(WorkUnitStatus), value))
					{
						throw new InvalidEnumArgumentException("Status", (int)value, typeof(WorkUnitStatus));
					}
					this.status = value;
					this.RaisePropertyChanged("Status");
					if (this.status == WorkUnitStatus.InProgress)
					{
						this.stopWatch.Reset();
						this.stopWatch.Start();
						return;
					}
					if (value == WorkUnitStatus.Completed || value == WorkUnitStatus.Failed)
					{
						this.stopWatch.Stop();
					}
				}
			}
		}

		public string Summary
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.Text);
				stringBuilder.AppendLine(LocalizedDescriptionAttribute.FromEnum(typeof(WorkUnitStatus), this.Status));
				if (base.Errors.Count > 0 || base.Warnings.Count > 0)
				{
					stringBuilder.AppendLine(this.ErrorsDescription);
					stringBuilder.AppendLine(this.WarningsDescription);
				}
				else
				{
					stringBuilder.AppendLine(this.Description);
				}
				if (this.Status != WorkUnitStatus.InProgress && this.Status != WorkUnitStatus.NotStarted && this.CanShowExecutedCommand && !string.IsNullOrEmpty(this.ExecutedCommandText))
				{
					stringBuilder.AppendLine(this.ExecutedCommandText);
				}
				if (this.CanShowElapsedTime && this.Status != WorkUnitStatus.NotStarted)
				{
					stringBuilder.AppendLine(this.ElapsedTimeText);
				}
				return stringBuilder.ToString();
			}
		}

		[DefaultValue(null)]
		public override object Target
		{
			get
			{
				return base.Target;
			}
			set
			{
				if (base.Target != value)
				{
					base.Target = value;
					this.RaisePropertyChanged("Target");
				}
			}
		}

		[DefaultValue("")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				value = (value ?? "");
				if (this.Text != value)
				{
					this.text = value;
					this.RaisePropertyChanged("Text");
				}
			}
		}

		public void ResetStatus()
		{
			this.PercentComplete = 0;
			this.CurrentStatus = 0;
			base.Errors.Clear();
			base.Warnings.Clear();
			this.StatusDescription = "";
			this.ExecutedCommandText = "";
			this.stopWatch.Reset();
		}

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private WorkUnitStatus ConvertWorkUnitBaseStatus(WorkUnitBaseStatus workUnitBase)
		{
			switch (workUnitBase)
			{
			case 0:
				return WorkUnitStatus.NotStarted;
			case 1:
				return WorkUnitStatus.InProgress;
			case 2:
				return WorkUnitStatus.Completed;
			case 3:
				return WorkUnitStatus.Failed;
			default:
				throw new ArgumentException("workUnitBase");
			}
		}

		private string description;

		private string statusDescription;

		private Icon icon;

		private int percentComplete;

		private WorkUnitStatus status;

		private string text;

		private Stopwatch stopWatch;

		private bool canShowElapsedTime;

		private bool canShowExecutedCommand;

		private string executedCommandText;
	}
}
