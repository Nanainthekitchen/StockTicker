using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System;

namespace StockTickerClient
{
    public partial class StockPriceHistory : Window
    {
        public StockPriceHistory(ObservableCollection<PriceHistory> priceHistory)
        {
            InitializeComponent();
            PriceHistoryListView.ItemsSource = priceHistory;
        }
    }
}
