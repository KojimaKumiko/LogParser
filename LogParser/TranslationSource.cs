using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace LogParser
{
    public class TranslationSource : INotifyPropertyChanged
    {
        private static readonly TranslationSource instance = new TranslationSource();

        private readonly ResourceManager resManager = Resources.Resource.ResourceManager;

        private CultureInfo currentCulture = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public static TranslationSource Instance
        {
            get { return instance; }
        }

        public CultureInfo CurrentCulture
        {
            get { return currentCulture; }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    var propChanged = PropertyChanged;
                    propChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        public string this[string key]
        {
            get { return resManager.GetString(key, currentCulture); }
        }
    }

    public class LocExtension : Binding
    {
        public LocExtension(string name) : base($"[{name}]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}
