using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EnumerableHashMap.HashMap
{
    public class MyHashMap<K, V> : IEnumerable<MyHashMap<K, V>.Entity>
    {
        private const int InitBucketCount = 16;
        private const double LoadFactor = 0.5;
        private Bucket[] _buckets;
        private int _size = 0;

        public MyHashMap()
        {
            _buckets = new Bucket[InitBucketCount];
        }

        public MyHashMap(int initCount)
        {
            _buckets = new Bucket[initCount];
        }

        public V Put(K key, V Value)
        {
            if (_buckets.Length * LoadFactor <= _size)
            {
                Recalculate();
            }

            int index = CalculateBucketIndex(key);
            Bucket bucket = _buckets[index];
            if (bucket == null)
            {
                bucket = new Bucket();
                _buckets[index] = bucket;
            }

            Entity entity = new Entity();
            entity.Key = key;
            entity.Value = Value;

            V res = (V)bucket.Add(entity);
            if (res == null)
            {
                _size++;
            }
            return res;
        }

        public V Remove(K key)
        {
            int index = CalculateBucketIndex(key);
            Bucket bucket = _buckets[index];
            if (bucket == null)
                return default;
            V res = bucket.Remove(key);
            if (res != null)
            {
                _size--;
            }
            return res;
        }

        public V Get(K key)
        {
            int index = CalculateBucketIndex(key);
            Bucket bucket = _buckets[index];
            if (bucket == null)
                return default;
            return bucket.Get(key);
        }

        private int CalculateBucketIndex(K key)
        {
            return Math.Abs(key.GetHashCode()) % _buckets.Length;
        }

        private void Recalculate()
        {
            _size = 0;
            Bucket[] old = _buckets;
            _buckets = new Bucket[old.Length * 2];
            for (int i = 0; i < old.Length; i++)
            {
                Bucket bucket = old[i];
                if (bucket != null)
                {
                    Bucket.Node node = bucket.Head;
                    while (node != null)
                    {
                        Put(node.Value.Key, node.Value.Value);
                        node = node.Next;
                    }
                }
            }
        }

        public IEnumerator<MyHashMap<K, V>.Entity> GetEnumerator() => new EnumeratorImplementation(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class Entity
        {
            public K Key;
            public V Value;
        }
        private class Bucket
        {
            internal Node Head;

            public V Add(MyHashMap<K, V>.Entity entity)
            {
                Node node = new Node();
                node.Value = entity;

                if (Head == null)
                {
                    Head = node;
                    return default;
                }

                Node currentNode = Head;
                while (true)
                {
                    if (currentNode.Value.Key.Equals(entity.Key))
                    {
                        V buf = currentNode.Value.Value;
                        currentNode.Value.Value = entity.Value;
                        return buf;
                    }
                    if (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                    }
                    else
                    {
                        currentNode.Next = node;
                        return default;
                    }
                }

            }

            public V Remove(K Key)
            {
                if (Head == null)
                    return default;
                if (Head.Value.Key.Equals(Key))
                {
                    V buf = (V)Head.Value.Value;
                    Head = Head.Next;
                    return buf;
                }
                else
                {
                    Node node = Head;
                    while (node.Next != null)
                    {
                        if (node.Next.Value.Key.Equals(Key))
                        {
                            V buf = node.Next.Value.Value;
                            node.Next = node.Next.Next;
                            return buf;
                        }
                        node = node.Next;
                    }
                    return default;
                }
            }

            public V Get(K Key)
            {
                Node node = Head;

                while (node != null)
                {
                    if (node.Value.Key.Equals(Key))
                        return node.Value.Value;
                    node = node.Next;
                }
                return default;
            }

            public class Node
            {
                public Node Next;
                public Entity Value;
            }
        }

        public class EnumeratorImplementation : IEnumerator<Entity>
        {
            private readonly MyHashMap<K, V> _self;
            private int _currIndex = -1;
            private IEnumerator _bucketEnumerator;
            private Bucket.Node _currentNode;
            public EnumeratorImplementation(MyHashMap<K, V> self)
            {
                _self = self;
                _bucketEnumerator = _self._buckets.GetEnumerator();
            }

            bool isIndexValid => _currIndex >= 0 && _currIndex < _self._buckets.Length;

            public MyHashMap<K, V>.Entity Current => _currentNode.Value;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_currentNode?.Next is not null)
                {
                    _currentNode = _currentNode.Next;
                }
                else
                {
                    bool canMoveNext;
                    do
                    {
                        _currIndex++;
                        canMoveNext = _bucketEnumerator.MoveNext();
                        if (!canMoveNext)
                            break;
                        _currentNode = (_bucketEnumerator.Current as Bucket)?.Head;
                    } while ((_currentNode is null || _currentNode.Value is null));
                }
                return isIndexValid;
            }

            public void Reset()
            {
                _currIndex = -1;
                _bucketEnumerator.Reset();
            }
            public void Dispose() { }
        }
    }
}
