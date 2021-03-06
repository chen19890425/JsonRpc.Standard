﻿using System;
using System.Reflection;

namespace JsonRpc.Standard.Contracts
{
    /// <summary>
    /// Declares a JSON RPC method scope. Defines some common traits of the methods in the scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class JsonRpcScopeAttribute : Attribute
    {
        private Type _NamingStrategyType;
        private Type _ValueConverterType;

        /// <summary>
        /// The prefix that is prepended to all the names of the JSON RPC methods in the scope.
        /// </summary>
        public string MethodPrefix { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="JsonRpcNamingStrategy"/> applied to the methods in this scope.
        /// </summary>
        public Type NamingStrategyType
        {
            get { return _NamingStrategyType; }
            set
            {
                if (value != null && !typeof(JsonRpcNamingStrategy).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                    throw new ArgumentException("Invalid JsonRpcNamingStrategy type.", nameof(value));
                _NamingStrategyType = value;
            }
        }

        /// <summary>
        /// The constructor parameters used to instantiate the specified <see cref="NamingStrategyType"/>
        /// </summary>
        public object[] NamingStrategyParameters { get; set; }

        internal JsonRpcNamingStrategy GetNamingStrategy()
        {
            return JsonRpcParameterAttribute.GetNamingStrategy(NamingStrategyType, NamingStrategyParameters);
        }

        /// <summary>
        /// The <see cref="Type"/> of <see cref="IJsonValueConverter"/> that applies to the parameters of
        /// all the JSON RPC methods in the scope.
        /// </summary>
        public Type ValueConverterType
        {
            get { return _ValueConverterType; }
            set
            {
                if (value != null && !typeof(IJsonValueConverter).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                    throw new ArgumentException("Invalid IJsonValueConverter type.", nameof(value));
                _ValueConverterType = value;
            }
        }

        /// <summary>
        /// The constructor parameters used to instantiate the specified <see cref="ValueConverterType"/>.
        /// </summary>
        public object[] ValueConverterParameters { get; set; }

        internal IJsonValueConverter GetValueConverter()
        {
            return JsonRpcParameterAttribute.GetValueConverter(ValueConverterType, ValueConverterParameters);
        }
    }

    /// <summary>
    /// Indicates the method is exposed for JSON RPC invocation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class JsonRpcMethodAttribute : Attribute
    {
        private Type _NamingStrategyType;
        private Type _ValueConverterType;

        /// <summary>
        /// Creates a default attribute instance.
        /// </summary>
        public JsonRpcMethodAttribute() : this(null)
        {
        }

        /// <summary>
        /// Creates an attribute instance.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        public JsonRpcMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// The name of the method. <c>null</c> to use the applied method name.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Indicates whether the method is a notification request.
        /// </summary>
        /// <remarks>
        /// This property is currently used in the client stub, and it's highly recommended that
        /// this property be set for the server-side JSON RPC methods.
        /// </remarks>
        public bool IsNotification { get; set; }
        
        /// <summary>
        /// Used in the server. Whether allows extra parameters on this method when matching signature.
        /// </summary>
        public bool AllowExtensionData { get; set; }

        ///// <summary>
        ///// Whether the method is cancellable.
        ///// </summary>
        ///// <value>
        ///// <c>true</c> if the method can be cancelled via a <see cref="CancellationToken"/>.
        ///// <c>false</c> otherwise.
        ///// <c>null</c> to infer from the parameters. (e.g. there is an argument that accepts <see cref="CancellationToken"/>.)
        ///// </value>
        //public bool? Cancellable { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="JsonRpcNamingStrategy"/> applied to the methods in this scope.
        /// </summary>
        public Type NamingStrategyType
        {
            get { return _NamingStrategyType; }
            set
            {
                if (value != null && !typeof(JsonRpcNamingStrategy).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                    throw new ArgumentException("Invalid JsonRpcNamingStrategy type.", nameof(value));
                _NamingStrategyType = value;
            }
        }

        /// <summary>
        /// The constructor parameters used to instantiate the specified <see cref="NamingStrategyType"/>
        /// </summary>
        public object[] NamingStrategyParameters { get; set; }

        internal JsonRpcNamingStrategy GetNamingStrategy()
        {
            return JsonRpcParameterAttribute.GetNamingStrategy(NamingStrategyType, NamingStrategyParameters);
        }

        /// <summary>
        /// The <see cref="Type"/> of <see cref="IJsonValueConverter"/> that applies to the parameters of this method.
        /// </summary>
        public Type ValueConverterType
        {
            get { return _ValueConverterType; }
            set
            {
                if (value != null && !typeof(IJsonValueConverter).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                    throw new ArgumentException("Invalid IJsonValueConverter type.", nameof(value));
                _ValueConverterType = value;
            }
        }

        /// <summary>
        /// The constructor parameters used to instantiate the specified <see cref="ValueConverterType"/>
        /// </summary>
        public object[] ValueConverterParameters { get; set; }

        internal IJsonValueConverter GetValueConverter()
        {
            return JsonRpcParameterAttribute.GetValueConverter(ValueConverterType, ValueConverterParameters);
        }
    }

    /// <summary>
    /// Specifies the parameter options of a JSON RPC method.
    /// </summary>
    /// <remarks>
    /// You can apply this attribute to the return value of a method. In C#, you may specify it as follows
    /// <code>
    /// [return: JsonRpcParameter]
    /// // Your function signature here…
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = false)]
    public sealed class JsonRpcParameterAttribute : Attribute
    {
        private Type _ValueConverterType;
        private object _DefaultValue;

        /// <summary>
        /// Creates a default attribute instance.
        /// </summary>
        public JsonRpcParameterAttribute() : this(null)
        {
        }

        /// <summary>
        /// Creates an attribute instance.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        public JsonRpcParameterAttribute(string parameterName)
        {
            ParameterName = parameterName;
            IsOptional = null;
        }

        /// <summary>
        /// The name of the parameter. <c>null</c> to use the applied Parameter name.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Whether the parameter is optional.
        /// </summary>
        /// <value>
        /// If the value is <c>null</c>, the default behavior (whether CLR parameter is optional or not)
        /// will be used.
        /// </value>
        public bool? IsOptional { get; set; }

        /// <summary>
        /// The default value for the optional parameter.
        /// </summary>
        /// <remarks>
        /// This value is in effect only if <see cref="IsOptional"/> is <c>true</c>.
        /// Setting this value will automatically set <see cref="IsOptional"/> to <c>true</c>.
        /// If you are setting both <see cref="IsOptional"/> and this property, the resulting
        /// behavior is undefined.
        /// </remarks>
        public object DefaultValue
        {
            get { return _DefaultValue; }
            set
            {
                _DefaultValue = value;
                IsOptional = true;
            }
        }

        /// <summary>
        /// The <see cref="Type"/> of <see cref="IJsonValueConverter"/> that applies to this parameter.
        /// </summary>
        public Type ValueConverterType
        {
            get { return _ValueConverterType; }
            set
            {
                if (value != null && !typeof(IJsonValueConverter).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                    throw new ArgumentException("Invalid IJsonValueConverter type.", nameof(value));
                _ValueConverterType = value;
            }
        }

        /// <summary>
        /// The constructor parameters used to instantiate the specified <see cref="ValueConverterType"/>
        /// </summary>
        public object[] ValueConverterParameters { get; set; }

        internal IJsonValueConverter GetValueConverter()
        {
            return GetValueConverter(ValueConverterType, ValueConverterParameters);
        }

        internal static IJsonValueConverter GetValueConverter(Type type, object[] parameters)
        {
            if (type == null) return null;
            if (type == typeof(JsonValueConverter)) return JsonValueConverter.Default;
            if (type == typeof(CamelCaseJsonValueConverter)) return CamelCaseJsonValueConverter.CamelCaseDefault;
            return (IJsonValueConverter)Activator.CreateInstance(type, parameters);
        }

        internal static JsonRpcNamingStrategy GetNamingStrategy(Type type, object[] parameters)
        {
            if (type == null) return null;
            if (type == typeof(JsonRpcNamingStrategy)) return JsonRpcNamingStrategy.Default;
            if (type == typeof(CamelCaseJsonRpcNamingStrategy)) return CamelCaseJsonRpcNamingStrategy.CamelCaseDefault;
            return (JsonRpcNamingStrategy) Activator.CreateInstance(type, parameters);
        }
    }
}