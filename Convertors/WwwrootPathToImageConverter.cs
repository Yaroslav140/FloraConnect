using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FlowerShop.WpfClient.Convertors
{
    public class WwwrootPathToImageConverter : IValueConverter
    {
        public string Wwwroot { get; set; } = @"C:\Users\param\source\repos\FlowerShopApp\FlowerShop\wwwroot";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string rel || string.IsNullOrWhiteSpace(rel))
                return null;

            var relative = rel.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);

            var fullPath = Path.IsPathRooted(relative)
                ? relative
                : Path.Combine(Wwwroot, relative);

            if (!File.Exists(fullPath))
                return null;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
