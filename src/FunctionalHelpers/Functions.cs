using System;
using System.Collections.Generic;

namespace FunctionalHelpers
{
    using static Core;
    public static class Core
    {
        //Use f helper for inference type, it allows to write:
        //var inc = f( (int i) => i++)
        //instead of 
        //Func<int,int> inc = i => i++
        public static Func<T> f<T>(Func<T> newFunction) => newFunction;
        public static Func<T1,T2> f<T1,T2>(Func<T1,T2> newFunction) => newFunction;
        public static Func<T1,T2,T3> f<T1,T2,T3>(Func<T1,T2,T3> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4> f<T1,T2,T3,T4>(Func<T1,T2,T3,T4> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5> f<T1,T2,T3,T4,T5>(Func<T1,T2,T3,T4,T5> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5,T6> f<T1,T2,T3,T4,T5,T6>(Func<T1,T2,T3,T4,T5,T6> newFunction) => newFunction;
        public static Func<T1,T2,T3,T4,T5,T6,T7> f<T1,T2,T3,T4,T5,T6,T7>(Func<T1,T2,T3,T4,T5,T6,T7> newFunction) => newFunction;



        //simple continuation, as you can't pipe things in c#, it's usefull to continue
        //like this with any objects.ie, instead of
        //Func<int,int> inc = i => i++;
        //Func<int,string> write = i => $"value is {i}";
        //var start = 1;
        //var value = inc(start);
        //value = inc(value);
        //var message = write(next);
        //you can write :
        //var message = 1
        //      .then(inc)
        //      .then(inc)
        //      .then(write)
        public static U Then<T,U>(this T me, Func<T,U> next) => next(me);
        public static U Then<T,U>(this IEnumerable<T> me, Func<IEnumerable<T>,U> next) => next(me);



        ///<summary>
        ///Helpers to verify that parameters are as expected
        ///</summary>
        public static class Ensure
        {
            public static T ValueNotNull<T>(T value)
            {
                return EqualityComparer<T>.Default.Equals(value, default(T))
                    ? Throw<T>(Doc.En.ValueCantBeNull)
                    : value;
            }
        }
        
        ///<summary>
        ///simplified throw of our internal exception type
        ///</summary>
        public static T Throw<T>(string message, params object[] args)
        => throw new FunctionalHelpersException(message,args);

        ///<summary>
        ///create a successful result from value
        ///</summary>
        public static Result<T> Success<T>(T value) => new Success<T>(value);

        ///<summary>
        ///create a successful result from value
        ///</summary>
        public static Result<T> Failure<T>(string message) => new Failure<T>(message);

    }
    public class FunctionalHelpersException : Exception {
        public FunctionalHelpersException(string message, params object[] args) 
            : base(string.Format(message,args)){} 
    }
    internal struct Doc
    {
        internal struct En {
            internal static string ValueCantBeNull => "Value Can't be null";
            internal static string UnhandledException(Exception source) => $"Unhandled exception : {source.Message}";
        }
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
            Value = Ensure.ValueNotNull(value);

        public static Some<T> NewValue(T value) => new Some<T>(value);
    }

    public abstract class Result {
        public static Result operator >= (Result current, Func<Result, Result> next)
        {
            return next(current);
        }

        public static Result operator <= (Result current,Func<Result, Result> next)
        {
            return next(current);
        }
    }
    public abstract class Result<T> : Result
    {
        bool IsSuccess {get;}

        public static Result<T> operator >= ( Result<T> current, NextOperationResult<T> next)
        {
            return next(current);
        }

        public static Result<T> operator <= ( Result<T> current,NextOperationResult<T> next)
        {
            return next(current);
        }

        public static Result<T> operator >= ( Result<T> current, Func<T,Result<T>> next)
        {
            return current.Then(next);
        }

        public static Result<T> operator <= ( Result<T> current,Func<T,Result<T>> next)
        {
            return current.Then(next);
        }
        public static object operator >= ( 
            Result<T> current, 
            (Func<T,object> success, Func<FunctionalHelpersException,object> failure) next)
        {
            return current.Final(next.success,next.failure);
        }

        public static object operator <= ( 
            Result<T> current, 
            (Func<T,object> success, Func<FunctionalHelpersException,object> failure) next)
        {
            return current.Final(next.success,next.failure);
        }
    }

    public delegate Result<T> NextOperationResult<T>(Result<T> next);
    public static class ResultExtensions
    {
        ///<summary>
        ///take a result, transform it with success or failure functions and return a new result
        ///from the functions passed to extract new values
        ///</summary>
        public static Result<U> Then<T,U>(this Result<T> me, 
            Func<T,U> success, 
            Func<FunctionalHelpersException,U> failure)
        {
            switch(me)
            {
                case Success<T> successResult:
                    return Success(success(successResult.Value));
                case Failure<T> failureResult:
                    return Success(failure(failureResult.Exception));
                default:
                    return Throw<Result<U>>("result has not the expected type");
            }
        }


        ///<summary>
        ///take a result, transform it with success or failure functions and return a new result
        ///from the functions passed to extract new values
        ///</summary>
        public static Result<U> Then<T,U>(this Result<T> me, 
            Func<T,Result<U>> success, 
            Func<FunctionalHelpersException,Result<U>> failure)
        {
            switch(me)
            {
                case Success<T> successResult:
                    return success(successResult.Value);
                case Failure<T> failureResult:
                    return failure(failureResult.Exception);
                default:
                    return Throw<Result<U>>("result has not the expected type");
            }
        }

        ///<summary>
        ///take a result, transform it with success or failure functions and return a new result
        ///from the functions passed to extract new values
        ///</summary>
        public static Result<U> Then<T,U>(this Result<T> me, 
            Func<T,Result<U>> success)
        {
            switch(me)
            {
                case Success<T> successResult:
                    try
                    {
                        return success(successResult.Value);
                    }
                    catch (System.Exception e)
                    {
                        return Failure<U>(e.Message);
                    }
                    
                case Failure<T> failureResult:
                    return failureResult.FromPrevious<U>();
                default:
                    return Throw<Result<U>>("result has not the expected type");
            }
        }
        public static Result<U> Then<T,U>(this Result me, 
            Func<T,Result<U>> success)
        {
            switch(me)
            {
                case Success<T> successResult:
                    return successResult.Then(success);
                case Failure<T> failureResult:
                    return failureResult.FromPrevious<U>();
                default:
                    return Throw<Result<U>>("result has not the expected type");
            }
        }

        public static U Final<T,U>(this Result<T> me, 
            Func<T,U> success, 
            Func<FunctionalHelpersException,U> failure)
        {
            switch(me)
            {
                case Success<T> successResult:
                    return success(successResult.Value);
                case Failure<T> failureResult:
                    return failure(failureResult.Exception);
                default:
                    return Throw<U>("result has not the expected type");
            }
        }
    }
    public class Success<T> : Result<T>
    {
        public bool IsSuccess => true;
        
        public T Value;
        public Success(T value)
        {
            Value = Ensure.ValueNotNull(value);
        }

        
    }
    
    public class Failure<T> : Result<T>
    {
        public bool IsSuccess => false;
        public FunctionalHelpersException Exception;
        public Failure(string exceptionMessage, params object[] args)
        {
            Exception = new FunctionalHelpersException(exceptionMessage, args);
        }
        public Failure(FunctionalHelpersException exception) => Exception = exception;
        public Failure<U> FromPrevious<U>() => new Failure<U>(Exception);

    }
    
}
