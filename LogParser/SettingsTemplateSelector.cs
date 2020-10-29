using Database.Models;
using Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LogParser
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BooleanTemplate { get; set; }

        public DataTemplate IntegerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Settings setting = (Settings)item;

            if (setting == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return setting.SettingsType switch
            {
                SettingsType.Boolean => BooleanTemplate,
                SettingsType.Integer => IntegerTemplate,
                _ => base.SelectTemplate(item, container),
            };
        }
    }
}
