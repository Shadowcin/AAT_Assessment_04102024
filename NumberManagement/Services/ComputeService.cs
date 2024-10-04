using NumberManagement.Services.Interfaces;
using System.Collections.Concurrent;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;

namespace NumberManagement.Services
{
    public class ComputeService(INumberRepository numberRepository) : IComputeService
    {
        private readonly INumberRepository _numberRepository = numberRepository ?? throw new ArgumentNullException(nameof(numberRepository));

        private ConcurrentBag<NumberModel> _sharedList = new();
        private const int MaxEntries = 10_000_000;
        private const int StartEvenAfter = 2_500_000;
        private CancellationTokenSource cts = new();
        static object listLock = new object();
        static bool stopThreads = false;

        /// <inheritdoc />
        public async Task StartAsync()
        {
            //// Start the odd and prime number threads
            var oddNumberTask = Task.Run(() => AddOddNumbers(cts));
            var primeNumberTask = Task.Run(() => AddPrimeNumbers(cts));
            Task? evenNumberTask = null;

            while (_sharedList.Count < MaxEntries)
            {
                // Start even number generation after the list has 2,500,000 entries
                if (_sharedList.Count >= StartEvenAfter && evenNumberTask == null)
                {
                    evenNumberTask = Task.Run(() => AddEvenNumbers(cts));
                }

                // Stop all threads once the max entries are reached
                if (_sharedList.Count >= MaxEntries)
                {
                    await cts.CancelAsync();
                    break;
                }

                // Small delay to prevent high CPU usage
                await Task.Delay(100);
            }

            // Wait for all threads to complete
            await Task.WhenAll(oddNumberTask, primeNumberTask, evenNumberTask ?? Task.CompletedTask);
        }

        private void AddEvenNumbers(CancellationTokenSource token)
        {
            var evenNumber = 0;
            while (!stopThreads && !token.IsCancellationRequested)
            {
                evenNumber += 2;
                lock (listLock)
                {
                    if (_sharedList.Count < MaxEntries)
                    {
                        _sharedList.Add(new NumberModel
                        {
                            Value = evenNumber
                        });
                    }
                    if (_sharedList.Count >= MaxEntries)
                    {
                        stopThreads = true;
                    }
                }
            }
        }

        private void AddOddNumbers(CancellationTokenSource token)
        {
            var random = new Random();
            while (!stopThreads && !token.IsCancellationRequested)
            {
                var oddNumber = random.Next(1, int.MaxValue);
                if (oddNumber % 2 == 0) continue;
                lock (listLock)
                {
                    if (_sharedList.Count < MaxEntries)
                    {
                        _sharedList.Add(new NumberModel
                        {
                            Value = oddNumber
                        });
                    }
                    if (_sharedList.Count >= MaxEntries)
                    {
                        stopThreads = true;
                    }
                }
            }
        }

        private void AddPrimeNumbers(CancellationTokenSource token)
        {
            var num = 2;
            while (!stopThreads && !token.IsCancellationRequested)
            {
                if (IsPrime(num))
                {
                    lock (listLock)
                    {
                        if (_sharedList.Count < MaxEntries)
                        {
                            _sharedList.Add(new NumberModel
                            {
                                Value = -num,
                                IsPrime = 1
                            });
                        }
                        if (_sharedList.Count >= MaxEntries)
                        {
                            stopThreads = true;
                        }
                    }
                }
                num++;
            }
        }

        private bool IsPrime(int num)
        {
            if (num <= 1) return false;
            if (num == 2) return true;
            if (num % 2 == 0) return false;

            for (var i = 3; i <= Math.Sqrt(num); i += 2)
            {
                if (num % i == 0) return false;
            }
            return true;
        }

        /// <inheritdoc />
        public TotalsModel DisplayStatistics()
        {
            lock (listLock)
            {
                //sort the list asc
                _sharedList.Order();

                var result = new TotalsModel
                {
                    Total = _sharedList.Count,
                    OddNumbers = _sharedList.Count(n => n.Value % 2 != 0),
                    EvenNumbers = _sharedList.Count(n => n.Value % 2 == 0)
                };

                return result;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveComputeData()
        {
            return await _numberRepository.BulkNumberInsert(_sharedList);
        }
    }
}
