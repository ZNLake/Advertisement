using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
class AdCreative 
{
    static async Task AdCreativeTransformer (string url, int Width, int Height, string prod_name, string prod_price, string prod_categ) //takes in a product image url, overlays ad creative, saves final ad as ad output.png within AdCreatives folder
    {
        Image imageAdTemplate = Image.FromFile("AdCreatives/overlay.png"); //TODO: default working directory unknown, file paths inaccurate
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {

            }
        }
        Image imageProduct = Image.FromFile("AdCreatives/product.png");
        ResizeImage(imageProduct, Width, Height);
        Image img = new Bitmap(imageAdTemplate.Width, imageAdTemplate.Height);
        using (Graphics gr = Graphics.FromImage(img))
        {
            gr.DrawImage(imageAdTemplate, new Point(0, 0));
            gr.DrawImage(imageProduct, new Point(0, 0));
            gr.DrawString('$' + prod_price, new Font("Verdana", 24), Brushes.Black, new PointF(Width / 10, (Height / 10)));
            gr.DrawString(prod_name, new Font("Verdana", 24), Brushes.Black, new PointF((Width/10) + 20, Height/10));
            gr.DrawString(prod_categ, new Font("Verdana", 24), Brushes.Black, new PointF(Width/10, Height/10 + 10));


        }
        img.Save("AdCreatives/" + prod_name + "_ad.png", ImageFormat.Png);
    }

    static Image ReturnAd(string image) //return a specific image using image name (prod_name)
    {
        Image ad = Image.FromFile("AdCreatives/" + image + ".png");
        return ad;
    }
    static Image RequestAd() //return a random ad in image format from the AdCreatives folder
    {
        var rand = new Random();
        var files = Directory.GetFiles("AdCreatives", "*.png");
        Image image = Image.FromFile(files[rand.Next(files.Length)]);
        return image;
    }

    public static Bitmap ResizeImage(Image image, int Width, int Height) //helper 
    {
  
        var destRect = new Rectangle(0, 0, Width, Height);
        var destImage = new Bitmap(Width, Height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }
}