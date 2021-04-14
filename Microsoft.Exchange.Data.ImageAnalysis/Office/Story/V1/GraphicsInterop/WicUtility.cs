using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;
using Microsoft.Office.Story.V1.GraphicsInterop.Wic;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	internal static class WicUtility
	{
		public static IWICImagingFactory CreateFactory()
		{
			return GraphicsInteropNativeMethods.CreateComInstanceFromInterface<IWICImagingFactory>(WicGuids.CLSID_WICImagingFactory);
		}

		public static IWICBitmapFrameDecode Load(this IWICImagingFactory factory, string fileName)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			IWICBitmapDecoder iwicbitmapDecoder = factory.CreateDecoderFromFilename(fileName, null, GenericAccess.GENERIC_READ, WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
			IWICBitmapFrameDecode result;
			iwicbitmapDecoder.GetFrame(0, out result);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapDecoder);
			return result;
		}

		public static IWICBitmapFrameDecode Load(this IWICImagingFactory factory, Stream stream)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			StreamWrapper pIStream = new StreamWrapper(stream);
			IWICBitmapDecoder iwicbitmapDecoder = factory.CreateDecoderFromStream(pIStream, null, WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
			IWICBitmapFrameDecode result;
			iwicbitmapDecoder.GetFrame(0, out result);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapDecoder);
			return result;
		}

		public static IWICBitmap ToBitmap(this IWICImagingFactory factory, IWICBitmapSource source)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IWICBitmap result;
			factory.CreateBitmapFromSource(source, WICBitmapCreateCacheOption.WICBitmapCacheOnDemand, out result);
			return result;
		}

		public static IWICBitmapSource ConvertFormat(this IWICImagingFactory factory, IWICBitmapSource source, Guid imageFormat)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			IWICFormatConverter iwicformatConverter;
			factory.CreateFormatConverter(out iwicformatConverter);
			iwicformatConverter.Initialize(source, ref imageFormat, WICBitmapDitherType.WICBitmapDitherTypeNone, null, 0.0, WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);
			return iwicformatConverter;
		}

		public static IWICBitmapSource GetScaledImageSource(this IWICImagingFactory factory, IWICBitmapSource source, float scale)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num;
			int num2;
			source.GetSize(out num, out num2);
			IWICBitmapScaler iwicbitmapScaler;
			factory.CreateBitmapScaler(out iwicbitmapScaler);
			iwicbitmapScaler.Initialize(source, (int)((float)num / scale), (int)((float)num2 / scale), WICBitmapInterpolationMode.WICBitmapInterpolationModeFant);
			return iwicbitmapScaler;
		}

		public static IWICBitmapSource GetOrientedImageSource(this IWICImagingFactory factory, IWICBitmapSource source, WICBitmapTransformOptions opt)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (opt == WICBitmapTransformOptions.WICBitmapTransformRotate0)
			{
				return source;
			}
			IWICBitmap iwicbitmap = null;
			factory.CreateBitmapFromSource(source, WICBitmapCreateCacheOption.WICBitmapCacheOnDemand, out iwicbitmap);
			IWICBitmapFlipRotator iwicbitmapFlipRotator = null;
			factory.CreateBitmapFlipRotator(out iwicbitmapFlipRotator);
			iwicbitmapFlipRotator.Initialize(iwicbitmap, opt);
			IWICBitmapSource result = iwicbitmapFlipRotator;
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmap);
			return result;
		}

		public static float ComputeDownScale(int width, int height, int targetSize, Func<float, float, float> sizeSelector = null)
		{
			if (sizeSelector == null)
			{
				sizeSelector = new Func<float, float, float>(Math.Max);
			}
			return Math.Max(sizeSelector((float)width, (float)height) / (float)targetSize, 1f);
		}

		public static float ComputeDownScale(this IWICBitmapSource imageSource, int targetSize, Func<float, float, float> sizeSelector = null)
		{
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			int width;
			int height;
			imageSource.GetSize(out width, out height);
			return WicUtility.ComputeDownScale(width, height, targetSize, sizeSelector);
		}

		public static void Save(this IWICImagingFactory factory, IWICBitmapSource imageSource, IWICStream stream, Guid encoderId)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			IWICBitmapEncoder iwicbitmapEncoder = factory.CreateEncoder(ref encoderId, null);
			iwicbitmapEncoder.Initialize(stream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);
			IWICBitmapFrameEncode iwicbitmapFrameEncode;
			iwicbitmapEncoder.CreateNewFrame(out iwicbitmapFrameEncode, null);
			iwicbitmapFrameEncode.Initialize(null);
			iwicbitmapFrameEncode.WriteSource(imageSource, null);
			iwicbitmapFrameEncode.Commit();
			iwicbitmapEncoder.Commit();
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapEncoder);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapFrameEncode);
		}

		public static void Save(this IWICImagingFactory factory, IWICBitmapSource imageSource, Stream destinationStream, Guid encoderId)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			IWICStream iwicstream;
			factory.CreateStream(out iwicstream);
			iwicstream.InitializeFromIStream(new StreamWrapper(destinationStream));
			factory.Save(imageSource, iwicstream, encoderId);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicstream);
		}

		public static void Save(this IWICImagingFactory factory, IWICBitmapSource imageSource, string fileName)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			string extension = Path.GetExtension(fileName);
			if (string.IsNullOrEmpty(extension))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "File name '{0}' does not have an extension so encoder type can not be determined.", new object[]
				{
					fileName
				}));
			}
			Guid encoderId;
			if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
			{
				encoderId = WicGuids.GUID_ContainerFormatJpeg;
			}
			else if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
			{
				encoderId = WicGuids.GUID_ContainerFormatPng;
			}
			else if (extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
			{
				encoderId = WicGuids.GUID_ContainerFormatBmp;
			}
			else
			{
				if (!extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "File name '{0}' has extension {1} which we can't handle.", new object[]
					{
						fileName,
						extension
					}));
				}
				encoderId = WicGuids.GUID_ContainerFormatTiff;
			}
			IWICStream iwicstream;
			factory.CreateStream(out iwicstream);
			iwicstream.InitializeFromFilename(fileName, GenericAccess.GENERIC_WRITE);
			factory.Save(imageSource, iwicstream, encoderId);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicstream);
		}

		public static void FillBlobFromBitmapSource(this IWICImagingFactory factory, IWICBitmapSource source, Array blob, Guid imageFormat)
		{
			IWICBitmapSource iwicbitmapSource = factory.ConvertFormat(source, imageFormat);
			iwicbitmapSource.FillBlobFromBitmapSource(blob);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapSource);
		}

		public static void FillBlobFromBitmapSource(this IWICImagingFactory factory, IWICBitmapSource source, IntPtr blob, int bufferStride, int bufferSize, Guid imageFormat)
		{
			IWICBitmapSource iwicbitmapSource = factory.ConvertFormat(source, imageFormat);
			iwicbitmapSource.FillBlobFromBitmapSource(blob, bufferStride, bufferSize);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapSource);
		}

		public static void FillBlobFromBitmapSource(this IWICBitmapSource bitmap, IntPtr blob, int bufferStride, int bufferSize)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			WICRect wicrect = default(WICRect);
			bitmap.GetSize(out wicrect.Width, out wicrect.Height);
			using (PinHelper pinHelper = new PinHelper(blob))
			{
				bitmap.CopyPixels(ref wicrect, bufferStride, bufferSize, pinHelper.Addr);
			}
		}

		public static void FillBlobFromBitmapSource(this IWICBitmapSource bitmap, Array blob)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			WICRect wicrect = default(WICRect);
			bitmap.GetSize(out wicrect.Width, out wicrect.Height);
			int arrayStride = WicUtility.GetArrayStride(blob, wicrect.Width, wicrect.Height);
			int arrayBufferSize = WicUtility.GetArrayBufferSize(blob);
			using (PinHelper pinHelper = new PinHelper(blob))
			{
				bitmap.CopyPixels(ref wicrect, arrayStride, arrayBufferSize, pinHelper.Addr);
			}
		}

		public static IWICBitmap CreateBitmapFromBlob(this IWICImagingFactory factory, Array blob, int width, int height, int stride, Guid imageFormat)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			int arrayBufferSize = WicUtility.GetArrayBufferSize(blob);
			IWICBitmap result;
			using (PinHelper pinHelper = new PinHelper(blob))
			{
				factory.CreateBitmapFromMemory(width, height, ref imageFormat, stride, arrayBufferSize, pinHelper.Addr, out result);
			}
			return result;
		}

		public static int GetArrayStride(Array buffer, int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			int elementSize = WicUtility.GetElementSize(buffer);
			return elementSize * width;
		}

		public static int GetArrayBufferSize(Array buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int elementSize = WicUtility.GetElementSize(buffer);
			return elementSize * buffer.Length;
		}

		public static int GetElementSize(Array buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			Type type = buffer.GetType();
			Type elementType = type.GetElementType();
			return Marshal.SizeOf(elementType);
		}

		public static bool TryGetMetadataProperty<T>(this IWICMetadataQueryReader queryReader, string propertyPath, out T value)
		{
			if (queryReader == null)
			{
				throw new ArgumentNullException("queryReader");
			}
			value = default(T);
			PROPVARIANT propvariant = default(PROPVARIANT);
			int metadataByName = queryReader.GetMetadataByName(propertyPath, out propvariant);
			if (-2003292352 != metadataByName)
			{
				GraphicsInteropNativeMethods.CheckNativeResult(metadataByName);
				value = (T)((object)propvariant.Value);
				propvariant.Dispose();
				return true;
			}
			propvariant.Dispose();
			return false;
		}
	}
}
