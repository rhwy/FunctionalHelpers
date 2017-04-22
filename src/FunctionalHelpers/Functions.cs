using System;
using System.Collections.Generic;

namespace FunctionalHelpers
{
    
    public static class Core
    {
        public static Func<T> f<T>(Func<T> newFunction) => newFunction;
        public static Func<T1,T2> f<T1,T2>(Func<T1,T2> newFunction) => newFunction;
        public static Func<T1,T2,T3> f<T1,T2,T3>(Func<T1,T2,T3> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4> f<T1,T2,T3,T4>(Func<T1,T2,T3,T4> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5> f<T1,T2,T3,T4,T5>(Func<T1,T2,T3,T4,T5> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5,T6> f<T1,T2,T3,T4,T5,T6>(Func<T1,T2,T3,T4,T5,T6> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5,T6,T7> f<T1,T2,T3,T4,T5,T6,T7>(Func<T1,T2,T3,T4,T5,T6,T7> newFunction) => newFunction;



    
        public static U Then<T,U>(this T me, Func<T,U> next) => next(me);
        public static U Then<T,U>(this IEnumerable<T> me, Func<IEnumerable<T>,U> next) => next(me);
    }


    public interface Option<T>
    {
        bool HasValue {get;}
    }

    public static class OptionExtensions
    {
        public static None<T> None<T>(this T currentObject) 
            => new None<T>();

        public static T WithValue<T>(this Option<T> currentOption,
            Func<T> none, Func<T,T> some)
        {
            switch (currentOption)
            {
                case Some<T> someValue:
                    return some(someValue.Value);
                default:
                    return none();
            }
        }
    }
    public struct None<T> : Option<T>
    {
        public bool HasValue => false;
        public static None<T> New => new None<T>();
    }

    public struct Some<T> : Option<T>
    {
        public T Value {get;}

        public bool HasValue => true;
        private Some(T value) => 
            Value = EqualityComparer<T>.Default.Equals(value, default(T))
            ? throw new InvalidOperationException("value can't be null")
            : value ;

        public static Some<T> NewValue(T value) => new Some<T>(value);
    }
}
