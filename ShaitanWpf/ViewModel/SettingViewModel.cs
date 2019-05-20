using ShaitanWpf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ShaitanWpf.ViewModel
{
    class SettingViewModel: INotifyPropertyChanged
    {

        private CultureInfo selectedLang;
        public CultureInfo SelectedLang
        {
            get
            {
                return selectedLang;
            }
            set
            {
                if(selectedLang != value)
                {
                    selectedLang = value;
                    ChangeLanguage(selectedLang);
                }
                OnPropertyChanged("SelectedLang");
            }
        }

        public List<CultureInfo> LangList
        {
            get { return App.Languages; }
           
        }

        public SettingViewModel()
        {
            SelectedLang = App.Language;
            App.LanguageChanged += LanguageChanged;
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            SelectedLang = currLang;
        }

        private void ChangeLanguage(CultureInfo lang)
        {

                if (lang != null)
                {
                    App.Language = lang;
                }
          
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
