using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using Currency.BusinessEntities;
using Currency.Helpers;

namespace Currency.Views
{
    public partial class CurrencyChart : Window
    {

        #region Private Fields

        private Dictionary<Abbreviation, List<Rate>> rates;
        private bool leftButtonPressed;
        private bool rightButtonPressed;
        private bool dateChangeInAction;
        private double delta;

        #endregion

        public CurrencyChart()
        {
            InitializeComponent();
            chart.ChartAreas.Add(new ChartArea("Default"));

            lbCurrency.SelectedItem = Abbreviation.USD;
            dpDateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpDateTo.SelectedDate = DateTime.Now;
            leftButtonPressed = false;
            rightButtonPressed = false;
            dateChangeInAction = false;
            delta = 0;
            rates = new Dictionary<Abbreviation, List<Rate>>();

            lbCurrency.SelectionChanged += lbCurrency_SelectionChanged;
            dpDateFrom.SelectedDateChanged += dpDate_SelectedDateChanged;
            dpDateTo.SelectedDateChanged += dpDate_SelectedDateChanged;

            rates[Abbreviation.USD] = RatesHelper.GetRatesForPeriod(Abbreviation.USD, DateTime.Now.AddYears(-1), DateTime.Now);
            RatesHelper.GetCurrenciesData().OrderBy(item => item.Abbreviation).ToList().ForEach(item => lbCurrency.Items.Add(AbbreviationHelper.ToAbbreviation(item.Abbreviation)));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chart.Dispose();
        }

        private void RefreshChart()
        {
            var data = rates.Select(list => list.Value.Where(rate => dpDateTo.SelectedDate.Value >= rate.Date && dpDateFrom.SelectedDate.Value <= rate.Date));
            var excessData = chart.Series.Where(item => !data.Select(it => it.First().Abbreviation.ToString()).Contains(item.Name)).ToList();
            if (excessData.Any())
            {
                excessData.ForEach(item => chart.Series.Remove(item));
            }
            foreach (var list in data)
            {
                var abb = list.First().Abbreviation.ToString();

                if (!chart.Series.Any(item => item.Name.Equals(abb)))
                {
                    chart.Series.Add(new Series(abb));
                    chart.Series[abb].ChartArea = "Default";
                    chart.Series[abb].ChartType = SeriesChartType.Line;
                }

                chart.Series[abb].ToolTip = abb + " #VAL{N2}";
                chart.Series[abb].MarkerStyle = MarkerStyle.Circle;
                chart.Series[abb].MarkerSize = 7;

                chart.Series[abb].Points.DataBindXY(list.Select(item => item.Date.ToShortDateString()).ToArray(), list.Select(item => item.OfficialRate).ToArray());
            }
            if (data.Any())
            {
                var max = data.Max(item => item.Max(it => it.OfficialRate));
                var min = data.Min(item => item.Min(it => it.OfficialRate));
                var delta = (max - min) * 0.1;
                chart.ChartAreas[0].AxisY.Minimum = min - delta;
                chart.ChartAreas[0].AxisY.Maximum = max + delta;
            }
        }

        private void lbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Abbreviation abb = e.RemovedItems.Count == 0 ? (Abbreviation)(int)e.AddedItems[0] : (Abbreviation)(int)e.RemovedItems[0];

            if (e.RemovedItems.Count != 0)
            {
                rates.Remove(abb);
            }

            if (e.AddedItems.Count != 0 && !rates.Any(item => item.Key == abb))
            {
                rates.Add(abb, RatesHelper.GetRatesForPeriod(abb, DateTime.Now.AddYears(-1), DateTime.Now));
            }
            RefreshChart();
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpDateTo.SelectedDate != null && dpDateTo.SelectedDate != null && dpDateFrom.SelectedDate < dpDateTo.SelectedDate)
            {
                RefreshChart();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        #region Button Handlers

        private async void btnLeft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!dateChangeInAction)
            {
                leftButtonPressed = true;
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                long count = 0;

                while (leftButtonPressed)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        dateChangeInAction = true;
                        Thread.Sleep(count++ > 3 ? 100 : 500);
                    }).
                    ContinueWith(_ =>
                    {
                        if (dpDateTo.SelectedDate != null && dpDateTo.SelectedDate != null && dpDateFrom.SelectedDate < dpDateTo.SelectedDate)
                        {
                            dpDateFrom.SelectedDate = dpDateFrom.SelectedDate.Value.AddDays(-1);
                            dpDateTo.SelectedDate = dpDateTo.SelectedDate.Value.AddDays(-1);
                            RefreshChart();
                        }
                    }, context);
                    dateChangeInAction = false;
                }
            }
        }

        private void btnLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftButtonPressed = false;
        }

        private async void btnRight_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!dateChangeInAction)
            {
                rightButtonPressed = true;
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                long count = 0;

                while (rightButtonPressed)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        dateChangeInAction = true;
                        Thread.Sleep(count++ > 3 ? 100 : 500);
                    }).
                    ContinueWith(_ =>
                    {
                        if (dpDateTo.SelectedDate != null && dpDateTo.SelectedDate != null && dpDateFrom.SelectedDate < dpDateTo.SelectedDate)
                        {
                            dpDateFrom.SelectedDate = dpDateFrom.SelectedDate.Value.AddDays(1);
                            dpDateTo.SelectedDate = dpDateTo.SelectedDate.Value.AddDays(1);
                            RefreshChart();
                        }
                    }, context);
                    dateChangeInAction = false;
                }
            }
        }

        private void btnRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rightButtonPressed = false;
        }

        #endregion

    }
}
