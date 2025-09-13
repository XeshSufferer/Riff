using System.Globalization;

namespace Riff.Converters
{
    public class IsCurrentUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string senderId)
            {
                var currentUserId = Preferences.Get("USERID", "");
                return senderId == currentUserId;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
