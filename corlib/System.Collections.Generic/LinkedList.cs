using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
#if !LOCALTEST
namespace System.Collections.Generic
{
    public struct ListEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private LinkedList<T> list;
        private LinkedListNode<T> node;
        private int version;
        private T current;
        private int index;
        internal ListEnumerator(LinkedList<T> list)
        {
            this.list = list;
            this.version = list.version;
            this.node = list.head;
            this.current = default(T);
            this.index = 0;
        }

        public T Current
        {
            get
            {
                return this.current;
            }
        }
        object IEnumerator.Current
        {
            get
            {
                if ((this.index == 0) || (this.index == (this.list.Count + 1)))
                {
                    throw new InvalidOperationException();
                    //  ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                }
                return this.current;
            }
        }
        public bool MoveNext()
        {
            if (this.version != this.list.version)
            {
                throw new InvalidOperationException();
                // throw new InvalidOperationException(SR.GetString(0x43, new object[0]));
            }
            if (this.node == null)
            {
                this.index = this.list.Count + 1;
                return false;
            }
            this.index++;
            this.current = this.node.item;
            this.node = this.node.next;
            if (this.node == this.list.head)
            {
                this.node = null;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            if (this.version != this.list.version)
            {
                throw new InvalidOperationException();
                //                throw new InvalidOperationException(SR.GetString(0x43, new object[0]));
            }
            this.current = default(T);
            this.node = this.list.head;
            this.index = 0;
        }

        public void Dispose()
        {
        }
    }

    public sealed class LinkedListNode<T>
    {
        internal T item;
        internal LinkedList<T> list;
        internal LinkedListNode<T> next;
        internal LinkedListNode<T> prev;

        public LinkedListNode(T value)
        {
            this.item = value;
        }

        internal LinkedListNode(LinkedList<T> list, T value)
        {
            this.list = list;
            this.item = value;
        }

        internal void Invalidate()
        {
            this.list = null;
            this.next = null;
            this.prev = null;
        }

        public LinkedList<T> List
        {
            get
            {
                return this.list;
            }
        }

        public LinkedListNode<T> Next
        {
            get
            {
                if ((this.next != null) && (this.next != this.list.head))
                {
                    return this.next;
                }
                return null;
            }
        }

        public LinkedListNode<T> Previous
        {
            get
            {
                if ((this.prev != null) && (this != this.list.head))
                {
                    return this.prev;
                }
                return null;
            }
        }

        public T Value
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }
    }




    public class LinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private object _syncRoot;
        internal int count;
        internal LinkedListNode<T> head;
        internal int version;

