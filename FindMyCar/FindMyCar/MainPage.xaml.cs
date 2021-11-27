using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FindMyCar.Resources;
using System.Windows.Threading;

namespace FindMyCar
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ParkingViewModel viewModel = null;
        private DispatcherTimer refreshTimer = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = new TimeSpan(0, 0, 10);
            refreshTimer.Tick += refreshTimer_Tick;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            viewModel.RefreshCurrentPos();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (viewModel == null)
            {
                viewModel = new ParkingViewModel();
                DataContext = viewModel;
            }

            viewModel.RefreshCurrentPos();
            

            refreshTimer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            refreshTimer.Stop();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            viewModel.FindRoute();
        }

        private void btnPark_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ParkCar();
        }
    }
}