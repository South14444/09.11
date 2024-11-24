using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NumberGenerator
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSourcePrimes;
        private CancellationTokenSource _cancellationTokenSourceFibonacci;
        private volatile bool _isFibonacciPaused;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int lowerBound = string.IsNullOrEmpty(LowerBoundTextBox.Text) ? 2 : int.Parse(LowerBoundTextBox.Text);
            int upperBound = string.IsNullOrEmpty(UpperBoundTextBox.Text) ? int.MaxValue : int.Parse(UpperBoundTextBox.Text);
            _cancellationTokenSourcePrimes = new CancellationTokenSource();
            _cancellationTokenSourceFibonacci = new CancellationTokenSource();
            _isFibonacciPaused = false;
            StopPrimesButton.IsEnabled = true;
            StopFibonacciButton.IsEnabled = true;
            PauseFibonacciButton.IsEnabled = true;
            ResumeFibonacciButton.IsEnabled = true;
            RestartButton.IsEnabled = true;
            Task.Run(() => GeneratePrimes(lowerBound, upperBound, _cancellationTokenSourcePrimes.Token));
            Task.Run(() => GenerateFibonacci(_cancellationTokenSourceFibonacci.Token));
        }
        private async Task GeneratePrimes(int lowerBound, int upperBound, CancellationToken cancellationToken)
        {
            for (int num = lowerBound; num <= upperBound; num++)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (IsPrime(num))
                {
                    AppendOutput(num.ToString());
                }
                await Task.Delay(100);
            }
        }
        private async Task GenerateFibonacci(CancellationToken cancellationToken)
        {
            int a = 0, b = 1;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_isFibonacciPaused)
                {
                    await Task.Delay(100);
                    continue;
                }
                AppendOutput(a.ToString());
                int next = a + b;
                a = b;
                b = next;

                await Task.Delay(100);
            }
        }
        private void StopPrimesButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSourcePrimes.Cancel();
            StopPrimesButton.IsEnabled = false;
        }
        private void StopFibonacciButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSourceFibonacci.Cancel();
            StopFibonacciButton.IsEnabled = false;
        }
        private void PauseFibonacciButton_Click(object sender, RoutedEventArgs e)
        {
            _isFibonacciPaused = true;
            PauseFibonacciButton.IsEnabled = false;
            ResumeFibonacciButton.IsEnabled = true;
        }
        private void ResumeFibonacciButton_Click(object sender, RoutedEventArgs e)
        {
            _isFibonacciPaused = false;
            ResumeFibonacciButton.IsEnabled = false;
            PauseFibonacciButton.IsEnabled = true;
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StopPrimesButton_Click(sender, e);
            StopFibonacciButton_Click(sender, e);
            StartButton_Click(sender, e);
        }
        private void AppendOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText(text + Environment.NewLine);
                OutputTextBox.ScrollToEnd();
            });
        }
        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}