        public LinkedList()
        {
        }

        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (T local in collection)
            {
                this.AddLast(local);
            }
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            this.ValidateNode(node);
            this.ValidateNewNode(newNode);
            this.InternalInsertNodeBefore(node.next, newNode);
            newNode.list = (LinkedList<T>)this;
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            this.ValidateNode(node);
            LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
            this.InternalInsertNodeBefore(node.next, newNode);
            return newNode;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            this.ValidateNode(node);
            this.ValidateNewNode(newNode);
            this.InternalInsertNodeBefore(node, newNode);
            newNode.list = (LinkedList<T>)this;
            if (node == this.head)
            {
                this.head = newNode;
            }
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            this.ValidateNode(node);
            LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
            this.InternalInsertNodeBefore(node, newNode);
            if (node == this.head)
            {
                this.head = newNode;
            }
            return newNode;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            this.ValidateNewNode(node);
            if (this.head == null)
            {
                this.InternalInsertNodeToEmptyList(node);
            }
            else
            {
                this.InternalInsertNodeBefore(this.head, node);
                this.head = node;
            }
            node.list = (LinkedList<T>)this;
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
            if (this.head == null)
            {
                this.InternalInsertNodeToEmptyList(newNode);
                return newNode;
            }
            this.InternalInsertNodeBefore(this.head, newNode);
            this.head = newNode;
            return newNode;
        }

        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
            if (this.head == null)
            {
                this.InternalInsertNodeToEmptyList(newNode);
                return newNode;
            }
            this.InternalInsertNodeBefore(this.head, newNode);
            return newNode;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            this.ValidateNewNode(node);
            if (this.head == null)
            {
                this.InternalInsertNodeToEmptyList(node);
            }
            else
            {
                this.InternalInsertNodeBefore(this.head, node);
            }
            node.list = (LinkedList<T>)this;
        }

        public void Clear()
        {
            LinkedListNode<T> head = this.head;
            while (head != null)
            {
                LinkedListNode<T> node2 = head;
                head = head.Next;
                node2.Invalidate();
            }
            this.head = null;
            this.count = 0;
            this.version++;
        }

        public bool Contains(T value)
        {
            return (this.Find(value) != null);
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentException();
                //  throw new ArgumentNullException("array");
            }
            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentException();
                // throw new ArgumentOutOfRangeException("index", SR.GetString(0x8e, new object[] { index }));
            }
            if ((array.Length - index) < this.Count)
            {
                throw new ArgumentException();
                // throw new ArgumentException(SR.GetString(0x8f, new object[0]));
            }
            LinkedListNode<T> head = this.head;
            if (head != null)
            {
                do
                {
                    array[index++] = head.item;
                    head = head.next;
                }
                while (head != this.head);
            }
        }

        public LinkedListNode<T> Find(T value)
        {
            LinkedListNode<T> head = this.head;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            if (head != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (comparer.Equals(head.item, value))
                        {
                            return head;
                        }
                        head = head.next;
                    }
                    while (head != this.head);
                }
                else
                {
                    do
                    {
                        if (head.item == null)
                        {
                            return head;
                        }
                        head = head.next;
                    }
                    while (head != this.head);
                }
            }
            return null;
        }

        public LinkedListNode<T> FindLast(T value)
        {
            if (this.head != null)
            {
                LinkedListNode<T> prev = this.head.prev;
                LinkedListNode<T> node2 = prev;
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                if (node2 != null)
                {
                    if (value != null)
                    {
                        do
                        {
                            if (comparer.Equals(node2.item, value))
                            {
                                return node2;
                            }
                            node2 = node2.prev;
                        }
                        while (node2 != prev);
                    }
                    else
                    {
                        do
                        {
                            if (node2.item == null)
                            {
                                return node2;
                            }
                            node2 = node2.prev;
                        }
                        while (node2 != prev);
                    }
                }
            }
            return null;
        }

        public ListEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>((LinkedList<T>)this);
        }

        private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;
            this.version++;
            this.count++;
        }

        private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
        {
            newNode.next = newNode;
            newNode.prev = newNode;
            this.head = newNode;
            this.version++;
            this.count++;
        }

        internal void InternalRemoveNode(LinkedListNode<T> node)
        {
            if (node.next == node)
            {
                this.head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                if (this.head == node)
                {
                    this.head = node.next;
                }
            }
            node.Invalidate();
            this.count--;
            this.version++;
        }

        public void Remove(LinkedListNode<T> node)
        {
            this.ValidateNode(node);
            this.InternalRemoveNode(node);
        }

        public bool Remove(T value)
        {
            LinkedListNode<T> node = this.Find(value);
            if (node != null)
            {
                this.InternalRemoveNode(node);
                return true;
            }
            return false;
        }

        public void RemoveFirst()
        {
            if (this.head == null)
            {
                throw new InvalidOperationException();
            }
            this.InternalRemoveNode(this.head);
        }

        public void RemoveLast()
        {
            if (this.head == null)
            {
                throw new InvalidOperationException();
            }
            this.InternalRemoveNode(this.head.prev);
        }

        void ICollection<T>.Add(T value)
        {
            this.AddLast(value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
            //if (array == null)
            //{
            //    throw new ArgumentException();
            //    //throw new ArgumentNullException("array");
            //}
            //if (array.Rank != 1)
            //{
            //    throw new ArgumentException();
            //   // throw new ArgumentException(SR.GetString(0x8d, new object[0]));
            //}
            //if (array.GetLowerBound(0) != 0)
            //{
            //    throw new ArgumentException();
            //   // throw new ArgumentException();
            //}
            //if (index < 0)
            //{
            //    throw new ArgumentOutOfRangeException();//"index", SR.GetString(0x8e, new object[] { index }));
            //}
            //if ((array.Length - index) < this.Count)
            //{
            //    throw new ArgumentException();//SR.GetString(0x8f, new object[0]));
            //}
            //T[] localArray = array as T[];
            //if (localArray != null)
            //{
            //    this.CopyTo(localArray, index);
            //}
            //else
            //{
            //    Type elementType = array.GetType().GetElementType();
            //    Type c = typeof(T);
            //    if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
            //    {
            //        throw new ArgumentException();
            //    }
            //    object[] objArray = array as object[];
            //    if (objArray == null)
            //    {
            //        throw new ArgumentException();
            //    }
            //    LinkedListNode<T> head = this.head;
            //    try
            //    {
            //        if (head != null)
            //        {
            //            do
            //            {
            //                objArray[index++] = head.item;
            //                head = head.next;
            //            }
            //            while (head != this.head);
            //        }
            //    }
            //    catch (ArrayTypeMismatchException)
            //    {
            //        throw new ArgumentException();
            //    }
            //    catch (InvalidCastException)
            //    {
            //        throw new ArgumentException();
            //    }
            //}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal void ValidateNewNode(LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.list != null)
            {
                throw new InvalidOperationException();
            }
        }

        internal void ValidateNode(LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.list != this)
            {
                throw new InvalidOperationException();
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public LinkedListNode<T> First
        {
            get
            {
                return this.head;
            }
        }

        public LinkedListNode<T> Last
        {
            get
            {
                if (this.head != null)
                {
                    return this.head.prev;
                }
                return null;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotImplementedException();
                //if (this._syncRoot == null)
                //{
                //    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                //}
                //return this._syncRoot;
            }
        }

        //    [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private LinkedList<T> list;
            private LinkedListNode<T> node;
            private int version;
            private T current;
            private int index;
            internal Enumerator(LinkedList<T> list)
            {
                this.list = list;
                this.version = list.version;
                this.node = list.head;
                this.current = default(T);
                this.index = 0;
            }

            public T Current
            {
                get
                {
                    return this.current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this.list.Count + 1)))
                    {
                        throw new InvalidOperationException();
                        //                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.current;
                }
            }
            public bool MoveNext()
            {
                if (this.version != this.list.version)
                {
                    throw new InvalidOperationException();
                    //                throw new InvalidOperationException(SR.GetString(0x43, new object[0]));
                }
                if (this.node == null)
                {
                    this.index = this.list.Count + 1;
                    return false;
                }
                this.index++;
                this.current = this.node.item;
                this.node = this.node.next;
                if (this.node == this.list.head)
                {
                    this.node = null;
                }
                return true;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this.list.version)
                {
                    throw new InvalidOperationException();//SR.GetString(0x43, new object[0]));
                }
                this.current = default(T);
                this.node = this.list.head;
                this.index = 0;
            }

            public void Dispose()
            {
            }
        }
    }



}
#endif