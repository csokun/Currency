using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Currency.BusinessEntities;
using Currency.Helpers;
using Currency.Views;

namespace Currency.ViewModels
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {

        #region Methods

        private void RefreshRates()
        {
            var rates = RatesHelper.GetLastDailyRates();
            var prevRates = RatesHelper.GetPrevDailyRates();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("Rates for {0}", rates.First().Date.AddDays(DateTime.Now.DayOfWeek == DayOfWeek.Friday ? -3 : -1).ToShortDateString()));

            rates = rates.
                Where(item => (item.Abbreviation & (Abbreviation.USD | Abbreviation.EUR | Abbreviation.RUB)) != Abbreviation.NONE).ToList();
            prevRates = prevRates.
                Where(item => (item.Abbreviation & (Abbreviation.USD | Abbreviation.EUR | Abbreviation.RUB)) != Abbreviation.NONE).ToList();

            foreach (var rate in rates)
            {
                double prev = prevRates.First(item => item.Abbreviation == rate.Abbreviation).OfficialRate;
                sb.AppendLine(String.Format("{0} {1} -> {2} | {3}", rate.Abbreviation.ToString(), prev, rate.OfficialRate, rate.OfficialRate - prev > 0 ? "+" + (rate.OfficialRate - prev) : (rate.OfficialRate - prev).ToString()));
            }

            Rates = sb.ToString();
        }

        private void ShowDialog(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        #endregion

        #region Public Properties

        private string _rates;

        public string Rates
        {
            get
            {
                return _rates ?? String.Empty;
            }
            set
            {
                _rates = value;
                OnPropertyChanged("Rates");
            }
        }

        #endregion

        #region Commands

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        public ICommand ViewChartCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => ShowDialog(new CurrencyChart()) };
            }
        }

        public ICommand NotifyIconClickCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => RefreshRates() };
            }
        }

        public ICommand CopyToClipBoardCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Clipboard.SetText(Rates) };
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }


}
