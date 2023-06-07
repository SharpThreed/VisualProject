using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualCOM.ContentHelp;
using VisualCOM.Model;
using VisualCOM.View;

namespace VisualCOM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private static SettingPage _settingPage = new SettingPage();
        private static UartPage _uartPage = new UartPage();
        private static ChartsPage _chartsPage = new ChartsPage();
        private static MorePage _morePage=new MorePage();

        public MainViewModel()
        {
            
            ThemeToggleCommand = new RelayCommand(ToggleTheme);
            WindowMinCommand = new RelayCommand(MinWindow);
            WindowCloseCommand = new RelayCommand(CloseWindow);
            OpenCommand = new RelayCommand<string>(OpenPage);
        }

        private object pageContent;

        public object PageContent
        {
            get { return pageContent; }
            set
            {
                if (pageContent == value)
                {
                    return;
                }
                pageContent = value;
                OnPropertyChanged(nameof(PageContent));
            }
        }


        private bool isTheme = true;
        public bool IsTheme
        {
            get { return isTheme; }
            set
            {
                isTheme = value;
                OnPropertyChanged(nameof(isTheme));
            }
        }

        #region method
        private void OpenPage(string para)
        {
            switch (para)
            {
                case "Uart":
                    {
                        PageContent = _uartPage;
                        break;
                    }
                case "Setting":
                    {
                        PageContent = _settingPage;
                        break;
                    }
                case "Charts":
                    {
                        PageContent=_chartsPage; 
                        break;
                    }
                case "More":
                    {
                        PageContent=_morePage; 
                        break;
                    }
            }
        }
        private void MinWindow()
        {
            WeakReferenceMessenger.Default.Send(new InfoMessage() { MessageTitle = MessageHead.MainVMToMin, MessageCommand = true });
        }
        private void CloseWindow()
        {
            ComPortHelp.isRun = false;
            WeakReferenceMessenger.Default.Send(new InfoMessage() { MessageTitle = MessageHead.MainVMToClose, MessageCommand = true });
        }
        private void ToggleTheme()
        {
            IsTheme = !IsTheme;
        }
        #endregion

        #region command
        public ICommand ThemeToggleCommand { get; set; }
        public ICommand WindowMinCommand { get; set; }
        public ICommand WindowCloseCommand { get; set; }
        public ICommand OpenCommand { get; set; }

        #endregion 
    }
}
