using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NumberManagement.Services.Interfaces;

namespace NumberManagement.Components.Pages
{
    public partial class Compute
    {
        [Inject] public IComputeService NumberService { get; set; }
        [Inject] public IDataDownloadService DataDownloadService { get; set; }
        [Inject] public IJSRuntime JS { get; set; }

        public bool ShowLoading { get; set; } = false;
        public int Total { get; set; }
        public int TotalOdd { get; set; }
        public int TotalEven { get; set; }
        public bool IsDataSaved { get; set; } = false;

        protected bool DisableCompute = false;
        protected bool DisableSaveData = true;

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("Component initialized.");
            ShowLoading = false;
            StateHasChanged();
        }

        private async Task StartProcess()
        {
            try
            {
                DisableCompute = true;
                ShowLoading = true;
                await NumberService.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                var results = NumberService.DisplayStatistics();
                if (results != null)
                {
                    Total = results.Total;
                    TotalOdd = results.OddNumbers;
                    TotalEven = results.EvenNumbers;
                }
                ShowLoading = false;
                DisableSaveData = false;
            }
        }

        public async void DownloadBinary()
        {
            try
            {
                ShowLoading = true;
                StateHasChanged();
                await Task.Delay(100);

                var isFileSaved = await DataDownloadService.DownloadBinary();
                if (isFileSaved)
                {
                    var fileName = "computeDataBinary.bin";
                    var fileURL = "/files/computeDataBinary.bin";
                    await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving data: {ex.Message}");
            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }
        }

        public async void DownloadXml()
        {
            try
            {
                ShowLoading = true;
                StateHasChanged();
                await Task.Delay(100);

                var isFileSaved = await DataDownloadService.DownloadXml();
                if (isFileSaved)
                {
                    var fileName = "computeDataXml.xml";
                    var fileURL = "/files/computeDataXml.xml";
                    await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving data: {ex.Message}");
            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }
        }

        private async Task SaveData()
        {
            try
            {
                DisableSaveData = true;
                ShowLoading = true;
                StateHasChanged();
                await Task.Delay(100);
                var result = await NumberService.SaveComputeData(); 

                if (result)
                {
                    IsDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving data: {ex.Message}");
            }
            finally
            {
                ShowLoading = false;
                StateHasChanged(); 
            }
        }
    }
}
