using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace LogParser.Converter
{
    public class ResourceConverter : MarkupExtension, IValueConverter
    {
        private Control target;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resource = (string)value;

            return target?.FindResource(resource) ?? resource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (rootObjectProvider == null)
            {
                return this;
            }

            target = rootObjectProvider.RootObject as Control;
            return this;
        }
    }
}
