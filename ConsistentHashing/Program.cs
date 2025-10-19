using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Initialize the consistent hash with 100 replicas for each node
        var consistentHash = new ConsistentHash<string>(100);

        // Add some server nodes
        consistentHash.AddNode("ServerA");
        consistentHash.AddNode("ServerB");
        consistentHash.AddNode("ServerC");

        Console.WriteLine("Initial key distribution:");
        DistributeKeys(consistentHash, new List<string> { "user1", "user2", "user3", "user4", "user5" });

        // Demonstrate removing a node
        Console.WriteLine("\nRemoving ServerB...");
        consistentHash.RemoveNode("ServerB");
        Console.WriteLine("Key distribution after removing ServerB:");
        DistributeKeys(consistentHash, new List<string> { "user1", "user2", "user3", "user4", "user5" });

        // Demonstrate adding a new node
        Console.WriteLine("\nAdding ServerD...");
        consistentHash.AddNode("ServerD");
        Console.WriteLine("Key distribution after adding ServerD:");
        DistributeKeys(consistentHash, new List<string> { "user1", "user2", "user3", "user4", "user5" });
    }

    static void DistributeKeys(ConsistentHash<string> consistentHash, List<string> keys)
    {
        var distribution = new Dictionary<string, List<string>>();
        foreach (var key in keys)
        {
            var node = consistentHash.GetNode(key);
            if (!distribution.ContainsKey(node))
            {
                distribution[node] = new List<string>();
            }
            distribution[node].Add(key);
        }

        foreach (var entry in distribution)
        {
            Console.WriteLine($"  Node {entry.Key}: {string.Join(", ", entry.Value)}");
        }
    }
}
