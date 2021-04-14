using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	internal static class InitialsImageGenerator
	{
		public static Bitmap Generate(string text, int size)
		{
			string text2 = InitialsImageGenerator.MakeInitials(text);
			Bitmap bitmap = new Bitmap(size, size);
			Color color = InitialsImageGenerator.CreateRandomColor(text);
			using (Font font = new Font("Segoe UI", (float)size / 3f, FontStyle.Regular, GraphicsUnit.Pixel))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					using (Brush brush = new SolidBrush(InitialsImageGenerator.ContrastColor(color)))
					{
						SizeF sizeF = graphics.MeasureString(text2, font);
						float x = (float)size / 2f - sizeF.Width / 2f;
						float y = (float)size / 2f - sizeF.Height / 2f;
						PointF point = new PointF(x, y);
						graphics.Clear(color);
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
						graphics.DrawString(text2, font, brush, point);
						graphics.Flush();
					}
				}
			}
			return bitmap;
		}

		public static Stream GenerateAsStream(string text, int size)
		{
			Stream result;
			using (Bitmap bitmap = InitialsImageGenerator.Generate(text, size))
			{
				MemoryStream memoryStream = new MemoryStream(65536);
				bitmap.Save(memoryStream, ImageFormat.Png);
				result = memoryStream;
			}
			return result;
		}

		private static string MakeInitials(string text)
		{
			MatchCollection matchCollection = InitialsImageGenerator.GroupNameRegex.Matches(text);
			StringBuilder stringBuilder = new StringBuilder(4);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				stringBuilder.Append(char.ToUpper(match.Value[0]));
				stringBuilder.Append('\u0018');
				if (stringBuilder.Length >= 4)
				{
					break;
				}
			}
			if (stringBuilder.Length <= 2 && matchCollection[0].Value.Length > 1)
			{
				stringBuilder.Append(char.ToUpper(matchCollection[0].Value[1]));
				stringBuilder.Append('\u0018');
			}
			return stringBuilder.ToString().Trim();
		}

		private static Color CreateRandomColor(string text)
		{
			Color result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				byte[] value = sha256Cng.ComputeHash(Encoding.Unicode.GetBytes(text));
				long num = (long)((ulong)BitConverter.ToUInt32(value, 0) % (ulong)((long)InitialsImageGenerator.RandomColors.Length));
				result = InitialsImageGenerator.RandomColors[(int)(checked((IntPtr)num))];
			}
			return result;
		}

		private static Color ContrastColor(Color color)
		{
			double num = 0.2126 * Math.Pow((double)color.R / 255.0, 2.2) + 0.7152 * Math.Pow((double)color.G / 255.0, 2.2) + 0.0722 * Math.Pow((double)color.B / 255.0, 2.2);
			if (num < 0.5)
			{
				return Color.White;
			}
			return Color.Black;
		}

		private const char SpaceChar = '\u0018';

		private const int ImageMemorySteamBufferSize = 65536;

		private const double LuminosityMaxColor = 255.0;

		private const double LuminosityColorPower = 2.2;

		private const double LuminosityRedWeight = 0.2126;

		private const double LuminosityGreenWeight = 0.7152;

		private const double LuminosityBlueWeight = 0.0722;

		private static readonly Color[] RandomColors = new Color[]
		{
			Color.FromArgb(153, 180, 51),
			Color.FromArgb(107, 165, 231),
			Color.FromArgb(231, 115, 189),
			Color.FromArgb(0, 163, 0),
			Color.FromArgb(30, 113, 69),
			Color.FromArgb(255, 0, 151),
			Color.FromArgb(126, 56, 120),
			Color.FromArgb(96, 60, 186),
			Color.FromArgb(29, 29, 29),
			Color.FromArgb(0, 171, 169),
			Color.FromArgb(45, 137, 239),
			Color.FromArgb(43, 87, 151),
			Color.FromArgb(218, 83, 44),
			Color.FromArgb(185, 29, 71),
			Color.FromArgb(238, 17, 17)
		};

		private static readonly Regex GroupNameRegex = new Regex("\\S+", RegexOptions.Compiled);
	}
}
