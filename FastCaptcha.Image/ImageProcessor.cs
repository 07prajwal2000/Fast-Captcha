using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace FastCaptcha.ImageProcessing;

public enum ImageFormats
{
    Jpeg, Png, Webp
}

public class ImageProcessor
{
    public string GenerateImageFromText(string text, ImageFormats formats = ImageFormats.Webp)
    {
        using var image = new Image<Rgba32>(150, 50);
        
        var options = ImageProcessorExtensions.ImageOptions;
        
        image.Mutate(x =>
        {
            x.DrawImage(ImageProcessorExtensions.BackgroundImage, 1);
            x.DrawText(options, text, Color.Black);
            x.GaussianBlur(1.6f);
        });
        // using var ms = new MemoryStream();
        // image.SaveAsync(ms, JpegFormat.Instance).GetAwaiter().GetResult();
        // return ms.ToArray();
        
        var imageFormat = formats switch
        {
            ImageFormats.Jpeg => JpegFormat.Instance,
            ImageFormats.Png => PngFormat.Instance,
            ImageFormats.Webp => WebpFormat.Instance as IImageFormat,
            _ => WebpFormat.Instance
        };
        return image.ToBase64String(imageFormat);
    }
}

public static class ImageProcessorExtensions
{
    private static readonly FontCollection FontCollection = new();
    public static readonly Image BackgroundImage;
    public static readonly RichTextOptions ImageOptions;
    
    static ImageProcessorExtensions()
    {
        var dir = Path.GetDirectoryName(typeof(ImageProcessorExtensions).Assembly.Location)!;
        var path = Path.Combine(dir, "Fonts/ARIAL.TTF");
        var family = FontCollection.Add(path);
        
        var arialFont = family.CreateFont(38, FontStyle.BoldItalic);
        
        BackgroundImage = Image.Load(Path.Combine(dir, "CaptchaBackgrounds/captcha_bg.jpg"));
        ImageOptions = new RichTextOptions(arialFont)
        {
            Origin = new PointF(0, 0),
            Dpi = 50,
            Path = new SixLabors.ImageSharp.Drawing.Path(new PointF[]
            {
                new (3, 17), new (47, 8), new (87, 13), new (125, 20), new(150, 30)
            }),
            TextJustification = TextJustification.InterCharacter,
        };
    }

    public static void AddImageProcessorServices(this IServiceCollection services)
    {
        services.AddScoped<ImageProcessor>();
    }
}