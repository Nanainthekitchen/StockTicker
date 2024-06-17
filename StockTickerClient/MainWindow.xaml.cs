using StockTickerService;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;

namespace StockTickerClient
{
    public partial class MainWindow : Window
    {
        private StockPriceProvider _stockPriceProvider;
        private ObservableCollection<StockPriceViewModel> _stockPrices;

        public MainWindow()
        {
            InitializeComponent();
            _stockPrices = new ObservableCollection<StockPriceViewModel>();
            ListViewStock.ItemsSource = _stockPrices;

            //connecting to service
            _stockPriceProvider = new StockPriceProvider();
            _stockPriceProvider.StockPriceChanged += StockPriceProvider_StockPriceChanged;
            SubscribeToStocks();
        }

        private void SubscribeToStocks()
        {
            _stockPriceProvider.Subscribe("Stock1");
            _stockPriceProvider.Subscribe("Stock2");
        }
        private void UnSubscribeToStocks()
        {
            _stockPriceProvider.Unsubscribe("Stock1");
            _stockPriceProvider.Unsubscribe("Stock2");
        }

        private void StockPriceProvider_StockPriceChanged(object sender, StockPriceChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var stock = _stockPrices.FirstOrDefault(sp => sp.Ticker == e.Ticker);
                if (stock == null)
                {
                    stock = new StockPriceViewModel { Ticker = e.Ticker, Price = e.Price };
                    _stockPrices.Add(stock);
                }
                else
                {
                    stock.UpdatePrice(e.Price);
                }
            });
        }

        private void StockListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListViewStock.SelectedItem.GetType().Name.Equals("StockPriceViewModel"))
            {
                var priceHistory = (ListViewStock.SelectedItem as StockPriceViewModel).PriceHistory;
                var historyWindow = new StockPriceHistory(priceHistory);
                historyWindow.Show();
            }
        }
    }

    public class StockPriceViewModel : INotifyPropertyChanged
    {
        public string Ticker { get; set; }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get => _background;
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public ObservableCollection<PriceHistory> PriceHistory { get; }

        public StockPriceViewModel()
        {
            PriceHistory = new ObservableCollection<PriceHistory>();
        }

        public void UpdatePrice(decimal newPrice)
        {
            PriceHistory.Add(new PriceHistory() { date = DateTime.Now.ToString("dd MM yyyy H:mm:ss"), price = newPrice });
            Background = newPrice > Price ? Brushes.LightGreen : newPrice < Price ? Brushes.Red : Brushes.White;
            Price = newPrice;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class PriceHistory
    {
        public string date { get; set; }
        public decimal price { get; set; }
    }
}
