using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;

namespace Ignite.DynamicLINQ.Data;

public class IgniteService
{
    private static IgniteConfiguration GetIgniteConfig()
    {
        return new IgniteConfiguration
        {
            DiscoverySpi = new TcpDiscoverySpi
            {
                IpFinder = new TcpDiscoveryStaticIpFinder
                {
                    Endpoints = new[] { "127.0.0.1:47500" }
                },
                SocketTimeout = TimeSpan.FromSeconds(0.5),
            },
            Localhost = "127.0.0.1"
        };
    }

    public IgniteService()
    {
        Ignite = Start();

        var cacheConfiguration = new CacheConfiguration("cars", new QueryEntity(typeof(int), typeof(Car)));
        Cars = Ignite.GetOrCreateCache<int, Car>(cacheConfiguration);
    }

    public IIgnite Ignite { get; }
    
    public ICache<int, Car> Cars { get; }

    private static IIgnite Start()
    {
        return Ignition.Start(GetIgniteConfig());
    }
}
