using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Telemetry
{
    class LapTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "0";
            float lapTime = (float)value;
            int mins = (int)lapTime / 60;
            int secs = (int)(lapTime - mins * 60);
            string time = lapTime.ToString();
            int index = time.IndexOf(".");
            if (index > 0 && lapTime > 0)
            {
                string milli = time.Substring(index + 1, time.Length-index - 1);
                result = "" + mins + ":" + secs + ":" + milli;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
