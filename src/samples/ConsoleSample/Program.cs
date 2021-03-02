﻿using System;
using Jab;

namespace ConsoleSample
{ 
    public class Network
    {
    }

    public class WalletManager
    {
        public WalletManager(Network network)
        {
        }
    }

    public class KeyManager
    {
    }

    public abstract partial class BaseViewModel
    {
    }

    public partial class OptimisePrivacyViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public OptimisePrivacyViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
    
    public partial class PrivacyControlViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public PrivacyControlViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
    
    public partial class PrivacySuggestionControlViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public PrivacySuggestionControlViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }

    public partial class SendViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public SendViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Console.WriteLine("SendViewModel()");
        }
    }

    public partial class TransactionPreviewViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public TransactionPreviewViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }

    [ServiceProviderModule]
    [Transient(typeof(OptimisePrivacyViewModel))]
    [Transient(typeof(PrivacyControlViewModel))]
    [Transient(typeof(PrivacySuggestionControlViewModel))]
    [Transient(typeof(SendViewModel))]
    [Transient(typeof(TransactionPreviewViewModel))]
    public interface IWalletSendModule
    {
    }

    public partial class ReceiveViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        
        public ReceiveViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Console.WriteLine("ReceiveViewModel()");
        }
    }

    public partial class AddWalletViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;
        private WalletManager _walletManager;
        private KeyManager _keyManager;
        
        public AddWalletViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _walletManager = (WalletManager)serviceProvider.GetService(typeof(WalletManager));
            _keyManager = (KeyManager)serviceProvider.GetService(typeof(KeyManager));
            Console.WriteLine("AddWalletViewModel()");
        }
    }

    [ServiceProvider]
    [Singleton(typeof(IServiceProvider), typeof(ServiceProvider))]
    [Singleton(typeof(Program))]
    [Singleton(typeof(Logger))]
    [Scoped(typeof(ReceiveViewModel))]
    [Singleton(typeof(AddWalletViewModel))]
    [Import(typeof(IWalletSendModule))]
    partial class ServiceProvider
    {
    }

    public class Logger
    {
        public void Log(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.Error.WriteLine(message);
        }
    }

    class Program
    {
        private readonly Logger _logger;

        public Program(Logger logger)
        {
            _logger = logger;
        }

        static void Main(string[] args)
        {
            var service = new ServiceProvider();

            IServiceProvider serviceProvider = service;

            using (var scope1 = service.CreateScope())
            {
                var sendViewModel = scope1.GetService<SendViewModel>();
                var receiveViewModel = scope1.GetService<ReceiveViewModel>();
                var addWalletViewModel = scope1.GetService<AddWalletViewModel>();
            }

            using (var scope2 = service.CreateScope())
            {
                var sendViewModel = scope2.GetService<SendViewModel>();
                var receiveViewModel = scope2.GetService<ReceiveViewModel>();
                var addWalletViewModel = scope2.GetService<AddWalletViewModel>();
            }

            var rootScope = service.CreateScope();
            var logger1 = rootScope.GetService<Logger>();
            var logger2 = service.GetService<Logger>();
            var logger3 = serviceProvider.GetService(typeof(Logger));
            
            Console.WriteLine(logger1 == logger2);
            Console.WriteLine(logger2 == logger3);

            var receiveViewModel1 = service.GetService<ReceiveViewModel>();
            var receiveViewModel2 = service.GetService<ReceiveViewModel>();
            Console.WriteLine(receiveViewModel1 == receiveViewModel2);
            var receiveViewModel3 = rootScope.GetService<ReceiveViewModel>();
            
            var addWalletViewModel1 = service.GetService<AddWalletViewModel>();

            service.GetService<Program>().Run(args);
        }

        public void Run(string[] args)
        {
            _logger.Log("Starting");
            _logger.LogError("Error happened");
        }
    }
}
