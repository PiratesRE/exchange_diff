using System;
using System.IO;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class XsoGreeting : GreetingBase
	{
		internal XsoGreeting(UMSubscriber umuser, string name)
		{
			this.umuser = umuser;
			this.name = name;
		}

		internal override string Name
		{
			get
			{
				return "Um.CustomGreetings." + this.name;
			}
		}

		internal override void Put(string sourceFileName)
		{
			ITempWavFile tempWavFile = null;
			try
			{
				using (PcmReader pcmReader = new PcmReader(sourceFileName))
				{
					if (pcmReader.WaveFormat.SamplesPerSec == 16000)
					{
						tempWavFile = MediaMethods.Pcm16ToPcm8(pcmReader, false);
						sourceFileName = tempWavFile.FilePath;
					}
				}
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.umuser.CreateSessionLock())
				{
					using (UserConfiguration configuration = this.GetConfiguration(mailboxSessionLock.Session, true))
					{
						using (Stream stream = configuration.GetStream())
						{
							using (FileStream fileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
							{
								stream.SetLength(0L);
								CommonUtil.CopyStream(fileStream, stream);
								configuration.Save();
							}
						}
					}
				}
			}
			finally
			{
				if (tempWavFile != null)
				{
					tempWavFile.Dispose();
				}
			}
		}

		internal override void Delete()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.umuser.CreateSessionLock())
			{
				mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					this.Name
				});
			}
		}

		internal override ITempWavFile Get()
		{
			ITempWavFile result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.umuser.CreateSessionLock())
			{
				using (UserConfiguration configuration = this.GetConfiguration(mailboxSessionLock.Session, false))
				{
					if (configuration == null)
					{
						result = null;
					}
					else
					{
						FileStream fileStream = null;
						Stream stream = null;
						try
						{
							ITempWavFile tempWavFile = TempFileFactory.CreateTempWavFile();
							fileStream = new FileStream(tempWavFile.FilePath, FileMode.Create);
							stream = configuration.GetStream();
							CommonUtil.CopyStream(stream, fileStream);
							result = tempWavFile;
						}
						catch (CorruptDataException)
						{
							this.Delete();
							result = null;
						}
						catch (InvalidOperationException)
						{
							this.Delete();
							result = null;
						}
						finally
						{
							if (fileStream != null)
							{
								fileStream.Dispose();
							}
							if (stream != null)
							{
								stream.Dispose();
							}
						}
					}
				}
			}
			return result;
		}

		internal override bool Exists()
		{
			bool result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.umuser.CreateSessionLock())
			{
				using (UserConfiguration configuration = this.GetConfiguration(mailboxSessionLock.Session, false))
				{
					result = (null != configuration);
				}
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XsoGreeting>(this);
		}

		private UserConfiguration GetConfiguration(MailboxSession session, bool create)
		{
			UserConfiguration result = null;
			try
			{
				result = session.UserConfigurationManager.GetMailboxConfiguration(this.Name, UserConfigurationTypes.Stream);
			}
			catch (ObjectNotFoundException)
			{
				if (create)
				{
					result = session.UserConfigurationManager.CreateMailboxConfiguration(this.Name, UserConfigurationTypes.Stream);
				}
			}
			catch (CorruptDataException)
			{
				this.Delete();
				if (create)
				{
					result = session.UserConfigurationManager.CreateMailboxConfiguration(this.Name, UserConfigurationTypes.Stream);
				}
			}
			catch (InvalidOperationException)
			{
				this.Delete();
				if (create)
				{
					result = session.UserConfigurationManager.CreateMailboxConfiguration(this.Name, UserConfigurationTypes.Stream);
				}
			}
			return result;
		}

		private UMSubscriber umuser;

		private string name;
	}
}
