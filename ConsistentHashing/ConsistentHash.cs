using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class ConsistentHash<T> where T : notnull
{
    private readonly SortedDictionary<int, T> _ring = new SortedDictionary<int, T>();
    private readonly HashSet<T> _nodes = new HashSet<T>();
    private readonly int _replicas;

    public int NodeCount => _nodes.Count;

    public ConsistentHash(int replicas)
    {
        _replicas = replicas;
    }

    public void AddNode(T node)
    {
        _nodes.Add(node);
        for (int i = 0; i < _replicas; i++)
        {
            int hash = GetHash(node.ToString() + i);
            _ring[hash] = node;
        }
    }

    public void RemoveNode(T node)
    {
        _nodes.Remove(node);
        for (int i = 0; i < _replicas; i++)
        {
            int hash = GetHash(node.ToString() + i);
            if (_ring.ContainsKey(hash))
            {
                _ring.Remove(hash);
            }
        }
    }

    public T GetNode(string key)
    {
        if (_ring.Count == 0)
        {
            throw new InvalidOperationException("No nodes have been added to the consistent hash ring.");
        }

        int hash = GetHash(key);

        // Find the first key greater than or equal to the hash
        foreach (var ringKey in _ring.Keys)
        {
            if (ringKey >= hash)
            {
                return _ring[ringKey];
            }
        }

        // If we've gone through all keys and none are >= hash, wrap around to the first one.
        return _ring.First().Value;
    }

    private int GetHash(string key)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] data = Encoding.UTF8.GetBytes(key);
            byte[] hash = sha256.ComputeHash(data);
            return BitConverter.ToInt32(hash, 0);
        }
    }
}
