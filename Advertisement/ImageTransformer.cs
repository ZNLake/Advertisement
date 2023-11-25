using System.Drawing;
using System.Drawing.Imaging;
class AdCreative 
{
    static async Task AdCreativeTransformer (string url, string prod_name, string prod_price, string prod_categ) //takes in a product image url, overlays ad creative, saves final ad as ad output.png within AdCreatives folder
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

        Image img = new Bitmap(imageAdTemplate.Width, imageAdTemplate.Height);
        using (Graphics gr = Graphics.FromImage(img))
        {
            gr.DrawImage(imageAdTemplate, new Point(0, 0));
            gr.DrawImage(imageProduct, new Point(0, 0));
            gr.DrawString(prod_name, new Font("Verdana", 24), Brushes.Black, new PointF(25, 10));
            gr.DrawString(prod_categ, new Font("Verdana", 24), Brushes.Black, new PointF(200, 5));
            gr.DrawString('$' + prod_price, new Font("Verdana", 24), Brushes.Black, new PointF(25, 50));


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
        var files = Directory.GetFiles("AdCreatives", "*.jpg");
        Image image = Image.FromFile(files[rand.Next(files.Length)]);
        return image;
    }
}