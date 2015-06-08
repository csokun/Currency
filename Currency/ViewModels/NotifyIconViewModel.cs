using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Currency.BusinessEntities;
using Currency.Helpers;
using Currency.Views;

namespace Currency.ViewModels
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {

        #region ctor

        public NotifyIconViewModel()
        {
            _processKillers = new Dictionary<string, CancellationTokenSource>();
            _tasks = new Dictionary<string, Task>();
        }

        #endregion

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

        private async Task KillProcessAsync(CancellationToken cancellToken, string processName, int period)
        {
            while (!cancellToken.IsCancellationRequested)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Any())
                {
                    var proc = processes.First();
                    proc.Kill();
                }
                await Task.Delay(period * 1000);
            }
        }

        private void ShowDialog(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        #endregion

        #region Properties

        Dictionary<string, CancellationTokenSource> _processKillers;
        Dictionary<string, Task> _tasks;

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

        public string[] Processes
        {
            get
            {
                return new string[] { "dotnetfx64.exe" };
            }
        }

        #endregion

        #region Commands

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = (param) => Application.Current.Shutdown() };
            }
        }

        public ICommand ToggleProcessKillerCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = (param) =>
                    {
                        if (_processKillers.Any(item => item.Key == param.ToString()))
                        {
                            var cts = _processKillers.First(item => item.Key == param.ToString());
                            cts.Value.Cancel();

                            _processKillers.Remove(param.ToString());
                            _tasks.Remove(param.ToString());
                        }
                        else
                        {
                            var cts = new CancellationTokenSource();
                            _tasks.Add(param.ToString(), Task.Run(() => KillProcessAsync(cts.Token, param.ToString(), 3)));

                            _processKillers.Add(param.ToString(), cts);
                        }
                    }
                };
            }
        }

        public ICommand ViewChartCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = (param) => ShowDialog(new CurrencyChart()) };
            }
        }

        public ICommand NotifyIconClickCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = (param) => RefreshRates() };
            }
        }

        public ICommand CopyToClipBoardCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = (param) => Clipboard.SetText(Rates) };
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
