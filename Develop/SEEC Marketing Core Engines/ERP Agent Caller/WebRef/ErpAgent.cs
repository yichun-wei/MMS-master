﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// 此原始程式碼由 wsdl 版本=4.6.81.0 自動產生。
// 

namespace Seec.Marketing.NetTalk.WebService.Client
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ErpAgentSoap", Namespace = "http://tempuri.org/")]
    public partial class ErpAgent : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback GetCustomersInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetWarehouseInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetInventoryInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetOnHandInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetPriceBookInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetCurrencyBookInfoOperationCompleted;

        private System.Threading.SendOrPostCallback GetDiscountsInfoOperationCompleted;

        private System.Threading.SendOrPostCallback UploadErpOperationCompleted;

        private System.Threading.SendOrPostCallback GetErpOrderInfoOperationCompleted;

        /// <remarks/>
        public ErpAgent()
        {
            this.Url = "http://59.120.19.247/net_talk/ws/erp_agent.asmx";
        }

        /// <remarks/>
        public event GetCustomersInfoCompletedEventHandler GetCustomersInfoCompleted;

        /// <remarks/>
        public event GetWarehouseInfoCompletedEventHandler GetWarehouseInfoCompleted;

        /// <remarks/>
        public event GetInventoryInfoCompletedEventHandler GetInventoryInfoCompleted;

        /// <remarks/>
        public event GetOnHandInfoCompletedEventHandler GetOnHandInfoCompleted;

        /// <remarks/>
        public event GetPriceBookInfoCompletedEventHandler GetPriceBookInfoCompleted;

        /// <remarks/>
        public event GetCurrencyBookInfoCompletedEventHandler GetCurrencyBookInfoCompleted;

        /// <remarks/>
        public event GetDiscountsInfoCompletedEventHandler GetDiscountsInfoCompleted;

        /// <remarks/>
        public event UploadErpCompletedEventHandler UploadErpCompleted;

        /// <remarks/>
        public event GetErpOrderInfoCompletedEventHandler GetErpOrderInfoCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCustomersInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetCustomersInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetCustomersInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetCustomersInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetCustomersInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetCustomersInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetCustomersInfoAsync(string jsonConds, string hashVal)
        {
            this.GetCustomersInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetCustomersInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetCustomersInfoOperationCompleted == null))
            {
                this.GetCustomersInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCustomersInfoOperationCompleted);
            }
            this.InvokeAsync("GetCustomersInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetCustomersInfoOperationCompleted, userState);
        }

        private void OnGetCustomersInfoOperationCompleted(object arg)
        {
            if ((this.GetCustomersInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCustomersInfoCompleted(this, new GetCustomersInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetWarehouseInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetWarehouseInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetWarehouseInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetWarehouseInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetWarehouseInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetWarehouseInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetWarehouseInfoAsync(string jsonConds, string hashVal)
        {
            this.GetWarehouseInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetWarehouseInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetWarehouseInfoOperationCompleted == null))
            {
                this.GetWarehouseInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetWarehouseInfoOperationCompleted);
            }
            this.InvokeAsync("GetWarehouseInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetWarehouseInfoOperationCompleted, userState);
        }

        private void OnGetWarehouseInfoOperationCompleted(object arg)
        {
            if ((this.GetWarehouseInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetWarehouseInfoCompleted(this, new GetWarehouseInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetInventoryInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetInventoryInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetInventoryInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetInventoryInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetInventoryInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetInventoryInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetInventoryInfoAsync(string jsonConds, string hashVal)
        {
            this.GetInventoryInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetInventoryInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetInventoryInfoOperationCompleted == null))
            {
                this.GetInventoryInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetInventoryInfoOperationCompleted);
            }
            this.InvokeAsync("GetInventoryInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetInventoryInfoOperationCompleted, userState);
        }

        private void OnGetInventoryInfoOperationCompleted(object arg)
        {
            if ((this.GetInventoryInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetInventoryInfoCompleted(this, new GetInventoryInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetOnHandInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetOnHandInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetOnHandInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetOnHandInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetOnHandInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetOnHandInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetOnHandInfoAsync(string jsonConds, string hashVal)
        {
            this.GetOnHandInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetOnHandInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetOnHandInfoOperationCompleted == null))
            {
                this.GetOnHandInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetOnHandInfoOperationCompleted);
            }
            this.InvokeAsync("GetOnHandInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetOnHandInfoOperationCompleted, userState);
        }

        private void OnGetOnHandInfoOperationCompleted(object arg)
        {
            if ((this.GetOnHandInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetOnHandInfoCompleted(this, new GetOnHandInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetPriceBookInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetPriceBookInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetPriceBookInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetPriceBookInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetPriceBookInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetPriceBookInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetPriceBookInfoAsync(string jsonConds, string hashVal)
        {
            this.GetPriceBookInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetPriceBookInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetPriceBookInfoOperationCompleted == null))
            {
                this.GetPriceBookInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPriceBookInfoOperationCompleted);
            }
            this.InvokeAsync("GetPriceBookInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetPriceBookInfoOperationCompleted, userState);
        }

        private void OnGetPriceBookInfoOperationCompleted(object arg)
        {
            if ((this.GetPriceBookInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPriceBookInfoCompleted(this, new GetPriceBookInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCurrencyBookInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetCurrencyBookInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetCurrencyBookInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetCurrencyBookInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetCurrencyBookInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetCurrencyBookInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetCurrencyBookInfoAsync(string jsonConds, string hashVal)
        {
            this.GetCurrencyBookInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetCurrencyBookInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetCurrencyBookInfoOperationCompleted == null))
            {
                this.GetCurrencyBookInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCurrencyBookInfoOperationCompleted);
            }
            this.InvokeAsync("GetCurrencyBookInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetCurrencyBookInfoOperationCompleted, userState);
        }

        private void OnGetCurrencyBookInfoOperationCompleted(object arg)
        {
            if ((this.GetCurrencyBookInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCurrencyBookInfoCompleted(this, new GetCurrencyBookInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDiscountsInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetDiscountsInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetDiscountsInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetDiscountsInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetDiscountsInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetDiscountsInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetDiscountsInfoAsync(string jsonConds, string hashVal)
        {
            this.GetDiscountsInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetDiscountsInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetDiscountsInfoOperationCompleted == null))
            {
                this.GetDiscountsInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDiscountsInfoOperationCompleted);
            }
            this.InvokeAsync("GetDiscountsInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetDiscountsInfoOperationCompleted, userState);
        }

        private void OnGetDiscountsInfoOperationCompleted(object arg)
        {
            if ((this.GetDiscountsInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDiscountsInfoCompleted(this, new GetDiscountsInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UploadErp", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string UploadErp(string jsonInput, string hashVal)
        {
            object[] results = this.Invoke("UploadErp", new object[] {
                    jsonInput,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginUploadErp(string jsonInput, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UploadErp", new object[] {
                    jsonInput,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndUploadErp(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void UploadErpAsync(string jsonInput, string hashVal)
        {
            this.UploadErpAsync(jsonInput, hashVal, null);
        }

        /// <remarks/>
        public void UploadErpAsync(string jsonInput, string hashVal, object userState)
        {
            if ((this.UploadErpOperationCompleted == null))
            {
                this.UploadErpOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUploadErpOperationCompleted);
            }
            this.InvokeAsync("UploadErp", new object[] {
                    jsonInput,
                    hashVal}, this.UploadErpOperationCompleted, userState);
        }

        private void OnUploadErpOperationCompleted(object arg)
        {
            if ((this.UploadErpCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UploadErpCompleted(this, new UploadErpCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetErpOrderInfo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetErpOrderInfo(string jsonConds, string hashVal)
        {
            object[] results = this.Invoke("GetErpOrderInfo", new object[] {
                    jsonConds,
                    hashVal});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetErpOrderInfo(string jsonConds, string hashVal, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetErpOrderInfo", new object[] {
                    jsonConds,
                    hashVal}, callback, asyncState);
        }

        /// <remarks/>
        public string EndGetErpOrderInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void GetErpOrderInfoAsync(string jsonConds, string hashVal)
        {
            this.GetErpOrderInfoAsync(jsonConds, hashVal, null);
        }

        /// <remarks/>
        public void GetErpOrderInfoAsync(string jsonConds, string hashVal, object userState)
        {
            if ((this.GetErpOrderInfoOperationCompleted == null))
            {
                this.GetErpOrderInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetErpOrderInfoOperationCompleted);
            }
            this.InvokeAsync("GetErpOrderInfo", new object[] {
                    jsonConds,
                    hashVal}, this.GetErpOrderInfoOperationCompleted, userState);
        }

        private void OnGetErpOrderInfoOperationCompleted(object arg)
        {
            if ((this.GetErpOrderInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetErpOrderInfoCompleted(this, new GetErpOrderInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetCustomersInfoCompletedEventHandler(object sender, GetCustomersInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCustomersInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetCustomersInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetWarehouseInfoCompletedEventHandler(object sender, GetWarehouseInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetWarehouseInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetWarehouseInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetInventoryInfoCompletedEventHandler(object sender, GetInventoryInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetInventoryInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetInventoryInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetOnHandInfoCompletedEventHandler(object sender, GetOnHandInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetOnHandInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetOnHandInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetPriceBookInfoCompletedEventHandler(object sender, GetPriceBookInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPriceBookInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetPriceBookInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetCurrencyBookInfoCompletedEventHandler(object sender, GetCurrencyBookInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCurrencyBookInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetCurrencyBookInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetDiscountsInfoCompletedEventHandler(object sender, GetDiscountsInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDiscountsInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetDiscountsInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void UploadErpCompletedEventHandler(object sender, UploadErpCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UploadErpCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal UploadErpCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    public delegate void GetErpOrderInfoCompletedEventHandler(object sender, GetErpOrderInfoCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetErpOrderInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetErpOrderInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}
