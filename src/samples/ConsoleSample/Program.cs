using System;
using Jab;

namespace ConsoleSample
{ 
    //
    // BACKEND
    //

    public class Network
    {
    }

    public class WalletManager
    {
        public Network Network { get; }

        public WalletManager(Network network)
        {
            Network = network;
            Console.WriteLine($"WalletManager Network {network is not null}");
        }
    }

    public class KeyManager
    {
    }

    [ServiceProviderModule]
    [Singleton(typeof(Network))]
    [Singleton(typeof(WalletManager))]
    [Singleton(typeof(KeyManager))]
    public interface IBackendModule
    {
    }
    
    //
    // FRONTEND
    //

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
            Console.WriteLine($"AddWalletViewModel WalletManager {_walletManager is not null}");

            _keyManager = (KeyManager)serviceProvider.GetService(typeof(KeyManager));
            Console.WriteLine($"AddWalletViewModel KeyManager {_keyManager is not null}");

            Console.WriteLine("AddWalletViewModel()");
        }
    }

    [ServiceProviderModule]
    [Transient(typeof(OptimisePrivacyViewModel))]
    [Transient(typeof(PrivacyControlViewModel))]
    [Transient(typeof(PrivacySuggestionControlViewModel))]
    [Transient(typeof(SendViewModel))]
    [Transient(typeof(TransactionPreviewViewModel))]
    [Scoped(typeof(ReceiveViewModel))]
    [Singleton(typeof(AddWalletViewModel))]
    public interface IFrontendModule
    {
    }

    //
    // COMMON
    //
    
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

    //
    // ServiceProvider
    //
    
    [ServiceProvider]
    [Singleton(typeof(IServiceProvider), typeof(ServiceProvider))]
    [Singleton(typeof(Program))]
    [Singleton(typeof(Logger))]
    [Import(typeof(IFrontendModule))]
    [Import(typeof(IBackendModule))]
    partial class ServiceProvider
    {
    }

    //
    // Program
    //
    
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

            var network = service.GetService<Network>();
            Console.WriteLine($"Main Network {network is not null}");
            
            var walletManager = service.GetService<WalletManager>();
            Console.WriteLine($"Main WalletManager {walletManager is not null}");
            Console.WriteLine($"Main WalletManager.Network {walletManager?.Network is not null}");

            var keyManager = service.GetService<KeyManager>();
            Console.WriteLine($"Main KeyManager {keyManager is not null}");

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
            _logger.Log("[Log] Starting");
            _logger.LogError("[Log] Error happened");
        }
    }
}
