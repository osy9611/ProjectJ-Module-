namespace Module.Core.Systems
{
    public class Args<T1> : System.EventArgs
    {
        public T1 Arg1 { get; set; }

        public Args() { }
        public Args(T1 arg1)
        {
            Arg1 = arg1;
        }
    }

    public class Args<T1, T2> : System.EventArgs
    {
        public T1 Arg1 { get; set; }
        public T2 Arg2 { get; set; }

        public Args() { }
        public Args(T1 arg1, T2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }

    public class Args<T1, T2, T3> : System.EventArgs
    {
        public T1 Arg1 { get; set; }
        public T2 Arg2 { get; set; }
        public T3 Arg3 { get; set; }

        public Args() { }
        public Args(T1 arg1, T2 arg2, T3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
    }

    public class Args<T1, T2, T3, T4> : System.EventArgs
    {
        public T1 Arg1 { get; set; }
        public T2 Arg2 { get; set; }
        public T3 Arg3 { get; set; }
        public T4 Arg4 { get; set; }

        public Args() { }
        public Args(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
    }
}