using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Kulman.WPA81.Interfaces;
using Kulman.WPA81.Services;
using QRReader.ViewModels;
using QRReader.Views;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace QRReader
{
    public sealed partial class App
    {
        private WinRTContainer container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

            container = new WinRTContainer();

            container.RegisterWinRTServices();

            container.PerRequest<MainViewModel>();
            container.RegisterSingleton(typeof(IDialogService),null,typeof(DialogService));

            PrepareViewFirst();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Initialize();

            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            if (RootFrame.Content == null)
                DisplayRootView<MainView>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}