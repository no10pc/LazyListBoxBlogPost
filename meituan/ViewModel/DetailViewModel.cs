using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using meituan.Model;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Microsoft.Phone.Shell;

namespace meituan.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class DetailViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the DetailViewModel class.
        /// </summary>
        /// 
        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string MyPropertyPropertyName = "deal_title";

        private string _deal_title;

        /// <summary>
        /// Sets and gets the MyProperty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string deal_title
        {
            get
            {
                return _deal_title;
            }

            set
            {
                if (_deal_title == value)
                {
                    return;
                }

                _deal_title = value;
                RaisePropertyChanged(MyPropertyPropertyName);
            }
        }


        public DetailViewModel()
        {
            
        }
        public ICommand GetMessage
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Register<Deal>(this, msg => { MessageBox.Show(msg.Deal_title); });
                });
            }
        }
    }
}