using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Currency.ApplicationData;
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
            SharedData.ProcessesToKill.Processes.Where(proc => proc.AutoStart).ToList()
                .ForEach(proc => ToggleProcessKilling(proc.Name, proc.Period));
        }

        #endregion

        #region Properties

        Dictionary<string, CancellationTokenSource> _processKillers;

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

        public ProcessesDictionary Processes
        {
            get
            {
                return SharedData.ProcessesToKill;
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
                        ToggleProcessKilling(param.ToString(), Processes.Processes.First(pr => pr.Name == param.ToString()).Period);
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

        private void ToggleProcessKilling(string processName, int period)
        {
            if (_processKillers.Any(item => item.Key == processName))
            {
                var cts = _processKillers.First(item => item.Key == processName);
                cts.Value.Cancel();
                _processKillers.Remove(processName);
            }
            else
            {
                var cts = new CancellationTokenSource();
                KillProcessAsync(cts.Token, processName, period);
                _processKillers.Add(processName, cts);
            }
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
