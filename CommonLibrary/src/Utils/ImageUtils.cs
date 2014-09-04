using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace CommonLibrary
{
  /// <summary>
  /// Формат изображения.
  /// </summary>
  public enum ImageFormat
  {
    /// <summary>
    /// Bmp.
    /// </summary>
    Bmp,

    /// <summary>
    /// Ico.
    /// </summary>
    Ico,

    /// <summary>
    /// Gif.
    /// </summary>
    Gif,

    /// <summary>
    /// Jpeg.
    /// </summary>
    Jpeg,

    /// <summary>
    /// Png.
    /// </summary>
    Png,

    /// <summary>
    /// Tiff.
    /// </summary>
    Tiff,

    /// <summary>
    /// Wmp.
    /// </summary>
    Wmp
  }

  /// <summary>
  /// Вспомогательный класс для работы с изображениями.
  /// </summary>
  public static class ImageUtils
  {
    #region Константы

    /// <summary>
    /// Префикс формата пути к ресурсу.
    /// </summary>
    private const string ResourceUriPrefix = "pack://application:,,,/";

    /// <summary>
    /// Размер буфера при загрузке изображения.
    /// </summary>
    private const int BufferSize = 65536;

    public const int DefaultImageWidth = 300;

    public const int DefaultImageHeight = 300;

    /// <summary>
    /// Разрешение изображения по-умолчанию.
    /// </summary>
    public const int DefaultImageDpi = 96;

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Константы с гуидами контейнеров для разных форматов изображений.
    /// </summary>
    private static readonly Guid ContainerFormatBmp = new Guid(0xaf1d87e, 0xfcfe, 0x4188, 0xbd, 0xeb, 0xa7, 0x90, 100, 0x71, 0xcb, 0xe3);
    private static readonly Guid ContainerFormatIco = new Guid(0xa3a860c4, 0x338f, 0x4c17, 0x91, 0x9a, 0xfb, 0xa4, 0xb5, 0x62, 0x8f, 0x21);
    private static readonly Guid ContainerFormatGif = new Guid(0x1f8a5601, 0x7d4d, 0x4cbd, 0x9c, 130, 0x1b, 200, 0xd4, 0xee, 0xb9, 0xa5);
    private static readonly Guid ContainerFormatJpeg = new Guid(0x19e4a5aa, 0x5662, 0x4fc5, 160, 0xc0, 0x17, 0x58, 2, 0x8e, 0x10, 0x57);
    private static readonly Guid ContainerFormatPng = new Guid(0x1b7cfaf4, 0x713f, 0x473c, 0xbb, 0xcd, 0x61, 0x37, 0x42, 0x5f, 0xae, 0xaf);
    private static readonly Guid ContainerFormatTiff = new Guid(0x163bcc30, 0xe2e9, 0x4f0b, 150, 0x1d, 0xa3, 0xe9, 0xfd, 0xb7, 0x88, 0xa3);
    private static readonly Guid ContainerFormatWmp = new Guid(0x57a37caa, 0x367a, 0x4540, 0x91, 0x6b, 0xf1, 0x83, 0xc5, 9, 0x3a, 0x4b);

    private static readonly Guid[] ImageContainerFormats =
    {
      ContainerFormatBmp, ContainerFormatIco, ContainerFormatGif, ContainerFormatJpeg,
      ContainerFormatPng, ContainerFormatTiff, ContainerFormatWmp
    };

    /// <summary>
    /// Кэш для декодированных картинок.
    /// </summary>
    private static readonly ConcurrentDictionary<Uri, BitmapDecoder> ImageDecoderCache = new ConcurrentDictionary<Uri, BitmapDecoder>();

    /// <summary>
    /// Пустое изображение.
    /// </summary>
    /// <remarks>Используется для нормального определения ковертером пустого изображения.</remarks>
    private static Lazy<BitmapImage> emptyImage = new Lazy<BitmapImage>(() => new BitmapImage(), true);

    /// <summary>
    /// Получить или установить признак, который показывает игнорировать ли декодирование изображения.
    /// </summary>
    public static bool IgnorePixelDecode { get; set; }

    #endregion

    #region Методы

    /// <summary>
    /// Установить "визуальные" размеры картинки в оригинальные размеры источника независимо от системного DPI.
    /// </summary>
    /// <param name="image">Картинка.</param>
    public static void SetImageSizeToSourceSizeIgnoringSystemDpiSettings(Image image)
    {
      image.SetBinding(FrameworkElement.WidthProperty, new Binding("Source.PixelWidth") { Source = image, Converter = DpiScaledWidthToOriginalWidthConverter.Instance });
      image.SetBinding(FrameworkElement.HeightProperty, new Binding("Source.PixelHeight") { Source = image, Converter = DpiScaledHeightToOriginalHeightConverter.Instance });
    }

    /// <summary>
    /// Получить формат контейнера изображения.
    /// </summary>
    /// <param name="imageFormat">Формат изображения.</param>
    /// <returns>Формат контейнера изображения.</returns>
    public static Guid GetImageContainerFormat(ImageFormat? imageFormat)
    {
      return imageFormat.HasValue ? ImageContainerFormats[(int)imageFormat.Value] : Guid.Empty;
    }

    /// <summary>
    /// Получение формата изображения по строке формата.
    /// </summary>
    /// <param name="value">Строка формата.</param>
    /// <returns>Формат изображения.</returns>
    public static ImageFormat? GetImageFormatFromString(string value)
    {
      // TODO: тупой switch
      switch (value.ToUpperInvariant())
      {
        case "JPG":
        case "JPEG":
        case "JPE":
        case "JFIF":
          return ImageFormat.Jpeg;
        case "GIF":
          return ImageFormat.Gif;
        case "ICO":
          return ImageFormat.Ico;
        case "BMP":
        case "DIB":
          return ImageFormat.Bmp;
        case "PNG":
          return ImageFormat.Png;
        case "TIF":
        case "TIFF":
          return ImageFormat.Tiff;
        case "WMP":
          return ImageFormat.Wmp;
        default:
          return null;
      }
    }

    /// <summary>
    /// Сохранить изображение в поток.
    /// </summary>
    /// <param name="outputStream">Исходящий поток, полученный из изображения.</param>
    /// <param name="imageFormat">Формат изображения.</param>
    /// <param name="source">Источник изображения.</param>
    public static void SaveImageToStream(Stream outputStream, ImageFormat? imageFormat, BitmapSource source)
    {
      BitmapEncoder encoder = BitmapEncoder.Create(GetImageContainerFormat(imageFormat));
      encoder.Frames.Add((BitmapFrame)source);
      encoder.Save(outputStream);
    }

    /// <summary>
    /// Загрузить изображение из потока.
    /// </summary>
    /// <param name="inputStream">Входящий поток.</param>
    /// <param name="decodePixelWidth">Ширина декодирования.</param>
    /// <param name="decodePixelHeight">Высота декодирования.</param>
    /// <returns>Изображение.</returns>
    public static BitmapSource LoadImageFromStream(Stream inputStream, int decodePixelWidth, int decodePixelHeight)
    {
      if (inputStream != Stream.Null)
      {
        try
        {
          if (inputStream.CanSeek)
            inputStream.Position = 0;

          using (MemoryStream memoryStream = new MemoryStream())
          {
            inputStream.CopyTo(memoryStream, BufferSize);
            memoryStream.Position = 0;

            BitmapImage result = ImageUtils.BeginInit();
            if ((decodePixelWidth > 0 || decodePixelHeight > 0) && !ImageUtils.IgnorePixelDecode)
            {
              BitmapDecoder decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
              BitmapSource bitmapSource = decoder.Frames[0];

              memoryStream.Position = 0;
              result.StreamSource = memoryStream;
              ImageUtils.MeasureSize(ref result, bitmapSource, decodePixelWidth, decodePixelHeight);
            }
            else
              result.StreamSource = memoryStream;
            ImageUtils.EndInit(result);
            return result;
          }
        }
        catch (NotSupportedException)
        {
          return null;
        }
      }
      else
        return emptyImage.Value;
    }

    /// <summary>
    /// Декодировать изображение из массива байт.
    /// </summary>
    /// <param name="imageSource">Изображение в виде массива байт.</param>
    /// <param name="decodePixelWidth">Ширина декодирования.</param>
    /// <param name="decodePixelHeight">Высота декодирования.</param>
    /// <returns>Изображение.</returns>
    public static BitmapSource DecodeImageFromByteArray(byte[] imageSource, int decodePixelWidth, int decodePixelHeight)
    {
      try
      {
        if (imageSource == null)
          return null;
        using (MemoryStream memoryStream = new MemoryStream(imageSource))
        {
          BitmapImage result = ImageUtils.BeginInit();
          if ((decodePixelWidth > 0 || decodePixelHeight > 0) && !ImageUtils.IgnorePixelDecode)
          {
            BitmapDecoder decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapSource bitmapSource = decoder.Frames[0];

            memoryStream.Position = 0;
            result.StreamSource = memoryStream;
            ImageUtils.MeasureSize(ref result, bitmapSource, decodePixelWidth, decodePixelHeight);
          }
          else
            result.StreamSource = memoryStream;
          ImageUtils.EndInit(result);
          return result;
        }
      }
      catch (NotSupportedException)
      {
        return null;
      }
    }

    /// <summary>
    /// Получить изображение из массива байт.
    /// </summary>
    /// <param name="imageSource">Изображение в виде массива байт.</param>
    /// <returns>Изображение.</returns>
    public static BitmapSource GetImageFromByteArray(byte[] imageSource)
    {
      try
      {
        if (imageSource == null)
          return null;
        using (var stream = new MemoryStream(imageSource))
        {
          BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
          BitmapSource result = decoder.Frames[0];
          return result;
        }
      }
      catch (NotSupportedException)
      {
        return null;
      }
    }

    /// <summary>
    /// Кодировать изображение в массив байт.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <returns>Массив байт.</returns>
    public static byte[] EncodeImageFromBitmapSource(BitmapSource image)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        var formatString = ((BitmapMetadata)image.Metadata).Format;
        var imageFormat = ImageUtils.GetImageFormatFromString(formatString);
        BitmapEncoder encoder = BitmapEncoder.Create(GetImageContainerFormat(imageFormat));
        encoder.Frames.Add(BitmapFrame.Create(image, null, (BitmapMetadata)image.Metadata, ((BitmapFrame)image).ColorContexts));
        encoder.Save(stream);
        return stream.ToArray();
      }
    }

    /// <summary>
    /// Декодировать изображение в нужный размер.
    /// </summary>
    /// <param name="inputImage">Изображение для декодирования.</param>
    /// <param name="decodePixelWidth">Ширина декодирования.</param>
    /// <param name="decodePixelHeight">Высота декодирования.</param>
    /// <returns>Декодированное изображение.</returns>
    public static BitmapSource DecodeImageFromBitmapFrame(BitmapSource inputImage, int decodePixelWidth, int decodePixelHeight)
    {
      try
      {
        using (var stream = new MemoryStream())
        {
          ImageUtils.SaveImageToStream(stream, ImageUtils.GetImageFormatFromString(((BitmapMetadata)inputImage.Metadata).Format), inputImage);
          stream.Position = 0;

          BitmapImage result = ImageUtils.BeginInit();
          if ((decodePixelWidth > 0 || decodePixelHeight > 0) && !ImageUtils.IgnorePixelDecode)
          {
            result.StreamSource = stream;
            ImageUtils.MeasureSize(ref result, inputImage, decodePixelWidth, decodePixelHeight);
          }
          else
            result.StreamSource = stream;
          ImageUtils.EndInit(result);
          return result;
        }
      }
      catch (NotSupportedException)
      {
        return null;
      }
    }

    /// <summary>
    /// Изменить размеры картинки.
    /// </summary>
    /// <param name="image">Исходная картинка.</param>
    /// <param name="width">Ширина.</param>
    /// <param name="height">Высота.</param>
    /// <returns>Картинка с измененными размерами.</returns>
    public static BitmapSource ResizeImage(BitmapFrame image, int width, int height)
    {
      BitmapSource img = image;
      var cache = new CachedBitmap(img, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
      double scaleFactor = img.Width > img.Height ? (double)width / img.Width : (double)height / img.Height;

      BitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(new TransformedBitmap(cache, new ScaleTransform(scaleFactor, scaleFactor)),
        null, (BitmapMetadata)image.Metadata == null ? null : (BitmapMetadata)image.Metadata.Clone(), image.ColorContexts));

      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        stream.Position = 0;
        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
      }
    }

    /// <summary>
    /// Получить Uri изображение по его пути к ресурсу.
    /// </summary>
    /// <param name="imagePath">Путь к ресурсу изображения.</param>
    /// <returns>Uri изображения.</returns>
    public static Uri GetImageUri(string imagePath)
    {
      return string.IsNullOrEmpty(imagePath) ? null : new Uri(imagePath, UriKind.RelativeOrAbsolute);
    }

    /// <summary>
    /// Получить изображение из ресурса по его uri.
    /// </summary>
    /// <param name="imageUri">Uri ресурса с изображением.</param>
    /// <returns>Изображение.</returns>
    public static BitmapSource GetImage(Uri imageUri)
    {
      if (imageUri == null)
        return null;

      var decoder = GetBitmapDecoder(imageUri);
      return decoder.Frames.FirstOrDefault();
    }

    /// <summary>
    /// Конвертировать изображение в формат Png.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <returns>Изображение в формате Png.</returns>
    public static BitmapSource ConvertImageToPng(BitmapSource image)
    {
      if (image == null)
        return null;
      
      BitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(image));
      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        stream.Position = 0;
        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
      }
    }

    /// <summary>
    /// Получить изображение из ресурса по его uri.
    /// </summary>
    /// <param name="imageUri">Uri ресурса с изображением.</param>
    /// <param name="preferredSize">Предпочтительный размер изображения.</param>
    /// <returns></returns>
    public static BitmapSource GetImage(Uri imageUri, int preferredSize)
    {
      if (imageUri == null)
        return null;

      var decoder = GetBitmapDecoder(imageUri);
      return GetDecoderFrame(decoder, preferredSize);
    }

    /// <summary>
    /// Добавить наложение на изображение.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <param name="overlay">Налагаемое изображение.</param>
    /// <returns>Изображние с наложением.</returns>
    public static BitmapSource AddOverlayToImage(ImageSource image, ImageSource overlay)
    {
      // TODO: Mosolov_KN. В данный момент изображение накладывается в правый нижний угол, в идеале хотелось бы управлять этим через параметр типа ContentAlignment.
      var group = new DrawingGroup();
      group.Children.Add(new ImageDrawing(image, new Rect(0, 0, image.Width, image.Height)));
      group.Children.Add(new ImageDrawing(overlay, new Rect(image.Width - overlay.Width, image.Height - overlay.Height, overlay.Width, overlay.Height)));

      var drawingVisual = new DrawingVisual();
      var drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawDrawing(group);
      drawingContext.Close();

      var targetBitmap = new RenderTargetBitmap((int)group.Bounds.Width, (int)group.Bounds.Height, DefaultImageDpi, DefaultImageDpi, PixelFormats.Pbgra32);
      targetBitmap.Render(drawingVisual);

      var encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
      }
    }

    /// <summary>
    /// Получить основной цвет изображения.
    /// </summary>
    /// <param name="picture">Изображения.</param>
    /// <returns>Основной цвет.</returns>
    public static System.Windows.Media.Color GetBasicColor(Bitmap picture)
    {
      var countColorsUsage = new Dictionary<System.Drawing.Color, int>();
      for (int row = 0; row < picture.Size.Width; row++)
      {
        for (int col = 0; col < picture.Size.Height; col++)
        {
          var c = picture.GetPixel(row, col);
          if (Math.Abs(c.R - c.G) < 10 && Math.Abs(c.R - c.B) < 10)
            continue;
          var pixelColor = picture.GetPixel(row, col);

          if (countColorsUsage.Keys.Contains(pixelColor))
            countColorsUsage[pixelColor]++;
          else
            countColorsUsage.Add(pixelColor, 1);
        }
      }
      var basicColor = countColorsUsage.OrderByDescending(x => x.Value).First().Key;
      return System.Windows.Media.Color.FromRgb(basicColor.R, basicColor.G, basicColor.B);
    }

    /// <summary>
    /// Пересчитать размер изображения.
    /// </summary>
    /// <param name="resultImage">Экземпляр типа BitmapImage размеры, которого будут пересчитаны.</param>
    /// <param name="bitmapSource">Источник оригинального изображения.</param>
    /// <param name="decodePixelWidth">Ширина области декодирования.</param>
    /// <param name="decodePixelHeight">Высота области декодирования.</param>
    private static void MeasureSize(ref BitmapImage resultImage, BitmapSource bitmapSource, int decodePixelWidth, int decodePixelHeight)
    {
      if (bitmapSource.PixelHeight > bitmapSource.PixelWidth)
        resultImage.DecodePixelHeight = bitmapSource.PixelHeight > decodePixelHeight ? decodePixelHeight : bitmapSource.PixelHeight;
      else
        resultImage.DecodePixelWidth = bitmapSource.PixelWidth > decodePixelWidth ? decodePixelWidth : bitmapSource.PixelWidth;
    }

    /// <summary>
    /// Инициализировать экземпляр типа BitmapImage.
    /// </summary>
    /// <returns>Экземпляр типа BitmapImage.</returns>
    private static BitmapImage BeginInit()
    {
      BitmapImage result = new BitmapImage();
      result.BeginInit();
      return result;
    }

    /// <summary>
    /// Завершить инициализацию экземпляра типа BitmapImage. Объект так же будет помечен как IsFrozen.
    /// </summary>
    /// <param name="bitmapImage">Экземпляр типа BitmapImage, который нужно инициализировать.</param>
    private static void EndInit(BitmapImage bitmapImage)
    {
      bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
      bitmapImage.EndInit();
      bitmapImage.Freeze();
    }

    /// <summary>
    /// Получить декодер изображений из ресурсов.
    /// </summary>
    /// <param name="imageUri">Uri ресурса изображения.</param>
    /// <returns>Декодер изображений.</returns>
    private static BitmapDecoder GetBitmapDecoder(Uri imageUri)
    {
      if (imageUri.IsFile)
      {
        using (var stream = File.Open(imageUri.LocalPath, FileMode.Open, FileAccess.ReadWrite))
          return BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
      }
      return ImageDecoderCache.GetOrAdd(imageUri,
        uri => BitmapDecoder.Create(imageUri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad));
    }

    /// <summary>
    /// Получить фрейм с изображением желаемого размера.
    /// </summary>
    /// <param name="decoder">Декодер.</param>
    /// <param name="preferredSize">Предпочтительный размер изображения.</param>
    /// <returns>Изображение.</returns>
    private static BitmapSource GetDecoderFrame(BitmapDecoder decoder, int preferredSize)
    {
      // TODO: Sokolov_AV. Разобраться с порядком сортировки по BitsPerPixel, подбором под неточный размер (здесь и в возможных копипастах).
      return decoder.Frames.Where(f => f.PixelWidth == preferredSize).OrderBy(f => f.Format.BitsPerPixel).FirstOrDefault() ??
        decoder.Frames.OrderBy(f => f.PixelWidth).First();
    }

    #endregion
  }
}