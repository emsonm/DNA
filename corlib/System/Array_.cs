namespace System
{
    using System.Collections;
    //using System.PInvoke;

    public abstract class Array_ //: ICloneable, IList, ICollection, IEnumerable
    {
        private int cElems;


        //public static int BinarySearch(Array_ array, object value)
        //{
        //    return BinarySearch(array, 0, array.cElems, value, null);
        //}

        //public static int BinarySearch(Array_ array, int index, int length, object value, IComparer comparer)
        //{
        //    if (array == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if ((index < 0) || (length < 0))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if ((array.cElems - index) < length)
        //    {
        //        throw new ArgumentException();
        //    }
        //    if (array.Rank != 1)
        //    {
        //        throw new RankException();
        //    }
        //    if (comparer == null)
        //    {
        //        comparer = Comparer.Default;
        //    }
        //    int num = index;
        //    int num2 = (index + length) - 1;
        //    object[] objArray = array as object[];
        //    while (num <= num2)
        //    {
        //        int num4;
        //        int num3 = (num + num2) >> 1;
        //        try
        //        {
        //            if (objArray != null)
        //            {
        //                num4 = comparer.Compare(objArray[num3], value);
        //            }
        //            else
        //            {
        //                num4 = comparer.Compare(array.GetValue(num3), value);
        //            }
        //        }
        //        catch
        //        {
        //            throw new InvalidOperationException();
        //        }
        //        if (num4 == 0)
        //        {
        //            return num3;
        //        }
        //        if (num4 < 0)
        //        {
        //            num = num3 + 1;
        //        }
        //        else
        //        {
        //            num2 = num3 - 1;
        //        }
        //    }
        //    return ~num;
        //}

     

        //public static int IndexOf(Array array, object value, int startIndex, int count)
        //{
        //    if (array == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if ((startIndex < 0) || (startIndex > array.cElems))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if ((count < 0) || (count > (array.cElems - startIndex)))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if (array.Rank != 1)
        //    {
        //        throw new RankException();
        //    }
        //    object[] objArray = array as object[];
        //    int num = startIndex + count;
        //    if (objArray != null)
        //    {
        //        if (value == null)
        //        {
        //            for (int i = startIndex; i < num; i++)
        //            {
        //                if (objArray[i] == null)
        //                {
        //                    return i;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int j = startIndex; j < num; j++)
        //            {
        //                object obj2 = objArray[j];
        //                if ((obj2 != null) && value.Equals(obj2))
        //                {
        //                    return j;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (value == null)
        //        {
        //            return -1;
        //        }
        //        for (int k = startIndex; k < num; k++)
        //        {
        //            object obj3 = array.GetValue(k);
        //            if ((obj3 != null) && value.Equals(obj3))
        //            {
        //                return k;
        //            }
        //        }
        //    }
        //    return -1;
        //}

        //public void Initialize()
        //{
        //    EE.Array_Init(this);
        //}

        //public static int LastIndexOf(Array array, object value, int startIndex, int count)
        //{
        //    if (array == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if ((startIndex < 0) || (startIndex >= array.cElems))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if (count < 0)
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if (count > (startIndex + 1))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if (array.Rank != 1)
        //    {
        //        throw new RankException();
        //    }
        //    object[] objArray = array as object[];
        //    int num = (startIndex - count) + 1;
        //    if (objArray != null)
        //    {
        //        if (value == null)
        //        {
        //            for (int i = startIndex; i >= num; i--)
        //            {
        //                if (objArray[i] == null)
        //                {
        //                    return i;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int j = startIndex; j >= num; j--)
        //            {
        //                object obj2 = objArray[j];
        //                if ((obj2 != null) && value.Equals(obj2))
        //                {
        //                    return j;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (value == null)
        //        {
        //            return -1;
        //        }
        //        for (int k = startIndex; k >= num; k--)
        //        {
        //            object obj3 = array.GetValue(k);
        //            if ((obj3 != null) && value.Equals(obj3))
        //            {
        //                return k;
        //            }
        //        }
        //    }
        //    return -1;
        //}

        //public static void Reverse(Array array, int index, int length)
        //{
        //    if (array == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if ((index < 0) || (length < 0))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if ((array.cElems - index) < length)
        //    {
        //        throw new ArgumentException();
        //    }
        //    if (array.Rank != 1)
        //    {
        //        throw new RankException();
        //    }
        //    int num = index;
        //    int num2 = (index + length) - 1;
        //    object[] objArray = array as object[];
        //    if (objArray != null)
        //    {
        //        while (num < num2)
        //        {
        //            object obj2 = objArray[num];
        //            objArray[num] = objArray[num2];
        //            objArray[num2] = obj2;
        //            num++;
        //            num2--;
        //        }
        //    }
        //    else
        //    {
        //        while (num < num2)
        //        {
        //            object obj3 = array.GetValue(num);
        //            array.SetValue(array.GetValue(num2), num);
        //            array.SetValue(obj3, num2);
        //            num++;
        //            num2--;
        //        }
        //    }
        //}

        //public void SetValue(object value, params int[] indices)
        //{
        //    if (indices == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    EE.Array_SetValueEx(this, value, indices, indices.cElems);
        //}

        //public void SetValue(object value, int index)
        //{
        //    EE.Array_SetValue(this, value, index);
        //}

        //public static void Sort(Array array)
        //{
        //    if (array == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    Sort(array, null, 0, array.cElems, null);
        //}

        //public static void Sort(Array array, int index, int length, IComparer comparer)
        //{
        //    Sort(array, null, index, length, comparer);
        //}

        //public static void Sort(Array keys, Array items, int index, int length, IComparer comparer)
        //{
        //    if (keys == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    if ((keys.Rank != 1) || ((items != null) && (items.Rank != 1)))
        //    {
        //        throw new RankException();
        //    }
        //    if ((index < 0) || (length < 0))
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //    if (((keys.cElems - index) < length) || ((items != null) && (index > (items.cElems - length))))
        //    {
        //        throw new ArgumentException();
        //    }
        //    if (length > 1)
        //    {
        //        object[] objArray = keys as object[];
        //        object[] objArray2 = null;
        //        if (objArray != null)
        //        {
        //            objArray2 = items as object[];
        //        }
        //        if ((objArray != null) && ((items == null) || (objArray2 != null)))
        //        {
        //            new SorterObjectArray(objArray, objArray2, comparer).QuickSort(index, (index + length) - 1);
        //        }
        //        else
        //        {
        //            new SorterGenericArray(keys, items, comparer).QuickSort(index, (index + length) - 1);
        //        }
        //    }
        //}

        //int IList.Add(object value)
        //{
        //    throw new NotSupportedException();
        //}

        //void IList.Clear()
        //{
        //    Clear(this, 0, this.cElems);
        //}

        //bool IList.Contains(object obj)
        //{
        //    return (IndexOf(this, obj, 0, this.cElems) >= 0);
        //}

        //int IList.IndexOf(object value)
        //{
        //    return IndexOf(this, value, 0, this.cElems);
        //}

        //void IList.Insert(int index, object value)
        //{
        //    throw new NotSupportedException();
        //}

        //void IList.Remove(object value)
        //{
        //    throw new NotSupportedException();
        //}

        //void IList.RemoveAt(int index)
        //{
        //    throw new NotSupportedException();
        //}

        //public virtual bool IsFixedSize
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //public virtual bool IsReadOnly
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        //public virtual bool IsSynchronized
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        //public int Length
        //{
        //    get
        //    {
        //        return this.cElems;
        //    }
        //}

        //public int Rank
        //{
        //    get
        //    {
        //        return EE.Array_GetRank(this);
        //    }
        //}

        //public virtual object SyncRoot
        //{
        //    get
        //    {
        //        return this;
        //    }
        //}

        //int ICollection.Count
        //{
        //    get
        //    {
        //        return this.cElems;
        //    }
        //}

        //object IList.this[int index]
        //{
        //    get
        //    {
        //        return this.GetValue(index);
        //    }
        //    set
        //    {
        //        this.SetValue(value, index);
        //    }
        //}

        //private class ArrayEnumerator : IEnumerator, ICloneable
        //{
        //    private bool _complete;
        //    private int[] _indices;
        //    private Array array;
        //    private int endIndex;
        //    private int index;
        //    private int startIndex;

        //    internal ArrayEnumerator(Array array, int index, int count)
        //    {
        //        IntPtr ptr;
        //        this.array = array;
        //        this.index = index - 1;
        //        this.startIndex = index;
        //        this.endIndex = index + count;
        //        this._indices = new int[array.Rank];
        //        int num = 1;
        //        for (int i = 0; i < this._indices.Length; i++)
        //        {
        //            num *= array.GetLength(i);
        //        }
        //        this._indices[(int) (ptr = (IntPtr) (this._indices.Length - 1))] = this._indices[(int) ptr] - 1;
        //        this._complete = num == 0;
        //    }

        //    public virtual object Clone()
        //    {
        //        throw new NotImplementedException():
        //    //    return base.MemberwiseClone();
        //    }

        //    private void IncArray()
        //    {
        //        IntPtr ptr;
        //        int rank = this.array.Rank;
        //        this._indices[(int) (ptr = (IntPtr) (rank - 1))] = this._indices[(int) ptr] + 1;
        //        for (int i = rank - 1; i >= 0; i--)
        //        {
        //            if (this._indices[i] > this.array.GetUpperBound(i))
        //            {
        //                if (i == 0)
        //                {
        //                    this._complete = true;
        //                    return;
        //                }
        //                for (int j = i; j < rank; j++)
        //                {
        //                    this._indices[j] = 0;
        //                }
        //                this._indices[(int) (ptr = (IntPtr) (i - 1))] = this._indices[(int) ptr] + 1;
        //            }
        //        }
        //    }

        //    public virtual bool MoveNext()
        //    {
        //        if (this._complete)
        //        {
        //            this.index = this.endIndex;
        //            return false;
        //        }
        //        this.index++;
        //        this.IncArray();
        //        return !this._complete;
        //    }

        //    public virtual void Reset()
        //    {
        //        IntPtr ptr;
        //        this.index = this.startIndex - 1;
        //        int num = 1;
        //        for (int i = 0; i < this.array.Rank; i++)
        //        {
        //            this._indices[i] = 0;
        //            num *= this.array.GetLength(i);
        //        }
        //        this._complete = num == 0;
        //        this._indices[(int) (ptr = (IntPtr) (this._indices.cElems - 1))] = this._indices[(int) ptr] - 1;
        //    }

        //    public virtual object Current
        //    {
        //        get
        //        {
        //            if (this.index < this.startIndex)
        //            {
        //                throw new InvalidOperationException();
        //            }
        //            if (this._complete)
        //            {
        //                throw new InvalidOperationException();
        //            }
        //            return this.array.GetValue(this._indices);
        //        }
        //    }
        //}

       
    }
    internal class SorterGenericArray
    {
        private IComparer comparer;
        private Array items;
        private Array keys;

        public SorterGenericArray(Array keys, Array items, IComparer comparer)
        {
            if (comparer == null)
            {
                comparer = Comparer.Default;
            }
            this.keys = keys;
            this.items = items;
            this.comparer = comparer;
        }

        public virtual void QuickSort(int left, int right)
        {
            do
            {
                int index = left;
                int num2 = right;
                object y = this.keys.GetValue((int)((index + num2) >> 1));
                do
                {
                    try
                    {
                        while (this.comparer.Compare(this.keys.GetValue(index), y) < 0)
                        {
                            index++;
                        }
                        while (this.comparer.Compare(y, this.keys.GetValue(num2)) < 0)
                        {
                            num2--;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException();
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException();
                    }
                    if (index > num2)
                    {
                        break;
                    }
                    if (index < num2)
                    {
                        object obj3 = this.keys.GetValue(index);
                        this.keys.SetValue(this.keys.GetValue(num2), index);
                        this.keys.SetValue(obj3, num2);
                        if (this.items != null)
                        {
                            object obj4 = this.items.GetValue(index);
                            this.items.SetValue(this.items.GetValue(num2), index);
                            this.items.SetValue(obj4, num2);
                        }
                    }
                    index++;
                    num2--;
                }
                while (index <= num2);
                if ((num2 - left) <= (right - index))
                {
                    if (left < num2)
                    {
                        this.QuickSort(left, num2);
                    }
                    left = index;
                }
                else
                {
                    if (index < right)
                    {
                        this.QuickSort(index, right);
                    }
                    right = num2;
                }
            }
            while (left < right);
        }
    }

    internal class SorterObjectArray
    {
        private IComparer comparer;
        private object[] items;
        private object[] keys;

        public SorterObjectArray(object[] keys, object[] items, IComparer comparer)
        {
            if (comparer == null)
            {
                comparer = Comparer.Default;
            }
            this.keys = keys;
            this.items = items;
            this.comparer = comparer;
        }

        public virtual void QuickSort(int left, int right)
        {
            do
            {
                int index = left;
                int num2 = right;
                object y = this.keys[(index + num2) >> 1];
                do
                {
                    try
                    {
                        while (this.comparer.Compare(this.keys[index], y) < 0)
                        {
                            index++;
                        }
                        while (this.comparer.Compare(y, this.keys[num2]) < 0)
                        {
                            num2--;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException();
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException();
                    }
                    if (index > num2)
                    {
                        break;
                    }
                    if (index < num2)
                    {
                        object obj3 = this.keys[index];
                        this.keys[index] = this.keys[num2];
                        this.keys[num2] = obj3;
                        if (this.items != null)
                        {
                            object obj4 = this.items[index];
                            this.items[index] = this.items[num2];
                            this.items[num2] = obj4;
                        }
                    }
                    index++;
                    num2--;
                }
                while (index <= num2);
                if ((num2 - left) <= (right - index))
                {
                    if (left < num2)
                    {
                        this.QuickSort(left, num2);
                    }
                    left = index;
                }
                else
                {
                    if (index < right)
                    {
                        this.QuickSort(index, right);
                    }
                    right = num2;
                }
            }
            while (left < right);
        }
    }
}

