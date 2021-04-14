using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MsiUIHandler
	{
		public MsiUIHandler()
		{
			this.UIHandlerDelegate = new MsiUIHandlerDelegate(this.ExternalUIHandler);
		}

		public int ExternalUIHandler(IntPtr context, uint messageType, string message)
		{
			return this.ExternalUIHandler(messageType, message);
		}

		private int ExternalUIHandler(uint messageType, string message)
		{
			TaskLogger.LogEnter();
			int result = 1;
			InstallMessage installMessage = (InstallMessage)(4278190080U & messageType);
			InstallMessage installMessage2 = installMessage;
			if (installMessage2 <= InstallMessage.FilesInUse)
			{
				if (installMessage2 != InstallMessage.FatalExit)
				{
					if (installMessage2 != InstallMessage.Error)
					{
						if (installMessage2 == InstallMessage.FilesInUse)
						{
							result = 0;
						}
					}
					else if (this.OnMsiError != null)
					{
						this.OnMsiError(message);
					}
				}
			}
			else if (installMessage2 != InstallMessage.OutOfDiskSpace)
			{
				if (installMessage2 != InstallMessage.ActionData)
				{
					if (installMessage2 == InstallMessage.Progress)
					{
						if (this.HandleProgressMessage(this.ParseProgressString(message)))
						{
							this.UpdateProgress();
						}
					}
				}
				else if (this.HandleActionDataMessage())
				{
					this.UpdateProgress();
				}
			}
			else
			{
				result = 0;
			}
			if (this.IsCanceled())
			{
				result = 2;
			}
			TaskLogger.LogExit();
			return result;
		}

		private bool HandleActionDataMessage()
		{
			TaskLogger.LogEnter();
			bool result = false;
			if (this.progressTotal != 0 && this.isEnableActionData)
			{
				if (this.isForwardProgress)
				{
					this.progress += this.progressStep;
					if (this.progress > this.progressTotal)
					{
						this.progress = this.progressTotal;
					}
				}
				else
				{
					this.progress -= this.progressStep;
					if (this.progress < 0)
					{
						this.progress = 0;
					}
				}
				result = true;
			}
			TaskLogger.LogExit();
			return result;
		}

		private bool HandleProgressMessage(int[] fields)
		{
			TaskLogger.LogEnter();
			bool result = false;
			if (fields != null && fields.Length == 4)
			{
				switch (fields[0])
				{
				case 0:
					this.progressTotal = fields[1];
					this.progressStep = 0;
					this.isEnableActionData = false;
					this.isForwardProgress = (fields[2] == 0);
					if (!this.isForwardProgress && this.phase == MsiUIHandler.MsiPhase.None)
					{
						this.isProgressEnabled = false;
						this.phase = MsiUIHandler.MsiPhase.Rollback;
					}
					else if (this.isForwardProgress)
					{
						this.progress = 0;
						if (fields[3] == 1)
						{
							if (this.phase == MsiUIHandler.MsiPhase.None || this.phase == MsiUIHandler.MsiPhase.Rollback)
							{
								this.isProgressEnabled = true;
								this.phase = MsiUIHandler.MsiPhase.ScriptGeneration;
							}
							else
							{
								this.isProgressEnabled = false;
							}
						}
						else if (fields[3] == 0)
						{
							if (this.phase == MsiUIHandler.MsiPhase.None || this.phase == MsiUIHandler.MsiPhase.ScriptGeneration)
							{
								this.isProgressEnabled = true;
								this.phase = MsiUIHandler.MsiPhase.ScriptExecution;
							}
							else if (this.phase == MsiUIHandler.MsiPhase.ScriptExecution)
							{
								this.isProgressEnabled = true;
								this.phase = MsiUIHandler.MsiPhase.Cleanup;
							}
							else
							{
								this.isProgressEnabled = false;
							}
						}
						else
						{
							this.isProgressEnabled = false;
						}
						result = true;
					}
					else
					{
						this.progress = this.progressTotal;
						this.phase = MsiUIHandler.MsiPhase.Rollback;
					}
					break;
				case 1:
					if (this.isProgressEnabled)
					{
						if (fields[2] == 1)
						{
							this.progressStep = fields[1];
							this.isEnableActionData = true;
						}
						else
						{
							this.isEnableActionData = false;
						}
					}
					break;
				case 2:
					if (this.isProgressEnabled && fields[1] != 0)
					{
						if (this.isForwardProgress)
						{
							this.progress += fields[1];
						}
						else
						{
							this.progress -= fields[1];
						}
						if (this.progress > this.progressTotal)
						{
							this.progress = this.progressTotal;
						}
						else if (this.progress < 0)
						{
							this.progress = 0;
						}
						result = true;
					}
					break;
				case 3:
					if (this.isProgressEnabled)
					{
						this.progressTotal += fields[1];
						result = true;
					}
					break;
				}
			}
			TaskLogger.LogEnter();
			return result;
		}

		private int[] ParseProgressString(string progressString)
		{
			TaskLogger.LogEnter();
			int[] array = new int[4];
			int[] array2 = array;
			Regex regex = new Regex("(?<1>\\d):\\s?(?<2>[-+]?\\d+)\\s");
			int num = 0;
			foreach (object obj in regex.Matches(progressString))
			{
				Match match = (Match)obj;
				num = int.Parse(match.Groups[1].Captures[0].Value);
				if (num > 4 || num < 1)
				{
					num = 0;
					break;
				}
				array2[num - 1] = int.Parse(match.Groups[2].Captures[0].Value);
			}
			TaskLogger.LogExit();
			if (num == 0)
			{
				return null;
			}
			return array2;
		}

		private void UpdateProgress()
		{
			TaskLogger.LogEnter();
			if (this.isProgressEnabled)
			{
				int num;
				if (this.progressTotal == 0)
				{
					num = 100;
				}
				else
				{
					num = (int)((long)this.progress * 100L / (long)this.progressTotal);
				}
				switch (this.phase)
				{
				case MsiUIHandler.MsiPhase.ScriptGeneration:
					num = num * 20 / 100;
					break;
				case MsiUIHandler.MsiPhase.ScriptExecution:
					num = num * 70 / 100 + 20;
					break;
				case MsiUIHandler.MsiPhase.Cleanup:
					num = num * 10 / 100 + 20 + 70;
					break;
				}
				if ((num > this.previousPercentage && this.isForwardProgress) || (num < this.previousPercentage && !this.isForwardProgress))
				{
					this.previousPercentage = num;
					this.OnProgress(num);
				}
			}
			TaskLogger.LogExit();
		}

		private const int GenerationPercentage = 20;

		private const int ExecutionPercentage = 70;

		private const int CleanupPercentage = 10;

		private int progressTotal;

		private int progress;

		private bool isForwardProgress = true;

		private bool isEnableActionData;

		private int progressStep;

		private bool isProgressEnabled;

		private MsiUIHandler.MsiPhase phase;

		private int previousPercentage;

		public MsiUIHandler.ProgressHandler OnProgress;

		public MsiUIHandler.IsCanceledHandler IsCanceled;

		public MsiUIHandler.MsiErrorHandler OnMsiError;

		public MsiUIHandlerDelegate UIHandlerDelegate;

		private enum MsiPhase
		{
			None,
			Rollback,
			ScriptGeneration,
			ScriptExecution,
			Cleanup
		}

		public delegate void ProgressHandler(int progress);

		public delegate bool IsCanceledHandler();

		public delegate void MsiErrorHandler(string errorMsg);
	}
}
