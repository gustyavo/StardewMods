namespace Entoarox.Framework.Advanced
{
    class Helper
    {
        public static int CreateHashCode(params object[] objects)
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            int i = 0;
            foreach (object obj in objects)
            {
                int hashCode = obj.GetHashCode();
                if (i % 2 == 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                else
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;
                ++i;
            }
            return hash1 + (hash2 * 1566083941);
        }
    }
    public struct PolyTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public PolyTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = this.Item1;
            item2 = this.Item2;
        }
        public override bool Equals(object obj)
        {
            return obj is PolyTuple<T1, T2> tpl && this.Equals(tpl);
        }
        public bool Equals(PolyTuple<T1,T2> obj)
        {
            return this.Item1.Equals(obj.Item1) && this.Item2.Equals(obj.Item2);
        }
        public override int GetHashCode()
        {
            return Helper.CreateHashCode(this.Item1, this.Item2);
        }
    }
    public struct PolyTuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public PolyTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
        public void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
        {
            item1 = this.Item1;
            item2 = this.Item2;
            item3 = this.Item3;
        }
        public override bool Equals(object obj)
        {
            return obj is PolyTuple<T1, T2, T3> tpl && this.Equals(tpl);
        }
        public bool Equals(PolyTuple<T1, T2, T3> obj)
        {
            return this.Item1.Equals(obj.Item1) && this.Item2.Equals(obj.Item2) && this.Item3.Equals(obj.Item3);
        }
        public override int GetHashCode()
        {
            return Helper.CreateHashCode(this.Item1, this.Item2, this.Item3);
        }
    }
    public struct PolyTuple<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public PolyTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }
        public void Deconstruct(out T1 item1, out T2 item2, out T3 item3, out T4 item4)
        {
            item1 = this.Item1;
            item2 = this.Item2;
            item3 = this.Item3;
            item4 = this.Item4;
        }
        public override bool Equals(object obj)
        {
            return obj is PolyTuple<T1, T2, T3, T4> tpl && this.Equals(tpl);
        }
        public bool Equals(PolyTuple<T1, T2, T3, T4> obj)
        {
            return this.Item1.Equals(obj.Item1) && this.Item2.Equals(obj.Item2) && this.Item3.Equals(obj.Item3) && this.Item4.Equals(obj.Item4);
        }
        public override int GetHashCode()
        {
            return Helper.CreateHashCode(this.Item1, this.Item2, this.Item3, this.Item4);
        }
    }
    public struct PolyTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public PolyTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }
        public void Deconstruct(out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
        {
            item1 = this.Item1;
            item2 = this.Item2;
            item3 = this.Item3;
            item4 = this.Item4;
            item5 = this.Item5;
        }
        public override bool Equals(object obj)
        {
            return obj is PolyTuple<T1, T2, T3, T4, T5> tpl && this.Equals(tpl);
        }
        public bool Equals(PolyTuple<T1, T2, T3, T4, T5> obj)
        {
            return this.Item1.Equals(obj.Item1) && this.Item2.Equals(obj.Item2) && this.Item3.Equals(obj.Item3) && this.Item4.Equals(obj.Item4) && this.Item5.Equals(obj.Item5);
        }
        public override int GetHashCode()
        {
            return Helper.CreateHashCode(this.Item1, this.Item2, this.Item3, this.Item4, this.Item5);
        }
    }
}
