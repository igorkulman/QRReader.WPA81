using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Kulman.WPA81.Interfaces;
using QRReader.Messages;

namespace QRReader.ViewModels
{
    public class MainViewModel : Screen, IHandle<QRDecodedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

        public MainViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventAggregator.Unsubscribe(this);
        }

        public void Handle(QRDecodedMessage message)
        {
            _dialogService.ShowMessageDialog(message.Result, "QRReader");
        }
    }
}
