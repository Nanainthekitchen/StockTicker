using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StockTickerService
{
    public class StockPriceProvider
    {
        private readonly Dictionary<string, decimal> _prices = new Dictionary<string, decimal>
        {
            { "Stock1", 255 },
            { "Stock2", 195 }
        };

        public event EventHandler<StockPriceChangedEventArgs> StockPriceChanged;

        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new Dictionary<string, CancellationTokenSource>();

        public void Subscribe(string ticker)
        {
            if (!_cancellationTokens.ContainsKey(ticker))
            {
                //assign new token to the ticker
                var cts = new CancellationTokenSource();
                _cancellationTokens[ticker] = cts;
                Task.Run(() => PublishPrices(ticker, cts.Token), cts.Token);
            }
        }
        
        public void Unsubscribe(string ticker)
        {
            if (_cancellationTokens.TryGetValue(ticker, out var cts))
            {
                cts.Cancel();//stop task
                _cancellationTokens.Remove(ticker);
            }
        }

        private async Task PublishPrices(string ticker, CancellationToken token)
        {
            var random = new Random();
            while (!token.IsCancellationRequested)
            {
                //price changes every second
                await Task.Delay(1000, token);

                var oldPrice = _prices[ticker];
                var newPrice = oldPrice;

                if (ticker == "Stock1")
                {
                    newPrice = random.Next(240, 271);
                }
                else if (ticker == "Stock2")
                {
                    newPrice = random.Next(180, 211);
                }

                _prices[ticker] = newPrice;
                StockPriceChanged?.Invoke(this, new StockPriceChangedEventArgs(ticker, newPrice));
            }
        }
    }
    public class StockPriceChangedEventArgs : EventArgs
    {
        public string Ticker { get; }
        public decimal Price { get; }

        public StockPriceChangedEventArgs(string ticker, decimal price)
        {
            Ticker = ticker;
            Price = price;
        }
    }
}
